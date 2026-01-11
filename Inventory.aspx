<%@ Page Language="C#" MasterPageFile="~/SellerPage.Master"
    AutoEventWireup="true"
    CodeBehind="Inventory.aspx.cs"
    Inherits="Business_App_Dev.Inventory" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Seller Inventory</h2>

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
</asp:Content>
