<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="Business_App_Dev.Cart" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shopping Cart | EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Site + Cart CSS -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/Cart.css") %>" rel="stylesheet" />

    <!-- Font -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="ck-body ck-has-items">
<form id="form1" runat="server">

    <div class="ck-container">

        <a class="ck-back" href="Product.aspx">← Continue Shopping</a>

        <!-- Title -->
        <div class="ck-header-title">
            <div class="ck-h-title">
                <span class="ck-cart-ic">🛒</span>
                Shopping Cart
                <span class="ck-muted ck-title-count">
                    (<asp:Label ID="lblItemCount" runat="server" Text="0" /> items)
                </span>
            </div>
        </div>

        <div class="ck-grid">

            <!-- LEFT -->
            <div class="ck-left">

                <!-- Empty state -->
                <asp:Panel ID="lblEmpty" runat="server" Visible="false" CssClass="ck-empty">
                    Your cart is empty.
                </asp:Panel>

                <!-- Select all row -->
                <asp:Panel ID="pnlSelectAll" runat="server" CssClass="ck-leftbar" Visible="false">
                    <label class="ck-selectall">
                        <asp:CheckBox ID="chkSelectAll"
                            runat="server"
                            AutoPostBack="true"
                            OnCheckedChanged="chkSelectAll_CheckedChanged" />
                        <span>Select all</span>
                    </label>

                    <!-- Optional: if you want, set this text from code-behind -->
                    <asp:Label ID="lblSelectedCount" runat="server" CssClass="ck-selected-count" Text="" />
                </asp:Panel>

                <!-- Items list -->
                <asp:Repeater ID="rptCart" runat="server"
                    OnItemCommand="rptCart_ItemCommand"
                    OnItemDataBound="rptCart_ItemDataBound">

                    <ItemTemplate>
                        <div class="ck-item-card">

                            <!-- selection checkbox -->
                            <div class="ck-item-select">
                                <asp:HiddenField ID="hfPid" runat="server" Value='<%# Eval("ProductID") %>' />
                                <asp:CheckBox ID="chkSelect"
                                    runat="server"
                                    AutoPostBack="true"
                                    OnCheckedChanged="chkSelect_CheckedChanged" />
                            </div>

                            <!-- image -->
                            <div class="ck-item-img"
                                 style='background-image:url("<%# Eval("ImageUrl") %>");'>
                            </div>

                            <!-- middle -->
                            <div class="ck-item-mid">
                                <div class="ck-item-name"><%# Eval("ProductName") %></div>
                                <div class="ck-item-desc"><%# Eval("Subtitle") %></div>

                                <div class="ck-item-price">
                                    <div class="ck-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></div>
                                </div>
                            </div>

                            <!-- right -->
                            <div class="ck-item-right">
                                <div class="ck-qty-pill">
                                    <asp:LinkButton ID="btnDec" runat="server"
                                        CssClass="ck-qty-icon"
                                        CommandName="DEC"
                                        CommandArgument='<%# Eval("ProductID") %>'
                                        CausesValidation="false">−</asp:LinkButton>

                                    <span class="ck-qty-val"><%# Eval("Quantity") %></span>

                                    <asp:LinkButton ID="btnInc" runat="server"
                                        CssClass="ck-qty-icon"
                                        CommandName="INC"
                                        CommandArgument='<%# Eval("ProductID") %>'
                                        CausesValidation="false">+</asp:LinkButton>
                                </div>

                                <div class="ck-line-total">
                                    $<%# Eval("LineTotal", "{0:0.00}") %>
                                </div>

                                <asp:LinkButton ID="btnRemove" runat="server"
                                    CssClass="ck-remove-ic"
                                    CommandName="REMOVE"
                                    CommandArgument='<%# Eval("ProductID") %>'
                                    CausesValidation="false">🗑</asp:LinkButton>
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
                        <div class="ck-muted">Subtotal (selected)</div>
                        <div class="ck-strong">$<asp:Label ID="lblSubtotal" runat="server" Text="0.00" /></div>
                    </div>

                    <div class="ck-sum-row">
                        <div class="ck-muted">Delivery</div>
                        <div class="ck-pickup">Self Pickup</div>
                    </div>

                    <div class="ck-divider"></div>

                    <div class="ck-total-row">
                        <div class="ck-total-label">Total</div>
                        <div class="ck-total-val">$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
                    </div>

                    <div class="ck-impact-box">
                        <div class="ck-impact-icon">🌿</div>
                        <div>
                            <div class="ck-impact-top">Total Impact (selected)</div>
                            <div class="ck-impact-big">
                                <asp:Label ID="lblCO2" runat="server" Text="0.0" /> kg CO₂ saved
                            </div>
                        </div>
                    </div>

                    <asp:Label ID="lblPayMsg" runat="server" CssClass="ck-msg" />

                    <div class="ck-cta-wrap">
                        <asp:Button ID="btnPay" runat="server"
                            CssClass="ck-cta-btn"
                            Text="Payment"
                            OnClick="btnPay_Click" />
                    </div>
                </div>
            </div>

        </div>
    </div>

</form>
</body>
</html>
