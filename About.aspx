<%@ Page Title="EcoEats - About Us"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="About.aspx.cs"
    Inherits="Business_App_Dev.About" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Reuse main eco design -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- If Site.Master already has a ScriptManager, delete this line to avoid "Only one ScriptManager" error --%>
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- HERO -->
  <section class="ee-hero ee-hero-green">
    <div class="ee-container">
        <h1>About EcoEats</h1>
        <p>We connect hungry customers with surplus food from local restaurants — saving meals, money, and the planet.</p>
    </div>
</section>

    <section style="padding:60px 0 80px; max-width:980px; margin:0 auto; line-height:1.7; font-size:17px; color:#333;">

        <h2 style="font-size:28px; font-weight:700; margin-top:20px; margin-bottom:12px; color:#0d6b3b;">
            Why EcoEats?
        </h2>
        <p style="margin-bottom:18px;">
            EcoEats is a food-saving app that connects customers with local stores offering surplus
            food at discounted prices. Instead of letting perfectly good food go to waste, EcoEats
            helps users discover affordable, last-minute deals while supporting a more sustainable
            food system.
        </p>

        <h2 style="font-size:28px; font-weight:700; margin-top:45px; margin-bottom:12px; color:#0d6b3b;">
            Our Mission
        </h2>
        <p style="margin-bottom:18px;">
            Our mission is to reduce food waste in Singapore by making it easy and rewarding for
            people to rescue surplus food. By partnering with restaurants, cafés, and bakeries,
            EcoEats helps businesses sell extra meals while giving customers budget-friendly options.
        </p>

        <h2 style="font-size:28px; font-weight:700; margin-top:45px; margin-bottom:12px; color:#0d6b3b;">
            Our Story
        </h2>
        <p style="margin-bottom:18px;">
            EcoEats was created by students who noticed how much food was thrown away at the end of
            each day. We built a platform where users can browse nearby offers, reserve meals in a
            few taps, and pick them up before closing time. Every rescued meal proves that small
            choices create big impact.
        </p>

        <h2 style="font-size:28px; font-weight:700; margin-top:45px; margin-bottom:12px; color:#0d6b3b;">
            Sustainability Tips
        </h2>
        <p style="margin-bottom:12px;">
            Boost your eco-impact by following these simple steps:
        </p>

        <ul style="padding-left:20px; margin-top:12px;">
            <li style="margin-bottom:10px; font-size:17px;">
                🛍️ <strong>Bring your own containers</strong> to reduce single-use packaging
            </li>
            <li style="margin-bottom:10px; font-size:17px;">
                🍽️ <strong>Rescue only what you can finish</strong> or share with friends
            </li>
            <li style="margin-bottom:10px; font-size:17px;">
                🚶‍♀️ <strong>Choose nearby stores</strong> to cut down transport emissions
            </li>
            <li style="margin-bottom:10px; font-size:17px;">
                ⭐ <strong>Leave reviews</strong> to support sustainable sellers
            </li>
            <li style="margin-bottom:10px; font-size:17px;">
                📣 <strong>Spread the word</strong> to fight food waste together
            </li>
        </ul>

    </section>

</asp:Content>