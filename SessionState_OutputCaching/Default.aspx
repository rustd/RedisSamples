<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SessionState_OutputCaching.Default" %>
<%@ OutputCache Duration="100" VaryByParam="*" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                Please set the redis host name and accesskey in web.config
            <ul>
                <li><a href="https://msdn.microsoft.com/en-us/library/azure/dn798898.aspx">Redis OutputCache Provider </a></li>
                <li><a href="https://msdn.microsoft.com/en-us/library/azure/dn690522.aspx">Redis SessionState Provider </a></li>
            </ul>

            </p>
            This value is from Redis SessionState Provider <%= Session["redis"] %>
        </div>
    </form>
</body>
</html>
