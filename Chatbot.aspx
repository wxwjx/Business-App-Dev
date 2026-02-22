<%@ Page Title="EcoEats - Chat with Eco"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    Async="true"
    MaintainScrollPositionOnPostback="true"
    CodeBehind="Chatbot.aspx.cs"
    Inherits="Business_App_Dev.Chatbot" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
        html, body { height: 100%; margin: 0; }
        body { background: #f4efe6; font-family: 'Inter', Arial, sans-serif; }

        .ee-chat-shell {
            min-height: calc(100vh - 64px);
            display: flex;
            flex-direction: column;
        }

        .ee-chat-area {
            flex: 1;
            padding: 28px;
            box-sizing: border-box;
        }

        .chatCard {
            width: 100%;
            height: 100%;
            border-radius: 26px;
            background: white;
            overflow: hidden;
            box-shadow: 0 10px 30px rgba(0,0,0,0.10);
            display: flex;
            flex-direction: column;
        }

        .chatHeader {
            background: linear-gradient(90deg,#21a79e,#16967f);
            color: white;
            padding: 18px 20px;
            font-weight: 700;
        }

        .chatHeader small {
            display: block;
            font-weight: 400;
            opacity: .9;
            margin-top: 3px;
        }

        .chatTip {
            padding: 10px 16px;
            background: #ffffff;
            border-bottom: 1px solid #eef3f2;
            font-size: 13px;
            color: #2d3a38;
        }

        .chatBody {
            flex: 1;
            padding: 16px;
            overflow: auto;
            background: #f7fbfa;
        }

        .msg {
            max-width: 78%;
            padding: 12px 14px;
            border-radius: 14px;
            margin: 10px 0;
            line-height: 1.35;
            font-size: 14px;
            white-space: pre-wrap;
            word-wrap: break-word;
        }

        .bot {
            background: white;
            border: 1px solid #e7efee;
        }

        .user {
            background: #1f8f77;
            color: white;
            margin-left: auto;
        }

        .quickRow {
            display: flex;
            gap: 10px;
            padding: 12px 16px;
            flex-wrap: wrap;
            border-top: 1px solid #eef3f2;
            background: white;
        }

        .quickBtn {
            border: 1px solid #e3e8e7;
            padding: 8px 12px;
            border-radius: 999px;
            background: #fff;
            cursor: pointer;
            font-size: 13px;
        }

        .quickBtn:hover { background: #f1fbf8; }

        .inputRow {
            display: flex;
            gap: 10px;
            padding: 14px 16px 18px;
            border-top: 1px solid #eef3f2;
            background: white;
            align-items: center;
        }

        .txt {
            flex: 1;
            border: 1px solid #e3e8e7;
            border-radius: 999px;
            padding: 12px 14px;
            outline: none;
            font-size: 14px;
        }

        .send {
            width: 46px;
            height: 46px;
            border-radius: 50%;
            border: none;
            background: #1f8f77;
            color: white;
            cursor: pointer;
            font-weight: 700;
        }

        .chatLinkBtn {
            display: inline-block;
            padding: 10px 16px;
            background: #1f8f77;
            color: white !important;
            border-radius: 999px;
            text-decoration: none;
            font-weight: 600;
            margin-top: 8px;
        }

        .chatLinkBtn:hover { background: #176b5a; }
       .chat-hero-green {
    width: 100%;
    padding: 30px 0;
    background: linear-gradient(90deg, #2f7d32, #16967f);
    color: white;
}

.chat-hero-green h1 {
    font-size: 48px;
    font-weight: 800;
    margin-bottom: 12px;
}

.chat-hero-green p {
    font-size: 18px;
    opacity: 0.9;
}
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Remove this line if your MasterPage already has ScriptManager --%>
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <!-- HERO -->
    <section class="ee-hero chat-hero-green">
    <div class="ee-container">
        <h1>Chat with Eco</h1>
        <p>Your EcoEats assistant</p>
    </div>
</section>

    <!-- CHAT -->
    <div class="ee-chat-shell">
        <div class="ee-chat-area">
            <div class="chatCard">

                <div class="chatHeader">
                    🌿 Chat with Eco
                    <small>Your EcoEats assistant</small>
                </div>

                <div class="chatBody" id="chatBody">
                    <asp:Literal ID="litChat" runat="server" />
                </div>

                <!-- Suggested Questions -->
                <div class="quickRow">
                    <asp:Button ID="qAbout" runat="server" CssClass="quickBtn"
                        Text="About EcoEats 🌿"
                        OnClick="Quick_Click"
                        CommandArgument="about ecoeats"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qOrders" runat="server" CssClass="quickBtn"
                        Text="Show my orders 📦"
                        OnClick="Quick_Click"
                        CommandArgument="show my orders"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qLatest" runat="server" CssClass="quickBtn"
                        Text="Latest order 🧾"
                        OnClick="Quick_Click"
                        CommandArgument="show my latest order"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qFeedback" runat="server" CssClass="quickBtn"
                        Text="Show my feedback ⭐"
                        OnClick="Quick_Click"
                        CommandArgument="show my feedback"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qGive" runat="server" CssClass="quickBtn"
                        Text="Give feedback ✍️"
                        OnClick="Quick_Click"
                        CommandArgument="give feedback"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qChat" runat="server" CssClass="quickBtn"
                        Text="My chat history 🕘"
                        OnClick="Quick_Click"
                        CommandArgument="show my chat history"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />

                    <asp:Button ID="qSupport" runat="server" CssClass="quickBtn"
                        Text="Admin Support 💬"
                        OnClick="Quick_Click"
                        CommandArgument="__ADMIN_SUPPORT__"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />
                </div>

                <div class="inputRow">
                    <asp:TextBox ID="txtMsg" runat="server"
                        CssClass="txt"
                        placeholder="Type a message..." />

                    <asp:Button ID="btnSend" runat="server"
                        CssClass="send"
                        Text="➤"
                        OnClick="btnSend_Click" />
                </div>

            </div>
        </div>
    </div>

    <script>
        function scrollChatToBottom() {
            var chatBody = document.querySelector('.chatBody');
            if (chatBody) chatBody.scrollTop = chatBody.scrollHeight;
        }
        window.addEventListener('load', scrollChatToBottom);
    </script>

</asp:Content>