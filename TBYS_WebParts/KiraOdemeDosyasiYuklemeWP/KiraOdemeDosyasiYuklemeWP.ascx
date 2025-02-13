<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraOdemeDosyasiYuklemeWP.ascx.cs" Inherits="TBYS_WebParts.KiraOdemeDosyasiYuklemeWP.KiraOdemeDosyasiYuklemeWP" %>
<div class="container w-50">
    <div class="card text-left shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger" runat="server" Text="Banka Extresi (Kira Ödemeleri) Yükleme"></asp:Label>
            </h3>
        </div>

        <div id="BankalarCard" class="card-body text-center">
            <div class="card  m-4">
                <div class="card-header">
                    <asp:Label ID="Vakifbank2Lbl" runat="server" Text="VakıfBank (Extre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF5900"></asp:Label>
                </div>
                <div class="card-body">
                    <asp:FileUpload ID="Vakifbank2FU" runat="server" CssClass="form-control" ToolTip="Vakıfbank Ekstre dosyası (excel) seçiniz" />
                </div>
            </div>
        </div>
        <div id="FooterCard" class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="NextBtn" runat="server" Text="İleri >>" OnClick="NextBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="KiraEkstreListesiBtn" runat="server" Text="Kira Ekstre Listesi" OnClick="NextBtn_Click" />
        </div>
    </div>
</div>
