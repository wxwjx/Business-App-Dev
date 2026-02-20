<%@ Page Title="Rate Order | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="RateOrder.aspx.cs"
    Inherits="Business_App_Dev.RateOrder" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
    .ee-hero {
        padding: 28px 0 18px;
    }

        .ee-hero h1 {
            margin: 0;
            font-size: 34px;
        }

        .ee-hero p {
            margin: 8px 0 0;
            opacity: .75;
        }

    .ro-wrap {
        padding: 18px 0 42px;
    }

    .ro-alert {
        background: #fff3f3;
        border: 1px solid #ffd1d1;
        color: #a32020;
        padding: 14px 16px;
        border-radius: 14px;
        margin-bottom: 14px;
    }

    .ro-card {
        background: #fff;
        border: 1px solid rgba(0,0,0,.08);
        border-radius: 18px;
        box-shadow: 0 10px 30px rgba(0,0,0,.06);
        padding: 18px;
    }

    .ro-head {
        margin-bottom: 14px;
    }

    .ro-title {
        font-size: 18px;
        font-weight: 700;
    }

    .ro-sub {
        margin-top: 6px;
        opacity: .7;
    }

    .ro-field {
        margin-top: 14px;
    }

    .ro-label {
        display: block;
        font-weight: 600;
        margin-bottom: 8px;
    }

    .ro-stars {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .ro-star {
        border: 1px solid rgba(0,0,0,.12);
        background: #fff;
        border-radius: 12px;
        padding: 10px 12px;
        cursor: pointer;
        font-size: 18px;
        line-height: 1;
        transition: transform .08s ease, background .12s ease, border-color .12s ease;
    }

        .ro-star:hover {
            transform: translateY(-1px);
        }

        .ro-star.is-on {
            background: rgba(34, 139, 94, .10);
            border-color: rgba(34, 139, 94, .35);
        }

    .ro-score {
        margin-left: 8px;
        font-weight: 700;
        opacity: .75;
    }

    .ro-hint {
        margin-top: 6px;
        font-size: 13px;
        opacity: .7;
    }

    .ro-textarea {
        width: 100%;
        border: 1px solid rgba(0,0,0,.14);
        border-radius: 14px;
        padding: 12px 12px;
        resize: vertical;
        min-height: 120px;
        outline: none;
    }

        .ro-textarea:focus {
            border-color: rgba(34, 139, 94, .55);
            box-shadow: 0 0 0 4px rgba(34, 139, 94, .12);
        }

    .ro-meta {
        display: flex;
        justify-content: space-between;
        margin-top: 8px;
        font-size: 12.5px;
    }

    .ro-muted {
        opacity: .65;
    }

    .ro-count {
        opacity: .75;
        font-weight: 600;
    }

    .ro-actions {
        margin-top: 18px;
        display: flex;
        justify-content: space-between;
        gap: 12px;
        flex-wrap: wrap;
    }

    .ro-btn {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        padding: 10px 14px;
        border-radius: 14px;
        text-decoration: none;
        font-weight: 700;
        border: 1px solid transparent;
        cursor: pointer;
    }

    .ro-btn-primary {
        background: #0f5b3e;
        color: #fff;
        border-color: rgba(0,0,0,.06);
    }

        .ro-btn-primary:hover {
            filter: brightness(0.98);
        }

    .ro-btn-ghost {
        background: #fff;
        color: #0f5b3e;
        border-color: rgba(0,0,0,.14);
    }

        .ro-btn-ghost:hover {
            background: rgba(15,91,62,.06);
        }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- HERO -->
    <section class="ee-hero">
        <div class="ee-container">
            <h1>Rate Your Order</h1>
            <p>Help the seller improve by sharing your experience.</p>
        </div>
    </section>

    <div class="ee-container ro-wrap">

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="ro-alert">
            <asp:Label ID="lblMessage" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlForm" runat="server" CssClass="ro-card">

            <div class="ro-head">
                <div class="ro-title">Your feedback</div>
                <div class="ro-sub">Tap a star and add a short comment (optional).</div>
            </div>

            <!-- Hidden field to store rating (1-5) -->
            <asp:HiddenField ID="hfRating" runat="server" Value="5" />

            <div class="ro-field">
                <label class="ro-label">Rating</label>

                <!-- Stars -->
                <div class="ro-stars" data-target="<%= hfRating.ClientID %>">
                    <button type="button" class="ro-star" data-val="1" aria-label="1 star">★</button>
                    <button type="button" class="ro-star" data-val="2" aria-label="2 stars">★</button>
                    <button type="button" class="ro-star" data-val="3" aria-label="3 stars">★</button>
                    <button type="button" class="ro-star" data-val="4" aria-label="4 stars">★</button>
                    <button type="button" class="ro-star" data-val="5" aria-label="5 stars">★</button>

                    <span class="ro-score" id="roScoreText">5/5</span>
                </div>

                <div class="ro-hint">Your rating is saved with this order.</div>
            </div>

            <div class="ro-field">
                <label class="ro-label" for="<%= txtComment.ClientID %>">Comment</label>

                <asp:TextBox ID="txtComment"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="5"
                    MaxLength="500"
                    CssClass="ro-textarea"
                    placeholder="E.g., pickup was smooth, food was still fresh…" />

                <div class="ro-meta">
                    <span class="ro-muted">Max 500 characters</span>
                    <span class="ro-count" id="roCount">0/500</span>
                </div>
            </div>

            <div class="ro-actions">
                <asp:HyperLink ID="lnkBack" runat="server" CssClass="ro-btn ro-btn-ghost" Text="← Back to Order"
                    NavigateUrl="OrderHistory.aspx" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit Feedback" CssClass="ro-btn ro-btn-primary"
                    OnClick="btnSubmit_Click" />
            </div>

        </asp:Panel>

    </div>

    <script>
        (function () {
            // Stars
            const wrap = document.querySelector(".ro-stars");
            if (wrap) {
                const targetId = wrap.getAttribute("data-target");
                const hidden = document.getElementById(targetId);
                const scoreText = document.getElementById("roScoreText");
                const stars = Array.from(wrap.querySelectorAll(".ro-star"));

                function setRating(val) {
                    hidden.value = String(val);
                    scoreText.textContent = val + "/5";
                    stars.forEach(btn => {
                        const v = Number(btn.getAttribute("data-val"));
                        btn.classList.toggle("is-on", v <= val);
                    });
                }

                // default 5
                setRating(Number(hidden.value || "5"));

                stars.forEach(btn => {
                    btn.addEventListener("click", () => {
                        setRating(Number(btn.getAttribute("data-val")));
                    });
                });
            }

            // Counter
            const ta = document.getElementById("<%= txtComment.ClientID %>");
            const count = document.getElementById("roCount");
            if (ta && count) {
                const update = () => count.textContent = (ta.value.length || 0) + "/500";
                ta.addEventListener("input", update);
                update();
            }
        })();
    </script>

</asp:Content>
