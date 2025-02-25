<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonelListesiWP.ascx.cs" Inherits="IKYS_WebParts.PersonelListesiWP.PersonelListesiWP" %>

<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Personel Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>

        </div>
        <div class="card-body">
            <div class="form-group col-3">
                <label for="PersonelTipiDDL" class="col-form-label">Personel Tipi</label>
                <div>
                    <asp:DropDownList ID="PersonelTipiDDL" runat="server" class="form-control " OnSelectedIndexChanged="PersonelTipiDDL_SelectedIndexChanged" AutoPostBack="true" style="Height:auto"></asp:DropDownList>
                </div>
            </div>
            <asp:UpdatePanel ID="upPanel" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table small table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th id="dynamic-header" colspan="8"></th>
                                </tr>
                                <tr>
                                    <th>Sıra No</th>
                                    <th>Adı</th>
                                    <th>Soyadı</th>
                                    <th>Ünvan</th>
                                    <th>Personel Tipi</th>
                                    <th>Birim/Şube</th>
                                    <th>Personel Kartı</th>
                                    <th>Kişisel Sayfa</th>
                                    <th>Düzenle</th>
                                    <th>Sicil No</th>
                                    <th>Tahsili</th>
                                    <th>Kullanici Adi</th>
                                    <th>TCKimlikNo</th>
                                    <th>Anne Adı</th>
                                    <th>Baba Adı</th>
                                    <th>Doğum Yeri</th>
                                    <th>Doğum Tar.</th>
                                    <th>Medeni Hali</th>
                                    <th>Evlilik Tar.</th>
                                    <th>Cinsiyet</th>
                                    <th>Kan Grubu</th>
                                    <th>İşe Başlama Tar.</th>
                                    <th>İzin Dönemi Baş. Tar.</th>
                                    <th>SGK Sicil No</th>
                                    <th>Vakıf Öncesi Prim Günü</th>
                                    <th>Emeklilik Tarihi</th>
                                    <th>Calışma Durumu</th>
                                    <th>Cep Telefonu</th>
                                    <th>Adres</th>
                                    <th>İli</th>
                                    <th>İlçesi</th>
                                    <th>Eşi</th>
                                    <th>Eş TC Kimlik No</th>
                                    <th>Eş telefon</th>
                                    <th>Araç-Plakası</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="PersonelTipiDDL" EventName="SelectedIndexChanged" />
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
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
<%--    <asp:UpdateProgress ID="updateProgress" runat="server">
        <ProgressTemplate>
            <div class='loaderMainContainer'>
                <div class='loaderContainer'>
                    <div class='loaderCircle'></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
</div>
