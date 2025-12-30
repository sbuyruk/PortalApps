<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VasiyetciListesiWP.ascx.cs" Inherits="TBYS_WebParts.VasiyetciListesiWP.VasiyetciListesiWP" %>

<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    table tr td th {
        font-size: small;
    }
    thead {
        display: table-header-group;
    }
    tfoot {
        display: none;
    }
</style>

<div class="col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Vasiyetçi Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
               
                <table id="CustomDataTable" class="table table-striped row-border table-recursive" width="100%">
                    <thead>
                    </thead>
                    <tfoot> 
                        <tr >
                            <th>V.No</th>
                            <th>Adı</th>
                            <th>Soyadı</th>
                            <th>TC Kimlik No</th>
                            <th>İkamet İli</th>
                            <th>İkamet İlçesi</th>                            
                            <th>İkamet Adres</th>
                            <th>Telefon</th>
                            <th>"Vasiyet Yılı"</th>
                            <th>Bölge</th>
                            <th>Vasiyet</th>
                            <th>Düzenle</th>
                            <th>Sağ/Vefat</th>
                        </tr>
                    </tfoot>
                    <tbody></tbody>
                    
                </table>
            </div>
        </div>
        <div class="card-footer">
            <%--<asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Vasiyetçi Girişi" OnClick="YeniKayitBtn_Click"></asp:LinkButton>--%>
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>

    </div>
</div>
