using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Twilio;
using Twilio.Rest.Verify.V2.Service;



namespace Business_App_Dev
{
    public partial class RegisterSeller : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string owner = (txtOwnerName.Text ?? "").Trim();
            string store = (txtStoreName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string address = (txtAddress.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";
            string confirm = txtConfirm.Text ?? "";
            string phone = BuildE164Phone();
            string otp = (txtOtp.Text ?? "").Trim();

            // ✅ Read selected food categories
            string categories = string.Join(", ",
                cblCategories.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value));

            if (string.IsNullOrWhiteSpace(categories))
            {
                lblError.Text = "Please select at least one food category.";
                return;
            }



            // ---------- BASIC VALIDATION ----------
            if (string.IsNullOrWhiteSpace(owner) ||
                string.IsNullOrWhiteSpace(store) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(otp) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                lblError.Text = "Please fill in all required fields (including phone + OTP).";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblError.Text = "Please enter a valid email address.";
                return;
            }
            if (!IsValidE164(phone))
            {
                lblError.Text = "Please enter a valid phone number.";
                return;
            }

            if (!chkTerms.Checked)
            {
                lblError.Text = "Please agree to the Terms and Conditions.";
                return;
            }

            if (!IsStrongPassword(password, out string pwMsg))
            {
                lblError.Text = pwMsg;
                return;
            }

            if (password != confirm)
            {
                lblError.Text = "Passwords do not match.";
                return;
            }
            // 7) Ensure OTP was requested for THIS phone
            if ((Session["OtpPhone"] as string) != phone)
            {
                lblError.Text = "OTP phone mismatch. Please request OTP again.";
                return;
            }

            // 8) OTP attempt limit
            int attempts = (Session["OtpAttempts"] as int?) ?? 0;
            if (attempts >= 5)
            {
                lblError.Text = "Too many OTP attempts. Please request a new OTP.";
                return;
            }

            // 9) Verify OTP with Twilio
            bool otpOk;
            try
            {
                otpOk = CheckOtp(phone, otp);
            }
            catch
            {
                otpOk = false;
            }

            if (!otpOk)
            {
                Session["OtpAttempts"] = attempts + 1;
                lblError.Text = "❌ Invalid OTP. Try again.";
                return;
            }

            // OTP verified ✅ clear state
            Session.Remove("OtpAttempts");
            Session.Remove("OtpPhone");
            Session.Remove("OtpLastSent");

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    using (SqlCommand dup = new SqlCommand(@"
                        SELECT
                            (SELECT COUNT(1)
                             FROM Users
                             WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                          + (SELECT COUNT(1)
                             FROM SellerApplications
                             WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                    ", conn))
                    {
                        dup.Parameters.AddWithValue("@Email", email.Trim());
                        int exists = Convert.ToInt32(dup.ExecuteScalar());

                        if (exists > 0)
                        {
                            lblError.Text = "This email is already registered as a customer or seller.";
                            return;
                        }
                    }


                    // ---------- HASH PASSWORD ----------
                    string hashed = HashPassword(password);


                    // ✅ Duplicate phone check (same style as customer)
                    using (SqlCommand checkPhoneCmd = new SqlCommand(@"
                            SELECT
                                (SELECT COUNT(1) FROM Users WHERE LTRIM(RTRIM(PhoneNumber)) = LTRIM(RTRIM(@Phone)))
                              + (SELECT COUNT(1) FROM SellerApplications WHERE LTRIM(RTRIM(PhoneNumber)) = LTRIM(RTRIM(@Phone)))
                        ", conn))
                    {
                        checkPhoneCmd.Parameters.AddWithValue("@Phone", phone);
                        int phoneExists = Convert.ToInt32(checkPhoneCmd.ExecuteScalar());

                        if (phoneExists > 0)
                        {
                            lblError.Text = "This phone number is already registered.";
                            return;
                        }
                    }
                    // ---------- INSERT SELLER APPLICATION ----------
                    using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO SellerApplications
                            (BusinessName, Owner, Email, Address, Category, Status, SubmitDate, PasswordHash, PhoneNumber)
                            VALUES
                            (@BusinessName, @Owner, @Email, @Address, @Category, @Status, @SubmitDate, @PasswordHash, @PhoneNumber)
    

                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BusinessName", store);
                        cmd.Parameters.AddWithValue("@Owner", owner);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@Category", categories);
                        cmd.Parameters.AddWithValue("@Status", "Pending");
                        cmd.Parameters.AddWithValue("@SubmitDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashed);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phone);

                        cmd.ExecuteNonQuery();
                    }

                }

                // ✅ REDIRECT TO WAITING PAGE
                Response.Redirect("~/SellerPending.aspx");
            }
            catch
            {
                lblError.Text = "Something went wrong. Please try again.";
            }
        }

        // =====================
        // HELPERS
        // =====================
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsStrongPassword(string password, out string message)
        {
            message = "";

            if (password.Length < 8)
            {
                message = "Password must be at least 8 characters.";
                return false;
            }

            bool hasLetter = password.Any(char.IsLetter);
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasLetter || !hasNumber || !hasSpecial)
            {
                message = "Password must contain at least 1 letter, 1 number, and 1 special character.";
                return false;
            }

            return true;
        }

        // PBKDF2 (same format as customer)
        private string HashPassword(string password)
        {
            const int iterations = 100000;
            const int saltSize = 16;
            const int keySize = 32;

            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                key = pbkdf2.GetBytes(keySize);

            return $"pbkdf2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }
        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            string email = (txtEmail.Text ?? "").Trim();

            lblEmailStatus.Text = "";

            if (!IsValidEmail(email))
            {
                lblEmailStatus.Text = "Please enter a valid email address.";
                lblEmailStatus.CssClass = "field-msg bad";
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
            SELECT
                (SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
              + (SELECT COUNT(1) FROM SellerApplications WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
        ", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblEmailStatus.Text = "This email is already registered.";
                        lblEmailStatus.CssClass = "field-msg bad";
                    }
                    else
                    {
                        lblEmailStatus.Text = "Email is available ✅";
                        lblEmailStatus.CssClass = "field-msg ok";
                    }
                }
            }
        }

        protected void txtOwnerName_TextChanged(object sender, EventArgs e)
        {

        }
        private string BuildE164Phone()
        {
            string cc = (ddlCountryCode.SelectedValue ?? "").Trim();
            string raw = (txtPhone.Text ?? "").Trim();
            raw = new string(raw.Where(char.IsDigit).ToArray());
            return cc + raw; // e.g. +65 + 84362985 => +6584362985
        }

        private bool IsValidE164(string phone)
        {
            return Regex.IsMatch(phone ?? "", @"^\+\d{8,15}$");
        }

        private void SendOtpSms(string phone)
        {
            string sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            string verifySid = Environment.GetEnvironmentVariable("TWILIO_VERIFY_SID");

            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(verifySid))
                throw new Exception("Twilio env vars missing.");

            TwilioClient.Init(sid, token);

            VerificationResource.Create(
                to: phone,
                channel: "sms",
                pathServiceSid: verifySid
            );
        }

        private bool CheckOtp(string phone, string code)
        {
            string sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            string verifySid = Environment.GetEnvironmentVariable("TWILIO_VERIFY_SID");

            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(verifySid))
                throw new Exception("Twilio env vars missing.");

            TwilioClient.Init(sid, token);

            var check = VerificationCheckResource.Create(
                to: phone,
                code: code,
                pathServiceSid: verifySid
            );

            return string.Equals(check.Status, "approved", StringComparison.OrdinalIgnoreCase);
        }
        protected void btnGetOtp_Click(object sender, EventArgs e)
        {
            lblOtpMsg.Text = "";

            string phone = BuildE164Phone();

            if (!IsValidE164(phone))
            {
                lblOtpMsg.Text = "❌ Invalid phone number. Please enter a valid number for "
                                 + ddlCountryCode.SelectedItem.Text + ".";
                return;
            }

            // Rate limit: 30s cooldown
            if (Session["OtpLastSent"] is DateTime last && (DateTime.Now - last).TotalSeconds < 30)
            {
                lblOtpMsg.Text = "⏳ Please wait 30 seconds before requesting again.";
                return;
            }

            try
            {
                SendOtpSms(phone);

                Session["OtpLastSent"] = DateTime.Now;
                Session["OtpPhone"] = phone;     // bind OTP to this phone
                Session["OtpAttempts"] = 0;      // reset attempts on new OTP request

                lblOtpMsg.Text = "✅ OTP sent. Please check your SMS.";
            }
            catch
            {
                lblOtpMsg.Text = "❌ Failed to send OTP";
            }
        }

    }
}