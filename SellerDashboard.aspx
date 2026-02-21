<%@ Page Title="Sales Dashboard"
    Language="C#"
    MasterPageFile="~/SellPage.master"
    AutoEventWireup="true"
    CodeBehind="SallesDashboard.aspx.cs"
    Inherits="Business_App_Dev.SalesDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>login Successful</p>
   <%-- <style>
        .sdash-wrap {
            max-width: 1100px;
            margin: 24px auto;
            padding: 0 10px;
        }

        .sdash-head {
            display: flex;
            justify-content: space-between;
            align-items: flex-end;
            gap: 12px;
            margin-bottom: 14px;
            flex-wrap: wrap;
        }

        .sdash-title h2 {
            margin: 0;
        }

        .sdash-title p {
            margin: 6px 0 0;
            color: #6b7280;
            font-size: 14px;
        }

        .sdash-filters {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            align-items: center;
        }

        .sdash-pill {
            border: 1px solid #e5e7eb;
            background: #fff;
            border-radius: 999px;
            padding: 8px 12px;
            font-weight: 700;
            cursor: pointer;
        }

            .sdash-pill.active {
                box-shadow: 0 8px 18px rgba(0,0,0,.08);
                border-color: #d1d5db;
            }

        .sdash-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 14px;
        }

        .kpi-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 12px;
        }

        @media(max-width: 900px) {
            .kpi-grid {
                grid-template-columns: repeat(2,1fr);
            }
        }

        @media(max-width: 520px) {
            .kpi-grid {
                grid-template-columns: 1fr;
            }
        }

        .card {
            background: #fff;
            border-radius: 16px;
            padding: 18px;
            box-shadow: 0 6px 18px rgba(0,0,0,0.06);
        }

        .kpi-label {
            color: #6b7280;
            font-weight: 700;
            font-size: 13px;
        }

        .kpi-value {
            font-size: 26px;
            font-weight: 900;
            margin-top: 6px;
            color: #111827;
        }

        .kpi-sub {
            margin-top: 6px;
            color: #6b7280;
            font-size: 13px;
        }

        .two-col {
            display: grid;
            grid-template-columns: 1.2fr .8fr;
            gap: 14px;
        }

        @media(max-width: 992px) {
            .two-col {
                grid-template-columns: 1fr;
            }
        }

        .tbl {
            width: 100%;
            border-collapse: collapse;
        }

            .tbl th, .tbl td {
                padding: 10px 8px;
                border-bottom: 1px solid #eee;
                text-align: left;
                font-size: 14px;
            }

            .tbl th {
                color: #6b7280;
                font-size: 12px;
                text-transform: uppercase;
                letter-spacing: .04em;
            }

        .badge {
            display: inline-flex;
            padding: 6px 10px;
            border-radius: 999px;
            background: rgba(0,0,0,0.04);
            font-weight: 800;
            font-size: 12px;
        }
    </style>

    <div class="sdash-wrap">

        <div class="sdash-head">
            <div class="sdash-title">
                <p>Login Sussessful!</p>
                <h2>Sales Dashboard</h2>
                <p>Track orders, revenue, trends, and best-selling items.</p>
            </div>

            <div class="sdash-filters">
                <asp:Button ID="btn7" runat="server" Text="Last 7 days" CssClass="sdash-pill" OnClick="Range_Click" CommandArgument="7" />
                <asp:Button ID="btn30" runat="server" Text="Last 30 days" CssClass="sdash-pill active" OnClick="Range_Click" CommandArgument="30" />
                <asp:Button ID="btn90" runat="server" Text="Last 90 days" CssClass="sdash-pill" OnClick="Range_Click" CommandArgument="90" />
            </div>
        </div>

        <!-- KPI ROW -->
        <div class="kpi-grid">
            <div class="card">
                <div class="kpi-label">Orders</div>
                <div class="kpi-value">
                    <asp:Label ID="lblOrders" runat="server" Text="0" /></div>
                <div class="kpi-sub">Total orders in selected period</div>
            </div>

            <div class="card">
                <div class="kpi-label">Revenue</div>
                <div class="kpi-value">$<asp:Label ID="lblRevenue" runat="server" Text="0.00" /></div>
                <div class="kpi-sub">Gross sales (before platform fees)</div>
            </div>

            <div class="card">
                <div class="kpi-label">Avg Order Value</div>
                <div class="kpi-value">$<asp:Label ID="lblAOV" runat="server" Text="0.00" /></div>
                <div class="kpi-sub">Revenue / Orders</div>
            </div>

            <div class="card">
                <div class="kpi-label">Top Item</div>
                <div class="kpi-value">
                    <asp:Label ID="lblTopItem" runat="server" Text="—" /></div>
                <div class="kpi-sub"><span class="badge">
                    <asp:Label ID="lblTopItemUnits" runat="server" Text="0 sold" /></span></div>
            </div>
        </div>

        <div class="two-col">
            <!-- Sales Trend -->
            <div class="card">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                    <div style="font-weight: 900; font-size: 16px;">Sales Trend</div>
                    <div style="color: #6b7280; font-size: 13px;">
                        <asp:Label ID="lblRange" runat="server" /></div>
                </div>
                <canvas id="salesLine" height="110"></canvas>
            </div>

            <!-- Best Selling Items -->
            <div class="card">
                <div style="font-weight: 900; font-size: 16px; margin-bottom: 10px;">Best-selling items</div>

                <asp:GridView ID="gvTopItems" runat="server" AutoGenerateColumns="false" CssClass="tbl"
                    BorderStyle="None" GridLines="None" HeaderStyle-CssClass=""
                    RowStyle-CssClass="">
                    <Columns>
                        <asp:BoundField DataField="ProductName" HeaderText="Item" />
                        <asp:BoundField DataField="UnitsSold" HeaderText="Units" />
                        <asp:BoundField DataField="Revenue" HeaderText="Revenue" DataFormatString="{0:C}" />
                    </Columns>
                </asp:GridView>

                <div style="margin-top: 10px; color: #6b7280; font-size: 13px;">
                    Tip: If one item dominates sales, consider adjusting stock + pricing.
               
                </div>
            </div>
        </div>

        <!-- Hidden fields to pass chart JSON -->
        <asp:HiddenField ID="hfLabels" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfValues" runat="server" ClientIDMode="Static" />
    </div>

    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script>
        function renderSalesChart() {
            const labels = JSON.parse(document.getElementById("hfLabels").value || "[]");
            const values = JSON.parse(document.getElementById("hfValues").value || "[]");

            const ctx = document.getElementById("salesLine").getContext("2d");
            new Chart(ctx, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                        label: "Revenue ($)",
                        data: values,
                        tension: 0.35,
                        fill: false
                    }]
                },
                options: {
                    responsive: true,
                    plugins: { legend: { display: true } },
                    scales: {
                        y: { beginAtZero: true }
                    }
                }
            });
        }

        // render after page load
        window.addEventListener("load", renderSalesChart);
    </script>--%>

</asp:Content>
