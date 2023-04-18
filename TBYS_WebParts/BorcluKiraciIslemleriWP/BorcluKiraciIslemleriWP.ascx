<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
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

     $(document).ready(function () {
         $('.timepicker').timepicker({
             zindex: 9999 ,
             timeFormat: 'HH:mm',
             minTime: '06:00', 
             maxHour: 23,
             maxMinutes: 30,
             startTime: new Date(0, 0, 0, 10, 0, 0), // 10:00:00 AM - 
             interval: 5 // 5 minutes

         });
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
        $("#OdemePlaniModal").modal({ backdrop: true });
    }
    function OpenModalOnay(kiraciId,kiraSozlesmeId,kiraBorcuAySayisi,takipIslemi) {
        document.getElementById('<%= paramKiraciIdLbl.ClientID%>').value = kiraciId;
        document.getElementById('<%= paramKiraSozlesmeIdLbl.ClientID%>').value = kiraSozlesmeId ;
        document.getElementById('<%= paramTakipIslemi.ClientID%>').value = takipIslemi.replace("#"," ");
        document.getElementById('<%= TakipIslemiYapBtn.ClientID%>').click();
        $("#ModalOnayDiv").modal({ backdrop: true });
    }
</script>
<div id="MainContainer" class="container col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold text-center" ID="TitleLbl" runat="server" Text="Borçlu Kiracı İşlemleri"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div style="display: none">
                <input id="paramKiraSozlesmeIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramKiraciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramTakipIslemi" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" OnClientClick="{return true;};" OnClick="OdemePlaniGoruntuleBtn_Click"></asp:LinkButton>
                <asp:LinkButton ID="TakipIslemiYapBtn" runat="server" OnClientClick="{return true;};" OnClick="TakipIslemiYapBtn_Click"></asp:LinkButton>
            </div>
            <asp:Table ID="BorcluKiracilarTable" runat="server" class="table table-bordered table-hover table-striped">
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">S.NO</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">D.NO</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">BÖLGE</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRACININ ADI VE SOYADI</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">İLK SÖZLEŞME TARİHİ</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRA BEDELİ (TL/AY)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">BORÇ MİKTARI (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">FAİZLİ BAKİYE (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRA BORCU (AY)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">TAKİP İŞLEMİ</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
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
								<asp:Label ID="KiraciTitleLbl" runat="server" CssClass="col-form-label font-weight-bold" Text=""></asp:Label>	
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
                <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 650px;">
            <div class="modal-body">

                <div>
                    <h3>
                        <asp:Label ID="Label1" class="col-form-label text-danger" runat="server" Text="Takip İşlemi"></asp:Label>
                    </h3>
                </div>

                <div class="card-body">

                    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                        <ContentTemplate>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="KiraciAdiLbl" class="col-form-label font-weight-bold text-center" runat="server" Text=""></asp:Label>
                                    </h3>
                                </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TakipIslemiYapBtn" EventName="click" />
                        </Triggers>
                    </asp:UpdatePanel>
                            <div class="row">
                                <div class="form-group col-3">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="İşlem Tarihi"></asp:Label>
                                    <asp:TextBox ID="IslemTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                </div>
                                <div class="form-group col-3">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="İşlem Saati"></asp:Label>
                                    <asp:TextBox ID="IslemSaatiTxt" CssClass="input-time timepicker" placeholder="hh:mm"  runat="server"></asp:TextBox>
                                </div>
                            </div>
                     <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="UyariDiv" class="form-group" style="display: block">
                                UYARI
                                Saat
                                Dakika
                            </div>
                            <div id="YaziliIhtarDiv" class="form-group" style="display: block">
                                YAZILI İHTAR
                                Yazının Dosya Numarası
                            </div>
                            <div id="IcraDavasiDiv" class="form-group" style="display: block">
                                İCRA DAVASI
                                Dava Numarası
                            </div>
                            <div class="form-group">
                                <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" class="form-control" type="text" />
                            </div>
                            <div>
                                <asp:Label ID="MesajLbl" CssClass="col-form-label text-danger" runat="server" Text="Takip İşlemi Yapılacak"></asp:Label>
                                <asp:Label ID="OnayLbl" class="col-form-label text-danger" runat="server" Text="Takip İşlemi"></asp:Label>
                            </div>
                    </ContentTemplate>
                        <triggers>
                            <asp:AsyncPostBackTrigger ControlID="TakipIslemiYapBtn" EventName="click" />
                        </triggers>
                    </asp:UpdatePanel>
                </div>

            </div>
            <div class="modal-footer">
                <asp:LinkButton CssClass="btn btn-success" ID="TakipIslemiYapNowBtn" runat="server" CausesValidation="false" Text="Takip İşlemini Kaydet" OnClientClick="{return true;};" OnClick="TakipIslemiYapNowBtn_Click" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
