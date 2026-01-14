<%@ Page Title="Enable 2FA" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="Enable2FA.aspx.cs" Inherits="Business_App_Dev.Enable2FA" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card" style="width:560px; max-width:94vw;">
        <div class="card-header">
            <div class="header-title">Enable 2FA (Google Authenticator)</div>
            <div class="header-sub">Scan QR code, then enter the 6-digit code</div>
        </div>

        <div class="card-body">
            <asp:Image ID="imgQr" runat="server" style="display:block;margin:12px auto;max-width:260px;" />

            <div class="field">
                <div class="label">6-digit code</div>
                <asp:TextBox ID="txtCode" runat="server" CssClass="textbox" placeholder="123456" />
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="error" />
            <asp:Button ID="btnVerify" runat="server" Text="Verify & Enable" CssClass="btn" OnClick="btnVerify_Click" />
        </div>
    </div>
</asp:Content>
