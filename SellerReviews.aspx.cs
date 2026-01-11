using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodSaver
{
    public partial class SellerReviews : System.Web.UI.Page
    {
        // 1) Fake feedback model
        private class FeedbackItem
        {
            public DateTime CreatedAt { get; set; }
            public string CustomerName { get; set; }
            public int Rating { get; set; }          // 1-5
            public string OrderRef { get; set; }     // e.g. ORD-10021
            public string Comment { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 2) Optional: basic seller login gate (adjust to your session key)
            if (Session["SellerAuthenticated"] as bool? != true)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRatingDropdown();
                BindGrid();
            }
        }

        private void LoadRatingDropdown()
        {
            ddlRating.Items.Clear();
            ddlRating.Items.Add("All Ratings");
            ddlRating.Items.Add("5");
            ddlRating.Items.Add("4");
            ddlRating.Items.Add("3");
            ddlRating.Items.Add("2");
            ddlRating.Items.Add("1");
        }

        // 3) Fake feedback data source (hardcoded)
        private List<FeedbackItem> GetFakeFeedback()
        {
            return new List<FeedbackItem>
            {
                new FeedbackItem {
                    CreatedAt = DateTime.Today.AddDays(-1),
                    CustomerName = "Alicia Tan",
                    Rating = 5,
                    OrderRef = "ORD-10231",
                    Comment = "Fast pickup and food was still fresh. Great deal!"
                },
                new FeedbackItem {
                    CreatedAt = DateTime.Today.AddDays(-2),
                    CustomerName = "Marcus Lim",
                    Rating = 4,
                    OrderRef = "ORD-10210",
                    Comment = "Good value, but collection took a bit longer than expected."
                },
                new FeedbackItem {
                    CreatedAt = DateTime.Today.AddDays(-4),
                    CustomerName = "Nur Aisyah",
                    Rating = 2,
                    OrderRef = "ORD-10188",
                    Comment = "Portion smaller than expected. Would like clearer listing photos."
                },
                new FeedbackItem {
                    CreatedAt = DateTime.Today.AddDays(-6),
                    CustomerName = "Darren Goh",
                    Rating = 3,
                    OrderRef = "ORD-10150",
                    Comment = "Okay overall. Packaging could be improved."
                }
            };
        }

        private void BindGrid()
        {
            var data = GetFakeFeedback();

            // 4) Apply filters (optional but makes it feel “real”)
            // Rating filter
            if (ddlRating.SelectedValue != "All Ratings")
            {
                if (int.TryParse(ddlRating.SelectedValue, out int rating))
                    data = data.Where(x => x.Rating == rating).ToList();
            }

            // Search filter
            var q = (txtSearch.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(q))
            {
                data = data.Where(x =>
                    (x.CustomerName ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (x.Comment ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (x.OrderRef ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }

            // Summary
            lblSummary.Text = $"{data.Count} shown";

            gvFeedback.DataSource = data
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            gvFeedback.DataBind();
        }

        protected void FiltersChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlRating.SelectedIndex = 0;
            txtSearch.Text = "";
            BindGrid();
        }
    }
}