using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Business_App_Dev
{
    public partial class SellerFeedbackPage : System.Web.UI.Page
    {
        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerID"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindFeedback();
            }
        }

        private void BindFeedback()
        {
            int sellerId = Convert.ToInt32(Session["SellerID"]);

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT OrderID, Rating, Comment, CreatedAt
        FROM dbo.SellerFeedback
        WHERE SellerID = @SID
        ORDER BY CreatedAt DESC;
    ", con))
            {
                cmd.Parameters.AddWithValue("@SID", sellerId);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                // Empty state
                pnlEmpty.Visible = dt.Rows.Count == 0;

                // Summary
                if (dt.Rows.Count > 0)
                {
                    double avg = dt.AsEnumerable().Average(r => Convert.ToDouble(r["Rating"]));
                    lblAvgRating.Text = avg.ToString("0.0");
                    lblTotalReviews.Text = dt.Rows.Count.ToString();
                    lblLatestDate.Text = Convert.ToDateTime(dt.Rows[0]["CreatedAt"]).ToString("dd MMM yyyy");
                }
                else
                {
                    lblAvgRating.Text = "-";
                    lblTotalReviews.Text = "0";
                    lblLatestDate.Text = "-";
                }

                rptFeedback.DataSource = dt;
                rptFeedback.DataBind();
            }
        }
    }
}
