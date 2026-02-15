using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EcoEats
{
    public partial class SellerWasteTracker : System.Web.UI.Page
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (Session["SellerID"] == null)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindCategories();
                LoadTracker();
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            LoadTracker();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtFrom.Text = "";
            txtTo.Text = "";
            ddlCategory.SelectedIndex = 0;
            LoadTracker();
        }

        private int GetSellerId() => Convert.ToInt32(Session["SellerID"]);

        private void BindCategories()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem("All", ""));

            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                SELECT DISTINCT ISNULL(Category, 'Uncategorized') AS Category
                FROM Products
                WHERE SellerID = @SellerID
                ORDER BY Category;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", GetSellerId());
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ddlCategory.Items.Add(r.GetString(0));
                    }
                }
            }
        }

        private void LoadTracker()
        {
            try
            {
                int sellerId = GetSellerId();

                DateTime? from = ParseDate(txtFrom.Text);
                DateTime? to = ParseDate(txtTo.Text);
                string category = ddlCategory.SelectedValue; // "" means all

                // 1) KPI totals
                using (var conn = new SqlConnection(_connStr))
                using (var cmd = new SqlCommand(@"
                    SELECT
                        ISNULL(SUM(oi.Quantity), 0) AS ItemsSaved,
                        ISNULL(SUM(oi.LineTotal), 0) AS Revenue,
                        ISNULL(SUM(oi.Quantity * ISNULL(p.CO2Saved, 0)), 0) AS CO2Saved
                    FROM OrderItems oi
                    INNER JOIN Orders o ON o.OrderID = oi.OrderID
                    INNER JOIN Products p ON p.ProductID = oi.ProductID
                    WHERE oi.SellerID = @SellerID
                      AND o.PayStatus = @PayStatus
                      AND (@From IS NULL OR o.CreatedAt >= @From)
                      AND (@To IS NULL OR o.CreatedAt < DATEADD(day, 1, @To))
                      AND (@Category = '' OR ISNULL(p.Category,'Uncategorized') = @Category);
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);

                    // ⚠️ change this if your system uses a different value
                    cmd.Parameters.AddWithValue("@PayStatus", "PAID");

                    cmd.Parameters.AddWithValue("@From", (object)from ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@To", (object)to ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", category ?? "");

                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            lblItemsSaved.Text = r["ItemsSaved"].ToString();
                            lblRevenue.Text = Convert.ToDecimal(r["Revenue"]).ToString("$0.00");
                            lblCO2.Text = Convert.ToDecimal(r["CO2Saved"]).ToString("0.00");
                        }
                    }
                }

                // 2) Breakdown by product
                DataTable dt = new DataTable();
                using (var conn = new SqlConnection(_connStr))
                using (var cmd = new SqlCommand(@"
                    SELECT
                        p.ProductName,
                        ISNULL(p.Category, 'Uncategorized') AS Category,
                        SUM(oi.Quantity) AS QtySold,
                        SUM(oi.LineTotal) AS Revenue,
                        SUM(oi.Quantity * ISNULL(p.CO2Saved, 0)) AS CO2Saved
                    FROM OrderItems oi
                    INNER JOIN Orders o ON o.OrderID = oi.OrderID
                    INNER JOIN Products p ON p.ProductID = oi.ProductID
                    WHERE oi.SellerID = @SellerID
                      AND o.PayStatus = @PayStatus
                      AND (@From IS NULL OR o.CreatedAt >= @From)
                      AND (@To IS NULL OR o.CreatedAt < DATEADD(day, 1, @To))
                      AND (@Category = '' OR ISNULL(p.Category,'Uncategorized') = @Category)
                    GROUP BY p.ProductName, ISNULL(p.Category, 'Uncategorized')
                    ORDER BY QtySold DESC;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    cmd.Parameters.AddWithValue("@PayStatus", "PAID");
                    cmd.Parameters.AddWithValue("@From", (object)from ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@To", (object)to ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", category ?? "");

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                gvBreakdown.DataSource = dt;
                gvBreakdown.DataBind();
                pnlEmpty.Visible = (dt.Rows.Count == 0);
            }
            catch (Exception ex)
            {
                lblError.Text = "Error loading tracker: " + ex.Message;
            }
        }

        private DateTime? ParseDate(string input)
        {
            if (DateTime.TryParse(input, out var d)) return d.Date;
            return null;
        }
    }
}
