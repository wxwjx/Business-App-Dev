<%@ Page Title="Product Details | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs"
    Inherits="Business_App_Dev.ProductDetails" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/ProductDetails.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="pd-container">
        <a class="pd-back" href="Product.aspx">
            ← <asp:Label ID="lblBackHome" runat="server" Text="Back to Home" />
        </a>

        <div class="pd-grid">

            <!-- LEFT -->
            <div class="pd-image-wrap">
                <asp:Image ID="imgProduct" runat="server" CssClass="pd-image" AlternateText="Product" />
                <span class="pd-badge">
                    <asp:Label ID="lblDiscount" runat="server" />% OFF
                </span>
            </div>

            <!-- RIGHT -->
            <div class="pd-right">

                <h1 class="pd-title">
                    <asp:Label ID="lblName" runat="server" />
                </h1>

                <div class="pd-subtitle">
                    <asp:Label ID="lblSubtitle" runat="server" />
                </div>

                <div class="pd-rating-row">
                    <span class="pd-stars">★★★★★</span>
                    <span class="pd-rating"><asp:Label ID="lblRating" runat="server" /></span>
                    <span class="pd-reviews">
                        (<asp:Label ID="lblReviews" runat="server" />
                        <asp:Label ID="lblReviewsText" runat="server" Text="reviews" />)
                    </span>
                </div>

                <div class="pd-info-row">
                    <span class="pd-info">
                        📍 <asp:Label ID="lblDistance" runat="server" />
                        <asp:Label ID="lblKmAway" runat="server" Text="km away" />
                    </span>
                    <span class="pd-info danger">
                        ⏰ <asp:Label ID="lblExpiresIn" runat="server" Text="Expires in" />
                        <asp:Label ID="lblExpiry" runat="server" />h
                    </span>
                </div>

                <div class="pd-impact">
                    <div class="pd-impact-icon">🌿</div>
                    <div>
                        <div class="pd-impact-title">
                            <asp:Label ID="lblImpactTitle" runat="server" Text="Sustainability Impact" />
                        </div>
                        <div class="pd-impact-text">
                            <asp:Label ID="lblImpactMeal" runat="server" Text="1 meal =" />
                            <asp:Label ID="lblCO2" runat="server" />
                            <asp:Label ID="lblImpactCO2" runat="server" Text="kg CO₂ saved" />
                        </div>
                    </div>
                </div>

                <div class="pd-section">
                    <div class="pd-section-title">
                        <asp:Label ID="lblDescTitle" runat="server" Text="Description" />
                    </div>
                    <div class="pd-desc">
                        <asp:Label ID="lblDescription" runat="server" />
                    </div>
                </div>

                <div class="pd-divider"></div>

                <div class="pd-price-row">
                    <div class="pd-price">
                        <span class="pd-price-now">$<asp:Label ID="lblPriceNow" runat="server" /></span>
                        <span class="pd-price-old">$<asp:Label ID="lblPriceOld" runat="server" /></span>
                        <span class="pd-save">
                            <asp:Label ID="lblSaveText" runat="server" Text="Save" /> $
                            <asp:Label ID="lblSave" runat="server" />
                        </span>
                    </div>
                </div>

                <div class="pd-actions">
                    <div class="pd-qty">
                        <asp:Button ID="btnMinus" runat="server" Text="-" CssClass="pd-qty-btn" OnClick="btnMinus_Click" />
                        <asp:TextBox ID="txtQty" runat="server" CssClass="pd-qty-input" Text="1" />
                        <asp:Button ID="btnPlus" runat="server" Text="+" CssClass="pd-qty-btn" OnClick="btnPlus_Click" />
                    </div>

                    <asp:Button ID="btnAddToCart"
                        runat="server"
                        Text="Add to Cart"
                        CssClass="pd-add"
                        OnClick="btnAddToCart_Click"
                        OnClientClick="showToast(); return true;" />

                    <asp:Button ID="btnBuyNow"
                        runat="server"
                        Text="Buy Now"
                        CssClass="pd-buy"
                        OnClick="btnBuyNow_Click" />
                </div>

                <div class="pd-why">
                    <div class="pd-why-title">
                        <asp:Label ID="lblWhyTitle" runat="server" Text="Why Choose This?" />
                    </div>
                    <ul class="pd-why-list">
                        <li>✓ <asp:Label ID="lblWhy1" runat="server" Text="Fresh and high quality" /></li>
                        <li>✓ <asp:Label ID="lblWhy2" runat="server" Text="Save money on delicious food" /></li>
                        <li>✓ <asp:Label ID="lblWhy3" runat="server" Text="Help reduce food waste" /></li>
                        <li>✓ <asp:Label ID="lblWhy4" runat="server" Text="Support local businesses" /></li>
                    </ul>
                </div>

            </div>
        </div>
    </div>

    <!-- Toast -->
    <div id="toast" class="toast">
        <span class="toast-icon">🛒</span>
        <div class="toast-text">
            <div class="toast-title">
                <asp:Label ID="lblToastTitle" runat="server" Text="Added to cart" />
            </div>
            <div class="toast-sub">
                <asp:Label ID="lblToastSub" runat="server" Text="You can checkout anytime" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showToast() {
            const t = document.getElementById("toast");
            t.classList.add("show");
            clearTimeout(window.toastTimer);
            window.toastTimer = setTimeout(() => t.classList.remove("show"), 1800);
            t.onclick = () => t.classList.remove("show");
        }
    </script>
</asp:Content>
