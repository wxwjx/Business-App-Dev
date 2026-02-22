<%@ Page Title="Store Details"
    Language="C#"
    MasterPageFile="~/SellPage.master"
    AutoEventWireup="true"
    CodeBehind="StoreDetails.aspx.cs"
    Inherits="Business_App_Dev.StoreDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<style>
    .sd-container { max-width: 1100px; margin: 28px auto; padding: 0 14px; }
    .sd-header { display:flex; justify-content:flex-start; align-items:flex-end; gap:16px; margin-bottom:14px; }
    .sd-header h2 { margin:0; font-size:28px; letter-spacing:-0.02em; }

    .sd-grid { display: grid; grid-template-columns: 1fr 420px; gap: 22px; align-items: start; }
    @media (max-width: 992px) { .sd-grid { grid-template-columns: 1fr; } }

    .sd-card {
        background:#fff;
        border-radius: 18px;
        padding: 22px 22px;
        box-shadow: 0 10px 28px rgba(0,0,0,0.06);
        border: 1px solid rgba(0,0,0,0.04);
    }

    .sd-right-col{ display:flex; flex-direction:column; gap: 14px; }

    .sd-actions-top{ display:flex; justify-content:flex-end; gap:10px; }

    .sd-btn{
        padding: 10px 16px;
        border-radius: 999px;
        border: none;
        cursor: pointer;
        font-weight: 800;
        font-size: 14px;
    }
    .sd-edit-btn{ background:#27ae60; color:#fff; }
    .sd-edit-btn:hover{ filter:brightness(.96); }
    .sd-save-btn{ background:#2ecc71; color:#fff; }
    .sd-save-btn:hover{ filter:brightness(.96); }
    .sd-cancel-btn{ background:#e5e7eb; color:#111827; }
    .sd-cancel-btn:hover{ filter:brightness(.97); }

    .sd-left-stack{ display:flex; flex-direction:column; gap: 14px; }

    .sd-info-row{
        padding: 12px 14px;
        border-radius: 14px;
        border: 1px solid #eef2f7;
        background: #fafafa;
    }
    .sd-info-row label{
        display:block;
        font-weight: 900;
        color:#111827;
        margin-bottom: 4px;
        font-size: 14px;
    }
    .sd-value{ color:#374151; font-size: 15px; line-height: 1.5; white-space: normal; }

    .sd-field { margin-bottom: 18px; }
    .sd-field label { font-weight: 800; display: block; margin-bottom: 6px; color: #111827; }

    .sd-input, .sd-textarea {
        width: 100%;
        padding: 10px 12px;
        border-radius: 10px;
        border: 1px solid #ddd;
        font-size: 14px;
        background: #fff;
        outline: none;
    }
    .sd-textarea { resize: vertical; }

    .sd-actions { margin-top: 14px; display:flex; gap:10px; flex-wrap:wrap; }

    .sd-hours-row { display: grid; grid-template-columns: 70px 120px 1fr; gap: 10px; align-items: center; margin-bottom: 10px; }
    .sd-day { font-weight: 800; color: #111827; }
    .sd-hours-timewrap { display: flex; gap: 10px; }
    .sd-hours-time { min-width: 140px; }

    .sd-note { margin-top: 12px; color: #6b7280; font-size: 13px; font-weight: 700; }

    .sd-map-wrap{ margin-top: 10px; }
    #map{ border: 1px solid #eef2f7 !important; border-radius:14px; overflow:hidden; }

    .sd-modal-overlay {
        position: fixed; inset: 0;
        background: rgba(0,0,0,0.45);
        display: none;
        align-items: center;
        justify-content: center;
        z-index: 9999;
    }
    .sd-modal {
        width: min(520px, 92vw);
        background: #fff;
        border-radius: 16px;
        box-shadow: 0 22px 70px rgba(0,0,0,0.25);
        overflow: hidden;
    }
    .sd-modal-head {
        display:flex; align-items:center; justify-content:space-between;
        padding: 14px 16px; border-bottom: 1px solid #eee;
    }
    .sd-modal-title { font-weight: 900; color:#111827; }
    .sd-modal-x { background: transparent; border: none; font-size: 18px; cursor: pointer; }
    .sd-modal-body { padding: 16px; color:#374151; font-weight:700; }
    .sd-modal-actions {
        display:flex; justify-content:flex-end; gap:10px;
        padding: 14px 16px; border-top: 1px solid #eee;
    }

    .sd-pw-card{
        border: 1px solid #eaf3ef;
        background: #f7fbfa;
        border-radius: 18px;
        padding: 18px;
    }
    .sd-pw-title{ font-weight: 950; color:#111827; margin:0 0 6px; font-size:18px; }
    .sd-pw-sub{ margin:0 0 12px; font-size: 13px; color:#6b7280; line-height:1.5; font-weight:700; }

    .sd-pw-field{ position:relative; margin-top: 12px; }
    .sd-pw-input{
        width:100%;
        border:1px solid #e3e8e7;
        border-radius: 14px;
        padding: 12px 44px 12px 12px;
        font-size: 14px;
        background:#fff;
        outline:none;
    }
    .sd-pw-input:focus{
        border-color: rgba(39,174,96,.55);
        box-shadow: 0 0 0 4px rgba(39,174,96,.12);
    }
    .sd-pw-eye{
        position:absolute;
        right: 12px;
        top: 50%;
        transform: translateY(-50%);
        border:0;
        background:transparent;
        cursor:pointer;
        font-size: 16px;
        opacity:.85;
    }

    .sd-pw-rules{
        margin-top: 14px;
        padding: 14px;
        border-radius: 14px;
        border:1px solid #e5e7eb;
        background:#fff;
        display:grid;
        gap: 8px;
    }
    .sd-pw-rule{
        font-size: 13px;
        font-weight: 900;
        color:#6b7280;
        display:flex;
        align-items:center;
        gap: 8px;
    }
    .sd-pw-rule::before{
        content:"✕";
        font-weight: 900;
        color:#9ca3af;
        width:18px;
        display:inline-block;
        text-align:center;
    }
    .sd-pw-rule.ok{ color:#065F46; }
    .sd-pw-rule.ok::before{ content:"✓"; color:#16a34a; }

    .sd-pw-confirm{
        margin-top: 14px;
        width:100%;
        border-radius: 999px;
        padding: 12px 14px;
        font-weight: 950;
        border:none;
        cursor:pointer;
        background:#27ae60;
        color:#fff;
    }
    .sd-pw-confirm:hover{ filter:brightness(.95); }

    .sd-pw-msg{
        margin-top: 12px;
        border-radius: 12px;
        padding: 10px 12px;
        font-size: 13px;
        font-weight: 900;
        display:none;
    }
</style>

<div class="sd-container">

    <asp:HiddenField ID="hfEditMode" runat="server" Value="0" />

    <asp:Button ID="btnLogoutTrigger" runat="server"
        Text=""
        Style="display:none;"
        OnClick="btnLogout_Click"
        CausesValidation="false"
        UseSubmitBehavior="false" />

    <div class="sd-header">
        <h2>Store Details</h2>
    </div>

    <!-- VIEW MODE -->
    <div id="viewSection">
        <div class="sd-grid">

            <div class="sd-left-stack">

                <div class="sd-info-row">
                    <label>Store Name</label>
                    <asp:Label ID="lblStoreName" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-info-row">
                    <label>Phone</label>
                    <asp:Label ID="lblPhone" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-info-row">
                    <label>Email</label>
                    <asp:Label ID="lblEmail" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-info-row">
                    <label>Store Address</label>
                    <asp:Label ID="lblAddress" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-info-row">
                    <label>Location</label>
                    <div class="sd-value" id="locText"></div>

                    <div class="sd-map-wrap">
                        <div id="map" style="width:100%; height:280px;"></div>
                    </div>

                    <asp:HiddenField ID="hfLat" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfLng" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfAddr" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfShopName" runat="server" ClientIDMode="Static" />
                </div>

                <div class="sd-info-row">
                    <label>Description</label>
                    <asp:Label ID="lblDescription" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-info-row">
                    <label>Business Hours</label>
                    <asp:Label ID="lblHours" runat="server" CssClass="sd-value" />
                </div>

            </div>

            <div class="sd-right-col">

                <div class="sd-actions-top">
                    <button type="button" class="sd-btn sd-edit-btn" onclick="toggleEdit(true)">Edit</button>

                    <asp:Button ID="btnLogout" runat="server"
                        Text="Log Out"
                        CssClass="sd-btn sd-cancel-btn"
                        OnClientClick="openLogoutModal(); return false;"
                        CausesValidation="false"
                        UseSubmitBehavior="false" />
                </div>

                <div class="sd-pw-card">
                    <div class="sd-pw-title">Store Password</div>
                    <div class="sd-pw-sub">
                        Current password is secured (PBKDF2). You can only change it.
                    </div>

                    <div style="font-size:14px; color:#374151; font-weight:800; margin-top:6px;">
                        <span style="color:#111827;">Password:</span>
                        <asp:Label ID="lblStorePassword" runat="server" Text="•••••••• (secured)" />
                    </div>

                    <div class="sd-pw-field">
                        <asp:TextBox ID="txtStorePw" runat="server"
                            TextMode="Password"
                            CssClass="sd-pw-input"
                            placeholder="New password"
                            onkeyup="sdUpdatePasswordUI()"
                            onchange="sdUpdatePasswordUI()" />
                        <button type="button" class="sd-pw-eye" onclick="sdTogglePw('txtStorePw')">👁</button>
                    </div>

                    <div class="sd-pw-field">
                        <asp:TextBox ID="txtStorePwConfirm" runat="server"
                            TextMode="Password"
                            CssClass="sd-pw-input"
                            placeholder="Confirm new password"
                            onkeyup="sdUpdatePasswordUI()"
                            onchange="sdUpdatePasswordUI()" />
                        <button type="button" class="sd-pw-eye" onclick="sdTogglePw('txtStorePwConfirm')">👁</button>
                    </div>

                    <div class="sd-pw-rules">
                        <div class="sd-pw-rule" id="sdRuleLen">At least 8 characters</div>
                        <div class="sd-pw-rule" id="sdRuleLetter">Contains a letter (A–Z)</div>
                        <div class="sd-pw-rule" id="sdRuleNum">Contains a number (0–9)</div>
                        <div class="sd-pw-rule" id="sdRuleSpecial">Contains a special character (!@#...)</div>
                    </div>

                    <asp:Button ID="btnUpdateStorePassword" runat="server"
                        Text="Confirm Password Change"
                        CssClass="sd-pw-confirm"
                        OnClick="btnUpdateStorePassword_Click" />

                    <asp:Label ID="lblStorePwMsg" runat="server" CssClass="sd-pw-msg" />
                </div>

            </div>

        </div>
    </div>

    <!-- EDIT MODE -->
    <div id="editSection" style="display:none" class="sd-card">

        <div class="sd-field">
            <label>Store Name</label>
            <asp:TextBox ID="tbStoreName" runat="server" CssClass="sd-input" />
        </div>

        <div class="sd-field">
            <label>Phone</label>
            <asp:TextBox ID="tbPhone" runat="server" CssClass="sd-input" />
        </div>

        <div class="sd-field">
            <label>Email</label>
            <asp:TextBox ID="tbEmail" runat="server" CssClass="sd-input" TextMode="Email" />
        </div>

        <div class="sd-field">
            <label>Store Address</label>
            <asp:TextBox ID="tbAddress" runat="server" CssClass="sd-input" />
        </div>

        <div class="sd-field">
            <label>Description</label>
            <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Rows="4" CssClass="sd-textarea" />
        </div>

        <div class="sd-field">
            <label>Operating Hours</label>

            <div class="sd-hours-row">
                <div class="sd-day">Default</div>
                <div></div>
                <span class="sd-hours-timewrap">
                    <asp:TextBox ID="tbDefaultFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbDefaultTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Mon</div>
                <asp:DropDownList ID="ddlMonMode" runat="server" CssClass="sd-input" onchange="toggleDay('Mon')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapMon" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbMonFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbMonTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Tue</div>
                <asp:DropDownList ID="ddlTueMode" runat="server" CssClass="sd-input" onchange="toggleDay('Tue')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapTue" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbTueFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbTueTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Wed</div>
                <asp:DropDownList ID="ddlWedMode" runat="server" CssClass="sd-input" onchange="toggleDay('Wed')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapWed" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbWedFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbWedTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Thu</div>
                <asp:DropDownList ID="ddlThuMode" runat="server" CssClass="sd-input" onchange="toggleDay('Thu')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapThu" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbThuFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbThuTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Fri</div>
                <asp:DropDownList ID="ddlFriMode" runat="server" CssClass="sd-input" onchange="toggleDay('Fri')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapFri" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbFriFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbFriTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Sat</div>
                <asp:DropDownList ID="ddlSatMode" runat="server" CssClass="sd-input" onchange="toggleDay('Sat')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapSat" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbSatFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbSatTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-hours-row">
                <div class="sd-day">Sun</div>
                <asp:DropDownList ID="ddlSunMode" runat="server" CssClass="sd-input" onchange="toggleDay('Sun')">
                    <asp:ListItem Text="Default" Value="Default" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                    <asp:ListItem Text="Custom" Value="Custom" />
                </asp:DropDownList>
                <span id="wrapSun" class="sd-hours-timewrap">
                    <asp:TextBox ID="tbSunFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                    <asp:TextBox ID="tbSunTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                </span>
            </div>

            <div class="sd-note">
                Tip: Set Default once, then only change days that are Closed or Custom.
            </div>
        </div>

        <div class="sd-actions">
            <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="sd-btn sd-save-btn" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="sd-btn sd-cancel-btn" OnClick="btnCancel_Click" />
        </div>

    </div>
</div>

<div id="sdLogoutModal" class="sd-modal-overlay" style="display:none;">
    <div class="sd-modal">
        <div class="sd-modal-head">
            <div class="sd-modal-title">Log out</div>
            <button type="button" class="sd-modal-x" onclick="closeLogoutModal()">✕</button>
        </div>

        <div class="sd-modal-body">Do you really want to log out?</div>

        <div class="sd-modal-actions">
            <button type="button" class="sd-btn sd-cancel-btn" onclick="closeLogoutModal()">Cancel</button>
            <button type="button" class="sd-btn sd-save-btn" onclick="submitLogout()">Log out</button>
        </div>
    </div>
</div>

<script>
    function toggleDay(dayKey) {
        var ddlId = {
            "Mon": "<%= ddlMonMode.ClientID %>",
            "Tue": "<%= ddlTueMode.ClientID %>",
            "Wed": "<%= ddlWedMode.ClientID %>",
            "Thu": "<%= ddlThuMode.ClientID %>",
            "Fri": "<%= ddlFriMode.ClientID %>",
            "Sat": "<%= ddlSatMode.ClientID %>",
            "Sun": "<%= ddlSunMode.ClientID %>"
        }[dayKey];

        var ddl = document.getElementById(ddlId);
        var wrap = document.getElementById("wrap" + dayKey);
        if (!ddl || !wrap) return;

        wrap.style.display = (ddl.value === "Custom") ? "flex" : "none";
    }

    function toggleEdit(toEdit) {
        var view = document.getElementById("viewSection");
        var edit = document.getElementById("editSection");
        if (!view || !edit) return;

        if (toEdit) {
            view.style.setProperty("display", "none", "important");
            edit.style.setProperty("display", "block", "important");
            window.scrollTo({ top: 0, behavior: "smooth" });
            ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"].forEach(toggleDay);
        } else {
            edit.style.setProperty("display", "none", "important");
            view.style.setProperty("display", "block", "important");
            window.scrollTo({ top: 0, behavior: "smooth" });
        }
    }

    function openLogoutModal() {
        document.getElementById("sdLogoutModal").style.display = "flex";
    }
    function closeLogoutModal() {
        document.getElementById("sdLogoutModal").style.display = "none";
    }
    function submitLogout() {
        closeLogoutModal();
        document.getElementById("<%= btnLogoutTrigger.ClientID %>").click();
    }

    let map, marker;

    function initMap() {
        const latStr = document.getElementById("hfLat")?.value || "";
        const lngStr = document.getElementById("hfLng")?.value || "";
        const addr = document.getElementById("hfAddr")?.value || "";
        const shop = document.getElementById("hfShopName")?.value || "Store";
        const locText = document.getElementById("locText");

        const sg = { lat: 1.3521, lng: 103.8198 };

        const lat = parseFloat(latStr);
        const lng = parseFloat(lngStr);
        const hasLatLng = !isNaN(lat) && !isNaN(lng) && Math.abs(lat) > 0.0001 && Math.abs(lng) > 0.0001;

        map = new google.maps.Map(document.getElementById("map"), {
            center: hasLatLng ? { lat, lng } : sg,
            zoom: hasLatLng ? 16 : 12,
            mapTypeControl: false,
            streetViewControl: false
        });

        marker = new google.maps.Marker({
            map: map,
            position: hasLatLng ? { lat, lng } : sg,
            title: shop
        });

        if (locText) {
            locText.innerHTML = hasLatLng
                ? `Lat: ${lat.toFixed(6)}, Lng: ${lng.toFixed(6)}`
                : (addr ? addr : "Location not set");
        }

        if (!hasLatLng && addr) {
            const geocoder = new google.maps.Geocoder();
            geocoder.geocode({ address: addr }, (results, status) => {
                if (status === "OK" && results[0]) {
                    const p = results[0].geometry.location;
                    map.setCenter(p);
                    map.setZoom(16);
                    marker.setPosition(p);
                }
            });
        }
    }

    function sdTogglePw(serverId) {
        var pw = document.getElementById("<%= txtStorePw.ClientID %>");
        var cf = document.getElementById("<%= txtStorePwConfirm.ClientID %>");

        var target = null;
        if (serverId === "txtStorePw") target = pw;
        if (serverId === "txtStorePwConfirm") target = cf;
        if (!target) return;

        target.type = (target.type === "password") ? "text" : "password";
    }

    function sdUpdatePasswordUI() {
        const pwEl = document.getElementById("<%= txtStorePw.ClientID %>");
        const cfEl = document.getElementById("<%= txtStorePwConfirm.ClientID %>");
        if (!pwEl || !cfEl) return;

        const pw = pwEl.value || "";

        const hasLen = pw.length >= 8;
        const hasLetter = /[A-Za-z]/.test(pw);
        const hasNum = /[0-9]/.test(pw);
        const hasSpecial = /[^A-Za-z0-9]/.test(pw);

        document.getElementById("sdRuleLen")?.classList.toggle("ok", hasLen);
        document.getElementById("sdRuleLetter")?.classList.toggle("ok", hasLetter);
        document.getElementById("sdRuleNum")?.classList.toggle("ok", hasNum);
        document.getElementById("sdRuleSpecial")?.classList.toggle("ok", hasSpecial);
    }

    window.addEventListener("load", sdUpdatePasswordUI);
</script>

<script async defer
    src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCS1t5r-1IlT_qqHoUT2HuUhM9S0DjIczo&callback=initMap&libraries=places">
</script>

</asp:Content>