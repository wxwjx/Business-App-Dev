<%@ Page Title="EcoEats | Save Food, Save Money"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Landing.aspx.cs"
    Inherits="Business_App_Dev.Landing" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/Landing.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section class="lp-hero">
        <div class="lp-hero-overlay"></div>

        <div class="lp-hero-inner">

            <div class="lp-left">
                <div class="lp-pill">🌱 EcoEats • Save food, save money</div>

                <h1 class="lp-title">
                    Save meals.<br />
                    Save money.<br />
                    Save the planet.
                </h1>

                <p class="lp-subtitle">
                    Rescue surplus food from nearby restaurants at a fraction of the price —
                    and reduce food waste with every order.
                </p>

                <div class="lp-cta">
                    <a class="lp-btn lp-btn-primary" href="Product.aspx">🛍 Explore Deals</a>
                    <a class="lp-btn lp-btn-ghost" href="About.aspx">✨ How it works</a>
                    <a class="lp-btn lp-btn-ghost" href="SellerSignup.aspx">🏬 Become a seller</a>
                </div>
            </div>

            <div class="lp-right">
                <div class="lp-impact">
                    <div class="lp-impact-title">Today’s impact (demo)</div>
                    <div class="lp-impact-desc">
                        EcoEats turns surplus meals into savings — and real sustainability wins.
                    </div>

                    <div class="lp-stats">
                        <div class="lp-stat">
                            <div class="lp-stat-value">2,100+</div>
                            <div class="lp-stat-label">Meals saved</div>
                        </div>
                        <div class="lp-stat">
                            <div class="lp-stat-value">$4,550+</div>
                            <div class="lp-stat-label">Money saved</div>
                        </div>
                        <div class="lp-stat">
                            <div class="lp-stat-value">525 kg</div>
                            <div class="lp-stat-label">CO₂ avoided</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="lp-features">
                <div class="lp-feature">
                    <div class="lp-feature-title">⚡ AI-Recommended Deals</div>
                    <div class="lp-feature-desc">
                        Find the best value based on popularity, discount, and distance — instantly.
                    </div>
                </div>

                <div class="lp-feature">
                    <div class="lp-feature-title">📍 Easy Pickup</div>
                    <div class="lp-feature-desc">
                        Clear pickup windows and directions so you can grab and go.
                    </div>
                </div>
            </div>

        </div>
    </section>

</asp:Content>
