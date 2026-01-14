<%@ Page Title="Messages" Language="C#" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="FoodSaver.Messages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mb-3">Messages</h2>

        <asp:Label ID="lblInfo" runat="server" CssClass="text-muted" />
        <asp:Label ID="lblError" runat="server" CssClass="text-danger" />

        <asp:Panel ID="pnlSeller" runat="server" Visible="false">
            <div class="row g-3">
                <!-- LEFT: Inbox -->
                <div class="col-lg-4">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title mb-3">Inbox</h5>

                            <asp:GridView ID="gvInbox" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-sm"
                                OnRowCommand="gvInbox_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="UnreadCount" HeaderText="Unread" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" Text="Open"
                                                CommandName="OpenChat"
                                                CommandArgument='<%# Eval("ConversationID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <hr />

                            <h6 class="mb-2">Start new chat (demo)</h6>
                            <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-select mb-2" />
                            <asp:Button ID="btnStartChat" runat="server" Text="Start / Open Chat"
                                CssClass="btn btn-outline-primary btn-sm" OnClick="btnStartChat_Click" />
                        </div>
                    </div>
                </div>

                <!-- RIGHT: Chat -->
                <div class="col-lg-8">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title mb-3">
                                Chat
                                <asp:Label ID="lblChatWith" runat="server" CssClass="text-muted fs-6" />
                            </h5>

                            <asp:Panel ID="pnlChat" runat="server" Visible="false">

                                <!-- Chat messages area -->
                                <div id="chatWindow" class="chat-window mb-3">
                                    <asp:Repeater ID="rptMessages" runat="server">
                                        <ItemTemplate>
                                            <div class='msg-row <%# (Convert.ToInt32(Eval("IsMine")) == 1 ? "mine" : "theirs") %>'>
                                                <div class='msg-bubble'
                                                    oncontextmenu='return showMsgMenu(event, "<%# Eval("MessageID") %>", <%# Eval("IsMine") %>, "<%# HttpUtility.JavaScriptStringEncode(Eval("MessageText").ToString()) %>");'>

                                                    <div class="msg-text"><%# Server.HtmlEncode(Eval("MessageText").ToString()) %></div>

                                                    <div class="msg-meta">
                                                        <%# Convert.ToDateTime(Eval("SentAt")).ToString("HH:mm") %>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <!-- Send box -->
                                <div class="chat-input">
                                    <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="2"
                                        CssClass="form-control" Placeholder="Type a message..." />
                                    <div class="d-flex justify-content-end mt-2">
                                        <asp:Button ID="btnSend" runat="server" Text="Send"
                                            CssClass="btn btn-success" OnClick="btnSend_Click" />
                                    </div>
                                </div>

                                <!-- Hidden fields + hidden buttons for context actions -->
                                <asp:HiddenField ID="hfSelectedMessageId" runat="server" />
                                <asp:HiddenField ID="hfEditText" runat="server" />

                                <asp:LinkButton ID="btnCtxDelete" runat="server" OnClick="btnCtxDelete_Click" Style="display: none" />
                                <asp:LinkButton ID="btnSaveEdit" runat="server" OnClick="btnSaveEdit_Click" Style="display: none" />

                            </asp:Panel>

                            <!-- Right-click context menu -->
                            <div id="msgMenu" class="msg-menu" style="display: none;">
                                <button type="button" class="dropdown-item" onclick="openEditModal()">Edit</button>
                                <button type="button" class="dropdown-item text-danger" onclick="deleteMessage()">Delete</button>
                            </div>

                            <!-- Bootstrap Edit Modal -->
                            <div class="modal fade" id="editMsgModal" tabindex="-1" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title">Edit message</h5>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">
                                            <textarea id="editMsgBox" class="form-control" rows="3"></textarea>
                                            <div class="small text-muted mt-2">Right-click only works on desktop.</div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancel</button>
                                            <button type="button" class="btn btn-primary" onclick="saveEdit()">Save</button>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <asp:Panel ID="pnlNoChat" runat="server" Visible="true">
                                <p class="text-muted mb-0">Select a conversation from the inbox to view messages.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCustomerPlaceholder" runat="server" Visible="false">
            <div class="alert alert-info">
                Customer messaging is not implemented yet (no customer login). This page will support both sellers and customers later.
            </div>
        </asp:Panel>
    </div>
    <script>
    function showMsgMenu(e, messageId, isMine, messageText) {
        e.preventDefault();

        // Only allow context menu for your own messages
        if (parseInt(isMine) !== 1) return false;

        document.getElementById('<%= hfSelectedMessageId.ClientID %>').value = messageId;
        document.getElementById('<%= hfEditText.ClientID %>').value = messageText;

        const menu = document.getElementById('msgMenu');
        menu.style.display = 'block';
        menu.style.left = e.clientX + 'px';
        menu.style.top = e.clientY + 'px';

        return false;
    }

    function hideMsgMenu() {
        const menu = document.getElementById('msgMenu');
        if (menu) menu.style.display = 'none';
    }

    document.addEventListener('click', hideMsgMenu);
    document.addEventListener('scroll', hideMsgMenu, true);

    function openEditModal() {
        hideMsgMenu();

        const text = document.getElementById('<%= hfEditText.ClientID %>').value || "";
        document.getElementById('editMsgBox').value = text;

        const modal = new bootstrap.Modal(document.getElementById('editMsgModal'));
        modal.show();
    }

    function saveEdit() {
        const newText = document.getElementById('editMsgBox').value || "";
        document.getElementById('<%= hfEditText.ClientID %>').value = newText;

        document.getElementById('<%= btnSaveEdit.ClientID %>').click();
    }

    function deleteMessage() {
        hideMsgMenu();
        if (!confirm("Delete this message?")) return;
        document.getElementById('<%= btnCtxDelete.ClientID %>').click();
    }

    // Auto-scroll to bottom on load (after render)
    window.addEventListener('load', function () {
        const chat = document.getElementById('chatWindow');
        if (chat) chat.scrollTop = chat.scrollHeight;
    });
    </script>

</asp:Content>

