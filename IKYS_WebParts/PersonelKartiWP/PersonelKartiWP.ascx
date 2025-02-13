<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonelKartiWP.ascx.cs" Inherits="IKYS_WebParts.PersonelKartiWP.PersonelKartiWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<style>
    .BilgiFormuHeaderCell {
        border-style: solid;
        border-color: black;
        font-family: Arial;
        font-size: large;
        font-weight: bold;
        min-height: 50px;
        text-align: center;
        vertical-align: middle;
    }

    .BilgiFormuCell {
        border-style: solid;
        border-color: black;
        font-family: Arial;
        font-size: small;
        min-height: 30px;
    }
</style>
<div class="container shadow w-50">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-primary font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Personel Bilgi Formu"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <asp:Table ID="PersonelTable" runat="server" CssClass="table table-sm">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="5" CssClass="text-center" Font-Names="Arial" Font-Size="12" Font-Bold="true" Height="35" Style="vertical-align: middle;">TSKGV PERSONEL BİLGİ FORMU</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="5" Font-Names="Arial" Font-Size="12" Font-Bold="true" Height="35" Style="vertical-align: middle;" HorizontalAlign="left">KİMLİK BİLGİLERİ</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF" >Adı</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="3" ID="AdiCell"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" RowSpan="6" ID="ResimCell">
                            <asp:Image HorizontalAlign="center" ID="DisplayImage" Style="height: 192px;" ImageUrl="/PersonelResimleri/personel.jpg" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Soyadı</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="3" ID="SoyadiCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Ünvanı</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="3" ID="UnvaniCell"></asp:TableCell>

                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">T.C.Kimlik No.</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="3" ID="TCKimlikNOCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Doğum Yeri</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="3" ID="DogumYeriCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Doğum Tarihi</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="3" ID="DogumTarihiCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Medeni Hali</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="4" ID="MedeniHaliCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Evlilik Tarihi</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle; text-align: left" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="4" ID="EvlilikTarihiCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">İşe Giriş Tarihi</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="4" ID="IseGirisTarihiCell"></asp:TableCell>
                    </asp:TableRow>
                    <%--<asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle" RowSpan="2" BackColor="#D7F2FF">Kayıtlı Olduğu</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" BackColor="#D8EBBA">İl/İlçe</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="3" ID="NufusIlIlceCell">Sivas Merkez</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" BackColor="#D8EBBA">Mahalle/Köy</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="3" ID="NufusMahalleKoyCell">Kaleardı</asp:TableCell>
                    </asp:TableRow>--%>
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="5" Font-Names="Arial" Font-Size="12" Font-Bold="true" Height="35" Style="vertical-align: middle;" HorizontalAlign="Left">AİLE BİLGİLERİ</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF" Width="110px">Aile Bireyi</asp:TableHeaderCell>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA" Width="100px">Adi</asp:TableHeaderCell>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA" Width="110px">Soyadi</asp:TableHeaderCell>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA" Width="120px">Doğum Tarihi</asp:TableHeaderCell>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA" Width="145px">Mesleği</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="5" Font-Names="Arial" Font-Size="12" Font-Bold="true" Height="35" Style="vertical-align: middle;" HorizontalAlign="left">İLETİŞİM BİLGİLERİ</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Adresi</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="4" ID="AdresCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Ev Telefonu</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="4" ID="EvTelCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D7F2FF">Cep telefonu</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" HorizontalAlign="Left" ColumnSpan="4" ID="CepTelCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="5" Font-Names="Arial" Font-Size="12" Font-Bold="true" Height="35" Style="vertical-align: middle;" HorizontalAlign="left">EĞİTİM VE İŞ DENEYİMİ BİLGİLERİ</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" RowSpan="6" BackColor="#D7F2FF">Tahsili</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA" ColumnSpan="2">Okul/Bölüm</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" BackColor="#D8EBBA">Mez.Tarihi</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Lise</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="LiseOkulCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="LiseMezuniyetTarCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Ön Lisans</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="OnLisansOkulCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="OnLisansMezuniyetTarCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Lisans</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="LisansOkulCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="LisansMezuniyetTarCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Y.Lisans</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YLisansOkulCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YLisansMezuniyetTarCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Doktora</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="DoktoraOkulCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="DoktoraMezuniyetTarCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" RowSpan="4" BackColor="#D7F2FF">Lisan Durumu</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" BackColor="#D8EBBA">Yabancı Dil</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ColumnSpan="3" BackColor="#D8EBBA">Derece/Not(YDS,TOEFL,...)</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDilCell">-</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDilNotuCell" ColumnSpan="3">-</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDil1Cell">-</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDilNotu1Cell" ColumnSpan="3">-</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDil2Cell">-</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle;" Font-Names="Arial" Font-Size="11" Height="25" ID="YabanciDilNotu2Cell" ColumnSpan="3">-</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" Style="vertical-align: middle" RowSpan="4" BackColor="#D7F2FF">İş Deneyimi</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ColumnSpan="2" BackColor="#D8EBBA">Yer</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" BackColor="#D8EBBA">Pozisyon/Ünvan</asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" BackColor="#D8EBBA">Dönem</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="IsyeriCell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="PozisyonCell"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="DonemCell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Isyeri1Cell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Pozisyon1Cell"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Donem1Cell"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Isyeri2Cell" ColumnSpan="2"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Pozisyon2Cell"></asp:TableCell>
                        <asp:TableCell BorderStyle="Solid" BorderColor="Black" ID="Donem2Cell"></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="PersonelListesiBtn" runat="server" Text="Personel Listesi" CausesValidation="false" OnClick="PersonelListesiBtn_Click" />
            <%--<asp:LinkButton CssClass="btn btn-outline-success float-end" ID="PdfBtn" ClientIDMode="Static" runat="server" Text="PDF'e Aktar" OnClick="PdfBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />--%>
        </div>
    </div>
</div>
