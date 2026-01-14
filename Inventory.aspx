<%@ Page Language="C#" MasterPageFile="~/SellPage.Master"
    AutoEventWireup="true"
    CodeBehind="Inventory.aspx.cs"
    Inherits="Business_App_Dev.Inventory" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
       <div class="page-title">Seller Inventory</div>

    <asp:GridView ID="gvProducts"
        runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-striped"
        DataKeyNames="ProductID"
        OnRowEditing="gvProducts_RowEditing"
        OnRowDeleting="gvProducts_RowDeleting"
        OnRowCancelingEdit="gvProducts_RowCancelingEdit"
        OnRowUpdating="gvProducts_RowUpdating">

        <Columns>

           
            <asp:BoundField DataField="ProductID" HeaderText="Product ID" ReadOnly="true" />

       
            <asp:TemplateField HeaderText="Product Name">
                <ItemTemplate>
                    <%# Eval("ProductName") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtName" runat="server" CssClass="grid-input"
                        Text='<%# Bind("ProductName") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

        
           <%-- <asp:TemplateField HeaderText="Image URL">
                <ItemTemplate>
                    <span class="truncate"><%# Eval("ImageUrl") %></span>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtImageUrl" runat="server" CssClass="grid-input"
                        Text='<%# Bind("ImageUrl") %>' />
                </EditItemTemplate>
            </asp:TemplateField>--%>

         
            <asp:TemplateField HeaderText="Price ($)">
                <ItemTemplate>
                    <%# Eval("PriceNow", "{0:0.00}") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtPriceNow" runat="server" CssClass="grid-input"
                        Text='<%# Bind("PriceNow", "{0:0.00}") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

       
            <asp:TemplateField HeaderText="Old Price ($)">
                <ItemTemplate>
                    <%# Eval("PriceOld", "{0:0.00}") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtPriceOld" runat="server" CssClass="grid-input"
                        Text='<%# Bind("PriceOld", "{0:0.00}") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

         
            <asp:TemplateField HeaderText="Discount (%)">
                <ItemTemplate>
                    <%# Eval("DiscountPercent") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtDiscount" runat="server" CssClass="grid-input"
                        Text='<%# Bind("DiscountPercent") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

          
            <asp:TemplateField HeaderText="Expiry (Hours)">
                <ItemTemplate>
                    <%# Eval("ExpiryHours") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtExpiry" runat="server" CssClass="grid-input"
                        Text='<%# Bind("ExpiryHours") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

        
            <asp:TemplateField HeaderText="Quantity">
                <ItemTemplate>
                    <%# Eval("Quantity") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtQuantity" runat="server" CssClass="grid-input"
                        Text='<%# Bind("Quantity") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

          
            <asp:TemplateField HeaderText="Category">
                <ItemTemplate>
                    <%# Eval("Category") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="grid-input"
                        Text='<%# Bind("Category") %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>

    <asp:Button ID="btn_addProduct"
        runat="server"
        CssClass="btn btn-success btn-add"
        OnClick="btn_addProduct_Click"
        Text="Add New Product"
        UseSubmitBehavior="False" />
</asp:Content>

