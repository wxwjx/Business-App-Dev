using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Business_App_Dev
{
    public partial class RegisterCustomer : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
            // lblOtpMsg exists in ASPX
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string fullName = (txtFullName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string phone = BuildE164Phone();
            string otp = (txtOtp.Text ?? "").Trim();

            string password = txtPassword.Text ?? "";
            string confirm = txtConfirm.Text ?? "";

            lblError.Text = "";

            // 1) Basic required checks
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(otp) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                lblError.Text = "Please fill in all fields (including phone + OTP).";
                return;
            }

            if (!Regex.IsMatch(phone ?? "", @"^\+\d{8,15}$"))
            {
                lblError.Text = "Please enter a valid phone number.";
                return;
            }

            // 2) Email format
            if (!IsValidEmail(email))
            {
                lblError.Text = "Please enter a valid email address.";
                return;
            }

            // 4) Terms checkbox
            if (!chkTerms.Checked)
            {
                lblError.Text = "Please agree to the Terms and Conditions.";
                return;
            }

            // 5) Password strength
            if (!IsStrongPassword(password, out string pwMsg))
            {
                lblError.Text = pwMsg;
                return;
            }

            // 6) Confirm password match
            if (password != confirm)
            {
                lblError.Text = "Password and Confirm Password do not match.";
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

                    // Check duplicate email
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            lblError.Text = "This email is already registered.";
                            return;
                        }
                    }

                    // OPTIONAL: Check duplicate phone
                    using (SqlCommand checkPhoneCmd = new SqlCommand(
                        "SELECT COUNT(1) FROM Users WHERE LTRIM(RTRIM(PhoneNumber)) = LTRIM(RTRIM(@Phone))", conn))
                    {
                        checkPhoneCmd.Parameters.AddWithValue("@Phone", phone);
                        int phoneExists = Convert.ToInt32(checkPhoneCmd.ExecuteScalar());
                        if (phoneExists > 0)
                        {
                            lblError.Text = "This phone number is already registered.";
                            return;
                        }
                    }

                    // Hash password (PBKDF2)
                    string hashedPassword = HashPassword(password);

                    // Insert new customer (ADD PhoneNumber column in DB)
                    using (SqlCommand insertCmd = new SqlCommand(@"
                        INSERT INTO Users (FullName, Email, Password, PhoneNumber, IsPremium, MemberSince)
                        VALUES (@FullName, @Email, @Password, @PhoneNumber, @IsPremium, @MemberSince)
                    ", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FullName", fullName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@PhoneNumber", phone);
                        insertCmd.Parameters.AddWithValue("@IsPremium", false);
                        insertCmd.Parameters.AddWithValue("@MemberSince", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect("~/Login.aspx");
            }
            catch
            {
                lblError.Text = "Something went wrong. Please try again.";
            }
        }

        protected void btnGetOtp_Click(object sender, EventArgs e)
        {
            lblOtpMsg.Text = "";

            string phone = BuildE164Phone();

            string cc = ddlCountryCode.SelectedValue;  // +65 / +60 etc

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

        // ------------------------
        // Twilio Verify helpers
        // ------------------------
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

        private bool IsValidE164(string phone)
        {
            // E.164: + followed by 8 to 15 digits
            return Regex.IsMatch(phone ?? "", @"^\+\d{8,15}$");
        }

        // ------------------------
        // Validation Helpers
        // ------------------------
        private bool IsStrongPassword(string password, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(password))
            {
                message = "Password cannot be empty.";
                return false;
            }

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

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // ------------------------
        // Password Hashing (PBKDF2)
        // Stores: pbkdf2$<iterations>$<saltBase64>$<hashBase64>
        // ------------------------
        private string HashPassword(string password)
        {
            const int iterations = 100000;
            const int saltSize = 16;
            const int keySize = 32;

            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                key = pbkdf2.GetBytes(keySize);
            }

            string saltB64 = Convert.ToBase64String(salt);
            string keyB64 = Convert.ToBase64String(key);

            return $"pbkdf2${iterations}${saltB64}${keyB64}";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static int EmailExists(string email)
        {
            email = (email ?? "").Trim();
            if (string.IsNullOrWhiteSpace(email)) return 0;

            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0 ? 1 : 0;
                }
            }
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
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))",
                    conn))
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
        private string BuildE164Phone()
        {
            // ddlCountryCode: +65 / +60 etc
            string cc = (ddlCountryCode.SelectedValue ?? "").Trim();  // needs asp:DropDownList in ASPX
            string raw = (txtPhone.Text ?? "").Trim();

            // keep digits only (so user can type spaces/dashes and still ok)
            raw = new string(raw.Where(char.IsDigit).ToArray());

            return cc + raw; // e.g. +65 + 84362985 => +6584362985
        }
    }
}
