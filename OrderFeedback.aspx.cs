using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Business_App_Dev
{
    public partial class OrderFeedback : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Prefer querystring: OrderFeedback.aspx?orderId=123
                string qs = Request.QueryString["orderId"];

                if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out int orderIdFromQs))
                {
                    hfOrderId.Value = orderIdFromQs.ToString();
                    lblOrderIdDisplay.Text = "#" + orderIdFromQs;
                }
                // 2) Fallback: Session["LastOrderId"]
                else if (Session["LastOrderId"] != null && int.TryParse(Session["LastOrderId"].ToString(), out int orderIdFromSession))
                {
                    hfOrderId.Value = orderIdFromSession.ToString();
                    lblOrderIdDisplay.Text = "#" + orderIdFromSession;
                }
                else
                {
                    hfOrderId.Value = "";
                    lblOrderIdDisplay.Text = "(open from Orders)";
                    ShowMsg("Please open this page from Orders so we know which order you are rating.", false);
                }

                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT ReviewId, OrderId, Rating, Quality, OnTime, OrderAgain, Comments, CreatedAt FROM OrderReviews ORDER BY CreatedAt DESC", conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvReviews.DataSource = dt;
                gvReviews.DataBind();
            }
        }

        private int GetSelectedRating()
        {
            if (star5.Checked) return 5;
            if (star4.Checked) return 4;
            if (star3.Checked) return 3;
            if (star2.Checked) return 2;
            if (star1.Checked) return 1;
            return 0;
        }

        private void SetStars(int rating)
        {
            star1.Checked = rating == 1;
            star2.Checked = rating == 2;
            star3.Checked = rating == 3;
            star4.Checked = rating == 4;
            star5.Checked = rating == 5;
        }

        private void ShowMsg(string text, bool ok)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = ok ? "msg ok" : "msg err";
        }

        private void ClearForm()
        {
            hfReviewId.Value = "";
            ddlQuality.SelectedIndex = 0;
            ddlOnTime.SelectedIndex = 0;
            ddlAgain.SelectedIndex = 0;
            txtComments.Text = "";
            SetStars(0);
        }

        // --------- CREATE ----------
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfOrderId.Value) || !int.TryParse(hfOrderId.Value, out int orderId))
            {
                ShowMsg("Order is not set. Please open this page from Orders.", false);
                return;
            }

            int rating = GetSelectedRating();
            if (rating == 0)
            {
                ShowMsg("Please select a star rating (1–5).", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlQuality.SelectedValue) ||
                string.IsNullOrWhiteSpace(ddlOnTime.SelectedValue) ||
                string.IsNullOrWhiteSpace(ddlAgain.SelectedValue))
            {
                ShowMsg("Please answer all questions before submitting.", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO OrderReviews (OrderId, Rating, Quality, OnTime, OrderAgain, Comments, CreatedAt)
                VALUES (@OrderId, @Rating, @Quality, @OnTime, @OrderAgain, @Comments, GETDATE())
            ", conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Quality", ddlQuality.SelectedValue);
                cmd.Parameters.AddWithValue("@OnTime", ddlOnTime.SelectedValue);
                cmd.Parameters.AddWithValue("@OrderAgain", ddlAgain.SelectedValue);
                cmd.Parameters.AddWithValue("@Comments", (object)txtComments.Text.Trim() ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
            ClearForm();
            ShowMsg("✅ Thanks! Your feedback was submitted.", true);
        }

        // --------- READ (Select row to load) ----------
        protected void gvReviews_SelectedIndexChanged(object sender, EventArgs e)
        {
            int reviewId = Convert.ToInt32(gvReviews.SelectedDataKey.Value);
            hfReviewId.Value = reviewId.ToString();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM OrderReviews WHERE ReviewId=@ReviewId", conn))
            {
                cmd.Parameters.AddWithValue("@ReviewId", reviewId);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // keep the orderId in hidden field
                        hfOrderId.Value = dr["OrderId"].ToString();
                        lblOrderIdDisplay.Text = "#" + dr["OrderId"].ToString();

                        SetStars(Convert.ToInt32(dr["Rating"]));
                        ddlQuality.SelectedValue = dr["Quality"].ToString();
                        ddlOnTime.SelectedValue = dr["OnTime"].ToString();
                        ddlAgain.SelectedValue = dr["OrderAgain"].ToString();
                        txtComments.Text = dr["Comments"] == DBNull.Value ? "" : dr["Comments"].ToString();

                        ShowMsg("Loaded ✅ You can update or delete this review.", true);
                    }
                }
            }
        }

        // --------- UPDATE ----------
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfReviewId.Value))
            {
                ShowMsg("Please click Edit on a review first.", false);
                return;
            }

            if (!int.TryParse(hfReviewId.Value, out int reviewId))
            {
                ShowMsg("Invalid review selected.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(hfOrderId.Value) || !int.TryParse(hfOrderId.Value, out int orderId))
            {
                ShowMsg("Order is not set. Please open from Orders.", false);
                return;
            }

            int rating = GetSelectedRating();
            if (rating == 0)
            {
                ShowMsg("Please select a star rating (1–5).", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE OrderReviews
                SET OrderId=@OrderId,
                    Rating=@Rating,
                    Quality=@Quality,
                    OnTime=@OnTime,
                    OrderAgain=@OrderAgain,
                    Comments=@Comments
                WHERE ReviewId=@ReviewId
            ", conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Quality", ddlQuality.SelectedValue);
                cmd.Parameters.AddWithValue("@OnTime", ddlOnTime.SelectedValue);
                cmd.Parameters.AddWithValue("@OrderAgain", ddlAgain.SelectedValue);
                cmd.Parameters.AddWithValue("@Comments", (object)txtComments.Text.Trim() ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ReviewId", reviewId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
            ClearForm();
            ShowMsg("✅ Review updated.", true);
        }

        // --------- DELETE ----------
        protected void gvReviews_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int reviewId = Convert.ToInt32(gvReviews.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM OrderReviews WHERE ReviewId=@ReviewId", conn))
            {
                cmd.Parameters.AddWithValue("@ReviewId", reviewId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
            ClearForm();
            ShowMsg("🗑️ Review deleted.", true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowMsg("Cleared.", true);
        }
    }
}