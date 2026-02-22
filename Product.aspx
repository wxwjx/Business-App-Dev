<%@ Page Title="EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeFile="Product.aspx.cs"
    Inherits="Business_App_Dev.Product" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/Product.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <asp:HiddenField ID="hfLat" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfLng" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfHasLoc" runat="server" ClientIDMode="Static" Value="0" />

    <asp:HiddenField ID="hfMode" runat="server" Value="AI" />
    <asp:HiddenField ID="hfCategory" runat="server" Value="" />

    <asp:Button ID="btnRefreshByLoc" runat="server"
        Text="refresh"
        OnClick="btnRefreshByLoc_Click"
        Style="display:none;"
        UseSubmitBehavior="true" />

    <section class="ee-hero">
        <div class="ee-container">
            <h1>Save meals, save money, save the planet</h1>
            <p>Discover surplus food from local restaurants at amazing prices</p>
        </div>
    </section>

    <div class="ee-container">

        <asp:UpdatePanel ID="upProducts" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
                    <asp:Label ID="lblError" runat="server" />
                </asp:Panel>

                <div class="ee-searchbar" style="margin-top:16px;">
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="ee-search-input"
                        placeholder="Search meals or stores..."
                        AutoPostBack="true"
                        OnTextChanged="txtSearch_TextChanged" />

                    <asp:LinkButton ID="btnClearSearch" runat="server"
                        CssClass="ee-search-clear"
                        OnClick="btnClearSearch_Click"
                        CausesValidation="false">Clear</asp:LinkButton>
                </div>

                <div class="ee-pills" style="margin-top:14px;">
                    <asp:LinkButton ID="btnAI" runat="server" CssClass="ee-pill" OnClick="btnAI_Click">✨ Recommended</asp:LinkButton>
                    <asp:LinkButton ID="btnDeals" runat="server" CssClass="ee-pill" OnClick="btnDeals_Click">🔥 Daily Best Deals</asp:LinkButton>
                    <asp:LinkButton ID="btnCats" runat="server" CssClass="ee-pill" OnClick="btnCats_Click">🧭 Explore Categories</asp:LinkButton>
                </div>

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

                <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
                    <div class="ee-search-meta" style="margin:10px 0;">
                        <asp:Literal ID="litSearchMeta" runat="server" />
                    </div>

                    <div class="ee-grid">
                        <asp:Repeater ID="rptSearch" runat="server">
                            <ItemTemplate>
                                <a class="ee-card" href='<%# "ProductDetails.aspx?id=" + Eval("ProductID") %>'>
                                    <div class="ee-card-img"
                                         style='<%# "background-image:url(" + ResolveUrl(Eval("ImageUrl") == null ? "" : Eval("ImageUrl").ToString()) + ");" %>'>
                                        <span class="ee-discount"><%# Eval("DiscountPercent") %>% OFF</span>
                                    </div>

                                    <div class="ee-card-body">
                                        <div class="ee-title"><%# Eval("ProductName") %></div>
                                        <div class="ee-subtitle"><%# Eval("Subtitle") %></div>

                                        <div class="ee-meta">
                                            ⭐ <%# Eval("Rating","{0:0.0}") %> •
                                            <%# Eval("Reviews") %> reviews •
                                            <%# Eval("DistanceKm","{0:0.0}") %> km
                                        </div>

                                        <div class="ee-price-row">
                                            <span class="ee-price-now">$<%# Eval("PriceNow","{0:0.00}") %></span>
                                            <span class="ee-price-old">$<%# Eval("PriceOld","{0:0.00}") %></span>
                                        </div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlBrowse" runat="server" Visible="true">
                    <div class="ee-grid">
                        <asp:Repeater ID="ProductRepeater" runat="server">
                            <ItemTemplate>
                                <a class="ee-card" href='<%# "ProductDetails.aspx?id=" + Eval("ProductID") %>'>
                                    <div class="ee-card-img"
                                         style='<%# "background-image:url(" + ResolveUrl(Eval("ImageUrl") == null ? "" : Eval("ImageUrl").ToString()) + ");" %>'>
                                        <span class="ee-discount"><%# Eval("DiscountPercent") %>% OFF</span>
                                    </div>

                                    <div class="ee-card-body">
                                        <div class="ee-title"><%# Eval("ProductName") %></div>
                                        <div class="ee-subtitle"><%# Eval("Subtitle") %></div>

                                        <div class="ee-meta">
                                            ⭐ <%# Eval("Rating","{0:0.0}") %> •
                                            <%# Eval("Reviews") %> reviews •
                                            <%# Eval("DistanceKm","{0:0.0}") %> km
                                        </div>

                                        <div class="ee-price-row">
                                            <span class="ee-price-now">$<%# Eval("PriceNow","{0:0.00}") %></span>
                                            <span class="ee-price-old">$<%# Eval("PriceOld","{0:0.00}") %></span>
                                        </div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

            </ContentTemplate>

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnRefreshByLoc" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAI" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnDeals" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnCats" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnClearCategory" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="txtSearch" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="btnClearSearch" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
<script>
(function () {

    function byId(id) { return document.getElementById(id); }

    function setHidden(lat, lng) {
        var latEl = byId("hfLat");
        var lngEl = byId("hfLng");
        var hasEl = byId("hfHasLoc");

        if (!latEl || !lngEl || !hasEl) return;

        latEl.value = lat;
        lngEl.value = lng;

        var latNum = parseFloat(lat);
        var lngNum = parseFloat(lng);

        var ok = !isNaN(latNum) && !isNaN(lngNum) &&
                 Math.abs(latNum) > 0.0001 &&
                 Math.abs(lngNum) > 0.0001;

        hasEl.value = ok ? "1" : "0";

        if (ok) {
            __doPostBack("<%= btnRefreshByLoc.UniqueID %>", "");
            }
        }

        function requestLocation() {
            if (!navigator.geolocation) return;

            navigator.geolocation.getCurrentPosition(
                function (pos) {
                    var lat = pos.coords.latitude.toFixed(6);
                    var lng = pos.coords.longitude.toFixed(6);
                    setHidden(lat, lng);
                },
                function () {
                    setHidden("", "");
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                }
            );
        }

        function run() {
            requestLocation();
        }

        if (window.Sys && Sys.Application && Sys.Application.add_load) {
            Sys.Application.add_load(run);
        } else {
            window.addEventListener("load", run);
        }

    })();
</script>
</asp:Content>