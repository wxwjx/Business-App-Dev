<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="OrderHistory.aspx.cs"
    Inherits="Business_App_Dev.OrderHistory" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Order History | EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- GLOBAL -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <!-- PAGE -->
    <link href="<%= ResolveUrl("~/Content/OrderHistory.css") %>" rel="stylesheet" />

    <!-- Fonts (if EcoEats.css already imports, you can remove) -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>

<body class="ee-body">
<form id="form1" runat="server">

    <!-- TOP BAR (same vibe as Product page) -->
    <header class="ee-topbar">
        <div class="ee-container ee-topbar-inner">
            <a class="ee-brand" href="<%= ResolveUrl("~/Product.aspx") %>">
                <div class="ee-logo">🌿</div>
                <div class="ee-brand-text">
                    <div class="ee-brand-name">EcoEats</div>
                    <div class="ee-brand-tag">Save food. Save money.</div>
                </div>
            </a>

            <nav class="oh-nav">
                <a class="oh-navlink" href="<%= ResolveUrl("~/Product.aspx") %>">Browse</a>
                <a class="oh-navlink" href="<%= ResolveUrl("~/Cart.aspx") %>">Cart</a>
                <a class="oh-navlink is-active" href="<%= ResolveUrl("~/OrderHistory.aspx") %>">Orders</a>
            </nav>
        </div>
    </header>

    <!-- PAGE -->
    <main class="ee-container oh-page">

        <div class="oh-head">
            <a class="oh-back" href="<%= ResolveUrl("~/Product.aspx") %>">← Back to Products</a>

            <div class="oh-titleRow">
                <div>
                    <h1 class="oh-title">Order History</h1>
                    <div class="oh-subtitle">Track your past orders and view pickup details.</div>
                </div>

                <div class="oh-chip">
                    <span class="oh-chip-label">Status</span>
                    <span class="oh-chip-value">PAID / PROCESSING</span>
                </div>
            </div>
        </div>

        <!-- ERROR -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="oh-alert oh-alert-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <!-- EMPTY -->
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="oh-alert oh-alert-empty">
            <div class="oh-empty-title">No orders yet</div>
            <div class="oh-empty-text">When you place an order, it will appear here.</div>
            <a class="oh-primary" href="<%= ResolveUrl("~/Product.aspx") %>">Browse deals</a>
        </asp:Panel>

        <!-- LIST -->
        <asp:Repeater ID="rptOrders" runat="server">
            <ItemTemplate>
                <article class="oh-card">
                    <div class="oh-cardTop">
                        <div>
                            <div class="oh-orderno">Order #<%# Eval("OrderID") %></div>
                            <div class="oh-date"><%# Eval("CreatedAt", "{0:dd MMM yyyy, hh:mm tt}") %></div>
                        </div>

                        <div class="oh-right">
                            <span class='oh-pill <%# (Eval("PayStatus").ToString().ToUpper() == "PAID") ? "is-paid" : "is-other" %>'>
                                <%# Eval("PayStatus") %>
                            </span>
                            <div class="oh-total">$<%# Eval("TotalAmount", "{0:0.00}") %></div>
                        </div>
                    </div>

                    <div class="oh-divider"></div>

                    <div class="oh-cardBottom">
                        <div class="oh-ref">
                            Payment Ref: <span><%# Eval("StripeSessionId") %></span>
                        </div>

                        <!-- Choose ONE based on your existing OrderDetails parameter -->
                        <!-- If your OrderDetails uses orderId: -->
                        <a class="oh-secondary"
                           href='OrderDetails.aspx?orderId=<%# Eval("OrderID") %>'>
                            View details
                        </a>

                        <!-- If your OrderDetails uses session_id, use this instead and delete the one above:
                        <a class="oh-secondary"
                           href='OrderDetails.aspx?session_id=<%# Eval("StripeSessionId") %>'>
                            View details
                        </a>
                        -->
                    </div>
                </article>
            </ItemTemplate>
        </asp:Repeater>

    </main>

</form>
</body>
</html>
