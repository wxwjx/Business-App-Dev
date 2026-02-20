<%@ Page Title="Seller Feedback | EcoEats"
    Language="C#"
    MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true"
    CodeBehind="SellerFeedback.aspx.cs"
    Inherits="Business_App_Dev.SellerFeedbackPage" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
    .ee-hero {
        padding: 28px 0 18px;
    }

        .ee-hero h1 {
            margin: 0;
            font-size: 34px;
        }

        .ee-hero p {
            margin: 8px 0 0;
            opacity: .75;
        }

    .sf-wrap {
        padding: 18px 0 44px;
    }

    .sf-summary {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 12px;
        margin-bottom: 14px;
    }

    @media (max-width: 900px) {
        .sf-summary {
            grid-template-columns: 1fr;
        }
    }

    .sf-sum-card {
        background: #fff;
        border: 1px solid rgba(0,0,0,.08);
        border-radius: 18px;
        padding: 14px 16px;
        box-shadow: 0 10px 30px rgba(0,0,0,.05);
    }

    .sf-sum-label {
        font-size: 13px;
        opacity: .7;
    }

    .sf-sum-value {
        margin-top: 6px;
        font-size: 22px;
        font-weight: 800;
    }

    .sf-sum-sub {
        font-size: 14px;
        opacity: .7;
        margin-left: 6px;
        font-weight: 700;
    }

    .sf-sum-small {
        font-size: 16px;
        font-weight: 800;
    }

    .sf-empty {
        background: rgba(0,0,0,.03);
        border: 1px dashed rgba(0,0,0,.18);
        border-radius: 18px;
        padding: 22px;
    }

    .sf-empty-title {
        font-weight: 800;
        font-size: 18px;
    }

    .sf-empty-sub {
        margin-top: 6px;
        opacity: .75;
    }

    .sf-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
    }

    .sf-card {
        background: #fff;
        border: 1px solid rgba(0,0,0,.08);
        border-radius: 18px;
        padding: 14px 16px;
        box-shadow: 0 10px 30px rgba(0,0,0,.05);
    }

    .sf-card-top {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 14px;
    }

    .sf-order {
        font-weight: 900;
        font-size: 16px;
    }

    .sf-date {
        margin-top: 4px;
        font-size: 13px;
        opacity: .7;
    }

    .sf-right {
        text-align: right;
    }

    .sf-stars {
        font-size: 16px;
        letter-spacing: 2px;
    }

        .sf-stars span {
            opacity: .25;
        }

            .sf-stars span.on {
                opacity: 1;
            }

    .sf-score {
        margin-top: 4px;
        font-weight: 800;
        opacity: .8;
        font-size: 13px;
    }

    .sf-comment {
        margin-top: 10px;
        line-height: 1.5;
    }

    .sf-muted {
        opacity: .65;
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <section class="ee-hero">
        <div class="ee-container">
            <h1>Customer Feedback</h1>
            <p>See how customers rated their orders and what they said.</p>
        </div>
    </section>

    <div class="ee-container sf-wrap">

        <!-- Summary row -->
        <div class="sf-summary">
            <div class="sf-sum-card">
                <div class="sf-sum-label">Average rating</div>
                <div class="sf-sum-value">
                    <asp:Label ID="lblAvgRating" runat="server" Text="-" />
                    <span class="sf-sum-sub">/ 5</span>
                </div>
            </div>

            <div class="sf-sum-card">
                <div class="sf-sum-label">Total reviews</div>
                <div class="sf-sum-value">
                    <asp:Label ID="lblTotalReviews" runat="server" Text="0" />
                </div>
            </div>

            <div class="sf-sum-card">
                <div class="sf-sum-label">Latest review</div>
                <div class="sf-sum-value sf-sum-small">
                    <asp:Label ID="lblLatestDate" runat="server" Text="-" />
                </div>
            </div>
        </div>

        <!-- Empty state -->
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="sf-empty">
            <div class="sf-empty-title">No feedback yet</div>
            <div class="sf-empty-sub">Once customers rate their orders, you’ll see it here.</div>
        </asp:Panel>

        <!-- List -->
        <asp:Repeater ID="rptFeedback" runat="server">
            <HeaderTemplate>
                <div class="sf-list">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="sf-card">
                    <div class="sf-card-top">

                        <div class="sf-left">
                            <div class="sf-order">
                                Order #<%# Eval("OrderID") %>
                            </div>
                            <div class="sf-date">
                                <%# Convert.ToDateTime(Eval("CreatedAt")).ToString("dd MMM yyyy, hh:mm tt") %>
                            </div>
                        </div>

                        <div class="sf-right">
                            <div class="sf-stars" aria-label="Rating">
                                <span class='<%# (Convert.ToInt32(Eval("Rating")) >= 1 ? "on" : "") %>'>★</span>
                                <span class='<%# (Convert.ToInt32(Eval("Rating")) >= 2 ? "on" : "") %>'>★</span>
                                <span class='<%# (Convert.ToInt32(Eval("Rating")) >= 3 ? "on" : "") %>'>★</span>
                                <span class='<%# (Convert.ToInt32(Eval("Rating")) >= 4 ? "on" : "") %>'>★</span>
                                <span class='<%# (Convert.ToInt32(Eval("Rating")) >= 5 ? "on" : "") %>'>★</span>
                            </div>
                            <div class="sf-score">
                                <%# Eval("Rating") %>/5
                            </div>
                        </div>

                    </div>

                    <div class="sf-comment">
                        <%# string.IsNullOrWhiteSpace(Eval("Comment")?.ToString())
                            ? "<span class='sf-muted'>No comment provided.</span>"
                            : Server.HtmlEncode(Eval("Comment").ToString()) %>
                    </div>

                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

    </div>

</asp:Content>
