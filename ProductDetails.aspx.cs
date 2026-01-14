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
                return int.TryParse(Request.QueryString["id"], out int id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (CurrentProductId <= 0)
            {
                // Optional: redirect if missing/invalid id
                // Response.Redirect("Product.aspx");
                return;
            }

            try
            {
                LoadProduct(CurrentProductId);
            }
            catch (SqlException)
            {
                // Optional: show an error label/panel if you have one
                // lblError.Text = "Database error loading product details.";
            }
            catch (Exception)
            {
                // lblError.Text = "Unexpected error loading product details.";
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
                    if (!r.Read())
                        return;

                    lblName.Text = r["ProductName"]?.ToString() ?? "";
                    lblSubtitle.Text = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "";
                    lblDescription.Text = r["Subtitle"] != DBNull.Value ? r["Subtitle"].ToString() : "";
                    imgProduct.ImageUrl = r["ImageUrl"] != DBNull.Value ? r["ImageUrl"].ToString() : "";

                    decimal now = r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0m;
                    decimal old = r["PriceOld"] != DBNull.Value ? Convert.ToDecimal(r["PriceOld"]) : now;

                    lblPriceNow.Text = now.ToString("0.00");
                    lblPriceOld.Text = old.ToString("0.00");
                    lblSave.Text = (old - now).ToString("0.00");

                    lblRating.Text = r["Rating"] != DBNull.Value ? r["Rating"].ToString() : "0";
                    lblReviews.Text = r["Reviews"] != DBNull.Value ? r["Reviews"].ToString() : "0";
                    lblDistance.Text = r["DistanceKm"] != DBNull.Value ? r["DistanceKm"].ToString() : "0";
                    lblExpiry.Text = r["ExpiryHours"] != DBNull.Value ? r["ExpiryHours"].ToString() : "0";
                    lblCO2.Text = r["CO2Saved"] != DBNull.Value ? r["CO2Saved"].ToString() : "0";
                    lblDiscount.Text = r["DiscountPercent"] != DBNull.Value ? r["DiscountPercent"].ToString() : "0";

                    // Optional: ensure txtQty has a default value
                    if (string.IsNullOrWhiteSpace(txtQty.Text))
                        txtQty.Text = "1";
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

        private int GetQtySafe()
        {
            // clamp between 1 and 99
            if (!int.TryParse(txtQty.Text, out int qty))
                qty = 1;

            qty = Math.Max(1, qty);
            qty = Math.Min(99, qty);
            return qty;
        }

        private void AddItemToCart()
        {
            int qty = GetQtySafe();
            var cart = GetCart();

            // parse labels safely
            decimal priceNow = 0m;
            decimal priceOld = 0m;
            double co2 = 0;

            decimal.TryParse(lblPriceNow.Text, out priceNow);
            decimal.TryParse(lblPriceOld.Text, out priceOld);
            double.TryParse(lblCO2.Text, out co2);

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
                    ProductName = lblName.Text ?? "",
                    Subtitle = lblSubtitle.Text ?? "",
                    ImageUrl = imgProduct.ImageUrl ?? "",
                    PriceNow = priceNow,
                    PriceOld = priceOld,
                    CO2SavedPerMeal = co2,
                    Quantity = qty
                });
            }

            Session[CART_KEY] = cart;
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            int qty = GetQtySafe();
            txtQty.Text = Math.Max(1, qty - 1).ToString();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            int qty = GetQtySafe();
            txtQty.Text = Math.Min(99, qty + 1).ToString();
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                AddItemToCart();
                // Optional: show toast/label
                // lblMsg.Text = "Added to cart!";
            }
            catch (Exception)
            {
                // lblMsg.Text = "Could not add to cart. Please try again.";
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            try
            {
                AddItemToCart();
                Response.Redirect("Cart.aspx");
            }
            catch (Exception)
            {
                // lblMsg.Text = "Could not proceed to cart. Please try again.";
            }
        }
    }
}
