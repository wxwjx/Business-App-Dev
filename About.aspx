<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Business_App_Dev.About" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats - About Us</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Reuse main eco design -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <!-- SAME HEADER -->
    <header class="ee-topbar">
        <div class="ee-container ee-topbar-inner">

            <div class="ee-brand">
                <div class="ee-logo">
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
                <a href="Product.aspx">Home</a>
                <a href="Order.aspx">Orders</a>
                <a href="#">Profile</a>
                <a class="active" href="About.aspx">About Us</a>
                <a href="#">Help</a>
                <a href="#">Feedback</a>
                <a href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="#" title="Account">👤</a>
            </div>

        </div>
    </header>

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>About EcoEats</h1>
            <p>We connect hungry customers with surplus food from local restaurants — saving meals, money, and the planet.</p>
        </div>
    </section>

    <!-- ABOUT CONTENT -->
    <section class="ee-container" style="padding:40px 0;">
        <h2 style="font-weight:700;">Our Mission</h2>
        <p>
            EcoEats aims to reduce food waste in Singapore by helping customers rescue perfectly good food 
            that would otherwise be thrown away.
        </p>

        <h2 style="font-weight:700; margin-top:30px;">Our Story</h2>
        <p>
            Built by students passionate about sustainability, EcoEats encourages eco-friendly habits 
            while providing affordable meals.
        </p>

        <h2 style="font-weight:700; margin-top:30px;">Meet the Team</h2>
        <ul>
            <li>💡 Student Developers</li>
            <li>🍽 Food Waste Fighters</li>
            <li>🌿 Eco Advocates</li>
        </ul>
    </section>

</form>
</body>
</html>
