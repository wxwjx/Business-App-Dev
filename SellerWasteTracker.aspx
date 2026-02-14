<%@ Page Title="Waste Reduction Tracker" Language="C#" MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true" CodeBehind="SellerWasteTracker.aspx.cs"
    Inherits="EcoEats.SellerWasteTracker" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .wr-title { font-weight: 700; letter-spacing: -0.3px; }
    .wr-sub { color: #6c757d; }

    .wr-card { border: 1px solid rgba(0,0,0,.075); border-radius: 14px; }
    .wr-card .card-body { padding: 18px; }

    .wr-filter-grid { display: grid; grid-template-columns: 1fr 1fr 1fr auto; gap: 12px; align-items: end; }
    @media (max-width: 992px) { .wr-filter-grid { grid-template-columns: 1fr 1fr; } }
    @media (max-width: 576px) { .wr-filter-grid { grid-template-columns: 1fr; } }

    .wr-label { font-size: .85rem; color: #6c757d; margin-bottom: 6px; display: block; }
    .wr-input { height: 42px; border-radius: 10px; }

    .wr-btn { height: 42px; border-radius: 10px; padding: 0 16px; }
    .wr-btn-row { display: flex; gap: 10px; }

    .wr-kpi { display: flex; gap: 12px; align-items: center; }
    .wr-kpi-icon {
        width: 44px; height: 44px; border-radius: 12px;
        display: grid; place-items: center;
        background: rgba(25,135,84,.10); color: #198754;
        font-size: 18px;
    }
    .wr-kpi-label { font-size: .85rem; color: #6c757d; margin-bottom: 2px; }
    .wr-kpi-value { font-size: 1.6rem; font-weight: 700; margin: 0; }

    .wr-section-title { font-weight: 700; margin: 0; }
    .wr-table-wrap { overflow-x: auto; }

    .wr-empty {
        border: 1px dashed rgba(0,0,0,.2);
        border-radius: 14px;
        padding: 18px;
        background: rgba(0,0,0,.02);
        color: #6c757d;
    }
</style>

<div class="container mt-4">

    <div class="d-flex align-items-center gap-2 mb-2">
        <span style="font-size:20px;">🌿</span>
        <h2 class="wr-title mb-0">Waste Reduction Tracker</h2>
    </div>
    <div class="wr-sub mb-4">Track how much surplus food you helped save through completed sales.</div>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block mb-3" />

    <!-- Filters -->
    <div class="card wr-card shadow-sm mb-4">
        <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="wr-section-title">Filters</h5>
                <span class="text-muted small">Tip: leave blank to view all-time</span>
            </div>

            <div class="wr-filter-grid">
                <div>
                    <label class="wr-label">From</label>
                    <asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="form-control wr-input" />
                </div>

                <div>
                    <label class="wr-label">To</label>
                    <asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="form-control wr-input" />
                </div>

                <div>
                    <label class="wr-label">Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select wr-input" />
                </div>

                <div class="wr-btn-row">
                    <asp:Button ID="btnApply" runat="server" Text="Apply" CssClass="btn btn-success wr-btn"
                        OnClick="btnApply_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-secondary wr-btn"
                        OnClick="btnReset_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- KPI cards -->
    <div class="row g-3 mb-4">
        <div class="col-lg-4">
            <div class="card wr-card shadow-sm">
                <div class="card-body">
                    <div class="wr-kpi">
                        <div class="wr-kpi-icon">🧺</div>
                        <div>
                            <div class="wr-kpi-label">Items Saved (Sold)</div>
                            <p class="wr-kpi-value">
                                <asp:Label ID="lblItemsSaved" runat="server" Text="0" />
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="card wr-card shadow-sm">
                <div class="card-body">
                    <div class="wr-kpi">
                        <div class="wr-kpi-icon">💰</div>
                        <div>
                            <div class="wr-kpi-label">Revenue from Saved Items</div>
                            <p class="wr-kpi-value">
                                <asp:Label ID="lblRevenue" runat="server" Text="$0.00" />
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="card wr-card shadow-sm">
                <div class="card-body">
                    <div class="wr-kpi">
                        <div class="wr-kpi-icon">🌍</div>
                        <div>
                            <div class="wr-kpi-label">Estimated CO₂ Saved</div>
                            <p class="wr-kpi-value mb-1">
                                <asp:Label ID="lblCO2" runat="server" Text="0.00" />
                            </p>
                            <div class="text-muted small">kg CO₂e</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Breakdown -->
    <div class="card wr-card shadow-sm">
        <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="wr-section-title">Breakdown by Product</h5>
                <span class="text-muted small">Sorted by Qty Sold</span>
            </div>

            <div class="wr-table-wrap">
                <asp:GridView ID="gvBreakdown" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-hover align-middle mb-0"
                    EmptyDataText="">
                    <Columns>
                        <asp:BoundField DataField="ProductName" HeaderText="Product" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:BoundField DataField="QtySold" HeaderText="Qty Sold" />
                        <asp:BoundField DataField="Revenue" HeaderText="Revenue ($)" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="CO2Saved" HeaderText="CO₂ Saved (kg)" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Nice empty state (only shown if grid has no rows) -->
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="wr-empty mt-3">
                <div class="fw-semibold mb-1">No records found</div>
                <div class="small">Try widening the date range, selecting “All” categories, or confirm orders are marked as PAID.</div>
            </asp:Panel>

        </div>
    </div>

</div>

</asp:Content>

