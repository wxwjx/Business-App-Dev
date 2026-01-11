<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="Business_App_Dev.Cart" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Shopping Cart</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="Content/site.css" rel="stylesheet" />
    <link href="Content/EcoEats.css" rel="stylesheet" />
    <link href="Content/Cart.css" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="cart-body">
<form id="form1" runat="server">

    <div class="cart-container">
        <a class="cart-back" href="Product.aspx">← Continue Shopping</a>

        <div class="cart-title-row">
            <div class="cart-title">
                <span class="cart-ic">🛒</span>
                <span>Shopping Cart</span>
            </div>
            <div class="cart-count">
                (<asp:Label ID="lblItemCount" runat="server" Text="0"></asp:Label> items)
            </div>
        </div>

        <div class="cart-grid">

            <!-- LEFT: items -->
            <section class="cart-left">
                <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="cart-empty">
                    Your cart is empty. <a href="Product.aspx">Browse meals</a>
                </asp:Panel>

                <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                    <ItemTemplate>
                        <div class="cart-item">

                            <div class="cart-item-img"
                                 style='background-image:url("<%# ResolveUrl(Eval("ImageUrl").ToString()) %>");'>
                            </div>

                            <div class="cart-item-mid">
                                <div class="cart-item-name"><%# Eval("ProductName") %></div>
                                <div class="cart-item-sub"><%# Eval("Subtitle") %></div>

                                <div class="cart-item-price">
                                    <span class="now">$<%# Eval("PriceNow", "{0:0.00}") %></span>
                                    <span class="old">$<%# Eval("PriceOld", "{0:0.00}") %></span>
                                </div>
                            </div>

                            <div class="cart-item-right">
                                <asp:LinkButton ID="btnRemove" runat="server"
                                    CssClass="cart-remove"
                                    CommandName="remove"
                                    CommandArgument='<%# Eval("ProductID") %>'
                                    ToolTip="Remove">🗑</asp:LinkButton>

                                <div class="cart-qty">
                                    <asp:LinkButton ID="btnMinus" runat="server"
                                        CssClass="cart-qty-btn"
                                        CommandName="minus"
                                        CommandArgument='<%# Eval("ProductID") %>'>-</asp:LinkButton>

                                    <span class="cart-qty-val"><%# Eval("Quantity") %></span>

                                    <asp:LinkButton ID="btnPlus" runat="server"
                                        CssClass="cart-qty-btn"
                                        CommandName="plus"
                                        CommandArgument='<%# Eval("ProductID") %>'>+</asp:LinkButton>
                                </div>

                                <div class="cart-line-total">
                                    $<%# Eval("LineTotal", "{0:0.00}") %>
                                </div>
                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </section>

            <!-- RIGHT: summary -->
            <aside class="cart-right">
                <div class="sum-card">
                    <div class="sum-title">Order Summary</div>

                    <div class="sum-row">
                        <span>Subtotal</span>
                        <span>$<asp:Label ID="lblSubtotal" runat="server" Text="0.00"></asp:Label></span>
                    </div>

                    <div class="sum-row">
                        <span>Delivery Fee</span>
                        <span class="free">FREE</span>
                    </div>

                    <div class="sum-divider"></div>

                    <div class="sum-total">
                        <span>Total</span>
                        <span class="total-val">$<asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label></span>
                    </div>

                    <div class="impact-card">
                        <div class="impact-ic">🌿</div>
                        <div>
                            <div class="impact-title">Total Impact</div>
                            <div class="impact-val">
                                <asp:Label ID="lblCO2" runat="server" Text="0.0"></asp:Label> kg CO₂ saved
                            </div>
                        </div>
                    </div>

                    <div class="sum-discount">
                        <asp:TextBox ID="txtCode" runat="server" CssClass="sum-code" placeholder="Enter discount code"></asp:TextBox>
                        <asp:Button ID="btnApply" runat="server" Text="Apply Code" CssClass="sum-apply" OnClick="btnApply_Click" />
                        <asp:Label ID="lblCodeMsg" runat="server" CssClass="sum-msg" />
                    </div>

                    <asp:Button ID="btnCheckout" runat="server"
                        Text="Proceed to Checkout"
                        CssClass="sum-checkout"
                        OnClick="btnCheckout_Click" />
                </div>
            </aside>

        </div>
    </div>

</form>
</body>
</html>
