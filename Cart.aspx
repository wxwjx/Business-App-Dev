<%@ Page Title="Shopping Cart | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs"
    Inherits="Business_App_Dev.Cart" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/Cart.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="ck-container">

        <a class="ck-back" href="Product.aspx">
            ← <asp:Label ID="lblContinueShopping" runat="server" Text="Continue Shopping" />
        </a>

        <!-- Title -->
        <div class="ck-header-title">
            <div class="ck-h-title">
                <span class="ck-cart-ic">🛒</span>
                <asp:Label ID="lblCartTitle" runat="server" Text="Shopping Cart" />
                <span class="ck-muted ck-title-count">
                    (<asp:Label ID="lblItemCount" runat="server" Text="0" />
                    <asp:Label ID="lblItemsText" runat="server" Text="items" />)
                </span>
            </div>
        </div>

        <div class="ck-grid">

            <!-- LEFT -->
            <div class="ck-left">

                <!-- Empty state -->
                <asp:Panel ID="lblEmpty" runat="server" Visible="false" CssClass="ck-empty">
                    <asp:Label ID="lblEmptyText" runat="server" Text="Your cart is empty." />
                </asp:Panel>

                <!-- Select all row -->
                <asp:Panel ID="pnlSelectAll" runat="server" CssClass="ck-leftbar" Visible="false">
                    <label class="ck-selectall">
                        <asp:CheckBox ID="chkSelectAll"
                            runat="server"
                            AutoPostBack="true"
                            OnCheckedChanged="chkSelectAll_CheckedChanged" />
                        <span><asp:Label ID="lblSelectAllText" runat="server" Text="Select all" /></span>
                    </label>

                    <asp:Label ID="lblSelectedCount" runat="server" CssClass="ck-selected-count" Text="" />
                </asp:Panel>

                <!-- Items list -->
                <asp:Repeater ID="rptCart" runat="server"
                    OnItemCommand="rptCart_ItemCommand"
                    OnItemDataBound="rptCart_ItemDataBound">

                    <ItemTemplate>
                        <div class="ck-item-card">

                            <div class="ck-item-select">
                                <asp:HiddenField ID="hfPid" runat="server" Value='<%# Eval("ProductID") %>' />
                                <asp:CheckBox ID="chkSelect"
                                    runat="server"
                                    AutoPostBack="true"
                                    OnCheckedChanged="chkSelect_CheckedChanged" />
                            </div>

                            <div class="ck-item-img"
                                 style='background-image:url("<%# Eval("ImageUrl") %>");'>
                            </div>

                            <div class="ck-item-mid">
                                <div class="ck-item-name"><%# Eval("ProductName") %></div>
                                <div class="ck-item-desc"><%# Eval("Subtitle") %></div>

                                <div class="ck-item-price">
                                    <div class="ck-price-now">$<%# Eval("PriceNow", "{0:0.00}") %></div>
                                </div>
                            </div>

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
                    <div class="ck-summary-title">
                        <asp:Label ID="lblOrderSummaryTitle" runat="server" Text="Order Summary" />
                    </div>

                    <div class="ck-sum-row">
                        <div class="ck-muted">
                            <asp:Label ID="lblSubtotalText" runat="server" Text="Subtotal (selected)" />
                        </div>
                        <div class="ck-strong">$<asp:Label ID="lblSubtotal" runat="server" Text="0.00" /></div>
                    </div>

                    <div class="ck-sum-row">
                        <div class="ck-muted">
                            <asp:Label ID="lblDeliveryText" runat="server" Text="Delivery" />
                        </div>
                        <div class="ck-pickup">
                            <asp:Label ID="lblSelfPickup" runat="server" Text="Self Pickup" />
                        </div>
                    </div>

                    <div class="ck-divider"></div>

                    <div class="ck-total-row">
                        <div class="ck-total-label">
                            <asp:Label ID="lblTotalText" runat="server" Text="Total" />
                        </div>
                        <div class="ck-total-val">$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
                    </div>

                    <div class="ck-impact-box">
                        <div class="ck-impact-icon">🌿</div>
                        <div>
                            <div class="ck-impact-top">
                                <asp:Label ID="lblTotalImpactText" runat="server" Text="Total Impact (selected)" />
                            </div>
                            <div class="ck-impact-big">
                                <asp:Label ID="lblCO2" runat="server" Text="0.0" />
                                <asp:Label ID="lblKgCO2SavedText" runat="server" Text="kg CO₂ saved" />
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

</asp:Content>
