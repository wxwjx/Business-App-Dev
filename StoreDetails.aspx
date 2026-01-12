<%@ Page Title="" Language="C#" MasterPageFile="~/SellerPage.Master" AutoEventWireup="true" CodeBehind="StoreDetails.aspx.cs" Inherits="Business_App_Dev.StoreDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Store Details</h2>
    <div class="eco-card p-4 mt-3">
        <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3" Visible="false"></asp:Label>
        <div class="mb-3">
            <label class="form-label">Store Name</label>
            <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label class="form-label">Store Address</label>
            <asp:TextBox ID="txtStoreAddress" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label class="form-label">Operating Hours</label>
            <asp:TextBox ID="txtStoreHours" runat="server" CssClass="form-control" Placeholder="9am - 9pm" />
        </div>
        <div class="mb-3">
            <label class="form-label">Contact Number</label>
            <asp:TextBox ID="txtStoreContact" runat="server" CssClass="form-control" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
