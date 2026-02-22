using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace Business_App_Dev
{
    public partial class StoreDetails : System.Web.UI.Page
    {
        private int sellerId;
        private static string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"]?.ToString() != "Seller" || Session["SellerId"] == null)
            {
                Response.Redirect("~/Login.aspx?role=Seller");
                return;
            }

            if (!IsPostBack)
            {
                sellerId = Convert.ToInt32(Session["SellerId"]);
                LoadStore(sellerId);
            }
        }

        private void LoadStore(int sellerId)
        {
            var store = GetStoreDetails(sellerId);
            if (store == null) return;

            hfLat.Value = store.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            hfLng.Value = store.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            hfAddr.Value = store.Address;
            hfShopName.Value = store.ShopName;

            lblStoreName.Text = store.ShopName;
            lblPhone.Text = store.Phone;
            lblEmail.Text = store.Email;
            lblDescription.Text = store.Description;
            lblAddress.Text = store.Address;
            lblHours.Text = FormatPickupWindowForDisplay(store.PickupWindow);

            tbStoreName.Text = store.ShopName;
            tbAddress.Text = store.Address;
            tbEmail.Text = store.Email;
            tbPhone.Text = store.Phone;
            tbDescription.Text = store.Description;

            ApplyPickupWindowToUI(store.PickupWindow);
        }

        private StoreDetailsModel GetStoreDetails(int sellerId)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT SellerID, ShopName, Address, PostalCode,
       Latitude, Longitude, PickupWindow, Email, CreatedAt,
       Phone, Description
FROM Seller
WHERE SellerID = @SellerID", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new StoreDetailsModel
                    {
                        SellerID = Convert.ToInt32(r["SellerID"]),
                        ShopName = r["ShopName"]?.ToString() ?? "",
                        Address = r["Address"]?.ToString() ?? "",
                        Phone = r["Phone"]?.ToString() ?? "",
                        Description = r["Description"]?.ToString() ?? "",
                        PostalCode = r["PostalCode"]?.ToString() ?? "",
                        Latitude = r["Latitude"] != DBNull.Value ? Convert.ToDouble(r["Latitude"]) : 0,
                        Longitude = r["Longitude"] != DBNull.Value ? Convert.ToDouble(r["Longitude"]) : 0,
                        PickupWindow = r["PickupWindow"]?.ToString() ?? "",
                        Email = r["Email"]?.ToString() ?? "",
                        CreatedAt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now
                    };
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbDefaultFrom.Text) || string.IsNullOrWhiteSpace(tbDefaultTo.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "err",
                    "alert('Please set default operating hours.');", true);
                return;
            }

            int sellerId = Convert.ToInt32(Session["SellerId"]);

            string shopName = (tbStoreName.Text ?? "").Trim();
            string address = (tbAddress.Text ?? "").Trim();
            string newEmail = (tbEmail.Text ?? "").Trim();
            string phone = (tbPhone.Text ?? "").Trim();
            string description = (tbDescription.Text ?? "").Trim();
            string pickupWindow = BuildPickupWindowText();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                string oldEmail = "";
                using (SqlCommand getCmd = new SqlCommand("SELECT Email FROM Seller WHERE SellerID = @SellerID", conn))
                {
                    getCmd.Parameters.AddWithValue("@SellerID", sellerId);
                    oldEmail = getCmd.ExecuteScalar()?.ToString() ?? "";
                }

                using (SqlCommand cmd = new SqlCommand(@"
UPDATE Seller
SET ShopName = @ShopName,
    Address = @Address,
    PickupWindow = @PickupWindow,
    Email = @Email,
    Phone = @Phone,
    Description = @Description
WHERE SellerID = @SellerID;", conn))
                {
                    cmd.Parameters.AddWithValue("@ShopName", shopName);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@PickupWindow", pickupWindow);
                    cmd.Parameters.AddWithValue("@Email", newEmail);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd2 = new SqlCommand(@"
UPDATE SellerApplications
SET BusinessName = @BusinessName,
    Address = @Address,
    Email = @NewEmail,
    PhoneNumber = @Phone
WHERE Email = @OldEmail;", conn))
                {
                    cmd2.Parameters.AddWithValue("@BusinessName", shopName);
                    cmd2.Parameters.AddWithValue("@Address", address);
                    cmd2.Parameters.AddWithValue("@NewEmail", newEmail);
                    cmd2.Parameters.AddWithValue("@Phone", phone);
                    cmd2.Parameters.AddWithValue("@OldEmail", oldEmail);
                    cmd2.ExecuteNonQuery();
                }
            }

            LoadStore(sellerId);

            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "backToView", "toggleEdit(false);", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int sellerId = Convert.ToInt32(Session["SellerId"]);
            LoadStore(sellerId);

            hfEditMode.Value = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "backToView", "toggleEdit(false);", true);
        }

        private string BuildPickupWindowText()
        {
            string defFrom = (tbDefaultFrom.Text ?? "").Trim();
            string defTo = (tbDefaultTo.Text ?? "").Trim();

            string DayPart(string day, DropDownList mode, TextBox from, TextBox to)
            {
                string m = mode.SelectedValue;

                if (m == "Closed") return $"{day}=Closed";
                if (m == "Default") return $"{day}=Default";

                string f = (from.Text ?? "").Trim();
                string t = (to.Text ?? "").Trim();
                if (string.IsNullOrEmpty(f) || string.IsNullOrEmpty(t))
                    return $"{day}=Custom";

                return $"{day}=Custom:{f}-{t}";
            }

            var parts = new List<string>
            {
                $"Default={defFrom}-{defTo}",
                DayPart("Mon", ddlMonMode, tbMonFrom, tbMonTo),
                DayPart("Tue", ddlTueMode, tbTueFrom, tbTueTo),
                DayPart("Wed", ddlWedMode, tbWedFrom, tbWedTo),
                DayPart("Thu", ddlThuMode, tbThuFrom, tbThuTo),
                DayPart("Fri", ddlFriMode, tbFriFrom, tbFriTo),
                DayPart("Sat", ddlSatMode, tbSatFrom, tbSatTo),
                DayPart("Sun", ddlSunMode, tbSunFrom, tbSunTo),
            };

            return string.Join(" | ", parts);
        }

        private void ApplyPickupWindowToUI(string pickupWindow)
        {
            tbDefaultFrom.Text = "";
            tbDefaultTo.Text = "";

            void SetMode(DropDownList ddl, string mode)
            {
                if (ddl.Items.FindByValue(mode) != null) ddl.SelectedValue = mode;
                else ddl.SelectedValue = "Default";
            }

            SetMode(ddlMonMode, "Default");
            SetMode(ddlTueMode, "Default");
            SetMode(ddlWedMode, "Default");
            SetMode(ddlThuMode, "Default");
            SetMode(ddlFriMode, "Default");
            SetMode(ddlSatMode, "Default");
            SetMode(ddlSunMode, "Default");

            if (string.IsNullOrWhiteSpace(pickupWindow)) return;

            var chunks = pickupWindow.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => x.Trim());

            foreach (var c in chunks)
            {
                var kv = c.Split(new[] { '=' }, 2);
                if (kv.Length != 2) continue;

                string key = kv[0].Trim();
                string val = kv[1].Trim();

                if (key.Equals("Default", StringComparison.OrdinalIgnoreCase))
                {
                    var times = val.Split('-');
                    if (times.Length == 2)
                    {
                        tbDefaultFrom.Text = times[0].Trim();
                        tbDefaultTo.Text = times[1].Trim();
                    }
                    continue;
                }

                DropDownList ddl = null;
                TextBox from = null, to = null;

                switch (key)
                {
                    case "Mon": ddl = ddlMonMode; from = tbMonFrom; to = tbMonTo; break;
                    case "Tue": ddl = ddlTueMode; from = tbTueFrom; to = tbTueTo; break;
                    case "Wed": ddl = ddlWedMode; from = tbWedFrom; to = tbWedTo; break;
                    case "Thu": ddl = ddlThuMode; from = tbThuFrom; to = tbThuTo; break;
                    case "Fri": ddl = ddlFriMode; from = tbFriFrom; to = tbFriTo; break;
                    case "Sat": ddl = ddlSatMode; from = tbSatFrom; to = tbSatTo; break;
                    case "Sun": ddl = ddlSunMode; from = tbSunFrom; to = tbSunTo; break;
                }

                if (ddl == null) continue;

                if (val.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                {
                    ddl.SelectedValue = "Closed";
                }
                else if (val.Equals("Default", StringComparison.OrdinalIgnoreCase))
                {
                    ddl.SelectedValue = "Default";
                }
                else if (val.StartsWith("Custom", StringComparison.OrdinalIgnoreCase))
                {
                    ddl.SelectedValue = "Custom";
                    var idx = val.IndexOf(':');
                    if (idx >= 0)
                    {
                        var times = val.Substring(idx + 1).Split('-');
                        if (times.Length == 2)
                        {
                            from.Text = times[0].Trim();
                            to.Text = times[1].Trim();
                        }
                    }
                }
            }
        }

        private string FormatPickupWindowForDisplay(string pickupWindow)
        {
            if (string.IsNullOrWhiteSpace(pickupWindow))
                return "";

            if (pickupWindow.IndexOf("Default=", StringComparison.OrdinalIgnoreCase) < 0 && pickupWindow.Contains("-"))
                return $"Daily: {pickupWindow}";

            string defaultRange = "";
            var dayMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var chunks = pickupWindow.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => x.Trim());

            foreach (var c in chunks)
            {
                var kv = c.Split(new[] { '=' }, 2);
                if (kv.Length != 2) continue;

                var key = kv[0].Trim();
                var val = kv[1].Trim();

                if (key.Equals("Default", StringComparison.OrdinalIgnoreCase))
                {
                    defaultRange = val;
                    continue;
                }

                if (val.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                    dayMap[key] = "Closed";
                else if (val.Equals("Default", StringComparison.OrdinalIgnoreCase))
                    dayMap[key] = defaultRange;
                else if (val.StartsWith("Custom", StringComparison.OrdinalIgnoreCase))
                {
                    var idx = val.IndexOf(':');
                    dayMap[key] = idx >= 0 ? val.Substring(idx + 1).Trim() : "Custom";
                }
                else
                    dayMap[key] = val;
            }

            string Get(string day) => dayMap.TryGetValue(day, out var v) ? v : (defaultRange != "" ? defaultRange : "");

            return $"Mon: {Get("Mon")}<br/>" +
                   $"Tue: {Get("Tue")}<br/>" +
                   $"Wed: {Get("Wed")}<br/>" +
                   $"Thu: {Get("Thu")}<br/>" +
                   $"Fri: {Get("Fri")}<br/>" +
                   $"Sat: {Get("Sat")}<br/>" +
                   $"Sun: {Get("Sun")}";
        }

        public class StoreDetailsModel
        {
            public int SellerID { get; set; }
            public string ShopName { get; set; } = "";
            public string Address { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Description { get; set; } = "";
            public string PostalCode { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PickupWindow { get; set; } = "";
            public string Email { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            FormsAuthentication.SignOut();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var auth = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                auth.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(auth);
            }

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var s = new HttpCookie("ASP.NET_SessionId", "");
                s.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(s);
            }

            Response.Redirect("~/login.aspx?role=Seller", true);
        }

        private string HashPasswordPbkdf2(string password)
        {
            const int iterations = 100000;
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return $"pbkdf2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
            }
        }

        protected void btnUpdateStorePassword_Click(object sender, EventArgs e)
        {
            lblStorePwMsg.Style["display"] = "none";
            lblStorePwMsg.Text = "";

            string sellerEmail = (Session["SellerEmail"] as string);

            if (string.IsNullOrWhiteSpace(sellerEmail))
                sellerEmail = (lblEmail.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(sellerEmail))
            {
                lblStorePwMsg.Style["display"] = "block";
                lblStorePwMsg.Style["background"] = "#FEF2F2";
                lblStorePwMsg.Style["border"] = "1px solid #FCA5A5";
                lblStorePwMsg.Style["color"] = "#991B1B";
                lblStorePwMsg.Text = "❌ Cannot detect seller email. Please log in again.";
                return;
            }

            string pw = (txtStorePw.Text ?? "").Trim();
            string cf = (txtStorePwConfirm.Text ?? "").Trim();

            bool hasLen = pw.Length >= 8;
            bool hasLetter = System.Text.RegularExpressions.Regex.IsMatch(pw, "[A-Za-z]");
            bool hasNum = System.Text.RegularExpressions.Regex.IsMatch(pw, "[0-9]");
            bool hasSpecial = System.Text.RegularExpressions.Regex.IsMatch(pw, "[^A-Za-z0-9]");

            if (!hasLen || !hasLetter || !hasNum || !hasSpecial)
            {
                lblStorePwMsg.Style["display"] = "block";
                lblStorePwMsg.Style["background"] = "#FEF2F2";
                lblStorePwMsg.Style["border"] = "1px solid #FCA5A5";
                lblStorePwMsg.Style["color"] = "#991B1B";
                lblStorePwMsg.Text = "❌ Password does not meet the requirements.";
                return;
            }

            if (pw != cf)
            {
                lblStorePwMsg.Style["display"] = "block";
                lblStorePwMsg.Style["background"] = "#FEF2F2";
                lblStorePwMsg.Style["border"] = "1px solid #FCA5A5";
                lblStorePwMsg.Style["color"] = "#991B1B";
                lblStorePwMsg.Text = "❌ Confirm password does not match.";
                return;
            }

            try
            {
                string newHash = HashPasswordPbkdf2(pw);

                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    string sql = @"
UPDATE SellerApplications
SET PasswordHash = @Pw
WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Pw", newHash);
                        cmd.Parameters.AddWithValue("@Email", sellerEmail);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            lblStorePwMsg.Style["display"] = "block";
                            lblStorePwMsg.Style["background"] = "#FEF2F2";
                            lblStorePwMsg.Style["border"] = "1px solid #FCA5A5";
                            lblStorePwMsg.Style["color"] = "#991B1B";
                            lblStorePwMsg.Text = "❌ Update failed. Seller application not found.";
                            return;
                        }
                    }
                }

                lblStorePwMsg.Style["display"] = "block";
                lblStorePwMsg.Style["background"] = "#ECFDF5";
                lblStorePwMsg.Style["border"] = "1px solid #86EFAC";
                lblStorePwMsg.Style["color"] = "#065F46";
                lblStorePwMsg.Text = "✅ Password changed successfully.";

                txtStorePw.Text = "";
                txtStorePwConfirm.Text = "";
                lblStorePassword.Text = "•••••••• (secured)";
            }
            catch (Exception ex)
            {
                lblStorePwMsg.Style["display"] = "block";
                lblStorePwMsg.Style["background"] = "#FEF2F2";
                lblStorePwMsg.Style["border"] = "1px solid #FCA5A5";
                lblStorePwMsg.Style["color"] = "#991B1B";
                lblStorePwMsg.Text = "❌ Server error: " + ex.Message;
            }
        }
    }
}