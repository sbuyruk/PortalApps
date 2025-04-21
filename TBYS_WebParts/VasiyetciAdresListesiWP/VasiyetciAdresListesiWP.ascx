<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VasiyetciAdresListesiWP.ascx.cs" Inherits="TBYS_WebParts.VasiyetciAdresListesiWP.VasiyetciAdresListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }

    .ui-resizable-column {
        white-space: normal;
    }
</style>
<script>
   
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

    $(document).ready(function () {
        $(function () {
            if ($('.input-integerOnly').toArray().forEach(function (field) {
                new Cleave('.input-integerOnly', {
                numericOnly: true,
                numeral: true,
                numeralDecimalMark: 'none',
                delimiter: ''
            });
            }));
        });
    });
</script>

<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card ">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Vasiyetçi Adres Listesi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body mt-1">
                    <div class="row border border-dark pt-2 bg-light">
                        <div class="col-3 form-group">
                            <label for="IliDDL" class="col-form-label text-white">. . . . . . . . . . . </label>
                            <asp:CheckBox ID="VefatEdenlerHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="VefatEdenlerHaricChk_TextChanged" Text=" Vefat Edenler Hariç" Checked="True" />
                        </div>
                        <div class="col-3 form-group">
                            <label for="IliDDL" class="col-form-label fw-bold">Etiket Adedi: </label>
                            <asp:DropDownList ID="EtiketAdediDDL" runat="server" CssClass="form-control" Height="34px"></asp:DropDownList>
                        </div>

                    </div>
                    <div class="row mt-3" runat="server">
                        <div style="display: none">
                            <input id="paramVasiyetciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        </div>
                        <div class="form-group">
                            <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                                <thead>
                                    <tr>
                                        <th>Bölge</th>
                                        <th>Adı</th>
                                        <th>Soyadı</th>
                                        <th>İkamet Adresi</th>
                                        <th>İkamet İli</th>
                                        <th>İkamet İlçesi</th>
                                        <th>Telefon</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Teşekkür Belgeleri</asp:HyperLink>
                        <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiketleri</asp:HyperLink>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success" ID="AdresEtiketiBtn" runat="server" Text="Adres Etiketi Oluştur" OnClick="AdresEtiketiBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
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