<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevOnayListesiKisiselWP.ascx.cs" Inherits="IKYS_WebParts.GorevOnayListesiKisiselWP.GorevOnayListesiKisiselWP" %>

<div class="container small">
    <div class="card shadow">
        <div class="card-header">
            <h3 class="mb-1 text-primary">Kişisel Görev Onay Listesi</h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <table id="<%= System.Web.HttpUtility.HtmlAttributeEncode(TabloId) %>" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Adı Soyadı</th>
                            <th>Görevin Sebebi</th>
                            <th>Gidiş Tarihi</th>
                            <th>Dönüş Tarihi</th>
                            <th>Görevin Yeri</th>
                            <th>Amir Onayı</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    jQuery(function () {
        var element = document.getElementById(<%= System.Web.HttpUtility.JavaScriptStringEncode(TabloId, true) %>);
        if (!element) {
            return;
        }
        if (jQuery.fn.dataTable.isDataTable(element)) {
            jQuery(element).DataTable().destroy();
        }
        jQuery(element).DataTable({
            data: JSON.parse(<%= System.Web.HttpUtility.JavaScriptStringEncode(TabloJson, true) %>),
            columns: [
                { data: 'AdiSoyadi' },
                { data: 'GorevinSebebi' },
                {
                    data: 'BaslangicTarihi',
                    render: function (data, type, row) {
                        return type === 'sort' || type === 'type' ? row.BaslangicTarihiSira : data;
                    }
                },
                {
                    data: 'BitisTarihi',
                    render: function (data, type, row) {
                        return type === 'sort' || type === 'type' ? row.BitisTarihiSira : data;
                    }
                },
                { data: 'GorevinYeri' },
                { data: 'AmirOnayi', visible: <%= Utility.ProjeGlobal.ProjeConstants.AMIRONAYIETKINMI.ToString().ToLowerInvariant() %> },
                { data: 'Duzenle', orderable: false, searchable: false }
            ],
            columnDefs: [
                { type: 'turkish', targets: [0, 1, 4] }
            ],
            order: [[3, 'desc']],
            language: {
                url: '/OrtakBelgeler/Turkish.txt',
                decimal: ',',
                thousands: '.'
            },
            responsive: true
        });
    });
</script>
