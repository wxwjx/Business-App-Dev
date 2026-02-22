<%@ Page Title="EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Product.aspx.cs"
    Inherits="Business_App_Dev.Pages.ProductPage" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/Product.css") %>" rel="stylesheet" />

    <!-- CLEAN + CONSISTENT UI (inline so it overrides weird defaults) -->
    <style>
        .p-wrap{max-width:1150px;margin:20px auto;padding:0 14px;}
        .p-top{display:flex;align-items:flex-end;justify-content:space-between;gap:12px;flex-wrap:wrap;margin:6px 0 14px;}
        .p-search{display:flex;gap:10px;align-items:center;flex-wrap:wrap;}

        /* ASP TextBox/Button */
        .p-search input[type="text"],
        .p-search input[type="search"],
        .p-search input{
            width:260px;max-width:72vw;padding:10px 12px;
            border:1px solid #e5e7eb;border-radius:12px;outline:none;background:#fff;font:inherit;
            box-shadow:0 1px 0 rgba(0,0,0,.03);
        }
        .p-search input:focus{
            border-color:#16a34a;
            box-shadow:0 0 0 4px rgba(22,163,74,.12);
        }
        .p-search input[type="submit"],
        .p-search button{
            height:42px;padding:0 14px;border:0;border-radius:12px;background:#16a34a;color:#fff;
            font-weight:900;cursor:pointer;
        }
        .p-search input[type="submit"]:hover,
        .p-search button:hover{filter:brightness(.96);}

        .p-btnlink{background:transparent;border:0;color:#111827;cursor:pointer;font-weight:900;padding:8px 6px;text-decoration:none;}
        .p-btnlink:hover{text-decoration:underline;}

        .p-tabs{display:flex;gap:10px;flex-wrap:wrap;margin:10px 0 16px;}
        .p-tab{
            display:inline-flex;align-items:center;gap:8px;
            border:1px solid #e5e7eb;background:#fff;padding:10px 12px;border-radius:999px;
            font-weight:900;cursor:pointer;color:#111827;text-decoration:none;
            box-shadow:0 1px 0 rgba(0,0,0,.03);
        }
        .p-tab:hover{border-color:#d1d5db;transform:translateY(-1px);}

        .cat-wrap{display:flex;gap:10px;flex-wrap:wrap;margin:10px 0 16px;}
        .cat-chip{
            display:inline-flex;align-items:center;gap:8px;padding:9px 12px;border-radius:999px;
            border:1px solid #e5e7eb;background:#fff;font-weight:900;color:#111827;cursor:pointer;text-decoration:none;
        }
        .cat-chip:hover{border-color:#d1d5db;}
        .cat-chip.active{background:#16a34a;color:#fff;border-color:#16a34a;}

        .p-grid{display:grid;grid-template-columns:repeat(3,1fr);gap:16px;}
        @media (max-width:980px){.p-grid{grid-template-columns:repeat(2,1fr);}}
        @media (max-width:640px){.p-grid{grid-template-columns:1fr;}}

        .p-card{
            border:1px solid #e5e7eb;border-radius:18px;overflow:hidden;background:#fff;
            display:flex;flex-direction:column;
            box-shadow:0 10px 30px rgba(0,0,0,.06);
        }
        .p-card.clickable{cursor:pointer;transition:transform .08s ease, box-shadow .12s ease;}
        .p-card.clickable:hover{transform:translateY(-2px);box-shadow:0 18px 45px rgba(0,0,0,.10);}
        .p-card.clickable:active{transform:translateY(0);box-shadow:0 10px 30px rgba(0,0,0,.06);}

        .p-img{height:180px;background:#f3f4f6;display:flex;align-items:center;justify-content:center;}
        .p-img img{width:100%;height:100%;object-fit:cover;display:block;}

        .p-info{padding:14px;display:flex;flex-direction:column;gap:8px;}
        .p-headrow{display:flex;align-items:flex-start;justify-content:space-between;gap:10px;}
        .p-name{margin:0;font-weight:1000;font-size:18px;line-height:1.15;color:#111827;}
        .p-badge{
            display:inline-flex;align-items:center;height:28px;padding:0 10px;border-radius:999px;
            background:#f59e0b;color:#111827;font-weight:1000;font-size:12px;white-space:nowrap;
        }
        .p-meta{color:#6b7280;font-weight:800;font-size:13px;line-height:1.35;}
        .p-price{margin-top:2px;font-weight:1000;font-size:18px;color:#111827;}

        .ee-assistant-fab{position:fixed;right:22px;bottom:22px;z-index:9998;background:#0b1220;color:#fff;border:0;border-radius:999px;padding:12px 16px;display:flex;gap:10px;align-items:center;box-shadow:0 18px 45px rgba(0,0,0,.18);cursor:pointer;font-weight:800;}
        .ee-assistant-panel{position:fixed;right:22px;bottom:86px;width:360px;max-width:calc(100vw - 44px);height:520px;max-height:calc(100vh - 140px);z-index:9999;background:#fff;border:1px solid #e5e7eb;border-radius:18px;overflow:hidden;box-shadow:0 22px 70px rgba(0,0,0,.20);display:none;}
        .ee-assistant-panel.open{display:block;}
        .ee-assistant-head{background:#0b1220;color:#fff;padding:14px 14px;display:flex;align-items:center;justify-content:space-between;}
        .ee-assistant-title{font-weight:900;font-size:18px;}
        .ee-assistant-close{background:transparent;border:0;color:#fff;font-size:22px;cursor:pointer;line-height:1;}
        .ee-assistant-body{height:calc(100% - 120px);padding:14px;overflow:auto;background:#f8fafc;}
        .ee-msg{max-width:85%;padding:10px 12px;border-radius:14px;margin:8px 0;border:1px solid #e5e7eb;background:#fff;line-height:1.25;white-space:pre-wrap;}
        .ee-msg.me{margin-left:auto;background:#16a34a;color:#fff;border-color:rgba(255,255,255,.15);font-weight:800;}
        .ee-assistant-foot{height:120px;border-top:1px solid #e5e7eb;background:#fff;padding:12px;display:grid;grid-template-columns:1fr auto;gap:10px;align-items:end;}
        .ee-input{width:100%;min-height:44px;max-height:88px;resize:none;border:1px solid #e5e7eb;border-radius:12px;padding:10px 12px;outline:none;font:inherit;}
        .ee-send{height:44px;padding:0 16px;border:0;border-radius:12px;background:#16a34a;color:#fff;cursor:pointer;font-weight:900;}
        .ee-send:disabled{opacity:.55;cursor:not-allowed;}
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <asp:HiddenField ID="hfLat" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfLng" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfGeoDone" runat="server" ClientIDMode="Static" Value="0" />
    <asp:LinkButton ID="btnGeoRefresh" runat="server" OnClick="btnGeoRefresh_Click" Style="display:none;">geo</asp:LinkButton>

    <div class="p-wrap">
        <div class="p-top">
            <div class="p-search">
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Search meals or stores..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnClear" runat="server" CssClass="p-btnlink" OnClick="btnClear_Click" CausesValidation="false">Clear</asp:LinkButton>
            </div>
        </div>

        <div class="p-tabs">
            <asp:LinkButton ID="btnTabRecommended" runat="server" CssClass="p-tab" OnClick="btnTabRecommended_Click" CausesValidation="false">✨ Recommended</asp:LinkButton>
            <asp:LinkButton ID="btnTabDeals" runat="server" CssClass="p-tab" OnClick="btnTabDeals_Click" CausesValidation="false">🔥 Daily Best Deals</asp:LinkButton>
            <asp:LinkButton ID="btnTabCategories" runat="server" CssClass="p-tab" OnClick="btnTabCategories_Click" CausesValidation="false">🧭 Explore Categories</asp:LinkButton>
        </div>

        <asp:Panel ID="pnlCategories" runat="server" Visible="false">
            <div class="cat-wrap">
                <asp:LinkButton ID="btnCatAll" runat="server" CssClass="cat-chip" OnClick="btnCatAll_Click" CausesValidation="false">All</asp:LinkButton>

                <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbCat" runat="server"
                            CommandName="pick"
                            CommandArgument="<%# Container.DataItem.ToString() %>"
                            CssClass="cat-chip"
                            Text="<%# Container.DataItem.ToString() %>"
                            CausesValidation="false" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <asp:Repeater ID="rptProducts" runat="server">
            <HeaderTemplate><div class="p-grid" id="productGrid"></HeaderTemplate>

            <ItemTemplate>
                <div class="p-card ee-product clickable"
                     role="button"
                     tabindex="0"
                     onclick='location.href="ProductDetails.aspx?id=<%# Eval("ProductID") %>"'
                     onkeydown='if(event.key==="Enter"){location.href="ProductDetails.aspx?id=<%# Eval("ProductID") %>";}'
                     data-id="<%# Eval("ProductID") %>"
                     data-name="<%# Server.HtmlEncode(Eval("ProductName").ToString()) %>"
                     data-category="<%# Server.HtmlEncode(Eval("Category").ToString()) %>"
                     data-price="<%# Eval("PriceNow") %>"
                     data-distance="<%# Eval("DistanceKm") %>"
                     data-store="<%# Server.HtmlEncode(Eval("StoreName").ToString()) %>">

                    <div class="p-img">
                        <img src="<%# Eval("ImageUrl") %>" alt="" onerror="this.style.display='none';" />
                    </div>

                    <div class="p-info">
                        <div class="p-headrow">
                            <p class="p-name"><%# Eval("ProductName") %></p>
                            <span class="p-badge"><%# Eval("DiscountPercent") %>% OFF</span>
                        </div>

                        <div class="p-meta">
                            ⭐ <%# Eval("Rating") %> • <%# Eval("Reviews") %> reviews •
                            <%# (Eval("DistanceKm") == null ? "— km" : (string.Format("{0:0.0}", Eval("DistanceKm")) + " km")) %>
                        </div>

                        <div class="p-meta"><%# Eval("StoreName") %> • <%# Eval("Category") %></div>
                        <div class="p-price">$<%# string.Format("{0:0.00}", Eval("PriceNow")) %></div>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
    </div>

    <!-- Assistant -->
    <button type="button" class="ee-assistant-fab" id="eeFab">🤖 <span>Ask EcoEats</span></button>

    <div class="ee-assistant-panel" id="eePanel" aria-live="polite">
        <div class="ee-assistant-head">
            <div class="ee-assistant-title">EcoEats Assistant</div>
            <button type="button" class="ee-assistant-close" id="eeClose">×</button>
        </div>

        <div class="ee-assistant-body" id="eeChat">
            <div class="ee-msg">Tell me what you want (spicy/sweet/budget/halal/no seafood). I’ll recommend from the items on your screen.</div>
        </div>

        <div class="ee-assistant-foot">
            <textarea class="ee-input" id="eeInput" placeholder="Type here..."></textarea>
            <button type="button" class="ee-send" id="eeSend">Send</button>
        </div>
    </div>

    <script>
        (function () {
            const panel = document.getElementById("eePanel");
            const fab = document.getElementById("eeFab");
            const closeBtn = document.getElementById("eeClose");
            const chat = document.getElementById("eeChat");
            const input = document.getElementById("eeInput");
            const sendBtn = document.getElementById("eeSend");

            function addMsg(text, who) {
                const div = document.createElement("div");
                div.className = "ee-msg" + (who === "me" ? " me" : "");
                div.textContent = text;
                chat.appendChild(div);
                chat.scrollTop = chat.scrollHeight;
            }

            function collectScreenItems(limit = 18) {
                const nodes = Array.from(document.querySelectorAll(".ee-product"));
                const items = [];
                for (const n of nodes) {
                    if (items.length >= limit) break;
                    const name = n.getAttribute("data-name") || "";
                    const category = n.getAttribute("data-category") || "";
                    const price = n.getAttribute("data-price") || "";
                    const distance = n.getAttribute("data-distance") || "";
                    const store = n.getAttribute("data-store") || "";
                    const id = n.getAttribute("data-id") || "";
                    if (!name) continue;
                    items.push({ id, name, category, price, distanceKm: distance, store });
                }
                return items;
            }

            async function send() {
                const msg = (input.value || "").trim();
                if (!msg) return;

                addMsg(msg, "me");
                input.value = "";
                sendBtn.disabled = true;

                try {
                    const payload = {
                        message: msg,
                        items: collectScreenItems(),
                        lat: document.getElementById("hfLat")?.value || "",
                        lng: document.getElementById("hfLng")?.value || ""
                    };

                    const res = await fetch("<%= ResolveUrl("~/ChatAssistant.ashx") %>", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(payload)
                    });

                    if (!res.ok) throw new Error("HTTP " + res.status);
                    const data = await res.json();
                    if (!data || !data.reply) throw new Error("Bad response");
                    addMsg(data.reply, "bot");
                } catch (e) {
                    addMsg("Network/server error. Try again.", "bot");
                } finally {
                    sendBtn.disabled = false;
                }
            }

            fab.addEventListener("click", () => panel.classList.add("open"));
            closeBtn.addEventListener("click", () => panel.classList.remove("open"));
            sendBtn.addEventListener("click", send);

            input.addEventListener("keydown", (ev) => {
                if (ev.key === "Enter" && !ev.shiftKey) {
                    ev.preventDefault();
                    send();
                }
            });

            // Geo -> refresh products with distance
            try {
                const latEl = document.getElementById("hfLat");
                const lngEl = document.getElementById("hfLng");
                const doneEl = document.getElementById("hfGeoDone");

                const already =
                    (doneEl && doneEl.value === "1") ||
                    sessionStorage.getItem("ee_geo_done") === "1";

                if (!already && navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(
                        (pos) => {
                            if (latEl) latEl.value = pos.coords.latitude;
                            if (lngEl) lngEl.value = pos.coords.longitude;
                            if (doneEl) doneEl.value = "1";
                            sessionStorage.setItem("ee_geo_done", "1");
                            __doPostBack("<%= btnGeoRefresh.UniqueID %>", "");
                        },
                        () => { },
                        { enableHighAccuracy: true, timeout: 8000, maximumAge: 600000 }
                    );
                }
            } catch (_) { }
        })();
    </script>
</asp:Content>