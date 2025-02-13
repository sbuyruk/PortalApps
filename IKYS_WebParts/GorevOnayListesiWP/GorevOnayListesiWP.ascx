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
    var aniObjesiIdArray = [];

    function AddRemoveSecimListesi(aniObjesiId, chkbox) {

        var isChecked = chkbox.checked;
        ArrayDoldur();
        var index = aniObjesiIdArray.indexOf(aniObjesiId.toString());
        if (isChecked) {
            if (index > -1) {
                aniObjesiIdArray.splice(index, 1);
            }
            aniObjesiIdArray.push(aniObjesiId.toString());
        } else if (!isChecked && (index > -1)) {
            aniObjesiIdArray.splice(index, 1);
        }
        document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value = aniObjesiIdArray;
    }
    function SecilenleriKaydetTriggerBtnClicked() {
        document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value = aniObjesiIdArray;
        document.getElementById('<%= SecilenleriKaydetBtn.ClientID%>').click();
    }
    function ArrayDoldur() {
        var idString = document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value;
        var idList = idString.split(',');
        aniObjesiIdArray = [];
        for (var i = 0; i < idList.length; i++) {
            aniObjesiIdArray.push(idList[i]);
        }
    }
</script>

<script type="text/javascript">
  
    

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

        <%--jQuery('#' + '<%: VisibleChk.ClientID %>').addClass("custom-control-input");--%>
        ArrayDoldur();
    });
 
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label  btn-outline-primary" Id="TitleLbl" runat="server" Text="Görev Onay Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="30" runat="server" ></asp:Label>
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
            <div id="InvisibleDiv" style="display: none">
                <input id="paramAniObjesiIdArray" runat="server" type="text" />
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