using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodSaver
{
    public partial class SellerWasteTracker : Page
    {
        private class WasteRecord
        {
            public DateTime Date { get; set; }
            public int ItemsRescued { get; set; }
            public double KgSaved { get; set; }
            public double Co2eAvoided { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Match your current login system
            if (Session["SellerAuthenticated"] as bool? != true)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindTracker();
            }
        }

        private void BindTracker()
        {
            // Fake assumptions for demo
            const double co2ePerKgFood = 2.5;  // estimate multiplier
            const int totalItemsListedLast14Days = 180; // fake denominator for “rescue rate”

            var data = GenerateFakeWasteData(days: 14, co2ePerKgFood: co2ePerKgFood);

            // KPIs
            double totalKg = data.Sum(x => x.KgSaved);
            int totalItems = data.Sum(x => x.ItemsRescued);
            double totalCo2 = data.Sum(x => x.Co2eAvoided);

            // Rescue rate (fake but consistent)
            double rescueRate = totalItemsListedLast14Days == 0 ? 0 :
                (double)totalItems / totalItemsListedLast14Days * 100.0;

            lblFoodSaved.Text = totalKg.ToString("0.00");
            lblMealsRescued.Text = totalItems.ToString();
            lblCo2Avoided.Text = totalCo2.ToString("0.00");
            lblRescueRate.Text = rescueRate.ToString("0.0") + "%";

            lblUpdated.Text = "Updated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            lblPeriod.Text = "Tracking period: last 14 days (prototype data)";

            gvWaste.DataSource = data.OrderByDescending(x => x.Date).ToList();
            gvWaste.DataBind();
        }

        private List<WasteRecord> GenerateFakeWasteData(int days, double co2ePerKgFood)
        {
            // Use a seed so it looks stable across refreshes (same fake numbers each run)
            var rng = new Random(12345);

            var list = new List<WasteRecord>();
            for (int i = 0; i < days; i++)
            {
                var date = DateTime.Today.AddDays(-i);

                // Fake “items rescued” and kg saved
                int items = rng.Next(6, 18);                 // 6 to 17 items/day
                double kg = Math.Round(items * rng.NextDouble() * 0.35 + 1.2, 2); // roughly 1.2–7kg/day

                list.Add(new WasteRecord
                {
                    Date = date,
                    ItemsRescued = items,
                    KgSaved = kg,
                    Co2eAvoided = Math.Round(kg * co2ePerKgFood, 2)
                });
            }

            return list;
        }
    }
}