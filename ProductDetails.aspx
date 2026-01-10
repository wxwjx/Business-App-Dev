<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="Business_App_Dev.ProductDetails" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Product Details</title>
</head>
<body>
    <form id="form1" runat="server">

        <asp:Image ID="imgProduct" runat="server" Width="320" />

        <h2><asp:Label ID="lblName" runat="server" /></h2>

        <p><asp:Label ID="lblSubtitle" runat="server" /></p>

        <p>
            <strong>$<asp:Label ID="lblPriceNow" runat="server" /></strong>
            <del>$<asp:Label ID="lblPriceOld" runat="server" /></del>
        </p>

        <p>Rating: <asp:Label ID="lblRating" runat="server" /></p>
        <p>Reviews: <asp:Label ID="lblReviews" runat="server" /></p>
        <p>Distance: <asp:Label ID="lblDistance" runat="server" /> km</p>
        <p>Expiry: <asp:Label ID="lblExpiry" runat="server" /> hours</p>

    </form>
</body>
</html>
