<%@ Page Language="C#" MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true"
    CodeBehind="AddNewProduct.aspx.cs"
    Inherits="Business_App_Dev.AddNewProduct" %>

<asp:Content ID="HeadBlock" ContentPlaceHolderID="head" runat="server">
        <link href="<%= ResolveUrl("~/Content/EcoEats.css") %>" rel="stylesheet" />

    <style>
        .form-wrap{max-width:820px;}
        .form-title{font-size:24px;font-weight:800;margin:12px 0 18px;}
        .form-row{display:grid;grid-template-columns:180px 1fr;gap:12px;align-items:center;margin:10px 0;}
        .form-row label{font-weight:600;}
        .input{width:100%;padding:8px 10px;border:1px solid #d0d7de;border-radius:8px;}
        .actions{margin-top:14px;display:flex;gap:10px;}
        .hint{opacity:.7;font-size:12px;margin-top:6px;}
        .err{color:red;}
    </style>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="form-wrap">
        <div class="form-title">Add New Product</div>

        <asp:ValidationSummary ID="ValidationSummary1" runat="server"
            ShowMessageBox="True" ShowSummary="False"
            HeaderText="Please fix the following errors:" ForeColor="Red" />

        <div class="form-row">
            <label>Product Name</label>
            <div>
                <asp:TextBox ID="tb_ProductName" runat="server" CssClass="input" />
                <asp:RequiredFieldValidator ID="rfv_ProductName" runat="server"
                    ControlToValidate="tb_ProductName" ErrorMessage="Product Name is required"
                    CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Subtitle (optional)</label>
            <div>
                <asp:TextBox ID="tb_Subtitle" runat="server" CssClass="input" />
            </div>
        </div>

        <div class="form-row">
            <label>Image URL (optional)</label>
            <div>
                <asp:TextBox ID="tb_ImageUrl" runat="server" CssClass="input" />
                <div class="hint">Example: https://images…/meal.jpg</div>
            </div>
        </div>

        <div class="form-row">
            <label>Price</label>
            <div>
                <asp:TextBox ID="tb_Price" runat="server" CssClass="input" />
                <asp:RequiredFieldValidator ID="rfv_Price" runat="server"
                    ControlToValidate="tb_Price" ErrorMessage="Price is required"
                    CssClass="err" Display="Dynamic" />
                <asp:CompareValidator ID="cv_Price" runat="server"
                    ControlToValidate="tb_Price" Operator="DataTypeCheck" Type="Double"
                    ErrorMessage="Price must be a number" CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Old Price (optional)</label>
            <div>
                <asp:TextBox ID="tb_OldPrice" runat="server" CssClass="input" />
                <asp:CompareValidator ID="cv_OldPrice" runat="server"
                    ControlToValidate="tb_OldPrice" Operator="DataTypeCheck" Type="Double"
                    ErrorMessage="Old Price must be a number" CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Discount Percent (optional)</label>
            <div>
                <asp:TextBox ID="tb_DiscountPercent" runat="server" CssClass="input" />
                <asp:CompareValidator ID="cv_Discount" runat="server"
                    ControlToValidate="tb_DiscountPercent" Operator="DataTypeCheck" Type="Integer"
                    ErrorMessage="Discount must be an integer" CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Expiry Hours</label>
            <div>
                <asp:TextBox ID="tb_Expiry" runat="server" CssClass="input" />
                <asp:RequiredFieldValidator ID="rfv_Expiry" runat="server"
                    ControlToValidate="tb_Expiry" ErrorMessage="Expiry Hours is required"
                    CssClass="err" Display="Dynamic" />
                <asp:CompareValidator ID="cv_Expiry" runat="server"
                    ControlToValidate="tb_Expiry" Operator="DataTypeCheck" Type="Integer"
                    ErrorMessage="Expiry must be an integer" CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Quantity</label>
            <div>
                <asp:TextBox ID="tb_quantity" runat="server" CssClass="input" />
                <asp:RequiredFieldValidator ID="rfv_quantity" runat="server"
                    ControlToValidate="tb_quantity" ErrorMessage="Quantity is required"
                    CssClass="err" Display="Dynamic" />
                <asp:CompareValidator ID="cv_Quantity" runat="server"
                    ControlToValidate="tb_quantity" Operator="DataTypeCheck" Type="Integer"
                    ErrorMessage="Quantity must be an integer" CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Category</label>
            <div>
                <asp:TextBox ID="tb_category" runat="server" CssClass="input" />
                <asp:RequiredFieldValidator ID="rfv_category" runat="server"
                    ControlToValidate="tb_category" ErrorMessage="Category is required"
                    CssClass="err" Display="Dynamic" />
            </div>
        </div>

        <div class="actions">
            <asp:Button ID="btn_Insert" runat="server" Text="Insert"
                CssClass="btn btn-success" OnClick="btn_Insert_Click" />
            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel"
                CssClass="btn btn-secondary" CausesValidation="false"
                OnClick="btn_Cancel_Click" />
        </div>
    </div>

</asp:Content>
