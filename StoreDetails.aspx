<%@ Page Title="" Language="C#" MasterPageFile="~/SellPage.Master" AutoEventWireup="true" CodeBehind="StoreDetails.aspx.cs" Inherits="Business_App_Dev.StoreDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<!-- If you prefer, move CSS into EcoEats.css instead of inline -->
    <style>
        /* ===== Store Details Page (Front-end only) ===== */
        .sd-wrap { padding: 18px 0 40px; }
        .sd-grid { display: grid; grid-template-columns: 1.6fr 1fr; gap: 18px; align-items: start; }

        .sd-card {
            background: #fff;
            border: 1px solid rgba(0,0,0,0.08);
            border-radius: 18px;
            padding: 18px;
            box-shadow: 0 10px 26px rgba(0,0,0,0.04);
        }

        .sd-card h2 {
            margin: 0 0 14px;
            font-size: 18px;
            font-weight: 800;
            letter-spacing: -0.2px;
        }

        .sd-muted { color: rgba(0,0,0,0.55); font-size: 13px; margin-top: -6px; margin-bottom: 14px; }

        .sd-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
        .sd-row-1 { display: grid; grid-template-columns: 1fr; gap: 12px; }
        .sd-field { margin-bottom: 12px; }

        .sd-label { display: block; font-weight: 700; margin-bottom: 8px; font-size: 13px; }
        .sd-input, .sd-textarea, .sd-select {
            width: 100%;
            border: 1px solid rgba(0,0,0,0.10);
            background: rgba(0,0,0,0.02);
            border-radius: 14px;
            padding: 12px 12px;
            outline: none;
            font-size: 14px;
        }
        .sd-textarea { min-height: 120px; resize: vertical; }

        .sd-input:focus, .sd-textarea:focus, .sd-select:focus {
            border-color: rgba(0,0,0,0.22);
            background: #fff;
        }

        .sd-inline {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .sd-chipbox {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .sd-chip {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 10px;
            border: 1px solid rgba(0,0,0,0.10);
            background: #fff;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 600;
        }

        .sd-chip input { accent-color: #2f8f46; }

        .sd-actions {
            display: flex;
            gap: 10px;
            justify-content: flex-end;
            margin-top: 10px;
        }

        .sd-btn {
            border: none;
            border-radius: 14px;
            padding: 11px 14px;
            font-weight: 800;
            cursor: pointer;
            font-size: 14px;
        }

        .sd-btn-primary { background: #1f7a3b; color: #fff; }
        .sd-btn-ghost { background: rgba(0,0,0,0.06); color: #111; }

        .sd-hours {
            width: 100%;
            border-collapse: collapse;
            overflow: hidden;
            border-radius: 14px;
            border: 1px solid rgba(0,0,0,0.08);
            background: #fff;
        }
        .sd-hours th, .sd-hours td {
            padding: 12px 12px;
            border-bottom: 1px solid rgba(0,0,0,0.06);
            text-align: left;
            font-size: 14px;
        }
        .sd-hours th { background: rgba(0,0,0,0.03); font-size: 12px; text-transform: uppercase; letter-spacing: .6px; }
        .sd-hours tr:last-child td { border-bottom: none; }

        .sd-time { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
        .sd-toggle {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 10px 12px;
            border-radius: 14px;
            border: 1px solid rgba(0,0,0,0.08);
            background: rgba(0,0,0,0.02);
        }

        .sd-sidecard .sd-linkbtn {
            display: flex;
            align-items: center;
            gap: 10px;
            width: 100%;
            border: 1px solid rgba(0,0,0,0.10);
            background: #fff;
            padding: 12px 12px;
            border-radius: 14px;
            text-decoration: none;
            font-weight: 800;
            color: #111;
        }

        .sd-sidecard .sd-linkbtn:hover { background: rgba(0,0,0,0.02); }

        .sd-badge {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 12px;
            font-weight: 800;
            padding: 8px 10px;
            border-radius: 999px;
            background: rgba(31,122,59,0.10);
            color: #1f7a3b;
            border: 1px solid rgba(31,122,59,0.20);
        }

        /* Responsive */
        @media (max-width: 980px) {
            .sd-grid { grid-template-columns: 1fr; }
        }
        @media (max-width: 560px) {
            .sd-row { grid-template-columns: 1fr; }
            .sd-actions { flex-direction: column; }
            .sd-btn { width: 100%; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="sd-wrap">
        <div class="sd-grid">

            <!-- LEFT: Store details form -->
            <section class="sd-card">
                <h2>Store Details</h2>
                <div class="sd-muted">Update your store profile so customers can find you easily.</div>

                <div class="sd-row">
                    <div class="sd-field">
                        <label class="sd-label">Store Name</label>
                        <asp:TextBox ID="tbStoreName" runat="server" CssClass="sd-input" placeholder="e.g., Pho House" />
                    </div>

                    <div class="sd-field">
                        <label class="sd-label">Contact Number</label>
                        <asp:TextBox ID="tbPhone" runat="server" CssClass="sd-input" placeholder="+65 9123 4567" />
                    </div>
                </div>

                <div class="sd-row-1">
                    <div class="sd-field">
                        <label class="sd-label">Store Address</label>
                        <asp:TextBox ID="tbAddress" runat="server" CssClass="sd-input"
                            placeholder="e.g., 123 Food Street, #01-45, Singapore 123456" />
                    </div>

                    <div class="sd-field">
                        <label class="sd-label">Store Description</label>
                        <asp:TextBox ID="tbDescription" runat="server" CssClass="sd-textarea" TextMode="MultiLine"
                            placeholder="Tell customers what you sell and what makes your store special…" />
                    </div>
                </div>

                <div class="sd-field">
                    <label class="sd-label">Categories</label>
                    <div class="sd-chipbox">
                        <!-- Front-end only: swap to DB-driven later -->
                        <label class="sd-chip"><input type="checkbox" /> Asian Cuisine</label>
                        <label class="sd-chip"><input type="checkbox" /> Vietnamese</label>
                        <label class="sd-chip"><input type="checkbox" /> Healthy</label>
                        <label class="sd-chip"><input type="checkbox" /> Fresh</label>
                        <label class="sd-chip"><input type="checkbox" /> Halal</label>
                        <label class="sd-chip"><input type="checkbox" /> Vegetarian</label>
                    </div>
                </div>

                <div class="sd-field">
                    <label class="sd-label">Store Availability</label>
                    <div class="sd-toggle">
                        <asp:CheckBox ID="cbStoreOpen" runat="server" />
                        <div>
                            <div style="font-weight:800;">Accepting Orders</div>
                            <div class="sd-muted" style="margin:4px 0 0;">Turn off if you are temporarily unavailable.</div>
                        </div>
                    </div>
                </div>

                <div class="sd-actions">
                    <asp:Button ID="btnCancel" runat="server" CssClass="sd-btn sd-btn-ghost" Text="Cancel"
                        CausesValidation="false" PostBackUrl="~/SellerDashboard.aspx" />
                    <asp:Button ID="btnSave" runat="server" CssClass="sd-btn sd-btn-primary" Text="Save Changes"
                        CausesValidation="false" />
                </div>
            </section>

            <!-- RIGHT: Quick actions card (NO stats / NO impact) -->
            <aside class="sd-card sd-sidecard">
                <h2>Quick Actions</h2>
                <div class="sd-muted">Jump to frequently used pages.</div>

                <div style="display:flex; flex-direction:column; gap:10px;">
                    <a class="sd-linkbtn" href="SellerDashboard.aspx">← Back to Dashboard</a>
                    <a class="sd-linkbtn" href="Inventory.aspx">📦 Manage Inventory</a>
                    <a class="sd-linkbtn" href="SellerMessages.aspx">💬 View Messages</a>
                </div>

                <div style="margin-top:14px;">
                    <span class="sd-badge">Tip: Keep your hours updated</span>
                </div>
            </aside>

            <!-- FULL WIDTH: Business Hours -->
            <section class="sd-card" style="grid-column: 1 / -1;">
                <h2>Business Hours</h2>
                <div class="sd-muted">Set your opening hours so customers know when to order.</div>

                <table class="sd-hours">
                    <thead>
                        <tr>
                            <th style="width: 28%;">Day</th>
                            <th>Opening</th>
                            <th>Closing</th>
                            <th style="width: 18%;">Closed</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><strong>Monday</strong></td>
                            <td><input class="sd-input" placeholder="11:00 AM" /></td>
                            <td><input class="sd-input" placeholder="9:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Tuesday</strong></td>
                            <td><input class="sd-input" placeholder="11:00 AM" /></td>
                            <td><input class="sd-input" placeholder="9:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Wednesday</strong></td>
                            <td><input class="sd-input" placeholder="11:00 AM" /></td>
                            <td><input class="sd-input" placeholder="9:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Thursday</strong></td>
                            <td><input class="sd-input" placeholder="11:00 AM" /></td>
                            <td><input class="sd-input" placeholder="9:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Friday</strong></td>
                            <td><input class="sd-input" placeholder="11:00 AM" /></td>
                            <td><input class="sd-input" placeholder="9:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Saturday</strong></td>
                            <td><input class="sd-input" placeholder="10:00 AM" /></td>
                            <td><input class="sd-input" placeholder="10:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                        <tr>
                            <td><strong>Sunday</strong></td>
                            <td><input class="sd-input" placeholder="10:00 AM" /></td>
                            <td><input class="sd-input" placeholder="8:00 PM" /></td>
                            <td><input type="checkbox" /></td>
                        </tr>
                    </tbody>
                </table>

                <div class="sd-actions">
                    <asp:Button ID="btnHoursCancel" runat="server" CssClass="sd-btn sd-btn-ghost" Text="Reset"
                        CausesValidation="false" />
                    <asp:Button ID="btnHoursSave" runat="server" CssClass="sd-btn sd-btn-primary" Text="Save Hours"
                        CausesValidation="false" />
                </div>
            </section>

        </div>
    </div>
 </asp:Content>
