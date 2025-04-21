<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraBakiyeDevriWP.ascx.cs" Inherits="TBYS_WebParts.KiraBakiyeDevriWP.KiraBakiyeDevriWP" %>


<script>
    function DevirAl(sozlesmeId) {
        document.getElementById('<%= paramSozlesmeIdLbl.ClientID%>').value = sozlesmeId;
        document.getElementById('<%= DevirAlBtn.ClientID%>').click();
    }
    //eğer aktif=0 ise satırı gri yap
    function SetRowColor(rowData, prop, counter) {
        if (rowData.Aktif == 'False') {
            var trElement = document.getElementsByTagName("table")[0];
            var row = trElement.rows[counter];
            $(row).addClass('table-secondary'); 
        }
        if (rowData.SozlesmeBasladi < 0) { //ileri tarhli
            var trElement = document.getElementsByTagName("table")[0];
            var row = trElement.rows[counter];
            $(row).addClass('table-warning'); 
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
                        <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Kira Bakiye Devri"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="form-group col">
                            <div class="form-group">
                                <label class="col-form-label" for="AdiLbl">Adı/Ünvanı</label>
                                <asp:Label ID="AdiLbl" runat="server" class="form-control " ToolTip="Kiracının Adı" ReadOnly="true"></asp:Label>
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="AdresLbl">Adres</label>
                                <asp:Label ID="AdresLbl" runat="server" class="form-control " ToolTip="Kiracının adresi" ReadOnly="true"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group col">
                            <div class="form-group row">
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="TCKimlikNoLbl">TC Kimlik Nu</label>
                                        <asp:Label ID="TCKimlikNoLbl" runat="server" class="form-control " ToolTip="Kiracının TC Kimlik Numarası" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="KiralamaAmaciLbl">Kir. Amacı</label>
                                        <asp:Label ID="KiralamaAmaciLbl" runat="server" class="form-control" ToolTip="Kiralama Amacı" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="IliDDL">İli</label>
                                        <asp:Label ID="IliLbl" runat="server" class="form-control" ReadOnly="true" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="IlcesiLbl">İlçesi</label>
                                        <asp:Label ID="IlcesiLbl" runat="server" class="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="SorumluBolgeLbl">Sor. Bölge</label>
                                        <asp:Label ID="SorumluBolgeLbl" runat="server" class="form-control " ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="TelefonLbl">Telefon</label>
                                        <asp:Label ID="TelefonLbl" runat="server" class="form-control " ToolTip="Kiracının telefonu" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th colspan="9">
                                        <h3 class="text-center">Kiracının Sözleşmeleri</h3> 
                                    </th>
                                </tr>
                                <tr>
                                    <th>Dosya No</th>
                                    <th>Tarih Aralığı</th>
                                    <th>Kira Bedeli</th>
                                    <th>Devir Anapara</th>
                                    <th>Devir Faiz</th>
                                    <th>Devir Faizli Bakiye</th>
                                    <th>Devir Al</th>
                                    <th>Sözleşme</th>
                                    <th>Ödeme Planı</th>
                                </tr>
                            </thead>
                        </table>
                    </div>

                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Sonraki=>" Visible="false" OnClick="NextBtn_Click" />
                    <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="<=Önceki" Visible="false" OnClick="PrevBtn_Click" />

                    <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Ödm.Pln Görüntüle" Visible="false" OnClick="OdemePlaniGoruntuleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="OdemePlaniBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Ödeme Planı" Visible="false" OnClick="OdemePlaniBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KiraKartiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Kira Karti" Visible="false" OnClick="KiraKartiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KiraciListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="Kiracı Listesi" OnClick="KiraciListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KiraciAylikOdemeBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Aylık Ödemeler" OnClick="KiraciAylikOdemeBtn_Click" />
                </div>
            </div>
            <div style="display: none">
                <input id="paramSozlesmeIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="DevirAlBtn" runat="server" OnClientClick="{return true;};" OnClick="DevirAlBtn_Click"></asp:LinkButton>
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
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
