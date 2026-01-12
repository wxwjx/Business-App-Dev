using System;
using System.Collections.Generic;

namespace Business_App_Dev
{
    public partial class Inventory : System.Web.UI.Page
    {

  
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                SeedData();
                BindGrid();
            }
        }

        void SeedData()
        {
            if (Product.Products.Count == 0)
            {
                Product.Products.Add(new Product { ID = "P001", Name = "Chicken Rice", Price = 3.50m, Quantity = 10, Category = "Main" });
                Product.Products.Add(new Product { ID = "P002", Name = "Veg Bento", Price = 4.00m, Quantity = 8, Category = "Vegetarian" });
                Product.Products.Add(new Product { ID = "P003", Name = "Bread Loaf", Price = 1.50m, Quantity = 15, Category = "Bakery" });
            }
        }

        void BindGrid()
        {
            gvProducts.DataSource = Product.Products;
            gvProducts.DataBind();
        }

        protected void gvProducts_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvProducts.EditIndex = e.NewEditIndex;
            BindGrid();
        }


        protected void gvProducts_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            string id = gvProducts.DataKeys[e.RowIndex].Value.ToString();
            Product.Products.RemoveAll(x => x.ID == id);
            BindGrid();
        }

        

        protected void gvProducts_RowCancelingEdit1(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvProducts.EditIndex = -1;
            BindGrid();
        }

        protected void gvProducts_RowUpdating1(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            try
            {
                // get the product ID from DataKeys
                string id = gvProducts.DataKeys[e.RowIndex].Value.ToString();

                // find the product in list
                Product p = Product.Products.Find(x => x.ID == id);
                if (p == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Product not found.');", true);
                    return;
                }

                string newName = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                string priceText = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
                string quantityText = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[3].Controls[0]).Text;
                string newCategory = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[4].Controls[0]).Text;

                // validation
                if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(priceText) ||
                    string.IsNullOrWhiteSpace(quantityText) || string.IsNullOrWhiteSpace(newCategory))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please fill in all fields.');", true);
                    return;
                }

                // parse price and quantity safely
                if (!decimal.TryParse(priceText, out decimal newPrice))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Price must be a valid number.');", true);
                    return;
                }

                if (!int.TryParse(quantityText, out int newQuantity))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Quantity must be a valid integer.');", true);
                    return;
                }
                

                // grab new values from TextBoxes in the row
                p.Name = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
                p.Price = Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[2].Controls[0]).Text);
                p.Quantity = Convert.ToInt32(((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[3].Controls[0]).Text);
                p.Category = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[4].Controls[0]).Text;

                gvProducts.EditIndex = -1;
                BindGrid();

            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
            }

        }

        protected void btn_addProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddNewProduct.aspx");
        }
    }



    
}
