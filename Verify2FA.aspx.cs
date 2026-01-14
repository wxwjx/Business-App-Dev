using System;
using System.Configuration;
using System.Data.SqlClient;
using OtpNet;

namespace Business_App_Dev
{
    public partial class Verify2FA : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        private const int MaxAttempts = 3;
        private const string AttemptKey = "TwoFAAttempts";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (Session["Pending2FAEmail"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // init attempts counter if missing
            if (Session[AttemptKey] == null)
                Session[AttemptKey] = 0;
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string email = Session["Pending2FAEmail"].ToString();
            string code = (txtCode.Text ?? "").Trim().Replace(" ", "");

            // 1) Empty
            if (string.IsNullOrWhiteSpace(code))
            {
                lblMsg.Text = "Code cannot be empty.";
                return;
            }

            // 2) Not 6 digits
            if (code.Length != 6)
            {
                lblMsg.Text = "Code must be 6 digits.";
                return;
            }

            if (!IsAllDigits(code))
            {
                lblMsg.Text = "Code must contain only numbers.";
                return;
            }

            // 3) Check secret exists
            string secret = GetSecret(email);
            if (string.IsNullOrWhiteSpace(secret))
            {
                lblMsg.Text = "2FA is not set up for this account.";
                return;
            }


            // 4) Verify code
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            bool ok = totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));

            if (!ok)
            {
                int attempts = IncrementAttempts();
                int left = MaxAttempts - attempts;

                if (left <= 0)
                {
                    // lock out from 2FA step (simple)
                    Session.Remove("Pending2FAEmail");
                    Session.Remove(AttemptKey);

                    // show warning on login page
                    Response.Redirect("~/Login.aspx?err=2fa_locked");
                    return;
                }

                lblMsg.Text = $"Invalid code. Attempts left: {left}.";
                return;
            }

            // ✅ success: reset attempts + finalize login
            Session.Remove(AttemptKey);
            Session.Remove("Pending2FAEmail");

            Session["UserEmail"] = email;
            Session["UserRole"] = "Admin";

            Response.Redirect("~/EcoEatsAdmin.aspx");
        }

        private int IncrementAttempts()
        {
            int attempts = 0;
            if (Session[AttemptKey] != null)
                int.TryParse(Session[AttemptKey].ToString(), out attempts);

            attempts++;
            Session[AttemptKey] = attempts;
            return attempts;
        }

        private static bool IsAllDigits(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (!char.IsDigit(s[i])) return false;
            return true;
        }

        private string GetSecret(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TwoFASecret
                FROM Admin
                WHERE Email = @Email AND TwoFAEnabled = 1
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return cmd.ExecuteScalar() as string;
            }
        }
    }
}
