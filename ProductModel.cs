using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Business_App_Dev
{
    public class ProductModel
    {
            public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public decimal PriceNow { get; set; }
        public decimal PriceOld { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int DistanceKm { get; set; }
        public int ExpiryHours { get; set; }
        public double CO2Saved { get; set; }
        public int DiscountPercent { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}