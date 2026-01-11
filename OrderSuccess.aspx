<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderSuccess.aspx.cs" Inherits="Business_App_Dev.OrderSuccess" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Success</title>

    <!-- adjust path if your css is elsewhere -->
    <link href="Content/orders.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">

    <a class="back-link" href="Products.aspx">← Back to shopping</a>

    <asp:Panel ID="pnlError" runat="server" CssClass="error-box" Visible="false">
        <asp:Label ID="lblError" runat="server" />
    </asp:Panel>

    <div class="order-card">
        <div class="order-header">
            <div>
                <h2 style="margin:0;">Order Successful 🎉</h2>
                <div class="muted">Order ID: <asp:Label ID="lblOrderId" runat="server" /></div>
            </div>
            <div class="right">
                <div class="muted">Status</div>
                <div style="font-weight:800;">PAID</div>
            </div>
        </div>

        <!-- status pills -->
        <div class="order-status">
            <div class="active">Confirmed</div>
            <div>Preparing</div>
            <div>Ready for Pickup</div>
        </div>

        <!-- items -->
        <div class="items">
            <asp:Repeater ID="rptItems" runat="server">
                <ItemTemplate>
                    <div class="item-row">
                        <div>
                            <div class="item-name"><%# Eval("ProductName") %></div>
                            <div class="muted"><%# Eval("ShopName") %></div>
                        </div>
                        <div class="item-qty">x <%# Eval("Qty") %></div>
                        <div class="item-total">$<%# Eval("LineTotal", "{0:0.00}") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="order-total">
                <div>Total</div>
                <div>$<asp:Label ID="lblTotal" runat="server" /></div>
            </div>
        </div>

        <!-- pickup + map -->
        <div class="pickup-section">
            <div class="pickup-left">
                <h3 style="margin:0 0 8px;">Pickup Details</h3>

                <div class="muted">Shop</div>
                <div class="pickup-value"><asp:Label ID="lblShopName" runat="server" /></div>

                <div style="height:10px;"></div>

                <div class="muted">Address</div>
                <div class="pickup-value"><asp:Label ID="lblAddress" runat="server" /></div>

                <div style="height:10px;"></div>

                <div class="muted">Pickup Window</div>
                <div class="pickup-value"><asp:Label ID="lblPickupWindow" runat="server" Text="(Not specified)" /></div>

                <div class="pickup-actions">
                    <asp:HyperLink ID="lnkDirections" runat="server" Target="_blank" CssClass="btn-primary" Text="Directions" />
                </div>
            </div>

            <div class="pickup-right">
                <div class="map-wrap">
                    <iframe id="mapFrame" runat="server"
                        width="100%" height="280" style="border:0;"
                        loading="lazy" referrerpolicy="no-referrer-when-downgrade">
                    </iframe>
                </div>
                <div class="muted" style="margin-top:8px;">
                    Tip: open “Directions” on your phone for navigation.
                </div>
            </div>
        </div>

    </div>
</form>
</body>
</html>
