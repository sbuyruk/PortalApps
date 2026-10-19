<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogDurumTakipWP.ascx.cs" Inherits="Portal_WebParts.LogDurumTakipWP.LogDurumTakipWP" %>

<style type="text/css">
    .pl-wrap{font:14px/1.45 Segoe UI,Arial,sans-serif;color:#19313e;background:#f3f6f7;padding:18px;border-radius:12px}
    .pl-head{background:#132836;color:#fff;border-radius:12px;padding:18px 20px;margin-bottom:14px}.pl-head h2{margin:0 0 4px;font-size:24px}.pl-head p{margin:0;color:#cbdce0}
    .pl-cards{display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:9px;margin-bottom:14px}.pl-card{background:#fff;border:1px solid #dce5e8;border-radius:10px;padding:12px}.pl-card small{display:block;color:#637681;font-size:11px}.pl-card strong{display:block;font-size:22px;margin-top:4px}.pl-filter{background:#fff;border:1px solid #dce5e8;border-radius:10px;padding:12px;margin-bottom:14px;display:flex;flex-wrap:wrap;gap:8px}.pl-filter input,.pl-filter select{border:1px solid #ccd9df;border-radius:7px;padding:7px 9px;min-height:34px}.pl-section{background:#fff;border:1px solid #dce5e8;border-radius:10px;margin-top:14px;overflow:auto}.pl-section h3{font-size:16px;margin:0;padding:13px 15px;border-bottom:1px solid #e6edef}.pl-table{border-collapse:collapse;width:100%;min-width:850px}.pl-table th{background:#f7fafb;color:#647984;font-size:11px;text-align:left;padding:9px;border-bottom:1px solid #dce5e8}.pl-table td{padding:9px;border-bottom:1px solid #edf1f2;font-size:12px;vertical-align:top}.pl-badge{display:inline-block;border-radius:10px;padding:3px 7px;font-weight:700;font-size:11px}.pl-ok{background:#e8f6ef;color:#18745c}.pl-err{background:#fff0ee;color:#ae3e3c}.pl-warn{background:#fff4d9;color:#986511}.pl-detail{display:none;background:#f8fbfb;padding:10px}.pl-detail.open{display:table-row}.pl-detail td{font-size:12px}.pl-code{font:11px Consolas,monospace;white-space:pre-wrap;word-break:break-word;background:#edf2f4;padding:8px;border-radius:6px}.pl-empty{padding:18px;text-align:center;color:#637681}@media(max-width:900px){.pl-cards{grid-template-columns:repeat(2,1fr)}}
</style>

<div class="pl-wrap">
    <div class="pl-head"><h2>Log Durum Takip</h2><p>Portal Logging kayıtları · Ortak PortalLog_Table</p></div>
    <asp:Literal ID="DashboardLiteral" runat="server" />
</div>
