<%@ Page Title="Seller Feedback | EcoEats"
    Language="C#"
    MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true"
    CodeBehind="SellerFeedback.aspx.cs"
    Inherits="Business_App_Dev.SellerFeedbackPage" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="ee-container">
        <h2>Customer Feedback</h2>

        <asp:Repeater ID="rptFeedback" runat="server">
            <ItemTemplate>
                <div class="card mb-3 p-3">
                    <strong>Rating:</strong> <%# Eval("Rating") %> ⭐
                    <br />
                    <strong>Comment:</strong> <%# Eval("Comment") %>
                    <br />
                    <small>Order #<%# Eval("OrderID") %> —
                    <%# Eval("CreatedAt") %></small>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</asp:Content>
