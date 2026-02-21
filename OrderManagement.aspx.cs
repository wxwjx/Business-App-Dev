//using System;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Web.Security;
//using System.Web.UI;

//namespace Business_App_Dev
//{
//    public partial class OrdersManagement : Page
//    {
//        private static string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Session["UserRole"]?.ToString() != "Seller" || Session["SellerId"] == null)
//            {
//                Response.Redirect("~/Login.aspx?role=Seller");
//                return;
//            }

//            if (!IsPostBack)
//            {
//                ViewState["tab"] = "Pending";
//                LoadOrders();
//            }
//        }

//        protected void Tab_Click(object sender, EventArgs e)
//        {
//            var btn = (System.Web.UI.WebControls.Button)sender;
//            ViewState["tab"] = btn.CommandArgument;
//            LoadOrders();
//        }

//        private void LoadOrders()
//        {
//            int sellerId = Convert.ToInt32(Session["SellerId"]);
//            string tab = (ViewState["tab"]?.ToString() ?? "Pending");

//            lblTab.Text = tab;

//            // If your date column is OrderDate instead of CreatedAt, change it here.
//            string sql = @"
//                SELECT OrderID, CreatedAt, TotalAmount, Status, RejectReason
//                FROM Orders
//                WHERE SellerID = @SellerID
//                  AND Status = @Status
//                ORDER BY CreatedAt DESC;";

//            var dt = new DataTable();

//            using (var conn = new SqlConnection(ConnStr))
//            using (var cmd = new SqlCommand(sql, conn))
//            using (var da = new SqlDataAdapter(cmd))
//            {
//                cmd.Parameters.AddWithValue("@SellerID", sellerId);
//                cmd.Parameters.AddWithValue("@Status", tab);

//                da.Fill(dt);
//            }

//            // Add status css column for badge
//            dt.Columns.Add("StatusCss", typeof(string));
//            foreach (DataRow row in dt.Rows)
//            {
//                row["StatusCss"] = StatusToCss(row["Status"]?.ToString());
//            }

//            gvOrders.DataSource = dt;
//            gvOrders.DataBind();

//            lblMsg.Text = dt.Rows.Count == 0 ? "No orders in this status." : "";
//        }

//        private string StatusToCss(string status)
//        {
//            switch ((status ?? "").Trim())
//            {
//                case "Pending": return "b-pending";
//                case "Accepted": return "b-accepted";
//                case "Preparing": return "b-prep";
//                case "Completed": return "b-completed";
//                case "Rejected": return "b-rejected";
//                default: return "";
//            }
//        }

//        protected void gvOrders_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(e.CommandArgument?.ToString()))
//                return;

//            int orderId = Convert.ToInt32(e.CommandArgument);
//            int sellerId = Convert.ToInt32(Session["SellerId"]);

//            try
//            {
//                if (e.CommandName == "ACCEPT")
//                {
//                    // Pending -> Accepted
//                    UpdateStatus(orderId, sellerId, fromStatus: "Pending", toStatus: "Accepted", rejectReason: null);
//                }
//                else if (e.CommandName == "PREPARE")
//                {
//                    // Accepted -> Preparing
//                    UpdateStatus(orderId, sellerId, fromStatus: "Accepted", toStatus: "Preparing", rejectReason: null);
//                }
//                else if (e.CommandName == "COMPLETE")
//                {
//                    // Preparing -> Completed
//                    UpdateStatus(orderId, sellerId, fromStatus: "Preparing", toStatus: "Completed", rejectReason: null);
//                }
//                else if (e.CommandName == "OPEN_REJECT")
//                {
//                    // Open modal (client-side)
//                    ScriptManager.RegisterStartupScript(this, GetType(), "openReject",
//                        $"openReject({orderId});", true);
//                    return; // don't reload yet
//                }

//                LoadOrders();
//            }
//            catch (Exception ex)
//            {
//                lblMsg.Text = "Error: " + ex.Message;
//            }
//        }

//        protected void btnConfirmReject_Click(object sender, EventArgs e)
//        {
//            int sellerId = Convert.ToInt32(Session["SellerId"]);
//            int orderId;

//            if (!int.TryParse(hfRejectOrderId.Value, out orderId))
//                return;

//            string reason = (tbRejectReason.Text ?? "").Trim();
//            if (string.IsNullOrWhiteSpace(reason))
//            {
//                // keep modal open + show alert
//                ScriptManager.RegisterStartupScript(this, GetType(), "rejErr",
//                    "alert('Reject reason is required.'); openReject(document.getElementById('hfRejectOrderId').value);", true);
//                return;
//            }

//            try
//            {
//                // Pending -> Rejected (with reason)
//                UpdateStatus(orderId, sellerId, fromStatus: "Pending", toStatus: "Rejected", rejectReason: reason);

//                tbRejectReason.Text = "";
//                ScriptManager.RegisterStartupScript(this, GetType(), "closeReject", "closeReject();", true);

//                LoadOrders();
//            }
//            catch (Exception ex)
//            {
//                lblMsg.Text = "Error: " + ex.Message;
//            }
//        }

//        private void UpdateStatus(int orderId, int sellerId, string fromStatus, string toStatus, string rejectReason)
//        {
//            // IMPORTANT:
//            // If your columns are OrderDate / TotalPrice / etc, this doesn't matter here.
//            // Only these columns must exist: OrderID, SellerID, Status, RejectReason.

//            string sql = @"
//                UPDATE Orders
//                SET Status = @ToStatus,
//                    RejectReason = @RejectReason
//                WHERE OrderID = @OrderID
//                  AND SellerID = @SellerID
//                  AND Status = @FromStatus;";

//            using (var conn = new SqlConnection(ConnStr))
//            using (var cmd = new SqlCommand(sql, conn))
//            {
//                cmd.Parameters.AddWithValue("@OrderID", orderId);
//                cmd.Parameters.AddWithValue("@SellerID", sellerId);
//                cmd.Parameters.AddWithValue("@FromStatus", fromStatus);
//                cmd.Parameters.AddWithValue("@ToStatus", toStatus);

//                if (rejectReason == null)
//                    cmd.Parameters.AddWithValue("@RejectReason", DBNull.Value);
//                else
//                    cmd.Parameters.AddWithValue("@RejectReason", rejectReason);

//                conn.Open();
//                int rows = cmd.ExecuteNonQuery();

//                if (rows == 0)
//                    throw new Exception("Order status already changed (refresh and try again).");
//            }
//        }
//    }
//}