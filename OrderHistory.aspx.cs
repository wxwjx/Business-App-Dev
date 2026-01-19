using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Business_App_Dev.Services;

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

                ApplyTranslations();

                int userId = GetUserIdOrThrow();
                BindOrders(userId);
            }
            catch (SqlException)
            {
                ShowError(T("Database error while loading your order history. Please try again."));
            }
            catch (Exception ex)
            {
                ShowError(T(ex.Message));
            }
        }

        // -------- Translation helpers --------
        private string GetLang()
        {
            return (Session["LANG"] as string) ?? "en";
        }

        private string T(string text)
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return text ?? "";

            text = text ?? "";
            if (string.IsNullOrWhiteSpace(text)) return text;

            string key = $"tr:en->{lang}:{text}";
            return TranslationCache.GetOrAdd(key, () =>
                TranslationService.Translate(text, lang, "en"), hours: 24);
        }

        private void ApplyTranslations()
        {
            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            lblHeroTitle.Text = T(lblHeroTitle.Text);
            lblHeroSub.Text = T(lblHeroSub.Text);

            lblEmptyTitle.Text = T(lblEmptyTitle.Text);
            lblEmptyText.Text = T(lblEmptyText.Text);
            lblBrowseDeals.Text = T(lblBrowseDeals.Text);
        }

        // -------- Existing logic --------
        private int GetUserIdOrThrow()
        {
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

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            string lang = GetLang();
            if (lang.Equals("en", StringComparison.OrdinalIgnoreCase)) return;

            var lblOrderHash = e.Item.FindControl("lblOrderHash") as Label;
            if (lblOrderHash != null) lblOrderHash.Text = T(lblOrderHash.Text);

            var lblViewDetails = e.Item.FindControl("lblViewDetails") as Label;
            if (lblViewDetails != null) lblViewDetails.Text = T(lblViewDetails.Text);

            var lblPaymentRef = e.Item.FindControl("lblPaymentRef") as Label;
            if (lblPaymentRef != null) lblPaymentRef.Text = T(lblPaymentRef.Text);

            var lblPayStatusRow = e.Item.FindControl("lblPayStatusRow") as Label;
            if (lblPayStatusRow != null) lblPayStatusRow.Text = T(lblPayStatusRow.Text);
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            lblError.Text = msg;
        }
    }
}
