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
    .small-font{
        font-size:small;
    }
</style>

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
        "SecKaydet": "", "AdiSoyadi": "", "OdemeTarihi": "", "Tutar": "", "KiraciAdi": "","OdemeSebebi": "", "Aciklama": "", "Eslestir": "", "OdemeAyristir": ""
    }];
    jQuery(document).ready(function () {
        jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
        jQuery('#CustomDataTable').DataTable({
            'initComplete': function (settings, json) {//tablo yüklendiğinde
                var api = this.api();
                var row = api.row(function (idx, data, node) { //secilen kayda gider
                    return data['Secildi'] == true;
                });
                if (row.length > 0) {
                    row.select()
                        .show()
                        .draw(false);
                }
            },
            data: myjsons,
            columns: [
                { data: "SecKaydet" },
                { data: "AdiSoyadi", "width": "15%" },
                { data: "OdemeTarihi", "width": "10%" },
                { data: "Tutar", "width": "10%", "className": "text-right" },
                { data: "KiraciAdi", "width": "20%", "font-size":"small" },
                { data: "OdemeSebebi", "font-size":"small" },
                { data: "Aciklama", "width": "20%","font-size":"small" },
                { data: "Eslestir" },
                { data: "OdemeAyristir" },

            ],
            'order': [[2, 'desc']],//sort date desc
            "language": {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                "decimal": ",",
                "thousands": "."
            },
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

    });
</script>

<div class="container col-xl">
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
                        <asp:Label CssClass="col-2 col-form-label float-right" ID="RowCountLbl" runat="server" Text="" Font-Bold="True"></asp:Label>
                        <div class="checkbox pt-3">
                            <label>
                                <asp:CheckBox ID="AktarilanlarHaricChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="AktarilanlarHaricChk_CheckedChanged" ToolTip="Aktarilanları görmek için işareti kaldırınız." />
                                Aktarılanları Gösterme
                            </label>
                        </div>
                        <div class="checkbox pt-3">
                            <label>
                                <asp:CheckBox ID="KiraTeminatDigerChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="KiraTeminatDigerChk_CheckedChanged" ToolTip="Tüm ödemeleri görmek için işareti kaldırınız." />
                                Yalnızca Kira-Teminat-Diğer Olanları Göster
                            </label>
                        </div>
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
                                    <th>Ödeme Sebebi</th>
                                    <th>Açıklama</th>
                                    <th>Eşleştir</th>
                                    <th>Ödeme Ayrıştır</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <div id="BtnDiv" style="display: none">
                        <input id="SecilenleriKaydetTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Kaydet" onclick="SecilenleriKaydetTriggerBtnClicked();" />
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
                                                <button type="button" class="btn btn-default float-right" data-dismiss="modal">Kapat</button>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="SecilenleriKaydetBtn" EventName="click" />
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

    <div id="InvisibleDiv" style="display: none">
        <input id="ParamKaydedilecekArray" runat="server" type="text" />
        <input id="ParamKiraciIdLbl" runat="server" type="text" />
        <input id="ParamOdemeTarihiLbl" runat="server" type="text" />
        <asp:LinkButton ID="SecilenleriKaydetBtn" runat="server" CssClass="btn btn-success" Text=" Kaydet " OnClick="SecilenleriKaydetBtn_Click" />
        <asp:LinkButton ID="OdemePlaniModalAcBtn" runat="server" CssClass="btn btn-success" Text="Odeme Plani Görüntüle" OnClick="OdemePlaniModalAcBtn_Click" />

        <asp:TextBox ID="paramEkstreAktarmaIdTxt" runat="server"></asp:TextBox>
        <input id="paramKiraciIdArrayHiddenTxt" runat="server" type="text" />
        <input id="paramTutarArrayHiddenTxt" runat="server" type="text" />
    </div>
</div>
