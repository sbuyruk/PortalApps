<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaasOlusturmaWP.ascx.cs" Inherits="IKYS_WebParts.MaasOlusturmaWP.MaasOlusturmaWP" %>

<style>
    .header-center {
        text-align: center;
        vertical-align: middle !important;
    }

    .modal {
        z-index: 1050 !important;
    }

    .modal-backdrop {
        z-index: 1040 !important;
    }

    .modal-content {
        z-index: 1060 !important;
    }
</style>

<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }

</script>

<div class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Maaş Oluşturma | "></asp:Label>
                <asp:Label CssClass="form-label fw-light">Seçilen Tarih itibarı ile, Kesintisiz Maaş Listesi</asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TablesDiv" runat="server">
            <div class="form-group mb-3">
                <div class="form-group row">
                    <div class="form-group col-3">
                        <asp:Label CssClass="form-label fw-semibold" runat="server">Maaş Tarihi : </asp:Label>
                        <asp:DropDownList ID="TarihDDL" runat="server" class="form-control form-select form-select-lg fw-bold text-success" OnSelectedIndexChanged="TarihDDL_SelectedIndexChanged" AutoPostBack="true" />
                    </div>

                    <div class="checkbox col-3">
                        <label>
                            <asp:CheckBox ID="IkramiyeChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="IkramiyeChk_CheckedChanged" ToolTip="İkramiye alınan aylarda kutucuğu seçiniz." Enabled="False" />
                            İkramiye Ödensin
                        </label>
                    </div>
                </div>
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                        <thead>
                            <tr>
                                <th>Sıra</th>
                                <th>Adı Soyadı</th>
                                <th>Kadro ve Ünvanı</th>
                                <th>Kademe İlerleme Tarihi</th>
                                <th>Derece/Kademe</th>
                                <th>Ücret</th>
                                <th>İkramiye</th>
                                <th>AGİ Yerine İlave Ödeme</th>
                                <th>Toplam Ücret</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>

        <div class="card-footer">
            <asp:LinkButton ID="MaasOlusturBtn" CssClass="col-2 btn btn-success" runat="server" Text="Maaşı Kaydet" OnClick="MaasOlusturBtn_Click" Visible="true" />
        </div>
    </div>
        <!-- Modal -->
    <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content" style="width: 550px;">
                <div class="modal-body">
                    <div class="text-center">
                        <h3>
                            <asp:Label
                                ID="MessageTitleLbl"
                                CssClass="form-label fw-semibold"
                                runat="server"
                                Text="Lütfen Dikkat: Maaş Artışı Yapılacak" />
                        </h3>
                    </div>
                    <div class="card-body">
                        <asp:Label
                            ID="MessageTextLbl"
                            CssClass="form-label fw-semibold"
                            runat="server"
                            Text="Maaş Artışını Uygulamak İstiyor musunuz?" />
                    </div>
                </div>

                <div class="text-center">
                    <label class="form-label text-danger">
                        Tablolar oluşturulduktan sonra bu işlem geri alınamaz.
                    </label>
                </div>

                <div class="modal-footer text-center">
                    <asp:LinkButton
                        CssClass="btn btn-success"
                        ID="KaydetNowBtn"
                        runat="server"
                        CausesValidation="false"
                        Text="Maaş Oluştur ve Kaydet"
                        OnClientClick="{return true;};"
                        OnClick="KaydetNowBtn_Click" />

                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                </div>
            </div>
        </div>
    </div>
</div>


