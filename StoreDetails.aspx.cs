using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class StoreDetails : System.Web.UI.Page
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            lblMsg.CssClass = "sd-msg";

            if (Session["UserRole"]?.ToString() != "Seller" || Session["SellerId"] == null)
            {
                Response.Redirect("~/Login.aspx?role=Seller");
                return;
            }

            if (!IsPostBack)
            {
                int sellerId = Convert.ToInt32(Session["SellerId"]);
                LoadStore(sellerId);
            }
        }

        private void LoadStore(int sellerId)
        {
            var store = GetStoreDetails(sellerId);
            if (store == null) return;

            hfLat.Value = store.Latitude.ToString(CultureInfo.InvariantCulture);
            hfLng.Value = store.Longitude.ToString(CultureInfo.InvariantCulture);
            hfAddr.Value = store.Address;
            hfShopName.Value = store.ShopName;

            hfOldEmail.Value = store.Email;

            lblStoreName.Text = store.ShopName;
            lblPhone.Text = store.Phone;
            lblEmail.Text = store.Email;
            lblDescription.Text = store.Description;
            lblAddress.Text = store.Address;
            lblHours.Text = FormatPickupWindowForDisplay(store.PickupWindow);

            tbStoreName.Text = store.ShopName;
            tbEmail.Text = store.Email;
            tbPhone.Text = store.Phone;
            tbDescription.Text = store.Description;

            SplitAddressForUi(store.Address, store.PostalCode, out string addrLine, out string unit, out string postal);
            tbAddrLine.Text = addrLine;
            tbUnit.Text = unit;
            tbPostal.Text = string.IsNullOrWhiteSpace(store.PostalCode) ? postal : store.PostalCode;

            ApplyPickupWindowToUI(store.PickupWindow);
        }

        private StoreDetailsModel GetStoreDetails(int sellerId)
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    SellerID,
    ShopName,
    Address,
    PostalCode,
    Latitude,
    Longitude,
    PickupWindow,
    Email,
    CreatedAt,
    Phone,
    [Description]
FROM dbo.Seller
WHERE SellerID = @SellerID;", conn))
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
                        PostalCode = r["PostalCode"]?.ToString() ?? "",
                        Latitude = r["Latitude"] != DBNull.Value ? Convert.ToDouble(r["Latitude"]) : 0,
                        Longitude = r["Longitude"] != DBNull.Value ? Convert.ToDouble(r["Longitude"]) : 0,
                        PickupWindow = r["PickupWindow"]?.ToString() ?? "",
                        Email = r["Email"]?.ToString() ?? "",
                        CreatedAt = r["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(r["CreatedAt"]) : DateTime.Now,
                        Phone = r["Phone"]?.ToString() ?? "",
                        Description = r["Description"]?.ToString() ?? ""
                    };
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // keep edit mode if validation fails
            hfEditMode.Value = "1";

            if (string.IsNullOrWhiteSpace(tbDefaultFrom.Text) || string.IsNullOrWhiteSpace(tbDefaultTo.Text))
            {
                lblMsg.Text = "Please set Default operating hours.";
                lblMsg.CssClass = "sd-msg bad";
                return;
            }

            int sellerId = Convert.ToInt32(Session["SellerId"]);

            string shopName = (tbStoreName.Text ?? "").Trim();
            string email = (tbEmail.Text ?? "").Trim();
            string phone = (tbPhone.Text ?? "").Trim();
            string description = (tbDescription.Text ?? "").Trim();
            string pickupWindow = BuildPickupWindowText();

            string addrLine = (tbAddrLine.Text ?? "").Trim();
            string unit = (tbUnit.Text ?? "").Trim();
            string postal = (tbPostal.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(shopName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(addrLine))
            {
                lblMsg.Text = "Please fill in store name, email, and address line.";
                lblMsg.CssClass = "sd-msg bad";
                return;
            }

            if (!IsValidPostal(postal))
            {
                lblMsg.Text = "Postal code must be 6 digits (Singapore).";
                lblMsg.CssClass = "sd-msg bad";
                return;
            }

            if (!string.IsNullOrWhiteSpace(unit) && !IsValidUnit(unit))
            {
                lblMsg.Text = "Unit format should look like #16-1046 (optional).";
                lblMsg.CssClass = "sd-msg bad";
                return;
            }

            // ✅ Get lat/lng from hidden fields (set by Google Geocoder in JS)
            if (!double.TryParse(hfLat.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ||
                !double.TryParse(hfLng.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng) ||
                Math.Abs(lat) < 0.0001 || Math.Abs(lng) < 0.0001)
            {
                lblMsg.Text = "Location not resolved yet. Please try Save again (wait 1–2 seconds for Google Maps to load).";
                lblMsg.CssClass = "sd-msg bad";
                return;
            }

            string fullAddress = ComposeFullAddress(addrLine, unit, postal);

            string oldEmail = (hfOldEmail.Value ?? "").Trim();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                // Seller
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Seller
SET ShopName = @ShopName,
    Address = @Address,
    PostalCode = @PostalCode,
    Latitude = @Lat,
    Longitude = @Lng,
    PickupWindow = @PickupWindow,
    Email = @Email,
    Phone = @Phone,
    [Description] = @Description
WHERE SellerID = @SellerID;", conn))
                {
                    cmd.Parameters.AddWithValue("@ShopName", shopName);
                    cmd.Parameters.AddWithValue("@Address", fullAddress);
                    cmd.Parameters.AddWithValue("@PostalCode", postal);
                    cmd.Parameters.AddWithValue("@Lat", lat);
                    cmd.Parameters.AddWithValue("@Lng", lng);
                    cmd.Parameters.AddWithValue("@PickupWindow", pickupWindow);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    cmd.ExecuteNonQuery();
                }

                // SellerApplications sync (optional)
                using (SqlCommand cmd2 = new SqlCommand(@"
UPDATE dbo.SellerApplications
SET BusinessName = @BusinessName,
    Email = @NewEmail,
    Address = @Address,
    PostalCode = @PostalCode,
    Latitude = @Lat,
    Longitude = @Lng,
    PhoneNumber = @Phone
WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@OldEmail)));", conn))
                {
                    cmd2.Parameters.AddWithValue("@BusinessName", shopName);
                    cmd2.Parameters.AddWithValue("@NewEmail", email);
                    cmd2.Parameters.AddWithValue("@Address", fullAddress);
                    cmd2.Parameters.AddWithValue("@PostalCode", postal);
                    cmd2.Parameters.AddWithValue("@Lat", lat);
                    cmd2.Parameters.AddWithValue("@Lng", lng);
                    cmd2.Parameters.AddWithValue("@Phone", phone);
                    cmd2.Parameters.AddWithValue("@OldEmail", oldEmail);
                    cmd2.ExecuteNonQuery();
                }
            }

            lblMsg.Text = "✅ Saved. Location updated.";
            lblMsg.CssClass = "sd-msg ok";

            LoadStore(sellerId);
            hfEditMode.Value = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "backToView", "toggleEdit(false);", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int sellerId = Convert.ToInt32(Session["SellerId"]);
            LoadStore(sellerId);
            hfEditMode.Value = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "backToView", "toggleEdit(false);", true);
        }

        // =========================
        // Helpers
        // =========================
        private bool IsValidPostal(string postal)
            => Regex.IsMatch((postal ?? "").Trim(), @"^\d{6}$");

        private bool IsValidUnit(string unit)
        {
            string u = (unit ?? "").Trim();
            return Regex.IsMatch(u, @"^#\s*\d{1,3}\s*[-/ ]\s*\d{1,4}$");
        }

        private string ComposeFullAddress(string addrLine, string unit, string postal)
        {
            string a = (addrLine ?? "").Trim();
            string u = (unit ?? "").Trim();
            string p = (postal ?? "").Trim();

            if (!string.IsNullOrWhiteSpace(u))
                a = a + " " + u;

            if (!string.IsNullOrWhiteSpace(p))
                a = a + " Singapore " + p;

            return Regex.Replace(a, @"\s{2,}", " ").Trim();
        }

        private void SplitAddressForUi(string fullAddress, string dbPostal, out string addrLine, out string unit, out string postal)
        {
            addrLine = (fullAddress ?? "").Trim();
            unit = "";
            postal = (dbPostal ?? "").Trim();

            if (string.IsNullOrWhiteSpace(postal))
            {
                var mPostal = Regex.Match(addrLine, @"\b(\d{6})\b");
                if (mPostal.Success) postal = mPostal.Groups[1].Value;
            }

            var mUnit = Regex.Match(addrLine, @"#\s*\d{1,3}\s*[-/ ]\s*\d{1,4}");
            if (mUnit.Success) unit = mUnit.Value.Trim();

            addrLine = Regex.Replace(addrLine, @"\bSingapore\b", "", RegexOptions.IgnoreCase);
            addrLine = Regex.Replace(addrLine, @"\b\d{6}\b", "");
            if (!string.IsNullOrWhiteSpace(unit))
                addrLine = addrLine.Replace(unit, "");

            addrLine = Regex.Replace(addrLine, @"\s{2,}", " ").Trim().Trim(',');
        }

        // =========================
        // PickupWindow logic
        // =========================
        private string FormatPickupWindowForDisplay(string pickupWindow)
        {
            if (string.IsNullOrWhiteSpace(pickupWindow)) return "";

            if (pickupWindow.IndexOf("Default=", StringComparison.OrdinalIgnoreCase) < 0
                && pickupWindow.Contains("-"))
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
                    dayMap[key] = (idx >= 0) ? val.Substring(idx + 1).Trim() : "Custom";
                }
                else dayMap[key] = val;
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
                    ddl.SelectedValue = "Closed";
                else if (val.Equals("Default", StringComparison.OrdinalIgnoreCase))
                    ddl.SelectedValue = "Default";
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

        public class StoreDetailsModel
        {
            public int SellerID { get; set; }
            public string ShopName { get; set; } = "";
            public string Address { get; set; } = "";
            public string PostalCode { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PickupWindow { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Description { get; set; } = "";
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
    }
}