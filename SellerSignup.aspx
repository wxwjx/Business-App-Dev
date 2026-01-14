<%@ Page Title="Seller Signup" Language="C#" MasterPageFile="~/SellPage.master" AutoEventWireup="true"
    CodeBehind="SellerSignup.aspx.cs" Inherits="FoodSaver.SellerSignup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="max-width:640px;">
        <h2 class="mb-3">Create seller account</h2>

        <asp:ValidationSummary ID="vsSummarySignup" runat="server" CssClass="text-danger mb-2"
            HeaderText="Please fix the following:" />

        <div class="mb-3">
            <label for="txtStoreName" class="form-label">Business / Shop name</label>
            <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvStoreName" runat="server" ControlToValidate="txtStoreName"
                ErrorMessage="Business name is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="mb-3">
            <label for="txtOwner" class="form-label">Owner name</label>
            <asp:TextBox ID="txtOwner" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="txtOwner"
                ErrorMessage="Owner name is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="mb-3">
            <label for="ddlCategory" class="form-label">Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                <asp:ListItem Text="Select category" Value="" />
                <asp:ListItem Text="Bakery" Value="Bakery" />
                <asp:ListItem Text="Restaurant" Value="Restaurant" />
                <asp:ListItem Text="Cafe" Value="Cafe" />
                <asp:ListItem Text="Grocer" Value="Grocer" />
                <asp:ListItem Text="Other" Value="Other" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory"
                InitialValue="" ErrorMessage="Category is required." CssClass="text-danger" Display="Dynamic" />
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

        <asp:Button ID="btnSignup" runat="server" Text="Create account"
            CssClass="btn btn-success" OnClick="btnSignup_Click" />

        <div class="mt-3 small">
            Already have an account? <a href="SellerLogin.aspx">Sign in</a>
        </div>
    </div>
</asp:Content>
