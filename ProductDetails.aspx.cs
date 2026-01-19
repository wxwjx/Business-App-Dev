using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";

        private int CurrentProductId
        {
            get
            {
                return int.TryParse(Request.QueryString["id"], out int id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (CurrentProductId <= 0)
            {
                return;
            }

            try
            {
                LoadProduct(CurrentProductId);
                ApplyTranslations(); // translate UI labels + the loaded product text
            }
            catch (SqlException)
            {
            }
            catch (Exception)
            {
            }
        }

        private string GetLang()
        {
            return (Session["LANG"] as string) ?? "en";
        }

        private string TranslateCached(string text, string targetLang, string sourceLang = "en")
        {
            text = text ?? "";
            targetLang = (targetLang ?? "en").Trim().ToLowerInvariant();
            sourceLang = (sourceLang ?? "en").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(text)) return text;
            if (targetLang == "en" || targetLang == sourceLang) return text;

            string key = $"tr:{sourceLang}->{targetLang}:{text}";
            return TranslationCache.GetOrAdd(key, () =>
                TranslationService.Translate(text, targetLang, sourceLang), hours: 24);
        }

        private void ApplyTranslations()
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            // Translate product content (already loaded into labels)
            lblName.Text = TranslateCached(lblName.Text, lang, "en");
            lblSubtitle.Text = TranslateCached(lblSubtitle.Text, lang, "en");
            lblDescription.Text = TranslateCached(lblDescription.Text, lang, "en");

            // UI labels
            lblBackHome.Text = TranslateCached(lblBackHome.Text, lang, "en");
            lblReviewsText.Text = TranslateCached(lblReviewsText.Text, lang, "en");
            lblKmAway.Text = TranslateCached(lblKmAway.Text, lang, "en");
            lblExpiresIn.Text = TranslateCached(lblExpiresIn.Text, lang, "en");

            lblImpactTitle.Text = TranslateCached(lblImpactTitle.Text, lang, "en");
            lblImpactMeal.Text = TranslateCached(lblImpactMeal.Text, lang, "en");
            lblImpactCO2.Text = TranslateCached(lblImpactCO2.Text, lang, "en");

            lblDescTitle.Text = TranslateCached(lblDescTitle.Text, lang, "en");
            lblSaveText.Text = TranslateCached(lblSaveText.Text, lang, "en");

            btnAddToCart.Text = TranslateCached(btnAddToCart.Text, lang, "en");
            btnBuyNow.Text = TranslateCached(btnBuyNow.Text, lang, "en");

            lblWhyTitle.Text = TranslateCached(lblWhyTitle.Text, lang, "en");
            lblWhy1.Text = TranslateCached(lblWhy1.Text, lang, "en");
            lblWhy2.Text = TranslateCached(lblWhy2.Text, lang, "en");
            lblWhy3.Text = TranslateCached(lblWhy3.Text, lang, "en");
            lblWhy4.Text = TranslateCached(lblWhy4.Text, lang, "en");

            lblToastTitle.Text = TranslateCached(lblToastTitle.Text, lang, "en");
            lblToastSub.Text = TranslateCached(lblToastSub.Text, lang, "en");
        }

        private void LoadProduct(int id)
        {
            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return;

                    lblName.Text = r["ProductName"]?.ToString() ?? "";
                    lblSubtitle.Text = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "";

                    // You are using Subtitle as description currently
                    lblDescription.Text = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "";

                    imgProduct.ImageUrl = r["ImageUrl"] != DBNull.Value ? r["ImageUrl"].ToString() : "";

                    decimal now = r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0m;
                    decimal old = r["PriceOld"] != DBNull.Value ? Convert.ToDecimal(r["PriceOld"]) : now;

                    lblPriceNow.Text = now.ToString("0.00");
                    lblPriceOld.Text = old.ToString("0.00");
                    lblSave.Text = (old - now).ToString("0.00");

                    lblRating.Text = r["Rating"] != DBNull.Value ? r["Rating"].ToString() : "0";
                    lblReviews.Text = r["Reviews"] != DBNull.Value ? r["Reviews"].ToString() : "0";
                    lblDistance.Text = r["DistanceKm"] != DBNull.Value ? r["DistanceKm"].ToString() : "0";
                    lblExpiry.Text = r["ExpiryHours"] != DBNull.Value ? r["ExpiryHours"].ToString() : "0";
                    lblCO2.Text = r["CO2Saved"] != DBNull.Value ? r["CO2Saved"].ToString() : "0";
                    lblDiscount.Text = r["DiscountPercent"] != DBNull.Value ? r["DiscountPercent"].ToString() : "0";

                    if (string.IsNullOrWhiteSpace(txtQty.Text))
                        txtQty.Text = "1";
                }
            }
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

        private int GetQtySafe()
        {
            if (!int.TryParse(txtQty.Text, out int qty))
                qty = 1;

            qty = Math.Max(1, qty);
            qty = Math.Min(99, qty);
            return qty;
        }

        private void AddItemToCart()
        {
            int qty = GetQtySafe();
            var cart = GetCart();

            decimal priceNow = 0m;
            decimal priceOld = 0m;
            double co2 = 0;

            decimal.TryParse(lblPriceNow.Text, out priceNow);
            decimal.TryParse(lblPriceOld.Text, out priceOld);
            double.TryParse(lblCO2.Text, out co2);

            var item = cart.FirstOrDefault(x => x.ProductID == CurrentProductId);
            if (item != null)
            {
                item.Quantity += qty;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = CurrentProductId,
                    ProductName = lblName.Text ?? "",
                    Subtitle = lblSubtitle.Text ?? "",
                    ImageUrl = imgProduct.ImageUrl ?? "",
                    PriceNow = priceNow,
                    PriceOld = priceOld,
                    CO2SavedPerMeal = co2,
                    Quantity = qty
                });
            }

            Session[CART_KEY] = cart;
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            int qty = GetQtySafe();
            txtQty.Text = Math.Max(1, qty - 1).ToString();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int qty = GetQtySafe();
            txtQty.Text = Math.Min(99, qty + 1).ToString();
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                AddItemToCart();
            }
            catch (Exception)
            {
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            try
            {
                AddItemToCart();
                Response.Redirect("Cart.aspx");
            }
            catch (Exception)
            {
            }
        }
    }
}
