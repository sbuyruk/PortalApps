<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OdemeAyristirmaWP.ascx.cs" Inherits="TBYS_WebParts.OdemeAyristirmaWP.OdemeAyristirmaWP" %>
<script>
    function GuncelleModalDoldur(guid) {

        document.getElementById('<%= GuncelleGuidHdn.ClientID%>').value = guid;
        document.getElementById('<%= ModalGuncelleBtn.ClientID%>').click();
    }
    function SatirSil(guid) {

        document.getElementById('<%= SilGuidHdn.ClientID%>').value = guid;
        document.getElementById('<%= SatirSilBtn.ClientID%>').click();
    }
    function OpenOdemeEkleModal() {
        $("#ListeyeEkleModalDiv").modal({ backdrop: "static" });
    }
    function CloseModal() {
        $("#ListeyeEkleModalDiv").modal('hide');

    }

    if ($('.input-money').toArray().forEach(function (field) {
        new Cleave(field, {
            numeral: true,
            numeralDecimalMark: ',',
            delimiter: '.'
        });
    }));
    function ListeyeEkleNowBtnEnable(tutar) {
        var kaydetVeyaGuncelle = document.getElementById('<%= KaydetVeyaGuncelleHdn.ClientID%>').value;
        tutar = tutar.replace(",",".");
        if (tutar > 0) {
            if (kaydetVeyaGuncelle == "Kaydet") {
                document.getElementById('ListeyeEkleNowDiv').style.display = "block";
                document.getElementById('ListeyiGuncelleNowDiv').style.display = "none";
            } else if (kaydetVeyaGuncelle == "Guncelle") {
                document.getElementById('ListeyeEkleNowDiv').style.display = "none";
                document.getElementById('ListeyiGuncelleNowDiv').style.display = "block";
            }
                
        }
        else {

            document.getElementById('ListeyeEkleNowDiv').style.display = "none";
            document.getElementById('ListeyiGuncelleNowDiv').style.display = "none";
        }
        
    }
</script>
<script type="text/javascript">
    function DoIt() {
        var element = document.getElementById('CardDiv');

        var opt = {
            margin: [0, 0],
            filename: 'odeme.pdf',
            enableLinks: false,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'A4', orientation: 'portrait' }
        };

        // New Promise-based usage:
        html2pdf().set(opt).from(element).save();

        // Old monolithic-style usage:
        //html2pdf(element, opt);
    }


</script>

<script src="/Style Library/tskgv/js/jspdf.js"></script>
<script src="/Style Library/tskgv/js/jspdf.plugin.addimage.js"></script>
<script src="/Style Library/tskgv/js/html2canvas.min.js"></script>
<script src="/Style Library/tskgv/js/html2pdf.bundle.min.js"></script>
<div class="container">

    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Ödeme Ayrıştırma"></asp:Label>
                <asp:Label ID="AktarildiMiLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text=""></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <a href="#" class="btn btn-info" id="downloadPDF" onclick="DoIt();">PDF'e Aktar</a>
            </div>
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div id="CardDiv" class="card-body">
                            <asp:Label CssClass="col-form-label font-weight-bold" ID="AdiLbl" runat="server"></asp:Label>
                            <div class="form-group row">
                                <div class="form-group col">
                                    <label class="col-form-label" for="OdemeTarihiLbl">Ödeme Tarihi</label>
                                    <asp:Label ID="OdemeTarihiLbl" class="form-control alert-secondary" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="OdemeTutariLbl">Ödenen Tutar</label>
                                    <asp:Label ID="OdenenTutarLbl" class="form-control alert-secondary text-right" runat="server" Text=""></asp:Label>
                                </div>                                
                                <div class="form-group col">
                                    <label class="col-form-label" for="KiraTutariLbl">Kira Tutarı</label>
                                    <asp:Label ID="KiraTutariLbl" class="form-control alert-secondary text-right" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="KesinTeminatLbl">Kesin Teminat</label>
                                    <asp:Label ID="KesinTeminatLbl" class="form-control alert-secondary text-right" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="GeciciTeminatLbl">Geçici Teminat</label>
                                    <asp:Label ID="GeciciTeminatLbl" class="form-control alert-secondary text-right" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="KalanTutarLbl">Kalan Tutar</label>
                                     <asp:Label ID="KalanTutarLbl" class="form-control alert-secondary text-right" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="form-group row">

                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="OdemeAciklamaTxt">Ödeme Açıklaması</label>
                                <asp:TextBox ID="OdemeAciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="3" ToolTip="Ödemeye ait açıklama" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="table form-group">
                                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                                    <thead>
                                        <tr>
                                            <th>Ödeme Tarihi</th>
                                            <th>Ödeme Sebebi</th>
                                            <th>Tutar</th>
                                            <th>Kiraci</th>
                                            <th>Düzenle</th>
                                            <th>Sil</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="form-group pt-4 ">
                        <asp:LinkButton ID="KiraOdemesiEkleBtn" runat="server" CssClass="col-2 btn btn-primary mr-3" CausesValidation="false" Text="Kira Ödemesi Ekle" OnClientClick="{return true;};" OnClick="KiraOdemesiEkleBtn_Click" />
                        <asp:LinkButton ID="OdemeyiSozlesmelereBolBtn" runat="server" CssClass="col-3 btn btn-danger mr-3" CausesValidation="false" Text="Ödemeyi Sözleşmelere Böl" OnClientClick="{return true;};" OnClick="OdemeyiSozlesmelereBolBtn_Click" />
                        <asp:LinkButton ID="KesinTeminatEkleBtn" runat="server" CssClass="col-3 btn btn-success mr-3" CausesValidation="false" Text="Kesin Teminat Ödemesi Ekle" OnClientClick="{return true;};" OnClick="KesinTeminatEkleBtn_Click" />
                        <asp:LinkButton ID="GeciciTeminatEkleBtn" runat="server" CssClass="col-3 btn btn-info mr-3" CausesValidation="false" Text="Geçici Teminat Ödemesi Ekle" OnClientClick="{return true;};" OnClick="GeciciTeminatEkleBtn_Click" />
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="KaydetBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="SatirSilBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="ListeyeEkleNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="ListeyiGuncelleNowBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
           

        </div>
        <div class="card-footer">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:LinkButton ID="KaydetBtn" runat="server" CssClass="btn btn-success" Text="Ödeme Ayrıştır ve Kaydet" OnClick="KaydetBtn_Click" Visible="False"></asp:LinkButton>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ListeyeEkleNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="ListeyiGuncelleNowBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="SatirSilBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:LinkButton ID="KiraEkstreAktarmaBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Kira Ekstresine Dön" OnClick="KiraEkstreAktarmaBtn_Click"></asp:LinkButton>
        </div>
    </div>
</div>

<div class="modal" id="ListeyeEkleModalDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card" runat="server" id="Div1">
                            <div class="card-header">
                                <asp:Label ID="ModalTitleLbl" class="col-form-label font-weight-bold" runat="server" Text="Teminat İşlemi Eklenecek"></asp:Label>
                            </div>
                            <div class="card-body">
                                <div id="HiddenDiv" style="display: none">
                                    <asp:Label ID="KiraciIdLbl" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="OdemeSaatiLbl" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="DovizCinsiLbl" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="OdemeSebebiIdLbl" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="KiraEkstreAktarmaIdLbl" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="OdemeIdLbl" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="form-group row">
                                    <div class="form-group col-5">
                                        <label class="col-form-label font-weight-bold" for="OdemeSebebiTxt">Ödeme Sebebi</label>
                                        <asp:Label ID="OdemeSebebiLbl" CssClass="form-control alert-secondary" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="form-group col">
                                        <label class="col-form-label font-weight-bold" for="IslemTutariTxt">Tutar</label>
                                        <input class="form-control input-money text-right " id="IslemTutariTxt" runat="server" onkeyup="ListeyeEkleNowBtnEnable(this.value)" />
                                    </div>
                                </div>
                                <div class="form-group" id="KiraciDiv" runat="server" >
                                    <label class="col-form-label" for="KiraciDDL">Kiraci</label>
                                    <asp:DropDownList ID="KiraciDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                    <%--<asp:ListBox ID="KiraciListBox" runat="server" CssClass="form-control" Width="100%" style="overflow-x:auto;" >
                                    </asp:ListBox>--%>
                                </div>
                                <div class="form-group ">
                                    <label class="col-form-label" for="IslemAciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="IslemAciklamaTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-footer">
                                <div class="row">
                                    <div id="ListeyeEkleNowDiv" class="col form-group" style="display: none">
                                        <asp:LinkButton ID="ListeyeEkleNowBtn" Text="Tamam" runat="server" CssClass="btn btn-success" OnClick="ListeyeEkleNowBtn_Click"></asp:LinkButton>
                                    </div>
                                    <div id="ListeyiGuncelleNowDiv" class="col form-group" style="display: none">
                                        <asp:LinkButton ID="ListeyiGuncelleNowBtn" Text="Güncelle" runat="server" CssClass="btn btn-primary" OnClick="ListeyiGuncelleNowBtn_Click"></asp:LinkButton>
                                    </div>
                                    <div class="col form-group">
                                        <button type="button" class="btn btn-outline-secondary float-right" data-dismiss="modal">İptal</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div style="display: none">
                            <input id="KaydetVeyaGuncelleHdn" runat="server" type="text" />
                            <input id="GuncelleGuidHdn" runat="server" type="text" />
                            <input id="SilGuidHdn" runat="server" type="text" />
                            <asp:LinkButton ID="ModalGuncelleBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalGuncelleBtn_Click" />
                            <asp:LinkButton ID="SatirSilBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="SatirSilBtn_Click" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="KiraOdemesiEkleBtn" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="KesinTeminatEkleBtn" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="GeciciTeminatEkleBtn" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="OdemeyiSozlesmelereBolBtn" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>               
            </div>
        </div>
    </div>
</div>

