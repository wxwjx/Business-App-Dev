using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                
                if (string.IsNullOrWhiteSpace(tb_ProductID.Text) ||
                    string.IsNullOrWhiteSpace(tb_ProductName.Text) ||
                    string.IsNullOrWhiteSpace(tb_Price.Text) ||
                    string.IsNullOrWhiteSpace(tb_quantity.Text) ||
                    string.IsNullOrWhiteSpace(tb_category.Text))
                {
                    // Show a simple alert
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please fill in all fields.');", true);
                    return;
                }
                decimal price;
                int quantity;

                if (!decimal.TryParse(tb_Price.Text, out price))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Price must be a valid number.');", true);
                    return;
                }

                if (!int.TryParse(tb_quantity.Text, out quantity))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Quantity must be a valid integer.');", true);
                    return;
                }
                Product.Products.Add(new Product
                {

                    ID = tb_ProductID.Text,
                   Name = tb_ProductName.Text,
                   Price = Convert.ToDecimal(tb_Price.Text),
                   Quantity = Convert.ToInt32(tb_quantity.Text),
                   Category = tb_category.Text,

                });



            Response.Redirect("Inventory.aspx");
        }
            catch (Exception ex)
            {
                // Log or display error
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
            }
                  }

        //protected void btn_ProductView_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("Inventory.aspx");
        //}
    }
}