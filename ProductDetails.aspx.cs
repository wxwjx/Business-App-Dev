using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;


namespace Business_App_Dev
{
    public partial class ProductDetails : System.Web.UI.Page
    {
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
            if (!IsPostBack)
            {
                txtQty.Text = "1";

                if (CurrentProductId > 0)
                    LoadProduct(CurrentProductId);
            }
        }

        private void LoadProduct(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;

                    lblName.Text = reader["ProductName"].ToString();
                    lblSubtitle.Text = reader["Subtitle"] != DBNull.Value ? reader["Subtitle"].ToString() : "Fresh Market";

                    imgProduct.ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "";

                    decimal priceNow = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0;
                    decimal priceOld = reader["PriceOld"] != DBNull.Value ? (decimal)reader["PriceOld"] : 0;

                    double rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0;
                    int reviews = reader["Reviews"] != DBNull.Value ? Convert.ToInt32(reader["Reviews"]) : 0;
                    int distance = reader["DistanceKm"] != DBNull.Value ? Convert.ToInt32(reader["DistanceKm"]) : 0;
                    int expiry = reader["ExpiryHours"] != DBNull.Value ? Convert.ToInt32(reader["ExpiryHours"]) : 0;
                    double co2 = reader["CO2Saved"] != DBNull.Value ? Convert.ToDouble(reader["CO2Saved"]) : 0;
                    int discount = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0;

                    lblPriceNow.Text = priceNow.ToString("0.00");
                    lblPriceOld.Text = priceOld.ToString("0.00");
                    lblSave.Text = Math.Max(0, priceOld - priceNow).ToString("0.00");

                    lblRating.Text = rating.ToString("0.0");
                    lblReviews.Text = reviews.ToString();
                    lblDistance.Text = distance.ToString();
                    lblExpiry.Text = expiry.ToString();
                    lblCO2.Text = co2.ToString("0.0");
                    lblDiscount.Text = discount.ToString();

                    // If you don't have description column, reuse subtitle
                    lblDescription.Text = reader["Subtitle"] != DBNull.Value
                        ? reader["Subtitle"].ToString()
                        : "Fresh seasonal food at amazing prices.";
                }
            }
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;
            qty = Math.Max(1, qty - 1);
            txtQty.Text = qty.ToString();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;
            qty = Math.Min(99, qty + 1);
            txtQty.Text = qty.ToString();
        }

        private const string CART_KEY = "CART";

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

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;
            qty = Math.Max(1, Math.Min(99, qty));

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductID == CurrentProductId);

            if (existing != null)
            {
                existing.Quantity = Math.Min(99, existing.Quantity + qty);
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = CurrentProductId,
                    ProductName = lblName.Text,
                    Subtitle = lblSubtitle.Text,
                    ImageUrl = imgProduct.ImageUrl,
                    PriceNow = decimal.TryParse(lblPriceNow.Text, out var pNow) ? pNow : 0,
                    PriceOld = decimal.TryParse(lblPriceOld.Text, out var pOld) ? pOld : 0,
                    CO2SavedPerMeal = double.TryParse(lblCO2.Text, out var co2) ? co2 : 0,
                    Quantity = qty
                });
            }

            Session[CART_KEY] = cart;
            Response.Redirect("Cart.aspx");
        }


        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            int qty;
            if (!int.TryParse(txtQty.Text, out qty)) qty = 1;

            // Save info for order/checkout flow
            Session["BuyNowProductId"] = CurrentProductId;
            Session["BuyNowQty"] = qty;

            // Redirect to your next page
            // Change to Checkout.aspx when you create it
            Response.Redirect("Order.aspx");
        }
    }
}
