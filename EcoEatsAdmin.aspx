<%@ Page Title="EcoEats Admin" Language="C#" MasterPageFile="~/Admin.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="EcoEatsAdmin.aspx.cs" Inherits="Business_App_Dev.EcoEatsAdmin" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsAdmin.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">



    <!-- APPROVALS PANEL -->
    <section class="admin-panel active" data-tab="approvals">
        <div class="eco-cardbox">
            <div class="eco-cardbox-title">Pending Seller Applications</div>

            <div class="eco-table-wrap">
                <table class="eco-table">
                    <thead>
                        <tr>
                            <th>Business Name</th>
                            <th>Owner</th>
                            <th>Email</th>
                            <th>Category</th>
                            <th>Submit Date</th>
                            <th class="eco-actions">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                    <asp:Repeater ID="rptApps" runat="server" OnItemCommand="rptApps_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("BusinessName") %></td>
                                <td><%# Eval("Owner") %></td>
                                <td><%# Eval("Email") %></td>
                                <td>
                                    <span class="eco-badge"><%# Eval("Category") %></span>
                                </td>
                                <td><%# Eval("SubmitDate", "{0:yyyy-MM-dd}") %></td>
                                <td class="eco-actions">
                                    <asp:LinkButton ID="btnApprove" runat="server"
                                        CssClass="eco-btn eco-approve"
                                        CommandName="APPROVE"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        ✓ Approve
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnReject" runat="server"
                                        CssClass="eco-btn eco-reject"
                                        CommandName="REJECT"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Confirm reject this seller application?');">
                                        ✕ Reject
                                    </asp:LinkButton>


                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>

                </table>
                <asp:Label ID="lblMsg" runat="server" />

            </div>
        </div>
    </section>

    <!-- FEEDBACK PANEL -->
    <!-- FEEDBACK PANEL -->
    <section class="admin-panel" data-tab="feedback">
    <div class="eco-cardbox">

        <div class="eco-feedback-header">
            <div class="eco-cardbox-title">Customer Feedback</div>

            <asp:LinkButton ID="btnExportFeedback" runat="server"
                CssClass="eco-export-btn"
                OnClick="btnExportFeedback_Click"
                CausesValidation="false">
                <span class="eco-dl">↓</span> Export Report
            </asp:LinkButton>
        </div>

        <div class="eco-feedback-list">
            <asp:Repeater ID="rptFeedback" runat="server">
                <ItemTemplate>
                    <div class="eco-feedback-item">
                        <div class="eco-feedback-top">
                            <div class="eco-feedback-left">
                                <span class="eco-feedback-name"><%# Eval("CustomerName") %></span>

                                <span class="eco-feedback-stars">
                                    <%# GetStars(Convert.ToInt32(Eval("Rating"))) %>
                                </span>

                                <span class='eco-feedback-pill <%# GetFeedbackPillClass(Eval("FeedbackType").ToString()) %>'>
                                    <%# Eval("FeedbackType") %>
                                </span>
                            </div>
                        </div>

                        <div class="eco-feedback-date"><%# Eval("SubmitDate", "{0:yyyy-MM-dd}") %></div>
                        <div class="eco-feedback-text"><%# Eval("FeedbackText") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblFeedbackMsg" runat="server" />
        </div>
    </div>
    </section>



    <!-- CHAT PANEL -->
    <section class="admin-panel" data-tab="chat">
        <div class="eco-cardbox">
            <div class="eco-cardbox-title">Escalated Chats</div>

            <asp:Repeater ID="rptChats" runat="server"
                OnItemCommand="rptChats_ItemCommand">


                <ItemTemplate>
                    <div class="eco-chat-item">
                        <div class="eco-avatar">
                            <%# Eval("CustomerName").ToString().Substring(0,1) %>
                        </div>

                        <div class="eco-chat-info">
                            <div class="eco-chat-name"><%# Eval("CustomerName") %></div>
                            <div class="eco-chat-sub"><%# Eval("IssueTitle") %></div>
                            <div class="eco-chat-time">
                                <%# Eval("CreatedAt", "{0:dd MMM yyyy HH:mm}") %>
                            </div>
                        </div>

                        <div class="eco-chat-actions">
                            <span class="eco-chat-pill <%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>

                            <asp:LinkButton runat="server"
                                CssClass="eco-btn eco-takeover"
                                CommandName="TAKEOVER"
                                CommandArgument='<%# Eval("EscalationId") %>'
                                Visible='<%# Eval("Status").ToString() != "Resolved" %>'>
                                Take Over
                            </asp:LinkButton>
                        </div>

                        <!-- Reply box (shown only when selected) -->
                        <asp:Panel runat="server"
                            CssClass="eco-reply-box"
                            Visible='<%# IsReplying(Eval("EscalationId")) %>'>

                            <asp:TextBox ID="txtReply" runat="server"
                                TextMode="MultiLine"
                                Rows="3"
                                CssClass="eco-reply-input"
                                placeholder="Type your reply to the customer..." />

                            <div class="eco-reply-actions">
                                <asp:Button runat="server"
                                    Text="Send Reply"
                                    CssClass="eco-btn eco-approve"
                                    CommandName="SEND"
                                    CommandArgument='<%# Eval("EscalationId") %>' />

                                <asp:Button runat="server"
                                    Text="Cancel"
                                    CssClass="eco-btn eco-reject"
                                    CommandName="CANCEL"
                                    CommandArgument='<%# Eval("EscalationId") %>' />
                            </div>
                        </asp:Panel>


                    </div>
                </ItemTemplate>

            </asp:Repeater>
            
            <asp:Label ID="lblChatMsg" runat="server" CssClass="eco-msg" />
            
        </div>
    </section>


</asp:Content>




