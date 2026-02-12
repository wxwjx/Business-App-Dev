<%@ Page Title="EcoEats | Sign In" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Business_App_Dev.Login" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsLogin.css") %>" rel="stylesheet" />
    <script src="https://www.google.com/recaptcha/api.js" async defer></script>

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="login-page">
        <div class="brand">
            <div class="brand-icon">
                <!-- simple leaf icon (SVG) -->
                <svg viewBox="0 0 24 24" aria-hidden="true">
                    <path d="M20.5 3.5c-7.2.2-12.2 3.1-14.8 8.6C4.3 14.7 4 17 4 20c3 0 5.3-.3 7.9-1.7 5.5-2.6 8.4-7.6 8.6-14.8zM7.7 16.3c2.9-3.2 6.6-5.4 11.2-6.4-3.9 1.7-7 4.2-9.3 7.6-.5.8-1.5 1-2.3.5-.8-.5-1-1.5-.5-2.3.3-.4.6-.9.9-1.3z" />
                </svg>
            </div>

            <div class="brand-name">EcoEats</div>
            <div class="brand-tagline">Save meals, save money, save the planet</div>
        </div>

        <div class="card">
            <div class="card-header">
                <div class="header-title">Welcome Back</div>
                <div class="header-sub">Sign in to continue saving</div>
            </div>

            <div class="card-body">
                <div class="field">
                    <div class="label">I am a...</div>

                    <!-- role pill toggle (RadioButtonList) -->
                    <asp:RadioButtonList ID="rblRole" runat="server" RepeatDirection="Horizontal" CssClass="role-pills">
                        <asp:ListItem Text="Customer" Value="Customer" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Seller" Value="Seller"></asp:ListItem>
                        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>

                <div class="field">
                    <div class="label">Email</div>
                    <div class="input-wrap">
                        <span class="icon" aria-hidden="true">
                            <!-- mail icon -->
                            <svg viewBox="0 0 24 24">
                                <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>
                            </svg>
                        </span>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" TextMode="Email" placeholder="your@email.com" />
                    </div>
                </div>

                <div class="field">
                    <div class="label">Password</div>
                    <div class="input-wrap password-wrap">
                        <span class="icon" aria-hidden="true">
                            <!-- lock icon -->
                            <svg viewBox="0 0 24 24">
                                <path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5zm-3 8V6a3 3 0 016 0v3H9z"/>
                            </svg>
                        </span>

                        <asp:TextBox ID="txtPassword" runat="server"
                            CssClass="textbox"
                            TextMode="Password"
                            placeholder="••••••••" />

                        <!-- 👁 Eye icon -->
                        <span class="eye-icon" onclick="togglePassword()">
                            👁
                        </span>
                    </div>
                </div>
                <div id="forgotWrap" class="forgot-row">
                    <a id="forgotLink" href="ForgotPassword.aspx" 
                        class="forgot-link">Forgot password?</a>
                </div>

                <!-- reCAPTCHA -->
                <div class="captcha-wrap">
                    <div class="g-recaptcha" data-sitekey="6Lf6c2ksAAAAAPosb1sOq_vnpJgsshNqYlZmTEvY"></div>
                </div>

                <asp:Label ID="lblError" runat="server" CssClass="error" Text="" EnableViewState="false" />

                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn"
                    OnClick="btnSignIn_Click" />

                <div class="signup-wrap" id="signupWrap">
                    Don’t have an account?
                    <a id="signupLink" href="RegisterCustomer.aspx" class="signup-link">
                        Sign up
                    </a>
                </div>

            </div>
        </div>
    </div>
<script>
    function togglePassword() {
        var pwd = document.getElementById('<%= txtPassword.ClientID %>');

        if (pwd.type === "password") {
            pwd.type = "text";
        } else {
            pwd.type = "password";
        }
    }
    function getSelectedRole() {
        // RadioButtonList renders inputs; find checked one inside the list
        const list = document.getElementById("<%= rblRole.ClientID %>");
        if (!list) return "Customer";

        const checked = list.querySelector("input[type='radio']:checked");
        return checked ? checked.value : "Customer";
    }

    function updateSignupLink() {
        const role = getSelectedRole();
        const wrap = document.getElementById("signupWrap");
        const link = document.getElementById("signupLink");

        if (!wrap || !link) return;

        if (role === "Admin") {
            wrap.style.display = "none";
            return;
        }

        wrap.style.display = "block";

        if (role === "Seller") {
            link.href = "RegisterSeller.aspx";   // create this page
        } else {
            link.href = "RegisterCustomer.aspx"; // your existing page
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        updateSignupLink();

        const list = document.getElementById("<%= rblRole.ClientID %>");
        if (!list) return;

        // Update when role changes
        list.addEventListener("change", updateSignupLink);
    });

    function togglePassword() {
        var pwd = document.getElementById('<%= txtPassword.ClientID %>');
        pwd.type = (pwd.type === "password") ? "text" : "password";
    }
    function updateForgotVisibility() {
        // Find selected role from the radio list
        const selected = document.querySelector('input[name="<%= rblRole.UniqueID %>"]:checked');
        const role = selected ? selected.value : "Customer";

        const wrap = document.getElementById("forgotWrap");
        if (!wrap) return;

        // Show only for Customer & Seller
        const show = (role === "Customer" || role === "Seller");
        wrap.style.display = show ? "block" : "none";
    }

    document.addEventListener("DOMContentLoaded", function () {
        updateForgotVisibility();

        // When user changes pill selection
        const radios = document.querySelectorAll('input[name="<%= rblRole.UniqueID %>"]');
        radios.forEach(r => r.addEventListener("change", updateForgotVisibility));
    });
    function autoHideError() {
        const err = document.getElementById("<%= lblError.ClientID %>");
        if (!err) return;

        const text = (err.textContent || err.innerText || "").trim();
        if (!text) return;

        // show
        err.classList.add("show");

        // hide after 3s (but keep space)
        setTimeout(() => {
            err.classList.remove("show");
            // optional: clear text after fade so it won't reappear on refresh
            setTimeout(() => { err.innerHTML = ""; }, 300);
        }, 2000);
    }

    document.addEventListener("DOMContentLoaded", autoHideError);
</script>

</asp:Content>
