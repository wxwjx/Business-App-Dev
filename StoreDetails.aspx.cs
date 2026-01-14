using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//namespace Business_App_Dev
//{
//    public partial class StoreDetails : System.Web.UI.Page
//    {
//        string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                LoadStoreDetails();
//            }
//        }

//        void LoadStoreDetails()
//        {
//            using (SqlConnection conn = new SqlConnection(connStr))
//            {
//                string sql = "SELECT TOP 1 ShopName, Address, PostalCode, PickupWindow FROM Seller";
//                SqlCommand cmd = new SqlCommand(sql, conn);

//                conn.Open();
//                SqlDataReader dr = cmd.ExecuteReader();

//                if (dr.Read())
//                {
//                    lblShopName.Text = dr["ShopName"].ToString();
//                    lblAddress.Text = dr["Address"].ToString();
//                    lblPostalCode.Text = dr["PostalCode"].ToString();
//                    lblPickupTiming.Text = dr["PickupWindow"].ToString();
//                }
//            }
//        }
//    }
//}

namespace Business_App_Dev
{
    public partial class StoreDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerAuthenticated"] == null || !(bool)Session["SellerAuthenticated"])
            {
                Response.Redirect("~/SellerLogin.aspx");
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

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ShopName, Address, PostalCode, PickupWindow
                FROM Seller
                WHERE SellerID = @SellerID
            ", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", sellerId);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        lblShopName.Text = r["ShopName"].ToString();
                        lblAddress.Text = r["Address"].ToString();
                        lblPostalCode.Text = r["PostalCode"].ToString();
                        lblPickupTiming.Text = r["PickupWindow"].ToString();
                    }
                }
            }
        }

        protected void Img_Edit_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("EditStoreDetails.aspx");
        }
    }
}


