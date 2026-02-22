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
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
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

                    lblOrderId.Text = rdr["OrderID"].ToString();
                    lblCreatedAt.Text = Convert.ToDateTime(rdr["CreatedAt"]).ToString("dd MMM yyyy, hh:mm tt");

                    lblTotal.Text = Convert.ToDecimal(rdr["TotalAmount"]).ToString("0.00");

                    lblPayStatus.Text = rdr["PayStatus"]?.ToString();
                    lblOrderStatus.Text = rdr["OrderStatus"]?.ToString();
                    lblStripeSessionId.Text = rdr["StripeSessionId"]?.ToString();

                    refPill.Attributes["data-ref"] = lblStripeSessionId.Text;
                }
            }

            var sellerInfo = GetSellerInfoFromOrderItems(orderId, userId);

            if (sellerInfo.SellerCount == 1)
            {
                int sid = sellerInfo.SingleSellerId;

                int conversationId = GetOrCreateConversationId(userId, sid);
                lnkChatSeller.NavigateUrl = $"Messages.aspx?cid={conversationId}";
                lnkChatSeller.Visible = true;

                pnlError.Visible = false;
            }
            else
            {
                lnkChatSeller.Visible = false;

                pnlError.Visible = true;
                lblError.Text = sellerInfo.SellerCount == 0
                    ? "This order has no seller linked in OrderItems. Check OrderItems.SellerID and Products.SellerID."
                    : "This order contains items from multiple sellers. Please message sellers from the Order Success receipt groups (or implement seller grouping on this page).";
            }

            lnkRateOrder.NavigateUrl = $"RateOrder.aspx?orderId={orderId}";
        }

        private (int SellerCount, int SingleSellerId) GetSellerInfoFromOrderItems(int orderId, int userId)
        {
            string sql = @"
SELECT COUNT(DISTINCT oi.SellerID) AS Cnt,
       MAX(oi.SellerID) AS AnySeller
FROM dbo.OrderItems oi
JOIN dbo.Orders o ON o.OrderID = oi.OrderID
WHERE oi.OrderID = @OrderID AND o.UserID = @UserID AND oi.SellerID IS NOT NULL;";

            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return (0, 0);

                    int cnt = Convert.ToInt32(r["Cnt"]);
                    int anySeller = (r["AnySeller"] == DBNull.Value) ? 0 : Convert.ToInt32(r["AnySeller"]);

                    return (cnt, anySeller);
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

        private int GetOrCreateConversationId(int userId, int sellerId)
        {
            using (SqlConnection con = new SqlConnection(ConnStr()))
            {
                con.Open();

                using (SqlCommand find = new SqlCommand(@"
SELECT ConversationID
FROM dbo.Conversations
WHERE UserID = @UID AND SellerID = @SID;", con))
                {
                    find.Parameters.AddWithValue("@UID", userId);
                    find.Parameters.AddWithValue("@SID", sellerId);

                    object existing = find.ExecuteScalar();
                    if (existing != null && existing != DBNull.Value)
                        return Convert.ToInt32(existing);
                }

                using (SqlCommand create = new SqlCommand(@"
BEGIN TRY
    INSERT INTO dbo.Conversations (UserID, SellerID)
    VALUES (@UID, @SID);
END TRY
BEGIN CATCH
    IF ERROR_NUMBER() NOT IN (2601, 2627) THROW;
END CATCH;

SELECT ConversationID
FROM dbo.Conversations
WHERE UserID = @UID AND SellerID = @SID;", con))
                {
                    create.Parameters.AddWithValue("@UID", userId);
                    create.Parameters.AddWithValue("@SID", sellerId);

                    object cid = create.ExecuteScalar();
                    if (cid == null || cid == DBNull.Value)
                        throw new Exception("Unable to open conversation.");

                    return Convert.ToInt32(cid);
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