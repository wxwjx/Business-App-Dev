<%@ Page Title="EcoEats | Application Submitted" Language="C#" MasterPageFile="~/Login.Master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/SellerPending.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="login-page pending-page">
        <div class="card pending-card">
            <div class="card-header pending-header">
                <div class="header-title">Application Submitted</div>
                <div class="header-sub">Pending Admin Approval</div>
            </div>

            <div class="card-body pending-body">
                <div class="pending-icon">✅</div>

                <p>
                    Thank you for registering as a seller on <strong>EcoEats</strong>.
                </p>

                <p>
                    Your application is currently <strong>under review</strong>.
                    Once approved by an admin, you will be able to log in and start listing items.
                </p>

                <div class="pending-note">
                    📩 You will be notified via email after approval.
                </div>

                <a href="Login.aspx" class="btn pending-btn">Back to Login</a>
            </div>
        </div>
    </div>
</asp:Content>
