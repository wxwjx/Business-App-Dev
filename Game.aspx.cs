using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class Game : Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int userId = (Session["UserID"] != null) ? Convert.ToInt32(Session["UserID"]) : 0;

                // Not logged in -> lock the game
                if (userId == 0)
                {
                    hfLocked.Value = "1";
                    hfFinalized.Value = "1"; // prevent JS auto-start
                    lblServerMsg.Text = Msg("Please log in to play.", true);
                    SafeShowModal("Login required", "Please log in to play.");
                    return;
                }

                // Daily limit
                if (HasPlayedToday(userId))
                {
                    hfLocked.Value = "1";
                    hfFinalized.Value = "1"; // prevent JS auto-start
                }
                else
                {
                    hfLocked.Value = "0";
                    hfFinalized.Value = "0";
                }
            }
        }

        protected void btnSaveResult_Click(object sender, EventArgs e)
        {
            int userId = (Session["UserID"] != null) ? Convert.ToInt32(Session["UserID"]) : 0;
            if (userId == 0)
            {
                lblServerMsg.Text = Msg("Please log in first.", true);
                SafeShowModal("Login required", "Please log in first.");
                return;
            }

            // ✅ Server-side daily enforcement
            if (HasPlayedToday(userId))
            {
                hfLocked.Value = "1";
                hfFinalized.Value = "1";
                lblServerMsg.Text = Msg("You already played today. Try again tomorrow.", true);
                SafeShowModal("⛔ Daily Limit", "You already played today.<br/>Try again tomorrow.");
                return;
            }

            int score = SafeInt(hfScore.Value);
            int saved = SafeInt(hfSaved.Value);     // items saved count
            int wasted = SafeInt(hfWasted.Value);
            bool won = (hfWon.Value == "1");

            if (score == 0 && saved == 0 && wasted == 0)
            {
                lblServerMsg.Text = Msg("No result detected. Play first, then save.", true);
                SafeShowModal("No result", "No result detected.<br/>Play first, then save.");
                return;
            }

            try
            {
                // 1) Save history (PlayedAt defaults to GETDATE())
                InsertGameSession(userId, score, saved, wasted);

                // 2) Ensure rewards row exists
                EnsureUserRewardsRow(userId);

                // 3) Add points always
                AddPoints(userId, score);

                // 4) Issue voucher only if WON
                bool voucherIssued = false;
                if (won)
                {
                    voucherIssued = IssueVoucherIfEligible(userId, score);
                }

                // 5) Badges
                AwardBadges(userId, score, saved, wasted);

                // 6) Finalize page so JS won’t restart game on postback
                hfFinalized.Value = "1";
                hfLocked.Value = "1";

                // 7) Prepare popup details
                var summary = GetRewardsSummary(userId);

                if (won && voucherIssued && !string.IsNullOrEmpty(summary.code))
                {
                    string html =
                        $"Score: <b>{score}</b><br/>" +
                        $"Voucher Code: <b>{HttpUtility.HtmlEncode(summary.code)}</b><br/>" +
                        $"Discount: <b>{summary.discount}%</b><br/>" +
                        (summary.expiry.HasValue ? $"Expiry: <b>{summary.expiry.Value:dd MMM yyyy}</b>" : "");

                    lblServerMsg.Text = Msg(
                        $"<b>U Won!</b> Score: <b>{score}</b><br/>" +
                        $"Voucher Code: <b>{HttpUtility.HtmlEncode(summary.code)}</b> ({summary.discount}% off)",
                        false
                    );

                    SafeShowModal("🎉 U Won!", html);
                }
                else if (won)
                {
                    string html =
                        $"Score: <b>{score}</b><br/>" +
                        $"No voucher issued (need higher score or already claimed today).";

                    lblServerMsg.Text = Msg(
                        $"<b>U Won!</b> Score: <b>{score}</b><br/>" +
                        $"No voucher issued (need higher score or already claimed today).",
                        false
                    );

                    SafeShowModal("🎉 U Won!", html);
                }
                else
                {
                    string html =
                        $"Score: <b>{score}</b><br/><b>You Lost</b>, Try again tomorrow.";

                    lblServerMsg.Text = Msg(
                        $"Score: <b>{score}</b><br/><b>You Lost</b>, Try again tomorrow.",
                        true
                    );

                    SafeShowModal("😢 You Lost", html);
                }
            }
            catch (Exception ex)
            {
                // Show a helpful message while you are debugging
                lblServerMsg.Text = Msg("Error saving result. " + HttpUtility.HtmlEncode(ex.Message), true);
                SafeShowModal("Error", "Error saving result.<br/>" + HttpUtility.HtmlEncode(ex.Message));
            }
        }

        // -------------------- Helpers --------------------
        private int SafeInt(string s) => int.TryParse(s, out var x) ? x : 0;

        private string Msg(string html, bool isError)
        {
            string color = isError ? "#991B1B" : "#065F46";
            return $"<div style='margin-left:8px;color:{color};font-weight:900;font-size:14px;line-height:1.35;'>{html}</div>";
        }

        private string Js(string s) => HttpUtility.JavaScriptStringEncode(s ?? "");

        /// <summary>
        /// Safely tries to show the modal even if ScriptManager is missing.
        /// If ScriptManager isn't present, it will still work by using ClientScript.
        /// </summary>
        private void SafeShowModal(string title, string html)
        {
            string script = $"window.openResultModal('{Js(title)}','{Js(html)}');";

            // If ScriptManager exists (UpdatePanel etc.), use it
            if (ScriptManager.GetCurrent(Page) != null)
                ScriptManager.RegisterStartupScript(this, GetType(), "showResultModal", script, true);
            else
                ClientScript.RegisterStartupScript(GetType(), "showResultModal", script, true);
        }

        // ✅ DAILY LIMIT CHECK (uses PlayedAt)
        private bool HasPlayedToday(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM GameSessions
                WHERE UserId=@u
                  AND CAST(PlayedAt AS DATE) = CAST(GETDATE() AS DATE)", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // -------------------- GameSessions --------------------
        private void InsertGameSession(int userId, int score, int saved, int wasted)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO GameSessions (UserId, Score, ItemsSaved, ItemsWasted) VALUES (@u,@s,@sv,@w)", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@s", score);
                cmd.Parameters.AddWithValue("@sv", saved);
                cmd.Parameters.AddWithValue("@w", wasted);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // -------------------- UserRewards --------------------
        private void EnsureUserRewardsRow(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM UserRewards WHERE UserId=@u)
                BEGIN
                    INSERT INTO UserRewards (UserId, TotalPoints, Badges) VALUES (@u, 0, '');
                END", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddPoints(int userId, int pointsToAdd)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE UserRewards SET TotalPoints = TotalPoints + @p WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@p", pointsToAdd);
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // -------------------- Badges --------------------
        private void AwardBadges(int userId, int score, int saved, int wasted)
        {
            if (score >= 200) TryAddBadge(userId, "SURPLUS_SAVER");
            if (wasted == 0 && saved > 0) TryAddBadge(userId, "ZERO_WASTE_HERO");

            int totalSaved = GetTotalSavedAcrossAllSessions(userId);
            if (totalSaved >= 10) TryAddBadge(userId, "WASTE_WARRIOR");
        }

        private int GetTotalSavedAcrossAllSessions(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(ItemsSaved),0) FROM GameSessions WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void TryAddBadge(int userId, string badgeKey)
        {
            string badges = GetBadgesString(userId);
            if (ContainsBadge(badges, badgeKey)) return;

            string updated = string.IsNullOrWhiteSpace(badges) ? badgeKey : (badges + "," + badgeKey);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("UPDATE UserRewards SET Badges=@b WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@b", updated);
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string GetBadgesString(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT Badges FROM UserRewards WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result == null ? "" : result.ToString();
            }
        }

        private bool ContainsBadge(string badges, string badgeKey)
        {
            if (string.IsNullOrWhiteSpace(badges)) return false;

            foreach (var p in badges.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (p.Trim().Equals(badgeKey, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // -------------------- Voucher --------------------
        private bool IssueVoucherIfEligible(int userId, int score)
        {
            // If you changed win to >=50 and want voucher to match:
            // else if (score >= 50) discount = 5;

            int discount = 0;
            if (score >= 120) discount = 10;
            else if (score >= 60) discount = 5;

            if (discount == 0) return false;
            if (HasVoucherToday(userId)) return false;

            string code = "SAVEBITE" + discount + "-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            DateTime expiry = DateTime.Now.AddDays(7);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                UPDATE UserRewards
                SET LastVoucherCode=@c,
                    LastVoucherDiscount=@d,
                    LastVoucherExpiry=@e,
                    LastVoucherIssuedAt=GETDATE()
                WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@c", code);
                cmd.Parameters.AddWithValue("@d", discount);
                cmd.Parameters.AddWithValue("@e", expiry);
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        private bool HasVoucherToday(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM UserRewards
                WHERE UserId=@u
                  AND LastVoucherIssuedAt IS NOT NULL
                  AND CAST(LastVoucherIssuedAt AS DATE) = CAST(GETDATE() AS DATE)", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private (string code, int? discount, DateTime? expiry, string badges) GetRewardsSummary(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT LastVoucherCode, LastVoucherDiscount, LastVoucherExpiry, Badges
                FROM UserRewards
                WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return (null, null, null, "");

                    string code = r["LastVoucherCode"] as string;
                    int? disc = r["LastVoucherDiscount"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["LastVoucherDiscount"]);
                    DateTime? exp = r["LastVoucherExpiry"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["LastVoucherExpiry"]);
                    string badges = r["Badges"] as string ?? "";
                    return (code, disc, exp, badges);
                }
            }
        }
    }
}