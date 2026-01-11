using System;
using System.Collections.Generic;
using Stripe;
using Stripe.Checkout;

namespace Business_App_Dev
{
    public partial class OrderSuccess : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadOrder();
        }

        private void LoadOrder()
        {
            string sessionId = Request.QueryString["session_id"];
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                ShowError("Missing payment session. Please return to cart.");
                return;
            }

            // ✅ Verify Stripe payment
            // NOTE: StripeConfiguration.ApiKey must be set somewhere globally (recommended in Global.asax)
            // If not, set it here too.
            // StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];

            var service = new SessionService();
            Session stripeSession;

            try
            {
                stripeSession = service.Get(sessionId);
            }
            catch
            {
                ShowError("Unable to verify payment session with Stripe.");
                return;
            }

            if (!string.Equals(stripeSession.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Payment not completed. If you were charged, please contact support.");
                return;
            }

            // ✅ Load snapshot saved from Cart.aspx.cs
            var snapshot = Session["PENDING_ORDER_" + sessionId] as List<PurchasedItem>;
            if (snapshot == null || snapshot.Count == 0)
            {
                ShowError("Order details not found (session expired).");
                return;
            }

            pnlOrder.Visible = true;

            lblOrderId.Text = sessionId;
            lblOrderDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            rptItems.DataSource = snapshot;
            rptItems.DataBind();

            decimal total = 0m;
            foreach (var i in snapshot) total += i.LineTotal;
            lblTotal.Text = total.ToString("0.00");

            // ✅ clear cart after successful purchase
            Session[CART_KEY] = new List<CartItem>();

            // ✅ optional: clean up pending snapshot
            Session.Remove("PENDING_ORDER_" + sessionId);
            Session.Remove("PENDING_ORDER_TOTAL_" + sessionId);
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            pnlOrder.Visible = false;
            lblError.Text = msg;
        }
    }
}
