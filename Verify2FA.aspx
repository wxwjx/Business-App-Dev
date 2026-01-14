<%@ Page Title="EcoEats | Verify 2FA" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="Verify2FA.aspx.cs" Inherits="Business_App_Dev.Verify2FA" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/Verify2FA.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="verify-page">
        <!-- blurred background overlay -->
        <div class="bg-blur"></div>

        <div class="verify-wrap">
            <div class="brand">
                <div class="brand-icon">
                    <!-- same leaf icon as login -->
                    <svg viewBox="0 0 24 24" aria-hidden="true">
                        <path d="M20.5 3.5c-7.2.2-12.2 3.1-14.8 8.6C4.3 14.7 4 17 4 20c3 0 5.3-.3 7.9-1.7 5.5-2.6 8.4-7.6 8.6-14.8zM7.7 16.3c2.9-3.2 6.6-5.4 11.2-6.4-3.9 1.7-7 4.2-9.3 7.6-.5.8-1.5 1-2.3.5-.8-.5-1-1.5-.5-2.3.3-.4.6-.9.9-1.3z"/>
                    </svg>
                </div>

                <div class="brand-name">EcoEats</div>
                <div class="brand-tagline">Two-Factor Authentication</div>
            </div>

            <div class="card">
                <div class="card-header">
                    <div class="header-title">Verify your sign in</div>
                    <div class="header-sub">
                        Enter the 6-digit code from your Authenticator app
                    </div>
                </div>

                <div class="card-body">


                    <div class="field">
                        <div class="label">Authentication code</div>
                        <asp:TextBox ID="txtCode" runat="server" CssClass="textbox"
                            placeholder="Enter 6-digit code" MaxLength="6" />
                    </div>

                    <asp:Label ID="lblMsg" runat="server" CssClass="error" EnableViewState="false" />

                    <asp:Button ID="btnVerify" runat="server" Text="Verify"
                        CssClass="btn" OnClick="btnVerify_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
