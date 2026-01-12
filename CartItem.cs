using System;

namespace Business_App_Dev
{
    [Serializable]
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal PriceNow { get; set; }
        public decimal PriceOld { get; set; }
        public double CO2SavedPerMeal { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal => PriceNow * Quantity;
        public double LineCO2 => CO2SavedPerMeal * Quantity;
    }
}