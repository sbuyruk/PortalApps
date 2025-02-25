<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KPSSorguSonucuWP.ascx.cs" Inherits="TBYS_WebParts.KPSSorguSonucuWP.KPSSorguSonucuWP" %>

<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Nüfus Sorgu Sonuçları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group ">

                <div class="form-group">
                    <asp:Label ID="DosyaYukleLbl" runat="server" Text="Sonuç Dosyasını Yükleyin" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF5900"></asp:Label>
                </div>
                <div class="form-group">
                    <asp:FileUpload ID="DosyaYukleFU" runat="server" CssClass="form-control" ToolTip="Sonuç dosyasını (SonucListesi.json) seçiniz" />
                </div>
                <div class="form-group">
                    <asp:LinkButton ID="DosyayiYukleBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Dosyayı Yükle " OnClick="DosyayiYukleBtn_Click" />
                </div>
            </div>

                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>TC Kimlik No</th>
                                    <th>Adı Soyadı</th>
                                    <th>Sağ/Vefat</th>
                                    <th>Hata/Açıklama</th>
                                </tr>
                            </thead>
                        </table>
                    </div>

            <div class="card-footer">
                

            </div>
        </div>
    </div>
</div>
