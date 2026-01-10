<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Business_App_Dev.Product" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats Products</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="Content/EcoEats.css" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="ee-body">
<form id="form1" runat="server">

    <!-- Top Bar -->
    <header class="ee-topbar">
        <div class="ee-topbar-inner">

            <!-- UPDATED: logo beside EcoEats -->
            <div class="ee-brand">
                <div class="ee-logo" aria-hidden="true">
                    <!-- Leaf icon (SVG) -->
                    <svg class="ee-leaf" viewBox="0 0 24 24">
                        <path d="M19 3c-6.5.7-11 3.8-13.8 7.2C2.6 13.4 2.2 17.2 4 21c3.8 1.8 7.6 1.4 10.8-1.2C18.2 17 21.3 12.5 22 6c.1-1.2-.7-2.9-3-3z"></path>
                        <path d="M6 18c4.2-4.3 7.7-6.5 12-8"></path>
                    </svg>
                </div>
                <div class="ee-brand-name">EcoEats</div>
            </div>

            <div class="ee-search">
                <span class="ee-search-ic">⌕</span>
                <input class="ee-search-input" type="text" placeholder="Search for meals, restaurants..." />
            </div>

            <nav class="ee-nav">
                <a class="ee-nav-link active" href="Default.aspx">Home</a>
                <a class="ee-nav-link" href="Order.aspx">Orders</a>
                <a class="ee-nav-link" href="#">Profile</a>
                <a class="ee-nav-link" href="#">About Us</a>
                <a class="ee-nav-link" href="#">Help</a>
                <a class="ee-nav-link" href="#">Feedback</a>
                <a class="ee-nav-link" href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="#" title="Account">👤</a>
            </div>
        </div>
    </header>

    <!-- UPDATED: Banner color to match screenshot (green -> teal) -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1 class="ee-hero-title">Save meals, save money, save the planet</h1>
            <p class="ee-hero-subtitle">Discover surplus food from local restaurants at amazing prices</p>

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

    <!-- Chips / Filters -->
    <section class="ee-container ee-chips-wrap">
        <div class="ee-chips">
            <button type="button" class="ee-chip active">✨ AI Recommended</button>
            <button type="button" class="ee-chip">🔥 Daily Best Deals</button>
            <button type="button" class="ee-chip">🧭 Explore Categories</button>
        </div>
    </section>

    <!-- Products Grid (UNCHANGED) -->
    <main class="ee-container ee-grid-wrap">
        <asp:Repeater ID="ProductRepeater" runat="server">
            <ItemTemplate>

                <a class="ee-card"
                   href='<%# "ProductDetails.aspx?id=" + Eval("ProductID") %>'>

                    <div class="ee-card-img"
                         style='background-image:url("<%# ResolveUrl(Eval("ImageUrl").ToString()) %>");'>

                        <span class="ee-badge"><%# Eval("DiscountPercent") %>% OFF</span>

                        <button type="button" class="ee-like" title="Save">
                            ♥
                        </button>
                    </div>

                    <div class="ee-card-body">
                        <div class="ee-title"><%# Eval("ProductName") %></div>
                        <div class="ee-subtitle"><%# Eval("Subtitle") %></div>

                        <div class="ee-meta">
                            <span class="ee-meta-item">⭐ <%# Eval("Rating", "{0:0.0}") %></span>
                            <span class="ee-meta-dot">•</span>
                            <span class="ee-meta-item"><%# Eval("Reviews") %> reviews</span>
                            <span class="ee-meta-dot">•</span>
                            <span class="ee-meta-item"><%# Eval("DistanceKm") %> km</span>
                        </div>

                        <div class="ee-row">
                            <div class="ee-price">
                                <span class="ee-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></span>
                                <span class="ee-price-old">$<%# Eval("PriceOld", "{0:0.00}") %></span>
                            </div>

                            <span class="ee-pill"><%# Eval("CO2Saved", "{0:0.0}") %> kg CO₂ saved</span>
                        </div>

                        <div class="ee-foot">
                            <span class="ee-expiry">⏰ Expires in <%# Eval("ExpiryHours") %>h</span>
                        </div>
                    </div>
                </a>

            </ItemTemplate>
        </asp:Repeater>
    </main>

</form>
</body>
</html>
