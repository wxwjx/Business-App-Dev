using Microsoft.Ajax.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class SalesDashboard : Page
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seller guard
            if (Session["UserRole"]?.ToString() != "Seller" || Session["SellerId"] == null)
            {
                Response.Redirect("~/Login.aspx?role=Seller");
                return;
            }

            if (!IsPostBack)
            {
                ViewState["days"] = 30;
                SetActiveRangeButton(30);
                LoadDashboard(30);
            }
        }

        protected void Range_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int days = int.Parse(btn.CommandArgument);

            ViewState["days"] = days;
            SetActiveRangeButton(days);
            LoadDashboard(days);
        }

        private void SetActiveRangeButton(int days)
        {
            // reset classes
            btn7.CssClass = "sdash-pill";
            btn30.CssClass = "sdash-pill";
            btn90.CssClass = "sdash-pill";

            if (days == 7) btn7.CssClass = "sdash-pill active";
            else if (days == 30) btn30.CssClass = "sdash-pill active";
            else if (days == 90) btn90.CssClass = "sdash-pill active";
        }

        private void LoadDashboard(int days)
        {
            int sellerId = Convert.ToInt32(Session["SellerId"]);

            // Range: inclusive start, inclusive end-of-today
            DateTime start = DateTime.Today.AddDays(-days + 1);
            DateTime end = DateTime.Today.AddDays(1).AddTicks(-1);

            lblRange.Text = $"{start:dd MMM yyyy} – {DateTime.Today:dd MMM yyyy}";

            // 1) KPIs (Paid only)
            var kpi = GetKpis(sellerId, start, end);
            lblOrders.Text = kpi.Orders.ToString();
            lblRevenue.Text = kpi.Revenue.ToString("0.00");
            lblAOV.Text = (kpi.Orders == 0 ? 0m : (kpi.Revenue / kpi.Orders)).ToString("0.00");

            // 2) Top items (Paid only)
            DataTable topItems = GetTopItems(sellerId, start, end, topN: 8);
            gvTopItems.DataSource = topItems;
            gvTopItems.DataBind();

            if (topItems.Rows.Count > 0)
            {
                lblTopItem.Text = topItems.Rows[0]["ProductName"]?.ToString() ?? "—";
                lblTopItemUnits.Text = (topItems.Rows[0]["UnitsSold"]?.ToString() ?? "0") + " sold";
            }
            else
            {
                lblTopItem.Text = "—";
                lblTopItemUnits.Text = "0 sold";
            }

            // 3) Daily revenue trend (Paid only)
            DataTable trend = GetDailyRevenue(sellerId, start, end);

            var labels = trend.AsEnumerable()
                .Select(r => Convert.ToDateTime(r["Day"]).ToString("dd MMM"))
                .ToArray();

            var values = trend.AsEnumerable()
                .Select(r => Convert.ToDecimal(r["Revenue"]))
                .ToArray();

            hfLabels.Value = Newtonsoft.Json.JsonConvert.SerializeObject(labels);
            hfValues.Value = Newtonsoft.Json.JsonConvert.SerializeObject(values);

            // Ensure chart rerenders after postback
            ScriptManager.RegisterStartupScript(this, GetType(), "chart", "renderSalesChart();", true);
        }

        // ===== DATA =====

        private (int Orders, decimal Revenue) GetKpis(int sellerId, DateTime start, DateTime end)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT
                    COUNT(*) AS Orders,
                    ISNULL(SUM(TotalAmount), 0) AS Revenue
                FROM Orders
                WHERE SellerID = @SellerID
                  AND CreatedAt >= @StartDate AND CreatedAt <= @EndDate
                  AND PayStatus = 'Paid';", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                cmd.Parameters.AddWithValue("@StartDate", start);
                cmd.Parameters.AddWithValue("@EndDate", end);

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return (0, 0m);
                    return (Convert.ToInt32(r["Orders"]), Convert.ToDecimal(r["Revenue"]));
                }
            }
        }

        private DataTable GetDailyRevenue(int sellerId, DateTime start, DateTime end)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT
                    CAST(CreatedAt AS date) AS [Day],
                    ISNULL(SUM(TotalAmount), 0) AS Revenue
                FROM Orders
                WHERE SellerID = @SellerID
                  AND CreatedAt >= @StartDate AND CreatedAt <= @EndDate
                  AND PayStatus = 'Paid'
                GROUP BY CAST(CreatedAt AS date)
                ORDER BY [Day];", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                cmd.Parameters.AddWithValue("@StartDate", start);
                cmd.Parameters.AddWithValue("@EndDate", end);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);

                return FillMissingDays(dt, start.Date, DateTime.Today);
            }
        }

        private DataTable GetTopItems(int sellerId, DateTime start, DateTime end, int topN)
        {
            // Uses OrderItems snapshot fields; join Orders for date & PayStatus filtering
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (@TopN)
                    oi.ProductName,
                    SUM(oi.Quantity) AS UnitsSold,
                    ISNULL(SUM(oi.LineTotal), 0) AS Revenue
                FROM OrderItems oi
                INNER JOIN Orders o ON o.OrderID = oi.OrderID
                WHERE oi.SellerID = @SellerID
                  AND o.CreatedAt >= @StartDate AND o.CreatedAt <= @EndDate
                  AND o.PayStatus = 'Paid'
                GROUP BY oi.ProductName
                ORDER BY UnitsSold DESC, Revenue DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@TopN", topN);
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                cmd.Parameters.AddWithValue("@StartDate", start);
                cmd.Parameters.AddWithValue("@EndDate", end);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }

        private DataTable FillMissingDays(DataTable dt, DateTime start, DateTime end)
        {
            var result = dt.Clone();
            var map = dt.AsEnumerable().ToDictionary(
                r => Convert.ToDateTime(r["Day"]).Date,
                r => Convert.ToDecimal(r["Revenue"])
            );

            for (var d = start; d <= end; d = d.AddDays(1))
            {
                var row = result.NewRow();
                row["Day"] = d;
                row["Revenue"] = map.TryGetValue(d, out var rev) ? rev : 0m;
                result.Rows.Add(row);
            }
            return result;
        }
    }
}