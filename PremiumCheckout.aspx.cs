using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using Stripe;
using Stripe.Checkout;

namespace Business_App_Dev
{
    public partial class PremiumCheckout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Must be logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            // ✅ Stripe key
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];

            // ✅ Build absolute URLs (important for Stripe redirect)
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var options = new SessionCreateOptions
            {
                Mode = "payment",

                SuccessUrl = baseUrl + "/PremiumSuccess.aspx?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = baseUrl + "/Profile.aspx?payment=cancel",

                ClientReferenceId = userId.ToString(),

                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "sgd",
                            UnitAmount = 499, // S$4.99 = 499 cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "EcoEats Premium Membership",
                                Description = "Unlock premium perks and exclusive discounts"
                            }
                        }
                    }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            // ✅ Redirect user to Stripe Checkout
            Response.Redirect(session.Url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}