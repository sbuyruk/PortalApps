<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FTKIslemleriWP.ascx.cs" Inherits="NBYS_WebParts.FTKIslemleriWP.FTKIslemleriWP" %>
<style>
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }

    .aktif-degil {
        color: gray !important;
        background-color: lightgray !important;
    }

    .yeni-uye {
        color: blue !important;
    }

    .degisti {
        color: blue !important;
    }
</style>

<script type="text/javascript">
    function ModalFTKListesiAc() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalFTKListesiDiv'));
        myModalInstance.show();
    }
    function ModalFTKListesiKapat() {
        $("#ModalFTKListesiDiv").modal('hide');

    }
    function FTKKisiDuzenleBtnClick(ftkkisiId) {
        document.getElementById('<%= paramFTKIslemleriUyeIdLbl.ClientID%>').value = ftkkisiId;
        document.getElementById('<%= FTKKisiDuzenleBtn.ClientID%>').click();
    }
</script>

<div class="col-xl ">
    <div class="card shadow">
        <div class="card-header">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">

                        <a class=" btn btn-outline-primary float-end mr-4" runat="server" id="YonergeLnk"
                            data-fancybox
                            data-type="pdf"
                            data-width="960"
                            data-height="720"
                            href="">
                            <i class="fa fa-book" aria-hidden="true"></i>
                        </a>
                        <asp:Label CssClass="col-form-label text-warning font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="FTK İşlemleri"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="IlcesiDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="IliDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="KaydetNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="TumununGoreviniSonlandirNowBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-body alert-secondary">
            <div class="row">
                <div class="col-4">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                            <div class="row">
                                <div class="form-group col ">
                                    <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İli</asp:Label>
                                    <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="IliDDL" ForeColor="Red" ErrorMessage="İl Seçiniz..."> </asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İlçesi</asp:Label>
                                    <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control" AutoPostBack="true" Style="height: auto" OnSelectedIndexChanged="IlcesiDDL_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Sor.Bölge" Enabled="False"></asp:Label>
                                    <asp:TextBox ID="SorumluBolgeTxt" CssClass="form-control" runat="server" Style="height: auto" Text="" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Sor.Bölge Id" Enabled="False"></asp:Label>
                                    <asp:TextBox ID="BolgeIdTxt" CssClass="form-control" runat="server" Style="height: auto" Text="" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="FTK Kuruluş Tarihi"></asp:Label>
                                    <asp:TextBox ID="FTKKurulusTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="FTK Güncelleme Tarihi"></asp:Label>
                                    <asp:TextBox ID="FTKGuncellemeTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col-6">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Vali"></asp:Label>
                                    <asp:TextBox ID="ValiTxt" CssClass="form-control" runat="server" Text="" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="form-group col-6">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Kaymakam"></asp:Label>
                                    <asp:TextBox ID="KaymakamTxt" CssClass="form-control" runat="server" Text="" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" Text="Açıklama"></asp:Label>
                                <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="6"></asp:TextBox>
                            </div>
                            <div class="checkbox pt-3" id="KayitDuzeltmeDiv" runat="server" style="display: none">
                                <label class="col-form-label font-weight-bold">
                                    <asp:CheckBox ID="KayitDuzeltmesiChk" runat="server" Checked="false" />
                                    Bu bir Kayıt düzeltmesidir.
                                </label>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="KaydetNowBtn" EventName="click" />
                            <asp:AsyncPostBackTrigger ControlID="TumununGoreviniSonlandirNowBtn" EventName="click" />
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
                <div class="col-8">
                    <div class="card">
                        <div class="card-header">
                            <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="Fahri Tanıtım Kurulu Listesi"></asp:Label>
                            <div id="AktifOlmayanlariGostermeDiv" class="checkbox pt-3 float-end" runat="server">
                                <label>
                                    <asp:CheckBox ID="AktifOlmayanlariGostermeChk" runat="server" Checked="True" ToolTip="Görevi Bitenleri Gösterme" OnCheckedChanged="AktifOlmayanlariGostermeChk_CheckedChanged" AutoPostBack="true" />
                                    Görevi Bitenleri Gösterme
                                </label>
                            </div>
                        </div>
                        <div class="card-body" style="min-height: 374px;">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                                <ContentTemplate>
                                    <div class="form-group">
                                        <table id="CustomDataTable" class="table table-sm table-striped table-bordered" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>GorevId</th>
                                                    <th>Kayıt No</th>
                                                    <th>Adı Soyadı</th>
                                                    <th>Görevi</th>
                                                    <th>Unvanı</th>
                                                    <th>Kart No</th>
                                                    <th>Üyelik Durumu</th>
                                                    <th>Düzenle</th>
                                                </tr>
                                            </thead>
                                        </table>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="FTKKisiEkleBtn" EventName="click" />
                                    <asp:AsyncPostBackTrigger ControlID="AktifOlmayanlariGostermeChk" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                        <div class="card-footer">
                            <asp:LinkButton ID="FTKKisiEkleBtn" runat="server" CssClass="btn btn-outline-success" Text="FTK Üyesi Ekle" OnClick="FTKKisiEkleBtn_Click" />
                            <asp:LinkButton ID="UyelerinGoreviniSonlandirBtn" runat="server" CssClass="btn btn-outline-danger" Text="Üyelerin Görevini Sonlandır" OnClick="UyelerinGoreviniSonlandirBtn_Click"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <asp:LinkButton ID="KaydetBtn" runat="server" CssClass="btn btn-outline-success" Text="FTK Oluştur" OnClick="KaydetBtn_Click"></asp:LinkButton>
                    
                    <asp:LinkButton ID="FTKListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="FTK Listesi" OnClick="FTKListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="BolgelereGoreFTKRaporuBtn" runat="server" CssClass="btn btn-outline-secondary float-end mr-2" Text="Bölgelere göre FTK Dağılımı" OnClick="BolgelereGoreFTKRaporuBtn_Click" CausesValidation="False"></asp:LinkButton>
                    <asp:LinkButton ID="FTKYazilariBtn" runat="server" CssClass="btn btn-outline-secondary float-end mr-2" Text="FTK Yazisi" OnClick="FTKYazilariBtnBtn_Click" CausesValidation="False" Visible="False"></asp:LinkButton>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="IlcesiDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="IliDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="KaydetNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="TumununGoreviniSonlandirNowBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<div id="KatilimciHiddenDiv" style="display: none">
    <input id="paramFTKIslemleriUyeIdLbl" runat="server" type="text" />
    <asp:LinkButton ID="FTKKisiDuzenleBtn" runat="server" CssClass="btn btn-outline-danger" CausesValidation="false" Text="FTK Üyelikten Çıkar" OnClick="FTKKisiDuzenleBtn_Click" />
</div>
<asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="ModalFTKListesiDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="ModalFTKListesiBaslikLbl" runat="server" Text="FTK Listesi"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div class="row" id="ModalTarihDiv" runat="server" style="display: block">
                            <div class="form-group col">
                                <asp:Label CssClass="col-from-label font-weight-bold" runat="server" Text="FTK Kuruluş Tarihi"></asp:Label>
                                <asp:Label ID="ModalKurulusTarihiLbl" CssClass="form-control" runat="server"></asp:Label>
                            </div>
                            <div class="form-group col">
                                <asp:Label CssClass="col-from-label font-weight-bold" runat="server" Text="FTK Guncelleme Tarihi"></asp:Label>
                                <asp:Label ID="ModalGuncellemeTarihiLbl" CssClass="form-control" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group">
                            <table id="CustomModalDataTable" class="table table-striped table-bordered" width="100%">
                                <thead>
                                    <tr>
                                        <th>Kayıt No</th>
                                        <th>Adı Soyadı</th>
                                        <th>FTK Görevi</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="KaydetNowBtn" CssClass="btn btn-outline-success" runat="server" Text="FTK Oluştur" OnClick="KaydetNowBtn_Click"></asp:LinkButton>
                        <asp:LinkButton ID="TumununGoreviniSonlandirNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Üyelerin Görevini Sonlandır" OnClick="TumununGoreviniSonlandirNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="KaydetBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="UyelerinGoreviniSonlandirBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>


