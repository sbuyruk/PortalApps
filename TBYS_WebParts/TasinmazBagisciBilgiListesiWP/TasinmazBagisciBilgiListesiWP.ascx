<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisciBilgiListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBagisciBilgiListesiWP.TasinmazBagisciBilgiListesiWP" %>

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
        "Sirano": "", "Bolge": "", "AdiSoyadi": "", "Meslegi": "", "Adres": "", "IlceIl": "", "Telefon": "", "Talepleri": "", "Bagislari": "", "TahminiRayic": ""
    }];
    jQuery(document).ready(function () {

        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                //{ data: "Sirano" },
                { data: "Bolge" },
                { data: "AdiSoyadi" },
                { data: "Meslegi" },
                { data: "SagVefat" },
                { data: "Adres" },
                { data: "IlceIl" },
                { data: "Telefon" },
                { data: "Talepleri" },
                { data: "Bagislari" },
                { data: "TahminiRayic", "className": "text-end" },
            
            ],
            columnDefs: [
                
                { width: 15, targets: 1 }],
            'order': [[1, 'asc']],//sort Bolge
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
            ]
        });
    });
</script>
<div class="col-xl">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Bağışçılara Ait Bilgiler"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body mt-1">
            <div class="row form-group m-0 mb-3 p-2 border">
                <div class="col-3">
                    <asp:DropDownList ID="SagVefatDDL" runat="server" class="form-control" Height="34px" AutoPostBack="true" OnSelectedIndexChanged="SagVefatDDL_SelectedIndexChanged"></asp:DropDownList>
                </div>
            </div>
            <asp:UpdatePanel ID="upPanel" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                <table id="CustomDataTable" class="table small table-striped row-border" width="100%">

                    <thead>
                        <tr>
                            <th colspan="10" Id="BaslikTH" runat ="server" class="text-center fw-bold">Bağışçı Listesi</th>
                        </tr>
                        <tr>
                            <%--<th>Sıra</th>--%>
                            <th>Bölge</th>
                            <th>Adı Soyadı</th>
                            <th>Mesleği</th>
                            <th>Sağ/Vefat</th>
                            <th>Adresi</th>
                            <th>İlçesi/İli</th>
                            <th>Telefon1</th>
                            <th>Talepleri</th>
                            <th>Bağışları</th>
                            <th>Tahmini Rayiç</th>
                        </tr>
                    </thead>
                </table>
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

        <div class="card-footer">
        </div>
    </div>
</div>
