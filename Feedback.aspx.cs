using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class Feedback : Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Must be logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindFeedbackList();
            }
        }

        private void BindFeedbackList()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT FeedbackID, Rating, Tag, Comments, CreatedAt
                  FROM Feedback
                  WHERE UserID = @UserID
                  ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptFeedback.DataSource = dt;
                    rptFeedback.DataBind();

                    pnlNoFeedback.Visible = dt.Rows.Count == 0;
                    rptFeedback.Visible = dt.Rows.Count > 0;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            if (string.IsNullOrEmpty(ddlRating.SelectedValue))
            {
                lblFormMessage.Text = "Please choose a rating.";
                return;
            }

            int rating = int.Parse(ddlRating.SelectedValue);
            string tag = ddlTag.SelectedValue;
            string comments = txtComments.Text.Trim();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // If hfFeedbackID has value => update, else insert
                if (string.IsNullOrEmpty(hfFeedbackID.Value))
                {
                    // INSERT
                    string sql = @"INSERT INTO Feedback (UserID, Rating, Tag, Comments, CreatedAt)
                                   VALUES (@UserID, @Rating, @Tag, @Comments, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@Tag", (object)tag ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Comments",
                            string.IsNullOrEmpty(comments) ? (object)DBNull.Value : comments);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                    NotificationHelper.Add(
                        "Feedback",
                        "New feedback received",
                        "A customer submitted feedback.",
                        "feedback",
                        null
                    );

                    lblFormMessage.Text = "Feedback submitted. Thank you!";

                }
                else
                {
                    // UPDATE
                    int feedbackId = int.Parse(hfFeedbackID.Value);

                    string sql = @"UPDATE Feedback
                                   SET Rating = @Rating,
                                       Tag = @Tag,
                                       Comments = @Comments
                                   WHERE FeedbackID = @FeedbackID
                                     AND UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@Tag", (object)tag ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Comments",
                            string.IsNullOrEmpty(comments) ? (object)DBNull.Value : comments);

                        cmd.ExecuteNonQuery();
                    }

                    lblFormMessage.Text = "Feedback updated.";
                }
            }

            // Reset form and refresh list
            ClearForm();
            BindFeedbackList();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblFormMessage.Text = "Edit cancelled.";
        }

        private void ClearForm()
        {
            hfFeedbackID.Value = "";
            ddlRating.SelectedIndex = 0;
            ddlTag.SelectedIndex = 0;
            txtComments.Text = "";
            btnSubmit.Text = "Submit Feedback";
            btnCancelEdit.Visible = false;
        }

        protected void rptFeedback_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            int feedbackId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "edit")
            {
                // Load record into form
                using (SqlConnection conn = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT FeedbackID, Rating, Tag, Comments
                      FROM Feedback
                      WHERE FeedbackID = @FeedbackID AND UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hfFeedbackID.Value = feedbackId.ToString();
                            ddlRating.SelectedValue = reader["Rating"].ToString();
                            ddlTag.SelectedValue = reader["Tag"].ToString();
                            txtComments.Text = reader["Comments"].ToString();

                            btnSubmit.Text = "Update Feedback";
                            btnCancelEdit.Visible = true;
                            lblFormMessage.Text = "Editing existing feedback.";
                        }
                    }
                }
            }
            else if (e.CommandName == "delete")
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(
                    @"DELETE FROM Feedback
                      WHERE FeedbackID = @FeedbackID AND UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblFormMessage.Text = "Feedback deleted.";
                ClearForm();
                BindFeedbackList();
            }
        }
    }
}
