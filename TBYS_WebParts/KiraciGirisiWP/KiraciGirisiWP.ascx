<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraciGirisiWP.ascx.cs" Inherits="TBYS_WebParts.KiraciGirisiWP.KiraciGirisiWP" %>
<style>
    .ileri-tarihli-sozlesme {
        background-color: yellow;
    }
</style>
<script>
    function OpenModalOnay() {
        $("#OdemePlaniModal").modal({ backdrop: "static" });
    }
    function DeleteModalOnay() {
        $("#DeleteModalOnayDiv").modal({ backdrop: "static" });
    }
    function YeniSozlesmeModalOnay() {
        $("#YeniSozlesmeModal").modal({ backdrop: "static" });
    }
    function BitenSozlesmeModalOnay() {
        $("#BitenSozlesmeModal").modal({ backdrop: "static" });
    }
    //eğer aktif=0 ise satırı gri yap
    function contentFunc(rowData, prop, counter) {
        if (rowData.Aktif == 'False') {
            var trElement = document.getElementsByTagName("table")[0];
            var rowx = trElement.rows[counter];
            rowx.classList.add("ui-widget-content-disabled");
        }
        if (rowData.SozlesmeBasladi < 0) { //ileri tarhli
            var trElement = document.getElementsByTagName("table")[0];
            var rowx = trElement.rows[counter];
            rowx.classList.add("ileri-tarihli-sozlesme");
        }
    }
</script>
<div class="container shadow ">
    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card">
                <div class="card-header ">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Kiracı Düzenleme"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label" Visible="false"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="form-group col">
                            <div class="form-group">
                                <label class="col-form-label" for="AdiTxt">Adı/Ünvanı</label>
                                <asp:TextBox ID="AdiTxt" runat="server" class="form-control " ToolTip="Kiracının Adı" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group ">

                                <label class="col-form-label" for="SoyadiTxt">Soyadı/Ünvanı</label>
                                <asp:TextBox ID="SoyadiTxt" runat="server" class="form-control " ToolTip="Kiracının Soyadı" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="AdresTxt">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" class="form-control " TextMode="MultiLine" Rows="4" ToolTip="Kiracının adresi"></asp:TextBox>
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="EpostaTxt">E-Posta</label>
                                <asp:TextBox ID="EpostaTxt" runat="server" class="form-control " ToolTip="Kiracının E-posta Adresi"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col">
                            <div class="form-group row">
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="TCKimlikNoTxt">TC Kimlik Nu</label>
                                        <asp:TextBox ID="TCKimlikNoTxt" runat="server" class="form-control " ToolTip="Kiracının TC Kimlik Numarası" type="number"></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="KiralamaAmaciDDL">Kir. Amacı</label>
                                        <asp:DropDownList ID="KiralamaAmaciDDL" runat="server" class="form-control" ToolTip="Kiralama Amacı" Height="34px" ></asp:DropDownList>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="TelefonTxt">Telefon</label>
                                        <asp:TextBox ID="TelefonTxt" runat="server" class="form-control " ToolTip="Kiracının telefonu"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="IliDDL">İli</label>
                                        <asp:DropDownList ID="IliDDL" runat="server" class="form-control  " OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="IlcesiDDL">İlçesi</label>
                                        <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control" Height="34px" ></asp:DropDownList>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="SemtTxt">Semt</label>
                                        <asp:TextBox ID="SemtTxt" runat="server" class="form-control "></asp:TextBox>
                                    </div>

                                </div>
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="SorumluBolgeTxt">Sor. Bölge</label>
                                        <asp:TextBox ID="SorumluBolgeTxt" runat="server" class="form-control " ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="form-group ">
                                        <label class="col-form-label" for="VergiDairesiTxt">Vergi Dairesi</label>
                                        <asp:TextBox ID="VergiDairesiTxt" runat="server" class="form-control " ToolTip="Kiracının Vergi Dairesi"></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="VergiNoTxt">Vergi No</label>
                                        <asp:TextBox ID="VergiNoTxt" runat="server" class="form-control " ToolTip="Kiracının Vergi Numarası"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control " TextMode="MultiLine" Rows="4" ToolTip="Kiracıya ait notlar"></asp:TextBox>
                            </div>
                        </div>

                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-hower table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Dosya No</th>
                                    <th>Sözleşme tarihi</th>
                                    <th>Ödeme Şekli</th>
                                    <th>Kira Bedeli</th>
                                    <th>Adres</th>
                                    <th>Sözleşme</th>
                                    <th>Ödeme Planı</th>
                                    <th>Sözleşme Durumu</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div class="form-group">
                        <asp:LinkButton ID="BitenSozlesmeOlusturBtn" runat="server" CssClass="btn btn-outline-success float-right" Text="Bitmiş Sözleşme Oluştur" Visible="false" OnClick="BitenSozlesmeOlusturBtn_Click"></asp:LinkButton>
                        <asp:LinkButton ID="YeniSozlesmeOlusturBtn" runat="server" CssClass="btn btn-outline-success" Text="Yeni Sözleşme Oluştur" Visible="false" OnClick="YeniSozlesmeOlusturBtn_Click"></asp:LinkButton>
                    </div>
                </div>
                <div class="card-footer">

                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="SilBtn" Visible="false" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="SilBtn_Click" />
                    <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Sonraki=>" Visible="false" OnClick="NextBtn_Click" />
                    <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="<=Önceki" Visible="false" OnClick="PrevBtn_Click" />

                    <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Ödeme Planı Görüntüle" Visible="false" OnClick="OdemePlaniGoruntuleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="OdemeYapBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Ödeme Planı" Visible="false" OnClick="OdemeYapBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KiraKartiBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Kira Karti" Visible="false" OnClick="KiraKartiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KiraciListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="Kiracı Listesi" OnClick="KiraciListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="BakiyeDevirBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Bakiye Devir İşlemleri" OnClick="BakiyeDevirBtn_Click" />
                </div>
            </div>
            <div class="modal" id="OdemePlaniModal" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <div class="modal-body">
                            <div>
                                <h3>Kira Ödeme Planı
									<asp:Label ID="KiraciTitleLbl" class="label label-primary " runat="server"></asp:Label>
                                </h3>
                            </div>

                            <div class="card-body">
                                <asp:Label ID="DevirLbl" class="col-form-label" runat="server" Font-Bold="True"></asp:Label>
                                <asp:Table ID="OdemePlaniTable" runat="server" class="table table-sm">
                                    <asp:TableHeaderRow>
                                        <asp:TableHeaderCell>Sıra No </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Yil </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Ay </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Kira Bedeli </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Ödenen Tutar</asp:TableHeaderCell>
                                        <%--<asp:TableHeaderCell>Ödeme Tarihi</asp:TableHeaderCell>--%>
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
            <div class="modal" id="YeniSozlesmeModal" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="panel-heading alignCenter">
                                <h3>
                                    <asp:Label ID="ModalLbl" class="label label-danger " Text="Yeni Sözleşme Oluşturulacak" runat="server"></asp:Label></h3>
                            </div>
                            <div class="panel panel-body">
                                <div class="alignCenter">
                                    <asp:Label ID="MessageLbl" runat="server" class="text-danger">Onayladığınız takdirde bu kiracı için yeni bir sözleşme kaydı açılacak</asp:Label>
                                </div>

                            </div>
                            <div style="display: none">
                                <input id="paramLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="YeniSozlesmeOnayBtn" Text="Yeni Sözleşme Oluştur" runat="server" class="btn btn-danger" OnClick="YeniSozlesmeOnayBtn_Click"></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-dismiss="modal">İptal</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal" id="BitenSozlesmeModal" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-body">
                            <div class="panel-heading alignCenter">
                                <h3>
                                    <asp:Label ID="Label1" class="label label-danger " Text="Bitmiş Sözleşme Oluşturulacak" runat="server"></asp:Label></h3>
                            </div>
                            <div class="panel panel-body">
                                <div class="alignCenter">
                                    <asp:Label ID="Label2" runat="server" class="text-danger">Onayladığınız takdirde bu kiracı için bitmiş bir sözleşme kaydı açılacak</asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="BitenSozlesmeOnayBtn" Text="Bitmiş Sözleşme Oluştur" runat="server" class="btn btn-danger" OnClick="BitenSozlesmeOnayBtn_Click"></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-dismiss="modal">İptal</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal" id="DeleteModalOnayDiv" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="modal-body">
                                    <div class="card" runat="server" id="DeleteDiv" style="display: none">
                                        <div class="card-header">
                                            <h3>
                                                <asp:Label ID="Label3" class="label label-primary " runat="server" Text="Kiracı Kaydı Silinecek"></asp:Label></h3>
                                        </div>
                                        <div class="card-body">
                                            <asp:TextBox ID="SilmeSebebiTxt" TextMode="MultiLine" Rows="3" runat="server" class="form-control" type="text" />
                                            <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Kiracıyı Silmeyi Onaylıyor musunuz?"></asp:Label>
                                        </div>
                                        <div class="card-footer">
                                            <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Kiracyı Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" />
                                            <button type="button" class="btn btn-default" data-dismiss="modal">İptal</button>
                                        </div>
                                    </div>
                                    <div class="card" runat="server" id="WarnDiv" style="display: none">
                                        <div class="card-header">
                                            <h3>
                                                <asp:Label ID="Label5" class="label label-primary " runat="server" Text="Kiracı Kaydı Silinemiyor"></asp:Label></h3>
                                        </div>
                                        <div class="card-body">
                                            <asp:Label ID="UyariLbl" CssClass="col-form-label text-danger" runat="server" Text="Kiracıya ait Sözleşme bulunduğundan kayıt silinemedi."></asp:Label>
                                            <asp:Label ID="Label4" CssClass="col-form-label text-danger" runat="server" Text="Kiracıyı silmeden önce yapılmış sözleşmelerin silinmesi gerekmektedir."></asp:Label>
                                        </div>
                                        <div class="card-footer">
                                            <button type="button" class="btn btn-default float-right" data-dismiss="modal">Kapat</button>
                                        </div>
                                    </div>
                                    <div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
