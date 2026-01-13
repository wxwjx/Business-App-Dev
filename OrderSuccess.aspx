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
                <div style="font-weight:800;"><asp:Label ID="lblPayStatus" runat="server" Text="PAID" /></div>
            </div>
        </div>

        <div class="order-status">
            <div class="active">Confirmed</div>
            <div>Preparing</div>
            <div>Ready for Pickup</div>
        </div>

        <!-- ✅ SELLER GROUPS -->
        <asp:Repeater ID="rptSellerGroups" runat="server" OnItemDataBound="rptSellerGroups_ItemDataBound">
            <ItemTemplate>

                <div style="margin-top:16px; border:1px solid #f0f0f0; border-radius:16px; padding:14px;">
                    <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:10px;">
                        <div>
                            <div class="muted">Pickup Location</div>
                            <div style="font-weight:900;font-size:18px;"><%# Eval("SellerName") %></div>
                            <div class="muted" style="margin-top:4px;"><%# Eval("Address") %></div>
                            <div class="muted" style="margin-top:6px;">
                                Pickup Window: <span style="font-weight:800;"><%# Eval("PickupWindow") %></span>
                            </div>
                        </div>
                        <div style="text-align:right;">
                            <div class="muted">Seller Status</div>
                            <div style="font-weight:900;"><%# Eval("SellerStatus") %></div>
                        </div>
                    </div>

                    <!-- items for this seller -->
                    <div class="items" style="margin-top:12px;">
                        <asp:Repeater ID="rptItemsBySeller" runat="server">
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

                        <div class="order-total" style="margin-top:10px;">
                            <div>Seller Subtotal</div>
                            <div>$<%# Eval("SellerSubtotal", "{0:0.00}") %></div>
                        </div>
                    </div>

                    <!-- directions + map -->
                    <div style="margin-top:14px; display:grid; grid-template-columns: 1fr 1fr; gap:14px;">
                        <div style="border:1px solid #f0f0f0; border-radius:14px; padding:14px;">
                            <asp:HyperLink ID="lnkDirectionsSeller" runat="server" Target="_blank"
                                style="display:inline-block;padding:10px 14px;border-radius:12px;background:#111;color:#fff;font-weight:800;text-decoration:none;"
                                Text="Directions" />
                        </div>

                        <div style="border:1px solid #f0f0f0; border-radius:14px; overflow:hidden;">
                            <iframe id="mapFrameSeller" runat="server"
                                width="100%" height="220" style="border:0;"
                                loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>
                        </div>
                    </div>
                </div>

            </ItemTemplate>
        </asp:Repeater>

        <!-- ✅ GRAND TOTAL -->
        <div class="items" style="margin-top:16px;">
            <div class="order-total">
                <div>Total</div>
                <div>$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
            </div>
        </div>

    </div>
</form>
</body>
</html>
