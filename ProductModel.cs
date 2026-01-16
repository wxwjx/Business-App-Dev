using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public class ProductModel
    {
        // ====== Columns / Properties ======
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal PriceNow { get; set; }
        public decimal PriceOld { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public double DistanceKm { get; set; }
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

        // ====== Helper: Map SQL row → ProductModel ======
        private static ProductModel ReadProduct(SqlDataReader r)
        {
            return new ProductModel
            {
                ProductID = r["ProductID"] != DBNull.Value ? Convert.ToInt32(r["ProductID"]) : 0,
                ProductName = r["ProductName"]?.ToString() ?? "",
                Subtitle = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "",
                ImageUrl = r["ImageUrl"] != DBNull.Value ? r["ImageUrl"].ToString() : "",

                PriceNow = r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0m,
                PriceOld = r["PriceOld"] != DBNull.Value ? Convert.ToDecimal(r["PriceOld"]) : 0m,

                Rating = r["Rating"] != DBNull.Value ? Convert.ToDouble(r["Rating"]) : 0,
                Reviews = r["Reviews"] != DBNull.Value ? Convert.ToInt32(r["Reviews"]) : 0,

                DistanceKm = r["DistanceKm"] != DBNull.Value ? Convert.ToDouble(r["DistanceKm"]) : 0,

                ExpiryHours = r["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(r["ExpiryHours"]) : 0,
                CO2Saved = r["CO2Saved"] != DBNull.Value ? Convert.ToDouble(r["CO2Saved"]) : 0,

                DiscountPercent = r["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(r["DiscountPercent"]) : 0,
                Quantity = r["Quantity"] != DBNull.Value ? Convert.ToInt32(r["Quantity"]) : 0,
                Category = r["Category"] != DBNull.Value ? r["Category"].ToString() : "",
                CreatedAt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now,
                SellerID = r["SellerID"] != DBNull.Value ? Convert.ToInt32(r["SellerID"]) : 0
            };
        }

        // =========================================================
        // BACKWARD COMPAT (so your other pages won't break)
        // =========================================================
        public static List<ProductModel> GetAllProducts()
        {
            return GetProductsBySearch("");
        }

        public static List<ProductModel> GetAllProductsWithDistance(double userLat, double userLng)
        {
            return GetProductsWithDistanceAndSearch(userLat, userLng, "");
        }

        // =========================================================
        // AI MODE: Search (product/subtitle/category/shop) - no location
        // =========================================================
        public static List<ProductModel> GetProductsBySearch(string keyword)
        {
            var list = new List<ProductModel>();
            keyword = (keyword ?? "").Trim();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    ISNULL(CAST(p.DistanceKm AS FLOAT), 0) AS DistanceKm,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle    LIKE '%' + @kw + '%' OR
     p.Category    LIKE '%' + @kw + '%' OR
     s.ShopName    LIKE '%' + @kw + '%')
ORDER BY p.DiscountPercent DESC, p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@kw", keyword);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            return list;
        }

        // =========================================================
        // AI MODE: Search + Distance
        // =========================================================
        public static List<ProductModel> GetProductsWithDistanceAndSearch(double userLat, double userLng, string keyword)
        {
            var list = new List<ProductModel>();
            keyword = (keyword ?? "").Trim();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID,

    (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
    )) AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    s.Latitude IS NOT NULL AND s.Longitude IS NOT NULL
    AND
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle    LIKE '%' + @kw + '%' OR
     p.Category    LIKE '%' + @kw + '%' OR
     s.ShopName    LIKE '%' + @kw + '%')
ORDER BY DistanceKm ASC, p.DiscountPercent DESC, p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@lat", userLat);
                cmd.Parameters.AddWithValue("@lng", userLng);
                cmd.Parameters.AddWithValue("@kw", keyword);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            return list;
        }

        // =========================================================
        // DAILY BEST DEALS: Most bought today pinned + best discount/cheap
        // =========================================================
        private static int? GetMostBoughtProductIdToday()
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 oi.ProductID
FROM dbo.OrderItems oi
INNER JOIN dbo.Orders o ON o.OrderID = oi.OrderID
WHERE CAST(o.CreatedAt AS date) = CAST(GETDATE() AS date)
  AND o.PayStatus = 'PAID'
GROUP BY oi.ProductID
ORDER BY SUM(oi.Quantity) DESC;", conn))
            {
                conn.Open();
                object val = cmd.ExecuteScalar();
                if (val == null || val == DBNull.Value) return null;
                return Convert.ToInt32(val);
            }
        }

        public static List<ProductModel> GetDailyBestDeals(string keyword)
        {
            var list = new List<ProductModel>();
            keyword = (keyword ?? "").Trim();
            int? topId = GetMostBoughtProductIdToday();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    ISNULL(CAST(p.DistanceKm AS FLOAT), 0) AS DistanceKm,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle    LIKE '%' + @kw + '%' OR
     p.Category    LIKE '%' + @kw + '%' OR
     s.ShopName    LIKE '%' + @kw + '%')
ORDER BY
    CASE WHEN p.ProductID = @topId THEN 0 ELSE 1 END,
    p.DiscountPercent DESC,
    p.Price ASC,
    p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@kw", keyword);
                cmd.Parameters.AddWithValue("@topId", (object)topId ?? DBNull.Value);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            if (list.Count > 24) list = list.GetRange(0, 24);
            return list;
        }

        public static List<ProductModel> GetDailyBestDealsWithDistance(double userLat, double userLng, string keyword)
        {
            var list = new List<ProductModel>();
            keyword = (keyword ?? "").Trim();
            int? topId = GetMostBoughtProductIdToday();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID,

    (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
    )) AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    s.Latitude IS NOT NULL AND s.Longitude IS NOT NULL
    AND
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle    LIKE '%' + @kw + '%' OR
     p.Category LIKE '%' + @kw + '%' OR
     s.ShopName LIKE '%' + @kw + '%')
ORDER BY
    CASE WHEN p.ProductID = @topId THEN 0 ELSE 1 END,
    p.DiscountPercent DESC,
    p.Price ASC,
    p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@lat", userLat);
                cmd.Parameters.AddWithValue("@lng", userLng);
                cmd.Parameters.AddWithValue("@kw", keyword);
                cmd.Parameters.AddWithValue("@topId", (object)topId ?? DBNull.Value);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            if (list.Count > 24) list = list.GetRange(0, 24);
            return list;
        }

        // =========================================================
        // CATEGORIES
        // =========================================================
        public static List<string> GetCategories()
        {
            var list = new List<string>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT DISTINCT Category
FROM dbo.Products
WHERE Category IS NOT NULL AND LTRIM(RTRIM(Category)) <> ''
ORDER BY Category ASC;", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(r["Category"].ToString());
                }
            }

            return list;
        }

        public static List<ProductModel> GetProductsByCategory(string category, string keyword)
        {
            var list = new List<ProductModel>();
            category = (category ?? "").Trim();
            keyword = (keyword ?? "").Trim();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    ISNULL(CAST(p.DistanceKm AS FLOAT), 0) AS DistanceKm,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    (@cat = '' OR p.Category = @cat)
    AND
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle LIKE '%' + @kw + '%' OR
     s.ShopName LIKE '%' + @kw + '%')
ORDER BY p.DiscountPercent DESC, p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@cat", category);
                cmd.Parameters.AddWithValue("@kw", keyword);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            return list;
        }

        public static List<ProductModel> GetProductsByCategoryWithDistance(double userLat, double userLng, string category, string keyword)
        {
            var list = new List<ProductModel>();
            category = (category ?? "").Trim();
            keyword = (keyword ?? "").Trim();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    p.ProductID, p.ProductName, p.Subtitle, p.ImageUrl,
    p.Price, p.PriceOld, p.Rating, p.Reviews,
    p.ExpiryHours, p.CO2Saved, p.DiscountPercent, p.Quantity,
    p.Category, p.CreatedAt, p.SellerID,

    (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
    )) AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    s.Latitude IS NOT NULL AND s.Longitude IS NOT NULL
    AND (@cat = '' OR p.Category = @cat)
    AND
    (@kw = '' OR
     p.ProductName LIKE '%' + @kw + '%' OR
     p.Subtitle LIKE '%' + @kw + '%' OR
     s.ShopName LIKE '%' + @kw + '%')
ORDER BY DistanceKm ASC, p.DiscountPercent DESC, p.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@lat", userLat);
                cmd.Parameters.AddWithValue("@lng", userLng);
                cmd.Parameters.AddWithValue("@cat", category);
                cmd.Parameters.AddWithValue("@kw", keyword);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadProduct(r));
                }
            }

            return list;
        }

        // =========================================================
        // CRUD (KEEP - used by Inventory/AddNewProduct)
        // =========================================================
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
