<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs"
    Inherits="Business_App_Dev.ProductDetails" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Product Details | EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Only your main CSS -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/ProductDetails.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="pd-body">
<form id="form1" runat="server">

    <div class="pd-container">
        <a class="pd-back" href="Product.aspx">← Back to Home</a>

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
                    <span class="pd-reviews">(<asp:Label ID="lblReviews" runat="server" /> reviews)</span>
                </div>

                <div class="pd-info-row">
                    <span class="pd-info">📍 <asp:Label ID="lblDistance" runat="server" /> km away</span>
                    <span class="pd-info danger">⏰ Expires in <asp:Label ID="lblExpiry" runat="server" />h</span>
                </div>

                <div class="pd-impact">
                    <div class="pd-impact-icon">🌿</div>
                    <div>
                        <div class="pd-impact-title">Sustainability Impact</div>
                        <div class="pd-impact-text">
                            1 meal = <asp:Label ID="lblCO2" runat="server" /> kg CO₂ saved
                        </div>
                    </div>
                </div>

                <div class="pd-section">
                    <div class="pd-section-title">Description</div>
                    <div class="pd-desc">
                        <asp:Label ID="lblDescription" runat="server" />
                    </div>
                </div>

                <div class="pd-divider"></div>

                <div class="pd-price-row">
                    <div class="pd-price">
                        <span class="pd-price-now">$<asp:Label ID="lblPriceNow" runat="server" /></span>
                        <span class="pd-price-old">$<asp:Label ID="lblPriceOld" runat="server" /></span>
                        <span class="pd-save">Save $<asp:Label ID="lblSave" runat="server" /></span>
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
                    <div class="pd-why-title">Why Choose This?</div>
                    <ul class="pd-why-list">
                        <li>✓ Fresh and high quality</li>
                        <li>✓ Save money on delicious food</li>
                        <li>✓ Help reduce food waste</li>
                        <li>✓ Support local businesses</li>
                    </ul>
                </div>

            </div>
        </div>
    </div>

    <!-- Cute Toast -->
    <div id="toast" class="toast">
        <span class="toast-icon">🛒</span>
        <div class="toast-text">
            <div class="toast-title">Added to cart</div>
            <div class="toast-sub">You can checkout anytime</div>
        </div>
    </div>

</form>

<script>
    function showToast() {
        const t = document.getElementById("toast");
        t.classList.add("show");
        clearTimeout(window.toastTimer);
        window.toastTimer = setTimeout(() => t.classList.remove("show"), 1800);
        t.onclick = () => t.classList.remove("show");
    }
</script>

</body>
</html>
