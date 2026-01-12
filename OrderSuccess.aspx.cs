using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Business_App_Dev
{
    public partial class OrderSuccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sessionId = Request.QueryString["session_id"];
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    ShowError("Missing session_id from Stripe redirect.");
                    return;
                }

                // Show a nice "Order ID" (since you don't have Orders table yet)
                lblOrderId.Text = "ORD-" + ShortId(sessionId);

                // Load the purchase snapshot that you saved in Cart.aspx.cs
                var items = Session["PENDING_ORDER_" + sessionId] as List<PurchasedItem>;
                if (items == null || items.Count == 0)
                {
                    ShowError("Order data not found (Session expired). Try paying again, or implement Orders table to persist.");
                    return;
                }

                pnlError.Visible = false;

                rptItems.DataSource = items;
                rptItems.DataBind();

                // Total
                decimal total = 0m;
                if (Session["PENDING_ORDER_TOTAL_" + sessionId] is decimal t)
                    total = t;
                else
                {
                    foreach (var it in items) total += it.LineTotal;
                }
                lblTotal.Text = total.ToString("0.00");

                // Pickup + Map (use first item’s ProductID to find Seller)
                BindPickupAndMap(items[0].ProductID);
            }
        }

        private string ConnStr()
        {
            // CHANGE name if yours is different
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
        }

        private void BindPickupAndMap(int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr()))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1
                        s.ShopName,
                        s.Address,
                        s.PickupWindow,
                        s.Latitude,
                        s.Longitude
                    FROM Products p
                    INNER JOIN Seller s ON p.SellerID = s.SellerID
                    WHERE p.ProductID = @pid;
                ", con))
                {
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;

                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            ShowError("Seller/pickup info not found. Ensure Products.SellerID is filled and Seller table has data.");
                            return;
                        }

                        string shop = r["ShopName"]?.ToString() ?? "-";
                        string addr = r["Address"]?.ToString() ?? "-";
                        string window = r["PickupWindow"] == DBNull.Value ? "(Not specified)" : r["PickupWindow"].ToString();

                        lblShopName.Text = shop;
                        lblAddress.Text = addr;
                        lblPickupWindow.Text = window;

                        // GOOGLE MAPS (NO API KEY) — iframe embed
                        string mapSrc;
                        string dirUrl;

                        if (r["Latitude"] != DBNull.Value && r["Longitude"] != DBNull.Value)
                        {
                            decimal lat = (decimal)r["Latitude"];
                            decimal lng = (decimal)r["Longitude"];

                            string latStr = lat.ToString(CultureInfo.InvariantCulture);
                            string lngStr = lng.ToString(CultureInfo.InvariantCulture);

                            mapSrc = $"https://www.google.com/maps?q={latStr},{lngStr}&output=embed";
                            dirUrl = $"https://www.google.com/maps/dir/?api=1&destination={latStr},{lngStr}";
                        }
                        else
                        {
                            string q = Uri.EscapeDataString(addr);
                            mapSrc = $"https://www.google.com/maps?q={q}&output=embed";
                            dirUrl = $"https://www.google.com/maps/dir/?api=1&destination={q}";
                        }

                        mapFrame.Attributes["src"] = mapSrc;
                        lnkDirections.NavigateUrl = dirUrl;
                    }
                }
            }
            catch (Exception)
            {
                ShowError("Could not load map/pickup info. Check DB connection string + Seller/Product join.");
            }
        }

        private string ShortId(string sessionId)
        {
            // make it look like an order number
            // cs_test_abc... -> use last 10 chars
            if (sessionId.Length <= 10) return sessionId.ToUpperInvariant();
            return sessionId.Substring(sessionId.Length - 10).ToUpperInvariant();
        }

        private void ShowError(string msg)
        {
            pnlError.Visible = true;
            lblError.Text = msg;
        }
    }
}
