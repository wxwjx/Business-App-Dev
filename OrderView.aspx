<%@ Page Title="Order Details | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrderView.aspx.cs"
    Inherits="Business_App_Dev.OrderView" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/OrderView.css") %>?v=4" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section class="ee-hero">
        <div class="ee-container">
            <h1>Order Details</h1>
            <p>Review your order and take action below.</p>
        </div>
    </section>

    <div class="ee-container ov-wrap">

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlMain" runat="server" Visible="false">

            <div class="ov-card">

                <div class="ov-top">

                    <div>
                        <div class="ov-title">
                            Order #<asp:Label ID="lblOrderId" runat="server" />
                        </div>

                        <div class="ov-sub">
                            <asp:Label ID="lblCreatedAt" runat="server" />
                        </div>

                        <div class="ov-badges">
                            <span class="ov-badge">
                                <asp:Label ID="lblPayStatus" runat="server" />
                            </span>

                            <span class="ov-badge ov-badge-soft">
                                <asp:Label ID="lblOrderStatus" runat="server" />
                            </span>
                        </div>
                    </div>

                    <div class="ov-right">
                        <div class="ov-total">
                            $<asp:Label ID="lblTotal" runat="server" />
                        </div>

                        <a href="OrderHistory.aspx" class="ov-back">
                            ← Back to orders
                        </a>
                    </div>

                </div>

                <div class="ov-actions">

                    <asp:HyperLink ID="lnkChatSeller"
                        runat="server"
                        CssClass="ov-btn ov-btn-chat"
                        Text="💬 Message Seller" />

                    <asp:HyperLink ID="lnkRateOrder"
                        runat="server"
                        CssClass="ov-btn ov-btn-primary"
                        Text="⭐ Rate Your Order" />

                </div>

                <div class="ov-ref">

                    <span class="ov-ref-label">
                        Payment Ref
                    </span>

                    <asp:Panel ID="refPill"
                        runat="server"
                        CssClass="ov-ref-pill">

                        <span class="ov-ref-val">
                            <asp:Label ID="lblStripeSessionId" runat="server" />
                        </span>

                        <button type="button" class="ov-copy">
                            Copy
                        </button>

                    </asp:Panel>

                </div>

            </div>

            <div class="ov-card">

                <div class="ov-section-title">
                    Items
                </div>

                <asp:Panel ID="pnlNoItems"
                    runat="server"
                    Visible="false"
                    CssClass="ov-empty">
                    No items found for this order.
                </asp:Panel>

                <asp:Repeater ID="rptItems" runat="server">

                    <HeaderTemplate>
                        <div class="ov-items">
                    </HeaderTemplate>

                    <ItemTemplate>

                        <div class="ov-item">

                            <div>
                                <div class="ov-item-name">
                                    <%# Eval("ProductName") %>
                                </div>

                                <div class="ov-item-meta">
                                    Qty: <%# Eval("Quantity") %>
                                </div>
                            </div>

                            <div class="ov-item-right">
                                $<%# Eval("LineTotal", "{0:0.00}") %>
                            </div>

                        </div>

                    </ItemTemplate>

                    <FooterTemplate>
                        </div>
                    </FooterTemplate>

                </asp:Repeater>

            </div>

        </asp:Panel>

    </div>

    <script>
        document.addEventListener("click", async function (e) {

            const btn = e.target.closest(".ov-copy");
            if (!btn) return;

            const pill = btn.closest(".ov-ref-pill");
            const value = pill?.getAttribute("data-ref") || "";

            try {
                await navigator.clipboard.writeText(value);

                const old = btn.textContent;
                btn.textContent = "Copied";

                setTimeout(function () {
                    btn.textContent = old;
                }, 900);

            } catch (err) { }

        });
    </script>

</asp:Content>