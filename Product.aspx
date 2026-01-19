<%@ Page Title="EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Product.aspx.cs"
    Inherits="Business_App_Dev.Product" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- GEO -->
    <asp:HiddenField ID="hfLat" runat="server" />
    <asp:HiddenField ID="hfLng" runat="server" />
    <asp:HiddenField ID="hfHasLoc" runat="server" Value="0" />

    <!-- MODE -->
    <asp:HiddenField ID="hfMode" runat="server" Value="AI" />
    <asp:HiddenField ID="hfCategory" runat="server" Value="" />

    <asp:Button ID="btnRefreshByLoc" runat="server" Text="refresh"
        OnClick="btnRefreshByLoc_Click" Style="display:none;" UseSubmitBehavior="true" />

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>
                <asp:Label ID="lblHeroTitle" runat="server"
                    Text="Save meals, save money, save the planet" />
            </h1>

            <p>
                <asp:Label ID="lblHeroSubtitle" runat="server"
                    Text="Discover surplus food from local restaurants at amazing prices" />
            </p>

            <div class="ee-hero-stats">
                <div class="ee-stat">
                    <div class="ee-stat-value">210</div>
                    <div class="ee-stat-label">
                        <asp:Label ID="lblMealsSaved" runat="server" Text="Meals Saved" />
                    </div>
                </div>
                <div class="ee-stat">
                    <div class="ee-stat-value">$455</div>
                    <div class="ee-stat-label">
                        <asp:Label ID="lblMoneySaved" runat="server" Text="Money Saved" />
                    </div>
                </div>
                <div class="ee-stat">
                    <div class="ee-stat-value">525 kg</div>
                    <div class="ee-stat-label">
                        <asp:Label ID="lblCO2Saved" runat="server" Text="CO₂ Saved" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- CONTENT -->
    <div class="ee-container">

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <!-- PILLS -->
        <div class="ee-pills">
            <asp:LinkButton ID="btnAI" runat="server" CssClass="ee-pill" OnClick="btnAI_Click">✨ AI Recommended</asp:LinkButton>
            <asp:LinkButton ID="btnDeals" runat="server" CssClass="ee-pill" OnClick="btnDeals_Click">🔥 Daily Best Deals</asp:LinkButton>
            <asp:LinkButton ID="btnCats" runat="server" CssClass="ee-pill" OnClick="btnCats_Click">🧭 Explore Categories</asp:LinkButton>
        </div>

        <!-- CATEGORY CHIPS (shows only when Explore Categories mode) -->
        <asp:Panel ID="pnlCategories" runat="server" Visible="false" style="margin:10px 0;">
            <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                <ItemTemplate>
                    <asp:LinkButton runat="server"
                        CssClass="ee-pill ee-pill-small"
                        CommandName="Pick"
                        CommandArgument='<%# Container.DataItem.ToString() %>'>
                        <%# Container.DataItem.ToString() %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>

            <asp:LinkButton ID="btnClearCategory" runat="server"
                CssClass="ee-pill ee-pill-small"
                style="margin-left:8px;"
                OnClick="btnClearCategory_Click">Clear</asp:LinkButton>
        </asp:Panel>

        <!-- GRID -->
        <div class="ee-grid">
            <asp:Repeater ID="ProductRepeater" runat="server">
                <ItemTemplate>
                    <a class="ee-card" href='<%# "ProductDetails.aspx?id=" + Eval("ProductID") %>'>
                        <div class="ee-card-img"
                             style='<%# "background-image:url(" + ResolveUrl(Eval("ImageUrl") == null ? "" : Eval("ImageUrl").ToString()) + ");" %>'>
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
                                <span><%# Eval("DistanceKm", "{0:0.0}") %> km</span>
                            </div>

                            <div class="ee-price-row">
                                <div>
                                    <span class="ee-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></span>
                                    <span class="ee-price-old">$<%# Eval("PriceOld", "{0:0.00}") %></span>
                                </div>

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

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        (function () {
            var hasLoc = document.getElementById("<%= hfHasLoc.ClientID %>").value;
            if (hasLoc === "1") return;
            if (!navigator.geolocation) return;

            navigator.geolocation.getCurrentPosition(
                function (pos) {
                    document.getElementById("<%= hfLat.ClientID %>").value = pos.coords.latitude;
                    document.getElementById("<%= hfLng.ClientID %>").value = pos.coords.longitude;
                    document.getElementById("<%= hfHasLoc.ClientID %>").value = "1";
                    document.getElementById("<%= btnRefreshByLoc.ClientID %>").click();
                },
                function () {
                    document.getElementById("<%= hfHasLoc.ClientID %>").value = "0";
                },
                { enableHighAccuracy: true, timeout: 8000, maximumAge: 600000 }
            );
        })();
    </script>
</asp:Content>
