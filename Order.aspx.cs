using System;
using System.Collections.Generic;

namespace Business_App_Dev
{
    public partial class Order : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string idStr = Request.QueryString["id"];
            if (!int.TryParse(idStr, out int id))
            {
                Response.Redirect("Default.aspx");
                return;
            }

            var products = new Dictionary<int, (string Name, string Desc, string Img, string Price, string OldPrice)>
            {
                { 1, ("Seasonal Fruits Mix",
                      "Fresh seasonal fruits perfect for smoothies",
                      "https://images.pexels.com/photos/1437267/pexels-photo-1437267.jpeg",
                      "$3.50", "$8.99") },

                { 2, ("Sushi Platter",
                      "Assorted fresh sushi rolls",
                      "https://images.pexels.com/photos/2098085/pexels-photo-2098085.jpeg",
                      "$8.99", "$22.90") },

                { 3, ("Mr Wang's Pho Bowl",
                      "Authentic Vietnamese pho with fresh herbs",
                      "https://www.momswhothink.com/wp-content/uploads/2023/11/shutterstock-1079365169-huge-licensed-scaled.jpg",
                      "$4.99", "$12.99") }
            };

            if (!products.TryGetValue(id, out var p))
            {
                Response.Redirect("Default.aspx");
                return;
            }

            lblName.Text = p.Name;
            lblDesc.Text = p.Desc;
            imgProduct.ImageUrl = p.Img;   // <-- important (asp:Image)
            lblPrice.Text = p.Price;
            lblOldPrice.Text = p.OldPrice;
        }
    }
}
