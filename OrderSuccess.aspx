<%@ Page Title="Order Success"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrderSuccess.aspx.cs"
    Inherits="Business_App_Dev.OrderSuccess" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Content/orders.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <a class="back-link" href="Product.aspx">
        ← <asp:Label ID="lblBackToShopping" runat="server" Text="Back to shopping" />
    </a>

    <asp:Panel ID="pnlError" runat="server" CssClass="error-box" Visible="false">
        <asp:Label ID="lblError" runat="server" />
    </asp:Panel>

    <div class="order-card">
        <div class="order-header">
            <div>
                <h1 style="margin:0;">
                    <asp:Label ID="lblOrderSuccessful" runat="server" Text="Order Successful" /> 🎉
                </h1>
                <div class="muted">
                    <asp:Label ID="lblOrderIdText" runat="server" Text="Order ID:" />
                    <asp:Label ID="lblOrderId" runat="server" />
                </div>
            </div>
            <div class="right">
                <div class="muted"><asp:Label ID="lblStatusText" runat="server" Text="Status" /></div>
                <div style="font-weight:800;">
                    <asp:Label ID="lblPayStatus" runat="server" Text="PAID" />
                </div>
            </div>
        </div>

        <div class="order-status">
            <div class="active"><asp:Label ID="lblStepConfirmed" runat="server" Text="Confirmed" /></div>
            <div><asp:Label ID="lblStepPreparing" runat="server" Text="Preparing" /></div>
            <div><asp:Label ID="lblStepReady" runat="server" Text="Ready for Pickup" /></div>
        </div>

        <asp:Repeater ID="rptSellerGroups" runat="server" OnItemDataBound="rptSellerGroups_ItemDataBound">
            <ItemTemplate>

                <div style="margin-top:16px; border:1px solid #f0f0f0; border-radius:16px; padding:14px;">
                    <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:10px;">
                        <div>
                            <div class="muted">
                                <asp:Label ID="lblPickupLocationText" runat="server" Text="Pickup Location" />
                            </div>
                            <div style="font-weight:900;font-size:18px;"><%# Eval("SellerName") %></div>
                            <div class="muted" style="margin-top:4px;"><%# Eval("Address") %></div>
                            <div class="muted" style="margin-top:6px;">
                                <asp:Label ID="lblPickupWindowText" runat="server" Text="Pickup Window:" />
                                <span style="font-weight:800;"><%# Eval("PickupWindow") %></span>
                            </div>
                        </div>
                        <div style="text-align:right;">
                            <div class="muted">
                                <asp:Label ID="lblSellerStatusText" runat="server" Text="Seller Status" />
                            </div>
                            <div style="font-weight:900;"><%# Eval("SellerStatus") %></div>
                        </div>
                    </div>

                    <div class="items" style="margin-top:12px;">
                        <asp:Repeater ID="rptItemsBySeller" runat="server">
                            <ItemTemplate>
                                <div class="item-row">
                                    <div>
                                        <div class="item-name"><%# Eval("ProductName") %></div>
                                        <div class="muted">
                                            <asp:Label ID="lblQtyText" runat="server" Text="Qty:" />
                                            <%# Eval("Quantity") %>
                                        </div>
                                    </div>
                                    <div class="item-total">$<%# Eval("LineTotal", "{0:0.00}") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <div class="order-total" style="margin-top:10px;">
                            <div><asp:Label ID="lblSellerSubtotalText" runat="server" Text="Seller Subtotal" /></div>
                            <div>$<%# Eval("SellerSubtotal", "{0:0.00}") %></div>
                        </div>
                    </div>

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

        <div class="items" style="margin-top:16px;">
            <div class="order-total">
                <div><asp:Label ID="lblTotalText" runat="server" Text="Total" /></div>
                <div>$<asp:Label ID="lblTotal" runat="server" Text="0.00" /></div>
            </div>
        </div>

    </div>

</asp:Content>
