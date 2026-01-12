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
</head>
<body>
    <form id="form1" runat="server">

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
                    <p><strong>Password:</strong> •••••••• (hidden for security)</p>
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

    </form>
</body>
</html>
