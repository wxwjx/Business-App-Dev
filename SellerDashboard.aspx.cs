using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class SalesDashboard : Page
    {
        //private static string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            //            if (Session["UserRole"]?.ToString() != "Seller" || Session["SellerId"] == null)
            //            {
            //                Response.Redirect("~/Login.aspx?role=Seller");
            //                return;
            //            }

            //            if (!IsPostBack)
            //            {
            //                // default range: 30 days
            //                ViewState["days"] = 30;
            //                LoadDashboard(30);
            //}
        }
    }

//        protected void Range_Click(object sender, EventArgs e)
//        {
//            var btn = (System.Web.UI.WebControls.Button)sender;
//            int days = int.Parse(btn.CommandArgument);
//            ViewState["days"] = days;
//            LoadDashboard(days);
//        }

//        private void LoadDashboard(int days)
//        {
//            int sellerId = Convert.ToInt32(Session["SellerId"]);

//            DateTime end = DateTime.Today.AddDays(1).AddTicks(-1);  // end of today
//            DateTime start = DateTime.Today.AddDays(-days + 1);

//            lblRange.Text = $"{start:dd MMM} – {DateTime.Today:dd MMM}";
//            // (Optional) set active pill class via server later if you want

//            // 1) KPIs
//            var kpi = GetKpis(sellerId, start, end);
//            lblOrders.Text = kpi.Orders.ToString();
//            lblRevenue.Text = kpi.Revenue.ToString("0.00");
//            lblAOV.Text = (kpi.Orders == 0 ? 0 : kpi.Revenue / kpi.Orders).ToString("0.00");

//            // 2) Top items table + top item KPI
//            DataTable topItems = GetTopItems(sellerId, start, end, topN: 8);
//            gvTopItems.DataSource = topItems;
//            gvTopItems.DataBind();

//            if (topItems.Rows.Count > 0)
//            {
//                lblTopItem.Text = topItems.Rows[0]["ProductName"].ToString();
//                lblTopItemUnits.Text = topItems.Rows[0]["UnitsSold"] + " sold";
//            }
//            else
//            {
//                lblTopItem.Text = "—";
//                lblTopItemUnits.Text = "0 sold";
//            }

//            // 3) Trend chart
//            DataTable trend = GetDailyRevenue(sellerId, start, end);

//            var labels = trend.AsEnumerable()
//                .Select(r => Convert.ToDateTime(r["Day"]).ToString("dd MMM"))
//                .ToArray();

//            var values = trend.AsEnumerable()
//                .Select(r => Convert.ToDecimal(r["Revenue"]))
//                .ToArray();

//            // chart.js expects JSON arrays
//            hfLabels.Value = System.Web.Helpers.Json.Encode(labels);
//            hfValues.Value = System.Web.Helpers.Json.Encode(values);
//        }

//        // ====== DATA ACCESS (adjust table/column names to your schema) ======

//        private (int Orders, decimal Revenue) GetKpis(int sellerId, DateTime start, DateTime end)
//        {
//            // Assumption:
//            // Orders table: Orders(OrderId, SellerId, OrderDate, TotalAmount, Status)
//            // Only count paid/completed:
//            // Status IN ('Paid','Completed')
//            using (var conn = new SqlConnection(ConnStr))
//            using (var cmd = new SqlCommand(@"
//                SELECT
//                    COUNT(*) AS Orders,
//                    ISNULL(SUM(TotalAmount), 0) AS Revenue
//                FROM Orders
//                WHERE SellerID = @SellerID
//                  AND OrderDate >= @StartDate AND OrderDate <= @EndDate
//                  AND Status IN ('Paid','Completed');", conn))
//            {
//                cmd.Parameters.AddWithValue("@SellerID", sellerId);
//                cmd.Parameters.AddWithValue("@StartDate", start);
//                cmd.Parameters.AddWithValue("@EndDate", end);

//                conn.Open();
//                using (var r = cmd.ExecuteReader())
//                {
//                    if (!r.Read()) return (0, 0);
//                    int orders = Convert.ToInt32(r["Orders"]);
//                    decimal revenue = Convert.ToDecimal(r["Revenue"]);
//                    return (orders, revenue);
//                }
//            }
//        }

//        private DataTable GetDailyRevenue(int sellerId, DateTime start, DateTime end)
//        {
//            // Assumption:
//            // Orders(OrderDate, TotalAmount)
//            // Group by date for trend
//            using (var conn = new SqlConnection(ConnStr))
//            using (var cmd = new SqlCommand(@"
//                SELECT
//                    CAST(OrderDate AS date) AS [Day],
//                    ISNULL(SUM(TotalAmount), 0) AS Revenue
//                FROM Orders
//                WHERE SellerID = @SellerID
//                  AND OrderDate >= @StartDate AND OrderDate <= @EndDate
//                  AND Status IN ('Paid','Completed')
//                GROUP BY CAST(OrderDate AS date)
//                ORDER BY [Day];", conn))
//            {
//                cmd.Parameters.AddWithValue("@SellerID", sellerId);
//                cmd.Parameters.AddWithValue("@StartDate", start);
//                cmd.Parameters.AddWithValue("@EndDate", end);

//                var dt = new DataTable();
//                using (var da = new SqlDataAdapter(cmd))
//                {
//                    da.Fill(dt);
//                }

//                // (Optional) fill missing dates with 0 revenue so chart looks smooth
//                return FillMissingDays(dt, start.Date, end.Date);
//            }
//        }

//        private DataTable GetTopItems(int sellerId, DateTime start, DateTime end, int topN)
//        {
//            // Assumption:
//            // OrderItems(OrderId, ProductId, Quantity, UnitPrice)
//            // Products(ProductId, ProductName, SellerId)
//            // Orders(OrderId, OrderDate, Status)
//            using (var conn = new SqlConnection(ConnStr))
//            using (var cmd = new SqlCommand($@"
//                SELECT TOP (@TopN)
//                    p.ProductName,
//                    SUM(oi.Quantity) AS UnitsSold,
//                    SUM(oi.Quantity * oi.UnitPrice) AS Revenue
//                FROM OrderItems oi
//                INNER JOIN Orders o ON o.OrderID = oi.OrderID
//                INNER JOIN Products p ON p.ProductID = oi.ProductID
//                WHERE p.SellerID = @SellerID
//                  AND o.OrderDate >= @StartDate AND o.OrderDate <= @EndDate
//                  AND o.Status IN ('Paid','Completed')
//                GROUP BY p.ProductName
//                ORDER BY UnitsSold DESC, Revenue DESC;", conn))
//            {
//                cmd.Parameters.AddWithValue("@TopN", topN);
//                cmd.Parameters.AddWithValue("@SellerID", sellerId);
//                cmd.Parameters.AddWithValue("@StartDate", start);
//                cmd.Parameters.AddWithValue("@EndDate", end);

//                var dt = new DataTable();
//                using (var da = new SqlDataAdapter(cmd))
//                {
//                    da.Fill(dt);
//                }
//                return dt;
//            }
//        }

//        private DataTable FillMissingDays(DataTable dt, DateTime start, DateTime end)
//        {
//            // dt columns: Day (date), Revenue (decimal)
//            var result = dt.Clone();
//            var map = dt.AsEnumerable().ToDictionary(
//                r => Convert.ToDateTime(r["Day"]).Date,
//                r => Convert.ToDecimal(r["Revenue"])
//            );

//            for (var d = start; d <= end; d = d.AddDays(1))
//            {
//                var row = result.NewRow();
//                row["Day"] = d;
//                row["Revenue"] = map.TryGetValue(d, out var rev) ? rev : 0m;
//                result.Rows.Add(row);
//            }
//            return result;
//        }
//    }
}