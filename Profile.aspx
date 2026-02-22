<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Business_App_Dev.Profile" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats - Profile</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Reuse EcoEats main CSS -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <script type="text/javascript">
        function byId(id) { return document.getElementById(id); }

        // ===== Delete modal =====
        function openDeleteModal() {
            byId("eeDeleteError").style.display = "none";
            byId("eeDeleteInput").value = "";
            byId("eeDeleteModal").style.display = "flex";
            setTimeout(() => byId("eeDeleteInput").focus(), 0);
        }
        function closeDeleteModal() { byId("eeDeleteModal").style.display = "none"; }
        function submitDeleteIfValid() {
            const v = (byId("eeDeleteInput").value || "").trim();
            if (v !== "DELETE") {
                byId("eeDeleteError").style.display = "block";
                byId("eeDeleteInput").focus();
                return;
            }
            closeDeleteModal();
            __doPostBack('<%= btnDeleteAccount.UniqueID %>', '');
        }

        // ===== Logout modal =====
        function openLogoutModal() { byId("eeLogoutModal").style.display = "flex"; }
        function closeLogoutModal() { byId("eeLogoutModal").style.display = "none"; }
        function submitLogout() {
            closeLogoutModal();
            if (typeof __doPostBack !== "function") {
                alert("Postback not available. Move ScriptManager to top of form.");
                return;
            }
            __doPostBack('<%= btnLogout.UniqueID %>', '');
        }

        // close modal if click outside
        document.addEventListener("click", function (e) {
            if (e.target && e.target.id === "eeDeleteModal") closeDeleteModal();
            if (e.target && e.target.id === "eeLogoutModal") closeLogoutModal();
        });

        // ESC to close
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") {
                closeDeleteModal();
                closeLogoutModal();
            }
        });

        // ===== Password UI =====
        function togglePw(which) {
            var pwEl = document.getElementById("<%= txtPassword.ClientID %>");
            var cfEl = document.getElementById("<%= txtConfirm.ClientID %>");
            var target = (which === "pw") ? pwEl : cfEl;
            if (!target) return;
            target.type = (target.type === "password") ? "text" : "password";
        }

        function updatePasswordUI() {
            const pwEl = document.getElementById("<%= txtPassword.ClientID %>");
            if (!pwEl) return;

            const pw = pwEl.value || "";

            const hasLen = pw.length >= 8;
            const hasLetter = /[A-Za-z]/.test(pw);
            const hasNum = /[0-9]/.test(pw);
            const hasSpecial = /[^A-Za-z0-9]/.test(pw);

            const r1 = byId("ruleLen");
            const r2 = byId("ruleLetter");
            const r3 = byId("ruleNum");
            const r4 = byId("ruleSpecial");

            if (r1) r1.classList.toggle("ok", hasLen);
            if (r2) r2.classList.toggle("ok", hasLetter);
            if (r3) r3.classList.toggle("ok", hasNum);
            if (r4) r4.classList.toggle("ok", hasSpecial);
        }

        window.addEventListener("load", updatePasswordUI);
    </script>

    <!-- ✅ ADDED: Surplus Rush banner styles (ONLY ADDITION) -->
    <style>
        /* ========================= */
        /* 🎮 SURPLUS RUSH BANNER     */
        /* ========================= */
        .surplusRushCard {
            display: block;
            text-decoration: none;
            border-radius: 24px;
            overflow: hidden;
            position: relative;
            transition: all .35s ease;
            box-shadow: 0 20px 50px rgba(16, 114, 68, 0.18);
        }

        .sr-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 34px 36px;
            background: linear-gradient(135deg,#16967f,#21a79e,#1b9145);
            color: white;
            position: relative;
        }

        .sr-left h2 {
            font-size: 34px;
            margin: 8px 0 8px;
            font-weight: 800;
        }

        .sr-left p {
            font-size: 15px;
            opacity: .95;
            max-width: 520px;
            line-height: 1.5;
            margin: 0;
        }

        .sr-badge {
            background: rgba(255,255,255,0.20);
            padding: 6px 14px;
            border-radius: 999px;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
            display: inline-block;
            font-weight: 700;
        }

        .sr-cta {
            margin-top: 16px;
            display: inline-block;
            padding: 10px 22px;
            background: white;
            color: #16967f;
            border-radius: 999px;
            font-weight: 800;
            transition: all .3s ease;
        }

        .sr-right {
            display: flex;
            gap: 14px;
            align-items: center;
        }

        .sr-icon {
            font-size: 54px;
            animation: srFloat 3s ease-in-out infinite;
        }

        .sr-icon.small {
            font-size: 34px;
            opacity: .85;
        }

        @keyframes srFloat {
            0% { transform: translateY(0px); }
            50% { transform: translateY(-8px); }
            100% { transform: translateY(0px); }
        }

        /* Hover */
        .surplusRushCard:hover {
            transform: translateY(-6px);
            box-shadow: 0 28px 60px rgba(16, 114, 68, 0.35);
        }

        .surplusRushCard:hover .sr-cta {
            background: #0f9d58;
            color: white;
        }

        /* Mobile */
        @media (max-width: 700px) {
            .sr-content { flex-direction: column; align-items: flex-start; gap: 14px; }
            .sr-right { align-self: flex-end; }
            .sr-left h2 { font-size: 28px; }
        }
        .pw-wrap { margin-top: 12px; }
        .pw-field { position: relative; }
        .pw-input {
            width: 100%;
            padding: 12px 44px 12px 14px;
            border: 1px solid rgba(0,0,0,.12);
            border-radius: 14px;
            font-weight: 700;
            outline: none;
        }
        .pw-eye {
            position: absolute;
            right: 10px;
            top: 50%;
            transform: translateY(-50%);
            border: none;
            background: transparent;
            cursor: pointer;
            font-size: 18px;
            opacity: .75;
        }
        .pw-rules { margin-top: 10px; display: grid; gap: 6px; }
        .pw-rule {
            font-size: 13px;
            color: #666;
            padding-left: 18px;
            position: relative;
        }
        .pw-rule::before {
            content: "•";
            position: absolute;
            left: 6px;
            top: 0;
            opacity: .8;
        }
        .pw-rule.ok { color: #0f9d58; font-weight: 800; }
        .pw-confirm-full {
            width: 100%;
            margin-top: 12px;
            border-radius: 999px;
            padding: 10px 16px;
            font-weight: 800;
        }
        .pw-msg {
            display: block;
            margin-top: 10px;
            padding: 10px 12px;
            border-radius: 12px;
            font-weight: 700;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- TOP BAR (same as Product page) -->
        <header class="ee-topbar">
            <div class="ee-container ee-topbar-inner">

                <div class="ee-brand">
                    <div class="ee-logo">
                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M19 3c-6.5.7-11 3.8-13.8 7.2C2.6 13.4 2.2 17.2 4 21c3.8 1.8 7.6 1.4 10.8-1.2C18.2 17 21.3 12.5 22 6c.1-1.2-.7-2.9-3-3z"></path>
                        </svg>
                    </div>
                    <div class="ee-brand-name">EcoEats</div>
                </div>

                <div class="ee-search">
                    <input type="text" placeholder="Search for meals, restaurants..." />
                </div>

                <nav class="ee-nav">
                    <a href="Product.aspx">Home</a>
                    <a href="Order.aspx">Orders</a>
                    <a class="active" href="Profile.aspx">Profile</a>
                    <a href="About.aspx">About Us</a>
                    <a href="#">Help</a>
                    <a href="Feedback.aspx">Feedback</a>
                    <a href="#">Rate Sellers</a>
                </nav>

                <div class="ee-actions">
                    <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                    <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                    <!-- Account icon goes to Profile -->
                    <a class="ee-icon-btn" href="Profile.aspx" title="Account">👤</a>
                </div>

            </div>
        </header>

        <!-- HERO BANNER -->
        <section class="ee-hero">
            <div class="ee-container">
                <h1>Your EcoEats Profile</h1>
                <p>View your account details and manage your membership.</p>
            </div>
        </section>

        <!-- ✅ ADDED: Surplus Rush banner (ONLY ADDITION) -->
        <section class="ee-container" style="margin-top:16px; margin-bottom:20px;">
            <a href="Game.aspx" class="surplusRushCard" aria-label="Go to Surplus Rush game">
                <div class="sr-content">
                    <div class="sr-left">
                        <div class="sr-badge">🎮 EcoEats Game</div>
                        <h2>Surplus Rush</h2>
                        <p>
                            Rescue edible surplus and reduce food waste. Score <b>50+</b> points to unlock exclusive vouchers.
                        </p>
                        <div class="sr-cta">Play Now →</div>
                    </div>

                    <div class="sr-right" aria-hidden="true">
                        <div class="sr-icon">🍱</div>
                        <div class="sr-icon small">🥗</div>
                        <div class="sr-icon small">🍕</div>
                    </div>
                </div>
            </a>
        </section>

        <!-- MAIN PROFILE LAYOUT -->
        <section class="ee-container" style="padding: 40px 0 70px; display:flex; flex-wrap:wrap; gap:32px;">

            <!-- LEFT: ACCOUNT INFO -->
            <div style="
                flex:1 1 280px;
                max-width: 420px;
                background:#ffffff;
                border-radius:18px;
                box-shadow:0 18px 45px rgba(15, 90, 60, 0.12);
                padding:24px 28px;
            ">
                <h2 style="font-size:24px; font-weight:700; margin-bottom:10px;">Account Details</h2>
                <p style="margin:0 0 18px; color:#666;">These are the details you used to sign up.</p>

                <div style="display:flex; align-items:center; gap:14px; margin-bottom:22px;">
                    <div style="
                        width:48px; height:48px; border-radius:999px;
                        background:linear-gradient(135deg,#0f9d58,#34a853);
                        display:flex; align-items:center; justify-content:center;
                        font-size:24px; color:#fff;
                    ">
                        👤
                    </div>
                    <div>
                        <div style="font-weight:600; font-size:18px;">
                            <asp:Label ID="lblFullName" runat="server" Text=""></asp:Label>
                        </div>
                        <div style="font-size:14px; color:#777;">
                            <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>

                <div style="font-size:14px; color:#555; line-height:1.7;">
                    <p><strong>User ID:</strong> <asp:Label ID="lblUserId" runat="server" Text=""></asp:Label></p>
                    <p style="margin-bottom:8px;">
                        <strong>Password:</strong>
                        <asp:Label ID="lblPassword" runat="server" Text="•••••••• (secured)"></asp:Label>
                    </p>

                    <div class="pw-wrap">

                        <!-- New Password -->
                        <div class="pw-field">
                            <asp:TextBox ID="txtPassword" runat="server"
                                TextMode="Password"
                                CssClass="pw-input"
                                placeholder="New password"
                                onkeyup="updatePasswordUI()"
                                onchange="updatePasswordUI()" />
                            <button type="button" class="pw-eye" onclick="togglePw('pw')">👁</button>
                        </div>

                        <!-- Confirm Password -->
                        <div class="pw-field" style="margin-top:10px;">
                            <asp:TextBox ID="txtConfirm" runat="server"
                                TextMode="Password"
                                CssClass="pw-input"
                                placeholder="Confirm new password"
                                onkeyup="updatePasswordUI()"
                                onchange="updatePasswordUI()" />
                            <button type="button" class="pw-eye" onclick="togglePw('cf')">👁</button>
                        </div>

                        <!-- Rules -->
                        <div class="pw-rules">
                            <div class="pw-rule" id="ruleLen">At least 8 characters</div>
                            <div class="pw-rule" id="ruleLetter">Contains a letter (A–Z)</div>
                            <div class="pw-rule" id="ruleNum">Contains a number (0–9)</div>
                            <div class="pw-rule" id="ruleSpecial">Contains a special character (!@#...)</div>
                        </div>

                        <!-- Confirm Button -->
                        <asp:Button ID="btnUpdatePassword" runat="server"
                            Text="Confirm Password Change"
                            OnClick="btnUpdatePassword_Click"
                            CssClass="ee-btn-primary pw-confirm-full" />

                        <!-- Message -->
                        <asp:Label ID="lblPwdMsg" runat="server"
                            CssClass="pw-msg"
                            Style="display:none;"
                            EnableViewState="false"></asp:Label>

                    </div>

                    <!-- Buttons row: left = delete, right = logout -->
                    <div style="display:flex; justify-content:space-between; align-items:center; gap:12px; margin-top:6px;">
                        <asp:Button ID="btnDeleteAccount" runat="server"
                            Text="Delete Account"
                            OnClick="btnDeleteAccount_Click"
                            OnClientClick="openDeleteModal(); return false;"
                            CssClass="ee-btn-danger"
                            Style="border-radius:999px; padding:9px 16px; font-size:14px;" />

                        <asp:Button ID="btnLogout" runat="server"
                            Text="Log Out"
                            OnClick="btnLogout_Click"
                            OnClientClick="openLogoutModal(); return false;"
                            CssClass="ee-btn-secondary"
                            Style="border-radius:999px; padding:9px 16px; font-size:14px;" />
                    </div>

                    <!-- feedback message -->
                    <asp:Label ID="lblAccountActionMsg" runat="server"
                        Style="display:block; margin-top:10px; font-size:13px;"
                        ForeColor="#d93025"></asp:Label>
                </div>

            </div>

            <!-- RIGHT: MEMBERSHIP CARD -->
            <div style="
                flex:1 1 280px;
                max-width: 460px;
                display:flex;
                flex-direction:column;
                gap:18px;
            ">
                <div style="
                    background:linear-gradient(135deg,#1b9145,#21b36b,#1ab3b3);
                    border-radius:20px;
                    padding:22px 24px;
                    color:white;
                    box-shadow:0 18px 45px rgba(16, 114, 68, 0.35);
                    position:relative;
                    overflow:hidden;
                ">
                    <div style="font-size:14px; opacity:0.9; text-transform:uppercase; letter-spacing:0.09em;">EcoEats Membership</div>
                    <h2 style="margin-top:8px; margin-bottom:6px; font-size:26px;">
                        <asp:Label ID="lblMembershipTitle" runat="server" Text=""></asp:Label>
                    </h2>
                    <p style="margin:0 0 14px; font-size:14px;">
                        <asp:Label ID="lblMembershipSubtitle" runat="server" Text=""></asp:Label>
                    </p>

                    <div style="display:flex; justify-content:space-between; align-items:flex-end; gap:16px;">
                        <div>
                            <div style="font-size:12px; text-transform:uppercase; opacity:0.8;">Member since</div>
                            <div style="font-size:15px; font-weight:600;">
                                <asp:Label ID="lblMemberSince" runat="server" Text="-"></asp:Label>
                            </div>
                        </div>
                        <div style="text-align:right;">
                            <div style="font-size:12px; text-transform:uppercase; opacity:0.8;">Status</div>
                            <div style="font-size:15px; font-weight:600;">
                                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div style="margin-top:18px; display:flex; gap:10px; flex-wrap:wrap;">
                        <asp:Button ID="btnUpgrade" runat="server"
                                    Text="Upgrade to Premium - $4.99/month"
                                    OnClick="btnUpgrade_Click"
                                    CssClass="ee-btn-primary"
                                    Style="border-radius:999px; padding:9px 18px; font-size:14px;" />

                        <asp:Label ID="lblMessage" runat="server"
                                   ForeColor="White"
                                   Style="font-size:13px;"></asp:Label>
                    </div>
                </div>

                <div style="
                    background:#ffffff;
                    border-radius:16px;
                    padding:18px 20px;
                    font-size:14px;
                    color:#444;
                    box-shadow:0 10px 28px rgba(0,0,0,0.06);
                ">
                    <h3 style="margin-top:0; font-size:16px; font-weight:600;">Premium perks</h3>
                    <ul style="margin:8px 0 0 18px; padding:0;">
                        <li>Exclusive last-minute deals on surplus meals</li>
                        <li>Extra discounts and early access to special drops</li>
                        <li>Priority support and sustainability reports</li>
                    </ul>
                </div>
            </div>

        </section>

        <!-- ===== Delete Account Modal ===== -->
        <div id="eeDeleteModal" class="ee-modal-overlay" style="display:none;">
            <div class="ee-modal">
                <div class="ee-modal-head">
                    <div class="ee-modal-title">Delete account</div>
                    <button type="button" class="ee-modal-x" onclick="closeDeleteModal()">✕</button>
                </div>

                <div class="ee-modal-body">
                    <p class="ee-modal-text">
                        This action is permanent. To confirm, please type <strong>DELETE</strong>.
                    </p>

                    <input id="eeDeleteInput" class="ee-input" type="text" placeholder="Type DELETE" />

                    <div id="eeDeleteError" class="ee-error" style="display:none;">
                        You must type DELETE exactly.
                    </div>
                </div>

                <div class="ee-modal-actions">
                    <button type="button" class="ee-btn-secondary" onclick="closeDeleteModal()">Cancel</button>

                    <!-- IMPORTANT: use a normal HTML button to trigger the ASP.NET button click -->
                    <button type="button" class="ee-btn-danger" onclick="submitDeleteIfValid()">Delete</button>
                </div>
            </div>
        </div>

        <!-- ===== Logout Modal ===== -->
        <div id="eeLogoutModal" class="ee-modal-overlay" style="display:none;">
            <div class="ee-modal">
                <div class="ee-modal-head">
                    <div class="ee-modal-title">Log out</div>
                    <button type="button" class="ee-modal-x" onclick="closeLogoutModal()">✕</button>
                </div>

                <div class="ee-modal-body">
                    <p class="ee-modal-text">Do you really want to log out?</p>
                </div>

                <div class="ee-modal-actions">
                    <button type="button" class="ee-btn-secondary" onclick="closeLogoutModal()">Cancel</button>
                    <button type="button" class="ee-btn-primary" onclick="submitLogout()">Log out</button>
                </div>
            </div>
        </div>

    </form>
</body>
</html>