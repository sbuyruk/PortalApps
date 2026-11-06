<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazSatisListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazSatisListesiWP.TasinmazSatisListesiWP" %>
<div class="col-xl small tasinmaz-satis-listesi">
    <div class="card shadow">
        <div class="card-header"><h3 class="mb-2 text-danger fw-bold">Taşınmaz Satış Listesi</h3></div>
        <div class="card-body">
            <div class="row mb-3">
                <label class="col-md-2">İl
                    <select class="form-control" data-filter="il"><option value="">Tümü</option></select>
                </label>
                <label class="col-md-2">İlçe
                    <select class="form-control" data-filter="ilce"><option value="">Tümü</option></select>
                </label>
                <label class="col-md-2">Ada<input type="text" class="form-control" data-filter="ada" /></label>
                <label class="col-md-2">Parsel<input type="text" class="form-control" data-filter="parsel" /></label>
                <label class="col-md-2">Satış Tarihi (Başlangıç)<input type="date" class="form-control" data-filter="baslangic" /></label>
                <label class="col-md-2">Satış Tarihi (Bitiş)<input type="date" class="form-control" data-filter="bitis" /></label>
            </div>
            <button type="button" class="btn btn-outline-secondary mb-3" data-action="temizle">Filtreleri Temizle</button>
            <p class="text-danger" data-role="hata" role="alert" style="display:none"></p>
            <table class="table table-hover row-border" data-role="satis-tablosu" style="width:100%">
                <thead><tr>
                    <th>Adres</th><th>İli</th><th>İlçesi</th><th>Ada</th><th>Parsel</th>
                    <th>Yüzölçümü</th><th>Vakıf Hissesi</th><th>Satış Bedeli</th><th>Satış Tarihi</th><th>Açıklama</th>
                </tr></thead>
                <tbody></tbody>
            </table>
        </div>
    </div>
</div>
