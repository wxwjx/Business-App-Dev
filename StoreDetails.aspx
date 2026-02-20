<%@ Page Title="Store Details"
    Language="C#"
    MasterPageFile="~/SellPage.master"
    AutoEventWireup="true"
    CodeBehind="StoreDetails.aspx.cs"
    Inherits="Business_App_Dev.StoreDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .sd-container { max-width: 1100px; margin: 30px auto; padding: 0 10px; }
        .sd-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
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

        .sd-side-card-title { font-weight: 800; font-size: 18px; margin: 0 0 12px 0; color: #1f2d3d; }
        .sd-tags { display: flex; flex-wrap: wrap; gap: 10px; }
        .sd-tag { display: inline-flex; align-items: center; padding: 6px 12px; border-radius: 999px; border: 1px solid #e5e7eb; background: #fff; color: #111827; font-size: 13px; font-weight: 700; white-space: nowrap; }

        .sd-pill-list { display: flex; flex-wrap: wrap; gap: 10px; margin: 0; padding: 0; }
        .sd-pill-list label { display: inline-flex; align-items: center; gap: 8px; padding: 7px 12px; border-radius: 999px; border: 1px solid #e5e7eb; background: #fff; cursor: pointer; user-select: none; font-size: 13px; font-weight: 700; color: #111827; }
        .sd-pill-list input[type="checkbox"] { width: 16px; height: 16px; accent-color: #27ae60; }

        .sd-note { margin-top: 12px; color: #6b7280; font-size: 13px; }

        .sd-hours-row { display: grid; grid-template-columns: 70px 120px 1fr; gap: 10px; align-items: center; margin-bottom: 10px; }
        .sd-day { font-weight: 700; color: #2c3e50; }
        .sd-hours-timewrap { display: flex; gap: 10px; }
        .sd-hours-time { min-width: 140px; }
        #viewSection, #editSection { grid-column: 1; }
        .sd-sidebar { grid-column: 2; }
    </style>

    <div class="sd-container">

        <asp:HiddenField ID="hfEditMode" runat="server" Value="0" />

        <div class="sd-header">
            <h2 style="margin:0;">Store Details</h2>

           <div class="sd-header">
    <h2 style="margin:0;">Store Details</h2>

    <asp:Button ID="btnEdit"
        runat="server"
        Text="Edit"
        CssClass="sd-btn sd-edit-btn"
        OnClientClick="toggleEdit(true); return false;" />
</div>  <%-- ✅ CLOSE sd-header --%>

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

            <!-- ================= RIGHT ================= -->
            <div class="sd-sidebar">

                <div id="categoryViewCard" ClientIDMode="Static" class="sd-card" runat="server">
                    <div class="sd-side-card-title">Categories</div>
                    <div class="sd-tags">
                        <asp:Repeater ID="rptCategories" runat="server">
                            <ItemTemplate>
                                <span class="sd-tag"><%# Container.DataItem %></span>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div id="categoryEditCard" ClientIDMode="Static" class="sd-card" style="display:none;" runat="server">
                    <div class="sd-side-card-title">Categories</div>
                    <div class="sd-pill-list">
                        <asp:CheckBoxList ID="cblCategories" runat="server" RepeatLayout="Flow" />
                    </div>
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

            var catView = document.getElementById("categoryViewCard");
            var catEdit = document.getElementById("categoryEditCard");

            var hf = document.getElementById("<%= hfEditMode.ClientID %>");

            if (toEdit) {
                edit.style.display = "block";
                view.style.display = "none";
                catEdit.style.display = "block";
                catView.style.display = "none";
                hf.value = "1";
                ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"].forEach(toggleDay);
            } else {
                edit.style.display = "none";
                view.style.display = "block";
                catEdit.style.display = "none";
                catView.style.display = "block";
                hf.value = "0";
            }
        }
    </script>

</asp:Content>