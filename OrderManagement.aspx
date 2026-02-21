<%--<%@ Page Title="Order Management"
    Language="C#"
    MasterPageFile="~/SellPage.master"
    AutoEventWireup="true"
    CodeBehind="OrdersManagement.aspx.cs"
    Inherits="Business_App_Dev.OrdersManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .om-wrap { max-width: 1100px; margin: 24px auto; padding: 0 10px; }
        .om-head { display:flex; justify-content:space-between; align-items:flex-end; gap:12px; flex-wrap:wrap; margin-bottom:14px; }
        .om-head h2 { margin:0; }
        .om-sub { margin:6px 0 0; color:#6b7280; font-size:14px; }

        .om-tabs { display:flex; gap:10px; flex-wrap:wrap; margin: 10px 0 16px; }
        .om-tab { border:1px solid #e5e7eb; background:#fff; padding:8px 12px; border-radius:999px; font-weight:800; cursor:pointer; }
        .om-tab.active { box-shadow:0 8px 18px rgba(0,0,0,.08); border-color:#d1d5db; }

        .card { background:#fff; border-radius:16px; padding:16px; box-shadow:0 6px 18px rgba(0,0,0,.06); }

        .tbl { width:100%; border-collapse:collapse; }
        .tbl th, .tbl td { padding:10px 8px; border-bottom:1px solid #eee; text-align:left; font-size:14px; }
        .tbl th { color:#6b7280; font-size:12px; text-transform:uppercase; letter-spacing:.04em; }
        .muted { color:#6b7280; font-size:13px; }

        .badge { display:inline-flex; padding:6px 10px; border-radius:999px; font-weight:900; font-size:12px; background:rgba(0,0,0,0.05); }
        .b-pending { background: rgba(255,193,7,.18); }
        .b-accepted { background: rgba(46,204,113,.18); }
        .b-prep { background: rgba(52,152,219,.18); }
        .b-completed { background: rgba(155,89,182,.18); }
        .b-rejected { background: rgba(231,76,60,.18); }

        .btn { border:none; border-radius:10px; padding:8px 12px; font-weight:900; cursor:pointer; }
        .btn-accept { background:#2ecc71; color:#fff; }
        .btn-reject { background:#e74c3c; color:#fff; }
        .btn-prep { background:#3498db; color:#fff; }
        .btn-done { background:#8e44ad; color:#fff; }
        .btn-ghost { background:#ecf0f1; color:#1f2d3d; }

        /* Modal */
        .modal-overlay{
            position:fixed; inset:0; background:rgba(0,0,0,.45);
            display:none; align-items:center; justify-content:center; z-index:9999;
        }
        .modal{
            width:min(520px, 92vw);
            background:#fff; border-radius:16px; overflow:hidden;
            box-shadow:0 22px 70px rgba(0,0,0,.25);
        }
        .modal-head{ display:flex; justify-content:space-between; align-items:center; padding:14px 16px; border-bottom:1px solid #eee; }
        .modal-title{ font-weight:900; color:#111827; }
        .modal-x{ background:transparent; border:none; font-size:18px; cursor:pointer; }
        .modal-body{ padding:16px; }
        .modal-actions{ display:flex; justify-content:flex-end; gap:10px; padding:14px 16px; border-top:1px solid #eee; }

        .input, .textarea{
            width:100%; padding:10px 12px; border-radius:10px; border:1px solid #ddd; font-size:14px;
        }
        .textarea{ resize:vertical; }
    </style>

    <div class="om-wrap">

        <div class="om-head">
            <div>
                <h2>Order Management</h2>
                <div class="om-sub">Accept, reject (with reason), move orders to preparing, and mark completed.</div>
            </div>

            <div class="muted">
                Showing: <asp:Label ID="lblTab" runat="server" Text="Pending" />
            </div>
        </div>

        <!-- Tabs -->
        <div class="om-tabs">
            <asp:Button ID="btnTabPending" runat="server" Text="Pending" CssClass="om-tab active" CommandArgument="Pending" OnClick="Tab_Click" />
            <asp:Button ID="btnTabAccepted" runat="server" Text="Accepted" CssClass="om-tab" CommandArgument="Accepted" OnClick="Tab_Click" />
            <asp:Button ID="btnTabPreparing" runat="server" Text="Preparing" CssClass="om-tab" CommandArgument="Preparing" OnClick="Tab_Click" />
            <asp:Button ID="btnTabCompleted" runat="server" Text="Completed" CssClass="om-tab" CommandArgument="Completed" OnClick="Tab_Click" />
            <asp:Button ID="btnTabRejected" runat="server" Text="Rejected" CssClass="om-tab" CommandArgument="Rejected" OnClick="Tab_Click" />
        </div>

        <div class="card">
            <asp:GridView ID="gvOrders" runat="server"
                AutoGenerateColumns="false"
                CssClass="tbl"
                BorderStyle="None"
                GridLines="None"
                OnRowCommand="gvOrders_RowCommand">

                <Columns>
                    <asp:BoundField DataField="OrderID" HeaderText="Order #" />
                    <asp:BoundField DataField="CreatedAt" HeaderText="Time" DataFormatString="{0:dd MMM yyyy, HH:mm}" />
                    <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C}" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge <%# Eval("StatusCss") %>'><%# Eval("Status") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Reject Reason">
                        <ItemTemplate>
                            <span class="muted"><%# Eval("RejectReason") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <!-- Accept -->
                            <asp:LinkButton ID="lnkAccept" runat="server"
                                CommandName="ACCEPT"
                                CommandArgument='<%# Eval("OrderID") %>'
                                CssClass="btn btn-accept"
                                Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                Accept
                            </asp:LinkButton>

                            <!-- Reject -->
                            <asp:LinkButton ID="lnkReject" runat="server"
                                CommandName="OPEN_REJECT"
                                CommandArgument='<%# Eval("OrderID") %>'
                                CssClass="btn btn-reject"
                                Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                Reject
                            </asp:LinkButton>

                            <!-- Move to Preparing -->
                            <asp:LinkButton ID="lnkPrep" runat="server"
                                CommandName="PREPARE"
                                CommandArgument='<%# Eval("OrderID") %>'
                                CssClass="btn btn-prep"
                                Visible='<%# Eval("Status").ToString() == "Accepted" %>'>
                                Preparing
                            </asp:LinkButton>

                            <!-- Mark completed -->
                            <asp:LinkButton ID="lnkDone" runat="server"
                                CommandName="COMPLETE"
                                CommandArgument='<%# Eval("OrderID") %>'
                                CssClass="btn btn-done"
                                Visible='<%# Eval("Status").ToString() == "Preparing" %>'>
                                Completed
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMsg" runat="server" CssClass="muted" />
        </div>

        <!-- Reject Modal -->
        <asp:HiddenField ID="hfRejectOrderId" runat="server" ClientIDMode="Static" />

        <div id="rejectModal" class="modal-overlay">
            <div class="modal">
                <div class="modal-head">
                    <div class="modal-title">Reject Order</div>
                    <button type="button" class="modal-x" onclick="closeReject()">✕</button>
                </div>

                <div class="modal-body">
                    <div class="muted" style="margin-bottom:8px;">Please provide a reason (required).</div>
                    <asp:TextBox ID="tbRejectReason" runat="server" CssClass="textarea" TextMode="MultiLine" Rows="4"
                        placeholder="E.g. Out of stock / Shop closing / Unable to fulfil on time" />
                </div>

                <div class="modal-actions">
                    <button type="button" class="btn btn-ghost" onclick="closeReject()">Cancel</button>
                    <asp:Button ID="btnConfirmReject" runat="server" Text="Reject Order"
                        CssClass="btn btn-reject" OnClick="btnConfirmReject_Click" />
                </div>
            </div>
        </div>

    </div>

    <script>
        function openReject(orderId) {
            document.getElementById("hfRejectOrderId").value = orderId;
            document.getElementById("rejectModal").style.display = "flex";
        }
        function closeReject() {
            document.getElementById("rejectModal").style.display = "none";
        }
    </script>

</asp:Content>--%>