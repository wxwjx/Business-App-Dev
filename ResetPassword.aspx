<%@ Page Title="EcoEats | Reset Password" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Business_App_Dev.ResetPassword" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsRegister.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="login-page">
    <div class="brand">
        <div class="brand-icon">
            <svg viewBox="0 0 24 24" aria-hidden="true">
                <path d="M20.5 3.5c-7.2.2-12.2 3.1-14.8 8.6C4.3 14.7 4 17 4 20c3 0 5.3-.3 7.9-1.7 5.5-2.6 8.4-7.6 8.6-14.8z"/>
            </svg>
        </div>
        <div class="brand-name">EcoEats</div>
        <div class="brand-tagline">Create a new password</div>
    </div>

    <div class="card">
        <div class="card-header">
            <div class="header-title">Reset Password</div>
            <div class="header-sub">Enter your new password</div>
        </div>

        <div class="card-body">
            <asp:Panel ID="pnlReset" runat="server">

                <div class="field">
                    <div class="label">New Password</div>
                    <div class="input-wrap has-eye">
                        <span class="icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24"><path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5z"/></svg>
                        </span>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="textbox" TextMode="Password" placeholder="••••••••" />
                        <button type="button" class="eye-btn" onclick="togglePw('<%= txtNewPassword.ClientID %>', this)">👁</button>
                    </div>


                    <!-- Live rules box -->
                    <div id="pwRules" class="pw-rules">
                        <div id="ruleLen" class="rule">• At least 8 characters</div>
                        <div id="ruleLetter" class="rule">• At least 1 letter (A–Z)</div>
                        <div id="ruleNum" class="rule">• At least 1 number (0–9)</div>
                        <div id="ruleSpecial" class="rule">• At least 1 special character (!@#$...)</div>
                    </div>
                </div>

                <div class="field">
                    <div class="label">Confirm New Password</div>
                    <div class="input-wrap has-eye">
                        <span class="icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24"><path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5z"/></svg>
                        </span>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="textbox" TextMode="Password" placeholder="••••••••" />
                        <button type="button" class="eye-btn" onclick="togglePw('<%= txtConfirmPassword.ClientID %>', this)">👁</button>
                    </div>
                </div>

                <asp:Label ID="lblMsg" runat="server" CssClass="error" EnableViewState="false" />

                <asp:Button ID="btnReset" runat="server" Text="Update Password" CssClass="btn" OnClick="btnReset_Click" />
            </asp:Panel>

            <asp:Panel ID="pnlInvalid" runat="server" Visible="false">
                <asp:Label ID="lblInvalid" runat="server" CssClass="error" EnableViewState="false" />
                <div class="footer-link">
                    <a href="ForgotPassword.aspx" class="link">Request a new reset link</a>
                </div>
            </asp:Panel>

        </div>
    </div>
</div>

<script>
    function togglePw(inputId, btn) {
        const input = document.getElementById(inputId);
        if (!input) return;
        const isHidden = input.type === "password";
        input.type = isHidden ? "text" : "password";
        btn.textContent = isHidden ? "🙈" : "👁";
    }


    function updatePasswordUI() {
        const pwEl = document.getElementById("<%= txtNewPassword.ClientID %>");
        const cfEl = document.getElementById("<%= txtConfirmPassword.ClientID %>");
        if (!pwEl || !cfEl) return;

        const pw = pwEl.value || "";
        const confirm = cfEl.value || "";

        const hasLen = pw.length >= 8;
        const hasLetter = /[A-Za-z]/.test(pw);
        const hasNum = /[0-9]/.test(pw);
        const hasSpecial = /[^A-Za-z0-9]/.test(pw);

        document.getElementById("ruleLen").classList.toggle("ok", hasLen);
        document.getElementById("ruleLetter").classList.toggle("ok", hasLetter);
        document.getElementById("ruleNum").classList.toggle("ok", hasNum);
        document.getElementById("ruleSpecial").classList.toggle("ok", hasSpecial);

        const msg = document.getElementById("confirmMsg");
        if (!msg) return;

        if (!confirm) {
            msg.textContent = "";
            msg.classList.remove("ok", "bad");
        } else if (pw === confirm) {
            msg.textContent = "Passwords match ✅";
            msg.classList.add("ok");
            msg.classList.remove("bad");
        } else {
            msg.textContent = "Passwords do not match ❌";
            msg.classList.add("bad");
            msg.classList.remove("ok");
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        const pwEl = document.getElementById("<%= txtNewPassword.ClientID %>");
        const cfEl = document.getElementById("<%= txtConfirmPassword.ClientID %>");

        if (pwEl) pwEl.addEventListener("input", updatePasswordUI);
        if (cfEl) cfEl.addEventListener("input", updatePasswordUI);

        updatePasswordUI();
    });
</script>

</asp:Content>