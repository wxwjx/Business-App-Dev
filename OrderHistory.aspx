<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="Business_App_Dev.OrderHistory" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Order History | EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- GLOBAL CSS (same as Product.aspx) -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <!-- PAGE CSS -->
    <link href="<%= ResolveUrl("~/Content/OrderHistory.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <!-- TOP BAR (copied from Product.aspx, only changed active tab) -->
    <header class="ee-topbar">
        <div class="ee-container ee-topbar-inner">

            <div class="ee-brand">
                <div class="ee-logo">
                    <svg viewBox="0 0 24 24" aria-hidden="true">
                        <path d="M19 3c-6.5.7-11 3.8-13.8 7.2C2.6 13.4 2.2 17.2 4 21c3.8 1.8 7.6 1.4 10.8-1.2C18.2 17 21.3 12.5 22 6c.1-1.2-.7-2.9-3-3z"></path>
                    </svg>
                </div>
                <div class="ee-brand-name">EcoEats</div>
            </div>

            <div class="ee-search">
                <input type="text" placeholder="Search for meals, restaurants..." />
            </div>

            <nav class="ee-nav">
                <a href="Product.aspx">Home</a>
                <a class="active" href="OrderHistory.aspx">Orders</a>
                <a href="About.aspx">About Us</a>
                <a href="#">Help</a>
                <a href="Feedback.aspx">Feedback</a>
                <a href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="Profile.aspx" title="Account">👤</a>
            </div>

        </div>
    </header>

    <!-- PAGE HERO (smaller than Product hero, but same vibe) -->
    <section class="ee-hero oh-hero">
        <div class="ee-container">
            <h1>Your Orders</h1>
            <p>View your purchase history and open order details for pickup info.</p>
        </div>
    </section>

    <!-- CONTENT -->
    <div class="ee-container oh-content">

        <!-- error box using your global style -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <!-- empty state -->
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="oh-empty">
            <div class="oh-empty-title">No orders yet</div>
            <div class="oh-empty-text">Once you checkout, your past orders will appear here.</div>
            <a class="oh-empty-btn" href="Product.aspx">Browse deals</a>
        </asp:Panel>

        <!-- Orders list -->
        <asp:Repeater ID="rptOrders" runat="server">
            <ItemTemplate>
                <div class="oh-card">

                    <div class="oh-row">
                        <div>
                            <div class="oh-order-id">Order #<%# Eval("OrderID") %></div>
                            <div class="oh-date"><%# Eval("CreatedAt", "{0:dd MMM yyyy, hh:mm tt}") %></div>
                            <span class="oh-status"><%# Eval("PayStatus") %></span>
                        </div>

                        <div class="oh-right">
                            <div class="oh-total">$<%# Eval("TotalAmount", "{0:0.00}") %></div>

                            <!-- Choose ONE based on your existing OrderDetails parameter -->
                            <a class="oh-btn"
                               href='OrderDetails.aspx?orderId=<%# Eval("OrderID") %>'>
                                View Details
                            </a>

                            <%-- If your OrderDetails uses session_id instead, use this and delete the one above:
                            <a class="oh-btn" href='OrderDetails.aspx?session_id=<%# Eval("StripeSessionId") %>'>View Details</a>
                            --%>
                        </div>
                    </div>

                    <div class="oh-ref">
                        Payment Ref: <span><%# Eval("StripeSessionId") %></span>
                    </div>

                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>

</form>
</body>
</html>
