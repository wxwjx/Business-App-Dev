using System;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class AddNewProduct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerAuthenticated"] == null || !(bool)Session["SellerAuthenticated"])
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }
        }

        protected void btn_Insert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                // Get SellerID from session
                int sellerId = Convert.ToInt32(Session["SellerId"]);

                string name = (tb_ProductName.Text ?? "").Trim();
                string subtitle = (tb_Subtitle.Text ?? "").Trim();
                string category = (tb_category.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Alert("Product name is required.");
                    return;
                }

                if (!decimal.TryParse((tb_Price.Text ?? "").Trim(), out decimal priceNow) || priceNow < 0)
                {
                    Alert("Price must be a valid non-negative number.");
                    return;
                }

                decimal.TryParse((tb_OldPrice.Text ?? "").Trim(), out decimal priceOld);
                int.TryParse((tb_DiscountPercent.Text ?? "").Trim(), out int discountPercent);
                int.TryParse((tb_Expiry.Text ?? "").Trim(), out int expiryHours);
                int.TryParse((tb_quantity.Text ?? "").Trim(), out int quantity);

                if (expiryHours < 0 || quantity < 0 || discountPercent < 0 || priceOld < 0)
                {
                    Alert("Values cannot be negative.");
                    return;
                }

                var product = new ProductModel
                {
                    SellerID = sellerId,
                    ProductName = name,
                    Subtitle = subtitle,
                    PriceNow = priceNow,
                    PriceOld = priceOld,
                    DiscountPercent = discountPercent,
                    ExpiryHours = expiryHours,
                    Quantity = quantity,
                    Category = category,
                    Rating = 0,
                    Reviews = 0,
                    DistanceKm = 0,
                    CO2Saved = 0,
                    CreatedAt = DateTime.Now
                };

                int rows = ProductModel.AddProduct(product);
                if (rows > 0)
                {
                    Response.Redirect("Inventory.aspx");
                }
                else
                {
                    Alert("Failed to add product.");
                }
            }
            catch (Exception ex)
            {
                Alert("An error occurred: " + ex.Message);
            }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Inventory.aspx");
        }

        private void Alert(string msg)
        {
            msg = (msg ?? "")
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", "");

            ClientScript.RegisterStartupScript(
                GetType(),
                Guid.NewGuid().ToString(),
                $"alert('{msg}');",
                true
            );
        }
    }
}