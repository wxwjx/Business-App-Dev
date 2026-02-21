<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Game.aspx.cs" Inherits="Business_App_Dev.Game" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Surplus Rush - EcoEats</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- EcoEats CSS (same as Chatbot.aspx) -->
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
        body {
            background: #f4efe6;
            font-family: 'Inter', Arial, sans-serif;
            margin: 0;
        }

        .g-wrap {
            max-width: 900px;
            margin: 0 auto;
            padding: 22px 14px 60px;
        }

        .board {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 18px;
            box-shadow: 0 10px 28px rgba(0,0,0,.08);
            padding: 18px;
        }

        .itemBox {
            width: 100%;
            border: 2px solid #111827;
            border-radius: 10px;
            padding: 14px 16px;
            font-weight: 900;
            font-size: 20px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            gap: 12px;
        }

        .emo { font-size: 26px }

        .btnRow {
            margin-top: 18px;
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .bigBtn {
            width: 100%;
            border-radius: 999px;
            padding: 16px 18px;
            font-weight: 900;
            font-size: 16px;
            cursor: pointer;
            border: 2px solid #111827;
            background: #fff;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .bigBtn:active { transform: translateY(1px) }

        .btn-discount { background: #FFF7ED; border-color: #F59E0B }
        .btn-waste { background: #FEF2F2; border-color: #EF4444 }

        .bigBtn[disabled] {
            opacity: .55;
            cursor: not-allowed;
            transform: none;
        }

        .bottomRow {
            margin-top: 18px;
            display: flex;
            gap: 12px;
            justify-content: space-between;
            flex-wrap: wrap;
        }

        .bubble {
            flex: 1 1 240px;
            border: 2px solid #111827;
            border-radius: 999px;
            padding: 12px 16px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-weight: 900;
        }

        .bubble small { font-weight: 700; color: #374151 }

        .meter {
            width: 55%;
            height: 10px;
            border-radius: 999px;
            background: #e5e7eb;
            overflow: hidden;
            border: 1px solid #d1d5db;
            margin-left: 10px;
        }

        .meterFill {
            height: 100%;
            width: 0%;
            background: linear-gradient(90deg,#fb7185,#ef4444);
            border-radius: 999px;
            transition: width .2s ease;
        }

        .dropArea {
            position: relative;
            height: 220px;
            margin-top: 16px;
            border-radius: 14px;
            background: linear-gradient(180deg,#ffffff,#f8fafc);
            border: 1px dashed #e5e7eb;
            overflow: hidden;
        }

        .falling {
            position: absolute;
            left: 50%;
            transform: translateX(-50%);
            top: -80px;
            width: min(520px,92%);
            border-radius: 14px;
            border: 1px solid #e5e7eb;
            background: #fff;
            box-shadow: 0 12px 26px rgba(0,0,0,.10);
            padding: 10px 12px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            gap: 10px;
            animation: drop 5.5s linear forwards;
        }

        @keyframes drop {
            from { top: -80px; opacity: 1; }
            to { top: 150px; opacity: 1; }
        }

        .feedback {
            margin-top: 14px;
            display: none;
            padding: 10px 12px;
            border-radius: 12px;
            border: 1px solid #e5e7eb;
            font-size: 13px;
            font-weight: 800;
        }

        .endPanel {
            margin-top: 18px;
            border: 2px solid #111827;
            border-radius: 14px;
            padding: 14px 16px;
            display: none;
        }

        .endTitle { font-size: 20px; font-weight: 900; margin: 0 0 8px 0 }
        .endText  { font-size: 14px; font-weight: 800; color: #111827; line-height: 1.45 }

        .endBtns {
            margin-top: 12px;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            align-items: center;
        }

        .plainBtn {
            border-radius: 10px;
            padding: 10px 12px;
            font-weight: 900;
            border: 1px solid #d1d5db;
            background: #f8fafc;
            cursor: pointer;
        }

        .plainBtn[disabled] { opacity: .6; cursor: not-allowed }

        /* ✅ RESULT POPUP MODAL */
        .eeWinOverlay {
            position: fixed !important;
            inset: 0 !important;
            background: rgba(0,0,0,.45) !important;
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 2147483647 !important;
            padding: 16px;
        }

        .eeWinBox {
            width: min(520px, 96%);
            background: #ffffff !important;
            border-radius: 16px;
            border: 2px solid #111827;
            box-shadow: 0 18px 50px rgba(0,0,0,.25);
            padding: 16px 16px;
            text-align: center;
        }

        .eeWinTitle { margin: 0; font-size: 22px; font-weight: 900; color: #111827 }
        .eeWinText  { margin: 10px 0 0; font-size: 14px; font-weight: 800; color: #111827; line-height: 1.4 }

        .eeWinBtns { margin-top: 14px; display: flex; justify-content: center; gap: 10px; flex-wrap: wrap; }
        .eeWinBtnPrimary {
            border-radius: 999px;
            padding: 10px 14px;
            font-weight: 900;
            cursor: pointer;
            border: 2px solid #111827;
            background: #ECFDF5;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <!-- ✅ TOP BAR (same as Chatbot.aspx) -->
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
                <h1>Surplus Rush</h1>
            </div>
        </section>

        <!-- GAME -->
        <div class="g-wrap">
            <div class="board">

                <div class="itemBox">
                    <span class="emo" id="foodEmoji">🍱</span>
                    <span id="foodName">Chicken Rice Bento (24hrs expiry)</span>
                </div>

                <div class="dropArea" id="dropArea"></div>

                <div class="btnRow">
                    <button type="button" class="bigBtn btn-discount" id="btnDiscount">
                        <span>🟡 Discount Food / Rescue</span>
                        <span style="font-weight: 900;">Edible surplus</span>
                    </button>

                    <button type="button" class="bigBtn btn-waste" id="btnWaste">
                        <span>🔴 Waste (Throw)</span>
                        <span style="font-weight: 900;">Unsafe only</span>
                    </button>
                </div>

                <div class="feedback" id="feedback"></div>

                <div class="bottomRow">
                    <div class="bubble">
                        <span>Waste Meter:</span>
                        <div class="meter"><div class="meterFill" id="wasteFill"></div></div>
                        <small><span id="wastePct">0</span>%</small>
                    </div>

                    <div class="bubble">
                        <span>Time:</span>
                        <small style="font-size: 16px;"><span id="timeLeft">20</span>s</small>
                    </div>
                </div>

                <div class="endPanel" id="endPanel">
                    <div class="endTitle" id="endTitle">Round Complete</div>
                    <div class="endText" id="endText"></div>

                    <div class="endBtns">
                        <asp:Button ID="btnSaveResult" runat="server"
                            Text="Save Result & Claim Rewards"
                            OnClick="btnSaveResult_Click"
                            CssClass="plainBtn"
                            CausesValidation="false"
                            UseSubmitBehavior="true"
                            OnClientClick="this.value='Saving...';" />
                        <asp:Label ID="lblServerMsg" runat="server" />
                    </div>
                </div>

                <!-- Hidden fields -->
                <asp:HiddenField ID="hfScore" runat="server" />
                <asp:HiddenField ID="hfSaved" runat="server" />
                <asp:HiddenField ID="hfWasted" runat="server" />
                <asp:HiddenField ID="hfWon" runat="server" />
                <asp:HiddenField ID="hfLocked" runat="server" />
                <asp:HiddenField ID="hfFinalized" runat="server" />

            </div>
        </div>

        <!-- RESULT MODAL -->
        <div class="eeWinOverlay" id="winModal">
            <div class="eeWinBox">
                <h3 class="eeWinTitle" id="winTitle">Result</h3>
                <p class="eeWinText" id="winModalText"></p>
                <div class="eeWinBtns">
                    <button class="eeWinBtnPrimary" type="button" onclick="closeWinModal()">OK</button>
                </div>
            </div>
        </div>

        <script>
            window.closeWinModal = function () {
                document.getElementById("winModal").style.display = "none";
            };

            window.openResultModal = function (title, html) {
                const winModal = document.getElementById("winModal");
                document.getElementById("winTitle").textContent = title || "Result";
                document.getElementById("winModalText").innerHTML = html || "";
                winModal.style.display = "flex";
            };

            (() => {
                const foodEmoji = document.getElementById("foodEmoji");
                const foodName = document.getElementById("foodName");
                const dropArea = document.getElementById("dropArea");
                const btnDiscount = document.getElementById("btnDiscount");
                const btnWaste = document.getElementById("btnWaste");
                const feedback = document.getElementById("feedback");
                const wasteFill = document.getElementById("wasteFill");
                const wastePct = document.getElementById("wastePct");
                const timeEl = document.getElementById("timeLeft");
                const endPanel = document.getElementById("endPanel");
                const endTitle = document.getElementById("endTitle");
                const endText = document.getElementById("endText");

                const hfScore = document.getElementById("<%= hfScore.ClientID %>");
                const hfSaved = document.getElementById("<%= hfSaved.ClientID %>");
                const hfWasted = document.getElementById("<%= hfWasted.ClientID %>");
                const hfWon = document.getElementById("<%= hfWon.ClientID %>");
                const hfLocked = document.getElementById("<%= hfLocked.ClientID %>");

                const locked = (hfLocked && hfLocked.value === "1");
                const alreadySaved = (hfFinalized && hfFinalized.value === "1"); // if you set it server-side

                if (locked) {
                    btnDiscount.disabled = true;
                    btnWaste.disabled = true;
                    endPanel.style.display = "block";
                    endTitle.textContent = "⛔ Daily Limit";
                    endText.innerHTML = "You can only play <b>once a day</b>. Please come back tomorrow.";
                    window.openResultModal("⛔ Daily Limit", "You can only play <b>once a day</b>.<br/>Please come back tomorrow.");
                    return;
                }

                const edible = [
                    { emoji: "🍱", name: "Chicken Rice Bento (24hrs expiry)" },
                    { emoji: "🍛", name: "Curry Rice Set (24hrs expiry)" },
                    { emoji: "🍜", name: "Stir-fry Noodles (end of day)" },
                    { emoji: "🍝", name: "Creamy Pasta (end of day)" },
                    { emoji: "🍕", name: "Pizza Slices (end of day)" },
                    { emoji: "🥪", name: "Cafe Sandwich (24hrs expiry)" },
                    { emoji: "🍣", name: "Sushi Pack (same day)" }
                ];

                const unsafe = [
                    { emoji: "🥫", name: "Leaking Can" },
                    { emoji: "🐟", name: "Spoiled Fish" },
                    { emoji: "🧀", name: "Moldy Cheese" },
                    { emoji: "🥛", name: "Expired Milk (unsafe)" }
                ];

                let timeLeft = 20;
                let score = 0;
                let saved = 0;
                let wasted = 0;
                let wasteMeter = 0;

                let current = null;
                let currentType = null;
                let currentEl = null;
                let over = false;

                function updateBottom() {
                    timeEl.textContent = timeLeft;
                    const pct = Math.max(0, Math.min(100, wasteMeter));
                    wastePct.textContent = pct;
                    wasteFill.style.width = pct + "%";
                }

                function showFeedback(ok, text) {
                    feedback.style.display = "block";
                    feedback.style.background = ok ? "#ECFDF5" : "#FEF2F2";
                    feedback.style.borderColor = ok ? "#86EFAC" : "#FCA5A5";
                    feedback.style.color = ok ? "#065F46" : "#991B1B";
                    feedback.textContent = text;
                    setTimeout(() => feedback.style.display = "none", 900);
                }

                function pickItem() {
                    const isEdible = Math.random() < 0.75;
                    currentType = isEdible ? "EDIBLE" : "UNSAFE";
                    current = isEdible
                        ? edible[Math.floor(Math.random() * edible.length)]
                        : unsafe[Math.floor(Math.random() * unsafe.length)];

                    foodEmoji.textContent = current.emoji;
                    foodName.textContent = current.name;

                    if (currentEl && currentEl.parentNode) currentEl.parentNode.removeChild(currentEl);

                    const div = document.createElement("div");
                    div.className = "falling";
                    div.innerHTML = `
                        <div style="font-size:22px;">${current.emoji}</div>
                        <div style="font-weight:900;">${current.name}</div>
                    `;

                    div.addEventListener("animationend", () => {
                        if (over) return;
                        if (div === currentEl) {
                            wasted += 1;
                            wasteMeter += 18;
                            showFeedback(false, "Too slow! Item missed. Waste meter +18%");
                            nextOrEnd();
                        }
                    });

                    dropArea.appendChild(div);
                    currentEl = div;
                }

                function nextOrEnd() {
                    updateBottom();
                    if (wasteMeter >= 100) { endGame(false); return; }
                    if (!over) pickItem();
                }

                function answer(choice) {
                    if (over || !current) return;

                    const correct =
                        (currentType === "EDIBLE" && choice === "DISCOUNT") ||
                        (currentType === "UNSAFE" && choice === "WASTE");

                    if (correct) {
                        score += 10;
                        wasteMeter = Math.max(0, wasteMeter - 6);
                        if (choice === "DISCOUNT") saved += 1;
                        showFeedback(true, "Correct! (+10)");
                    } else {
                        score -= 10;
                        wasted += 1;
                        wasteMeter += (choice === "WASTE" && currentType === "EDIBLE") ? 22 : 16;
                        showFeedback(false, "Wrong! (-10)");
                    }

                    nextOrEnd();
                }

                function endGame(won) {
                    over = true;

                    hfScore.value = score;
                    hfSaved.value = saved;
                    hfWasted.value = wasted;
                    hfWon.value = won ? "1" : "0";

                    endPanel.style.display = "block";

                    const title = won ? "🎉 U Won!" : "😢 You Lost";
                    const msg = won
                        ? `Score: <b>${score}</b><br/>Click <b>Save Result</b> to get your voucher code.`
                        : `Score: <b>${score}</b><br/>You Lost, Try again tomorrow.`;
                    window.openResultModal(title, msg);

                    endTitle.textContent = title;
                    endText.innerHTML = msg;

                    btnDiscount.disabled = true;
                    btnWaste.disabled = true;
                }

                function decideWinAtTimeUp() {
                    return score >= 50 && wasteMeter < 100;
                }

                btnDiscount.addEventListener("click", () => answer("DISCOUNT"));
                btnWaste.addEventListener("click", () => answer("WASTE"));

                updateBottom();
                pickItem();

                const timer = setInterval(() => {
                    if (over) { clearInterval(timer); return; }
                    timeLeft -= 1;
                    updateBottom();
                    if (timeLeft <= 0) {
                        clearInterval(timer);
                        endGame(decideWinAtTimeUp());
                    }
                }, 1000);
            })();
        </script>

    </form>
</body>
</html>