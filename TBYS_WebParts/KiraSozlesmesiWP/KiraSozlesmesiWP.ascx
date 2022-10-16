<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraSozlesmesiWP.ascx.cs" Inherits="TBYS_WebParts.KiraSozlesmesiWP.KiraSozlesmesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }
</style>
<script type="text/javascript">
    function OpenModal() {
        $("#OnayModal").modal({ backdrop: "static" });
    }
    function CloseModal() {
        $("#OnayModal").modal('hide');

    }

    function FaizTutariHesapla() {
        var devirAnaPara = $('#<%= DevirAnaParaTxt.ClientID%>').val().replace(/\./g, "").replace(",", ".");
        var devirFaizliBakiye = $('#<%= DevirFaizliBakiyeTxt.ClientID%>').val().replace(/\./g, "").replace(",", ".");

        if (!devirAnaPara || devirAnaPara == undefined || devirAnaPara == "")
            devirAnaPara = 0;
        if (!devirFaizliBakiye || devirFaizliBakiye == undefined || devirFaizliBakiye == "")
            devirFaizliBakiye = 0;
        var sonuc = parseFloat(devirFaizliBakiye) - parseFloat(devirAnaPara);
        $('#<%= DevirFaizTutariTxt.ClientID%>').val(sonuc);
 
    }
    function DevirAlBtnClick() {
        document.getElementById('<%= DevirAlBtn.ClientID%>').click();

    }

</script>
<link rel="stylesheet" href="/Style Library/tskgv/css/fancybox.css" />
<script src="/Style Library/tskgv/js/fancybox.umd.js"></script>
<script src="/Style Library/tskgv/js/fancybox.esm.js"></script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <asp:Image ID="AktifPasifImg" ClientIDMode="Static" runat="server" ImageUrl="../_layouts/19/images/TBYS_WebParts/belli-degil.png" CssClass="float-right" onerror="this.src='../TBYSResimleri/belli-degil.png';" />
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold" ID="TitleLbl" runat="server" Text="Kira Sözleşmesi"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server" style="display: block;"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <div class="row pr-2">
                        <div id="TasinmazListDiv" class="form-group col-5">
                            <div id="DevirDiv" runat="server" class="form-group border border-dark p-2">
                                <div class="form-group border-bottom text-center">
                                    <label class="col-form-label font-weight-bold" for="DevirAnaParaTxt">Devir</label>
                                </div>
                                <div class="row ">
                                    <div class="form-group col-3">
                                        <label class="col-form-label" for="DevirAnaParaTxt">AnaPara</label>
                                        <input type="text" id="DevirAnaParaTxt" runat="server" class="form-control input-money text-right" tooltip="Önceki Sözleşmeden devreden borç (ana para)"
                                            onchange="FaizTutariHesapla()" onkeyup="FaizTutariHesapla()" oncut="FaizTutariHesapla()" onpaste="FaizTutariHesapla()" oninput="FaizTutariHesapla()" />
                                    </div>
                                    <div class="form-group col-3">
                                        <label class="col-form-label " for="DevirFaizTutariTxt">Faiz </label>
                                        <asp:TextBox type="text" ID="DevirFaizTutariTxt" runat="server" CssClass="form-control input-money text-right bg-secondary" ToolTip="Önceki Sözleşmeden devreden faiz tutarı" />
                                    </div>
                                    <div class="form-group col-3">
                                        <label class="col-form-label " for="DevirFaizliBakiyeTxt">FaizliBakiye</label>
                                        <input type="text" id="DevirFaizliBakiyeTxt" runat="server" class="form-control input-money text-right" tooltip="Önceki Sözleşmeden devreden faizli bakiye"
                                            onchange="FaizTutariHesapla()" onkeyup="FaizTutariHesapla()" oncut="FaizTutariHesapla()" onpaste="FaizTutariHesapla()" oninput="FaizTutariHesapla()" />
                                    </div>
                                    <div class="form-group col-3">
                                        <label class="col-form-label text-white" >......... .......</label>
                                        <asp:LinkButton CssClass="btn btn-danger" ID="DevirAlBtn" runat="server" CausesValidation="false" Text="Devir Al" OnClick="DevirAlBtn_Click" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group table border border-dark " >
                                <div class="form-group">
                                    <asp:LinkButton ID="KiraciTasinmazDegistirBtn" runat="server" CssClass="btn btn-primary float-right" Text="Kiracı/Taşınmaz Değiştir" OnClick="KiraciTasinmazDegistirBtn_Click"></asp:LinkButton>
                                </div>
                                <div class="form-group">
                                    <asp:Table ID="KiralikTable" runat="server" CssClass="table table-bordered table-striped"></asp:Table>
                                </div>

                            </div>
                            <div id="SozlesmeDurumuDiv" runat="server" class="p-1 border border-dark alert-secondary" style="display:none">
                                <div class="row form-group ">
                                    <label class="col-form-label col-5" for="SozlesmeDurumuDDL">Sözleşme Durumu</label>
                                    <div class="col-7">
                                        <asp:DropDownList ID="SozlesmeDurumuDDL" runat="server" class="form-control" ></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row form-group">
                                    <label class="col-form-label col-5" for="SozlesmeDurumuDDL">Değişme Tarihi</label>
                                    <div class="col-7">
                                        <input type="text" id="DurumDegismeTarTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group border border-dark p-2" style="background-color: aliceblue">
                                <div class="form-group">
                                    <asp:Label CssClass="col-form-label font-weight-bold" ID="Label3" runat="server">Sözleşme Formu</asp:Label>
                                </div>
                                <div class="form-group text-center">
                                    <a id="DosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Sözleşmeyi Görüntüle
                                    </a>
                                    <asp:LinkButton ID="BelgeSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="BelgeSilBtn_Click"
                                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                                </div>
                                <div class="form-group">
                                    <asp:FileUpload ID="BelgeYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Dosyayı Seçiniz" type="text" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group col-7 border-top">
                            <div class="row">
                                <div class="form-group col-3 border-right">
                                    <div class="form-group">
                                        <label class="col-form-label " for="DosyaNoTxt">Dosya No</label>
                                        <asp:TextBox ID="DosyaNoTxt" runat="server" CssClass="form-control input-integer" ToolTip="Dosya No"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label " for="IlkSozlesmeTarTxt">İlk Söz.Tar.</label>
                                        <input type="text" id="IlkSozlesmeTarTxt" name="IlkSozlesmeTarTxt" class="form-control input-date DateTimePickerV1" runat="server" placeholder="dd.MM.yyyy" />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="SozBasTarTxt">Söz.Baş.Tar.</label>
                                        <input type="text" id="SozBasTarTxt" name="SozBasTarTxt" class="form-control input-date DateTimePickerV1" runat="server" placeholder="dd.MM.yyyy"/>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="SozBitTarTxt">Söz.Bit.Tar.</label>
                                        <input type="text" id="SozBitTarTxt" name="SozBitTarTxt" class="form-control input-date DateTimePickerV1" runat="server" placeholder="dd.MM.yyyy"/>
                                    </div>

                                </div>
                                <div class="form-group col-3 border-right">
                                    <div class="form-group">
                                        <label class="col-form-label" for="OdemeSekliDDL">Ödeme Şekli</label>
                                        <asp:DropDownList ID="OdemeSekliDDL" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="OdemeSekliDDL_SelectedIndexChanged" Height="34px"></asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="TaksitSayisiTxt">Taksit Sayısı</label>
                                        <asp:TextBox ID="TaksitSayisiTxt" runat="server" CssClass="form-control input-integer" type="number" step="1" min="1" max="12" >12</asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="KiraBedeliTxt">Kira Bedeli</label>
                                        <input class="form-control input-money text-right " id="KiraBedeliTxt" runat="server" />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="ArtisAyiTxt">Artış Ayı</label>
                                        <asp:TextBox ID="ArtisAyiTxt" runat="server" CssClass="form-control input-integer" type="number" step="1" min="1" max="12" ToolTip="Artış Yenileme Ayı" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group col-6 border border-dark pr-2">
                                    <div class="row">
                                        <div class="form-group col-6 border-right ">
                                            <div class="form-group">
                                                <label class="col-form-label " for="TeminatCinsiTxt">Teminat Cinsi</label>
                                                <asp:TextBox ID="TeminatCinsiTxt" runat="server" CssClass="form-control" ToolTip="Teminat Cinsi" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <label class="col-form-label" for="TeminatTutariTxt">Belirlenen Teminat</label>
                                                <input class="form-control input-money text-right" id="TeminatTutariTxt" runat="server" readonly="readonly" />
                                            </div>
                                            <div class="form-group">
                                                <label class="col-form-label" for="IadeTeminatTutariTxt">İade-Mahsup</label>
                                                <input class="form-control input-money text-right" id="IadeTeminatTutariTxt" runat="server" readonly="readonly" />
                                            </div>
                                        </div>
                                        <div class="form-group col-6 ">
                                            <div class="form-group">
                                                <label class="col-form-label" for="TeminatOdemeTarTxt">Teminat Tarihi</label>
                                                <input type="text" id="TeminatOdemeTarTxt" name="TeminatOdemeTarTxt" class="form-control" runat="server" readonly="readonly" />
                                            </div>
                                            <div class="form-group">
                                                <label class="col-form-label" for="OdenenTeminatTutariTxt">Ödenen Teminat</label>
                                                <input class="form-control input-money text-right" id="OdenenTeminatTutariTxt" runat="server" readonly="readonly" />
                                            </div>
                                            <div class="form-group">
                                                <label class="col-form-label" for="KalanTeminatTutariTxt">Kalan Teminat</label>
                                                <input class="form-control input-money text-right" id="KalanTeminatTutariTxt" runat="server" readonly="readonly" />
                                            </div>

                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:TextBox ID="TeminatAciklamaTxt" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" ToolTip="Teminat Açıklama" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group pb-2">
                                        <asp:LinkButton ID="TeminatIslemleriBtn" CssClass="btn btn-outline-primary float-right" runat="server" Text="Teminat İşlemleri" OnClick="TeminatIslemleriBtn_Click" />
                                    </div>
                                </div>

                            </div>
                            <div class="row  border-top">
                                <div class="form-group col-5">
                                    <label class="col-form-label" for="KefilAdiSoyadiTxt">Kefil</label>
                                    <asp:TextBox ID="KefilAdiSoyadiTxt" runat="server" CssClass="form-control" ToolTip="Kefilin Adı Soyadı"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="KefilTcKimlikNoTxt">Kefil TC K.No</label>
                                    <asp:TextBox ID="KefilTcKimlikNoTxt" runat="server" CssClass="form-control" ToolTip="Kefilin TC Kimlik numarası"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="KefilTelTxt">Kefil Tel</label>
                                    <asp:TextBox ID="KefilTelTxt" runat="server" CssClass="form-control" ToolTip="Kefilin Telefonu"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label" for="KefilAdresTxt">Kefil Adres</label>
                                <asp:TextBox ID="KefilAdresTxt" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" ToolTip="Kefil Ardes"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" ToolTip="Açıklama"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div>
                        <asp:Label ID="UyariLbl" class="text-danger" runat="server"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Sonraki=>" OnClick="NextBtn_Click" />
            <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="<=Önceki" OnClick="PrevBtn_Click" />

            <asp:LinkButton ID="KiraSozlesmeListBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Sözleşme Listesi" OnClick="KiraSozlesmeListBtn_Click" />
            <asp:LinkButton ID="KiraciBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kiracı" OnClick="KiraciBtn_Click" />
            <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Ödeme Planı" OnClick="OdemePlaniGoruntuleBtn_Click" Visible="false"></asp:LinkButton>

            
            <asp:LinkButton ID="UpdateBtn" runat="server" CssClass="btn btn-primary" Text="Sözleşme Güncelle" OnClick="UpdateBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="DeleteBtn" runat="server" CssClass="btn btn-danger" Text="Sözleşmeyi Sil" OnClick="DeleteBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="SozlesmeYenileBtn" runat="server" CssClass="btn btn-warning" Text="Sözleşmeyi Yenile" OnClick="SozlesmeYenileBtn_Click" Visible="false"></asp:LinkButton>
            <asp:LinkButton ID="SozlesmeyiBitirBtn" runat="server" CssClass="btn " Text="Sözleşmeyi Bitir" OnClick="SozlesmeyiBitirBtn_Click" Visible="false" BackColor="Black" ForeColor="White"></asp:LinkButton>
            <asp:LinkButton ID="SozlesmeyiFeshetBtn" runat="server" CssClass="btn " Text="Sözleşmeyi Feshet" OnClick="SozlesmeyiFeshetBtn_Click" Visible="false" BackColor="#000099" ForeColor="White"></asp:LinkButton>
        </div>
        <!-- Modal -->
        <div class="modal" id="OnayModal" role="dialog">
            <div class="modal-dialog modal-lg">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="card">
                        <div class="card-header">
                            <h3>
                                <asp:Label ID="ModalLbl" class="col-form-label" Text="Sözleşme Güncelleme" runat="server"></asp:Label></h3>
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label ID="MessageLbl" runat="server" class="col-form-label"></asp:Label>
                                <asp:HiddenField ID="SenderHF" runat="server" />
                            </div>
                            <div class="form-group row">
                                <div class="form-group col-3">
                                     <asp:Label ID="DurumDegisimTarihiLbl" runat="server" class="col-form-label">Tarih </asp:Label>
                                    <asp:TextBox type="ModalDurumDegismeTarTxt" ID="ModalDurumDegismeTarTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" ReadOnly="true" />
                                </div>
                               <div class="form-group col" id="ArtisOraniDiv" runat="server" style="display:none">
                                   <asp:Label ID="Label1" runat="server" class="col-form-label">Artış Oranı </asp:Label>
                                   <asp:TextBox type="text" ID="ArtisOraniTxt" CssClass="form-control input-decimal" runat="server" ReadOnly="true"  />
                                   <asp:Label ID="ArtisOraniLbl" runat="server" class="col-form-label text-danger"></asp:Label>
                               </div>
                               <div class="form-group col" id="YeniKiraBedeliDiv" runat="server" style="display:none">
                                   <asp:Label ID="Label2" runat="server" class="col-form-label">Yeni Kira Bedeli </asp:Label>
                                   <asp:TextBox type="text" ID="YeniKiraBedeliTxt" CssClass="form-control input-money" runat="server" ReadOnly="true" />
                               </div>                                
                               
                            </div>
                        </div>
                        <div class="card-footer">
                            <asp:LinkButton ID="OnaylaBtn" Text="Onayla" runat="server" class="btn btn-outline-danger" OnClick="OnaylaBtn_Click"></asp:LinkButton>
                            <asp:LinkButton ID="OdemePlaniBtn" Text="Ödeme Planına Git" runat="server" class="btn btn-outline-primary" OnClick="OdemePlaniBtn_Click" Visible="false"></asp:LinkButton>
                            <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">İptal</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

