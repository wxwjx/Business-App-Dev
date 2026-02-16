using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class OrderView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            try
            {
                pnlError.Visible = false;
                pnlMain.Visible = false;

                int userId = GetUserIdOrThrow();
                int orderId = GetOrderIdOrThrow();

                LoadOrderHeader(userId, orderId);
                LoadOrderItems(userId, orderId);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private int GetUserIdOrThrow()
        {
            if (Session["UserID"] == null)
                throw new Exception("Session expired. Please log in again.");

            return Convert.ToInt32(Session["UserID"]);
        }

        private int GetOrderIdOrThrow()
        {
            if (!int.TryParse(Request.QueryString["orderId"], out int orderId))
                throw new Exception("Invalid order.");

            return orderId;
        }

        private string ConnStr()
        {
            return ConfigurationManager
                .ConnectionStrings["EcoEatsDb"]
                .ConnectionString;
        }

        private void LoadOrderHeader(int userId, int orderId)
        {
            string sql = @"
SELECT TOP 1
    o.OrderID,
    o.SellerID,
    o.StripeSessionId,
    o.TotalAmount,
    o.PayStatus,
    o.OrderStatus,
    o.CreatedAt
FROM dbo.Orders o
WHERE o.OrderID = @OrderID AND o.UserID = @UserID;";

            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        throw new Exception("Order not found.");

                    pnlMain.Visible = true;

                    string sellerId = rdr["SellerID"]?.ToString() ?? "";

                    lblOrderId.Text = rdr["OrderID"].ToString();
                    lblCreatedAt.Text = Convert
                        .ToDateTime(rdr["CreatedAt"])
                        .ToString("dd MMM yyyy, hh:mm tt");

                    lblTotal.Text = Convert
                        .ToDecimal(rdr["TotalAmount"])
                        .ToString("0.00");

                    lblPayStatus.Text = rdr["PayStatus"]?.ToString();
                    lblOrderStatus.Text = rdr["OrderStatus"]?.ToString();
                    lblStripeSessionId.Text = rdr["StripeSessionId"]?.ToString();

                    refPill.Attributes["data-ref"] = lblStripeSessionId.Text;

                    // Just navigate to friend pages
                    lnkChatSeller.NavigateUrl =
                        $"Chat.aspx?orderId={orderId}&sellerId={sellerId}";

                    lnkRateOrder.NavigateUrl =
                        $"RateOrder.aspx?orderId={orderId}";
                }
            }
        }

        private void LoadOrderItems(int userId, int orderId)
        {
            string sql = @"
SELECT
    p.ProductName,
    oi.Quantity,
    (oi.Quantity * oi.UnitPrice) AS LineTotal
FROM dbo.OrderItems oi
JOIN dbo.Products p ON p.ProductID = oi.ProductID
JOIN dbo.Orders o ON o.OrderID = oi.OrderID
WHERE oi.OrderID = @OrderID AND o.UserID = @UserID;";

            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        pnlNoItems.Visible = true;
                        return;
                    }

                    rptItems.DataSource = dt;
                    rptItems.DataBind();
                }
            }
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            pnlMain.Visible = false;
            lblError.Text = msg;
        }
    }
}
