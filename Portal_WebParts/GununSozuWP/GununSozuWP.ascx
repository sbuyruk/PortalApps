<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GununSozuWP.ascx.cs" Inherits="Portal_WebParts.GununSozuWP.GununSozuWP" %>

<div class="part-item word-of-day">
    <div class="title">
        <h5>Günün Sözü</h5>
    </div>
    <div class="part-inner">
        <p>
            <asp:Label ID="lblSubject" runat="server"></asp:Label>
        </p>
    </div>
    <asp:Panel ID="pnlAdmin" runat="server">
        <div id="wordOfDayAdd"><asp:HyperLink Target="_popup" ID="lnkDuyuruEkle" CssClass="word-of-day-add-button" runat="server">Günün Sözü Ekle</asp:HyperLink></div>       
    </asp:Panel>
</div>