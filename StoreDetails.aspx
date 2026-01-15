<%@ Page Title="" Language="C#" MasterPageFile="~/SellPage.Master" AutoEventWireup="true" CodeBehind="StoreDetails.aspx.cs" Inherits="Business_App_Dev.StoreDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 382px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <table class="w-100">
            <tr>
                <td class="auto-style1">Store Details</td>
                <td>
                    <asp:ImageButton ID="Img_Edit" runat="server" Height="39px" ImageUrl="~/Image/icons8-edit-100.png" OnClick="Img_Edit_Click" Width="41px" />
                </td>
            </tr>
        </table>
    </h2>

    <table class="table">
        <tr>
            <td><strong>Shop Name</strong></td>
            <td><asp:Label ID="lblShopName" runat="server" /></td>
        </tr>
        <tr>
            <td><strong>Address</strong></td>
            <td><asp:Label ID="lblAddress" runat="server" /></td>
        </tr>
        <tr>
            <td><strong>Postal Code</strong></td>
            <td><asp:Label ID="lblPostalCode" runat="server" /></td>
        </tr>
        <tr>
            <td><strong>Pickup Timing</strong></td>
            <td><asp:Label ID="lblPickupTiming" runat="server" /></td>
        </tr>
    </table>


    <asp:Button ID="btnLogout" runat="server" Text="Logout" 
    CssClass="btn btn-danger" OnClick="btnLogout_Click" />

<asp:Label ID="lblMsg" runat="server" ForeColor="Green" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
