using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Business_App_Dev.Pages
{
    public partial class ProductPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ViewState["tab"] == null) ViewState["tab"] = "recommended";
                if (ViewState["cat"] == null) ViewState["cat"] = "";
                BindCategories();
                BindProducts();
            }
        }

        private void BindCategories()
        {
            var cats = ProductModel.GetCategories() ?? new List<string>();
            rptCategories.DataSource = cats;
            rptCategories.DataBind();
        }

        private void BindProducts()
        {
            string tab = (ViewState["tab"] as string) ?? "recommended";
            string keyword = (txtSearch.Text ?? "").Trim();
            string selectedCat = (ViewState["cat"] as string) ?? "";

            double? lat = TryParseDouble(hfLat.Value);
            double? lng = TryParseDouble(hfLng.Value);

            int userId = GetUserId();

            List<ProductModel> rows;

            if (tab == "deals")
            {
                pnlCategories.Visible = false;

                if (lat.HasValue && lng.HasValue && lat.Value != 0 && lng.Value != 0)
                    rows = ProductModel.GetDailyBestDealsWithDistance(lat.Value, lng.Value, keyword);
                else
                    rows = ProductModel.GetDailyBestDeals(keyword);
            }
            else if (tab == "categories")
            {
                pnlCategories.Visible = true;

                if (lat.HasValue && lng.HasValue && lat.Value != 0 && lng.Value != 0)
                    rows = ProductModel.GetProductsByCategoryWithDistance(lat.Value, lng.Value, selectedCat, keyword);
                else
                    rows = ProductModel.GetProductsByCategory(selectedCat, keyword);

                rows = rows
                    .OrderBy(x => x.DistanceKm)
                    .ThenByDescending(x => x.DiscountPercent)
                    .ThenByDescending(x => x.CreatedAt)
                    .ToList();
            }
            else
            {
                pnlCategories.Visible = false;

                if (lat.HasValue && lng.HasValue && lat.Value != 0 && lng.Value != 0)
                    rows = ProductModel.GetAIRecommended(userId, lat.Value, lng.Value, keyword);
                else
                    rows = ProductModel.GetAIRecommended(userId, 0, 0, keyword);
            }

            if (rows == null) rows = new List<ProductModel>();

            var vm = rows.Select(p => new ProductCardVm
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName ?? "",
                Subtitle = p.Subtitle ?? "",
                ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? "Content/img/placeholder.jpg" : p.ImageUrl,
                PriceNow = p.PriceNow,
                Rating = p.Rating,
                Reviews = p.Reviews,
                DistanceKm = (p.DistanceKm >= 9999) ? (double?)null : p.DistanceKm,
                DiscountPercent = p.DiscountPercent,
                Category = p.Category ?? "",
                StoreName = GetShopNameFromSeller(p.SellerID)
            }).ToList();

            rptProducts.DataSource = vm;
            rptProducts.DataBind();

            MarkActiveCategoryChip(selectedCat);
        }

        private void MarkActiveCategoryChip(string selectedCat)
        {
            btnCatAll.CssClass = "cat-chip" + (string.IsNullOrWhiteSpace(selectedCat) ? " active" : "");

            foreach (RepeaterItem it in rptCategories.Items)
            {
                var lb = it.Controls.OfType<LinkButton>().FirstOrDefault();
                if (lb == null) continue;

                string cat = lb.CommandArgument ?? "";
                bool active = string.Equals(cat, selectedCat, StringComparison.OrdinalIgnoreCase);
                lb.CssClass = "cat-chip" + (active ? " active" : "");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) => BindProducts();

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindProducts();
        }

        protected void btnTabRecommended_Click(object sender, EventArgs e)
        {
            ViewState["tab"] = "recommended";
            BindProducts();
        }

        protected void btnTabDeals_Click(object sender, EventArgs e)
        {
            ViewState["tab"] = "deals";
            BindProducts();
        }

        protected void btnTabCategories_Click(object sender, EventArgs e)
        {
            ViewState["tab"] = "categories";
            BindCategories();
            BindProducts();
        }

        protected void btnGeoRefresh_Click(object sender, EventArgs e) => BindProducts();

        protected void btnCatAll_Click(object sender, EventArgs e)
        {
            ViewState["cat"] = "";
            BindProducts();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "pick")
            {
                ViewState["cat"] = (e.CommandArgument ?? "").ToString();
                BindProducts();
            }
        }

        private int GetUserId()
        {
            try
            {
                if (Session["UserID"] != null && int.TryParse(Session["UserID"].ToString(), out var id)) return id;
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out id)) return id;
                if (Session["MemberID"] != null && int.TryParse(Session["MemberID"].ToString(), out id)) return id;
            }
            catch { }
            return 0;
        }

        private static double? TryParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v)) return v;
            return null;
        }

        private static string GetShopNameFromSeller(int sellerId)
        {
            try
            {
                var cs = System.Configuration.ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(cs))
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT TOP 1 ShopName FROM dbo.Seller WHERE SellerID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", sellerId);
                    conn.Open();
                    var v = cmd.ExecuteScalar();
                    var s = v == null || v == DBNull.Value ? "" : v.ToString();
                    return string.IsNullOrWhiteSpace(s) ? "EcoEats" : s;
                }
            }
            catch { }
            return "EcoEats";
        }

        public class ProductCardVm
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string Subtitle { get; set; }
            public string ImageUrl { get; set; }
            public decimal PriceNow { get; set; }
            public double Rating { get; set; }
            public int Reviews { get; set; }
            public double? DistanceKm { get; set; }
            public int DiscountPercent { get; set; }
            public string Category { get; set; }
            public string StoreName { get; set; }
        }
    }
}