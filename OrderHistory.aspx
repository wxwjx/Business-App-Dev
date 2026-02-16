<%@ Page Title="Order History | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrderHistory.aspx.cs"
    Inherits="Business_App_Dev.OrderHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/OrderHistory.css") %>?v=3" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- HERO -->
    <section class="ee-hero oh-hero">
        <div class="ee-container">
            <h1><asp:Label ID="lblHeroTitle" runat="server" Text="Your Orders" /></h1>
            <p><asp:Label ID="lblHeroSub" runat="server" Text="View your purchase history and open order details for pickup info." /></p>
        </div>
    </section>

    <div class="ee-container oh-content">

        <!-- Error -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="ee-error">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <!-- Toolbar -->
        <div class="oh-toolbar">
            <div class="oh-search">
                <span class="oh-ico">🔎</span>
                <input class="oh-search-input" type="search" placeholder="Search order # or payment ref" />
            </div>

            <div class="oh-toolbar-right">
                <div class="oh-filters">
                    <a class="oh-filter is-active" href="OrderHistory.aspx">All</a>
                    <a class="oh-filter" href="OrderHistory.aspx?status=Paid">Paid</a>
                    <a class="oh-filter" href="OrderHistory.aspx?status=Processing">Processing</a>
                    <a class="oh-filter" href="OrderHistory.aspx?status=Delayed">Delayed</a>
                </div>

                <select class="oh-select">
                    <option>Newest first</option>
                    <option>Oldest first</option>
                </select>
            </div>
        </div>

        <!-- Empty -->
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

        <!-- Orders -->
        <asp:Repeater ID="rptOrders" runat="server" OnItemDataBound="rptOrders_ItemDataBound">
            <ItemTemplate>

                <article class="oh-card oh-clickable"
                         data-href='OrderView.aspx?orderId=<%# Eval("OrderID") %>'>

                    <div class="oh-row">
                        <div class="oh-main">

                            <div class="oh-order-id">
                                <asp:Label ID="lblOrderHash" runat="server" Text="Order #" />
                                <span class="oh-order-num"><%# Eval("OrderID") %></span>
                            </div>

                            <div class="oh-date">
                                <%# Eval("CreatedAt", "{0:dd MMM yyyy, hh:mm tt}") %>
                            </div>

                            <div class="oh-badges">
                                <span class="oh-badge">
                                    <asp:Label ID="lblPayStatusRow" runat="server"
                                        Text='<%# Eval("PayStatus") %>' />
                                </span>

                                <span class="oh-badge oh-badge-soft">
                                    <asp:Label ID="lblOrderStatusRow" runat="server"
                                        Text='<%# Eval("OrderStatus") %>' />
                                </span>
                            </div>
                        </div>

                        <div class="oh-side">
                            <div class="oh-total">
                                $<%# Eval("TotalAmount", "{0:0.00}") %>
                            </div>

                            <div class="oh-actions">
                                <a class="oh-btn"
                                   href='OrderView.aspx?orderId=<%# Eval("OrderID") %>'>
                                    <asp:Label ID="lblViewDetails" runat="server"
                                        Text="View details" />
                                </a>

                                <asp:HyperLink ID="lnkChatSeller" runat="server"
                                    CssClass="oh-btn oh-btn-secondary"
                                    Visible="false" />

                                <asp:HyperLink ID="lnkRateOrder" runat="server"
                                    CssClass="oh-btn oh-btn-secondary"
                                    Visible="false" />
                            </div>
                        </div>
                    </div>

                    <!-- Stuck Banner -->
                    <asp:Panel ID="pnlStuck" runat="server"
                        Visible="false"
                        CssClass="oh-alert oh-alert-warn">

                        <div class="oh-alert-icon">⏱</div>

                        <div class="oh-alert-body">
                            <div class="oh-alert-title">
                                <asp:Label ID="lblStuckTitle"
                                    runat="server"
                                    Text="Order is taking longer than usual" />
                            </div>

                            <div class="oh-alert-text">
                                <asp:Label ID="lblStuckText"
                                    runat="server"
                                    Text="Chat the seller to confirm the latest status." />
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Payment Ref -->
                    <div class="oh-ref">
                        <span class="oh-ref-label">
                            <asp:Label ID="lblPaymentRef"
                                runat="server"
                                Text="Payment Ref" />
                        </span>

                        <span class="oh-ref-pill"
                              data-ref="<%# Eval("StripeSessionId") %>">

                            <span class="oh-ref-val">
                                <%# Eval("StripeSessionId") %>
                            </span>

                            <button type="button" class="oh-copy">
                                Copy
                            </button>
                        </span>
                    </div>

                </article>

            </ItemTemplate>
        </asp:Repeater>

    </div>

    <!-- JS -->
    <script>
        // Card click navigation
        document.addEventListener("click", function (e) {

            // Copy button
            const copyBtn = e.target.closest(".oh-copy");
            if (copyBtn) {
                e.preventDefault();
                e.stopPropagation();

                const pill = copyBtn.closest(".oh-ref-pill");
                const value = pill?.getAttribute("data-ref") || "";

                navigator.clipboard.writeText(value).then(() => {
                    const old = copyBtn.textContent;
                    copyBtn.textContent = "Copied";
                    setTimeout(() => copyBtn.textContent = old, 900);
                });

                return;
            }

            // Card navigation
            const card = e.target.closest(".oh-clickable");
            if (!card) return;

            // Ignore real interactive elements
            if (e.target.closest("a, button, input, select, textarea, label"))
                return;

            const href = card.getAttribute("data-href");
            if (href) window.location.href = href;
        });
    </script>

</asp:Content>
