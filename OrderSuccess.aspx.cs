using System;
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
                int orderId = GetOrderId();
                if (orderId <= 0)
                {
                    ShowError("Missing orderId. Open like: OrderSuccess.aspx?orderId=123");
                    return;
                }

                lblOrderId.Text = orderId.ToString();

                BindItemsAndTotal(orderId);
                BindPickupAndMap(orderId);
            }
        }

        private int GetOrderId()
        {
            // prefer querystring
            string qs = Request.QueryString["orderId"];
            if (int.TryParse(qs, out int oid)) return oid;

            // fallback if you stored it after checkout
            if (Session["LastOrderID"] != null && int.TryParse(Session["LastOrderID"].ToString(), out int sid))
                return sid;

            return 0;
        }

        private string ConnStr()
        {
            // CHANGE "EcoEatsDb" to your real Web.config connection string name
            return ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
        }

        private void BindItemsAndTotal(int orderId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr()))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        p.ProductName,
                        s.ShopName,
                        oi.Quantity AS Qty,
                        (oi.Quantity * oi.UnitPrice) AS LineTotal
                    FROM OrderItem oi
                    INNER JOIN Products p ON oi.ProductID = p.ProductID
                    INNER JOIN Seller s ON p.SellerID = s.SellerID
                    WHERE oi.OrderID = @oid
                    ORDER BY oi.OrderItemID ASC;

                    SELECT 
                        ISNULL(SUM(oi.Quantity * oi.UnitPrice), 0)
                    FROM OrderItem oi
                    WHERE oi.OrderID = @oid;
                ", con))
                {
                    cmd.Parameters.Add("@oid", SqlDbType.Int).Value = orderId;

                    con.Open();

                    // 1) items
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(r);
                        rptItems.DataSource = dt;
                        rptItems.DataBind();

                        // 2) total (next result set)
                        if (r.NextResult() && r.Read())
                        {
                            decimal total = r.IsDBNull(0) ? 0m : r.GetDecimal(0);
                            lblTotal.Text = total.ToString("0.00");
                        }
                        else
                        {
                            lblTotal.Text = "0.00";
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowError("Could not load order items. Check your OrderItem table columns (Quantity, UnitPrice) and OrderID.");
            }
        }

        private void BindPickupAndMap(int orderId)
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
                    FROM [Order] o
                    INNER JOIN OrderItem oi ON o.OrderID = oi.OrderID
                    INNER JOIN Products p ON oi.ProductID = p.ProductID
                    INNER JOIN Seller s ON p.SellerID = s.SellerID
                    WHERE o.OrderID = @oid
                    ORDER BY oi.OrderItemID ASC;
                ", con))
                {
                    cmd.Parameters.Add("@oid", SqlDbType.Int).Value = orderId;

                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            ShowError("Pickup details not found. Make sure Products.SellerID is filled and Seller exists.");
                            return;
                        }

                        string shopName = r["ShopName"]?.ToString() ?? "";
                        string address = r["Address"]?.ToString() ?? "";
                        string pickupWindow = r["PickupWindow"]?.ToString() ?? "(Not specified)";

                        lblShopName.Text = shopName;
                        lblAddress.Text = address;
                        lblPickupWindow.Text = pickupWindow;

                        string mapSrc;
                        string directionsUrl;

                        // Use lat/lng if available, else fallback to address
                        if (r["Latitude"] != DBNull.Value && r["Longitude"] != DBNull.Value)
                        {
                            decimal lat = (decimal)r["Latitude"];
                            decimal lng = (decimal)r["Longitude"];

                            string latStr = lat.ToString(CultureInfo.InvariantCulture);
                            string lngStr = lng.ToString(CultureInfo.InvariantCulture);

                            mapSrc = $"https://www.google.com/maps?q={latStr},{lngStr}&output=embed";
                            directionsUrl = $"https://www.google.com/maps/dir/?api=1&destination={latStr},{lngStr}";
                        }
                        else
                        {
                            string q = Uri.EscapeDataString(address);
                            mapSrc = $"https://www.google.com/maps?q={q}&output=embed";
                            directionsUrl = $"https://www.google.com/maps/dir/?api=1&destination={q}";
                        }

                        mapFrame.Attributes["src"] = mapSrc;
                        lnkDirections.NavigateUrl = directionsUrl;
                    }
                }
            }
            catch (Exception)
            {
                ShowError("Could not load pickup/map info. Check Seller columns (Address, Latitude, Longitude) and your JOINs.");
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }
}
