<%@ Page Title="Messages" Language="C#" MasterPageFile="~/SellPage.master"
    AutoEventWireup="true" CodeBehind="SellerMessages.aspx.cs"
    Inherits="FoodSaver.SellerMessages" %>

<asp:Content ID="Head1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Force chat area + text to behave like a messaging UI (prevents “centered” weirdness) */
        .eechat { text-align: left !important; }
        .eechat * { text-align: left; }

        .eechat-thread {
            height: 420px;
            overflow: auto;
            padding: 14px;
            background: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 12px;
        }

        .eechat-row {
            display: flex;
            width: 100%;
            margin: 10px 0;
        }

        .eechat-row.customer { justify-content: flex-start; }
        .eechat-row.seller { justify-content: flex-end; }

        /* Stack meta above bubble. Inline-flex keeps width “hugging” bubble */
        .eechat-stack {
            display: inline-flex;
            flex-direction: column;
            gap: 4px;
            max-width: 70%;
        }

        .eechat-row.seller .eechat-stack { align-items: flex-end; }
        .eechat-row.customer .eechat-stack { align-items: flex-start; }

        .eechat-meta {
            font-size: 12px;
            color: #6c757d;
            line-height: 1;
            padding: 0 6px;
        }

        .eechat-row.seller .eechat-meta { text-align: right !important; }
        .eechat-row.customer .eechat-meta { text-align: left !important; }

        /* The bubble itself MUST hug content */
        .eechat-bubble {
            display: inline-block !important;
            width: auto !important;
            max-width: 520px;          /* prevents massive bubbles */
            padding: 10px 14px;
            border-radius: 18px;
            line-height: 1.35;
            white-space: pre-wrap;
            word-break: break-word;
            box-shadow: 0 1px 2px rgba(0,0,0,.06);
            text-align: left !important;
        }

        /* Ensure the text inside never “centers” */
        .eechat-text {
            display: block;
            text-align: left !important;
        }

        .eechat-bubble.customer {
            background: #ffffff;
            border: 1px solid #e9ecef;
            border-top-left-radius: 8px;
        }

        .eechat-bubble.seller {
            background: #dbeafe;
            border: 1px solid #cfe0ff;
            border-top-right-radius: 8px;
        }

        /* Context menu */
        .ctx-menu{
            position: fixed;
            z-index: 9999;
            display: none;
            min-width: 160px;
            background: white;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            box-shadow: 0 6px 18px rgba(0,0,0,.15);
            padding: 6px;
        }
        .ctx-menu button{
            width: 100%;
            text-align: left;
            border: 0;
            background: transparent;
            padding: 8px 10px;
            border-radius: 8px;
        }
        .ctx-menu button:hover{ background: #f1f3f5; }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container eechat" style="max-width: 1100px;">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h2 class="mb-0">Messages</h2>
            <asp:Label ID="lblStatus" runat="server" CssClass="text-muted"></asp:Label>
        </div>

        <div class="row g-3">

            <!-- Left: Conversation list -->
            <div class="col-md-4">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <h5 class="card-title mb-3">Conversations</h5>

                        <asp:ListBox ID="lstConversations" runat="server"
                            CssClass="form-select" Rows="10"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="lstConversations_SelectedIndexChanged" />

                        <div class="text-muted small mt-2">
                            * Demo conversations are hardcoded and stored in Session.
                        </div>
                    </div>
                </div>
            </div>

            <!-- Right: Thread + Composer -->
            <div class="col-md-8">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <h5 class="card-title mb-0">
                                <asp:Label ID="lblConversationTitle" runat="server" Text="Select a conversation"></asp:Label>
                            </h5>
                            <asp:Button ID="btnCancelEdit" runat="server"
                                CssClass="btn btn-sm btn-outline-secondary"
                                Text="Cancel Edit" Visible="false"
                                OnClick="btnCancelEdit_Click" />
                        </div>

                        <hr />

                        <!-- Thread -->
                        <div class="eechat-thread" id="chatThread">
                            <asp:Repeater ID="rptMessages" runat="server">
                                <ItemTemplate>
                                    <div class='eechat-row <%# ((string)Eval("Sender") == "Seller") ? "seller" : "customer" %>'>
                                        <div class="eechat-stack">

                                            <div class="eechat-meta">
                                                <span class="fw-semibold"><%# Eval("Sender") %></span>
                                                <span class="ms-2"><%# FormatTs((DateTime)Eval("CreatedAt")) %></span>
                                                <%# (bool)Eval("IsEdited") ? "<span class='ms-2 badge text-bg-light'>edited</span>" : "" %>
                                            </div>

                                            <div class='eechat-bubble <%# ((string)Eval("Sender") == "Seller") ? "seller" : "customer" %>'
                                                data-msgid="<%# Eval("MessageId") %>"
                                                data-sender="<%# Eval("Sender") %>"
                                                oncontextmenu="return showCtxMenu(event, this);">

                                                <span class="eechat-text"><%# Server.HtmlEncode((string)Eval("Body")) %></span>
                                            </div>

                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Composer -->
                        <hr />

                        <asp:HiddenField ID="hfEditingMessageId" runat="server" />

                        <div class="mb-2">
                            <asp:Label ID="lblComposerMode" runat="server" CssClass="text-muted small" />
                        </div>

                        <div class="input-group">
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control"
                                Placeholder="Type a message..." />
                            <asp:Button ID="btnSendOrUpdate" runat="server" CssClass="btn btn-primary"
                                Text="Send" OnClick="btnSendOrUpdate_Click" />
                        </div>

                        <asp:Label ID="lblError" runat="server" CssClass="text-danger mt-2 d-block"></asp:Label>

                        <!-- Hidden controls for right-click edit -->
                        <asp:HiddenField ID="hfContextMsgId" runat="server" />
                        <asp:Button ID="btnEditHidden" runat="server" Style="display:none;" OnClick="btnEditHidden_Click" />

                        <!-- Right-click context menu -->
                        <div id="ctxMenu" class="ctx-menu">
                            <button type="button" id="ctxEditBtn">Edit message</button>
                        </div>

                    </div>
                </div>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ID="Scripts1" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        const ctxMenu = () => document.getElementById("ctxMenu");

        function hideCtxMenu() {
            const m = ctxMenu();
            m.style.display = "none";
            m.dataset.msgid = "";
        }

        function showCtxMenu(e, el) {
            e.preventDefault();

            // Only allow edit for Seller messages
            const sender = el.dataset.sender;
            if (sender !== "Seller") {
                hideCtxMenu();
                return false;
            }

            const menu = ctxMenu();
            menu.style.display = "block";
            menu.style.left = e.clientX + "px";
            menu.style.top = e.clientY + "px";
            menu.dataset.msgid = el.dataset.msgid;

            return false;
        }

        document.addEventListener("click", function () { hideCtxMenu(); });
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") hideCtxMenu();
        });

        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById("ctxEditBtn").addEventListener("click", function () {
                const msgId = ctxMenu().dataset.msgid;
                if (!msgId) return;

                document.getElementById("<%= hfContextMsgId.ClientID %>").value = msgId;
                __doPostBack("<%= btnEditHidden.UniqueID %>", "");
                hideCtxMenu();
            });
        });
    </script>
</asp:Content>
