<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PremiumSuccess.aspx.cs" Inherits="Business_App_Dev.PremiumSuccess" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats - Payment Success</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Reuse EcoEats main CSS -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
        body {
            background: #f4efe6;
            font-family: 'Inter', Arial, sans-serif;
        }

        .successShell {
            padding: 34px 0 80px;
        }

        .successCard {
            background: #ffffff;
            border-radius: 22px;
            box-shadow: 0 18px 45px rgba(15, 90, 60, 0.12);
            padding: 26px 26px;
            max-width: 820px;
            margin: 0 auto;
            overflow: hidden;
            position: relative;
        }

        .successTop {
            display: flex;
            gap: 16px;
            align-items: flex-start;
            margin-bottom: 12px;
        }

        .successIcon {
            width: 54px;
            height: 54px;
            border-radius: 16px;
            background: linear-gradient(135deg,#0f9d58,#34a853);
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 26px;
            box-shadow: 0 10px 24px rgba(16, 114, 68, 0.25);
            flex: 0 0 auto;
        }

        .successTitle {
            margin: 0;
            font-size: 26px;
            font-weight: 800;
            color: #0b2f24;
            line-height: 1.2;
        }

        .successSub {
            margin: 6px 0 0;
            color: #3d4a48;
            font-size: 14px;
            line-height: 1.55;
        }

        .successStrip {
            margin-top: 16px;
            background: linear-gradient(135deg,#e8fff2,#f0fffb);
            border: 1px solid #c9f2df;
            border-radius: 16px;
            padding: 14px 16px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
        }

        .stripLeft {
            display: flex;
            flex-direction: column;
            gap: 2px;
        }

        .stripKicker {
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
            color: #1f8f77;
            font-weight: 800;
        }

        .stripValue {
            font-size: 14px;
            font-weight: 700;
            color: #0b2f24;
        }

        .btnRow {
            margin-top: 18px;
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btnPrimary {
            border: 2px solid #111827;
            background: #1f8f77;
            color: white;
            padding: 10px 16px;
            border-radius: 999px;
            font-weight: 800;
            cursor: pointer;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btnPrimary:hover { background: #176b5a; }

        .btnGhost {
            border: 2px solid #111827;
            background: #ffffff;
            color: #111827;
            padding: 10px 16px;
            border-radius: 999px;
            font-weight: 800;
            cursor: pointer;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btnGhost:hover { background: #f6f7f9; }

        .note {
            margin-top: 14px;
            font-size: 13px;
            color: #586463;
            line-height: 1.55;
        }

        /* Decorative blob */
        .blob {
            position: absolute;
            right: -120px;
            top: -120px;
            width: 260px;
            height: 260px;
            background: radial-gradient(circle at 30% 30%, rgba(31,143,119,.35), rgba(15,157,88,.10) 55%, rgba(255,255,255,0) 70%);
            pointer-events: none;
        }

        @media (max-width: 700px) {
            .successTitle { font-size: 22px; }
            .successCard { margin: 0 14px; }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <!-- TOP BAR (same as your other pages) -->
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
                <a href="Profile.aspx">Profile</a>
                <a href="About.aspx">About Us</a>
                <a href="#">Help</a>
                <a href="Feedback.aspx">Feedback</a>
                <a href="#">Rate Sellers</a>
            </nav>

            <div class="ee-actions">
                <a class="ee-icon-btn" href="#" title="Notifications">🔔</a>
                <a class="ee-icon-btn" href="Cart.aspx" title="Cart">🛒</a>
                <a class="ee-icon-btn" href="Profile.aspx" title="Account">👤</a>
            </div>

        </div>
    </header>

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>Payment Success</h1>
            <p>Welcome to Premium — thanks for supporting EcoEats 🌿</p>
        </div>
    </section>

    <!-- CONTENT -->
    <section class="ee-container successShell">
        <div class="successCard">
            <div class="blob"></div>

            <div class="successTop">
                <div class="successIcon">✅</div>

                <div>
                    <h2 class="successTitle">Payment Successful!</h2>
                    <p class="successSub">
                        Your membership has been upgraded to <b>Premium</b>.
                        Enjoy exclusive last-minute deals, extra discounts, and early access to special drops.
                    </p>
                </div>
            </div>

            <div class="successStrip">
                <div class="stripLeft">
                    <div class="stripKicker">Status</div>
                    <div class="stripValue">Premium activated</div>
                </div>

                <div class="stripLeft">
                    <div class="stripKicker">Next step</div>
                    <div class="stripValue">Return to Profile to view your membership</div>
                </div>
            </div>

            <div class="btnRow">
                <a class="btnPrimary" href="Profile.aspx">👤 Go to Profile</a>
                <a class="btnGhost" href="Product.aspx">🏠 Back to Home</a>
                <a class="btnGhost" href="Game.aspx">🎮 Play Surplus Rush</a>
            </div>

            <div class="note">
                If your Premium badge doesn’t appear immediately, refresh your Profile page once.
            </div>
        </div>
    </section>

</form>
</body>
</html>