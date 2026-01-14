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
    public partial class EditStoreDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerAuthenticated"] as bool? != true)
            {
                Response.Redirect("SellerLogin.aspx");
                return;

            }

            if (!IsPostBack)
            {
                LoadStoreDetails();
            }


        }

        private void LoadStoreDetails()
        {
            int sellerId = Convert.ToInt32(Session["SellerId"]);
            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT ShopName, Address, PostalCode, PickupWindow
                FROM Seller
                WHERE SellerID = @SellerID
            ", conn))

            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        tbShopName.Text = r["ShopName"].ToString();
                        tbAddress.Text = r["Address"].ToString();
                        tbPostalCode.Text = r["PostalCode"].ToString();
                        tbPickupTime.Text = r["PickupWindow"].ToString();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //try
            //{
                if (!Page.IsValid) return;

                int sellerId = Convert.ToInt32(Session["SellerId"]);
                string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

                using (var conn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(@"
                UPDATE Seller
                SET ShopName=@ShopName,
                    Address=@Address,
                    PostalCode=@PostalCode,
                    PickupWindow=@Pickup
                WHERE SellerID=@SellerID
            ", conn))
                {
                    cmd.Parameters.AddWithValue("@ShopName", tbShopName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", tbAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@PostalCode", tbPostalCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@Pickup", tbPickupTime.Text.Trim());
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        // Update session store name for display
                        Session["SellerStoreName"] = tbShopName.Text.Trim();
                        Response.Redirect("StoreDetails.aspx");
                    }
                    else
                    {
                        vsSummary.HeaderText = "Update failed";
                        vsSummary.Controls.Clear();
                        vsSummary.Controls.Add(new LiteralControl("Could not update store details."));
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    vsSummary.HeaderText = "An error occurred";
            //    vsSummary.Controls.Clear();
            //    vsSummary.Controls.Add(new LiteralControl(ex.Message));
            //}
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("StoreDetails.aspx");
        }
    }
}