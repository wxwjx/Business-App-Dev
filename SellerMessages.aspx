<%@ Page Title="Messages" Language="C#" MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true" CodeBehind="SellerMessages.aspx.cs" Inherits="Business_App_Dev.SellerMessages" %>


<asp:Content ID="HeadCss" ContentPlaceHolderID="head" runat="server">
    <style>
        .ee-msg-wrap{display:flex;gap:16px;align-items:stretch;margin:16px 0;}
        .ee-panel{background:#fff;border:1px solid #e6e8ee;border-radius:14px;box-shadow:0 6px 20px rgba(15,23,42,.06);overflow:hidden;}
        .ee-panel-h{padding:12px 14px;border-bottom:1px solid #eef0f6;display:flex;align-items:center;gap:10px;}
        .ee-panel-title{font-weight:800;font-size:16px;}
        .ee-subtle{opacity:.7;font-size:12px;}

        .ee-inbox{width:340px;min-width:300px;}
        .ee-chat{flex:1;display:flex;flex-direction:column;min-height:560px;}

        .ee-inbox-list{max-height:560px;overflow:auto;}
        .ee-inbox-item{display:flex;justify-content:space-between;gap:12px;padding:12px 14px;border-bottom:1px solid #f1f3f8;text-decoration:none;color:inherit;}
        .ee-inbox-item:hover{background:#fafbff;}
        .ee-inbox-name{font-weight:800;}
        .ee-inbox-preview{font-size:12px;opacity:.75;}

        .ee-pill{font-size:11px;padding:2px 8px;border-radius:999px;border:1px solid #e6e8ee;background:#f7f8fc;}
        .ee-pill.unread{background:#eef6ff;border-color:#cfe4ff;}

        .ee-msg-list{flex:1;overflow:auto;padding:14px;background:linear-gradient(#fff,#fbfcff);}
        .ee-row{display:flex;margin:10px 0;}
        .ee-row.me{justify-content:flex-end;}
        .ee-bubble{max-width:68%;padding:10px 12px;border-radius:14px;border:1px solid #e6e8ee;background:#fff;}
        .ee-row.me .ee-bubble{background:#ecfdf3;border-color:#bfead0;}
        .ee-meta{margin-top:6px;display:flex;align-items:center;gap:10px;font-size:12px;opacity:.7;}
        .ee-meta .spacer{margin-left:auto;}

        .ee-compose{padding:12px 14px;border-top:1px solid #eef0f6;display:flex;gap:10px;}
        .ee-input{flex:1;border:1px solid #e6e8ee;border-radius:10px;padding:10px 12px;}

        .ee-btn{border:0;border-radius:10px;padding:10px 14px;font-weight:800;cursor:pointer;}
        .ee-btn-primary{background:#0ea5e9;color:#fff;}
        .ee-btn-primary:hover{filter:brightness(.95);}
        .ee-action{border:0;background:transparent;cursor:pointer;font-weight:800;font-size:12px;opacity:.75;}
        .ee-action:hover{opacity:1;text-decoration:underline;}

        .ee-error{color:#b91c1c;font-weight:700;margin:8px 0;}
        /* --- Right-click menu for my messages --- */
        .ee-bubble {
            position: relative;
        }
        /* needed for absolute menu positioning */

        .ee-actions-menu {
            position: absolute;
            top: 100%;
            right: 0;
            margin-top: 6px;
            background: #fff;
            border: 1px solid #e6e8ee;
            border-radius: 12px;
            padding: 6px;
            box-shadow: 0 10px 25px rgba(15,23,42,.12);
            z-index: 9999;
            min-width: 120px;
            display: none; /* hidden by default */
        }

            .ee-actions-menu .ee-action {
                display: block;
                width: 100%;
                text-align: left;
                padding: 8px 10px;
                border-radius: 10px;
                opacity: .9;
                text-decoration: none;
            }

                .ee-actions-menu .ee-action:hover {
                    background: #f3f5fb;
                    opacity: 1;
                    text-decoration: none;
                }

    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="padding:16px 0;">
        <h2 style="margin:0 0 12px 0;">Messages</h2>

        <asp:Label ID="lblError" runat="server" CssClass="ee-error" />
        <asp:HiddenField ID="hfConversationID" runat="server" />
        <asp:HiddenField ID="hfEditingMessageID" runat="server" />

        <div class="ee-msg-wrap">

            <!-- Inbox -->
            <div class="ee-panel ee-inbox">
                <div class="ee-panel-h">
                    <div class="ee-panel-title">Inbox</div>
                    <div class="ee-subtle">Customers</div>
                </div>

                <div class="ee-inbox-list">
                    <asp:Repeater ID="rptInbox" runat="server" OnItemCommand="rptInbox_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkOpen" CssClass="ee-inbox-item"
                                CommandName="Open"
                                CommandArgument='<%# Eval("ConversationID") %>'
                                CausesValidation="false"
                                UseSubmitBehavior="false">

                                <div>
                                    <div class="ee-inbox-name"><%# Eval("FullName") %></div>
                                    <div class="ee-inbox-preview"><%# Eval("LastPreview") %></div>
                                    <div class="ee-subtle"><%# Eval("Email") %></div>
                                </div>
                                <div style="text-align:right;">
                                    <div class="ee-subtle"><%# Eval("LastMessageAt", "{0:dd MMM, HH:mm}") %></div>
                                    <asp:Panel runat="server" Visible='<%# (int)Eval("UnreadCount") > 0 %>'>
                                        <span class="ee-pill unread"><%# Eval("UnreadCount") %> new</span>
                                    </asp:Panel>
                                </div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <!-- Chat -->
            <div class="ee-panel ee-chat">
                <div class="ee-panel-h">
                    <div class="ee-panel-title">
                        <asp:Label ID="lblChatHeader" runat="server" Text="Select a conversation" />
                    </div>
                    <div class="ee-subtle">
                        <asp:Label ID="lblChatSub" runat="server" />
                    </div>
                </div>

                <div class="ee-msg-list">
                    <asp:Repeater ID="rptMessages" runat="server"
                        OnItemCommand="rptMessages_ItemCommand"
                        OnItemDataBound="rptMessages_ItemDataBound">
                        <ItemTemplate>
                            <div class='ee-row <%# (bool)Eval("IsMe") ? "me" : "" %>'>
                                <div class='ee-bubble <%# (bool)Eval("IsMe") ? "is-me" : "" %>'>

                                    <!-- VIEW -->
                                    <asp:Panel ID="pnlView" runat="server">
                                        <div><%# Server.HtmlEncode(Eval("MessageText").ToString()) %></div>

                                        <div class="ee-meta">
                                            <span><%# Eval("SentAt", "{0:dd MMM, HH:mm}") %></span>
                                            <span class="spacer"></span>

                                            <asp:Panel ID="pnlActions" runat="server"
                                                CssClass="ee-actions-menu"
                                                Visible='<%# (bool)Eval("IsMe") %>'>
                                                <asp:LinkButton runat="server" CssClass="ee-action"
                                                    CommandName="Edit" CommandArgument='<%# Eval("MessageID") %>'>Edit</asp:LinkButton>

                                                <asp:LinkButton runat="server" CssClass="ee-action"
                                                    CommandName="Delete" CommandArgument='<%# Eval("MessageID") %>'
                                                    OnClientClick="return confirm('Delete this message?');">Delete</asp:LinkButton>
                                            </asp:Panel>

                                        </div>
                                    </asp:Panel>

                                    <!-- EDIT -->
                                    <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                                        <asp:TextBox ID="txtEdit" runat="server" TextMode="MultiLine" Rows="3"
                                            CssClass="ee-input" Text='<%# Eval("MessageText") %>' />
                                        <div class="ee-meta">
                                            <asp:LinkButton runat="server" CssClass="ee-action"
                                                CommandName="Save" CommandArgument='<%# Eval("MessageID") %>'>Save</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="ee-action"
                                                CommandName="Cancel" CommandArgument='<%# Eval("MessageID") %>'>Cancel</asp:LinkButton>
                                        </div>
                                    </asp:Panel>

                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Panel ID="pnlCompose" runat="server" CssClass="ee-compose" DefaultButton="btnSend">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="ee-input" placeholder="Type a message..." />
                    <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="ee-btn ee-btn-primary"
                        OnClick="btnSend_Click" CausesValidation="false" />
                </asp:Panel>

            </div>

        </div>
    </div>
    <script>
        document.addEventListener("DOMContentLoaded", function () {

            function closeAllMenus() {
                document.querySelectorAll(".ee-actions-menu").forEach(m => m.style.display = "none");
            }

            // Right-click on MY bubble opens menu
            document.addEventListener("contextmenu", function (e) {
                const bubble = e.target.closest(".ee-bubble.is-me");
                if (!bubble) return; // allow normal right click elsewhere

                const menu = bubble.querySelector(".ee-actions-menu");
                if (!menu) return;

                e.preventDefault();
                closeAllMenus();
                menu.style.display = "block";
            });

            // Click anywhere closes menus
            document.addEventListener("click", function (e) {
                // If clicking inside the menu, let the LinkButton click proceed
                if (e.target.closest(".ee-actions-menu")) return;
                closeAllMenus();
            });

            // ESC closes
            document.addEventListener("keydown", function (e) {
                if (e.key === "Escape") closeAllMenus();
            });

        });
    </script>
</asp:Content>
