<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevOnayListesiWP.ascx.cs" Inherits="IKYS_WebParts.GorevOnayListesiWP.GorevOnayListesiWP" %>
<%-- Görev sçimi işlemleri --%>
<script type="text/javascript">
    var idArray = [];

    function AddRemoveSecimListesi(aniObjesiId, chkbox) {

        var isChecked = chkbox.checked;
        ArrayDoldur();
        var index = idArray.indexOf(aniObjesiId.toString());
        if (isChecked) {
            if (index > -1) {
                idArray.splice(index, 1);
            }
            idArray.push(aniObjesiId.toString());
        } else if (!isChecked && (index > -1)) {
            idArray.splice(index, 1);
        }
        document.getElementById('<%= paramidArray.ClientID%>').value = idArray;
    }
    function SecilenleriKaydetTriggerBtnClicked() {
        document.getElementById('<%= paramidArray.ClientID%>').value = idArray;
        document.getElementById('<%= SecilenleriKaydetBtn.ClientID%>').click();
    }
    function ArrayDoldur() {
        var idString = document.getElementById('<%= paramidArray.ClientID%>').value;
        var idList = idString.split(',');
        idArray = [];
        for (var i = 0; i < idList.length; i++) {
            idArray.push(idList[i]);
        }
    }
</script>

<script type="text/javascript">
    var table = $('#CustomDataTable').DataTable();
    $(document).on('change', '#CustomDataTable input[type="checkbox"]', function () {
        var checkboxes = table.rows({ page: 'current' }).nodes().to$().find('input[type="checkbox"]');
        var allChecked = checkboxes.length > 0 && checkboxes.filter(':checked').length === checkboxes.length;

        $('#checkAll').prop('checked', allChecked);
    });

    

    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{
        "SecChk": "","AdiSoyadi": "", "GorevinSebebi": "","BaslangicTarihi": "", "BitisTarihi": "", "GorevinYeri": "", "RaporAl": "", "Duzenle": ""
    }];

    jQuery(document).ready(function () {
        jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
        jQuery('#CustomDataTable').DataTable({
            'initComplete': function (settings, json) {//tablo yüklendiğinde
                var api = this.api();
                var row = api.row(function (idx, data, node) { //secilen kayıta gider
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
                { data: "SecChk" },
                { data: "AdiSoyadi" },
                { data: "GorevinSebebi" },
                { data: "BaslangicTarihi" },
                { data: "BitisTarihi" },
                { data: "GorevinYeri" },
                { data: "RaporAl" },
                { data: "Duzenle" },

            ],
            columnDefs: [
                { type: 'turkish', targets: [1, 2, 5] },
            ],
            'order': [[3, 'desc']],//sort date desc
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

        });

        ArrayDoldur();
        // Check All butonuna tıklanınca
        table = $('#CustomDataTable').DataTable();
        $('#checkAll').on('click', function () {
            var isChecked = $(this).is(':checked');
            
            // Sadece aktif sayfadaki checkbox'ları seç
            table.rows({ page: 'current' }).nodes().to$().find('input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') !== isChecked) {
                    $(this).click();
                }
            });
        });
        //tabloda sayfalar arası geçişte açılan sayfadaki tüm checkbox'lar checkli ise checkAll'ı checkli yap
        table.on('draw', function () {
            var checkboxes = table.rows({ page: 'current' }).nodes().to$().find('input[type="checkbox"]');

            if (checkboxes.length === 0) {
                $('#checkAll').prop('checked', false);
                return;
            }

            var allChecked = true;
            checkboxes.each(function () {
                if (!$(this).prop('checked')) {
                    allChecked = false;
                    return false; // break loop
                }
            });

            $('#checkAll').prop('checked', allChecked);
        });
    });


 
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="form-label fw-semibold  btn-outline-primary" Id="TitleLbl" runat="server" Text="Görev Onay Listesi"></asp:Label>
                <asp:Label CssClass="form-label fw-semibold text-secondary float-end" ID="EkranNo" Text="30" runat="server" ></asp:Label>
            </h3>

        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>Seç</th>
                                    <th>Adı Soyadı</th>
                                    <th>Görevin Sebebi</th>
                                    <th>Gidiş Tarihi</th>
                                    <th>Dönüş Tarihi</th>
                                    <th>Görevin Yeri</th>
                                    <th>Rapor Al</th>
                                    <th>Düzenle</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SecilenleriKaydetBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="RaporAlBtn" EventName="click" />
                </Triggers>

            </asp:UpdatePanel>
            <div class="form-group m-3">
                <label class="fw-semibold float-end">
                    <input type="checkbox" id="checkAll">
                    Bu Sayfadakileri Seç</label>
            </div>

            <div id="InvisibleDiv" style="display: none">
                <input id="paramidArray" runat="server" type="text" />
                <asp:LinkButton ID="SecilenleriKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="SecilenleriKaydetBtn_Click" />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniGorevOnayiBtn" CssClass="btn btn-outline-success " runat="server" Text="Yeni Görev Onayı" OnClick="YeniGorevOnayiBtn_Click" />
            <asp:LinkButton ID="RaporAlBtn" CssClass="btn btn-outline-success float-end" runat="server" Text="Seçilen Görevler İçin Rapor Al" OnClick="RaporAlBtn_Click" />
        </div>

    </div>
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