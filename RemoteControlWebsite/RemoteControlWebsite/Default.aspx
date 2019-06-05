<%@ Page Title="XS Remote Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RemoteControlWebsite._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script runat="server">


    </script>
    <asp:Image runat="server" ImageUrl="~/XSRemoteBigIcon.png"/>
    <p class="lead">Register to access your devices from wherever you want.</p>
    <td colspan="2">
        <asp:Label
            Text="E-Mail:"
            runat="server"
            Font-Italic="true" 
            Width="80"/>
        <asp:TextBox
            ID="m_txtEmail"
            runat="server" />
    </td>
    <br />
    <br />

    <td colspan="2">
        <asp:Label
            Text="Username:"
            runat="server"
            Font-Italic="true" 
            Width="80"/>
        <asp:TextBox
            ID="m_txtUser"
            runat="server" />
    </td>
    <br />
    <br />

    <td colspan="2">
        <asp:Label
            Text="Password:"
            runat="server"
            Font-Italic="true" 
            Width="80"/>
        <asp:TextBox
            ID="m_txtPassword"
            runat="server" 
            TextMode="Password"
            />
    </td>

    <br />
    <br />
    <asp:Button
        runat="server"
        Text="Register"
        OnClick="OnRegisterClick"
        Font-Bold="true"
        ForeColor="Black"
        Height="45"
        Width="150" />
    <br />
    <br />
    <asp:Label 
        ID="lblStatus"
        runat="server"
        />

</asp:Content>
