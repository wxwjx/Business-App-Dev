<%@ Page Title="EcoEats | Seller Sign Up" Language="C#" MasterPageFile="~/Login.Master"
    AutoEventWireup="true" CodeBehind="RegisterSeller.aspx.cs" Inherits="Business_App_Dev.RegisterSeller" %>

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
        <div class="brand-tagline">Save meals, save money, save the planet</div>
    </div>

    <div class="card">
        <div class="card-header">
            <div class="header-title">Seller Application</div>
            <div class="header-sub">Register your store to start listing surplus food</div>
        </div>

        <div class="card-body">

            <!-- Owner Name -->
            <div class="field">
                <div class="label">Owner Name</div>
                <div class="input-wrap">
                    <span class="icon" aria-hidden="true">
                        <!-- user icon -->
                        <svg viewBox="0 0 24 24">
                            <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.42 0-8 2.24-8 5v1h16v-1c0-2.76-3.58-5-8-5z"/>
                        </svg>
                    </span>
                    <asp:TextBox ID="txtOwnerName" runat="server" CssClass="textbox" placeholder="Enter owner name" OnTextChanged="txtOwnerName_TextChanged" />
                </div>
            </div>

            <!-- Store Name -->
            <div class="field">
                <div class="label">Store Name</div>
                <div class="input-wrap">
                    <span class="icon" aria-hidden="true">
                        <!-- store icon -->
                        <svg viewBox="0 0 24 24">
                            <path d="M3 9l1-5h16l1 5v2a3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1-3 3 3 3 0 0 1-3-3V9zm2 7v6h14v-6h-2v4H7v-4H5z"/>
                        </svg>
                    </span>
                    <asp:TextBox ID="txtStoreName" runat="server" CssClass="textbox" placeholder="Enter store name" />
                </div>
            </div>

            <!-- Email -->
            <div class="field">
                <div class="label">Business Email</div>
                <div class="input-wrap">
                    <span class="icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24">
                            <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5L4 8V6l8 5 8-5v2z"/>
                        </svg>
                    </span>

                    <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox"
                        TextMode="Email"
                        AutoPostBack="true"
                        OnTextChanged="txtEmail_TextChanged"
                        placeholder="business@email.com" />
                </div>

                <!-- server-side immediate message (same idea as your customer page) -->
                <asp:Label ID="lblEmailStatus" runat="server" CssClass="field-msg" />
            </div>
                        <!-- Phone -->
            <!-- Phone Number -->
            <div class="field">
                <div class="label">Phone Number</div>
                <div class="input-wrap">

                    <div class="phone-row">
                        <asp:DropDownList ID="ddlCountryCode" runat="server" CssClass="country-code">
                            <asp:ListItem Value="+65" Selected="True">🇸🇬 +65</asp:ListItem>
                            <asp:ListItem Value="+60">🇲🇾 +60</asp:ListItem>
                            <asp:ListItem Value="+62">🇮🇩 +62</asp:ListItem>
                            <asp:ListItem Value="+66">🇹🇭 +66</asp:ListItem>
                            <asp:ListItem Value="+84">🇻🇳 +84</asp:ListItem>
                        </asp:DropDownList>

                        <asp:TextBox ID="txtPhone" runat="server" CssClass="phone-textbox"
                            placeholder="Enter phone number" />
                    </div>

                </div>
            </div>

            <!-- OTP + Get OTP -->
            <div class="field">
                <div class="label">OTP</div>

                <div class="otp-row">
                    <div class="input-wrap otp-input">
                        <asp:TextBox ID="txtOtp" runat="server" CssClass="textbox" placeholder="Enter OTP" />
                    </div>

                    <asp:Button ID="btnGetOtp" runat="server" Text="Get OTP" CssClass="otp-btn"
                        OnClick="btnGetOtp_Click" UseSubmitBehavior="false" />
                </div>

                <asp:Label ID="lblOtpMsg" runat="server" CssClass="field-msg" EnableViewState="false" />
            </div>


            <!-- Address -->
            <div class="field">
                <div class="label">Address</div>
                <div class="input-wrap">
                    <span class="icon" aria-hidden="true">
                        <!-- location icon -->
                        <svg viewBox="0 0 24 24">
                            <path d="M12 2a7 7 0 0 0-7 7c0 5.25 7 13 7 13s7-7.75 7-13a7 7 0 0 0-7-7zm0 9.5A2.5 2.5 0 1 1 12 6.5a2.5 2.5 0 0 1 0 5z"/>
                        </svg>
                    </span>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="textbox" placeholder="Block, street, unit number" />
                </div>
            </div>


            <!-- Food Categories -->
            <div class="field">
                <div class="label">Food Categories (Select one or more)</div>

                <!-- IMPORTANT: no input-wrap here -->
                <div class="cat-wrap">
                    <asp:CheckBoxList
                        ID="cblCategories"
                        runat="server"
                        CssClass="cat-pills"
                        RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Text="Baked Goods" Value="Baked Goods" />
                        <asp:ListItem Text="Cooked Meals" Value="Cooked Meals" />
                        <asp:ListItem Text="Fruits & Vegetables" Value="Fruits & Vegetables" />
                        <asp:ListItem Text="Dairy" Value="Dairy" />
                        <asp:ListItem Text="Beverages" Value="Beverages" />
                        <asp:ListItem Text="Snacks" Value="Snacks" />
                        <asp:ListItem Text="Halal" Value="Halal" />
                        <asp:ListItem Text="Vegetarian" Value="Vegetarian" />
                    </asp:CheckBoxList>
                </div>

                <div class="hint">Pick all that apply.</div>
            </div>




            <!-- Password -->
            <div class="field">
                <div class="label">Password</div>

                <div class="input-wrap has-eye">
                    <span class="icon" aria-hidden="true">
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

                <div id="pwRules" class="pw-rules">
                    <div id="ruleLen" class="rule">• At least 8 characters</div>
                    <div id="ruleLetter" class="rule">• At least 1 letter (A–Z)</div>
                    <div id="ruleNum" class="rule">• At least 1 number (0–9)</div>
                    <div id="ruleSpecial" class="rule">• At least 1 special character (!@#$...)</div>
                </div>
            </div>

            <!-- Confirm Password -->
            <div class="field">
                <div class="label">Confirm Password</div>

                <div class="input-wrap has-eye">
                    <span class="icon" aria-hidden="true">
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

                <div id="confirmMsg" class="pw-confirm"></div>
            </div>

            <!-- Terms -->
            <div class="field terms">
                <asp:CheckBox ID="chkTerms" runat="server" />
                <span>
                    I agree to the
                    <a href="javascript:void(0)" class="link" onclick="openTerms()">Terms and Conditions</a>
                </span>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error" EnableViewState="false" />

            <asp:Button ID="btnCreate" runat="server" Text="Submit Seller Application" CssClass="btn"
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

    function openTerms() {
        document.getElementById("termsModal").classList.add("show");
    }
    function closeTerms() {
        document.getElementById("termsModal").classList.remove("show");
    }
</script>

<!-- Reuse your same Terms Modal -->
<div id="termsModal" class="terms-modal">
    <div class="terms-box">
        <div class="terms-header">
            <h3>EcoEats – Terms & Conditions</h3>
        </div>

        <div class="terms-body">
            <p><strong>Last updated:</strong> 2026</p>
            <p>By creating an account on <strong>EcoEats</strong>, you agree to comply with the following Terms and Conditions.</p>

            <h4>Seller Responsibilities</h4>
            <ul>
                <li>Sellers must provide accurate business information.</li>
                <li>Sellers are responsible for food safety and correct listings.</li>
                <li>EcoEats may reject or suspend seller accounts for violations.</li>
            </ul>

            <h4>Platform Use</h4>
            <p>EcoEats connects sellers with customers to reduce food waste through surplus food listings.</p>

            <h4>Data Protection</h4>
            <p>Personal/business data is used only for platform operations. Passwords are securely stored.</p>
        </div>

        <div class="terms-footer">
            <button type="button" class="btn terms-close" onclick="closeTerms()">Close</button>
        </div>
    </div>
</div>

</asp:Content>