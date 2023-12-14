<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YoneticiOzetiWP.ascx.cs" Inherits="Portal_WebParts.YoneticiOzetiWP.YoneticiOzetiWP" %>
<div class="mt-4 mb-4">
    <div class="card shadow">
        <div class="card-header">
            <h2 class="">Özet Bilgiler</h2>
        </div>
        <div class="card-body">
            <div class="grad1 mt-2 mb-2">
                <asp:Label ID="SonNakitBagislarLbl" runat="server" Text=""></asp:Label>
            </div>
            <div class="grad1  mt-2 mb-2">
                <asp:Label ID="BirOncekiGununNakitBagislariLbl" runat="server" Text=""></asp:Label>
            </div>
        </div>
        <div class="card-body">
            <div class="grad2 mt-2 mb-2">
                <asp:Label ID="Label1" runat="server" Text="sdfsdf"></asp:Label>
            </div>
            <div class="grad2  mt-2 mb-2">
                <asp:Label ID="Label2" runat="server" Text="weewewe"></asp:Label>
            </div>
        </div>
        <div class="card-footer">
            <p style='color:gray; font-family: arial;font-size:xx-small;'>TBYS ve NBYS Kayıtlarından Alınmıştır.  Bilgi Sistemleri Kısmı &trade; </p>
        </div>
    </div>
</div>
<style>

.grad1 {
  background-color: lightblue; /* For browsers that do not support gradients */
  background-image: linear-gradient(to right, lightblue , white);

}
.grad2 {
  background-color: lightblue; /* For browsers that do not support gradients */
  background-image: linear-gradient(to right, lightgreen , white);

}
</style>
