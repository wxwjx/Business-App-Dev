using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

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

        private static double Clamp01(double v)
        {
            if (v < 0) return 0;
            if (v > 1) return 1;
            return v;
        }

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
                p.DistanceKm = 9999;

            return p;
        }

        public static List<ProductModel> GetProductBySeller(int sellerId)
        {
            var list = new List<ProductModel>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM Products WHERE SellerID = @SellerID ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(ReadProduct(r));
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
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
    CASE
      WHEN s.Latitude IS NULL OR s.Longitude IS NULL THEN 9999
      ELSE (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
      ))
    END AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
            }

            return list;
        }

        public static List<ProductModel> GetAIRecommended(int userId, double userLat, double userLng, string keyword)
        {
            var candidates = (userLat != 0 && userLng != 0)
                ? (GetProductsWithDistanceAndSearch(userLat, userLng, keyword) ?? new List<ProductModel>())
                : (GetProductsBySearch(keyword) ?? new List<ProductModel>());

            if (candidates.Count > 250) candidates = candidates.Take(250).ToList();

            var popMap = (userLat != 0 && userLng != 0)
                ? GetLocalPopularityMap(userLat, userLng, radiusKm: 3.0, days: 30)
                : new Dictionary<int, double>();

            foreach (var p in candidates)
                p.LocalPopularity01 = popMap.TryGetValue(p.ProductID, out var pop01) ? pop01 : 0;

            // Guest: trending + deals + distance
            if (userId <= 0)
            {
                foreach (var p in candidates)
                {
                    double dist01 = (userLat != 0 && userLng != 0)
                        ? Clamp01((p.DistanceKm <= 0 ? 5 : p.DistanceKm) / 5.0)
                        : 0;

                    double deal01 = Clamp01(p.DiscountPercent / 60.0);

                    p.AIScore =
                        (0.70 * p.LocalPopularity01) +
                        (0.30 * deal01) -
                        (0.20 * dist01);
                }

                return candidates
                    .OrderByDescending(x => x.AIScore)
                    .ThenBy(x => x.DistanceKm)
                    .Take(24)
                    .ToList();
            }

            // Logged in: personal signals
            var userSignals = GetUserSignals(userId);
            var coBuyBoost = GetCoBuyBoostMap(userId, lookbackOrders: 20);

            var alreadyBought = new HashSet<int>(userSignals.PastProductIds);

            foreach (var p in candidates)
            {
                double dist01 = (userLat != 0 && userLng != 0)
                    ? Clamp01((p.DistanceKm <= 0 ? 5 : p.DistanceKm) / 5.0)
                    : 0;

                double deal01 = Clamp01(p.DiscountPercent / 60.0);

                double catBoost01 = userSignals.TopCategories.Contains(p.Category ?? "") ? 1.0 : 0.0;
                double sellerBoost01 = userSignals.TopSellers.Contains(p.SellerID) ? 1.0 : 0.0;

                double cobuy01 = coBuyBoost.TryGetValue(p.ProductID, out var cb) ? cb : 0.0;

                double novelty = alreadyBought.Contains(p.ProductID) ? 0.15 : 1.0;

                p.AIScore =
                    (0.35 * p.LocalPopularity01) +
                    (0.18 * deal01) +
                    (0.18 * catBoost01) +
                    (0.15 * sellerBoost01) +
                    (0.25 * cobuy01) +
                    (0.08 * novelty) -
                    (0.20 * dist01);
            }

            return candidates
                .OrderByDescending(x => x.AIScore)
                .ThenBy(x => x.DistanceKm)
                .Take(24)
                .ToList();
        }

        private class UserSignals
        {
            public List<int> PastProductIds = new List<int>();
            public HashSet<string> TopCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public HashSet<int> TopSellers = new HashSet<int>();
        }

        private static UserSignals GetUserSignals(int userId)
        {
            var s = new UserSignals();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
;WITH Past AS (
    SELECT TOP (80)
        oi.ProductID,
        p.Category,
        p.SellerID,
        o.CreatedAt
    FROM dbo.Orders o
    INNER JOIN dbo.OrderItems oi ON oi.OrderID = o.OrderID
    INNER JOIN dbo.Products p ON p.ProductID = oi.ProductID
    WHERE o.UserID = @uid AND o.PayStatus = 'PAID'
    ORDER BY o.CreatedAt DESC
)
SELECT ProductID FROM Past;

;WITH CatAgg AS (
    SELECT TOP (3) Category, COUNT(*) cnt
    FROM (
        SELECT TOP (80) p.Category
        FROM dbo.Orders o
        INNER JOIN dbo.OrderItems oi ON oi.OrderID = o.OrderID
        INNER JOIN dbo.Products p ON p.ProductID = oi.ProductID
        WHERE o.UserID = @uid AND o.PayStatus = 'PAID'
        ORDER BY o.CreatedAt DESC
    ) x
    WHERE Category IS NOT NULL AND LTRIM(RTRIM(Category)) <> ''
    GROUP BY Category
    ORDER BY cnt DESC
)
SELECT Category FROM CatAgg;

;WITH SellerAgg AS (
    SELECT TOP (3) SellerID, COUNT(*) cnt
    FROM (
        SELECT TOP (80) p.SellerID
        FROM dbo.Orders o
        INNER JOIN dbo.OrderItems oi ON oi.OrderID = o.OrderID
        INNER JOIN dbo.Products p ON p.ProductID = oi.ProductID
        WHERE o.UserID = @uid AND o.PayStatus = 'PAID'
        ORDER BY o.CreatedAt DESC
    ) x
    WHERE SellerID IS NOT NULL
    GROUP BY SellerID
    ORDER BY cnt DESC
)
SELECT SellerID FROM SellerAgg;
", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        if (r[0] != DBNull.Value) s.PastProductIds.Add(Convert.ToInt32(r[0]));

                    if (r.NextResult())
                        while (r.Read())
                            if (r[0] != DBNull.Value) s.TopCategories.Add(r[0].ToString());

                    if (r.NextResult())
                        while (r.Read())
                            if (r[0] != DBNull.Value) s.TopSellers.Add(Convert.ToInt32(r[0]));
                }
            }

            return s;
        }

        private static Dictionary<int, double> GetCoBuyBoostMap(int userId, int lookbackOrders)
        {
            var raw = new Dictionary<int, int>();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
;WITH RecentOrders AS (
    SELECT TOP (@n) o.OrderID
    FROM dbo.Orders o
    WHERE o.UserID = @uid AND o.PayStatus = 'PAID'
    ORDER BY o.CreatedAt DESC
),
SeedProducts AS (
    SELECT DISTINCT oi.ProductID
    FROM dbo.OrderItems oi
    INNER JOIN RecentOrders ro ON ro.OrderID = oi.OrderID
),
Co AS (
    SELECT oi2.ProductID, COUNT(*) cnt
    FROM dbo.OrderItems oi2
    INNER JOIN dbo.Orders o2 ON o2.OrderID = oi2.OrderID
    WHERE o2.PayStatus = 'PAID'
      AND EXISTS (
          SELECT 1
          FROM dbo.OrderItems oi1
          WHERE oi1.OrderID = oi2.OrderID
            AND oi1.ProductID IN (SELECT ProductID FROM SeedProducts)
      )
    GROUP BY oi2.ProductID
)
SELECT TOP 40 ProductID, cnt
FROM Co
ORDER BY cnt DESC;
", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@n", lookbackOrders);

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int pid = r[0] != DBNull.Value ? Convert.ToInt32(r[0]) : 0;
                        int cnt = r[1] != DBNull.Value ? Convert.ToInt32(r[1]) : 0;
                        if (pid > 0) raw[pid] = cnt;
                    }
                }
            }

            int max = raw.Count == 0 ? 0 : raw.Values.Max();
            var norm = new Dictionary<int, double>();
            if (max <= 0) return norm;

            foreach (var kv in raw)
                norm[kv.Key] = (double)kv.Value / max;

            return norm;
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
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
    CASE
      WHEN s.Latitude IS NULL OR s.Longitude IS NULL THEN 9999
      ELSE (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
      ))
    END AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
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
                    while (r.Read())
                        list.Add(r["Category"].ToString());
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
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
    CASE
      WHEN s.Latitude IS NULL OR s.Longitude IS NULL THEN 9999
      ELSE (6371 * ACOS(
        COS(RADIANS(@lat)) *
        COS(RADIANS(s.Latitude)) *
        COS(RADIANS(s.Longitude) - RADIANS(@lng)) +
        SIN(RADIANS(@lat)) *
        SIN(RADIANS(s.Latitude))
      ))
    END AS DistanceKm
FROM dbo.Products p
INNER JOIN dbo.Seller s ON s.SellerID = p.SellerID
WHERE
    (@cat = '' OR p.Category = @cat)
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
                    while (r.Read())
                        list.Add(ReadProduct(r));
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