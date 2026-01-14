using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using OtpNet;
using QRCoder;

namespace Business_App_Dev
{
    public partial class Enable2FA : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔒 Must already be logged in as Admin
            if (Session["Pending2FAEmail"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string email = Session["Pending2FAEmail"].ToString();

            // ✅ Double-check this email is an admin account (DB check)
            if (!IsAdminEmail(email))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            lblMsg.Text = "";

            if (!IsPostBack)
            {
                // 1️⃣ Generate secret ONCE
                byte[] secretBytes = new byte[20];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(secretBytes);
                }

                string secretBase32 = Base32Encoding.ToString(secretBytes);

                // Store in Session so Verify button uses SAME secret
                Session["2FA_SECRET"] = secretBase32;

                // 2️⃣ Build otpauth URI

                string issuer = "EcoEats";
                string label = $"{issuer}:{email}";

                string otpAuthUri =
                    $"otpauth://totp/{Uri.EscapeDataString(label)}" +
                    $"?secret={secretBase32}&issuer={Uri.EscapeDataString(issuer)}&digits=6";

                // 3️⃣ Generate QR code
                using (var qrGen = new QRCodeGenerator())
                using (var qrData = qrGen.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new PngByteQRCode(qrData))
                {
                    byte[] pngBytes = qrCode.GetGraphic(6);
                    imgQr.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(pngBytes);
                }
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string code = (txtCode.Text ?? "").Trim();

            if (code.Length != 6)
            {
                lblMsg.Text = "Enter the 6-digit code.";
                return;
            }

            // Retrieve SAME secret
            string secretBase32 = Session["2FA_SECRET"] as string;
            if (string.IsNullOrEmpty(secretBase32))
            {
                lblMsg.Text = "Session expired. Reload page.";
                return;
            }

            // Verify TOTP
            var totp = new Totp(Base32Encoding.ToBytes(secretBase32));
            bool ok = totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));

            if (!ok)
            {
                lblMsg.Text = "Invalid code. Try again.";
                return;
            }

            // Save secret + enable 2FA
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Admin
                SET TwoFAEnabled = 1,
                    TwoFASecret = @Secret
                WHERE Email = @Email
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Secret", secretBase32);
                cmd.Parameters.AddWithValue("@Email", Session["Pending2FAEmail"].ToString());

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Session.Remove("2FA_SECRET");
            Response.Redirect("~/Verify2FA.aspx");
        }
        private bool IsAdminEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Admin
                WHERE Email=@Email AND IsActive=1
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

    }
}
