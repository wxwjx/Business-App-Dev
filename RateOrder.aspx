<%@ Page Title="Rate Order | EcoEats"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="RateOrder.aspx.cs"
    Inherits="Business_App_Dev.RateOrder" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="ee-container">

        <h2>Rate Your Order</h2>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />

        <asp:Panel ID="pnlForm" runat="server">

            <label>Rating (1–5)</label>
            <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-control">
                <asp:ListItem Value="5">5 - Excellent</asp:ListItem>
                <asp:ListItem Value="4">4 - Good</asp:ListItem>
                <asp:ListItem Value="3">3 - Average</asp:ListItem>
                <asp:ListItem Value="2">2 - Poor</asp:ListItem>
                <asp:ListItem Value="1">1 - Very Poor</asp:ListItem>
            </asp:DropDownList>

            <br />

            <label>Comment</label>
            <asp:TextBox ID="txtComment" runat="server"
                TextMode="MultiLine"
                Rows="4"
                CssClass="form-control" />

            <br />

            <asp:Button ID="btnSubmit"
                runat="server"
                Text="Submit Feedback"
                CssClass="btn btn-primary"
                OnClick="btnSubmit_Click" />

        </asp:Panel>

    </div>

</asp:Content>
