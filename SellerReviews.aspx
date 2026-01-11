<%@ Page Title="Customer Reviews" Language="C#" MasterPageFile="~/SellPage.master"
    AutoEventWireup="true" CodeBehind="SellerReviews.aspx.cs"
    Inherits="FoodSaver.SellerReviews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container" style="max-width: 1100px;">
        <h2 class="mb-3">Customer Reviews</h2>

        <!-- Optional: simple filters -->
        <div class="row g-2 mb-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-select"
                    AutoPostBack="true" OnSelectedIndexChanged="FiltersChanged">
                </asp:DropDownList>
            </div>

            <div class="col-md-5">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                    Placeholder="Search comment / customer..."
                    AutoPostBack="true" OnTextChanged="FiltersChanged" />
            </div>

            <div class="col-md-2">
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-outline-secondary w-100"
                    Text="Clear" OnClick="btnClear_Click" />
            </div>

            <div class="col-md-2 d-flex align-items-center">
                <asp:Label ID="lblSummary" runat="server" CssClass="text-muted"></asp:Label>
            </div>
        </div>

        <asp:GridView ID="gvFeedback" runat="server" CssClass="table table-striped"
            AutoGenerateColumns="false" EmptyDataText="No feedback found.">
            <Columns>
                <asp:BoundField DataField="CreatedAt" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                <asp:BoundField DataField="Rating" HeaderText="Rating" />
                <asp:BoundField DataField="OrderRef" HeaderText="Order" />
                <asp:BoundField DataField="Comment" HeaderText="Comment" />
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>

