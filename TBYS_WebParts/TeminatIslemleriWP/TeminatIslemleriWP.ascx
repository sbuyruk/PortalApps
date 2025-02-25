<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeminatIslemleriWP.ascx.cs" Inherits="TBYS_WebParts.TeminatIslemleriWP.TeminatIslemleriWP" %>

<script>
    function DeleteModalDoldur(teminatId) {

        document.getElementById('<%= TeminatIslemIdSilLbl.ClientID%>').value = teminatId;
        document.getElementById('<%= ModalSilBtn.ClientID%>').click();
    }
    function OpenDeleteModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('DeleteModalOnayDiv'));
        myModalInstance.show();
    }
    function GuncelleModalDoldur(teminatId) {

        document.getElementById('<%= TeminatIslemIdGuncelleHdn.ClientID%>').value = teminatId;
        document.getElementById('<%= ModalGuncelleBtn.ClientID%>').click();
    }
    function OpenTeminatIslemiModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('TeminatIslemiEkleModal'));
        myModalInstance.show();
    }

    function CloseModal() {
        var myModalEl = document.getElementById('TeminatIslemiEkleModal');
        var modalInstance = bootstrap.Modal.getInstance(myModalEl);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
    if ($('.input-money').toArray().forEach(function (field) {
        new Cleave(field, {
            numeral: true,
            numeralDecimalMark: ',',
            delimiter: '.'
        });
    }));
</script>
<script type="text/javascript">
    function DoIt() {
        var element = document.getElementById('CardDiv');

        var opt = {
            margin: [0.5, 0.5],
            filename: 'teminat.pdf',
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
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Teminat İşlemleri"></asp:Label>
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
                            <asp:Label CssClass="col-form-label fw-bold" ID="AdiLbl" runat="server"></asp:Label>
                            <div class="form-group">
                                    <asp:Table ID="TasinmazAdresTable" runat="server" CssClass="table table-bordered table-striped"></asp:Table>
                                </div>
                            <hr />
                            <asp:Label CssClass="col-form-label" ID="SozlesmeLbl" runat="server"></asp:Label>
                            <div class="form-group row">
                                <div class="form-group col-3">
                                    <label class="col-form-label" for="TeminatCinsiDDL">Teminat Cinsi</label>
                                    <asp:DropDownList ID="TeminatCinsiDDL" runat="server" CssClass="form-control small" style="height:auto"></asp:DropDownList>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="TeminatTarihiTxt">Teminat Tarihi</label>
                                    <input type="text" id="TeminatTarihiTxt" name="TeminatTarihiTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly" />
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="TeminatTutariTxt">Belirlenen Teminat</label>
                                    <input class="form-control input-money text-end " id="TeminatTutariTxt" runat="server" />
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="OdenenTeminatTxt">Alınan Teminat</label>
                                    <input type="text" id="OdenenTeminatTxt" name="OdenenTeminatTxt" class="form-control input-money text-end " runat="server" readonly="readonly" />
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="IadeTeminatTxt">İade/Mahsup Ed.</label>
                                    <input type="text" id="IadeTeminatTxt" class="form-control input-money text-end " runat="server" readonly="readonly" />
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="KalanTeminatTxt">Kalan Teminat</label>
                                    <input type="text" id="KalanTeminatTxt" name="KalanTeminatTxt" class="form-control input-money text-end " runat="server" readonly="readonly" />
                                </div>
                            </div>
                            <div class="form-group row">

                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="TeminatAciklamaTxt">Teminat Açıklaması</label>
                                <asp:TextBox ID="TeminatAciklamaTxt" runat="server" class="form-control " TextMode="MultiLine" Rows="3" ToolTip="Teminata ait açıklama"></asp:TextBox>
                            </div>
                            <div class="table form-group">
                                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                                    <thead>
                                        <tr>
                                            <th>Tarih</th>
                                            <th>İşlem Tipi</th>
                                            <th>Tutar</th>
                                            <th>Açıklama</th>
                                            <th>Düzenle</th>
                                            <th>Sil</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ModalSilBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="TeminatGuncelleBtn" runat="server" CssClass="btn btn-primary" Text="Güncelle" OnClick="TeminatGuncelleBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="ModalEkleBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text="Teminat İşlemi Ekle" OnClientClick="{return true;};" OnClick="ModalEkleBtn_Click" />
            <asp:LinkButton ID="SozlesmeyeGitBtn" runat="server" CssClass="btn btn-outline-secondary " Text="Sözleşme" OnClick="SozlesmeyeGitBtn_Click"></asp:LinkButton>        
            <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Sonraki Söz.=>" OnClick="NextBtn_Click" />
            <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="<=Önceki Söz." OnClick="PrevBtn_Click" />
            <asp:LinkButton ID="KiraciListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Kiracı Listesi" OnClick="KiraciListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="KiraKartiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Kira Karti" OnClick="KiraKartiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="OdemePlaninaGitBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Ödeme Planı" OnClick="OdemePlaninaGitBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="TeminatListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Teminat Listesi" OnClick="TeminatListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="AylikOdemelerBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Aylık Ödemeler" OnClick="AylikOdemelerBtn_Click"></asp:LinkButton>
        </div>
    </div>
</div>

<div class="modal" id="TeminatIslemiEkleModal" role="dialog">
    <div class="modal-dialog  modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body">
                <div class="card" runat="server" id="Div1">
                    <div class="card-header">
                        <asp:Label ID="ModalTitleLbl" class="col-form-label fw-bold" runat="server" Text="Teminat İşlemi Eklenecek"></asp:Label>
                    </div>
                    <div class="card-body">
                        <div class="form-group row">
                            <div class="form-group col">
                                <label class="col-form-label" for="IslemTarihiTxt">İşlem Tarihi</label>
                                <input id="IslemTarihiTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly" />
                            </div>
                            <div class="form-group col">
                                <label class="col-form-label" for="IslemSaatiTxt">Saat</label>
                                <input class="form-control input-time" id="IslemSaatiTxt" runat="server" />
                            </div>
                            <div class="form-group col-5">
                                <label class="col-form-label" for="BelirlenenTeminatTxt">İşlem Tipi</label>
                                <asp:DropDownList ID="IslemTipiDDL" runat="server" CssClass="form-control" ToolTip="İşlem Tipi" Height="34px"></asp:DropDownList>
                                <asp:Label ID="IslemTipiTxt" runat="server" CssClass="form-control col-form-label" Visible="false"></asp:Label>
                            </div>
                            <div class="form-group col">
                                <label class="col-form-label" for="IslemTutariTxt">İşlem Tutarı</label>
                                <input class="form-control input-money text-end " id="IslemTutariTxt" runat="server" />
                            </div>
                        </div>
                        <div class="form-group ">
                            <label class="col-form-label" for="IslemAciklamaTxt">Açıklama</label>
                            <asp:TextBox ID="IslemAciklamaTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="3" ToolTip="İşleme ait notlar"></asp:TextBox>
                        </div>
                    </div>
                    <div class="card-footer">
                        <asp:LinkButton ID="ModalEkleNowBtn" Text="Kaydet" runat="server" CssClass="btn btn-success" OnClick="ModalEkleNowBtn_Click" Visible="false"></asp:LinkButton>
                        <asp:LinkButton ID="ModalGuncelleNowBtn" Text="Güncelle" runat="server" CssClass="btn btn-primary" OnClick="ModalGuncelleNowBtn_Click" Visible="false"></asp:LinkButton>
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">İptal</button>
                    </div>
                </div>
                <div style="display: none">
                    <input id="TeminatIslemIdGuncelleHdn" runat="server" type="text" />
                    <asp:LinkButton ID="ModalGuncelleBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalGuncelleBtn_Click" />
                </div>
            </div>

        </div>
    </div>
</div>
<div class="modal" id="DeleteModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body">
                <div class="card" runat="server" id="DeleteDiv">
                    <div class="card-header">
                        <asp:Label ID="Label3" class="col-form-label fw-bold" runat="server" Text="Teminat İşlemi Silinecek"></asp:Label>
                    </div>
                    <div class="card-body">
                        <div style="display: none">
                            <input id="TeminatIslemIdSilLbl" runat="server" type="text" />
                            <asp:LinkButton ID="ModalSilBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalSilBtn_Click" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                            <asp:Label ID="SilmeOnayiLbl" CssClass="col-form-label text-danger" runat="server" Text="Silme işlemini onaylıyor musunuz?"></asp:Label>
                        </div>
                    </div>
                    <div class="card-footer">
                        <asp:LinkButton CssClass="btn btn-danger" ID="ModalSilNowBtn" runat="server" CausesValidation="false" Text="Teminat İşlemini Sil" OnClientClick="{return true;};" OnClick="ModalSilNowBtn_Click" />
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">İptal</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
