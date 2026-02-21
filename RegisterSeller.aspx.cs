using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
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

        // =========================
        // SUBMIT APPLICATION
        // =========================
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

            string categories = string.Join(", ",
                cblCategories.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value));

            if (string.IsNullOrWhiteSpace(categories))
            {
                lblError.Text = "Please select at least one food category.";
                return;
            }

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

            if ((Session["OtpPhone"] as string) != phone)
            {
                lblError.Text = "OTP phone mismatch. Please request OTP again.";
                return;
            }

            int attempts = (Session["OtpAttempts"] as int?) ?? 0;
            if (attempts >= 5)
            {
                lblError.Text = "Too many OTP attempts. Please request a new OTP.";
                return;
            }

            bool otpOk;
            try { otpOk = CheckOtp(phone, otp); }
            catch { otpOk = false; }

            if (!otpOk)
            {
                Session["OtpAttempts"] = attempts + 1;
                lblError.Text = "❌ Invalid OTP. Try again.";
                return;
            }

            Session.Remove("OtpAttempts");
            Session.Remove("OtpPhone");
            Session.Remove("OtpLastSent");

            // ✅ OneMap geocode
            double lat, lng;
            string postal, geoErr;

            if (!TryGeocodeOneMap(address, out lat, out lng, out postal, out geoErr))
            {
                lblError.Text = "Address not found. Please enter a more specific SG address. (" + geoErr + ")";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    // DUP EMAIL (Users + SellerApplications)
                    using (SqlCommand dup = new SqlCommand(@"
SELECT
    (SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
  + (SELECT COUNT(1) FROM SellerApplications WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
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

                    // DUP PHONE (Users + SellerApplications)
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

                    string hashed = HashPassword(password);

                    // INSERT application
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.SellerApplications
(BusinessName, Owner, Email, Address, Category, Status, SubmitDate, PasswordHash, PhoneNumber, PostalCode, Latitude, Longitude)
VALUES
(@BusinessName, @Owner, @Email, @Address, @Category, @Status, @SubmitDate, @PasswordHash, @PhoneNumber, @PostalCode, @Latitude, @Longitude)
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

                        cmd.Parameters.AddWithValue("@PostalCode", postal);
                        cmd.Parameters.AddWithValue("@Latitude", lat);
                        cmd.Parameters.AddWithValue("@Longitude", lng);

                        cmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect("~/SellerPending.aspx");
            }
            catch
            {
                lblError.Text = "Something went wrong. Please try again.";
            }
        }

        // =====================
        // EMAIL LIVE CHECK
        // =====================
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

        protected void txtOwnerName_TextChanged(object sender, EventArgs e) { }

        // =====================
        // OTP
        // =====================
        protected void btnGetOtp_Click(object sender, EventArgs e)
        {
            lblOtpMsg.Text = "";

            string phone = BuildE164Phone();

            if (!IsValidE164(phone))
            {
                lblOtpMsg.Text = "❌ Invalid phone number. Please enter a valid number for " + ddlCountryCode.SelectedItem.Text + ".";
                return;
            }

            if (Session["OtpLastSent"] is DateTime last && (DateTime.Now - last).TotalSeconds < 30)
            {
                lblOtpMsg.Text = "⏳ Please wait 30 seconds before requesting again.";
                return;
            }

            try
            {
                SendOtpSms(phone);

                Session["OtpLastSent"] = DateTime.Now;
                Session["OtpPhone"] = phone;
                Session["OtpAttempts"] = 0;

                lblOtpMsg.Text = "✅ OTP sent. Please check your SMS.";
            }
            catch
            {
                lblOtpMsg.Text = "❌ Failed to send OTP";
            }
        }

        private void SendOtpSms(string phone)
        {
            string sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            string verifySid = Environment.GetEnvironmentVariable("TWILIO_VERIFY_SID");

            if (string.IsNullOrWhiteSpace(sid) ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(verifySid))
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

            if (string.IsNullOrWhiteSpace(sid) ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(verifySid))
                throw new Exception("Twilio env vars missing.");

            TwilioClient.Init(sid, token);

            var check = VerificationCheckResource.Create(
                to: phone,
                code: code,
                pathServiceSid: verifySid
            );

            return string.Equals(check.Status, "approved", StringComparison.OrdinalIgnoreCase);
        }

        // =====================
        // VALIDATION HELPERS
        // =====================
        private bool IsValidEmail(string email)
            => Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private bool IsStrongPassword(string password, out string message)
        {
            message = "";
            if ((password ?? "").Length < 8) { message = "Password must be at least 8 characters."; return false; }

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

        private string BuildE164Phone()
        {
            string cc = (ddlCountryCode.SelectedValue ?? "").Trim();
            string raw = (txtPhone.Text ?? "").Trim();
            raw = new string(raw.Where(char.IsDigit).ToArray());
            return cc + raw;
        }

        private bool IsValidE164(string phone)
            => Regex.IsMatch(phone ?? "", @"^\+\d{8,15}$");

        // PBKDF2
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

        // =====================
        // OneMap Geocoding
        // =====================
        private string SanitizeSgAddressForOneMap(string input)
        {
            string s = (input ?? "").Trim();
            s = Regex.Replace(s, @"#\s*\d+\s*[-]\s*\d+", "", RegexOptions.IgnoreCase);
            s = s.Replace(",", " ");
            s = Regex.Replace(s, @"\s{2,}", " ").Trim();

            if (!s.ToLowerInvariant().Contains("singapore"))
                s += " Singapore";

            return s.Trim();
        }

        private class OneMapSearchResponse { public System.Collections.Generic.List<OneMapResult> results { get; set; } }
        private class OneMapResult
        {
            public string LATITUDE { get; set; }
            public string LONGITUDE { get; set; }
            public string POSTAL { get; set; }
        }

        private bool TryGeocodeOneMap(string address, out double lat, out double lng, out string postal, out string err)
        {
            lat = 0; lng = 0; postal = ""; err = "";

            string q = SanitizeSgAddressForOneMap(address);
            if (string.IsNullOrWhiteSpace(q))
            {
                err = "Empty address";
                return false;
            }

            string url = "https://developers.onemap.sg/commonapi/search?searchVal=" +
                         Uri.EscapeDataString(q) +
                         "&returnGeom=Y&getAddrDetails=Y&pageNum=1";

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = 8000;
                req.UserAgent = "EcoEats/1.0 (student project)";

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    string json = sr.ReadToEnd();
                    var js = new JavaScriptSerializer();
                    var data = js.Deserialize<OneMapSearchResponse>(json);

                    if (data?.results == null || data.results.Count == 0)
                    {
                        err = "No results";
                        return false;
                    }

                    var top = data.results[0];

                    if (!double.TryParse(top.LATITUDE, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out lat))
                    { err = "Bad LATITUDE"; return false; }

                    if (!double.TryParse(top.LONGITUDE, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out lng))
                    { err = "Bad LONGITUDE"; return false; }

                    postal = (top.POSTAL ?? "").Trim();
                    if (postal.Length == 0)
                    { err = "Postal not returned"; return false; }

                    return true;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
    }
}