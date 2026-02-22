using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;

namespace Business_App_Dev.Pages
{
    public partial class ProductPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ViewState["tab"] == null) ViewState["tab"] = "recommended";
            }

            BindProducts();
        }

        private void BindProducts()
        {
            string tab = (ViewState["tab"] as string) ?? "recommended";
            string keyword = (txtSearch.Text ?? "").Trim();

            double? lat = TryParseDouble(hfLat.Value);
            double? lng = TryParseDouble(hfLng.Value);

            int userId = GetUserId();

            List<ProductModel> rows;

            if (tab == "deals")
            {
                if (lat.HasValue && lng.HasValue && lat.Value != 0 && lng.Value != 0)
                    rows = ProductModel.GetDailyBestDealsWithDistance(lat.Value, lng.Value, keyword);
                else
                    rows = ProductModel.GetDailyBestDeals(keyword);
            }
            else if (tab == "categories")
            {
                if (lat.HasValue && lng.HasValue && lat.Value != 0 && lng.Value != 0)
                    rows = ProductModel.GetProductsWithDistanceAndSearch(lat.Value, lng.Value, keyword);
                else
                    rows = ProductModel.GetProductsBySearch(keyword);

                rows = rows
                    .OrderBy(x => x.Category ?? "")
                    .ThenByDescending(x => x.DiscountPercent)
                    .ThenBy(x => x.DistanceKm)
                    .ThenByDescending(x => x.CreatedAt)
                    .ToList();
            }
            else
            {
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
                PriceOld = p.PriceOld,
                Rating = p.Rating,
                Reviews = p.Reviews,
                DistanceKm = (p.DistanceKm >= 9999) ? (double?)null : p.DistanceKm,
                ExpiryHours = p.ExpiryHours,
                CO2Saved = p.CO2Saved,
                DiscountPercent = p.DiscountPercent,
                Quantity = p.Quantity,
                Category = p.Category ?? "",
                CreatedAt = p.CreatedAt,
                SellerID = p.SellerID,

                StoreName = GetShopNameFallback(p)
            }).ToList();

            rptProducts.DataSource = vm;
            rptProducts.DataBind();
        }

        private static string GetShopNameFallback(ProductModel p)
        {
            var prop = p.GetType().GetProperty("ShopName");
            if (prop != null)
            {
                var v = prop.GetValue(p, null);
                var s = v == null ? "" : v.ToString();
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }

            var prop2 = p.GetType().GetProperty("StoreName");
            if (prop2 != null)
            {
                var v = prop2.GetValue(p, null);
                var s = v == null ? "" : v.ToString();
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }

            return "EcoEats";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindProducts();
        }

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
            BindProducts();
        }

        protected void btnGeoRefresh_Click(object sender, EventArgs e)
        {
            BindProducts();
        }

        private int GetUserId()
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    int id;
                    if (int.TryParse(Session["UserID"].ToString(), out id)) return id;
                }
                if (Session["UserId"] != null)
                {
                    int id;
                    if (int.TryParse(Session["UserId"].ToString(), out id)) return id;
                }
                if (Session["MemberID"] != null)
                {
                    int id;
                    if (int.TryParse(Session["MemberID"].ToString(), out id)) return id;
                }
            }
            catch { }
            return 0;
        }

        private static double? TryParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            double v;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v)) return v;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v)) return v;
            return null;
        }

        public class ProductCardVm
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string Subtitle { get; set; }
            public string ImageUrl { get; set; }
            public decimal PriceNow { get; set; }
            public decimal PriceOld { get; set; }
            public double Rating { get; set; }
            public int Reviews { get; set; }
            public double? DistanceKm { get; set; }
            public int ExpiryHours { get; set; }
            public double CO2Saved { get; set; }
            public int DiscountPercent { get; set; }
            public int Quantity { get; set; }
            public string Category { get; set; }
            public DateTime CreatedAt { get; set; }
            public int SellerID { get; set; }

            public string StoreName { get; set; }
        }
    }
}