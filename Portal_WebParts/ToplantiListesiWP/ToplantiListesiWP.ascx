<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToplantiListesiWP.ascx.cs" Inherits="Portal_WebParts.ToplantiListesiWP.ToplantiListesiWP" %>

<style>
    .pasif-toplanti {
        background-color: lightgrey !important;
        color: black !important;
    }

    .bugunku-toplanti {
        background-color: lightcyan !important;
        color: black !important;
    }
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18.5em;
        font-size: small;
    }
    table tr td {
        font-size: small;
    }
</style>

<div class="col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Toplantı Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group alert-secondary p-2">
                <div class="form-group ">
                    <div class="row">
                        <div class="col-2">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="col-form-label DateTimePickerV2 font-weight-bold">İlk Tarihi</asp:Label>
                                <asp:TextBox ID="BaslangicTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" placeholder="Tarih Seçiniz" AutoPostBack="true" OnTextChanged="BaslangicTarihiTxt_TextChanged" ></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Son Tarih</asp:Label>
                                <asp:TextBox ID="BitisTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="BitisTarihiTxt_TextChanged" placeholder="Tarih Seçiniz" ></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                            <thead>
                                <tr>
 
                                    <th>Başlama Zamanı</th>
                                    <th>Bitiş Zamanı</th>
                                    <th>Konusu</th>
                                    <th>Koordinatör</th>
                                    <th>Yeri</th>
                                    <th>İç Katılımcılar</th>
                                    <th>Dış Katılımcılar</th>
                                    <th>Bilgi</th>
                                    <th>Açıklama</th>
                                    <th>Çev. İçi</th>
                                    <th>Düzenle</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BaslangicTarihiTxt" EventName="TextChanged" />
                    <asp:AsyncPostBackTrigger ControlID="BitisTarihiTxt" EventName="TextChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:UpdateProgress ID="updateProgress" runat="server">
                <ProgressTemplate>
                    <div class='loaderMainContainer'>
                        <div class='loaderContainer'>
                            <div class='loaderCircle'></div>
                        </div>
                    </div>

                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Toplantı" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="ToplantiTakvimiBtn" CssClass="btn btn-outline-info float-end" runat="server" Text="Toplantı Takvimi" OnClick="ToplantiTakvimiBtn_Click"></asp:LinkButton>
        </div>
    </div>
</div>