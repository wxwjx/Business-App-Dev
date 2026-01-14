using System;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class AddNewProduct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btn_Insert_Click(object sender, EventArgs e)
        {
            try
            {
                // Let ASP.NET validators run first
                if (!Page.IsValid) return;

                // ✅ TEMP: fake seller until login is ready
                // (Make sure SellerID 1 exists in dbo.Seller — your screenshot shows it does)
                int fakeSellerId = 1;

                string name = (tb_ProductName.Text ?? "").Trim();
                string subtitle = (tb_Subtitle.Text ?? "").Trim();
                //string imageUrl = (tb_ImageUrl.Text ?? "").Trim();
                string category = (tb_category.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Alert("Product name is required.");
                    return;
                }

                if (!decimal.TryParse((tb_Price.Text ?? "").Trim(), out decimal priceNow))
                {
                    Alert("Price must be a valid number.");
                    return;
                }

                // optional fields
                decimal.TryParse((tb_OldPrice.Text ?? "").Trim(), out decimal priceOld);
                int.TryParse((tb_DiscountPercent.Text ?? "").Trim(), out int discountPercent);

                if (!int.TryParse((tb_Expiry.Text ?? "").Trim(), out int expiryHours))
                {
                    Alert("Expiry hours must be a valid integer.");
                    return;
                }

                if (!int.TryParse((tb_quantity.Text ?? "").Trim(), out int quantity))
                {
                    Alert("Quantity must be a valid integer.");
                    return;
                }

                if (expiryHours < 0 || quantity < 0 || discountPercent < 0 || priceNow < 0 || priceOld < 0)
                {
                    Alert("Values cannot be negative.");
                    return;
                }

                // ✅ Create ProductModel (no Product.cs)
                var p = new ProductModel
                {
                    SellerID = fakeSellerId,   // ✅ IMPORTANT: prevents NULL SellerID insert error

                    ProductName = name,
                    Subtitle = subtitle,
                    //ImageUrl = imageUrl,

                    PriceNow = priceNow,
                    PriceOld = priceOld,

                    DiscountPercent = discountPercent,
                    ExpiryHours = expiryHours,
                    Quantity = quantity,
                    Category = category,

                    // Defaults for fields not collected on this page
                    Rating = 0,
                    Reviews = 0,
                    DistanceKm = 0,
                    CO2Saved = 0,
                    CreatedAt = DateTime.Now
                };

                int rows = ProductModel.AddProduct(p);

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
