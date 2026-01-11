<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderSuccess.aspx.cs" Inherits="Business_App_Dev.OrderSuccess" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>My Orders</title>
    <link href="Content/orders.css" rel="stylesheet" />
</head>
<body>
<form runat="server">
    <a href="Default.aspx" class="back-link">← Back to Home</a>

    <h1>My Orders</h1>

    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="error-box">
        <asp:Label ID="lblError" runat="server" />
    </asp:Panel>

    <asp:Panel ID="pnlOrder" runat="server" Visible="false">
        <div class="order-card">
            <div class="order-header">
                <div>
                    <div class="muted">Order ID</div>
                    <strong><asp:Label ID="lblOrderId" runat="server" /></strong>
                </div>
                <div class="right">
                    <div class="muted">Order Date</div>
                    <strong><asp:Label ID="lblOrderDate" runat="server" /></strong>
                </div>
            </div>

            <div class="order-status">
                <div class="active">Confirmed</div>
                <div>Preparing</div>
                <div>Ready for Pickup</div>
            </div>

            <div class="items">
                <asp:Repeater ID="rptItems" runat="server">
                    <ItemTemplate>
                        <div class="item-row">
                            <div class="item-name"><%# Eval("ProductName") %></div>
                            <div class="item-qty">Qty: <%# Eval("Quantity") %> × $<%# Eval("UnitPrice", "{0:0.00}") %></div>
                            <div class="item-total">$<%# Eval("LineTotal", "{0:0.00}") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="order-total">
                Total <span>$<asp:Label ID="lblTotal" runat="server" /></span>
            </div>
        </div>
    </asp:Panel>

</form>
</body>
</html>
