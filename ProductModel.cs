using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public class ProductModel
    {
        // ====== Columns / Properties (your main model) ======
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal PriceNow { get; set; }
        public decimal PriceOld { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int DistanceKm { get; set; }
        public int ExpiryHours { get; set; }
        public double CO2Saved { get; set; }
        public int DiscountPercent { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int SellerID { get; set; }

        // ====== Connection String ======
        private static string ConnStr =>
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        // ====== READ: Get all products ======
        public static List<ProductModel> GetAllProducts()
        {
            var list = new List<ProductModel>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProductModel
                        {
                            ProductID = (int)reader["ProductID"],
                            ProductName = reader["ProductName"].ToString(),
                            Subtitle = reader["Subtitle"] != DBNull.Value ? reader["Subtitle"].ToString() : "",
                            ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "",
                            PriceNow = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m,
                            PriceOld = reader["PriceOld"] != DBNull.Value ? Convert.ToDecimal(reader["PriceOld"]) : 0m,
                            Rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0.0,
                            Reviews = reader["Reviews"] != DBNull.Value ? Convert.ToInt32(reader["Reviews"]) : 0,
                            DistanceKm = reader["DistanceKm"] != DBNull.Value ? Convert.ToInt32(reader["DistanceKm"]) : 0,
                            ExpiryHours = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0,
                            CO2Saved = reader["CO2Saved"] != DBNull.Value ? Convert.ToDouble(reader["CO2Saved"]) : 0.0,
                            DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0,
                            Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                            Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "",
                            CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now

                            // If you have SellerID:
                            // SellerID = reader["SellerID"] != DBNull.Value ? Convert.ToInt32(reader["SellerID"]) : 0,
                        });
                    }
                }
            }

            return list;
        }

        // ====== DELETE: Delete by ProductID ======
        public static int DeleteProduct(int productID)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", productID);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public static int AddProduct(ProductModel p)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                string query = @"
INSERT INTO Products
(SellerID, ProductName, Subtitle, ImageUrl, Price, PriceOld, Rating, Reviews, DistanceKm, ExpiryHours, CO2Saved, DiscountPercent, Quantity, Category, CreatedAt)
VALUES
(@SellerID, @Name, @Subtitle, @ImageUrl, @Price, @PriceOld, @Rating, @Reviews, @DistanceKm, @ExpiryHours, @CO2Saved, @DiscountPercent, @Quantity, @Category, @CreatedAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SellerID", p.SellerID);
                    cmd.Parameters.AddWithValue("@Name", p.ProductName ?? "");
                    cmd.Parameters.AddWithValue("@Subtitle", (object)(p.Subtitle ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)(p.ImageUrl ?? "") ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Price", p.PriceNow);
                    cmd.Parameters.AddWithValue("@PriceOld", p.PriceOld);
                    cmd.Parameters.AddWithValue("@Rating", p.Rating);
                    cmd.Parameters.AddWithValue("@Reviews", p.Reviews);
                    cmd.Parameters.AddWithValue("@DistanceKm", p.DistanceKm);
                    cmd.Parameters.AddWithValue("@ExpiryHours", p.ExpiryHours);
                    cmd.Parameters.AddWithValue("@CO2Saved", p.CO2Saved);
                    cmd.Parameters.AddWithValue("@DiscountPercent", p.DiscountPercent);
                    cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("@Category", p.Category ?? "");
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // ====== UPDATE: Update product ======
        public static int UpdateProduct(ProductModel p)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                string query = @"
UPDATE Products
SET ProductName=@Name,
    Subtitle=@Subtitle,
    ImageUrl=@ImageUrl,
    Price=@Price,
    PriceOld=@PriceOld,
    Rating=@Rating,
    Reviews=@Reviews,
    DistanceKm=@DistanceKm,
    ExpiryHours=@ExpiryHours,
    CO2Saved=@CO2Saved,
    DiscountPercent=@DiscountPercent,
    Quantity=@Quantity,
    Category=@Category
WHERE ProductID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", p.ProductID);
                    cmd.Parameters.AddWithValue("@Name", p.ProductName ?? "");
                    cmd.Parameters.AddWithValue("@Subtitle", (object)(p.Subtitle ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)(p.ImageUrl ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", p.PriceNow);
                    cmd.Parameters.AddWithValue("@PriceOld", p.PriceOld);
                    cmd.Parameters.AddWithValue("@Rating", p.Rating);
                    cmd.Parameters.AddWithValue("@Reviews", p.Reviews);
                    cmd.Parameters.AddWithValue("@DistanceKm", p.DistanceKm);
                    cmd.Parameters.AddWithValue("@ExpiryHours", p.ExpiryHours);
                    cmd.Parameters.AddWithValue("@CO2Saved", p.CO2Saved);
                    cmd.Parameters.AddWithValue("@DiscountPercent", p.DiscountPercent);
                    cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("@Category", p.Category ?? "");

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
