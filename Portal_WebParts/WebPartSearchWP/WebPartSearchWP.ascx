<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebPartSearchWP.ascx.cs" Inherits="Portal_WebParts.WebPartSearchWP.WebPartSearchWP" %>

<table cellpadding="4" cellspacing="0">
    <tr>
        <td>Site URL:</td>
        <td><asp:TextBox ID="txtSiteUrl" runat="server" Width="400px">http://tskgv-dev3</asp:TextBox></td>
    </tr>
    <tr>
        <td>Web Part Adı:</td>
        <td><asp:TextBox ID="txtWebPartName" runat="server" Width="400px">OlayListesiWP</asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:LinkButton ID="lnkSearch" runat="server" Text="Ara" OnClick="lnkSearch_Click" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label ID="lblStatus" runat="server" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" EmptyDataText="Sonuç bulunamadı .">
                <Columns>
                    <asp:BoundField DataField="PageUrl" HeaderText="Sayfa" />
                    <asp:BoundField DataField="WebUrl" HeaderText="Site" />
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
