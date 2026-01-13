<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Business_App_Dev.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>EcoEats - Customer Login</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">

        <!-- HERO -->
        <section class="ee-hero">
            <div class="ee-container">
                <h1>Customer Login</h1>
                <p>Log in to see your profile, orders, and rescued meals.</p>
            </div>
        </section>

        <!-- LOGIN CARD -->
        <section style="padding:40px 0 60px;">
            <div style="
                max-width:420px;
                margin:0 auto;
                background:#ffffff;
                padding:28px 24px;
                border-radius:16px;
                box-shadow:0 18px 40px rgba(15, 23, 42, 0.16);
                ">

                <h2 style="font-size:24px; font-weight:700; margin-bottom:4px;">Log In</h2>
                <p style="margin-bottom:20px; color:#555;">Enter your registered email and password.</p>

                <div style="margin-bottom:14px;">
                    <label for="txtLoginEmail" style="display:block; font-weight:600; margin-bottom:4px;">Email</label>
                    <asp:TextBox ID="txtLoginEmail" runat="server" TextMode="Email" Placeholder="name@example.com"
                        Style="width:100%; padding:8px 10px; border-radius:8px; border:1px solid #d4d4d4;" />
                </div>

                <div style="margin-bottom:14px;">
                    <label for="txtLoginPassword" style="display:block; font-weight:600; margin-bottom:4px;">Password</label>
                    <asp:TextBox ID="txtLoginPassword" runat="server" TextMode="Password" Placeholder="Your password"
                        Style="width:100%; padding:8px 10px; border-radius:8px; border:1px solid #d4d4d4;" />
                </div>

                <asp:Label ID="lblLoginMessage" runat="server" ForeColor="Red" />

                <asp:Button ID="btnLogin" runat="server"
                    Text="Log In"
                    OnClick="btnLogin_Click"
                    Style="margin-top:16px; width:100%; padding:10px 0; border:none; border-radius:999px; background:#16a34a; color:white; font-weight:600; cursor:pointer;" />

                <p style="margin-top:16px; font-size:14px; text-align:center;">
                    New customer?
                    <a href="SignUp.aspx" style="color:#15803d; font-weight:600; text-decoration:none;">Create an account</a>
                </p>

            </div>
        </section>

    </form>
</body>
</html>
