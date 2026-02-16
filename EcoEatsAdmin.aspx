<%@ Page Title="EcoEats Admin" Language="C#" MasterPageFile="~/Admin.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="EcoEatsAdmin.aspx.cs" Inherits="Business_App_Dev.EcoEatsAdmin" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsAdmin.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">


    <div id="ecoOverlay" class="eco-overlay" style="display:none;">
        <div class="eco-overlay-card">
            <div class="eco-overlay-title">Seller approval processing…</div>
            <div class="eco-overlay-sub">Please wait.</div>
        </div>
    </div>

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
                                        CssClass="eco-btn eco-approve eco-approve-once"
                                        CommandName="APPROVE"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return lockApprove(this, 'Seller approval processing...');">
                                        ✓ Approve
                                    </asp:LinkButton>




                                    <asp:LinkButton ID="btnReject" runat="server"
                                        CssClass="eco-btn eco-reject"
                                        CommandName="REJECT"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return openRejectModal(this);">
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
    <!-- FEEDBACK PANEL -->
    <section class="admin-panel" data-tab="feedback">
        <div class="eco-cardbox">

            <div class="eco-feedback-header">
                <div class="eco-cardbox-title">Customer Feedback</div>

                <div class="eco-feedback-actions">
                    <!-- Rating Filter -->
                    <div class="eco-rating-filter">
                        <span class="eco-filter-label">Filter:</span>

                        <asp:CheckBoxList ID="cblRatings" runat="server"
                            CssClass="eco-rating-checks"
                            RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="1">1★</asp:ListItem>
                            <asp:ListItem Value="2">2★</asp:ListItem>
                            <asp:ListItem Value="3">3★</asp:ListItem>
                            <asp:ListItem Value="4">4★</asp:ListItem>
                            <asp:ListItem Value="5">5★</asp:ListItem>
                        </asp:CheckBoxList>

                        <asp:Button ID="btnApplyRatingFilter" runat="server"
                            CssClass="eco-filter-btn"
                            Text="Apply"
                            OnClick="btnApplyRatingFilter_Click" />

                        <asp:LinkButton ID="btnClearRatingFilter" runat="server"
                            CssClass="eco-filter-clear"
                            Text="Clear"
                            OnClick="btnClearRatingFilter_Click"
                            CausesValidation="false" />
                    </div>

                        <asp:LinkButton ID="btnExportFeedback" runat="server"
                            CssClass="eco-export-btn"
                            OnClick="btnExportFeedback_Click"
                            CausesValidation="false">
                            <span class="eco-dl">↓</span> Export Report
                        </asp:LinkButton>
                    </div>
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

                                    <asp:PlaceHolder runat="server" Visible='<%# HasTag(Eval("Tag")) %>'>
                                        <span class='eco-feedback-pill <%# GetRatingPillClass(Convert.ToInt32(Eval("Rating"))) %>'>
                                            <%# Eval("Tag") %>
                                        </span>
                                    </asp:PlaceHolder>
                                </div>
                            </div>

                            <div class="eco-feedback-date"><%# Eval("CreatedAt", "{0:yyyy-MM-dd}") %></div>
                            <div class="eco-feedback-text"><%# Eval("Comments") %></div>
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
    <div id="ecoRejectModal" class="ee-modal-overlay" style="display:none;">
        <div class="ee-modal">
            <div class="ee-modal-head">
                <div class="ee-modal-title">Reject Seller Application</div>
                <button type="button" class="ee-modal-x" onclick="closeRejectModal()">✕</button>
            </div>

            <div class="ee-modal-body">
                <p class="ee-modal-text">
                    Are you sure you want to reject this seller application?
                </p>
            </div>

            <div class="ee-modal-actions">
                <button type="button" class="ee-btn-secondary" onclick="closeRejectModal()">Cancel</button>
                <button type="button" class="ee-btn-primary" onclick="confirmReject()">Reject</button>
            </div>
        </div>
    </div>


<script>
    function lockApprove(btn, message) {
        if (btn.getAttribute("data-locked") === "1") return false;
        btn.setAttribute("data-locked", "1");

        btn.classList.add("eco-btn-disabled");
        btn.style.pointerEvents = "none";
        btn.innerText = "Processing...";

        var overlay = document.getElementById("ecoOverlay");
        if (overlay) {
            var t = overlay.querySelector(".eco-overlay-title");
            if (t) t.innerText = message || "Seller approval processing…";
            overlay.style.display = "flex";
        }

        var href = btn.getAttribute("href") || "";
        var match = href.match(/__doPostBack\('([^']+)','([^']*)'\)/);

        if (match && typeof __doPostBack === "function") {
            requestAnimationFrame(function () {
                setTimeout(function () {
                    __doPostBack(match[1], match[2]);
                }, 300);
            });
            return false;
        }
        return true;
    }
    var rejectTarget = null;
    var rejectArgument = null;

    function openRejectModal(btn) {
        // extract __doPostBack args
        var href = btn.getAttribute("href") || "";
        var match = href.match(/__doPostBack\('([^']+)','([^']*)'\)/);

        if (match) {
            rejectTarget = match[1];
            rejectArgument = match[2];
        }

        document.getElementById("ecoRejectModal").style.display = "flex";
        return false; // stop postback for now
    }

    function closeRejectModal() {
        document.getElementById("ecoRejectModal").style.display = "none";
    }

    function confirmReject() {
        closeRejectModal();

        if (rejectTarget && typeof __doPostBack === "function") {
            __doPostBack(rejectTarget, rejectArgument);
        }
    }

    // click outside to close
    document.addEventListener("click", function (e) {
        if (e.target && e.target.id === "ecoRejectModal") closeRejectModal();
    });

    // ESC to close
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") closeRejectModal();
    });
</script>



</asp:Content>




