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
            // Requires dbo.Orders columns:
            // SellerID, OrderStatus, UpdatedAt
            string sql = @"
SELECT
    o.OrderID,
    o.SellerID,
    o.StripeSessionId,
    o.TotalAmount,
    o.PayStatus,
    o.OrderStatus,
    o.CreatedAt,
    o.UpdatedAt
FROM dbo.Orders o
WHERE o.UserID = @UserID
ORDER BY o.CreatedAt DESC;";

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

            var row = e.Item.DataItem as DataRowView;

            string orderStatus = (row?["OrderStatus"] ?? "").ToString();
            string orderId = (row?["OrderID"] ?? "").ToString();
            string sellerId = (row?["SellerID"] ?? "").ToString();

            bool isCompleted = orderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase);
            bool isCancelled = orderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);

            bool canChat = !isCompleted && !isCancelled;
            bool canRate = isCompleted;

            // If sellerId missing, hide both buttons to avoid broken links
            bool hasSellerId = !string.IsNullOrWhiteSpace(sellerId);

            var lnkChatSeller = e.Item.FindControl("lnkChatSeller") as HyperLink;
            var lnkRateOrder = e.Item.FindControl("lnkRateOrder") as HyperLink;
            var pnlStuck = e.Item.FindControl("pnlStuck") as Panel;

            if (lnkChatSeller != null)
            {
                lnkChatSeller.Visible = canChat && hasSellerId;
                lnkChatSeller.Text = "Chat Seller";
                lnkChatSeller.NavigateUrl = $"Chat.aspx?orderId={orderId}&sellerId={sellerId}";
            }

            if (lnkRateOrder != null)
            {
                lnkRateOrder.Visible = canRate && hasSellerId;
                lnkRateOrder.Text = "Rate Order";
                lnkRateOrder.NavigateUrl = $"SellerFeedback.aspx?orderId={orderId}&sellerId={sellerId}";
            }

            // Optional: stuck hint (60 mins since UpdatedAt)
            if (pnlStuck != null)
            {
                pnlStuck.Visible = false;

                if (canChat && row != null && row["UpdatedAt"] != DBNull.Value)
                {
                    DateTime updatedAt = Convert.ToDateTime(row["UpdatedAt"]);
                    bool isStuck = (DateTime.Now - updatedAt).TotalMinutes >= 60;
                    pnlStuck.Visible = isStuck;
                }
            }

            // ---- Translation logic ----
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

            var lblOrderStatusRow = e.Item.FindControl("lblOrderStatusRow") as Label;
            if (lblOrderStatusRow != null) lblOrderStatusRow.Text = T(lblOrderStatusRow.Text);

            if (lnkChatSeller != null) lnkChatSeller.Text = T(lnkChatSeller.Text);
            if (lnkRateOrder != null) lnkRateOrder.Text = T(lnkRateOrder.Text);

            var lblStuckText = e.Item.FindControl("lblStuckText") as Label;
            if (lblStuckText != null) lblStuckText.Text = T(lblStuckText.Text);
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            lblError.Text = msg;
        }
    }
}
