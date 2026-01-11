<%@ Page Title="Seller Signup" Language="C#" MasterPageFile="~/SellPage.master" AutoEventWireup="true"
    CodeBehind="SellerSignup.aspx.cs" Inherits="FoodSaver.SellerSignup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="max-width:640px;">
        <h2 class="mb-3">Create seller account</h2>

        <asp:ValidationSummary ID="vsSummarySignup" runat="server" CssClass="text-danger mb-2"
            HeaderText="Please fix the following:" />

        <div class="row">
            <div class="col-md-6 mb-3">
                <label for="txtStoreName" class="form-label">Store name</label>
                <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvStoreName" runat="server" ControlToValidate="txtStoreName"
                    ErrorMessage="Store name is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtPhone" class="form-label">Phone (optional)</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" Placeholder="e.g. 9123 4567" />
            </div>
        </div>

        <div class="mb-3">
            <label for="txtEmail" class="form-label">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="Invalid email format." CssClass="text-danger" Display="Dynamic"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label for="txtPassword" class="form-label">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6 mb-3">
                <label for="txtConfirmPassword" class="form-label">Confirm password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvConfirm" runat="server" ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Confirm password is required." CssClass="text-danger" Display="Dynamic" />
                <asp:CompareValidator ID="cvPasswords" runat="server" ControlToCompare="txtPassword"
                    ControlToValidate="txtConfirmPassword" ErrorMessage="Passwords do not match."
                    CssClass="text-danger" Display="Dynamic" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-8 mb-3">
                <label for="txtAddress" class="form-label">Store address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress"
                    ErrorMessage="Store address is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-4 mb-3">
                <label for="txtPostal" class="form-label">Postal code (optional)</label>
                <asp:TextBox ID="txtPostal" runat="server" CssClass="form-control" />
            </div>
        </div>

        <asp:Button ID="btnSignup" runat="server" Text="Create account"
            CssClass="btn btn-success" OnClick="btnSignup_Click" />

        <div class="mt-3 small">
            Already have an account? <a href="SellerLogin.aspx">Sign in</a>
        </div>
    </div>
</asp:Content>
