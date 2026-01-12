<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Business_App_Dev.Product" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- ONLY ONE CSS FILE -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <!-- TOP BAR -->
    <header class="ee-topbar">
        <div class="ee-container ee-topbar-inner">

            <div class="ee-brand">
                <div class="ee-logo">
                    <!-- leaf icon -->
                    <svg viewBox="0 0 24 24" aria-hidden="true">
                        <path d="M19 3c-6.5.7-11 3.8-13.8 7.2C2.6 13.4 2.2 17.2 4 21c3.8 1.8 7.6 1.4 10.8-1.2C18.2 17 21.3 12.5 22 6c.1-1.2-.7-2.9-3-3z"></path>
                    </svg>
                </div>
                <div class="ee-brand-name">EcoEats</div>
            </div>

            <div class="ee-search">
                <input type="text" placeholder="Search for meals, restaurants..." />
            </div>

            <nav class="ee-nav">
                <a class="active" href="Product.aspx">Home</a>
                <a href="Order.aspx">Orders</a>
                <a href="Profile.aspx">Profile</a>
                <a href="About.aspx">About Us</a>
                <a href="#">Help</a>
                <a href="Feedback.aspx">Feedback</a>
                <a href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="Profile.aspx" title="Account">👤</a>
            </div>

        </div>
    </header>

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>Save meals, save money, save the planet</h1>
            <p>Discover surplus food from local restaurants at amazing prices</p>

            <div class="ee-hero-stats">
                <div class="ee-stat">
                    <div class="ee-stat-value">210</div>
                    <div class="ee-stat-label">Meals Saved</div>
                </div>
                <div class="ee-stat">
                    <div class="ee-stat-value">$455</div>
                    <div class="ee-stat-label">Money Saved</div>
                </div>
                <div class="ee-stat">
                    <div class="ee-stat-value">525 kg</div>
                    <div class="ee-stat-label">CO₂ Saved</div>
                </div>
            </div>
        </div>
    </section>

    <!-- FILTER PILLS -->
    <div class="ee-container">
        <div class="ee-pills">
            <button type="button" class="ee-pill active">✨ AI Recommended</button>
            <button type="button" class="ee-pill">🔥 Daily Best Deals</button>
            <button type="button" class="ee-pill">🧭 Explore Categories</button>
        </div>

        <!-- GRID -->
        <div class="ee-grid">
            <asp:Repeater ID="ProductRepeater" runat="server">
                <ItemTemplate>

                    <a class="ee-card" href='<%# "ProductDetails.aspx?id=" + Eval("ProductID") %>'>

                        <div class="ee-card-img"
                             style='background-image:url("<%# ResolveUrl(Eval("ImageUrl").ToString()) %>");'>

                            <span class="ee-discount"><%# Eval("DiscountPercent") %>% OFF</span>
                            <span class="ee-like">♥</span>
                        </div>

                        <div class="ee-card-body">
                            <div class="ee-title"><%# Eval("ProductName") %></div>
                            <div class="ee-subtitle"><%# Eval("Subtitle") %></div>

                            <div class="ee-meta">
                                <span>⭐ <%# Eval("Rating", "{0:0.0}") %></span>
                                <span>•</span>
                                <span><%# Eval("Reviews") %> reviews</span>
                                <span>•</span>
                                <span><%# Eval("DistanceKm") %> km</span>
                            </div>

                            <div class="ee-price-row">
                                <div>
                                    <span class="ee-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></span><span class="ee-price-old">$<%# Eval("PriceOld", "{0:0.00}") %></span></div>

                                <span class="ee-pill-small"><%# Eval("CO2Saved", "{0:0.0}") %> kg CO₂ saved</span>
                            </div>

                            <div class="ee-meta" style="margin-top:10px;">
                                <span class="ee-expiry">⏰ Expires in <%# Eval("ExpiryHours") %>h</span>
                            </div>
                        </div>

                    </a>

                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

</form>
</body>
</html>
