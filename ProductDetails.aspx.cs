using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        private int CurrentProductId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtQty.Text = "1";
                if (CurrentProductId > 0)
                    LoadProduct(CurrentProductId);
            }
        }

        private void LoadProduct(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;

                    lblName.Text = reader["ProductName"].ToString();
                    lblSubtitle.Text = reader["Subtitle"] != DBNull.Value ? reader["Subtitle"].ToString() : "Fresh Market";

                    // image
                    imgProduct.ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "";

                    // numbers
                    decimal priceNow = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0;
                    decimal priceOld = reader["PriceOld"] != DBNull.Value ? (decimal)reader["PriceOld"] : 0;
                    double rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0;
                    int reviews = reader["Reviews"] != DBNull.Value ? Convert.ToInt32(reader["Reviews"]) : 0;
                    int distance = reader["DistanceKm"] != DBNull.Value ? Convert.ToInt32(reader["DistanceKm"]) : 0;
                    int expiry = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0;
                    double co2 = reader["CO2Saved"] != DBNull.Value ? Convert.ToDouble(reader["CO2Saved"]) : 0;
                    int discount = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0;

                    lblPriceNow.Text = priceNow.ToString("0.00");
                    lblPriceOld.Text = priceOld.ToString("0.00");
                    lblSave.Text = Math.Max(0, priceOld - priceNow).ToString("0.00");

                    lblRating.Text = rating.ToString("0.0");
                    lblReviews.Text = reviews.ToString();
                    lblDistance.Text = distance.ToString();
                    lblExpiry.Text = expiry.ToString();
                    lblCO2.Text = co2.ToString("0.0");
                    lblDiscount.Text = discount.ToString();

                    // description (if you don't have column, use subtitle)
                    lblDescription.Text = reader["Subtitle"] != DBNull.Value
                        ? reader["Subtitle"].ToString()
                        : "Fresh seasonal food at amazing prices.";
                }
            }
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;
            qty = Math.Max(1, qty - 1);
            txtQty.Text = qty.ToString();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;
            qty = Math.Min(99, qty + 1);
            txtQty.Text = qty.ToString();
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Placeholder: you can connect this to your Cart/Session later
            // For now: redirect to Order or Cart page
            Response.Redirect("Cart.aspx");
        }
    }
}
