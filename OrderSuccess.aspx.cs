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
                // 1) Read session_id from Stripe redirect
                string sessionId = Request.QueryString["session_id"];
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    ShowError("Missing session_id from Stripe redirect.");
                    return;
                }

                // 2) Verify payment with Stripe
                if (!VerifyStripePaid(sessionId, out string failMsg))
                {
                    ShowError(failMsg);
                    return;
                }

                // 3) Display an Order ID (nice format) - until you implement Orders table
                lblOrderId.Text = "ORD-" + ShortId(sessionId);

                // 4) Load snapshot from Session saved in Cart.aspx.cs
                var items = Session["PENDING_ORDER_" + sessionId] as List<PurchasedItem>;
                if (items == null || items.Count == 0)
                {
                    ShowError("Order data not found (Session expired). Try paying again, or implement Orders table to persist.");
                    return;
                }

                pnlError.Visible = false;

                // 5) Total
                decimal total;
                if (Session["PENDING_ORDER_TOTAL_" + sessionId] is decimal t)
                    total = t;
                else
                    total = items.Sum(x => x.LineTotal);

                lblTotal.Text = total.ToString("0.00");
                lblPayStatus.Text = "PAID";

                // 6) IMPORTANT: clear purchased items from cart AFTER payment confirmed
                // Make it safe against refresh: only remove if snapshot still exists
                RemovePurchasedItemsFromCart(items);

                // 7) Build seller groups using ProductID -> Seller data from DB
                var groups = BuildSellerGroups(items);

                if (groups.Count == 0)
                {
                    ShowError("Could not determine pickup locations. Ensure Products.SellerID is filled and Seller table has data.");
                    return;
                }

                // 8) Bind seller groups
                rptSellerGroups.DataSource = groups;
                rptSellerGroups.DataBind();

                // 9) Clean up pending snapshot so refresh won't remove again
                Session.Remove("PENDING_ORDER_" + sessionId);
                Session.Remove("PENDING_ORDER_TOTAL_" + sessionId);
            }
            catch (StripeException)
            {
                ShowError("Payment verification failed due to a payment service error. Please try again.");
            }
            catch (SqlException)
            {
                ShowError("Database error while loading pickup locations. Please try again later.");
            }
            catch (Exception)
            {
                ShowError("Unexpected error occurred while loading your order. Please try again.");
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

            // Get cart from session
            var cart = Session[CART_KEY] as List<CartItem> ?? new List<CartItem>();

            // Purchased product IDs
            var purchasedIds = purchasedItems.Select(x => x.ProductID).ToHashSet();

            // Remove purchased items from cart
            cart.RemoveAll(ci => purchasedIds.Contains(ci.ProductID));

            // Save back
            Session[CART_KEY] = cart;

            // Clear selection + init flag to avoid weird auto-select state
            Session[CART_SELECTED_KEY] = new HashSet<int>();
            Session[CART_SELECTED_INIT_KEY] = false;
        }

        private string ConnStr()
        {
            // CHANGE name if yours is different
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
        }

        // 1) Get Seller info for all productIds in one DB call
        // 2) Group items by SellerID and produce view model for repeater
        private List<SellerGroupVM> BuildSellerGroups(List<PurchasedItem> items)
        {
            var productIds = items.Select(i => i.ProductID).Distinct().ToList();
            var productSellerMap = LoadSellerInfoForProducts(productIds);

            var usableItems = items.Where(i => productSellerMap.ContainsKey(i.ProductID)).ToList();

            var groups = usableItems
                .GroupBy(i =>
                {
                    var s = productSellerMap[i.ProductID];
                    return s.SellerID;
                })
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

            if (productIds == null || productIds.Count == 0)
                return map;

            try
            {
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
WHERE p.ProductID IN ({inClause});
";

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
            }
            catch (SqlException)
            {
                // bubble up a clear message to Page_Load handler
                throw;
            }
            catch (Exception)
            {
                // bubble up
                throw;
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

    // PurchasedItem class is in Cart.aspx.cs already.
    // CartItem class is your existing CartItem model.
}
