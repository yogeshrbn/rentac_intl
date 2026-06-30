<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="FarmaAPI.WebForm1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtAmount" runat="server"></asp:TextBox>
        <asp:Button ID="btnToWords" runat="server" Text="Convert To Words" OnClick="convertToWords" />
         <asp:Button ID="Button1" runat="server" Text="Convert To Words" OnClick="convertToWords" />
        <asp:Label ID="lblWords" runat="server"></asp:Label>
        <%--<asp:Button ID="Button1" runat="server" Text="Print Report" OnClick="PrintReport" />--%>
        <asp:Button ID="btnSendEmail" runat="server" Text="Send Email" OnClick="SendEmail" />
        <div>
            <asp:ScriptManager ID="scpt" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="rpt" runat="server" Width="900px"></rsweb:ReportViewer>
        </div>

    </form>
</body>
</html>
