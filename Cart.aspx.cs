using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Stripe;
using Stripe.Checkout;

namespace Business_App_Dev
{
    public partial class Cart : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindAll();
        }

        private List<CartItem> GetCart()
        {
            if (Session[CART_KEY] is List<CartItem> cart)
                return cart;

            cart = new List<CartItem>();
            Session[CART_KEY] = cart;
            return cart;
        }

        private void BindAll()
        {
            var cart = GetCart();

            lblEmpty.Visible = (cart.Count == 0);
            rptCart.Visible = (cart.Count > 0);

            rptCart.DataSource = cart;
            rptCart.DataBind();

            decimal subtotal = cart.Sum(x => x.LineTotal);
            double co2 = cart.Sum(x => x.LineCO2);

            lblSubtotal.Text = subtotal.ToString("0.00");
            lblTotal.Text = subtotal.ToString("0.00");
            lblCO2.Text = co2.ToString("0.0");

            int itemCount = cart.Sum(x => x.Quantity);
            lblItemCount.Text = itemCount.ToString();

            btnPay.Enabled = cart.Count > 0;
            btnPay.Text = "Payment";
            lblPayMsg.Text = "";
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

        protected void rptCart_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            var cart = GetCart();

            if (!int.TryParse(e.CommandArgument?.ToString(), out int productId))
                return;

            var item = cart.FirstOrDefault(x => x.ProductID == productId);
            if (item == null) return;

            switch (e.CommandName)
            {
                case "INC":
                    item.Quantity += 1;
                    break;

                case "DEC":
                    item.Quantity -= 1;
                    if (item.Quantity <= 0)
                        cart.Remove(item);
                    break;

                case "REMOVE":
                    cart.Remove(item);
                    break;
            }

            Session[CART_KEY] = cart;
            BindAll();
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            lblPayMsg.Text = "";

            var cart = GetCart();
            if (cart.Count == 0)
            {
                lblPayMsg.Text = "Your cart is empty.";
                return;
            }

            RedirectToStripeCheckout(cart);
        }

        private void RedirectToStripeCheckout(List<CartItem> cart)
        {
            var key = ConfigurationManager.AppSettings["StripeSecretKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                lblPayMsg.Text = "Stripe is not configured (StripeSecretKey missing in Web.config).";
                return;
            }

            StripeConfiguration.ApiKey = key.Trim();

            var lineItems = cart.Select(item => new SessionLineItemOptions
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

                // ✅ send session id back so we can verify + load items
                SuccessUrl = baseUrl + "/OrderSuccess.aspx?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = baseUrl + "/Cart.aspx",
            };

            var service = new SessionService();
            var session = service.Create(options);

            // ✅ Save snapshot so success page can show EXACTLY what was purchased
            var snapshot = cart.Select(x => new PurchasedItem
            {
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                UnitPrice = x.PriceNow,
                Quantity = x.Quantity,
                LineTotal = x.LineTotal
            }).ToList();

            Session["PENDING_ORDER_" + session.Id] = snapshot;

            // Optional: store totals too
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