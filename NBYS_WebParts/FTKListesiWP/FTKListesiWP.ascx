<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FTKListesiWP.ascx.cs" Inherits="NBYS_WebParts.FTKListesiWP.FTKListesiWP" %>

<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }
</style>
<div class="col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            
            <h3 class="mb-2">
                <asp:LinkButton ID="YonergeBtn" class="btn btn-outline-primary float-end mr-4" runat="server" OnClick="YonergeBtn_Click" ToolTip="Kullanım Yönergesi"><i class="fa fa-book" aria-hidden="true"></i></asp:LinkButton>
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Güncel FTK Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="row">
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label" runat="server" for="BolgeDDL">Bölge</asp:Label>
                            <asp:DropDownList ID="BolgeDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="BolgeDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İli</asp:Label>
                            <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İlçesi</asp:Label>
                            <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IlcesiDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label" runat="server" for="GrupDDL">Gösterim</asp:Label>
                            <asp:DropDownList ID="GrupDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="GrupDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-from-label" runat="server" Text="Kurulus Tarihi"></asp:Label>
                            <asp:TextBox ID="KurulusTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static" AutoPostBack="True" OnTextChanged="KurulusTarihiTxt_TextChanged"></asp:TextBox>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-from-label" runat="server" Text="FTK Güncelleme Tarihi"></asp:Label>
                            <asp:TextBox ID="GuncellemeTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static" AutoPostBack="True" OnTextChanged="FTKGuncellemeTarihiTxt_TextChanged"></asp:TextBox>
                        </div>

                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                            <thead>
                                <tr>
                                    <th>İli Id</th>
                                    <th>İlçesi Id</th>
                                    <th>FTK Görevi Id</th>
                                    <th>Bölge</th>
                                    <th>İli</th>
                                    <th>İlçesi</th>
                                    <th>FTK Kuruluş Tarihi</th>
                                    <th>FTK Güncelleme Tarihi</th>
                                    <th>FTK Görevi</th>
                                    <th>Adı Soyadı</th>
                                    <th>Ünvanı</th>
                                    <th>Telefon</th>
                                    <th>KartNo</th>
                                </tr>
                            </thead>
                        </table>
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
        <div class="card-footer">
            <asp:LinkButton ID="FTKIslemleriBtn" CssClass="btn btn-outline-secondary float-end mr-3" runat="server" Text="FTK İşlemleri" OnClick="FTKIslemleriBtn_Click" CausesValidation="False"></asp:LinkButton>
            <asp:LinkButton ID="BolgelereGoreFTKRaporuBtn" runat="server" CssClass="btn btn-outline-secondary float-end mr-3" Text="Bölgelere göre FTK Dağılımı" OnClick="BolgelereGoreFTKRaporuBtn_Click" CausesValidation="False"></asp:LinkButton>
        </div>
    </div>

</div>
