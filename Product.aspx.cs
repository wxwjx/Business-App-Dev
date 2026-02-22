using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class Product : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetActivePillCss();
                TryLoad();
            }
        }

        protected void btnRefreshByLoc_Click(object sender, EventArgs e) => TryLoad();

        protected void txtSearch_TextChanged(object sender, EventArgs e) => TryLoad();

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            TryLoad();
        }

        protected void btnAI_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            hfMode.Value = "AI";
            hfCategory.Value = "";
            pnlCategories.Visible = false;
            SetActivePillCss();
            TryLoad();
        }

        protected void btnDeals_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            hfMode.Value = "DEALS";
            hfCategory.Value = "";
            pnlCategories.Visible = false;
            SetActivePillCss();
            TryLoad();
        }

        protected void btnCats_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            hfMode.Value = "CATS";
            pnlCategories.Visible = !pnlCategories.Visible;

            if (pnlCategories.Visible)
            {
                rptCategories.DataSource = ProductModel.GetCategories();
                rptCategories.DataBind();
            }

            SetActivePillCss();
            TryLoad();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Pick")
            {
                txtSearch.Text = "";
                hfMode.Value = "CATS";
                hfCategory.Value = (e.CommandArgument ?? "").ToString();
                pnlCategories.Visible = true;
                SetActivePillCss();
                TryLoad();
            }
        }

        protected void btnClearCategory_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            hfCategory.Value = "";
            pnlCategories.Visible = false;
            hfMode.Value = "AI";
            SetActivePillCss();
            TryLoad();
        }

        private string Keyword()
        {
            return (txtSearch.Text ?? "").Trim();
        }

        private double Lat { get { double.TryParse(hfLat.Value, out double v); return v; } }
        private double Lng { get { double.TryParse(hfLng.Value, out double v); return v; } }

        private bool HasLoc
        {
            get
            {
                if (hfHasLoc.Value != "1") return false;
                return double.TryParse(hfLat.Value, out _) && double.TryParse(hfLng.Value, out _);
            }
        }

        private int UserId
        {
            get
            {
                if (Session["UserID"] == null) return 0;
                int.TryParse(Session["UserID"].ToString(), out int id);
                return id;
            }
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
                pnlError.Visible = false;
                lblError.Text = "";
            }
            catch (SqlException ex)
            {
                pnlError.Visible = true;
                lblError.Text = "Database error: " + ex.Message;
            }
            catch (Exception ex)
            {
                pnlError.Visible = true;
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void LoadProducts()
        {
            string kw = Keyword();

            if (!string.IsNullOrWhiteSpace(kw))
            {
                pnlSearchResults.Visible = true;
                pnlBrowse.Visible = false;

                pnlCategories.Visible = false;
                hfCategory.Value = "";

                var results = HasLoc
                    ? ProductModel.SearchProductsWithDistance(Lat, Lng, kw, scope: "ALL")
                    : ProductModel.SearchProducts(kw, scope: "ALL");

                rptSearch.DataSource = results;
                rptSearch.DataBind();

                litSearchMeta.Text = "Showing results for <b>" + Server.HtmlEncode(kw) + "</b>";
                return;
            }

            pnlSearchResults.Visible = false;
            pnlBrowse.Visible = true;
            litSearchMeta.Text = "";

            string mode = (hfMode.Value ?? "AI").ToUpperInvariant();
            string category = (hfCategory.Value ?? "").Trim();

            if (mode == "DEALS")
            {
                var deals = HasLoc
                    ? ProductModel.GetDailyBestDealsWithDistance(Lat, Lng, "")
                    : ProductModel.GetDailyBestDeals("");

                ProductRepeater.DataSource = deals;
                ProductRepeater.DataBind();
                return;
            }

            if (mode == "CATS")
            {
                var cats = HasLoc
                    ? ProductModel.GetProductsByCategoryWithDistance(Lat, Lng, category, "")
                    : ProductModel.GetProductsByCategory(category, "");

                ProductRepeater.DataSource = cats;
                ProductRepeater.DataBind();
                return;
            }

            var ai = HasLoc
                ? ProductModel.GetAIRecommended(UserId, Lat, Lng, "")
                : ProductModel.GetProductsBySearch("");

            ProductRepeater.DataSource = ai;
            ProductRepeater.DataBind();
        }
    }
}