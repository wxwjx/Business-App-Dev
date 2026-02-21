using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public partial class Product : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyTranslations();   // UI text (hero/pills)
                SetActivePillCss();
                TryLoad();
            }
        }

        protected void btnRefreshByLoc_Click(object sender, EventArgs e)
        {
            TryLoad();
        }

        // Pills
        protected void btnAI_Click(object sender, EventArgs e)
        {
            hfMode.Value = "AI";
            hfCategory.Value = "";
            pnlCategories.Visible = false;
            SetActivePillCss();
            TryLoad();
        }

        protected void btnDeals_Click(object sender, EventArgs e)
        {
            hfMode.Value = "DEALS";
            hfCategory.Value = "";
            pnlCategories.Visible = false;
            SetActivePillCss();
            TryLoad();
        }

        protected void btnCats_Click(object sender, EventArgs e)
        {
            hfMode.Value = "CATS";
            pnlCategories.Visible = true;

            rptCategories.DataSource = ProductModel.GetCategories();
            rptCategories.DataBind();

            SetActivePillCss();
            TryLoad();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Pick")
            {
                hfCategory.Value = (e.CommandArgument ?? "").ToString();
                TryLoad();
            }
        }

        protected void btnClearCategory_Click(object sender, EventArgs e)
        {
            hfCategory.Value = "";
            TryLoad();
        }

        private void SetActivePillCss()
        {
            btnAI.CssClass = "ee-pill";
            btnDeals.CssClass = "ee-pill";
            btnCats.CssClass = "ee-pill";

            string mode = (hfMode.Value ?? "AI").ToUpperInvariant();
            if (mode == "AI") btnAI.CssClass = "ee-pill active";
            else if (mode == "DEALS") btnDeals.CssClass = "ee-pill active";
            else if (mode == "CATS") btnCats.CssClass = "ee-pill active";
        }

        private void TryLoad()
        {
            try
            {
                LoadProducts();
            }
            catch (SqlException ex)
            {
                pnlError.Visible = true;
                lblError.Text = "Database error loading products: " + ex.Message;
            }
            catch (Exception ex)
            {
                pnlError.Visible = true;
                lblError.Text = "Unexpected error loading products: " + ex.Message;
            }
        }

        private string GetKeywordFromMaster()
        {
            var tb = Master?.FindControl("txtSearch") as TextBox;
            return (tb?.Text ?? "").Trim();
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

            lblHeroTitle.Text = TranslateCached(lblHeroTitle.Text, lang, "en");
            lblHeroSubtitle.Text = TranslateCached(lblHeroSubtitle.Text, lang, "en");

            lblMealsSaved.Text = TranslateCached(lblMealsSaved.Text, lang, "en");
            lblMoneySaved.Text = TranslateCached(lblMoneySaved.Text, lang, "en");
            lblCO2Saved.Text = TranslateCached(lblCO2Saved.Text, lang, "en");

            btnAI.Text = TranslateCached("✨ AI Recommended", lang, "en");
            btnDeals.Text = TranslateCached("🔥 Daily Best Deals", lang, "en");
            btnCats.Text = TranslateCached("🧭 Explore Categories", lang, "en");
        }

        private List<ProductModel> TranslateProductsIfNeeded(List<ProductModel> products)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return products;
            if (products == null) return products;

            foreach (var p in products)
            {
                if (p == null) continue;

                if (!string.IsNullOrWhiteSpace(p.ProductName))
                    p.ProductName = TranslateCached(p.ProductName, lang, "en");

                if (!string.IsNullOrWhiteSpace(p.Subtitle))
                    p.Subtitle = TranslateCached(p.Subtitle, lang, "en");
            }

            return products;
        }

        private void LoadProducts()
        {
            double userLat = 0;
            double userLng = 0;

            bool hasLoc =
                double.TryParse(hfLat.Value, out userLat) &&
                double.TryParse(hfLng.Value, out userLng);

            string keyword = GetKeywordFromMaster();
            string mode = (hfMode.Value ?? "AI").ToUpperInvariant();
            string category = (hfCategory.Value ?? "").Trim();

            if (mode == "DEALS")
            {
                var products = hasLoc
                    ? ProductModel.GetDailyBestDealsWithDistance(userLat, userLng, keyword)
                    : ProductModel.GetDailyBestDeals(keyword);

                products = TranslateProductsIfNeeded(products);

                ProductRepeater.DataSource = products;
                ProductRepeater.DataBind();
                return;
            }

            if (mode == "CATS")
            {
                var products = hasLoc
                    ? ProductModel.GetProductsByCategoryWithDistance(userLat, userLng, category, keyword)
                    : ProductModel.GetProductsByCategory(category, keyword);

                products = TranslateProductsIfNeeded(products);

                ProductRepeater.DataSource = products;
                ProductRepeater.DataBind();
                return;
            }

            // ✅ AI Recommended
            int userId = 0;
            if (Session["UserID"] != null)
                int.TryParse(Session["UserID"].ToString(), out userId);

            var aiProducts = hasLoc
                ? ProductModel.GetAIRecommended(userId, userLat, userLng, keyword)
                : ProductModel.GetProductsBySearch(keyword);

            aiProducts = TranslateProductsIfNeeded(aiProducts);

            ProductRepeater.DataSource = aiProducts;
            ProductRepeater.DataBind();
        }
    }
}