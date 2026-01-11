using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class Cart : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindCart();
        }

        private List<CartItem> GetCart()
        {
            var cart = Session[CART_KEY] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CART_KEY] = cart;
            }
            return cart;
        }

        private void BindCart()
        {
            var cart = GetCart();

            pnlEmpty.Visible = cart.Count == 0;
            rptCart.Visible = cart.Count > 0;

            rptCart.DataSource = cart;
            rptCart.DataBind();

            int itemCount = cart.Sum(x => x.Quantity);
            lblItemCount.Text = itemCount.ToString();

            decimal subtotal = cart.Sum(x => x.LineTotal);
            double co2 = cart.Sum(x => x.LineCO2);

            lblSubtotal.Text = subtotal.ToString("0.00");
            lblTotal.Text = subtotal.ToString("0.00"); // delivery FREE
            lblCO2.Text = co2.ToString("0.0");
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var cart = GetCart();

            int productId;
            if (!int.TryParse(e.CommandArgument.ToString(), out productId))
                return;

            var item = cart.FirstOrDefault(x => x.ProductID == productId);
            if (item == null) return;

            switch (e.CommandName)
            {
                case "plus":
                    item.Quantity = Math.Min(99, item.Quantity + 1);
                    break;

                case "minus":
                    item.Quantity = Math.Max(1, item.Quantity - 1);
                    break;

                case "remove":
                    cart.Remove(item);
                    break;
            }

            Session[CART_KEY] = cart;
            BindCart();
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            string code = (txtCode.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                lblCodeMsg.Text = "Please enter a code.";
                return;
            }

            // Placeholder (demo)
            lblCodeMsg.Text = "Invalid code (demo).";
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            // Change to Checkout.aspx when you create it
            Response.Redirect("Order.aspx");
        }
    }
}
