<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BeratBelgesiYazisiWP.ascx.cs" Inherits="NBYS_WebParts.BeratBelgesiYazisiWP.BeratBelgesiYazisiWP" %>
<script>
    function CallButtonClick(fileName, labelFileName) {
        document.getElementById('<%= paramDosyaAdiLbl.ClientID%>').value = fileName;
        document.getElementById('<%= paramEtiketDosyaAdiLbl.ClientID%>').value = labelFileName;
        document.getElementById('<%= DosyayiSilBtn.ClientID%>').click();
    }
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>
<div class="container ">

            <div class="card shadow">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Berat Belgesi Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row p-2">
                        <div class="form-group col-4 border border-dark border-right-0">
                            <div class="row">
                                <div class="form-group col-4" style="display: block">
                                    <label class="col-form-label" for="GunDDL">Gün </label>
                                    <asp:DropDownList ID="GunDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="GunDDL_SelectedIndexChanged" Style="height: auto" />
                                </div>
                                <div class="form-group col-4">
                                    <label class="col-form-label" for="AyDDL">Ay </label>
                                    <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                                </div>
                                <div class="form-group col-4">
                                    <label class="col-form-label" for="YilDDL">Yıl </label>
                                    <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col-8">
                                    <label class="col-form-label" for="BolgeDDL">Bölge </label>
                                    <asp:DropDownList ID="BolgeDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="BolgeDDL_SelectedIndexChanged" Style="height: auto" />
                                </div>
                                <div class="form-group col-4">
                                    <label class="col-form-label" for="MadalyaDDL">Madalya </label>
                                    <asp:DropDownList ID="MadalyaDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="MadalyaDDL_SelectedIndexChanged" Style="height: auto" />
                                </div>
                            </div>
                        </div>

                        <div class="form-group col-4 border border-dark border-right-0">
                            <div class="form-group ">
                                <asp:Table ID="DurumTable" runat="server" class="table table-bordered table-striped">
                                </asp:Table>
                            </div>
                        </div>
                        <div class="form-group col-4 border border-dark">
                            <div class="row">
                                <div class="form-group col">
                                    <label for="ImzalayanTxt" class="col-form-label">İmza (Adi Soyadı)</label>
                                    <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <label for="ImzalayanUnvanTxt" class="col-form-label">İmza (Ünvan)</label>
                                    <asp:TextBox ID="ImzalayanUnvanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col">
                                    <label for="ImzalayanMakamTxt" class="col-form-label">İmza (Makam)</label>
                                    <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <label for="EvrakTarihiTxt" class="col-form-label">Evrak Tarihi</label>
                                    <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                    </div>
                </div>
<%--                <div class="form-group">
                    <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Berat Belgeleri</asp:HyperLink>
                    <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiketleri</asp:HyperLink>
                </div>--%>
                <div class="card-footer">
                    <asp:CheckBox ID="BeratDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                    <asp:LinkButton ID="DosyaOlusturBtn" runat="server" CssClass="btn btn-outline-success" OnClick="DosyaOlusturBtn_Click">Berat Belgelerini Oluştur</asp:LinkButton>
                </div>
            </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card">
                <div class="card-body">
                    <div style="display: none">
                        <input id="paramDosyaAdiLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <input id="paramEtiketDosyaAdiLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="DosyayiSilBtn" runat="server" OnClientClick="{return true;};" OnClick="DosyayiSilBtn_Click"></asp:LinkButton>
                    </div>
                    <div id="SuzmeBolumuDiv" class="form-group row">
                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered table-hover" width="100%">
                            <thead>
                                <tr>
                                    <th>Dosya Adı</th>
                                    <th>Etiket Dosyası</th>
                                    <th>Yazan</th>
                                    <th>Tarih</th>
                                    <th>Dosyaları Sil</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:Label ID="Label1" runat="server" Text="Label">İstediğiniz dosyayı indirmek için dosya ismine tıklayınız</asp:Label>
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog modal-dialog-centered">
                    <!-- Modal content-->
                    <div class="modal-content">

                        <div class="modal-body">
                            <div style="display: none">
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Dosya Silinecek"></asp:Label></h3>
                                </div>
                                <div class="card-body text-center">
                                    <div class="form-group">
                                        <asp:Label ID="DosyaAdiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="EtiketDosyaAdiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="SilMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Seçilen Dosyaları Silmek İstiyor musunuz?"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="DosyayiSilNowBtn" runat="server" CausesValidation="false" Text="Dosyayı Sil" OnClientClick="{return true;};" OnClick="DosyayiSilNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default float-end" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
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
