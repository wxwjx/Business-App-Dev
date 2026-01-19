using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Stripe;
using Stripe.Checkout;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public partial class OrderSuccess : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";
        private const string CART_SELECTED_KEY = "CART_SELECTED";
        private const string CART_SELECTED_INIT_KEY = "CART_SELECTED_INIT";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            try
            {
                ApplyPageTranslations(); // NEW

                string sessionId = Request.QueryString["session_id"];
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    ShowError(T("Missing session_id from Stripe redirect."));
                    return;
                }

                if (!VerifyStripePaid(sessionId, out string failMsg))
                {
                    ShowError(T(failMsg));
                    return;
                }

                lblOrderId.Text = "ORD-" + ShortId(sessionId);

                var items = Session["PENDING_ORDER_" + sessionId] as List<PurchasedItem>;
                if (items == null || items.Count == 0)
                {
                    ShowError(T("Order data not found (Session expired). Try paying again, or implement Orders table to persist."));
                    return;
                }

                pnlError.Visible = false;

                decimal total;
                if (Session["PENDING_ORDER_TOTAL_" + sessionId] is decimal t)
                    total = t;
                else
                    total = items.Sum(x => x.LineTotal);

                lblTotal.Text = total.ToString("0.00");
                lblPayStatus.Text = T("PAID");

                // OPTIONAL: translate purchased item names for the receipt
                items = TranslatePurchasedItemsIfNeeded(items);

                var groups = BuildSellerGroups(items);
                if (groups.Count == 0)
                {
                    ShowError(T("Could not determine pickup locations. Ensure Products.SellerID is filled and Seller table has data."));
                    return;
                }

                int userId = GetUserIdOrThrow();
                int orderId = SaveOrderIfNotExists(sessionId, userId, total, items);

                RemovePurchasedItemsFromCart(items);

                rptSellerGroups.DataSource = groups;
                rptSellerGroups.DataBind();

                // After binding, translate the repeated UI labels inside the repeaters
                ApplyRepeaterTranslations(rptSellerGroups);

                Session.Remove("PENDING_ORDER_" + sessionId);
                Session.Remove("PENDING_ORDER_TOTAL_" + sessionId);
            }
            catch (StripeException)
            {
                ShowError(T("Payment verification failed due to a payment service error. Please try again."));
            }
            catch (SqlException)
            {
                ShowError(T("Database error while saving/loading your order. Please try again later."));
            }
            catch (Exception ex)
            {
                ShowError(T(ex.Message));
            }
        }

        // =========================
        // TRANSLATION HELPERS
        // =========================
        private string GetLang()
        {
            return (Session["LANG"] as string) ?? "en";
        }

        private string T(string text)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return text ?? "";

            text = text ?? "";
            if (string.IsNullOrWhiteSpace(text)) return text;

            string key = $"tr:en->{lang}:{text}";
            return TranslationCache.GetOrAdd(key, () =>
                TranslationService.Translate(text, lang, "en"), hours: 24);
        }

        private void ApplyPageTranslations()
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            lblBackToShopping.Text = T(lblBackToShopping.Text);
            lblOrderSuccessful.Text = T(lblOrderSuccessful.Text);
            lblOrderIdText.Text = T(lblOrderIdText.Text);
            lblStatusText.Text = T(lblStatusText.Text);

            lblStepConfirmed.Text = T(lblStepConfirmed.Text);
            lblStepPreparing.Text = T(lblStepPreparing.Text);
            lblStepReady.Text = T(lblStepReady.Text);

            lblTotalText.Text = T(lblTotalText.Text);
        }

        private void ApplyRepeaterTranslations(Repeater rpt)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            foreach (RepeaterItem it in rpt.Items)
            {
                // seller header labels
                var pickupLoc = it.FindControl("lblPickupLocationText") as Label;
                if (pickupLoc != null) pickupLoc.Text = T(pickupLoc.Text);

                var pickupWin = it.FindControl("lblPickupWindowText") as Label;
                if (pickupWin != null) pickupWin.Text = T(pickupWin.Text);

                var sellerStatus = it.FindControl("lblSellerStatusText") as Label;
                if (sellerStatus != null) sellerStatus.Text = T(sellerStatus.Text);

                // directions button
                var lnk = it.FindControl("lnkDirectionsSeller") as HyperLink;
                if (lnk != null) lnk.Text = T(lnk.Text);

                // seller subtotal label
                var sellerSubtotal = it.FindControl("lblSellerSubtotalText") as Label;
                if (sellerSubtotal != null) sellerSubtotal.Text = T(sellerSubtotal.Text);

                // translate "Qty:" inside nested repeater items
                var inner = it.FindControl("rptItemsBySeller") as Repeater;
                if (inner != null)
                {
                    foreach (RepeaterItem row in inner.Items)
                    {
                        var qty = row.FindControl("lblQtyText") as Label;
                        if (qty != null) qty.Text = T(qty.Text);
                    }
                }
            }
        }

        private List<PurchasedItem> TranslatePurchasedItemsIfNeeded(List<PurchasedItem> items)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return items;
            if (items == null) return items;

            foreach (var it in items)
            {
                if (it == null) continue;
                if (!string.IsNullOrWhiteSpace(it.ProductName))
                    it.ProductName = T(it.ProductName);
            }

            return items;
        }

        /* =========================
         * USER / PROFILE LINK
         * ========================= */
        private int GetUserIdOrThrow()
        {
            if (Session["UserID"] == null)
                throw new Exception(T("Your session has expired. Please log in again."));

            if (!int.TryParse(Session["UserID"].ToString(), out int userId) || userId <= 0)
                throw new Exception(T("Invalid user session. Please log in again."));

            return userId;
        }

        /* =========================
         * SAVE ORDER (Orders + OrderItems)
         * ========================= */
        private int SaveOrderIfNotExists(string stripeSessionId, int userId, decimal total, List<PurchasedItem> items)
        {
            int existing = GetOrderIdByStripeSession(stripeSessionId);
            if (existing > 0)
                return existing;

            var productIds = items.Select(i => i.ProductID).Distinct().ToList();
            var productSellerMap = LoadSellerInfoForProducts(productIds);

            using (SqlConnection con = new SqlConnection(ConnStr()))
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    string insertOrderSql = @"
INSERT INTO dbo.Orders (UserID, StripeSessionId, TotalAmount, PayStatus)
OUTPUT INSERTED.OrderID
VALUES (@UserID, @StripeSessionId, @TotalAmount, @PayStatus);";

                    int orderId;
                    using (SqlCommand cmd = new SqlCommand(insertOrderSql, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@StripeSessionId", stripeSessionId);
                        cmd.Parameters.AddWithValue("@TotalAmount", total);
                        cmd.Parameters.AddWithValue("@PayStatus", "PAID");
                        orderId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string insertItemSql = @"
INSERT INTO dbo.OrderItems
(OrderID, ProductID, ProductName, Quantity, UnitPrice, LineTotal, SellerID)
VALUES
(@OrderID, @ProductID, @ProductName, @Quantity, @UnitPrice, @LineTotal, @SellerID);";

                    foreach (var it in items)
                    {
                        int? sellerId = null;
                        if (productSellerMap.TryGetValue(it.ProductID, out var sellerInfo))
                            sellerId = sellerInfo.SellerID;

                        using (SqlCommand cmd = new SqlCommand(insertItemSql, con, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@ProductID", it.ProductID);
                            cmd.Parameters.AddWithValue("@ProductName", it.ProductName ?? "");
                            cmd.Parameters.AddWithValue("@Quantity", it.Quantity);
                            cmd.Parameters.AddWithValue("@UnitPrice", it.UnitPrice);
                            cmd.Parameters.AddWithValue("@LineTotal", it.LineTotal);

                            if (sellerId.HasValue) cmd.Parameters.AddWithValue("@SellerID", sellerId.Value);
                            else cmd.Parameters.AddWithValue("@SellerID", DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return orderId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        private int GetOrderIdByStripeSession(string stripeSessionId)
        {
            string sql = "SELECT TOP 1 OrderID FROM dbo.Orders WHERE StripeSessionId = @sid;";
            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@sid", stripeSessionId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
        }

        /* =========================
         * STRIPE VERIFY
         * ========================= */
        private bool VerifyStripePaid(string sessionId, out string error)
        {
            error = "";

            var key = ConfigurationManager.AppSettings["StripeSecretKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                error = "Stripe is not configured (StripeSecretKey missing in Web.config).";
                return false;
            }

            StripeConfiguration.ApiKey = key.Trim();

            try
            {
                var service = new SessionService();
                var s = service.Get(sessionId);

                bool paid =
                    string.Equals(s.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(s.Status, "complete", StringComparison.OrdinalIgnoreCase);

                if (!paid)
                {
                    error = $"Payment not completed. Status: {s.PaymentStatus}";
                    return false;
                }

                return true;
            }
            catch (StripeException)
            {
                error = "Failed to verify payment with Stripe. Please try again.";
                return false;
            }
            catch (Exception)
            {
                error = "Failed to verify payment due to an unexpected error.";
                return false;
            }
        }

        /* =========================
         * CLEAR CART (PURCHASED ITEMS)
         * ========================= */
        private void RemovePurchasedItemsFromCart(List<PurchasedItem> purchasedItems)
        {
            if (purchasedItems == null || purchasedItems.Count == 0) return;

            var cart = Session[CART_KEY] as List<CartItem> ?? new List<CartItem>();
            var purchasedIds = purchasedItems.Select(x => x.ProductID).ToHashSet();

            cart.RemoveAll(ci => purchasedIds.Contains(ci.ProductID));

            Session[CART_KEY] = cart;
            Session[CART_SELECTED_KEY] = new HashSet<int>();
            Session[CART_SELECTED_INIT_KEY] = false;
        }

        private string ConnStr()
        {
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
        }

        private List<SellerGroupVM> BuildSellerGroups(List<PurchasedItem> items)
        {
            var productIds = items.Select(i => i.ProductID).Distinct().ToList();
            var productSellerMap = LoadSellerInfoForProducts(productIds);

            var usableItems = items.Where(i => productSellerMap.ContainsKey(i.ProductID)).ToList();

            var groups = usableItems
                .GroupBy(i => productSellerMap[i.ProductID].SellerID)
                .Select(g =>
                {
                    var first = productSellerMap[g.First().ProductID];

                    return new SellerGroupVM
                    {
                        SellerID = first.SellerID,
                        SellerName = first.ShopName ?? "-",
                        Address = first.Address ?? "-",
                        PickupWindow = string.IsNullOrWhiteSpace(first.PickupWindow) ? "(Not specified)" : first.PickupWindow,
                        SellerStatus = "Preparing",
                        Latitude = first.Latitude,
                        Longitude = first.Longitude,
                        Items = g.Select(x => new ItemVM
                        {
                            ProductID = x.ProductID,
                            ProductName = x.ProductName,
                            Quantity = x.Quantity,
                            LineTotal = x.LineTotal
                        }).ToList()
                    };
                })
                .OrderBy(x => x.SellerName)
                .ToList();

            foreach (var grp in groups)
                grp.SellerSubtotal = grp.Items.Sum(i => i.LineTotal);

            return groups;
        }

        private Dictionary<int, SellerInfo> LoadSellerInfoForProducts(List<int> productIds)
        {
            var map = new Dictionary<int, SellerInfo>();
            if (productIds == null || productIds.Count == 0) return map;

            var paramNames = productIds.Select((id, idx) => "@p" + idx).ToList();
            string inClause = string.Join(",", paramNames);

            string sql = $@"
SELECT
    p.ProductID,
    s.SellerID,
    s.ShopName,
    s.Address,
    s.PickupWindow,
    s.Latitude,
    s.Longitude
FROM Products p
INNER JOIN Seller s ON p.SellerID = s.SellerID
WHERE p.ProductID IN ({inClause});";

            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                for (int i = 0; i < productIds.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], productIds[i]);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int productId = Convert.ToInt32(r["ProductID"]);

                        var info = new SellerInfo
                        {
                            SellerID = Convert.ToInt32(r["SellerID"]),
                            ShopName = r["ShopName"]?.ToString(),
                            Address = r["Address"]?.ToString(),
                            PickupWindow = r["PickupWindow"] == DBNull.Value ? null : r["PickupWindow"].ToString(),
                            Latitude = r["Latitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Latitude"]),
                            Longitude = r["Longitude"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Longitude"])
                        };

                        map[productId] = info;
                    }
                }
            }

            return map;
        }

        protected void rptSellerGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (SellerGroupVM)e.Item.DataItem;

            var rpt = (Repeater)e.Item.FindControl("rptItemsBySeller");
            if (rpt != null)
            {
                rpt.DataSource = group.Items;
                rpt.DataBind();
            }

            var lnk = (HyperLink)e.Item.FindControl("lnkDirectionsSeller");
            if (lnk != null)
                lnk.NavigateUrl = BuildDirectionsUrl(group);

            var iframe = (HtmlIframe)e.Item.FindControl("mapFrameSeller");
            if (iframe != null)
                iframe.Attributes["src"] = BuildMapEmbedSrc(group);

            // translate per-group UI labels after binding
            string lang = GetLang();
            if (!lang.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                var pickupLoc = e.Item.FindControl("lblPickupLocationText") as Label;
                if (pickupLoc != null) pickupLoc.Text = T(pickupLoc.Text);

                var pickupWin = e.Item.FindControl("lblPickupWindowText") as Label;
                if (pickupWin != null) pickupWin.Text = T(pickupWin.Text);

                var sellerStatus = e.Item.FindControl("lblSellerStatusText") as Label;
                if (sellerStatus != null) sellerStatus.Text = T(sellerStatus.Text);

                var sellerSubtotal = e.Item.FindControl("lblSellerSubtotalText") as Label;
                if (sellerSubtotal != null) sellerSubtotal.Text = T(sellerSubtotal.Text);

                var link = e.Item.FindControl("lnkDirectionsSeller") as HyperLink;
                if (link != null) link.Text = T(link.Text);
            }
        }

        private string BuildDirectionsUrl(SellerGroupVM group)
        {
            if (group.Latitude.HasValue && group.Longitude.HasValue)
            {
                string latStr = group.Latitude.Value.ToString(CultureInfo.InvariantCulture);
                string lngStr = group.Longitude.Value.ToString(CultureInfo.InvariantCulture);
                return $"https://www.google.com/maps/dir/?api=1&destination={latStr},{lngStr}";
            }

            string addr = group.Address ?? "";
            string q = Uri.EscapeDataString(addr);
            return $"https://www.google.com/maps/dir/?api=1&destination={q}";
        }

        private string BuildMapEmbedSrc(SellerGroupVM group)
        {
            if (group.Latitude.HasValue && group.Longitude.HasValue)
            {
                string latStr = group.Latitude.Value.ToString(CultureInfo.InvariantCulture);
                string lngStr = group.Longitude.Value.ToString(CultureInfo.InvariantCulture);
                return $"https://www.google.com/maps?q={latStr},{lngStr}&output=embed";
            }

            string addr = group.Address ?? "";
            string q = Uri.EscapeDataString(addr);
            return $"https://www.google.com/maps?q={q}&output=embed";
        }

        private string ShortId(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return "UNKNOWN";
            if (sessionId.Length <= 10) return sessionId.ToUpperInvariant();
            return sessionId.Substring(sessionId.Length - 10).ToUpperInvariant();
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            lblError.Text = msg;
        }

        // ===== View Models for multi-seller UI =====
        [Serializable]
        private class SellerGroupVM
        {
            public int SellerID { get; set; }
            public string SellerName { get; set; }
            public string Address { get; set; }
            public string PickupWindow { get; set; }
            public string SellerStatus { get; set; }
            public decimal SellerSubtotal { get; set; }

            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }

            public List<ItemVM> Items { get; set; } = new List<ItemVM>();
        }

        [Serializable]
        private class ItemVM
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal LineTotal { get; set; }
        }

        private class SellerInfo
        {
            public int SellerID { get; set; }
            public string ShopName { get; set; }
            public string Address { get; set; }
            public string PickupWindow { get; set; }
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
        }
    }
}
