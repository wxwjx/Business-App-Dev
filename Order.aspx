<%@ Page Title="Order" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="Business_App_Dev.Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <a href="Default.aspx">← Back to Home</a>

    <h1><asp:Label ID="lblName" runat="server" /></h1>
    <p><asp:Label ID="lblDesc" runat="server" /></p>

    <asp:Image ID="imgProduct" runat="server"
    Style="max-width:600px;border-radius:16px;margin-bottom:16px;" />


    <div>
        <span style="font-size:24px;font-weight:600;">
            <asp:Label ID="lblPrice" runat="server" />
        </span>
        <span style="text-decoration:line-through;margin-left:8px;color:#999;">
            <asp:Label ID="lblOldPrice" runat="server" />
        </span>
    </div>

</asp:Content>
