<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderFeedback.aspx.cs" Inherits="Business_App_Dev.OrderFeedback" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>Order Feedback - EcoEats</title>

    <style>
        body { margin:0; font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif; background:#f3faf6; }
        a { text-decoration:none; }

        /* EcoEats-style hero like your screenshot */
        .hero {
            background: linear-gradient(90deg, #2f7d32 0%, #0ea5a8 100%);
            color: #fff;
            padding: 44px 0 120px;
        }
        .container { width: min(1100px, 92%); margin: 0 auto; }
        .hero h1 { margin:0; font-size: 48px; letter-spacing: -0.5px; }
        .hero p { margin: 10px 0 0; opacity: 0.95; font-size: 16px; }

        /* Floating card area */
        .surface {
            margin-top: -70px;
            padding-bottom: 40px;
        }
        .card {
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 10px 26px rgba(16, 24, 40, 0.10);
            padding: 20px;
        }
        .card + .card { margin-top: 18px; }

        .badge {
            display:inline-flex;
            align-items:center;
            gap:8px;
            background: rgba(47,125,50,0.12);
            color: #2f7d32;
            font-weight: 700;
            border-radius: 999px;
            padding: 6px 12px;
            font-size: 12px;
        }

        .titleRow {
            display:flex;
            justify-content:space-between;
            align-items:flex-start;
            gap:12px;
            flex-wrap:wrap;
            margin-top: 10px;
        }
        .subtle {
            color:#6b7280;
            font-size: 13px;
            margin-top: 4px;
        }

        label { display:block; margin-top: 12px; font-weight: 800; color:#111827; font-size: 13px; }
        select, textarea {
            width: 100%;
            margin-top: 6px;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            padding: 10px 12px;
            outline: none;
            font-size: 14px;
            background: #fff;
        }
        textarea { min-height: 92px; resize: vertical; }

        /* Star rating (EcoEats feel) */
        .starsWrap { margin-top: 10px; }
        .stars {
            display:inline-flex;
            flex-direction: row-reverse;
            gap: 6px;
            user-select:none;
        }
        .stars input { display:none; }
        .stars label {
            font-size: 34px;
            cursor:pointer;
            color:#d1d5db;
            line-height:1;
        }
        .stars input:checked ~ label { color:#f59e0b; }
        .stars label:hover, .stars label:hover ~ label { color:#fbbf24; }

        .grid2 { display:grid; grid-template-columns: 1fr 1fr; gap: 14px; }
        @media (max-width: 720px){ .grid2 { grid-template-columns: 1fr; } .hero h1{font-size:36px;} }

        .btnRow { display:flex; gap:10px; flex-wrap:wrap; margin-top: 16px; }
        .btn {
            border:0;
            border-radius: 999px;
            padding: 10px 16px;
            font-weight: 800;
            cursor:pointer;
            font-size: 14px;
        }
        .btnPrimary { background:#2f7d32; color:#fff; }
        .btnGhost { background:#eef2f7; color:#111827; }

        .msg { margin-top: 12px; font-weight: 800; }
        .ok { color:#16a34a; }
        .err { color:#dc2626; }

        /* GridView styling */
        .tableWrap { overflow:auto; }
        table { width:100%; border-collapse: collapse; }
        th, td { padding: 12px 10px; border-bottom: 1px solid #eef2f7; text-align:left; }
        th { font-size: 12px; color:#6b7280; text-transform: uppercase; letter-spacing: .06em; }
        td { font-size: 14px; color:#111827; }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <!-- HERO -->
    <div class="hero">
        <div class="container">
            <h1>Rate Your Order</h1>
            <p>Help EcoEats reduce food waste by sharing your experience.</p>
        </div>
    </div>

    <!-- CONTENT -->
    <div class="surface">
        <div class="container">

            <div class="card">
                <span class="badge">🌿 EcoEats Feedback</span>

                <div class="titleRow">
                    <div>
                        <h2 style="margin:8px 0 0; font-size:28px;">Order Feedback</h2>
                        <div class="subtle">
                            Your rating helps sellers improve and supports the EcoEats community.
                        </div>
                    </div>

                    <!-- Shows the orderId being used (read-only display) -->
                    <div class="subtle" style="font-weight:800;">
                        Order: <asp:Label ID="lblOrderIdDisplay" runat="server" Text="(not set)"></asp:Label>
                    </div>
                </div>

                <!-- Hidden fields (no OrderID input shown to user) -->
                <asp:HiddenField ID="hfReviewId" runat="server" />
                <asp:HiddenField ID="hfOrderId" runat="server" />

                <label>Overall Rating</label>
                <div class="starsWrap">
                    <div class="stars">
                        <input runat="server" id="star5" type="radio" name="rating" />
                        <label for="star5">&#9733;</label>

                        <input runat="server" id="star4" type="radio" name="rating" />
                        <label for="star4">&#9733;</label>

                        <input runat="server" id="star3" type="radio" name="rating" />
                        <label for="star3">&#9733;</label>

                        <input runat="server" id="star2" type="radio" name="rating" />
                        <label for="star2">&#9733;</label>

                        <input runat="server" id="star1" type="radio" name="rating" />
                        <label for="star1">&#9733;</label>
                    </div>
                </div>

                <div class="grid2">
                    <div>
                        <label>Food Quality</label>
                        <asp:DropDownList ID="ddlQuality" runat="server">
                            <asp:ListItem Text="Select..." Value=""></asp:ListItem>
                            <asp:ListItem Text="Excellent" Value="Excellent"></asp:ListItem>
                            <asp:ListItem Text="Good" Value="Good"></asp:ListItem>
                            <asp:ListItem Text="Okay" Value="Okay"></asp:ListItem>
                            <asp:ListItem Text="Poor" Value="Poor"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div>
                        <label>Pickup/Delivery On Time?</label>
                        <asp:DropDownList ID="ddlOnTime" runat="server">
                            <asp:ListItem Text="Select..." Value=""></asp:ListItem>
                            <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <label>Would you order again?</label>
                <asp:DropDownList ID="ddlAgain" runat="server">
                    <asp:ListItem Text="Select..." Value=""></asp:ListItem>
                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                    <asp:ListItem Text="Maybe" Value="Maybe"></asp:ListItem>
                    <asp:ListItem Text="No" Value="No"></asp:ListItem>
                </asp:DropDownList>

                <label>Comments (optional)</label>
                <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" placeholder="Tell us what went well or what can improve..."></asp:TextBox>

                <div class="btnRow">
                    <asp:Button ID="btnCreate" runat="server" Text="Submit" CssClass="btn btnPrimary" OnClick="btnCreate_Click" />
                    <asp:Button ID="btnUpdate" runat="server" Text="Update Selected" CssClass="btn btnGhost" OnClick="btnUpdate_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btnGhost" OnClick="btnClear_Click" />
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="msg"></asp:Label>
            </div>

            <div class="card">
                <div style="display:flex; justify-content:space-between; align-items:center; gap:12px; flex-wrap:wrap;">
                    <h3 style="margin:0; font-size:20px;">Your Reviews</h3>
                    <div class="subtle">Click <b>Edit</b> to load a review, or <b>Delete</b> to remove it.</div>
                </div>

                <div class="tableWrap" style="margin-top: 10px;">
                    <asp:GridView ID="gvReviews" runat="server" AutoGenerateColumns="False"
                        DataKeyNames="ReviewId"
                        OnSelectedIndexChanged="gvReviews_SelectedIndexChanged"
                        OnRowDeleting="gvReviews_RowDeleting"
                        CssClass="ecoTable">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />
                            <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
                            <asp:BoundField DataField="ReviewId" HeaderText="ID" />
                            <asp:BoundField DataField="OrderId" HeaderText="Order" />
                            <asp:BoundField DataField="Rating" HeaderText="Rating" />
                            <asp:BoundField DataField="Quality" HeaderText="Quality" />
                            <asp:BoundField DataField="OnTime" HeaderText="On Time" />
                            <asp:BoundField DataField="OrderAgain" HeaderText="Order Again" />
                            <asp:BoundField DataField="Comments" HeaderText="Comments" />
                            <asp:BoundField DataField="CreatedAt" HeaderText="Created" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

        </div>
    </div>

</form>
</body>
</html>