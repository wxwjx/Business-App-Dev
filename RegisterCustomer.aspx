<%@ Page Title="EcoEats | Sign Up"
    Language="C#"
    MasterPageFile="~/Login.Master"
    AutoEventWireup="true"
    CodeBehind="RegisterCustomer.aspx.cs"
    Inherits="Business_App_Dev.RegisterCustomer" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/EcoEatsRegister.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="login-page">
        <div class="brand">
            <div class="brand-icon">
                <!-- leaf icon -->
                <svg viewBox="0 0 24 24" aria-hidden="true">
                    <path d="M20.5 3.5c-7.2.2-12.2 3.1-14.8 8.6C4.3 14.7 4 17 4 20c3 0 5.3-.3 7.9-1.7 5.5-2.6 8.4-7.6 8.6-14.8zM7.7 16.3c2.9-3.2 6.6-5.4 11.2-6.4-3.9 1.7-7 4.2-9.3 7.6-.5.8-1.5 1-2.3.5-.8-.5-1-1.5-.5-2.3.3-.4.6-.9.9-1.3z" />
                </svg>
            </div>

            <div class="brand-name">EcoEats</div>
            <div class="brand-tagline">Save meals, save money, save the planet</div>
        </div>

        <div class="card">
            <div class="card-header">
                <div class="header-title">Create Account</div>
                <div class="header-sub">Join the movement to reduce food waste</div>
            </div>

            <div class="card-body">


                <div class="field">
                    <div class="label">Full Name</div>
                    <div class="input-wrap">
                        <span class="icon" aria-hidden="true">
                            <!-- user icon -->
                            <svg viewBox="0 0 24 24">
                                <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.42 0-8 2.24-8 5v1h16v-1c0-2.76-3.58-5-8-5z"/>
                            </svg>
                        </span>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="textbox" placeholder="Enter your name" />
                    </div>
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
                        <asp:TextBox 
                                    ID="txtEmail" 
                                    runat="server" 
                                    CssClass="textbox"
                                    TextMode="Email"
                                    AutoPostBack="true"
                                    OnTextChanged="txtEmail_TextChanged"
                                    placeholder="your@email.com" />
                        <asp:Label ID="lblEmailStatus" runat="server" CssClass="field-msg" />

                    </div>
                    <div class="email-status">
                        <div id="emailMsg" class="field-msg"></div>
                    </div>

                </div>

                <div class="field">
                    <div class="label">Password</div>

                    <div class="input-wrap has-eye">
                        <span class="icon" aria-hidden="true">
                            <!-- lock icon -->
                            <svg viewBox="0 0 24 24">
                                <path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5zm-3 8V6a3 3 0 016 0v3H9z"/>
                            </svg>
                        </span>

                        <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox" TextMode="Password"
                            placeholder="••••••••" />

                        <button type="button" class="eye-btn"
                            onclick="togglePw('<%= txtPassword.ClientID %>', this)"
                            aria-label="Show password">👁</button>
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
                    <div class="label">Confirm Password</div>

                    <div class="input-wrap has-eye">
                        <span class="icon" aria-hidden="true">
                            <!-- lock icon -->
                            <svg viewBox="0 0 24 24">
                                <path d="M12 1a5 5 0 00-5 5v3H6a2 2 0 00-2 2v9a2 2 0 002 2h12a2 2 0 002-2v-9a2 2 0 00-2-2h-1V6a5 5 0 00-5-5zm-3 8V6a3 3 0 016 0v3H9z"/>
                            </svg>
                        </span>

                        <asp:TextBox ID="txtConfirm" runat="server" CssClass="textbox" TextMode="Password"
                            placeholder="••••••••" />

                        <button type="button" class="eye-btn"
                            onclick="togglePw('<%= txtConfirm.ClientID %>', this)"
                            aria-label="Show confirm password">👁</button>
                    </div>

                    <!-- Live confirm message -->
                    <div id="confirmMsg" class="pw-confirm"></div>
                </div>


                <div class="field terms">
                    <asp:CheckBox ID="chkTerms" runat="server" />
                    <span>
                        I agree to the 
                        <a href="javascript:void(0)" class="link" onclick="openTerms()">Terms and Conditions</a>
                    </span>
                </div>

                <asp:Label ID="lblError" runat="server" CssClass="error" EnableViewState="false" />

                <asp:Button ID="btnCreate" runat="server" Text="Create Account" CssClass="btn"
                    OnClick="btnCreate_Click" />

                <div class="footer-link">
                    Already have an account?
                    <a href="Login.aspx" class="link">Sign in</a>
                </div>
                

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
        btn.setAttribute("aria-label", isHidden ? "Hide password" : "Show password");
    }

    function updatePasswordUI() {
        const pwEl = document.getElementById("<%= txtPassword.ClientID %>");
        const cfEl = document.getElementById("<%= txtConfirm.ClientID %>");
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
        const pwEl = document.getElementById("<%= txtPassword.ClientID %>");
        const cfEl = document.getElementById("<%= txtConfirm.ClientID %>");

        if (pwEl) pwEl.addEventListener("input", updatePasswordUI);
        if (cfEl) cfEl.addEventListener("input", updatePasswordUI);

        updatePasswordUI();
    });
    function setEmailMsg(text, type) {
        const msg = document.getElementById("emailMsg");
        if (!msg) return;

        msg.textContent = text || "";
        msg.classList.remove("ok", "bad");
        if (type) msg.classList.add(type);
    }

    function checkEmailExists() {
        const emailEl = document.getElementById("<%= txtEmail.ClientID %>");
        if (!emailEl) return;

        const email = (emailEl.value || "").trim();
        const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;

        if (!email) { setEmailMsg("", null); return; }
        if (!emailRegex.test(email)) {
            setEmailMsg("Please enter a valid email address.", "bad");
            return;
        }

        setEmailMsg("Checking email...", null);

        PageMethods.EmailExists(
            email,
            function (result) {
                // result is now 0 or 1 (SAFE)
                if (result === 1) {
                    setEmailMsg("This email is already registered.", "bad");
                } else {
                    setEmailMsg("Email is available ✅", "ok");
                }
            },
            function () {
                setEmailMsg("", null);
            }
        );

    }

    document.addEventListener("DOMContentLoaded", function () {
        const emailEl = document.getElementById("<%= txtEmail.ClientID %>");
        if (!emailEl) return;

        emailEl.addEventListener("blur", checkEmailExists);
        emailEl.addEventListener("input", function () {
            setEmailMsg("", null);
        });
    });
    function openTerms() {
        document.getElementById("termsModal").classList.add("show");
    }

    function closeTerms() {
        document.getElementById("termsModal").classList.remove("show");
    }
</script>
<!-- Terms & Conditions Modal -->
<div id="termsModal" class="terms-modal">
    <div class="terms-box">
        <div class="terms-header">
            <h3>EcoEats – Terms & Conditions</h3>
        </div>

        <div class="terms-body">
            <p><strong>Last updated:</strong> 2026</p>

            <p>
                By creating an account on <strong>EcoEats</strong>, you agree to comply with
                the following Terms and Conditions.
            </p>

            <h4>1. Acceptance of Terms</h4>
            <p>
                By registering for an EcoEats account, you confirm that the information provided
                is accurate and that you agree to these terms.
            </p>

            <h4>2. Platform Purpose</h4>
            <p>
                EcoEats is a platform designed to reduce food waste by connecting users with
                surplus food offerings from participating sellers.
            </p>

            <h4>3. Account Responsibilities</h4>
            <ul>
                <li>You must keep your login details confidential.</li>
                <li>You are responsible for all activities under your account.</li>
                <li>EcoEats may suspend accounts that violate these terms.</li>
            </ul>

            <h4>4. Food Safety Disclaimer</h4>
            <p>
                EcoEats does not prepare or inspect food items. Sellers are responsible for food
                safety and accuracy of listings.
            </p>

            <h4>5. Data Protection</h4>
            <p>
                Personal data is used only for platform functionality and account management.
                Passwords are securely stored.
            </p>

            <h4>6. Limitation of Liability</h4>
            <p>
                EcoEats shall not be liable for damages arising from the use of the platform.
            </p>

            <h4>7. Governing Law</h4>
            <p>
                These terms are governed by the laws of Singapore.
            </p>
        </div>

        <div class="terms-footer">
            <button type="button" class="btn terms-close" onclick="closeTerms()">Close</button>
        </div>
    </div>
</div>

</asp:Content>