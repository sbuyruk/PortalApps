<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraBankaEkstreListesiWP.ascx.cs" Inherits="TBYS_WebParts.KiraBankaEkstreListesiWP.KiraBankaEkstreListesiWP" %>

<style>
    .uyari {
        color:red;
    }
    .ekstre-aktarildi {
        color: grey;
    }

    .ekstre-aktarilmadi {
        color: black;
    }

    .ekstre-aktarilabilir {
        color: green;
    }

    .cakisma-var {
        color: red;
        font-weight: bold;
    }
     /*tblfilter hücre içine sığmazsa wordwrap yapsın*/ 
    .ui-datatable tbody td {
        white-space: normal;
    }
    .ui-column-title {
        white-space: normal;
    }
    .small-font{
        font-size:small;
    }
    .sil-checkbox {
        background-color: red!important;
    }
</style>
<%-- Silinecek kayıtlar --%>
<script type="text/javascript">
    
    var silinecekData = [];
    function addRemoveEkstreIdToDeleteList(ekstreAktarmaId, chkbox) {
        var isChecked = false;
        if (chkbox.checked)
            isChecked = true;
        SilineceklerListesineEkleCikar(ekstreAktarmaId, isChecked);
    }
    function SilineceklerListesineEkleCikar(ekstreAktarmaId, isChecked) {
        var index = silinecekData.indexOf(ekstreAktarmaId.toString());
        if (isChecked && (index < 0)) {
            silinecekData.push(ekstreAktarmaId.toString());
        } else if (!isChecked && (index > -1)) {
            silinecekData.splice(index, 1);
        }
        if (silinecekData.length > 0) {
            document.getElementById('SilinecekBtnDiv').style.display = "block";
        }
        else {
            document.getElementById('SilinecekBtnDiv').style.display = "none";
        }
    }
    function SecilenleriSilTriggerBtnClicked() {
        document.getElementById('<%= paramSilinecekArray.ClientID%>').value = silinecekData;
        document.getElementById('<%= SecilenleriSilBtn.ClientID%>').click();
    }
</script>
<%--Kaydet modal aç vs--%>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModalOnay() {
        $("#ModalOnayDiv").modal({ backdrop: true });
    }

    function CloseModalOnay() {
        $("#ModalOnayDiv").modal('hide');
    }
    function OdemePlaniModalAc(kiraEkstreAktarmaId, kiraciId, odemeTarihi) {
        document.getElementById('<%= ParamKiraciIdLbl.ClientID%>').value = kiraEkstreAktarmaId;
        document.getElementById('<%= ParamKiraciIdLbl.ClientID%>').value = kiraciId;
        document.getElementById('<%= ParamOdemeTarihiLbl.ClientID%>').value = odemeTarihi;
        document.getElementById('<%= OdemePlaniModalAcBtn.ClientID%>').click();
    }
    var tableData = [];
    function EkleCikar(ekstreAktarmaId, isChecked) {
        var index = tableData.indexOf(ekstreAktarmaId.toString());
        if (isChecked && (index < 0)) {
            tableData.push(ekstreAktarmaId.toString());
        } else if (!isChecked && (index > -1)) {
            tableData.splice(index, 1);
        }
        if (tableData.length > 0) {
            document.getElementById('BtnDiv').style.display = "block";
        }
        else {
            document.getElementById('BtnDiv').style.display = "none";
        }
    }
    function addRemoveEkstreIdToList(ekstreAktarmaId, chkbox) {
        var isChecked = false;
        if (chkbox.checked)
            isChecked = true;
        EkleCikar(ekstreAktarmaId, isChecked);
    }
    function SecilenleriKaydetTriggerBtnClicked() {
        document.getElementById('<%= ParamKaydedilecekArray.ClientID%>').value = tableData;
        document.getElementById('<%= SecilenleriKaydetBtn.ClientID%>').click();
    }
</script>
<%--CustomDataTable--%>
<script>
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{
        "SecKaydet": "", "AdiSoyadi": "", "OdemeTarihi": "", "Tutar": "", "KiraciAdi": "", "Aciklama": "", "Eslestir": "", "SecSil": ""
    }];
    jQuery(document).ready(function () {
        jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "SecKaydet", "width": "6%" },
                { data: "AdiSoyadi", "width": "15%" },
                { data: "OdemeTarihi", "width": "10%" },
                { data: "Tutar", "width": "10%", "className": "text-right" },
                { data: "KiraciAdi", "width": "20%", "font-size":"small" },
                { data: "Aciklama", "width": "22%","font-size":"small" },
                { data: "Eslestir", "width": "10%" },
                { data: "SecSil", "width": "7%" }

            ],
            'order': [[2, 'desc']],//sort date desc
            "language": {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                "decimal": ",",
                "thousands": "."
            },
            //column resizable
            //initComplete: function (settings) {
            //    $('#CustomDataTable').colResizable({ liveDrag: true });
            //},
            responsive: true,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'print',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'excel',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'pdf',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'copy',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                , 'pageLength', "colvis"
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.AktarildiMi == "True") {
                    $(row).addClass('ekstre-aktarildi');
                } else if (data.Uyari == "True") {
                    $(row).addClass('uyari');
                } else if (data.KiraciId > 0) {
                    $(row).addClass('ekstre-aktarilabilir');
                }
                
            },//set row color 
        });

        <%--jQuery('#' + '<%: VisibleChk.ClientID %>').addClass("custom-control-input");--%>
        ArrayDoldur();
    });
</script>
<%-- Odeme Ayrıştırma --%>
<script type="text/javascript"> 

    function OpenModalOdemeAyristir(ekstreAktarmaId) {
        document.getElementById('<%= paramEkstreAktarmaIdTxt.ClientID%>').value = ekstreAktarmaId;
        document.getElementById('<%= OdemeAyristirModalAcBtn.ClientID%>').click();
    }
    function OdemeAyristirModalAc() {
            $("#OdemeAyristirModal").modal({ backdrop: "static" });
        }
    function ArrayDoldur() {
        document.getElementById('<%= ToplamLbl.ClientID%>').value = "#";
        document.getElementById('<%= paramKiraciIdArrayHiddenTxt.ClientID%>').value = "";
        document.getElementById('<%= paramTutarArrayHiddenTxt.ClientID%>').value = "";
        var table = <%= OdemeAyristirmaTable.ClientID%>;
        var toplam = 0;
        for (var r = 1, n = table.rows.length; r < n; r++) {

            var kiraciIdValue = table.rows[r].cells[0].innerHTML;
            var tutarValue = table.rows[r].cells[4].childNodes[0].value;
            toplam += parseFloat(tutarValue.replace(".","").replace(",","."));
            document.getElementById('<%= paramKiraciIdArrayHiddenTxt.ClientID%>').value += kiraciIdValue + "#";
            document.getElementById('<%= paramTutarArrayHiddenTxt.ClientID%>').value += tutarValue + "#";

        }
        var odenenStr = document.getElementById('<%= OdemeTutariModalTxt.ClientID%>').value;
        var odenen = parseFloat(odenenStr.replace("TL", "").replace(".", "").replace(",", "."));
        var kalan = odenen - toplam;
        document.getElementById('<%= ToplamLbl.ClientID%>').value = kalan;

        OdeBtnEnable(kalan);
    }
    function AyristirilanOdemeleriKaydet() {
        ArrayDoldur();
        document.getElementById('<%= OdemeAyristirBtn.ClientID%>').click();
    }
    function OdeBtnEnable(tutar) {

        if (tutar ==0) {
            $("#OdemeAyristirModalBtn").attr('class', 'btn btn-outline-success');
            $("#OdemeAyristirModalBtn").attr('disabled', false);
            $("#OdemeAyristirModalBtn").show();
        }
        else {

            $("#OdemeAyristirModalBtn").attr('class', 'btn btn-outline-secondary');
            $("#OdemeAyristirModalBtn").attr('disabled', true);
            $("#OdemeAyristirModalBtn").hide();
        }
    }
</script>
<script type="text/javascript">
    function pageLoad(sender, args) {
        new Cleave('.input-4', {
            numeral: true,
            numeralDecimalMark: ',',
            delimiter: '.'
        });
    }      
</script>
<div class="container">
    <div class="card shadow">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
            <ContentTemplate>
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3>
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Ekstre Aktarma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <asp:CheckBox CssClass="col-3 form-control" ID="AktarilanlarHaricChk" AutoPostBack="true" runat="server" Text="Aktarılanları Gösterme " Checked="True" OnCheckedChanged="AktarilanlarHaricChk_CheckedChanged" TextAlign="Left" />
                        <asp:Label CssClass="col-2 col-form-label float-right" ID="RowCountLbl" runat="server" Text="" Font-Bold="True"></asp:Label>
                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Seç Kaydet</th>
                                    <th>Yatıran</th>
                                    <th>Ödeme Tarihi</th>
                                    <th>Tutar</th>
                                    <th>Kiracı</th>
                                    <th>Açıklama</th>
                                    <th>Eşleştir</th>
                                    <th>Seç Sil</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <div id="BtnDiv" style="display: none">
                        <input id="SecilenleriKaydetTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Kaydet" onclick="SecilenleriKaydetTriggerBtnClicked();" />
                    </div>
                    <div id="SilinecekBtnDiv" style="display: none">
                        <input id="SecilenleriSilTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Sil" onclick="SecilenleriSilTriggerBtnClicked();" />
                    </div>

                    <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
                <div class="modal" id="ModalOnayDiv" role="dialog">
                    <div class="modal-dialog">
                        <!-- Modal content-->
                        <div class="modal-content">
                            <div class="modal-body">
                                <asp:UpdatePanel ID="KaydetSilUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="card">
                                            <div class="card-header">
                                                <h3>
                                                    <asp:Label ID="ModalTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h3>
                                            </div>
                                            <div class="card-body">
                                                <div class="form-group">
                                                    <h4>
                                                        <asp:Label ID="ModalSubTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h4>
                                                    <h4>
                                                        <asp:Label ID="UyariMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h4>
                                                    <h4>
                                                        <asp:Label ID="OnayMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="İşlemi onaylıyor musunuz?"></asp:Label></h4>
                                                </div>
                                            </div>
                                            <div class="card-footer">
                                                <asp:LinkButton CssClass="btn btn-success" ID="KaydetNowBtn" runat="server" CausesValidation="false" Text="Seçilenleri Kaydet" OnClientClick="{return true;};" OnClick="KaydetNowBtn_Click" Visible="false" />
                                                <asp:LinkButton CssClass="btn btn-danger" ID="SilNowBtn" runat="server" CausesValidation="false" Text="Seçilenleri Sil" OnClientClick="{return true;};" OnClick="SilNowBtn_Click" Visible="false" />
                                                <button type="button" class="btn btn-default float-right" data-dismiss="modal">Kapat</button>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="SecilenleriKaydetBtn" EventName="click" />
                                        <asp:AsyncPostBackTrigger ControlID="SecilenleriSilBtn" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
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
     <%--Modal: Bir defada iki sözleşmenin kirası yatırılmışsa bu modal içinde ayrıştırılacak--%>
    <div class="modal" id="OdemePlaniModal" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card">
                                <div class="card-header">
                                    <h3>
                                        <asp:Label ID="KiraciTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <div class="card-body">
                                        <asp:Label ID="DevirLbl" class="col-form-label" runat="server" Font-Bold="True"></asp:Label>
                                        <asp:Table ID="OdemePlaniTable" runat="server" class="table table-sm">
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell>Sıra No </asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Yil </asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ay </asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Kira Bedeli </asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ödenen Tutar</asp:TableHeaderCell>
                                                <%--<asp:TableHeaderCell>Ödeme Tarihi</asp:TableHeaderCell>--%>
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                    </div>
                                </div>
                                <div class="card-footer">
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="OdemePlaniModalAcBtn" EventName="click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <div class="modal" id="OdemeAyristirModal" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">
                <div class="modal-header">
                    <h3>
                        <asp:Label ID="BaslikLbl" class="col-form-label " runat="server">Ödeme Ayrıştırma</asp:Label>
                    </h3>
                </div>
                <div class="modal-body">


                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                            <ContentTemplate>

                                    <div class="form-group  col">
                                        <asp:Label ID="Label1" CssClass="col-form-label font-weight-bold " runat="server">Ödeme Tarihi:</asp:Label>
                                        <asp:Label ID="OdemeTarihiLbl" CssClass="col-form-label " runat="server"></asp:Label>
                                    </div>

                                    <div class="form-group col ">
                                        <asp:Label ID="Label2" CssClass="font-weight-bold text-right" runat="server">Ödeme Tutarı: </asp:Label>
                                        <asp:TextBox ID="OdemeTutariModalTxt" CssClass="input-4 font-weight-bold text-danger text-right" runat="server" Enabled="False"></asp:TextBox>
                                        <asp:Label ID="DovizModalTxt" CssClass="font-weight-bold text-right text-danger" runat="server"> </asp:Label>
                                    </div>

                                <asp:Table ID="OdemeAyristirmaTable" runat="server" class="table table-sm table-striped table-bordered">
                                    <asp:TableHeaderRow>
                                        <asp:TableHeaderCell>Kiraci No </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Kiraci </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Sözleşme</asp:TableHeaderCell>
                                        <asp:TableHeaderCell HorizontalAlign="Right">Kira Bedeli</asp:TableHeaderCell>
                                        <asp:TableHeaderCell HorizontalAlign="Right">Ödenen Tutar</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>

                                </asp:Table>
                                <asp:Label ID="TextBox1" CssClass="font-weight-bold"  runat="server" >Kalan :</asp:Label>
                                <asp:TextBox ID="ToplamLbl" CssClass="input-4 text-danger font-weight-bold text-right"  runat="server" Enabled="False" ></asp:TextBox>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="OdemeAyristirBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="OdemeAyristirModalAcBtn" EventName="click" />
                                
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div class="modal-footer">
                    <input id="OdemeAyristirModalBtn" type="button" value="Ödemeleri Ayrıştır ve Kaydet" class="btn btn-success" onclick="AyristirilanOdemeleriKaydet();" disabled="disabled" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                </div>
            </div>
        </div>
    </div>


    <div id="InvisibleDiv" style="display: none">
        <input id="ParamKaydedilecekArray" runat="server" type="text" />
        <input id="paramSilinecekArray" runat="server" type="text" />
        <input id="ParamKiraciIdLbl" runat="server" type="text" />
        <input id="ParamOdemeTarihiLbl" runat="server" type="text" />
        <asp:LinkButton ID="SecilenleriKaydetBtn" runat="server" CssClass="btn btn-success" Text=" Kaydet " OnClick="SecilenleriKaydetBtn_Click" />
        <asp:LinkButton ID="SecilenleriSilBtn" runat="server" CssClass="btn btn-danger" Text=" Sil " OnClick="SecilenleriSilBtn_Click" />
        <asp:LinkButton ID="OdemePlaniModalAcBtn" runat="server" CssClass="btn btn-success" Text="Odeme Plani Görüntüle" OnClick="OdemePlaniModalAcBtn_Click" />

        <asp:TextBox ID="paramEkstreAktarmaIdTxt" runat="server"></asp:TextBox>
        <input id="paramKiraciIdArrayHiddenTxt" runat="server" type="text" />
        <input id="paramTutarArrayHiddenTxt" runat="server" type="text" />
        <asp:LinkButton ID="OdemeAyristirModalAcBtn" runat="server" CssClass="btn btn-secondary" Text="Ödemeleri Ayrıştır ve Kaydet" OnClick="OdemeAyristirModalAcBtn_Click" />
        <asp:LinkButton ID="OdemeAyristirBtn" runat="server" CssClass="btn btn-secondary" Text="Ödemeleri Ayrıştır ve Kaydet" OnClick="OdemeAyristirBtn_Click" />
    </div>
</div>
