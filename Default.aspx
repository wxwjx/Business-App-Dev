<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Business_App_Dev._Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- page-specific <head> stuff (if any) -->
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- HERO SECTION -->
    <section class="ee-hero">
        <div class="container">
            <h1>Save meals, save money, save the planet</h1>
            <p class="ee-hero-subtitle">
                Discover surplus food from local restaurants at amazing prices.
            </p>

            <div class="ee-hero-stats">
                <div class="ee-stat-card">
                    <div class="ee-stat-value">210</div>
                    <div class="ee-stat-label">Meals Saved</div>
                </div>
                <div class="ee-stat-card">
                    <div class="ee-stat-value">$455</div>
                    <div class="ee-stat-label">Money Saved</div>
                </div>
                <div class="ee-stat-card">
                    <div class="ee-stat-value">525 kg</div>
                    <div class="ee-stat-label">CO₂ Saved</div>
                </div>
            </div>
        </div>
    </section>

    <!-- PRODUCT LIST SECTION -->
    <section class="ee-products">
        <div class="container">

            <!-- Filter Pills -->
            <div class="ee-filter-row">
                <button class="ee-pill ee-pill-active">⚙ AI Recommended</button>
                <button class="ee-pill">📈 Daily Best Deals</button>
                <button class="ee-pill">🧭 Explore Categories</button>
            </div>

            <!-- Product Cards Row -->
            <div class="row">

                <!-- CARD 1 -->
                <div class="col-md-4 ee-product-wrapper">
                    <div class="ee-product-card">
                        <div class="ee-product-image"
                             style="background-image:url('https://images.pexels.com/photos/1437267/pexels-photo-1437267.jpeg');">
                            <span class="ee-discount-tag">60% OFF</span>
                            <button class="ee-heart-btn">♡</button>
                        </div>
                        <div class="ee-product-body">
                            <h3>Seasonal Fruits Mix</h3>
                            <p class="ee-product-subtitle">
                                Fresh seasonal fruits perfect for smoothies
                            </p>

                            <div class="ee-meta-row">
                                <span>⭐ 4.9 (456)</span>
                                <span>📍 2 km</span>
                            </div>
                            <div class="ee-meta-row ee-expiry">
                                <span>⏰ Expires in 6h</span>
                            </div>

                            <div class="ee-price-row">
                                <div>
                                    <span class="ee-price-now">$3.50</span>
                                    <span class="ee-price-old">$8.99</span>
                                </div>
                                <span class="ee-badge">1.8 kg CO₂ saved</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- CARD 2 -->
                <div class="col-md-4 ee-product-wrapper">
                    <div class="ee-product-card">
                        <div class="ee-product-image"
                             style="background-image:url('https://images.pexels.com/photos/2098085/pexels-photo-2098085.jpeg');">
                            <span class="ee-discount-tag">61% OFF</span>
                            <button class="ee-heart-btn">♡</button>
                        </div>
                        <div class="ee-product-body">
                            <h3>Sushi Platter</h3>
                            <p class="ee-product-subtitle">
                                Assorted fresh sushi rolls
                            </p>

                            <div class="ee-meta-row">
                                <span>⭐ 4.9 (421)</span>
                                <span>📍 3 km</span>
                            </div>
                            <div class="ee-meta-row ee-expiry">
                                <span>⏰ Expires in 2h</span>
                            </div>

                            <div class="ee-price-row">
                                <div>
                                    <span class="ee-price-now">$8.99</span>
                                    <span class="ee-price-old">$22.90</span>
                                </div>
                                <span class="ee-badge">3.5 kg CO₂ saved</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- CARD 3 -->
                <div class="col-md-4 ee-product-wrapper">
                    <div class="ee-product-card">
                        <div class="ee-product-image"
                             style="background-image:url('https://www.momswhothink.com/wp-content/uploads/2023/11/shutterstock-1079365169-huge-licensed-scaled.jpg');">
                            <span class="ee-discount-tag">60% OFF</span>
                            <button class="ee-heart-btn">♡</button>
                        </div>
                        <div class="ee-product-body">
                            <h3>Mr Wang's Pho Bowl</h3>
                            <p class="ee-product-subtitle">
                                Authentic Vietnamese pho with fresh herbs
                            </p>

                            <div class="ee-meta-row">
                                <span>⭐ 4.8 (234)</span>
                                <span>📍 5 km</span>
                            </div>
                            <div class="ee-meta-row ee-expiry">
                                <span>⏰ Expires in 3h</span>
                            </div>

                            <div class="ee-price-row">
                                <div>
                                    <span class="ee-price-now">$4.99</span>
                                    <span class="ee-price-old">$12.99</span>
                                </div>
                                <span class="ee-badge">2.5 kg CO₂ saved</span>
                            </div>
                        </div>
                    </div>
                </div>

            </div> <!-- /row -->
        </div> <!-- /container -->
    </section>

</asp:Content>
