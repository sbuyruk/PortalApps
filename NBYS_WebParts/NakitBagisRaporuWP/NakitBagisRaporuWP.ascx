<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisRaporuWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisRaporuWP.NakitBagisRaporuWP" %>

<%@ Register TagPrefix="asp" Namespace="System.Web.UI.WebControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<style>
    .ui-datatable tbody td {
        white-space: normal;
    }

    .bolded {
        font-weight: bold;
    }
</style>

<script>

    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        if (jQuery.fn.DataTable.isDataTable('#CustomModalDataTable')) {
            jQuery('#CustomModalDataTable').DataTable().destroy();
        }
        jQuery('#CustomModalDataTable tbody').empty();

        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
    }
</script>

<div class="container small">
    <div class="card shadow">
        <div class="card-header bg-light">
            <h4 class="mb-0 text-primary fw-bold">Nakit Bağış Raporu </h4>
        </div>

                <div class="card-body">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <!-- Bağış Hareket Filtreleri -->
                            <div class="row mb-3">
                                <div class="col-12">
                                    <h6 class="fw-bold text-secondary border-bottom pb-1">Bağış Hareket Filtreleri</h6>
                                </div>

                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Son Bağış</label>
                                    <asp:DropDownList ID="SonBagisDDL" runat="server" CssClass="form-control form-select fw-semibold"></asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Bağış Tarihi </label>
                                    <asp:TextBox ID="BagisTarihiBaslangicTxt" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Bağış Tarihi Bitiş</label>
                                    <asp:TextBox ID="BagisTarihiBitisTxt" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Bağış Miktarı Min</label>
                                    <asp:TextBox ID="BagisMiktariMinTxt" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Bağış Miktarı Max</label>
                                    <asp:TextBox ID="BagisMiktariMaxTxt" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Armağan</label>
                                    <asp:DropDownList ID="ArmaganDDL" runat="server" CssClass="form-control form-select fw-semibold" Enabled="false"></asp:DropDownList>
                                </div>

                            </div>

                            <!-- Bağışçı Filtreleri -->
                            <div class="row mb-3">
                                <div class="col-12">
                                    <h6 class="fw-bold text-secondary border-bottom pb-1">Bağışçı Filtreleri</h6>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">İl</label>
                                    <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control form-select fw-semibold" CausesValidation="false" AutoPostBack="true" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">İlçe</label>
                                    <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control form-select fw-semibold"></asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Sağ / Vefat Durumu</label>
                                    <asp:DropDownList ID="SagDDL" runat="server" CssClass="form-control form-select fw-semibold">
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Belge İstemiyor</label>
                                    <asp:DropDownList ID="BelgeIstemiyorDDL" runat="server" CssClass="form-control form-select fw-semibold">
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Ulaşılamıyor</label>
                                    <asp:DropDownList ID="UlasilamiyorDDL" runat="server" CssClass="form-control form-select fw-semibold">
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group col-lg-2 col-md-3 col-sm-4 mb-2">
                                    <label class="form-label fw-semibold">Tüzel/Özel</label>
                                    <asp:DropDownList ID="TuzelKisiDDL" runat="server" CssClass="form-control form-select fw-semibold">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12">
                                    <asp:LinkButton CssClass="col-2 btn btn-primary me-2" ID="ListeleBtn" runat="server" Text="Listele" OnClick="ListeleBtn_Click" />
                                    <%--<asp:LinkButton CssClass="btn btn-outline-secondary" ID="TemizleBtn" runat="server" Text="Temizle" OnClick="TemizleBtn_Click" />--%>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ListeleBtn" EventName="Click" />
                            <%--<asp:AsyncPostBackTrigger ControlID="TemizleBtn" EventName="Click" />--%>
                            <asp:AsyncPostBackTrigger ControlID="IliDDL" EventName="SelectedIndexChanged" />
                        </Triggers>
                        </asp:UpdatePanel>
                    <!-- Sonuç Tablosu -->
                    <div class="form-group">
                        <div style="display: none">
                            <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        </div>
                        <table id="CustomDataTable" class="table table-hover table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Adı</th>
                                    <th>Toplam Bağış Miktarı</th>
                                    <th>Son Bağış Tarihi</th>
                                    <th>İl</th>
                                    <th>İlçe</th>
                                    <th>Telefon</th>
                                    <th>Adres</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
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
</div>
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div>
                            <h3>
                                <asp:Label ID="AdiLbl" runat="server" Text="Label"></asp:Label>
                            </h3>
                            <div>
                                <asp:Label ID="TarihLbl" class="col-form-label " runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="card-body">
                            <div style="display: none">
                                <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />
                            </div>
                            <div class="m-1 text-center" id="NakitBagisciDiv">
                                <table id="CustomModalDataTable" class="table table-hover row-border" width="100%">
                                    <thead>
                                        <tr>
                                            <th>Bağış Tarihi</th>
                                            <th>Bağış Miktarı</th>
                                            <th>Armağan</th>
                                            <th>Bağış</th>
                                            <th>Armağan Durumu</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
             <asp:UpdateProgress ID="updateProgress1" runat="server">
                <ProgressTemplate>
                    <div class='loaderMainContainer'>
                        <div class='loaderContainer'>
                            <div class='loaderCircle'></div>
                        </div>
                    </div>

                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>
</div>