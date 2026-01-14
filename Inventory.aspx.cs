using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class Inventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            // Only ProductModel (no Product.cs)
            List<ProductModel> productList = ProductModel.GetAllProducts();
            gvProducts.DataSource = productList;
            gvProducts.DataBind();
        }

        protected void gvProducts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProducts.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvProducts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProducts.EditIndex = -1;
            BindGrid();
        }

        protected void gvProducts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //try
            //{
                int productId = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);

                GridViewRow row = gvProducts.Rows[e.RowIndex];

                // Find controls (EditItemTemplate IDs)
                var txtName = row.FindControl("txtName") as TextBox;
                var txtImageUrl = row.FindControl("txtImageUrl") as TextBox;
                var txtPriceNow = row.FindControl("txtPriceNow") as TextBox;
                var txtPriceOld = row.FindControl("txtPriceOld") as TextBox;
                var txtDiscount = row.FindControl("txtDiscount") as TextBox;
                var txtExpiry = row.FindControl("txtExpiry") as TextBox;
                var txtQuantity = row.FindControl("txtQuantity") as TextBox;
                var txtCategory = row.FindControl("txtCategory") as TextBox;

                string name = (txtName?.Text ?? "").Trim();
                string imageUrl = (txtImageUrl?.Text ?? "").Trim();
                string category = (txtCategory?.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Alert("Product Name cannot be empty");
                    return;
                }

                // Parse decimals safely (support "12.30" etc)
                if (!TryParseDecimal(txtPriceNow?.Text, out decimal priceNow) || priceNow < 0)
                {
                    Alert("Invalid Price");
                    return;
                }

                // Old price optional
                TryParseDecimal(txtPriceOld?.Text, out decimal priceOld);

                // ints
                if (!int.TryParse((txtDiscount?.Text ?? "").Trim(), out int discountPercent)) discountPercent = 0;
                if (discountPercent < 0) discountPercent = 0;

                if (!int.TryParse((txtExpiry?.Text ?? "").Trim(), out int expiryHours) || expiryHours < 0)
                {
                    Alert("Invalid Expiry Hours");
                    return;
                }

                if (!int.TryParse((txtQuantity?.Text ?? "").Trim(), out int quantity) || quantity < 0)
                {
                    Alert("Invalid Quantity");
                    return;
                }

                // Build updated ProductModel
                var updated = new ProductModel
                {
                    ProductID = productId,
                    ProductName = name,
                    ImageUrl = imageUrl,
                    PriceNow = priceNow,
                    PriceOld = priceOld,
                    DiscountPercent = discountPercent,
                    ExpiryHours = expiryHours,
                    Quantity = quantity,
                    Category = category
                };

                int result = ProductModel.UpdateProduct(updated);

                if (result > 0)
                {
                    gvProducts.EditIndex = -1;
                    BindGrid();
                    Alert("Product updated successfully");
                }
                else
                {
                    Alert("Product update failed");
                }
            //}
            //catch (Exception ex)
            //{
            //    Alert("Error: " + ex.Message);
            //}
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int productId = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);

                int result = ProductModel.DeleteProduct(productId);

                if (result > 0)
                {
                    BindGrid();
                    Alert("Product deleted successfully");
                }
                else
                {
                    Alert("Product deletion failed");
                }
            }
            catch (Exception ex)
            {
                Alert("Error: " + ex.Message);
            }
        }

        protected void btn_addProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddNewProduct.aspx");
        }

        // ===== Helpers =====

        private void Alert(string message)
        {
            ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(),
                $"alert('{EscapeJs(message)}');", true);
        }

        private string EscapeJs(string s)
        {
            return (s ?? "").Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "").Replace("\n", "");
        }

        private bool TryParseDecimal(string input, out decimal value)
        {
            input = (input ?? "").Trim();

            // Try current culture + invariant culture to reduce "comma/dot" issues
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value)
                   || decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}