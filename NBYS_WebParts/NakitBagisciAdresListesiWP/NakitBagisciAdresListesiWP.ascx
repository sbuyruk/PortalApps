<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciAdresListesiWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciAdresListesiWP.NakitBagisciAdresListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
</style>

<script>
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
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
    function CallButtonClick(bagisciId, dergiGonder) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = bagisciId;
        document.getElementById('<%= isDergiGonderLbl.ClientID%>').value = dergiGonder;
        document.getElementById('<%= DergiGondermeBtn.ClientID%>').click();

    }
</script>

<div class="container col-xl">
<%--    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>--%>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Nakit Bağışçı Adresleri (Belge Verilen)"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body mt-1">
                    <div class="row border border-dark pt-2 bg-light">
                        <div class="col-5 form-group row">
                            <div class="col form-group">
                                <label for="BagisciSayisiTxt" class="col-form-label fw-bold">Bağışçı Say.: </label>
                                <asp:TextBox ID="BagisciSayisiTxt" AutoPostBack="true" runat="server" CssClass="form-control input-integerOnly text-end" OnTextChanged="BagisTarihiTxt_TextChanged" ></asp:TextBox>
                            </div>
                            <div class="col form-group">
                                <label class="col-form-label fw-bold" for="BagisTarihiTxt">Baş.Tarihi</label>
                                <asp:TextBox ID="BasTarTxt" AutoPostBack="true" runat="server" CssClass="form-control float-left mb-2 DateTimePickerV1" OnTextChanged="BagisTarihiTxt_TextChanged" ></asp:TextBox>
                            </div>
                            <div class="col form-group">
                                <label class="col-form-label fw-bold" for="BagisTarihiTxt">Bit.Tarihi</label>
                                <asp:TextBox ID="BitTarTxt" AutoPostBack="true" runat="server" CssClass="form-control float-left mb-2 DateTimePickerV1" OnTextChanged="BagisTarihiTxt_TextChanged"  ></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-7 form-group row">
                            <div class="col form-group">
                                <asp:CheckBox ID="UlasilamayanlarHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Ulaşılamayanlar Hariç" Checked="True" />
                                <asp:CheckBox ID="BelgeIstemeyenlerHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Belge İstemeyenler Hariç" Checked="True" />
                                <asp:CheckBox ID="AdresiBosOlanlarHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Adresi Boş Olanlar Hariç" Checked="True" />
                            </div>
                            <div class="col form-group">
                                <asp:CheckBox ID="BelgesiPostadanIadeEdilenlerHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Belgesi Postadan İade edilenler Hariç" Checked="True" />
                                <asp:CheckBox ID="DergiGonderilmesinlerHaricChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Dergi Gönderilmeyecek Olanlar Hariç" Checked="True" />
                                <asp:CheckBox ID="SadeceYeniBagiscilarChk" AutoPostBack="true" runat="server" CssClass="form-control" OnCheckedChanged="BagisTarihiTxt_TextChanged" Text=" Sadece Yeni Bağışçılar" Checked="false" />
                            </div>
                        </div>
                    </div>
                    <div class="mt-3" >
                        <div style="display: none">
                            <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                            <input id="isDergiGonderLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                            <asp:LinkButton ID="DergiGondermeBtn" runat="server" OnClientClick="{return true;};" OnClick="DergiGondermeBtn_Click"></asp:LinkButton>
                        </div>

                        <div class="form-group">
                            <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                                <thead>
                                    <tr>
                                        <th>Adı</th>
                                        <th>T/O</th>
                                        <th>Adres</th>
                                        <th>İli</th>
                                        <th>İlçesi</th>
                                        <th>Telefon</th>
                                        <th>Duzenle</th>
                                        <th>Dergi</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
            </div>
<%--        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ModalDoldurBtn" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>--%>

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

<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="BagisciAdiLbl" runat="server" Text="Bağışçı Bilgileri" Font-Bold="True"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClick="ModalDoldurBtn_Click" />
                        </div>

                        <div class="m-1 text-center" id="NakitBagisciDiv">
                            <asp:Table CssClass="table text-center table-bordered table-striped" ID="BagisciTable" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Ad/Ünvan</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>TC Kimlik No</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Adres</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>İli/İlçesi</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Telefon</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Tüzel Kişi</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                        <div>
                            <h3>
                                <br />
                                <asp:Label ID="BagisBilgileriLbl" runat="server" Text="Bağışçının Yaptığı Nakit Bağışlar" Font-Bold="True"></asp:Label>
                            </h3>
                        </div>
                        <div class="form-group">
                            <table id="CustomModalDataTable" class="table table-bordered table-striped" width="100%">
                                <thead>
                                    <tr>
                                        <th>Bağış Tarihi</th>
                                        <th>Bağış Miktarı</th>
                                        <th>Banka</th>
                                        <th>Armağan</th>
                                        <th>Armağan Tutarı</th>
                                        <th>Armağan Durumu</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
