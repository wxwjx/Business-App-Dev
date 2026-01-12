using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Business_App_Dev
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        private const string CART_KEY = "CART";

        private int CurrentProductId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && CurrentProductId > 0)
            {
                LoadProduct(CurrentProductId);
            }
        }

        private void LoadProduct(int id)
        {
            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;

                    lblName.Text = r["ProductName"].ToString();
                    lblSubtitle.Text = r["Subtitle"].ToString();
                    lblDescription.Text = r["Subtitle"].ToString();
                    imgProduct.ImageUrl = r["ImageUrl"].ToString();

                    decimal now = (decimal)r["Price"];
                    decimal old = r["PriceOld"] != DBNull.Value ? (decimal)r["PriceOld"] : now;

                    lblPriceNow.Text = now.ToString("0.00");
                    lblPriceOld.Text = old.ToString("0.00");
                    lblSave.Text = (old - now).ToString("0.00");

                    lblRating.Text = r["Rating"].ToString();
                    lblReviews.Text = r["Reviews"].ToString();
                    lblDistance.Text = r["DistanceKm"].ToString();
                    lblExpiry.Text = r["ExpiryHours"].ToString();
                    lblCO2.Text = r["CO2Saved"].ToString();
                    lblDiscount.Text = r["DiscountPercent"].ToString();
                }
            }
        }

        private List<CartItem> GetCart()
        {
            var cart = Session[CART_KEY] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CART_KEY] = cart;
            }
            return cart;
        }

        private void AddItemToCart()
        {
            int qty = Math.Max(1, int.Parse(txtQty.Text));
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductID == CurrentProductId);
            if (item != null)
            {
                item.Quantity += qty;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = CurrentProductId,
                    ProductName = lblName.Text,
                    Subtitle = lblSubtitle.Text,
                    ImageUrl = imgProduct.ImageUrl,
                    PriceNow = decimal.Parse(lblPriceNow.Text),
                    PriceOld = decimal.Parse(lblPriceOld.Text),
                    CO2SavedPerMeal = double.Parse(lblCO2.Text),
                    Quantity = qty
                });
            }

            Session[CART_KEY] = cart;
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            int q = int.Parse(txtQty.Text);
            txtQty.Text = Math.Max(1, q - 1).ToString();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int q = int.Parse(txtQty.Text);
            txtQty.Text = Math.Min(99, q + 1).ToString();
        }

        // Add to Cart (toast only)
        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            AddItemToCart();
        }

        // Buy Now (redirect)
        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            AddItemToCart();
            Response.Redirect("Cart.aspx");
        }
    }
}
