<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="eRace.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Security Roles</h2>
    <h3>Clerk</h3>
    <p>Full Name: Susan Sharp <br />
        ID: 49 <br />
        User Name: SusanS<br />
        Password: Password
    </p>

    <h3>Food Service</h3>
    <p>Full Name: Karen Skent<br />
        ID: 44 <br />
        User Name: KarenS<br />
        Password: Password</p>

    <h3>Director</h3>
    <p>Full Name: Cletus Runningwolf<br />
        ID: 35 <br />
        User Name: CRunningwolf<br />
        Password: Password</p>

    <h3>Office Manager</h3>
    <p>Full Name: Karen Yates<br />
        ID: 12 <br />
        User Name: KarenY<br />
        Password: Password</p>

    <h3>Database Connection String</h3>
    <p>
  add name="DefaultConnection"
       connectionString="Data Source=.; Initial Catalog=eRace; Integrated Security=true;"
        providerName="System.Data.Sqlclient"
  add name="eRaceDB"
        connectionString="Data Source=.; Initial Catalog=eRace; Integrated Security=true;"
        providerName="System.Data.Sqlclient" </p>
</asp:Content>
