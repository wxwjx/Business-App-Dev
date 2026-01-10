<%@ Page Title="Seller Login" Language="C#" MasterPageFile="~/SellPage.master" AutoEventWireup="true" CodeBehind="SellerLogin.aspx.cs" Inherits="FoodSaver.SellerLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="max-width:480px;">
        <h2 class="mb-3">Seller sign in</h2>

        <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="text-danger mb-2" HeaderText="Please fix the following:" />

        <div class="mb-3">
            <label for="txtEmail" class="form-label">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="seller@example.com" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="mb-3">
            <label for="txtPassword" class="form-label">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="d-flex justify-content-between align-items-center mb-3">
            <asp:CheckBox ID="chkRemember" runat="server" /> <label for="chkRemember" class="mb-0 ms-1">Remember me</label>
            <a href="SellerDashboard.aspx" class="small">Continue as guest</a>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />

        <div class="mt-3 small text-muted">
            Use <strong>seller@example.com</strong> / <strong>Password123</strong> for the demo.
        </div>

        <div class="mt-3 text-center">
            <span class="small">Don't have a seller account? </span>
            <a class="small" href="SellerSignup.aspx">Create one</a>
        </div>
    </div>
</asp:Content>