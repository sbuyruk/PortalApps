<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OdemePlaniWP.ascx.cs" Inherits="TBYS_WebParts.OdemePlaniWP.OdemePlaniWP" %>
<style>
    .border-2 {
        border-width: 2px !important;
        font-weight: bold !important;
        border-style: inset !important;
    }
</style>
<script>
    function OdeBtnEnable() {
        var tutar = $('#<%= OdenenTutarTxt.ClientID%>').val().replace(/ /g, '') == '';
        var tarih = $('#<%= OdemeTarihiTxt.ClientID%>').val().replace(/ /g, '') == '';
        if (tutar || tarih) {
            $("#OdeBtn").attr('class', 'btn btn-outline-secondary');
            $("#OdeBtn").attr('disabled', true);
            $('#<%= OdeBtnDiv.ClientID%>').hide();
        }
        else {
            $("#OdeBtn").attr('class', 'btn btn-outline-success');
            $("#OdeBtn").attr('disabled', false);
            $('#<%= OdeBtnDiv.ClientID%>').show();
        }
    }
    function OpenModalOnay() {
        $("#ModalOnayDiv").modal({ backdrop: "static" });
    }
    function OpenModalOdemePlani() {
        $("#OdemePlaniModal").modal({ backdrop: "static" });
    }
</script>

<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="OdemePlaniUP" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <asp:Label CssClass="col-form-label text-danger float-right" ID="DosyaNoTxt" runat="server"></asp:Label>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Ödeme Planı"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body " id="MainCardDiv" runat="server">
                    <div class="form-group">
                        <div class="form-group ">
                            <asp:LinkButton ID="OdemePlaniEkleBtn" class="btn btn-outline-success mb-1" runat="server" Visible="false" Text="Ödeme Planı Ekle" OnClick="OdemePlaniEkleBtn_Click"></asp:LinkButton>
                        </div>
                        <div class="form-group">
                            <asp:LinkButton ID="OdemePlaniSilBtn" class="btn btn-outline-danger  float-right mb-1" runat="server" Visible="false" Text="Ödeme Planı Sil" OnClick="OdemePlaniSilBtn_Click"></asp:LinkButton>
                        </div>
                        <div class="form-group" style="display: block" runat="server">
                            <asp:CheckBox ID="GecikmeZammmiTipiChk" CssClass="float-right  mr-3" AutoPostBack="true" runat="server" Text="Günlük Gecikme Zammı Uygula " Checked="false" OnCheckedChanged="GecikmeZammmiTipiChk_CheckedChanged" TextAlign="Right" />
                        </div>
                    </div>
                    <div id="PlanAyrintiDiv">
                        <div class="mt-2">
                            <asp:Table ID="AyrintiTable" runat="server" class="table table-hover table-striped table-sm ">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Sıra </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Vade Tar</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Kira</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Ödenen</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Anapara</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Faiz Oranı </asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Faiz Tutarı</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="text-right">Faizli Bakiye</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ödeme</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                    </div>
                    <div id="OdemeYapDiv" class="border border-dark p-2" style="display: none" runat="server">

                        <div class="row">
                            <div class="form-group col-2">
                                <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Ödeme Tarihi</asp:Label>
                                <input runat="server" type="text" id="OdemeTarihiTxt" name="OdemeTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly"
                                    onchange="OdeBtnEnable()" onkeyup="OdeBtnEnable()" oncut="OdeBtnEnable()" onpaste="OdeBtnEnable()" oninput="OdeBtnEnable()" />
                            </div>
                            <div class="form-group col-2">
                                <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Ödenen Tutar</asp:Label>
                                <input id="OdenenTutarTxt" runat="server" class="form-control input-money text-right" type="text"
                                    onchange="OdeBtnEnable()" onkeyup="OdeBtnEnable()" oncut="OdeBtnEnable()" onpaste="OdeBtnEnable()" oninput="OdeBtnEnable()" />
                            </div>
                            <div class="form-group col-6">
                                <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Aciklama</asp:Label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group col-2" style="display: none" id="OdeBtnDiv" runat="server">
                                <asp:Label CssClass="col-form-label text-white" runat="server" Font-Bold="True">-----------:)</asp:Label>
                                <asp:LinkButton ID="OdeBtn" ClientIDMode="Static" runat="server" class="form-control btn btn-outline-success" type="text" OnClick="OdeBtn_Click">Ödeme Yap</asp:LinkButton>
                            </div>
                        </div>

                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-success" ID="YeniOdemeGirisiBtn" runat="server" Text="Yeni Ödeme Girişi" OnClick="YeniOdemeGirisiBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                    <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Sonraki=>" OnClick="NextBtn_Click" />
                    <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="<=Önceki" OnClick="PrevBtn_Click" />
                    
                    <asp:LinkButton ID="SozlesmeBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Sözleşme" OnClick="SozlesmeBtn_Click" />
                    <asp:LinkButton ID="KiraciBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kiracı" OnClick="KiraciBtn_Click" />
                    <asp:LinkButton ID="KiraciKartiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kira Kartı" OnClick="KiraciKartiBtn_Click" />
                    <asp:LinkButton ID="OdemePlaniListBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Ödeme Planları" OnClick="OdemePlaniListBtn_Click" />
                    <asp:LinkButton ID="KiraciAylikOdemeBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Aylık Ödemeler" OnClick="KiraciAylikOdemeBtn_Click" />
                    <asp:LinkButton ID="BakiyeDevirBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Bakiye Devir İşlemleri" OnClick="BakiyeDevirBtn_Click" />
                </div>
            </div>
            <%--Modal: Odemelerin Listesi--%>
            <div class="modal" id="OdemePlaniModal" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <div class="modal-body">
                            <div>
                                <h3>
                                    <asp:Label ID="BaslikLbl" class="col-form-label " runat="server"></asp:Label>

                                </h3>
                                <div>
                                    <asp:Label ID="TarihLbl" class="col-form-label " runat="server"></asp:Label>
                                </div>
                            </div>

                            <div class="card-body">
                                <asp:Table ID="OdemePlaniTable" runat="server" class="table table-sm table-striped table-bordered">
                                    <asp:TableHeaderRow>
                                        <asp:TableHeaderCell>Sıra No </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Ödeme Tarihi</asp:TableHeaderCell>
                                        <asp:TableHeaderCell HorizontalAlign="Right">Ödenen Tutar</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Açıklama </asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                </asp:Table>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
            <%--Modal: Onay Popup penceresi--%>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog modal-lg">
                    <!-- Modal content-->
                    <div class="modal-content">

                        <div class="modal-body">
                            <div style="display: none">
                            </div>
                            <div>
                                <div class="text-center">
                                    <h4>
                                        <asp:Label ID="PopupMesajLbl" class="col-form-label text-danger font-weight-bold" runat="server" Text=""></asp:Label></h4>
                                    <asp:Label ID="VadeBasTarLbl" CssClass="col-form-label col-3" runat="server" Text="Vade Bas. Tar."></asp:Label>
                                </div>
                                <div class="card-body p-0">

                                    <div style="display: none" class="border-2" id="KiraBedeliDegistirDiv" runat="server">
                                        <asp:HiddenField ID="HiddenOdemePlaniId" runat="server" />
                                        <div class="form-group row ">

                                            <asp:Label ID="SiraLbl" CssClass="col-form-label col-2 m-1" runat="server" Text="Sıra"></asp:Label>
                                            <asp:Label ID="YeniKiraBedeliLbl" CssClass="col-form-label text-danger col-3 m-1" runat="server" Text="Kira Bedeli : "></asp:Label>
                                            <asp:TextBox ID="YeniKiraBedeliTxt" CssClass="form-control input-money text-right col-3 m-1" runat="server"></asp:TextBox>
                                            <asp:LinkButton ID="KiraBedeliniDegistirBtn" ClientIDMode="Static" runat="server" CssClass="form-control btn btn-primary col-3 m-1" type="text" OnClientClick="{return true;};" OnClick="KiraBedeliniDegistirBtn_Click">Güncelle</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div style="display: none" class="border" id="GecikmeZammiOraniGosterDiv" runat="server">
                                        <asp:Table ID="GecikmeZammiTable" runat="server" class="table table-striped table-hover table-sm table-bordered">
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell Width="100">Tarih Aralığı</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100">Ana Para</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100">Ödenen Tutar</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100">Kalan Ana Para</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100">Gecikme Zammi Oranı</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100">Gecikme Zammi Tutarı</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="200">Açıklama</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                    </div>
                                    <div style="display: none" class="border-2" id="VadeGosterDiv" runat="server">                                        
                                        <div class="form-group row ">
                                            <div class="form-group col-6">
                                                <asp:Label ID="Label1" CssClass="col-form-label col-2 m-1" runat="server" Text="Vade Başlangıcı"></asp:Label>
                                                <input runat="server" type="text" id="VadeBasTarTxt" name="VadeBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div class="form-group col-6">
                                                <asp:Label ID="Label2" CssClass="col-form-label col-2 m-1" runat="server" Text="Vade Sonu"></asp:Label>
                                                <input runat="server" type="text" id="VadeBitTarTxt" name="VadeBitTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                        </div>
                                        <div class="form-group" >
                                            <asp:LinkButton ID="VadeDegistirBtn" ClientIDMode="Static" runat="server" CssClass="form-control btn btn-primary col-3 m-1" type="text" OnClientClick="{return true;};" OnClick="VadeDegistirBtn_Click">Güncelle</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
<%--        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="OdemePlaniEkleBtn" EventName="Click" />
        </Triggers>--%>
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
