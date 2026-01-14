<%@ Page Title="EcoEats | Admin Sign In" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="Business_App_Dev.AdminLogin" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsLogin.css") %>" rel="stylesheet" />
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
            <div class="brand-tagline">Admin Portal</div>
        </div>

        <div class="card">
            <div class="card-header">
                <div class="header-title">Admin Sign In</div>
                <div class="header-sub">Enter your admin credentials</div>
            </div>

            <div class="card-body">

                <!-- Removed role selector since this is AdminLogin -->
                <div class="field">
                    <div class="label">Email</div>
                    <div class="input-wrap">
                        <span class="icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24">
                                <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>
                            </svg>
                        </span>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" TextMode="Email" placeholder="admin@email.com" />
                    </div>
                </div>

                <div class="field">
                    <div class="label">Password</div>
                    <div class="input-wrap password-wrap">
                        <span class="icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24">
                                <path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5zm-3 8V6a3 3 0 016 0v3H9z"/>
                            </svg>
                        </span>

                        <asp:TextBox ID="txtPassword" runat="server"
                            CssClass="textbox"
                            TextMode="Password"
                            placeholder="••••••••" />

                        <span class="eye-icon" onclick="togglePassword()">👁</span>
                    </div>
                </div>

                <asp:Label ID="lblError" runat="server" CssClass="error" EnableViewState="false" />

                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn"
                    OnClick="btnSignIn_Click" />
            </div>
        </div>
    </div>

<script>
    function togglePassword() {
        var pwd = document.getElementById('<%= txtPassword.ClientID %>');
        pwd.type = (pwd.type === "password") ? "text" : "password";
    }
</script>

</asp:Content>
