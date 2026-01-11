<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="Business_App_Dev.Cart" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Shopping Cart | EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/Cart.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="ck-body">
<form id="form1" runat="server">

    <div class="ck-container">
        <a class="ck-back" href="Product.aspx">← Continue Shopping</a>

        <!-- HEADER -->
        <div class="ck-header">
            <div class="ck-h-title">
                <span class="ck-cart-ic" aria-hidden="true">🛒</span>
                <span>Shopping Cart</span>
                <span class="ck-items-count">
                    (<asp:Label ID="lblItemCount" runat="server" Text="0" /> items)
                </span>
            </div>
        </div>

        <div class="ck-grid">

            <!-- LEFT -->
            <div class="ck-left">
                <asp:Label ID="lblEmpty" runat="server" CssClass="ck-empty" Visible="false"
                           Text="Your cart is empty." />

                <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                    <ItemTemplate>
                        <div class="ck-item-card">

                            <div class="ck-item-img"
                                 style='background-image:url("<%# ResolveUrl(Eval("ImageUrl").ToString()) %>");'>
                            </div>

                            <div class="ck-item-mid">
                                <div class="ck-item-name"><%# Eval("ProductName") %></div>

                                <!-- SAFE: won't crash even if your model doesn't have Description -->
                                <div class="ck-item-desc"><%# GetDesc(Container.DataItem) %></div>

                                <div class="ck-item-price">
                                    <span class="ck-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></span>
                                </div>
                            </div>

                            <div class="ck-item-right">
                                <!-- qty pill -->
                                <div class="ck-qty-pill">
                                    <asp:LinkButton runat="server"
                                        CommandName="DEC"
                                        CommandArgument='<%# Eval("ProductID") %>'
                                        CssClass="ck-qty-icon">−</asp:LinkButton>

                                    <div class="ck-qty-val"><%# Eval("Quantity") %></div>

                                    <asp:LinkButton runat="server"
                                        CommandName="INC"
                                        CommandArgument='<%# Eval("ProductID") %>'
                                        CssClass="ck-qty-icon">+</asp:LinkButton>
                                </div>

                                <div class="ck-line-total">$<%# Eval("LineTotal", "{0:0.00}") %></div>

                                <!-- remove -->
                                <asp:LinkButton runat="server"
                                    CommandName="REMOVE"
                                    CommandArgument='<%# Eval("ProductID") %>'
                                    CssClass="ck-remove-ic"
                                    ToolTip="Remove">
                                    🗑
                                </asp:LinkButton>
                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- RIGHT -->
            <div class="ck-right">
                <div class="ck-summary-card">
                    <div class="ck-summary-title">Order Summary</div>

                    <div class="ck-sum-row">
                        <span class="ck-muted">Subtotal</span>
                        <span class="ck-strong">$<asp:Label ID="lblSubtotal" runat="server" Text="0.00" /></span>
                    </div>

                    <!-- NO DELIVERY -->
                    <div class="ck-sum-row">
                        <span class="ck-muted">Delivery</span>
                        <span class="ck-pickup">Self Pickup</span>
                    </div>

                    <div class="ck-divider"></div>

                    <div class="ck-total-row">
                        <span class="ck-total-label">Total</span>
                        <span class="ck-total-val">$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></span>
                    </div>

                    <div class="ck-impact-box">
                        <div class="ck-impact-icon" aria-hidden="true">🌿</div>
                        <div class="ck-impact-text">
                            <div class="ck-impact-top">Total Impact</div>
                            <div class="ck-impact-big">
                                <asp:Label ID="lblCO2" runat="server" Text="0.0" /> kg CO₂ saved
                            </div>
                        </div>
                    </div>

                    <!-- Discount UI (optional; can remove if you don't need) -->
                    <div class="ck-discount">
                        <asp:TextBox ID="txtDiscount" runat="server" CssClass="ck-discount-input" placeholder="Enter discount code" />
                        <asp:Button ID="btnApplyDiscount" runat="server" Text="Apply Code" CssClass="ck-apply-btn" />
                    </div>

                    <asp:Label ID="lblPayMsg" runat="server" CssClass="ck-msg" />

                    <div class="ck-cta-wrap">
                        <asp:Button ID="btnPay" runat="server"
                                    Text="Payment"
                                    CssClass="ck-cta-btn"
                                    OnClick="btnPay_Click" />
                    </div>

                </div>
            </div>

        </div>
    </div>

</form>
</body>
</html>
