<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Business_App_Dev.Feedback" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats - Feedback</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">

    <!-- TOP BAR (same as Product) -->
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
                <a href="Order.aspx">Orders</a>
                <a href="Profile.aspx">Profile</a>
                <a href="About.aspx">About Us</a>
                <a class="active" href="Feedback.aspx">Feedback</a>
                <a href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="Profile.aspx" title="Account">👤</a>
            </div>

        </div>
    </header>

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>Feedback</h1>
            <p>Share your EcoEats experience and manage your past feedback.</p>
        </div>
    </section>

    <!-- MAIN CONTENT -->
    <section class="ee-container" style="padding:40px 0 70px; display:flex; flex-wrap:wrap; gap:32px;">

        <!-- LEFT: SUBMIT NEW FEEDBACK -->
        <div style="
            flex:1 1 320px;
            max-width: 420px;
            background:#fff;
            border-radius:18px;
            padding:22px 24px;
            box-shadow:0 16px 40px rgba(0,0,0,0.08);
        ">
            <h2 style="font-size:22px; font-weight:700; margin-bottom:4px;">Submit New Feedback</h2>
            <p style="margin:0 0 16px; color:#666; font-size:14px;">How would you rate your experience?</p>

            <!-- hidden field for editing -->
            <asp:HiddenField ID="hfFeedbackID" runat="server" />

            <div style="margin-bottom:14px;">
                <label>Rating</label><br />
                <asp:DropDownList ID="ddlRating" runat="server" CssClass="ee-input"
                                  Style="margin-top:4px; padding:6px 10px;">
                    <asp:ListItem Text="Select rating" Value="" />
                    <asp:ListItem Text="1 - Very poor" Value="1" />
                    <asp:ListItem Text="2" Value="2" />
                    <asp:ListItem Text="3" Value="3" />
                    <asp:ListItem Text="4" Value="4" />
                    <asp:ListItem Text="5 - Excellent" Value="5" />
                </asp:DropDownList>
            </div>

            <div style="margin-bottom:14px;">
                <label>Quick feedback tag (optional)</label><br />
                <asp:DropDownList ID="ddlTag" runat="server" CssClass="ee-input"
                                  Style="margin-top:4px; padding:6px 10px;">
                    <asp:ListItem Text="-- choose a tag --" Value="" />
                    <asp:ListItem Text="Pricing" Value="Pricing" />
                    <asp:ListItem Text="Usability" Value="Usability" />
                    <asp:ListItem Text="Impact" Value="Impact" />
                    <asp:ListItem Text="Pickup" Value="Pickup" />
                    <asp:ListItem Text="Support" Value="Support" />
                    <asp:ListItem Text="Variety" Value="Variety" />

                </asp:DropDownList>
            </div>

            <div style="margin-bottom:16px;">
                <label>Tell us more (optional)</label><br />
                <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Rows="4"
                             CssClass="ee-input"
                             Style="margin-top:4px; width:100%; resize:vertical;"></asp:TextBox>
            </div>

            <div style="display:flex; gap:10px; align-items:center;">
                <asp:Button ID="btnSubmit" runat="server"
                            Text="Submit Feedback"
                            CssClass="ee-btn-primary"
                            Style="padding:8px 18px; border-radius:999px; font-size:14px;"
                            OnClick="btnSubmit_Click" />

                <asp:Button ID="btnCancelEdit" runat="server"
                            Text="Cancel"
                            Visible="False"
                            CssClass="ee-btn-secondary"
                            Style="padding:8px 16px; border-radius:999px; font-size:13px;"
                            OnClick="btnCancelEdit_Click" />

                <asp:Label ID="lblFormMessage" runat="server"
                           Style="margin-left:6px; font-size:13px; color:#0b7c3a;"></asp:Label>
            </div>
        </div>

        <!-- RIGHT: MY FEEDBACK LIST -->
        <div style="flex:1 1 360px; min-width:320px;">
            <h2 style="font-size:22px; font-weight:700; margin-bottom:10px;">My Feedback</h2>
            <p style="margin:0 0 16px; color:#666; font-size:14px;">
                View, edit or delete your previous feedback submissions.
            </p>

            <asp:Repeater ID="rptFeedback" runat="server" OnItemCommand="rptFeedback_ItemCommand">
                <ItemTemplate>
                    <div style="
                        background:#fff;
                        border-radius:14px;
                        padding:16px 18px;
                        margin-bottom:12px;
                        box-shadow:0 10px 26px rgba(0,0,0,0.04);
                    ">
                        <div style="display:flex; justify-content:space-between; align-items:center;">
                            <div>
                                <span style="color:#f5a623; font-size:16px;">
                                    <%# new string('★', Convert.ToInt32(Eval("Rating"))) %>
                                </span>
                                <span style="margin-left:6px; font-size:13px; color:#888;">
                                    <%# Eval("Rating") %>/5
                                </span>
                               <asp:Label ID="lblTag" runat="server"
           Text='<%# Convert.ToString(Eval("Tag")) %>'
           Visible='<%# !String.IsNullOrEmpty(Convert.ToString(Eval("Tag"))) %>'
           Style="margin-left:10px; padding:2px 8px; border-radius:999px;
                  background:#e8f5e9; font-size:11px; color:#1b5e20;">
</asp:Label>

                            </div>
                            <div style="font-size:11px; color:#999;">
                                <%# String.Format("{0:dd MMM yyyy}", Eval("CreatedAt")) %>
                            </div>
                        </div>

                        <p style="margin:10px 0 8px; font-size:14px; color:#444;">
                            <%# String.IsNullOrWhiteSpace(Eval("Comments").ToString())
                                    ? "(No additional comments)"
                                    : Eval("Comments") %>
                        </p>

                        <div style="font-size:13px; text-align:right;">
                            <asp:LinkButton ID="lnkEdit" runat="server"
                                            CommandName="edit"
                                            CommandArgument='<%# Eval("FeedbackID") %>'
                                            Style="margin-right:12px; color:#007bff; text-decoration:none;">
                                ✏ Edit
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server"
                                            CommandName="delete"
                                            CommandArgument='<%# Eval("FeedbackID") %>'
                                            OnClientClick="return confirm('Delete this feedback?');"
                                            Style="color:#d93025; text-decoration:none;">
                                🗑 Delete
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoFeedback" runat="server" Visible="false"
           Style="margin-top:16px; padding:12px 16px;
                  background:#fff8e1; border-radius:8px;
                  color:#8a6d3b; font-size:14px;">
    You haven't submitted any feedback yet.
</asp:Panel>
        </div>

    </section>

</form>
</body>
</html>
