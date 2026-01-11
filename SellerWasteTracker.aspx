<%@ Page Title="Waste Reduction Tracker" Language="C#" MasterPageFile="~/SellPage.master"
    AutoEventWireup="true" CodeBehind="SellerWasteTracker.aspx.cs"
    Inherits="FoodSaver.SellerWasteTracker" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container" style="max-width: 1100px;">
        <div class="d-flex justify-content-between align-items-center mb-2">
            <h2 class="mb-0">Waste Reduction Tracker</h2>
            <asp:Label ID="lblUpdated" runat="server" CssClass="text-muted"></asp:Label>
        </div>

        <asp:Label ID="lblPeriod" runat="server" CssClass="text-muted"></asp:Label>

        <!-- KPI Cards -->
        <div class="row g-3 my-3">
            <div class="col-md-3">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="text-muted">Food Saved (kg)</div>
                        <asp:Label ID="lblFoodSaved" runat="server" CssClass="fs-4 fw-semibold"></asp:Label>
                        <div class="text-muted small">Estimated (prototype)</div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="text-muted">Meals Rescued</div>
                        <asp:Label ID="lblMealsRescued" runat="server" CssClass="fs-4 fw-semibold"></asp:Label>
                        <div class="text-muted small">Count of rescued items</div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="text-muted">CO₂e Avoided (kg)</div>
                        <asp:Label ID="lblCo2Avoided" runat="server" CssClass="fs-4 fw-semibold"></asp:Label>
                        <div class="text-muted small">Conversion estimate</div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="text-muted">Rescue Rate</div>
                        <asp:Label ID="lblRescueRate" runat="server" CssClass="fs-4 fw-semibold"></asp:Label>
                        <div class="text-muted small">Rescued vs listed</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Table -->
        <h5 class="mt-4">Recent Activity (Last 14 Days)</h5>
        <asp:GridView ID="gvWaste" runat="server" CssClass="table table-striped mt-2"
            AutoGenerateColumns="false" EmptyDataText="No tracker data available.">
            <Columns>
                <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="ItemsRescued" HeaderText="Items Rescued" />
                <asp:BoundField DataField="KgSaved" HeaderText="Est. kg Saved" DataFormatString="{0:0.00}" />
                <asp:BoundField DataField="Co2eAvoided" HeaderText="Est. CO₂e Avoided (kg)" DataFormatString="{0:0.00}" />
            </Columns>
        </asp:GridView>

        <div class="text-muted small mt-2">
            * Prototype note: values are estimated and generated for demonstration (fake dataset).
        </div>
    </div>

</asp:Content>
