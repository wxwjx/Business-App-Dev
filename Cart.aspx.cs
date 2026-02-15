using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;
using Stripe.Checkout;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public partial class Cart : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";
        private const string CART_SELECTED_KEY = "CART_SELECTED";           // HashSet<int>
        private const string CART_SELECTED_INIT_KEY = "CART_SELECTED_INIT"; // bool

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    ApplyTranslations(); // NEW
                    BindAll();
                }
                catch (Exception)
                {
                    lblPayMsg.Text = TranslateUi("Unable to load your cart right now. Please refresh and try again.");
                    btnPay.Enabled = false;
                    rptCart.Visible = false;
                    pnlSelectAll.Visible = false;
                    lblEmpty.Visible = true;
                }
            }
        }

        // -------- Translation helpers --------

        private string GetLang()
        {
            return (Session["LANG"] as string) ?? "en";
        }

        private string TranslateUi(string text)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return text ?? "";

            text = text ?? "";
            if (string.IsNullOrWhiteSpace(text)) return text;

            string key = $"tr:en->{lang}:{text}";
            return TranslationCache.GetOrAdd(key, () =>
                TranslationService.Translate(text, lang, "en"), hours: 24);
        }

        private void ApplyTranslations()
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            // top / left
            lblContinueShopping.Text = TranslateUi(lblContinueShopping.Text);
            lblCartTitle.Text = TranslateUi(lblCartTitle.Text);
            lblItemsText.Text = TranslateUi(lblItemsText.Text);
            lblEmptyText.Text = TranslateUi(lblEmptyText.Text);
            lblSelectAllText.Text = TranslateUi(lblSelectAllText.Text);

            // right summary
            lblOrderSummaryTitle.Text = TranslateUi(lblOrderSummaryTitle.Text);
            lblSubtotalText.Text = TranslateUi(lblSubtotalText.Text);
            lblDeliveryText.Text = TranslateUi(lblDeliveryText.Text);
            lblSelfPickup.Text = TranslateUi(lblSelfPickup.Text);
            lblTotalText.Text = TranslateUi(lblTotalText.Text);
            lblTotalImpactText.Text = TranslateUi(lblTotalImpactText.Text);
            lblKgCO2SavedText.Text = TranslateUi(lblKgCO2SavedText.Text);

            // button
            btnPay.Text = TranslateUi(btnPay.Text);
        }

        // -------- Cart logic --------

        private List<CartItem> GetCart()
        {
            try
            {
                if (Session[CART_KEY] is List<CartItem> cart) return cart;
                cart = new List<CartItem>();
                Session[CART_KEY] = cart;
                return cart;
            }
            catch
            {
                var cart = new List<CartItem>();
                Session[CART_KEY] = cart;
                return cart;
            }
        }

        private HashSet<int> GetSelected()
        {
            try
            {
                if (Session[CART_SELECTED_KEY] is HashSet<int> set) return set;
                set = new HashSet<int>();
                Session[CART_SELECTED_KEY] = set;
                return set;
            }
            catch
            {
                var set = new HashSet<int>();
                Session[CART_SELECTED_KEY] = set;
                return set;
            }
        }

        private bool IsSelectionInitialized()
        {
            return (Session[CART_SELECTED_INIT_KEY] is bool b) && b;
        }

        private void MarkSelectionInitialized()
        {
            Session[CART_SELECTED_INIT_KEY] = true;
        }

        private void EnsureSelectionValid(List<CartItem> cart)
        {
            var selected = GetSelected();

            if (!IsSelectionInitialized() && cart.Count > 0)
            {
                selected.Clear();
                foreach (var it in cart)
                    selected.Add(it.ProductID);

                MarkSelectionInitialized();
            }

            var idsInCart = cart.Select(x => x.ProductID).ToHashSet();
            selected.RemoveWhere(id => !idsInCart.Contains(id));

            Session[CART_SELECTED_KEY] = selected;
        }

        private void BindAll()
        {
            var cart = GetCart();
            EnsureSelectionValid(cart);

            bool hasItems = cart.Count > 0;

            lblEmpty.Visible = !hasItems;
            rptCart.Visible = hasItems;
            pnlSelectAll.Visible = hasItems;

            if (!hasItems)
            {
                lblItemCount.Text = "0";
                lblSubtotal.Text = "0.00";
                lblTotal.Text = "0.00";
                lblCO2.Text = "0.0";
                btnPay.Enabled = false;
                lblPayMsg.Text = "";
                chkSelectAll.Checked = false;
                return;
            }

            // OPTIONAL: translate product names/subtitles in cart session too
            // (Only if your cart items are often English)
            string lang = GetLang();
            if (!lang.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var it in cart)
                {
                    if (!string.IsNullOrWhiteSpace(it.ProductName))
                        it.ProductName = TranslateUi(it.ProductName);

                    if (!string.IsNullOrWhiteSpace(it.Subtitle))
                        it.Subtitle = TranslateUi(it.Subtitle);
                }
                Session[CART_KEY] = cart;
            }

            rptCart.DataSource = cart;
            rptCart.DataBind();

            lblItemCount.Text = cart.Sum(x => x.Quantity).ToString();

            var selected = GetSelected();
            var selectedItems = cart.Where(x => selected.Contains(x.ProductID)).ToList();

            decimal subtotal = selectedItems.Sum(x => x.LineTotal);
            double co2 = selectedItems.Sum(x => x.LineCO2);

            lblSubtotal.Text = subtotal.ToString("0.00");
            lblTotal.Text = subtotal.ToString("0.00");
            lblCO2.Text = co2.ToString("0.0");

            chkSelectAll.Checked = (cart.Count > 0 && selected.Count == cart.Count);

            btnPay.Enabled = selectedItems.Count > 0;
            lblPayMsg.Text = (selectedItems.Count == 0)
                ? TranslateUi("Select at least 1 item to checkout.")
                : "";
        }

        protected void rptCart_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var hf = (HiddenField)e.Item.FindControl("hfPid");
            var cb = (CheckBox)e.Item.FindControl("chkSelect");
            if (hf == null || cb == null) return;

            if (int.TryParse(hf.Value, out int productId))
            {
                var selected = GetSelected();
                cb.Checked = selected.Contains(productId);
            }
        }

        public string GetDesc(object dataItem)
        {
            if (dataItem == null) return "";
            string[] candidates = { "Description", "ProductDescription", "Desc" };

            foreach (var name in candidates)
            {
                PropertyInfo prop = dataItem.GetType().GetProperty(name);
                if (prop != null)
                {
                    object val = prop.GetValue(dataItem, null);
                    return val?.ToString() ?? "";
                }
            }
            return "";
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var cb = sender as CheckBox;
                var item = cb?.NamingContainer as RepeaterItem;
                if (item == null) return;

                var hf = item.FindControl("hfPid") as HiddenField;
                if (hf == null) return;

                if (!int.TryParse(hf.Value, out int productId)) return;

                var selected = GetSelected();
                if (cb.Checked) selected.Add(productId);
                else selected.Remove(productId);

                Session[CART_SELECTED_KEY] = selected;
                BindAll();
            }
            catch (Exception)
            {
                lblPayMsg.Text = TranslateUi("Could not update selection. Please refresh and try again.");
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var cart = GetCart();
                var selected = GetSelected();

                selected.Clear();
                if (chkSelectAll.Checked)
                {
                    foreach (var it in cart)
                        selected.Add(it.ProductID);
                }

                Session[CART_SELECTED_KEY] = selected;
                BindAll();
            }
            catch (Exception)
            {
                lblPayMsg.Text = TranslateUi("Could not update selection. Please refresh and try again.");
            }
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                var cart = GetCart();
                var selected = GetSelected();

                if (!int.TryParse(e.CommandArgument?.ToString(), out int productId))
                    return;

                var item = cart.FirstOrDefault(x => x.ProductID == productId);
                if (item == null) return;

                switch (e.CommandName)
                {
                    case "INC":
                        item.Quantity += 1;
                        selected.Add(productId);
                        break;

                    case "DEC":
                        item.Quantity -= 1;
                        if (item.Quantity <= 0)
                        {
                            cart.Remove(item);
                            selected.Remove(productId);
                        }
                        break;

                    case "REMOVE":
                        cart.Remove(item);
                        selected.Remove(productId);
                        break;
                }

                Session[CART_KEY] = cart;
                Session[CART_SELECTED_KEY] = selected;
                BindAll();
            }
            catch (Exception)
            {
                lblPayMsg.Text = TranslateUi("Could not update cart. Please refresh and try again.");
            }
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                lblPayMsg.Text = "";

                var cart = GetCart();
                if (cart == null || cart.Count == 0)
                {
                    lblPayMsg.Text = TranslateUi("Your cart is empty.");
                    return;
                }

                var selected = GetSelected() ?? new HashSet<int>();
                var selectedItems = cart.Where(x => selected.Contains(x.ProductID)).ToList();

                if (selectedItems.Count == 0)
                {
                    lblPayMsg.Text = TranslateUi("Select at least 1 item to checkout.");
                    return;
                }

                RedirectToStripeCheckout(selectedItems);
            }
            catch (Exception)
            {
                lblPayMsg.Text = TranslateUi("Something went wrong starting payment. Please try again.");
            }
        }

        private void RedirectToStripeCheckout(List<CartItem> cartToPay)
        {
            try
            {
                if (cartToPay == null || cartToPay.Count == 0)
                {
                    lblPayMsg.Text = TranslateUi("No items selected.");
                    return;
                }

                var key = ConfigurationManager.AppSettings["StripeSecretKey"];
                if (string.IsNullOrWhiteSpace(key))
                {
                    lblPayMsg.Text = TranslateUi("Stripe is not configured (StripeSecretKey missing in Web.config).");
                    return;
                }

                StripeConfiguration.ApiKey = key.Trim();

                var lineItems = cartToPay.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "sgd",
                        UnitAmount = (long)(item.PriceNow * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        }
                    }
                }).ToList();

                string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";

                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    LineItems = lineItems,
                    SuccessUrl = baseUrl + "/OrderSuccess.aspx?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = baseUrl + "/Cart.aspx",
                };

                var service = new SessionService();
                var session = service.Create(options);

                var snapshot = cartToPay.Select(x => new PurchasedItem
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    UnitPrice = x.PriceNow,
                    Quantity = x.Quantity,
                    LineTotal = x.LineTotal
                }).ToList();

                Session["PENDING_ORDER_" + session.Id] = snapshot;
                Session["PENDING_ORDER_TOTAL_" + session.Id] = snapshot.Sum(i => i.LineTotal);

                Response.Redirect(session.Url, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (StripeException)
            {
                lblPayMsg.Text = TranslateUi("Payment service is unavailable right now. Please try again later.");
            }
            catch (Exception)
            {
                lblPayMsg.Text = TranslateUi("Could not start payment. Please try again.");
            }
        }
    }

    [Serializable]
    public class PurchasedItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
