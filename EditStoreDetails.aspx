<%@ Page Title="" Language="C#" MasterPageFile="~/SellPage.Master" AutoEventWireup="true" CodeBehind="EditStoreDetails.aspx.cs" Inherits="Business_App_Dev.EditStoreDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .auto-style1 {
            height: 35px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Edit Store Details</h2>
<asp:ValidationSummary ID="vsSummary" runat="server" ShowSummary="True" ForeColor="Red" />

<table class="store-table">
    <tr>
        <td class="auto-style1">Shop Name</td>
        <td class="auto-style1"><asp:TextBox ID="tbShopName" runat="server" Width="250px" />
            <asp:RequiredFieldValidator ID="rfvShopName" runat="server" ControlToValidate="tbShopName" ErrorMessage="Shop Name is required" ForeColor="Red"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td>Address</td>
        <td><asp:TextBox ID="tbAddress" runat="server" Width="250px" />
            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="tbAddress" ErrorMessage="Address is required" ForeColor="Red"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td>Postal Code</td>
        <td><asp:TextBox ID="tbPostalCode" runat="server" Width="150px" />
            <asp:RequiredFieldValidator ID="rfvPostalCode" runat="server" ControlToValidate="tbPostalCode" ErrorMessage="Postal Code is required" ForeColor="Red"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revPostal" runat="server" ControlToValidate="tbPostalCode"
    ErrorMessage="Postal Code must be 6 digits" ForeColor="Red" ValidationExpression="\d{6}" Display="Dynamic" />

    </tr>
    <tr>
        <td class="auto-style1">Pickup Timing</td>
        <td class="auto-style1"><asp:TextBox ID="tbPickupTime" runat="server" Width="150px" />
            <asp:RequiredFieldValidator ID="rfvPickup" runat="server"
                    ControlToValidate="tbPickupTime" ErrorMessage="Pickup Window is required"
                    CssClass="err" Display="Dynamic" ForeColor="Red" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="False" />
        </td>
    </tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
