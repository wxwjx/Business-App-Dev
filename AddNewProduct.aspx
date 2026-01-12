<%@ Page Title="" Language="C#" MasterPageFile="~/SellerPage.Master" AutoEventWireup="true" CodeBehind="AddNewProduct.aspx.cs" Inherits="Business_App_Dev.AddNewProduct" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 801px;
        }
        .auto-style2 {
            height: 31px;
        }
        .auto-style3 {
            width: 1343px;
            height: 31px;
        }
        .auto-style4 {
            width: 801px;
            height: 31px;
        }
        .auto-style5 {
            height: 30px;
            width: 159px;
        }
        .auto-style6 {
            width: 801px;
            height: 30px;
        }
        .auto-style7 {
            width: 159px;
        }
        .auto-style8 {
            height: 31px;
            width: 159px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="auto-style3">
        <tr>
            <td class="auto-style7">Product ID</td>
            <td class="auto-style1">
                <asp:TextBox ID="tb_ProductID" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_ProductID" runat="server" ControlToValidate="tb_ProductID" ErrorMessage="Please enter Product ID" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style7">Product Name</td>
            <td class="auto-style1">
                <asp:TextBox ID="tb_ProductName" runat="server" Width="182px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_ProductName" runat="server" ControlToValidate="tb_ProductName" ErrorMessage="Please Enter Name of the product" ForeColor="Red" Display ="Static" ></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style8">Price</td>
            <td class="auto-style4">
                <asp:TextBox ID="tb_Price" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_Price" runat="server" ControlToValidate="tb_Price" ErrorMessage="Please enter unit price for the product" Display="Static" ForeColor="Red" ></asp:RequiredFieldValidator>
                &nbsp;<asp:CompareValidator ID="cv_Price" runat="server" ControlToValidate="tb_Price" ErrorMessage="Only Numeric value is allowed" ForeColor="Red" Operator="DataTypeCheck" Type="Double"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style8">Quantity</td>
            <td class="auto-style4">
                <asp:TextBox ID="tb_quantity" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_quantity" runat="server" ControlToValidate="tb_quantity" ErrorMessage="Please enter quantity " ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cv_Quantity" runat="server" ErrorMessage="Only numeric interger is allowed" ForeColor="Red" Operator="DataTypeCheck" Type="Integer" ControlToValidate="tb_quantity"></asp:CompareValidator>
            </td>
            <td class="auto-style2"></td>
        </tr>
        <tr>
            <td class="auto-style5">Category</td>
            <td class="auto-style6">
                <asp:TextBox ID="tb_category" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_category" runat="server" ControlToValidate="tb_category" ErrorMessage="Please enter category" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style7">&nbsp;</td>
            <td class="auto-style1">
                <asp:Button ID="btn_Insert" runat="server" OnClick="btn_Insert_Click" Text="Insert" />
                <asp:Button ID="btn_ProductView" runat="server" OnClick="btn_ProductView_Click" Text="View Product List" CausesValidation="false"/>
            </td>
        </tr>
        <tr>
            <td class="auto-style7">&nbsp;</td>
            <td class="auto-style1">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style7">&nbsp;</td>
            <td class="auto-style1">
                &nbsp;</td>
        </tr>
    </table>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
    ShowMessageBox="True" 
    ShowSummary="True" 
    HeaderText="Please fix the following errors:" 
    ForeColor="Red" />

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
