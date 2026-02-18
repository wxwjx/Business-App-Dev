using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class RateOrder : System.Web.UI.Page
    {
        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ValidateAccess();
            }
        }

        private void ValidateAccess()
        {
            if (!int.TryParse(Request.QueryString["orderId"], out int orderId))
            {
                lblMessage.Text = "Invalid order.";
                pnlForm.Visible = false;
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Orders
                WHERE OrderID=@OID AND UserID=@UID;", con))
            {
                cmd.Parameters.AddWithValue("@OID", orderId);
                cmd.Parameters.AddWithValue("@UID", Session["UserID"]);

                con.Open();

                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    lblMessage.Text = "You cannot rate this order.";
                    pnlForm.Visible = false;
                }
            }
            lnkBack.NavigateUrl = $"OrderView.aspx?orderId={orderId}";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            int rating = 5;
            int.TryParse(hfRating.Value, out rating);
            rating = Math.Max(1, Math.Min(5, rating));

            string comment = (txtComment.Text ?? "").Trim();

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
        -- prevent duplicates safely
        IF EXISTS (SELECT 1 FROM dbo.SellerFeedback WHERE OrderID = @OID)
        BEGIN
            SELECT -1;
            RETURN;
        END

        INSERT INTO dbo.SellerFeedback (OrderID, SellerID, UserID, Rating, Comment)
        SELECT o.OrderID, o.SellerID, o.UserID, @Rating, @Comment
        FROM dbo.Orders o
        WHERE o.OrderID = @OID AND o.UserID = @UID;

        SELECT 1;
    ", con))
            {
                cmd.Parameters.AddWithValue("@OID", orderId);
                cmd.Parameters.AddWithValue("@UID", userId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Comment", (object)comment ?? DBNull.Value);

                con.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar());

                if (result == 1)
                {
                    // ✅ best UX: go back to order view
                    Response.Redirect($"OrderView.aspx?orderId={orderId}&rated=1");
                    return;
                }

                // result == -1 OR insert failed (0 rows) -> show message on same page
                pnlMsg.Visible = true;
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "You have already submitted feedback for this order.";
                pnlForm.Visible = false;
            }
        }
    }
}
