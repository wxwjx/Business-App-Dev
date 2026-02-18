using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT OrderID, Rating, Comment, CreatedAt
                FROM SellerFeedback
                WHERE SellerID=@SID
                ORDER BY CreatedAt DESC;", con))
            {
                cmd.Parameters.AddWithValue("@SID", Session["SellerID"]);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                rptFeedback.DataSource = dt;
                rptFeedback.DataBind();
            }
        }
    }
}
