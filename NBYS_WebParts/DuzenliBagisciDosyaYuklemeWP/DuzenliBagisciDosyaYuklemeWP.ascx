<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuzenliBagisciDosyaYuklemeWP.ascx.cs" Inherits="NBYS_WebParts.DuzenliBagisciDosyaYuklemeWP.DuzenliBagisciDosyaYuklemeWP" %>
<div class="container w-50">
    <div class="card text-left shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <a class=" btn btn-outline-primary float-end me-4" runat="server" id="YonergeLnk"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label CssClass="col-form-label text-danger" runat="server" Text="Düzenli Bağışçı Listesi Yükleme "></asp:Label>
            </h3>
        </div>

        <div class="card-body text-center">
            <div class="card  m-4">
                <div class="card-header">
                    <asp:Label ID="DosyaLbl" runat="server" Text="Düzenli Bağışçı Dosyası" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF5900"></asp:Label>
                </div>
                <div class="card-body">
                    <asp:FileUpload ID="DosyaFU" runat="server" CssClass="form-control" ToolTip="Düzenli bağışçı dosyası (excel) seçiniz" />
                </div>
            </div>
        </div>
        <div id="FooterCard" class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="DuzenliBagisciListesiBtn" runat="server" Text="Düzenli Bağışçı Listesi" OnClick="DuzenliBagisciListesiBtn_Click" />
        </div>
    </div>
</div>
