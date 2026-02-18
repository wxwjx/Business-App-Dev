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
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            int userId = Convert.ToInt32(Session["UserID"]);
            int rating = Convert.ToInt32(ddlRating.SelectedValue);
            string comment = txtComment.Text.Trim();

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO SellerFeedback (OrderID, SellerID, UserID, Rating, Comment)
                SELECT o.OrderID, o.SellerID, o.UserID, @Rating, @Comment
                FROM Orders o
                WHERE o.OrderID=@OID AND o.UserID=@UID;", con))
            {
                cmd.Parameters.AddWithValue("@OID", orderId);
                cmd.Parameters.AddWithValue("@UID", userId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Comment", comment);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    lblMessage.CssClass = "text-success";
                    lblMessage.Text = "Thank you for your feedback!";
                    pnlForm.Visible = false;
                }
                catch (SqlException)
                {
                    lblMessage.Text = "You have already submitted feedback for this order.";
                    pnlForm.Visible = false;
                }
            }
        }
    }
}
