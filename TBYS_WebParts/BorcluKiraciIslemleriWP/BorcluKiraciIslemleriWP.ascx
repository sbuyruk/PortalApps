<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BorcluKiraciIslemleriWP.ascx.cs" Inherits="TBYS_WebParts.BorcluKiraciIslemleriWP.BorcluKiraciIslemleriWP" %>

<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }

    .ui-timepicker-container {
        z-index: 9999 !important;
    }

    .warning-item {
        background-color: red !important;
        color: white !important;
    }
</style>
<script>
    $(function () {
        $("#IslemTarihiTxt").datepicker({ minDate: -7, maxDate: "+1M" });
    });

   
</script>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(kiraSozlesmeId) {
        document.getElementById('<%= paramKiraSozlesmeIdLbl.ClientID%>').value = kiraSozlesmeId;
        document.getElementById('<%= OdemePlaniGoruntuleBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OdemePlaniModal'));
        myModalInstance.show();
    }
    function OpenTakipIslemiModal(kiraciId, kiraSozlesmeId, odemePlaniId, kiraBedeli, toplamBorcu, kiraBorcuAySayisi, bolge, takipIslemi) {
        document.getElementById('<%= paramBolge.ClientID%>').value = bolge.replace("#", " ");
        document.getElementById('<%= paramTakipIslemi.ClientID%>').value = takipIslemi.replace("#", " ");
        document.getElementById('<%= paramKiraciIdLbl.ClientID%>').value = kiraciId;
        document.getElementById('<%= paramKiraSozlesmeIdLbl.ClientID%>').value = kiraSozlesmeId;
        document.getElementById('<%= paramOdemePlaniIdLbl.ClientID%>').value = odemePlaniId;
        document.getElementById('<%= paramKiraciBedeliLbl.ClientID%>').value = kiraBedeli;
        document.getElementById('<%= paramToplamBorcuLbl.ClientID%>').value = toplamBorcu;
        document.getElementById('<%= paramKiraBorcuAySayisiLbl.ClientID%>').value = kiraBorcuAySayisi;
        document.getElementById('<%= TakipIslemiModalAcBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('KiraBocuTakibiModalDiv'));
        myModalInstance.show();
    }
    function OpenTakipIslemiModalDuzenle(kiraBorcuTakipId) {
        document.getElementById('<%= paramKiraBorcuTakipIdLbl.ClientID%>').value = kiraBorcuTakipId;
        document.getElementById('<%= TakipIslemiModalDuzenleBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('KiraBocuTakibiModalDiv'));
        myModalInstance.show();
    }
    function CloseKiraBocuTakibiModal() {
        $("#KiraBocuTakibiModalDiv").modal('hide');

    }
</script>
<div id="MainContainer" class="container col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Borçlu Kiracı İşlemleri"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div style="display: none">
                <input id="paramBolge" runat="server" text="Label" style="border-style: none;" />
                <input id="paramTakipIslemi" runat="server" text="Label" style="border-style: none;" />
                <input id="paramKiraciIdLbl" runat="server" text="Label" style="border-style: none;" />
                <input id="paramKiraSozlesmeIdLbl" runat="server" text="Label" style="border-style: none;" />
                <input id="paramOdemePlaniIdLbl" runat="server" text="Label" style="border-style: none;" />
                <input id="paramKiraciBedeliLbl" runat="server" text="Label" style="border-style: none;" />
                <input id="paramToplamBorcuLbl" runat="server" text="Label" style="border-style: none;" />
                <input id="paramKiraBorcuAySayisiLbl" runat="server" text="Label" style="border-style: none;" />
                <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" OnClientClick="{return true;};" OnClick="OdemePlaniGoruntuleBtn_Click"></asp:LinkButton>
                <asp:LinkButton ID="TakipIslemiModalAcBtn" runat="server" OnClientClick="{return true;};" OnClick="TakipIslemiModalAcBtn_Click"></asp:LinkButton>
                <input id="paramKiraBorcuTakipIdLbl" runat="server" text="Label" style="border-style: none;" />
                <asp:LinkButton ID="TakipIslemiModalDuzenleBtn" runat="server" OnClientClick="{return true;};" OnClick="TakipIslemiModalDuzenleBtn_Click"></asp:LinkButton>
            </div>
            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                <ContentTemplate>
            <asp:Table ID="BorcluKiracilarTable" runat="server" class="table table-bordered table-hover table-striped">
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">S.No</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Bölge</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Kiracının Adı Soyadı</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">İlk Sözleşme Tarihi</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Kira Bedeli (TL/Ay)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Borç Miktarı (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Faizli Bakiye (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Kira Borcu (Ay)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Takip İşlemi</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Önceki Takip İşlemleri</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
                    </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="TakipIslemiYapNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="TakipIslemiGuncelleNowBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
<div class="modal" id="OdemePlaniModal" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 650px;">
            <div class="modal-body">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div>
                            <h3>Kira Ödeme Planı
								<asp:Label ID="KiraciTitleLbl" runat="server" CssClass="col-form-label fw-bold" Text=""></asp:Label>
                            </h3>
                        </div>

                        <div class="card-body">

                            <asp:Label ID="DevirLbl" CssClass="col-form-label" runat="server"></asp:Label>
                            <asp:Table ID="OdemePlaniTable" runat="server" CssClass="table table-striped table-bordered">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Sıra No </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Yil </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ay </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Kira Bedeli </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ödenen Tutar</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ödeme Tarihi</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="OdemePlaniGoruntuleBtn" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
<div class="modal" id="KiraBocuTakibiModalDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 650px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header">
                            <div class="text-center">
                                <h3>
                                    <asp:Label ID="BaslikLbl" class="col-form-label text-danger" runat="server" Text="Takip İşlemi"></asp:Label>

                                </h3>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="KiraciAdiLbl" class="col-form-label col-12 " runat="server" Text=""></asp:Label>
                                <asp:Label ID="SozlesmeLbl" class="col-form-label col-12" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                        <div class="card-body">
                            <div class="modal-body">
                                <div class="row">
                                    <div class="form-group col-3">
                                        <asp:Label ID="IslemTarihiLbl" CssClass="col-from-label" runat="server" Text="İşlem Tarihi"></asp:Label>
                                        <asp:TextBox ID="IslemTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="form-group col-3">
                                        <asp:Label ID="IslemSaatiLbl" CssClass="col-from-label" runat="server" Text="İşlem Saati"></asp:Label>
                                        <asp:DropDownList ID="IslemSaatiDDL" runat="server" CssClass="form-control" Style="height: auto" />
                                    </div>
                                     <div class="form-group col-3">
                                        <asp:Label ID="TebligTarihiLbl" CssClass="col-from-label" runat="server" Text="Tebliğ Tarihi"></asp:Label>
                                        <asp:TextBox ID="TebligTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="form-group col-3">
                                        <asp:Label ID="TebligEdilenKisiLbl" CssClass="col-from-label" runat="server" Text="Tebliğ Edilen Kişi"></asp:Label>
                                        <asp:TextBox ID="TebligEdilenKisiTxt" CssClass="form-control " placeholder="Tebliğ edilen kişi" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="form-group">

                                    <asp:TextBox ID="TakipIslemTxt" CssClass="form-control" placeholder="Telefon ile arandı"  runat="server" ></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="8" class="form-control" type="text" placeholder="Görüşme ayrıntılarını giriniz" />
                                </div>
                                <div>
                                    <asp:Label ID="MesajLbl" CssClass="col-form-label text-danger" runat="server" Text="Takip İşlemi Kaydedilecek"></asp:Label>
                                </div>

                            </div>
                        </div>
                        <div class="card-footer">
                            <div class="modal-footer">
                                <asp:LinkButton CssClass="btn btn-success" Visible="false" ID="TakipIslemiYapNowBtn" runat="server" CausesValidation="false" Text="Takip İşlemini Kaydet" OnClientClick="{return true;};" OnClick="TakipIslemiYapNowBtn_Click" />
                                <asp:LinkButton CssClass="btn btn-primary" Visible="false" ID="TakipIslemiGuncelleNowBtn" runat="server" CausesValidation="false" Text="Takip İşlemini Güncelle" OnClientClick="{return true;};" OnClick="TakipIslemiGuncelleNowBtn_Click" />
                                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="TakipIslemiModalAcBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="TakipIslemiModalDuzenleBtn" EventName="click" />
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
    </div>
</div>
