<%@ Page Language="C#" MasterPageFile="~/SellerPage.Master"
    AutoEventWireup="true"
    CodeBehind="Inventory.aspx.cs"
    Inherits="Business_App_Dev.Inventory" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <table class="w-100">
            <tr>
                <td class="auto-style1">Seller Inventory</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </h2>

  <asp:GridView ID="gvProducts"
        runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-striped" 
      OnRowEditing="gvProducts_RowEditing"
    OnRowDeleting="gvProducts_RowDeleting"
      OnRowCancelingEdit="gvProducts_RowCancelingEdit1" 
      OnRowUpdating="gvProducts_RowUpdating1"
    DataKeyNames="ID" >
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="Product ID" />
            <asp:BoundField DataField="Name" HeaderText="Product Name" />
            <asp:BoundField DataField="Price" HeaderText="Price ($)" />
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
            <asp:BoundField DataField="Category" HeaderText="Category" />
            <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowEditButton="True" />
        </Columns>
    </asp:GridView>
    <asp:Button ID="btn_addProduct" runat="server" OnClick="btn_addProduct_Click" Text="Add New Product" UseSubmitBehavior="False" Width="192px" />
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        .auto-style1 {
            width: 620px;
        }
    </style>
</asp:Content>

