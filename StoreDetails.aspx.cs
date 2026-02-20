using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                int sellerId = Convert.ToInt32(Session["SellerId"]);
                LoadStore(sellerId);
            }

           
        }

        private void LoadStore(int sellerId)
        {
            var store = GetStoreDetails(sellerId);
            if (store == null) return;

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

        private string FormatPickupWindowForDisplay(string pickupWindow)
        {
            if (string.IsNullOrWhiteSpace(pickupWindow))
                return "";

            // Old format: "11:00-20:00"
            if (pickupWindow.IndexOf("Default=", StringComparison.OrdinalIgnoreCase) < 0
            && pickupWindow.Contains("-"))
                return $"Daily: {pickupWindow}";

            // Parse new format: Default=.. | Mon=.. | Tue=.. ...
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
                    defaultRange = val; // e.g. 09:00-21:00
                    continue;
                }

                // val can be: Closed / Default / Custom:hh-hh / Custom
                if (val.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                    dayMap[key] = "Closed";
                else if (val.Equals("Default", StringComparison.OrdinalIgnoreCase))
                    dayMap[key] = defaultRange; // show time instead of word Default
                else if (val.StartsWith("Custom", StringComparison.OrdinalIgnoreCase))
                {
                    var idx = val.IndexOf(':');
                    if (idx >= 0) dayMap[key] = val.Substring(idx + 1).Trim(); // hh-hh
                    else dayMap[key] = "Custom";
                }
                else
                {
                    dayMap[key] = val;
                }
            }

            string Get(string day) => dayMap.TryGetValue(day, out var v) ? v : (defaultRange != "" ? defaultRange : "");

            // Nice readable display (minimal effort, no HTML changes)
            return $"Mon: {Get("Mon")}<br/>" +
            $"Tue: {Get("Tue")}<br/>" +
            $"Wed: {Get("Wed")}<br/>" +
            $"Thu: {Get("Thu")}<br/>" +
            $"Fri: {Get("Fri")}<br/>" +
            $"Sat: {Get("Sat")}<br/>" +
            $"Sun: {Get("Sun")}";
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
                // simplest: stop save + show message (or just return)
                // lblError.Text = "Please set Default operating hours.";
                return;
            }

            int sellerId = Convert.ToInt32(Session["SellerId"]);

            string shopName = (tbStoreName.Text ?? "").Trim();
            string address = (tbAddress.Text ?? "").Trim();
            string email = (tbEmail.Text ?? "").Trim();
            string phone = (tbPhone.Text ?? "").Trim();
            string description = (tbDescription.Text ?? "").Trim();
            // Save operating hours into PickupWindow as one string
            string pickupWindow = BuildPickupWindowText();

            using (SqlConnection conn = new SqlConnection(ConnStr))
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
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@SellerID", sellerId);


                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Reload latest data & go back to VIEW mode
            LoadStore(sellerId);
            hfEditMode.Value = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "backToView",
                "toggleEdit(false);", true);
            hfEditMode.Value = "0";


        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int sellerId = Convert.ToInt32(Session["SellerId"]);

            // Reload DB values (discard textbox changes)
            LoadStore(sellerId);
            hfEditMode.Value = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "backToView",
                "toggleEdit(false);", true);

            // Back to VIEW mode
            hfEditMode.Value = "0";

        }


        private string BuildPickupWindowText()
        {
            string defFrom = (tbDefaultFrom.Text ?? "").Trim();
            string defTo = (tbDefaultTo.Text ?? "").Trim();

            if (string.IsNullOrEmpty(defFrom) || string.IsNullOrEmpty(defTo))
                defFrom = defTo = ""; // allow blank, but you can enforce later

            string DayPart(string day, DropDownList mode, TextBox from, TextBox to)
            {
                string m = mode.SelectedValue;

                if (m == "Closed") return $"{day}=Closed";
                if (m == "Default") return $"{day}=Default";

                // Custom
                string f = (from.Text ?? "").Trim();
                string t = (to.Text ?? "").Trim();
                if (string.IsNullOrEmpty(f) || string.IsNullOrEmpty(t))
                    return $"{day}=Custom"; // fallback

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
            // defaults
            tbDefaultFrom.Text = "";
            tbDefaultTo.Text = "";

            void SetMode(DropDownList ddl, string mode)
            {
                if (ddl.Items.FindByValue(mode) != null) ddl.SelectedValue = mode;
                else ddl.SelectedValue = "Default";
            }

            // set all days to Default initially
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
                    // Default=10:00-18:00
                    var times = val.Split('-');
                    if (times.Length == 2)
                    {
                        tbDefaultFrom.Text = times[0].Trim();
                        tbDefaultTo.Text = times[1].Trim();
                    }
                    continue;
                }

                // Day=Closed OR Day=Default OR Day=Custom:hh-hh
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

                    // Custom:10:00-18:00
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
            public string Phone { get; set; } = "";
            public string Description { get; set; } = "";
            public string PostalCode { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PickupWindow { get; set; } = "";
            public string Email { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

    }
}