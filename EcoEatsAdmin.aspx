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
    <!-- CHAT PANEL -->
    <section class="admin-panel" data-tab="chat">
        <div class="eco-cardbox">

            <!-- Header + Filter -->
            <div class="eco-chat-header">
                <div class="eco-cardbox-title">Escalated Chats</div>

                <div class="eco-chat-filter">
                    <span class="eco-filter-label">Show:</span>
                    <asp:DropDownList ID="ddlChatStatus" runat="server"
                        CssClass="eco-filter-ddl"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlChatStatus_SelectedIndexChanged">
                        <asp:ListItem Text="Open" Value="OPEN" Selected="True" />
                        <asp:ListItem Text="Resolved" Value="RESOLVED" />
                        <asp:ListItem Text="All" Value="ALL" />
                    </asp:DropDownList>
                </div>
            </div>

            <asp:Repeater ID="rptChats" runat="server" OnItemCommand="rptChats_ItemCommand">
                <ItemTemplate>
                    <div class="eco-chat-item">

                        <div class="eco-avatar">
                            <%# GetInitial(Eval("CustomerName")) %>
                        </div>

                        <div class="eco-chat-info">
                            <div class="eco-chat-name"><%# Eval("CustomerName") %></div>
                            <div class="eco-chat-sub"><%# Eval("IssueTitle") %></div>
                            <div class="eco-chat-time">
                                <%# Eval("CreatedAt", "{0:dd MMM yyyy HH:mm}") %>
                            </div>
                        </div>

                        <div class="eco-chat-actions">
                            <span class="eco-chat-pill <%# (Eval("Status") ?? "").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>

                            <asp:LinkButton ID="btnTakeOver" runat="server"
                                CssClass="eco-btn eco-takeover"
                                CommandName="TAKEOVER"
                                CommandArgument='<%# Eval("LogId") %>'
                                Visible='<%# (Eval("Status") ?? "").ToString().ToUpper() != "RESOLVED" %>'>
                                Take Over
                            </asp:LinkButton>
                        </div>

                        <!-- Reply box (shown only when selected) -->
                        <asp:Panel runat="server"
                            CssClass="eco-reply-box"
                            Visible='<%# IsReplying(Eval("LogId")) %>'>

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
                                    CommandArgument='<%# Eval("LogId") %>' />

                                <asp:Button runat="server"
                                    Text="Cancel"
                                    CssClass="eco-btn eco-reject"
                                    CommandName="CANCEL"
                                    CommandArgument='<%# Eval("LogId") %>' />
                            </div>
                        </asp:Panel>

                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblChatMsg" runat="server" CssClass="eco-msg" />

        </div>
    </section>

    <section class="admin-panel" data-tab="analytics">
          <div class="eco-cardbox">
            <div class="eco-cardbox-title">Growth Analytics</div>

            <div class="ana-toolbar">
              <label class="ana-label">Range</label>
              <select id="ddlRange" class="ana-select" onchange="loadGrowthCharts()">
                <option value="3">Last 3 months</option>
                <option value="6" selected>Last 6 months</option>
                <option value="12">Last 12 months</option>
              </select>
            </div>

            <div class="ana-grid">

              <!-- USERS CARD -->
                <div class="ana-chart-card">
                  <div class="ana-chart-head">
                    <div class="ana-chart-title">User Analytics</div>

                    <select id="ddlUserView" class="ana-select ana-select--pill" onchange="loadGrowthCharts()">
                      <option value="growth">Growth</option>
                      <option value="premium" selected>Premium Split</option>
                    </select>
                  </div>

                  <div class="ana-chart-box">
                    <canvas id="usersChart"></canvas>
                    <canvas id="usersPie" style="display:none;"></canvas>
                  </div>
                </div>

                <!-- SELLERS CARD -->
                <div class="ana-chart-card">
                  <div class="ana-chart-head">
                    <div class="ana-chart-title">Seller Analytics</div>

                    <select id="ddlSellerView" class="ana-select ana-select--pill" onchange="loadGrowthCharts()">
                      <option value="approved" selected>Approved</option>
                      <option value="rejected">Rejected</option>
                    </select>
                  </div>

                  <div class="ana-chart-box">
                    <canvas id="sellersChart"></canvas>
                  </div>
                </div>

            </div>
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
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

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

    let usersChart = null;
    let sellersChart = null;
    let usersPie = null;

    // ---------- helpers ----------
    function setHidden(el, hidden) {
        if (!el) return;
        el.classList.toggle("ana-hidden", !!hidden);
    }

    function resetCanvas(canvasId) {
        const old = document.getElementById(canvasId);
        if (!old) return null;

        const parent = old.parentNode;
        const fresh = old.cloneNode(true);   // keep same id
        parent.replaceChild(fresh, old);
        return fresh;
    }

    function renderDualLineChart(canvasId, labels, seriesA, seriesB, oldChart, labelA, labelB) {
        if (oldChart) oldChart.destroy();

        const canvas = resetCanvas(canvasId);
        if (!canvas) return null;

        return new Chart(canvas.getContext("2d"), {
            type: "line",
            data: {
                labels,
                datasets: [
                    { label: labelA, data: seriesA, tension: 0.35, pointRadius: 3, borderWidth: 2, fill: false },
                    { label: labelB, data: seriesB, tension: 0.35, pointRadius: 3, borderWidth: 3, fill: true }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                plugins: { legend: { display: true } },
                scales: { y: { beginAtZero: true } }
            }
        });
    }

    function renderPieChart(canvasId, labels, values, oldChart) {
        if (oldChart) oldChart.destroy();

        const canvas = resetCanvas(canvasId);
        if (!canvas) return null;

        return new Chart(canvas.getContext("2d"), {
            type: "pie",
            data: { labels, datasets: [{ data: values }] },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                plugins: { legend: { display: true } }
            }
        });
    }

    // ---------- main loader ----------
    async function loadGrowthCharts() {
        try {
            const months = parseInt(document.getElementById("ddlRange")?.value || "6", 10);
            const userView = document.getElementById("ddlUserView")?.value || "growth";
            const sellerView = document.getElementById("ddlSellerView")?.value || "approved";

            const res = await fetch('<%= ResolveUrl("~/Services/AdminAnalytics.asmx/GetGrowthData") %>', {
                method: "POST",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({ months })
            });

            const json = await res.json();
            const data = json.d || {};

            const usersCanvas = document.getElementById("usersChart");
            const pieCanvas = document.getElementById("usersPie");

            // ✅ IMPORTANT: don't use display none/block (causes shifting)
            // We use .ana-hidden which keeps layout stable.

            // ---------- USERS ----------
            if (userView === "premium") {
                setHidden(usersCanvas, true);
                setHidden(pieCanvas, false);
                showCanvas(pieCanvas, usersCanvas);

                if (usersChart) { usersChart.destroy(); usersChart = null; }

                usersPie = renderPieChart(
                    "usersPie",
                    ["Premium", "Non-Premium"],
                    [data.premiumUsers || 0, data.nonPremiumUsers || 0],
                    usersPie
                );
            } else {
                setHidden(usersCanvas, false);
                setHidden(pieCanvas, true);
                showCanvas(usersCanvas, pieCanvas);

                if (usersPie) { usersPie.destroy(); usersPie = null; }

                usersChart = renderDualLineChart(
                    "usersChart",
                    data.labels || [],
                    data.monthlyUsers || [],
                    data.cumulativeUsers || [],
                    usersChart,
                    "New Users",
                    "Total Users"
                );
            }

            // ---------- SELLERS ----------
            if (sellerView === "rejected") {
                sellersChart = renderDualLineChart(
                    "sellersChart",
                    data.labels || [],
                    data.monthlyRejectedSellers || [],
                    data.cumulativeRejectedSellers || [],
                    sellersChart,
                    "Rejected (Monthly)",
                    "Rejected (Total)"
                );
            } else {
                sellersChart = renderDualLineChart(
                    "sellersChart",
                    data.labels || [],
                    data.monthlyApprovedSellers || [],
                    data.cumulativeApprovedSellers || [],
                    sellersChart,
                    "Approved (Monthly)",
                    "Approved (Total)"
                );
            }

            // Give layout a moment, then force resize (no animation)
            setTimeout(() => {
                if (usersChart) usersChart.resize();
                if (usersPie) usersPie.resize();
                if (sellersChart) sellersChart.resize();
            }, 120);

        } catch (err) {
            console.error("loadGrowthCharts error:", err);
        }
    }

    function showCanvas(front, back) {
        if (front) front.style.zIndex = "2";
        if (back) back.style.zIndex = "1";
    }
</script>



</asp:Content>




