using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class Product : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadProducts();
        }

        private void LoadProducts()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
            List<ProductModel> products = new List<ProductModel>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Products";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductModel
                            {
                                ProductID = (int)reader["ProductID"],
                                ProductName = reader["ProductName"].ToString(),
                                Subtitle = reader["Subtitle"] != DBNull.Value ? reader["Subtitle"].ToString() : "",
                                ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "",

                                // DB column is Price, map to PriceNow
                                PriceNow = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
                                PriceOld = reader["PriceOld"] != DBNull.Value ? (decimal)reader["PriceOld"] : 0,

                                Rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0,
                                Reviews = reader["Reviews"] != DBNull.Value ? Convert.ToInt32(reader["Reviews"]) : 0,
                                DistanceKm = reader["DistanceKm"] != DBNull.Value ? Convert.ToInt32(reader["DistanceKm"]) : 0,
                                ExpiryHours = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0,
                                CO2Saved = reader["CO2Saved"] != DBNull.Value ? Convert.ToDouble(reader["CO2Saved"]) : 0,
                                DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0,
                                Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                                Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "",
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now
                            });
                        }
                    }
                }
            }

            ProductRepeater.DataSource = products;
            ProductRepeater.DataBind();
        }
    }
}
