using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Business_App_Dev
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "";
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                lblMsg.Text = "Please enter a valid email address.";
                return;
            }

            // Always show generic message for security (don’t reveal if email exists)
            string generic = "If the email exists, a reset link has been sent.";

            if (!AccountExists(email))
            {
                lblMsg.Text = generic;
                return;
            }


            string token = GenerateToken(32); // 64 hex chars
            DateTime expires = DateTime.Now.AddMinutes(15);

            SaveResetToken(email, token, expires);

            string resetUrl = BuildResetUrl(token);

            // Send email (or simulate)
            try
            {
                SendResetEmail(email, resetUrl);
            }
            catch
            {
                // For school/demo: show link if SMTP not configured
                lblMsg.Text = generic + "<br/>Demo link: " + Server.HtmlEncode(resetUrl);
                return;
            }

            lblMsg.Text = generic;
        }

        private bool AccountExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    (SELECT COUNT(1) FROM Users WHERE LOWER(Email)=LOWER(@Email))
                  + (SELECT COUNT(1) FROM Seller WHERE LOWER(Email)=LOWER(@Email))
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }


        private void SaveResetToken(string email, string token, DateTime expiresAt)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO PasswordResets (Email, Token, ExpiresAt, IsUsed)
                VALUES (@Email, @Token, @ExpiresAt, 0)
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string BuildResetUrl(string token)
        {
            // Creates absolute URL like https://localhost:443xx/ResetPassword.aspx?token=...
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");
            return baseUrl + "ResetPassword.aspx?token=" + token;
        }

        private string GenerateToken(int bytes)
        {
            byte[] data = new byte[bytes];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(data);

            // hex
            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }

        private void SendResetEmail(string toEmail, string resetUrl)
        {
            // ✅ You must configure SMTP in Web.config or here.
            // For school, Gmail SMTP works with App Password (not your normal password).
            var msg = new MailMessage();
            msg.To.Add(toEmail);
            msg.Subject = "EcoEats Password Reset";
            msg.Body =
                "You requested a password reset.\n\n" +
                "Click the link below to reset your password (valid for 15 minutes):\n" +
                resetUrl + "\n\n" +
                "If you did not request this, you can ignore this email.";
            msg.IsBodyHtml = false;

            msg.From = new MailAddress("kwayongle54@gmail.com", "EcoEats");
            using (var smtp = CreateSmtpClient())
            {
                smtp.Send(msg);
            }

        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private SmtpClient CreateSmtpClient()
        {
            string appPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD");

            if (string.IsNullOrWhiteSpace(appPassword))
                throw new Exception("EMAIL_APP_PASSWORD is missing. Set it using setx.");

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("kwayongle54@gmail.com", appPassword)
            };

            return smtp;
        }

    }
}