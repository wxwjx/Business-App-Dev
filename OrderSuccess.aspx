<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderSuccess.aspx.cs" Inherits="Business_App_Dev.OrderSuccess" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Success</title>
    <link href="Content/orders.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">

    <a class="back-link" href="Product.aspx">← Back to shopping</a>

    <asp:Panel ID="pnlError" runat="server" CssClass="error-box" Visible="false">
        <asp:Label ID="lblError" runat="server" />
    </asp:Panel>

    <div class="order-card">
        <div class="order-header">
            <div>
                <h1 style="margin:0;">Order Successful 🎉</h1>
                <div class="muted">Order ID: <asp:Label ID="lblOrderId" runat="server" /></div>
            </div>
            <div class="right">
                <div class="muted">Status</div>
                <div style="font-weight:800;">PAID</div>
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
                        <div>
                            <div class="item-name"><%# Eval("ProductName") %></div>
                            <div class="muted">Qty: <%# Eval("Quantity") %></div>
                        </div>
                        <div class="item-total">$<%# Eval("LineTotal", "{0:0.00}") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="order-total">
                <div>Total</div>
                <div>$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
            </div>
        </div>

        <!-- Pickup + Map -->
        <div style="margin-top:16px; display:grid; grid-template-columns:1fr 1fr; gap:14px;">
            <div style="border:1px solid #f0f0f0; border-radius:14px; padding:14px;">
                <h3 style="margin:0 0 10px;">Pickup Details</h3>

                <div class="muted">Shop</div>
                <div style="font-weight:700;"><asp:Label ID="lblShopName" runat="server" Text="-" /></div>

                <div style="height:10px;"></div>

                <div class="muted">Address</div>
                <div style="font-weight:700;"><asp:Label ID="lblAddress" runat="server" Text="-" /></div>

                <div style="height:10px;"></div>

                <div class="muted">Pickup Window</div>
                <div style="font-weight:700;"><asp:Label ID="lblPickupWindow" runat="server" Text="(Not specified)" /></div>

                <div style="margin-top:12px;">
                    <asp:HyperLink ID="lnkDirections" runat="server" Target="_blank"
                        style="display:inline-block;padding:10px 14px;border-radius:12px;background:#111;color:#fff;font-weight:800;text-decoration:none;"
                        Text="Directions" />
                </div>
            </div>

            <div style="border:1px solid #f0f0f0; border-radius:14px; padding:14px;">
                <div style="border:1px solid #f0f0f0; border-radius:14px; overflow:hidden;">
                    <iframe id="mapFrame" runat="server"
                        width="100%" height="280" style="border:0;"
                        loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>
                </div>
            </div>
        </div>

    </div>
</form>
</body>
</html>
