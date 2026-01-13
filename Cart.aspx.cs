using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Stripe;
using Stripe.Checkout;

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
                BindAll();
        }

        private List<CartItem> GetCart()
        {
            if (Session[CART_KEY] is List<CartItem> cart) return cart;
            cart = new List<CartItem>();
            Session[CART_KEY] = cart;
            return cart;
        }

        private HashSet<int> GetSelected()
        {
            if (Session[CART_SELECTED_KEY] is HashSet<int> set) return set;
            set = new HashSet<int>();
            Session[CART_SELECTED_KEY] = set;
            return set;
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

            // Auto-select all only ONCE (first time user enters cart)
            if (!IsSelectionInitialized() && cart.Count > 0)
            {
                selected.Clear();
                foreach (var it in cart)
                    selected.Add(it.ProductID);

                MarkSelectionInitialized();
            }

            // Remove selections that no longer exist
            var idsInCart = cart.Select(x => x.ProductID).ToHashSet();
            selected.RemoveWhere(id => !idsInCart.Contains(id));

            Session[CART_SELECTED_KEY] = selected;
        }

        private void BindAll()
        {
            var cart = GetCart();
            EnsureSelectionValid(cart);

            bool hasItems = cart.Count > 0;

            // Empty state
            lblEmpty.Visible = !hasItems;
            rptCart.Visible = hasItems;

            // ✅ hide select-all when cart empty
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

            rptCart.DataSource = cart;
            rptCart.DataBind();

            lblItemCount.Text = cart.Sum(x => x.Quantity).ToString();

            // Totals based on selected items
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
                ? "Select at least 1 item to checkout."
                : "";
        }

        // ✅ checkbox stays checked after postback
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

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
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

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
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
                    selected.Add(productId); // keep selected
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

        protected void btnPay_Click(object sender, EventArgs e)
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                lblPayMsg.Text = "Your cart is empty.";
                return;
            }

            var selected = GetSelected();
            var selectedItems = cart.Where(x => selected.Contains(x.ProductID)).ToList();

            if (selectedItems.Count == 0)
            {
                lblPayMsg.Text = "Select at least 1 item to checkout.";
                return;
            }

            RedirectToStripeCheckout(selectedItems);
        }

        private void RedirectToStripeCheckout(List<CartItem> cartToPay)
        {
            var key = ConfigurationManager.AppSettings["StripeSecretKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                lblPayMsg.Text = "Stripe is not configured (StripeSecretKey missing in Web.config).";
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

            // Snapshot only selected
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

            Response.Redirect(session.Url);
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
