<%@ Page Title="EcoEats | Forgot Password" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Business_App_Dev.ForgotPassword" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsRegister.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="login-page">
    <div class="brand">
        <div class="brand-icon">
            <svg viewBox="0 0 24 24" aria-hidden="true">
                <path d="M20.5 3.5c-7.2.2-12.2 3.1-14.8 8.6C4.3 14.7 4 17 4 20c3 0 5.3-.3 7.9-1.7 5.5-2.6 8.4-7.6 8.6-14.8zM7.7 16.3c2.9-3.2 6.6-5.4 11.2-6.4-3.9 1.7-7 4.2-9.3 7.6-.5.8-1.5 1-2.3.5-.8-.5-1-1.5-.5-2.3.3-.4.6-.9.9-1.3z" />
            </svg>
        </div>
        <div class="brand-name">EcoEats</div>
        <div class="brand-tagline">Reset your password securely</div>
    </div>

    <div class="card">
        <div class="card-header">
            <div class="header-title">Forgot Password</div>
            <div class="header-sub">Enter your email to receive a reset link</div>
        </div>

        <div class="card-body">

            <div class="field">
                <div class="label">Email</div>
                <div class="input-wrap">
                    <span class="icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24">
                            <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>
                        </svg>
                    </span>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" TextMode="Email" placeholder="your@email.com" />
                </div>
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="error" EnableViewState="false" />

            <asp:Button ID="btnSend" runat="server" Text="Send Reset Link" CssClass="btn" OnClick="btnSend_Click" />

            <div class="footer-link">
                Back to
                <a href="Login.aspx" class="link">Sign in</a>
            </div>

        </div>
    </div>
</div>

</asp:Content>

