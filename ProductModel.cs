using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public class ProductModel
    {
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

        public double AIScore { get; set; }
        public double LocalPopularity01 { get; set; }

        private static string ConnStr =>
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        private static bool HasCol(SqlDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static ProductModel ReadProduct(SqlDataReader r)
        {
            var p = new ProductModel
            {
                ProductID = r["ProductID"] != DBNull.Value ? Convert.ToInt32(r["ProductID"]) : 0,
                ProductName = r["ProductName"]?.ToString() ?? "",
                Subtitle = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "",
                ImageUrl = r["ImageUrl"] != DBNull.Value ? r["ImageUrl"].ToString() : "",

                PriceNow = r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0m,
                PriceOld = r["PriceOld"] != DBNull.Value ? Convert.ToDecimal(r["PriceOld"]) : 0m,

                Rating = r["Rating"] != DBNull.Value ? Convert.ToDouble(r["Rating"]) : 0,
                Reviews = r["Reviews"] != DBNull.Value ? Convert.ToInt32(r["Reviews"]) : 0,

                ExpiryHours = r["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(r["ExpiryHours"]) : 0,
                CO2Saved = r["CO2Saved"] != DBNull.Value ? Convert.ToDouble(r["CO2Saved"]) : 0,

                DiscountPercent = r["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(r["DiscountPercent"]) : 0,
                Quantity = r["Quantity"] != DBNull.Value ? Convert.ToInt32(r["Quantity"]) : 0,
                Category = r["Category"] != DBNull.Value ? r["Category"].ToString() : "",
                CreatedAt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now,
                SellerID = r["SellerID"] != DBNull.Value ? Convert.ToInt32(r["SellerID"]) : 0
            };

            if (HasCol(r, "DistanceKm") && r["DistanceKm"] != DBNull.Value)
                p.DistanceKm = Convert.ToDouble(r["DistanceKm"]);
            else
                p.DistanceKm = 0;

            return p;
        }

        public static List<ProductModel> GetProductBySeller(int SellerId)
        {
            var list = new List<ProductModel>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM Products WHERE SellerID = @SellerID", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", SellerId);
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
                            DistanceKm = reader["DistanceKm"] != DBNull.Value ? Convert.ToDouble(reader["DistanceKm"]) : 0,
                            ExpiryHours = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0,
                            CO2Saved = reader["CO2Saved"] != DBNull.Value ? Convert.ToDouble(reader["CO2Saved"]) : 0.0,
                            DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0,
                            Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0,
                            Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "",
                            CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now,
                            SellerID = reader["SellerID"] != DBNull.Value ? Convert.ToInt32(reader["SellerID"]) : 0
                        });
                    }
                }
            }

            return list;
        }

        public static List<ProductModel> GetAllProducts() => GetProductsBySearch("");
        public static List<ProductModel> GetAllProductsWithDistance(double userLat, double userLng) => GetProductsWithDistanceAndSearch(userLat, userLng, "");

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

        public static List<ProductModel> GetAIRecommended(int userId, double userLat, double userLng, string keyword)
        {
            var candidates = GetProductsWithDistanceAndSearch(userLat, userLng, keyword);
            if (candidates == null) candidates = new List<ProductModel>();
            if (candidates.Count > 200) candidates = candidates.Take(200).ToList();

            var popMap = GetLocalPopularityMap(userLat, userLng, radiusKm: 3.0, days: 30);

            foreach (var p in candidates)
                p.LocalPopularity01 = popMap.TryGetValue(p.ProductID, out var pop01) ? pop01 : 0;

            if (userId <= 0)
            {
                foreach (var p in candidates)
                {
                    double dist01 = VectorMath.Clamp01((p.DistanceKm <= 0 ? 5 : p.DistanceKm) / 5.0);
                    double deal01 = VectorMath.Clamp01((p.DiscountPercent) / 60.0);

                    p.AIScore =
                        (0.65 * p.LocalPopularity01) +
                        (0.25 * deal01) -
                        (0.20 * dist01);
                }

                return candidates
                    .OrderByDescending(x => x.AIScore)
                    .Take(24)
                    .ToList();
            }

            var pastIds = GetUserPastPurchasedProductIds(userId, maxItems: 50);

            var allIds = candidates.Select(p => p.ProductID)
                                   .Concat(pastIds)
                                   .Distinct()
                                   .ToList();

            var emb = EmbeddingStore.GetProductEmbeddings(ConnStr, allIds);

            var userVecs = pastIds.Where(id => emb.ContainsKey(id)).Select(id => emb[id]).ToList();
            var userTaste = VectorMath.Average(userVecs);

            bool hasUserTaste = userTaste != null && userTaste.Length > 0;

            var alreadyBought = new HashSet<int>(pastIds);

            foreach (var p in candidates)
            {
                double sim = 0;
                if (hasUserTaste && emb.TryGetValue(p.ProductID, out var pv))
                    sim = VectorMath.Cosine(userTaste, pv);

                double dist01 = VectorMath.Clamp01((p.DistanceKm <= 0 ? 5 : p.DistanceKm) / 5.0);
                double deal01 = VectorMath.Clamp01((p.DiscountPercent) / 60.0);
                double novelty = alreadyBought.Contains(p.ProductID) ? 0.0 : 1.0;

                p.AIScore =
                    (hasUserTaste ? (0.55 * sim) : 0.0) +
                    (0.30 * p.LocalPopularity01) +
                    (0.15 * deal01) +
                    (0.05 * novelty) -
                    (0.20 * dist01);
            }

            return candidates
                .OrderByDescending(x => x.AIScore)
                .Take(24)
                .ToList();
        }

        public static List<int> GetUserPastPurchasedProductIds(int userId, int maxItems)
        {
            var list = new List<int>();
            if (userId <= 0) return list;

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP (@maxItems) oi.ProductID
FROM dbo.Orders o
INNER JOIN dbo.OrderItems oi ON o.OrderID = oi.OrderID
WHERE o.UserId = @uid
  AND o.PayStatus = 'PAID'
ORDER BY o.CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@maxItems", maxItems);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        if (r["ProductID"] != DBNull.Value)
                            list.Add(Convert.ToInt32(r["ProductID"]));
                    }
                }
            }

            return list;
        }

        public static Dictionary<int, double> GetLocalPopularityMap(double userLat, double userLng, double radiusKm, int days)
        {
            var raw = new Dictionary<int, int>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
WITH NearbyProducts AS (
    SELECT p.ProductID
    FROM dbo.Products p
    INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
    WHERE s.Latitude IS NOT NULL AND s.Longitude IS NOT NULL
      AND (6371 * ACOS(
            COS(RADIANS(@lat)) *
            COS(RADIANS(s.Latitude)) *
            COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
            SIN(RADIANS(@lat)) *
            SIN(RADIANS(s.Latitude))
          )) <= @radiusKm
)
SELECT oi.ProductID, SUM(oi.Quantity) AS Qty
FROM dbo.OrderItems oi
INNER JOIN dbo.Orders o ON o.OrderID = oi.OrderID
INNER JOIN NearbyProducts np ON np.ProductID = oi.ProductID
WHERE o.PayStatus = 'PAID'
  AND o.CreatedAt >= DATEADD(day, -@days, GETDATE())
GROUP BY oi.ProductID;", conn))
            {
                cmd.Parameters.AddWithValue("@lat", userLat);
                cmd.Parameters.AddWithValue("@lng", userLng);
                cmd.Parameters.AddWithValue("@radiusKm", radiusKm);
                cmd.Parameters.AddWithValue("@days", days);

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int pid = r["ProductID"] != DBNull.Value ? Convert.ToInt32(r["ProductID"]) : 0;
                        int qty = r["Qty"] != DBNull.Value ? Convert.ToInt32(r["Qty"]) : 0;
                        if (pid > 0) raw[pid] = qty;
                    }
                }
            }

            int maxQty = raw.Count == 0 ? 0 : raw.Values.Max();
            var norm = new Dictionary<int, double>();
            if (maxQty <= 0) return norm;

            foreach (var kv in raw)
                norm[kv.Key] = (double)kv.Value / maxQty;

            return norm;
        }

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

        public static List<ProductModel> SearchProducts(string keyword, string scope = "ALL")
        {
            return GetProductsBySearch(keyword);
        }

        public static List<ProductModel> SearchProductsWithDistance(double lat, double lng, string keyword, string scope = "ALL")
        {
            return GetProductsWithDistanceAndSearch(lat, lng, keyword);
        }

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