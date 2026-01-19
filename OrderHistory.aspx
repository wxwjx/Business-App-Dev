<%@ Page Title="Order History | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrderHistory.aspx.cs"
    Inherits="Business_App_Dev.OrderHistory" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/OrderHistory.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- PAGE HERO -->
    <section class="ee-hero oh-hero">
        <div class="ee-container">
            <h1><asp:Label ID="lblHeroTitle" runat="server" Text="Your Orders" /></h1>
            <p><asp:Label ID="lblHeroSub" runat="server" Text="View your purchase history and open order details for pickup info." /></p>
        </div>
    </section>

    <!-- CONTENT -->
    <div class="ee-container oh-content">

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <!-- empty state -->
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="oh-empty">
            <div class="oh-empty-title">
                <asp:Label ID="lblEmptyTitle" runat="server" Text="No orders yet" />
            </div>
            <div class="oh-empty-text">
                <asp:Label ID="lblEmptyText" runat="server" Text="Once you checkout, your past orders will appear here." />
            </div>
            <a class="oh-empty-btn" href="Product.aspx">
                <asp:Label ID="lblBrowseDeals" runat="server" Text="Browse deals" />
            </a>
        </asp:Panel>

        <!-- Orders list -->
        <asp:Repeater ID="rptOrders" runat="server" OnItemDataBound="rptOrders_ItemDataBound">
            <ItemTemplate>
                <div class="oh-card">

                    <div class="oh-row">
                        <div>
                            <div class="oh-order-id">
                                <asp:Label ID="lblOrderHash" runat="server" Text="Order #" />
                                <%# Eval("OrderID") %>
                            </div>

                            <div class="oh-date"><%# Eval("CreatedAt", "{0:dd MMM yyyy, hh:mm tt}") %></div>

                            <span class="oh-status">
                                <asp:Label ID="lblPayStatusRow" runat="server" Text='<%# Eval("PayStatus") %>' />
                            </span>
                        </div>

                        <div class="oh-right">
                            <div class="oh-total">$<%# Eval("TotalAmount", "{0:0.00}") %></div>

                            <a class="oh-btn" href='OrderDetails.aspx?orderId=<%# Eval("OrderID") %>'>
                                <asp:Label ID="lblViewDetails" runat="server" Text="View Details" />
                            </a>
                        </div>
                    </div>

                    <div class="oh-ref">
                        <asp:Label ID="lblPaymentRef" runat="server" Text="Payment Ref:" />
                        <span><%# Eval("StripeSessionId") %></span>
                    </div>

                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>

</asp:Content>
