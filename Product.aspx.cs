using System;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class Product : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetActivePillCss();
                TryLoad();
            }
        }

        protected void btnRefreshByLoc_Click(object sender, EventArgs e)
        {
            TryLoad();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            TryLoad();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
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

            // load chips
            rptCategories.DataSource = ProductModel.GetCategories();
            rptCategories.DataBind();

            SetActivePillCss();
            TryLoad();
        }

        protected void rptCategories_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
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
            // Reset
            btnAI.CssClass = "ee-pill";
            btnDeals.CssClass = "ee-pill";
            btnCats.CssClass = "ee-pill";

            // Active
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

        private void LoadProducts()
        {
            // Location parse (FIX CS0165)
            double userLat = 0;
            double userLng = 0;

            bool hasLoc =
                double.TryParse(hfLat.Value, out userLat) &&
                double.TryParse(hfLng.Value, out userLng);

            string keyword = (txtSearch.Text ?? "").Trim();
            string mode = (hfMode.Value ?? "AI").ToUpperInvariant();
            string category = (hfCategory.Value ?? "").Trim();

            if (mode == "DEALS")
            {
                // Deals mode: most bought today pinned + best deals
                var products = hasLoc
                    ? ProductModel.GetDailyBestDealsWithDistance(userLat, userLng, keyword)
                    : ProductModel.GetDailyBestDeals(keyword);

                ProductRepeater.DataSource = products;
                ProductRepeater.DataBind();
                return;
            }

            if (mode == "CATS")
            {
                // Category mode: filter by category (if picked), still allow search keyword
                var products = hasLoc
                    ? ProductModel.GetProductsByCategoryWithDistance(userLat, userLng, category, keyword)
                    : ProductModel.GetProductsByCategory(category, keyword);

                ProductRepeater.DataSource = products;
                ProductRepeater.DataBind();
                return;
            }

            // Default: AI mode (distance + search)
            var aiProducts = hasLoc
                ? ProductModel.GetProductsWithDistanceAndSearch(userLat, userLng, keyword)
                : ProductModel.GetProductsBySearch(keyword);

            ProductRepeater.DataSource = aiProducts;
            ProductRepeater.DataBind();
        }
    }
}
