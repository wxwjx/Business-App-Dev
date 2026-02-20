<%@ Page Title="Store Details"
    Language="C#"
    MasterPageFile="~/SellPage.master"
    AutoEventWireup="true"
    CodeBehind="StoreDetails.aspx.cs"
    Inherits="Business_App_Dev.StoreDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   <style>
        .sd-container { max-width: 1100px; margin: 30px auto; padding: 0 10px; }

        /* ✅ single header row */
        .sd-header { display:flex; justify-content:space-between; align-items:center; gap:12px; margin-bottom:16px; }
        .sd-header h2 { margin:0; }

        .sd-grid { display: grid; grid-template-columns: 1fr 320px; gap: 18px; align-items: start; }
        @media (max-width: 992px) { .sd-grid { grid-template-columns: 1fr; } }

        .sd-card { background: #fff; padding: 25px; border-radius: 16px; box-shadow: 0 6px 18px rgba(0,0,0,0.06); }
        .sd-field { margin-bottom: 18px; }
        .sd-field label { font-weight: 700; display: block; margin-bottom: 6px; color: #2c3e50; }
        .sd-value { color: #555; font-size: 15px; line-height: 1.5; white-space: normal; }

        .sd-input, .sd-textarea { width: 100%; padding: 10px 12px; border-radius: 10px; border: 1px solid #ddd; font-size: 14px; background: #fff; }
        .sd-textarea { resize: vertical; }

        .sd-actions { margin-top: 20px; display: flex; gap: 10px; flex-wrap: wrap; }
        .sd-btn { padding: 10px 18px; border-radius: 10px; border: none; cursor: pointer; font-weight: 700; }
        .sd-edit-btn { background: #27ae60; color: #fff; }
        .sd-save-btn { background: #2ecc71; color: #fff; }
        .sd-cancel-btn { background: #bdc3c7; color: #1f2d3d; }

     
        .sd-pill-list label { display: inline-flex; align-items: center; gap: 8px; padding: 7px 12px; border-radius: 999px; border: 1px solid #e5e7eb; background: #fff; cursor: pointer; user-select: none; font-size: 13px; font-weight: 700; color: #111827; }
        .sd-pill-list input[type="checkbox"] { width: 16px; height: 16px; accent-color: #27ae60; }

        .sd-note { margin-top: 12px; color: #6b7280; font-size: 13px; }

        .sd-hours-row { display: grid; grid-template-columns: 70px 120px 1fr; gap: 10px; align-items: center; margin-bottom: 10px; }
        .sd-day { font-weight: 700; color: #2c3e50; }
        .sd-hours-timewrap { display: flex; gap: 10px; }
        .sd-hours-time { min-width: 140px; }

        /* Optional: keep explicit grid positions */
        #viewSection, #editSection { grid-column: 1; }
        .sd-sidebar { grid-column: 2; }
        @media (max-width: 992px) {
            .sd-sidebar { grid-column: 1; }
        }
    </style>

    <div class="sd-container">

        <asp:HiddenField ID="hfEditMode" runat="server" Value="0" />

        <div class="sd-header">
            <h2 style="margin:0;">Store Details</h2>

    <asp:Button ID="btnEdit"
        runat="server"
        Text="Edit"
        CssClass="sd-btn sd-edit-btn"
        OnClientClick="toggleEdit(true); return false;" />
</div>  

<div class="sd-grid">

            <!-- ================= LEFT ================= -->

            <!-- VIEW MODE -->
            <div id="viewSection" ClientIDMode="Static" class="sd-card" runat="server">

                <div class="sd-field">
                    <label>Store Name</label>
                    <asp:Label ID="lblStoreName" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-field">
                    <label>Phone</label>
                    <asp:Label ID="lblPhone" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-field">
                    <label>Email</label>
                    <asp:Label ID="lblEmail" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-field">
                    <label>Store Address</label>
                    <asp:Label ID="lblAddress" runat="server" CssClass="sd-value" />
                </div>    
                
                <div class="sd-field">
                <label>Location</label>

                <!-- show small text (optional) -->
                <div class="sd-value" id="locText" style="margin-bottom:10px;"></div>

                <!-- map container -->
                <div id="map" style="width:100%; height:280px; border-radius:14px; overflow:hidden; border:1px solid #eee;"></div>

                <!-- pass values from server to JS -->
                <asp:HiddenField ID="hfLat" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfLng" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfAddr" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfShopName" runat="server" ClientIDMode="Static" />
            </div>

                <div class="sd-field">
                    <label>Description</label>
                    <asp:Label ID="lblDescription" runat="server" CssClass="sd-value" />
                </div>

                <div class="sd-field">
                    <label>Business Hours</label>
                    <asp:Label ID="lblHours" runat="server" CssClass="sd-value" />
                </div>

            </div>

            <!-- EDIT MODE -->
            <div id="editSection" ClientIDMode="Static" style="display:none" runat="server" class="sd-card">

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

                    <!-- DEFAULT -->
                    <div class="sd-hours-row">
                        <div class="sd-day">Default</div>
                        <div></div>
                        <span class="sd-hours-timewrap">
                            <asp:TextBox ID="tbDefaultFrom" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                            <asp:TextBox ID="tbDefaultTo" runat="server" CssClass="sd-input sd-hours-time" TextMode="Time" />
                        </span>
                    </div>

                    <!-- Day rows -->
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

      
            var hf = document.getElementById("<%= hfEditMode.ClientID %>");

            if (toEdit) {
                edit.style.display = "block";
                view.style.display = "none";
                hf.value = "1";
                ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"].forEach(toggleDay);
            } else {
                edit.style.display = "none";
                view.style.display = "block";
                hf.value = "0";
            }
        }


        let map, marker;

        function initMap() {
            const latStr = document.getElementById("hfLat")?.value || "";
            const lngStr = document.getElementById("hfLng")?.value || "";
            const addr = document.getElementById("hfAddr")?.value || "";
            const shop = document.getElementById("hfShopName")?.value || "Store";

            const locText = document.getElementById("locText");

            // Default (Singapore center) if nothing
            const sg = { lat: 1.3521, lng: 103.8198 };

            // If DB has lat/lng
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

            // Optional fallback: geocode address if no lat/lng
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
    </script>

    <script async defer
    src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCS1t5r-1IlT_qqHoUT2HuUhM9S0DjIczo&callback=initMap&libraries=places">
</script>

</asp:Content>