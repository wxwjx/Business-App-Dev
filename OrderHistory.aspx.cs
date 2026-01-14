using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class OrderHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            try
            {
                pnlError.Visible = false;
                pnlEmpty.Visible = false;

                int userId = GetUserIdOrThrow();
                BindOrders(userId);
            }
            catch (SqlException)
            {
                ShowError("Database error while loading your order history. Please try again.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private int GetUserIdOrThrow()
        {
            // CHANGE THIS KEY if your login uses a different session key
            // e.g. "CustomerID" instead of "UserID"
            if (Session["UserID"] == null)
                throw new Exception("Session expired. Please log in again.");

            if (!int.TryParse(Session["UserID"].ToString(), out int userId) || userId <= 0)
                throw new Exception("Invalid user session. Please log in again.");

            return userId;
        }

        private string ConnStr()
        {
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
        }

        private void BindOrders(int userId)
        {
            string sql = @"
SELECT
    OrderID,
    StripeSessionId,
    TotalAmount,
    PayStatus,
    CreatedAt
FROM dbo.Orders
WHERE UserID = @UserID
ORDER BY CreatedAt DESC;";

            using (SqlConnection con = new SqlConnection(ConnStr()))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        pnlEmpty.Visible = true;
                        rptOrders.DataSource = null;
                        rptOrders.DataBind();
                        return;
                    }

                    rptOrders.DataSource = dt;
                    rptOrders.DataBind();
                }
            }
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            lblError.Text = msg;
        }
    }
}
