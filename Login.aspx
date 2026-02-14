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
                    <div class="label" id="loginWithLabel">Login with...</div>
                    <div class="label" id="loginIdLabel2" style="margin-top:10px;">Email</div>
                    <!-- Email / Phone toggle (Customer/Seller only; Admin forced Email) -->
                    <div class="login-toggle" id="loginToggle">
                        <label class="toggle-pill">
                            <input type="radio" name="loginMode" value="Email" checked />
                            <span>Email</span>
                        </label>

                        <label class="toggle-pill">
                            <input type="radio" name="loginMode" value="Phone" />
                            <span>Phone</span>
                        </label>
                    </div>

                    <div class="input-wrap login-id-wrap">
                        <span class="icon" aria-hidden="true">
                            <!-- icon will be updated by JS (mail/phone) -->
                            <svg id="loginIcon" viewBox="0 0 24 24">
                                <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>
                            </svg>
                        </span>

                        <!-- country code (only for Phone mode) -->
                        <asp:DropDownList ID="ddlLoginCountryCode" runat="server" CssClass="country-code login-cc">
                            <asp:ListItem Value="+65" Selected="True">SG +65</asp:ListItem>
                            <asp:ListItem Value="+60">MY +60</asp:ListItem>
                            <asp:ListItem Value="+62">ID +62</asp:ListItem>
                            <asp:ListItem Value="+66">TH +66</asp:ListItem>
                            <asp:ListItem Value="+84">VN +84</asp:ListItem>
                        </asp:DropDownList>

                        <!-- one textbox -->
                        <asp:TextBox ID="txtLoginId" runat="server" CssClass="textbox login-id"
                            placeholder="your@email.com" />
                        <asp:HiddenField ID="hfLoginMode" runat="server" Value="Email" />

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
    // =========================
    // Helpers
    // =========================
    function togglePassword() {
        const pwd = document.getElementById('<%= txtPassword.ClientID %>');
        if (!pwd) return;
        pwd.type = (pwd.type === "password") ? "text" : "password";
    }

    function getSelectedRole() {
        const selected = document.querySelector('input[name="<%= rblRole.UniqueID %>"]:checked');
        return selected ? selected.value : "Customer";
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
        link.href = (role === "Seller") ? "RegisterSeller.aspx" : "RegisterCustomer.aspx";
    }

    function updateForgotVisibility() {
        const role = getSelectedRole();
        const wrap = document.getElementById("forgotWrap");
        if (!wrap) return;

        const show = (role === "Customer" || role === "Seller");
        wrap.style.display = show ? "block" : "none";
    }

    function autoHideError() {
        const err = document.getElementById("<%= lblError.ClientID %>");
        if (!err) return;

        const text = (err.textContent || err.innerText || "").trim();
        if (!text) return;

        err.classList.add("show");
        setTimeout(() => {
            err.classList.remove("show");
            setTimeout(() => { err.innerHTML = ""; }, 300);
        }, 2000);
    }

    // =========================
    // Clear inputs on switch
    // =========================
    function clearLoginInputs() {
        const input = document.getElementById("<%= txtLoginId.ClientID %>");
      const pwd = document.getElementById("<%= txtPassword.ClientID %>");
      const cc = document.getElementById("<%= ddlLoginCountryCode.ClientID %>");
      const err = document.getElementById("<%= lblError.ClientID %>");

    if (input) input.value = "";
    if (pwd) pwd.value = "";
    if (cc) cc.selectedIndex = 0;
    if (err) err.innerHTML = "";

    setPhoneHint(""); // hide phone warning msg
  }

  // =========================
  // Phone-only digits warning
  // =========================
  function setPhoneHint(msg) {
    let hint = document.getElementById("phoneDigitsMsg");

    if (!hint) {
      hint = document.createElement("div");
      hint.id = "phoneDigitsMsg";
      hint.className = "field-msg bad";
      hint.style.marginTop = "6px";

      const field = document.querySelector(".input-wrap.login-id-wrap")?.parentNode;
      if (field) field.appendChild(hint);
    }

    hint.textContent = msg || "";
    hint.style.display = msg ? "block" : "none";
  }

  function attachPhoneDigitGuard() {
    const input = document.getElementById("<%= txtLoginId.ClientID %>");
    const hf = document.getElementById("<%= hfLoginMode.ClientID %>");
    if (!input || !hf) return;

    input.addEventListener("input", function () {
      const mode = (hf.value || "Email");
      if (mode !== "Phone") {
        setPhoneHint("");
        return;
      }

      const original = input.value;
      const digitsOnly = original.replace(/\D/g, "");

      if (original !== digitsOnly) {
        input.value = digitsOnly;
        setPhoneHint("Only digits are allowed for phone number.");
      } else {
        setPhoneHint("");
      }
    });
  }

  // =========================
  // Login mode switcher
  // =========================
    function setLoginMode(mode) {
        const role = getSelectedRole();

        const loginWithLabel = document.getElementById("loginWithLabel");
        const label2 = document.getElementById("loginIdLabel2"); // the "Email" line
        const toggle = document.getElementById("loginToggle");

        const cc = document.getElementById("<%= ddlLoginCountryCode.ClientID %>");
      const input = document.getElementById("<%= txtLoginId.ClientID %>");
  const icon = document.getElementById("loginIcon");
        const hf = document.getElementById("<%= hfLoginMode.ClientID %>");

        if (!cc || !input || !toggle || !icon || !hf) return;

        // ✅ Always hide the extra "Email" label line (Customer/Seller/Admin)
        if (label2) label2.style.display = "none";

        if (role === "Admin") {

            // change Login with... into Email
            if (loginWithLabel) {
                loginWithLabel.style.display = "block";
                loginWithLabel.textContent = "Email";
            }

            // hide toggle for admin
            toggle.style.display = "none";

            // force email mode UI
            cc.style.display = "none";
            input.placeholder = "your@email.com";
            input.type = "email";
            input.classList.remove("phone-mode");
            input.classList.add("email-mode");

            icon.innerHTML =
                '<path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>';

            hf.value = "Email";
            setPhoneHint("");
            return;
        }


        // ✅ CUSTOMER/SELLER: show "Login with..." + show toggle
        if (loginWithLabel) {
            loginWithLabel.style.display = "block";
            loginWithLabel.textContent = "Login with...";
        }
        toggle.style.display = "flex";

        // normal behavior for email/phone
        if (mode === "Phone") {
            cc.style.display = "inline-block";
            input.placeholder = "81234567";
            input.type = "text";
            input.classList.remove("email-mode");
            input.classList.add("phone-mode");
            icon.innerHTML =
                '<path d="M6.6 10.8c1.3 2.6 3.4 4.7 6 6l2-2c.3-.3.8-.4 1.2-.3 1 .3 2 .5 3.1.5.6 0 1 .4 1 1V20c0 .6-.4 1-1 1C10.8 21 3 13.2 3 3c0-.6.4-1 1-1h3.5c.6 0 1 .4 1 1 0 1.1.2 2.1.5 3.1.1.4 0 .9-.3 1.2l-2 2z"/>';
        } else {
            cc.style.display = "none";
            input.placeholder = "your@email.com";
            input.type = "email";
            input.classList.remove("phone-mode");
            input.classList.add("email-mode");
            icon.innerHTML =
                '<path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>';
            setPhoneHint("");
        }

        // sync toggle + hidden field
        document.querySelectorAll('input[name="loginMode"]').forEach(r => r.checked = (r.value === mode));
        hf.value = mode;
    }


  // =========================
  // ONE DOMContentLoaded only
  // =========================
  document.addEventListener("DOMContentLoaded", function () {
    // init
    updateSignupLink();
    updateForgotVisibility();
    attachPhoneDigitGuard();
    setLoginMode("Email");
    autoHideError();

    // Role change -> clear all + update UI + force mode
    document.querySelectorAll('input[name="<%= rblRole.UniqueID %>"]').forEach(r =>
          r.addEventListener("change", function () {
              clearLoginInputs();
              updateSignupLink();
              updateForgotVisibility();

              const cur = document.querySelector('input[name="loginMode"]:checked')?.value || "Email";
              setLoginMode(cur);
          })
      );

      // Email/Phone toggle change -> clear all + set mode
      document.querySelectorAll('input[name="loginMode"]').forEach(r =>
          r.addEventListener("change", function () {
              clearLoginInputs();
              setLoginMode(r.value);
          })
      );
  });
</script>


</asp:Content>
