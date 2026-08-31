<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraBorcuYazisiWP.ascx.cs" Inherits="TBYS_WebParts.KiraBorcuYazisiWP.KiraBorcuYazisiWP" %>

<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Borcu Bildirimi Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row border m-1">
                        <div class="form-group col-2 ">
                            <div class="form-group" style="display: none">
                                <label class="col-form-label" for="AyDDL">Ay </label>
                                <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group" style="display: none">
                                <label class="col-form-label" for="YilDDL">Yıl </label>
                                <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="BolgeDDL">Bölge </label>
                                <asp:DropDownList ID="BolgeDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="BolgeDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                        </div>
                        <div class="form-group col-2">
                            <div class="form-group">
                                <label for="EvrakSayisiYiliTxt" class="col-form-label">Evrak Sayısı Yılı</label>
                                <asp:TextBox ID="EvrakSayisiYiliTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="EvrakTarihiTxt" class="col-form-label">Evrak Tarihi</label>
                                <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" ></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-2">
                            <div class="form-group">
                                <label for="GecerlilikTarihiTxt" class="col-form-label">Geçerlilik Tarihi</label>
                                <asp:TextBox ID="GecerlilikTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="SonOdemeTarihiTxt" class="col-form-label">Son Ödeme Tarihi</label>
                                <asp:TextBox ID="SonOdemeTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-4">
                            <div class="form-group">
                                <label for="Parafe1Txt" class="col-form-label">Parafe Eden (1)</label>
                                <asp:TextBox ID="Parafe1Txt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="Parafe2Txt" class="col-form-label">Parafe Eden (2)</label>
                                <asp:TextBox ID="Parafe2Txt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-2">
                            <div class="form-group">
                                <label for="ImzalayanTxt" class="col-form-label">İmza (Adi Soyadı)</label>
                                <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="ImzalayanMakamTxt" class="col-form-label">İmza (Makam)</label>
                                <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="form-group border m-1">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Kiracı</th>
                                    <th>Bölge</th>
                                    <th>Borç</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="form-group">
                            <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Kira Borcu Dosyasını Aç</asp:HyperLink>
                        <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiket Dosyasını Aç</asp:HyperLink>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="YaziyiOlusturBtn" runat="server" CssClass="btn btn-outline-success" OnClick="YaziyiOlusturBtn_Click">Kira Borcu Yazısını oluştur </asp:LinkButton>
                </div>
            </div>
        </ContentTemplate>
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

