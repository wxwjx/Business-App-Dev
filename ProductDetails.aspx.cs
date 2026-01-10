using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    LoadProduct(id);
                }
            }
        }

        private void LoadProduct(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblName.Text = reader["ProductName"].ToString();
                            lblSubtitle.Text = reader["Subtitle"] != DBNull.Value ? reader["Subtitle"].ToString() : "";

                            imgProduct.ImageUrl = reader["ImageUrl"] != DBNull.Value
                                ? reader["ImageUrl"].ToString()
                                : "";

                            decimal priceNow = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0;
                            decimal priceOld = reader["PriceOld"] != DBNull.Value ? (decimal)reader["PriceOld"] : 0;

                            lblPriceNow.Text = priceNow.ToString("0.00");
                            lblPriceOld.Text = priceOld.ToString("0.00");

                            double rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0;
                            int reviews = reader["Reviews"] != DBNull.Value ? Convert.ToInt32(reader["Reviews"]) : 0;
                            int distance = reader["DistanceKm"] != DBNull.Value ? Convert.ToInt32(reader["DistanceKm"]) : 0;
                            int expiry = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0;

                            lblRating.Text = rating.ToString("0.0");
                            lblReviews.Text = reviews.ToString();
                            lblDistance.Text = distance.ToString();
                            lblExpiry.Text = expiry.ToString();
                        }
                    }
                }
            }
        }
    }
}
