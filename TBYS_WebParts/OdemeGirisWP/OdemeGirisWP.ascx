<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OdemeGirisWP.ascx.cs" Inherits="TBYS_WebParts.OdemeGirisWP.OdemeGirisWP" %>

<script type="text/javascript">
    function OpenModalOnay() {
        $("#ModalOnay").modal({ backdrop: "static" });
    }
    function OpenKiraciSecModal() {
        $("#KiraciSecDiv").modal({ backdrop: false });
    }
    function CloseModalOnay() {
        $("#ModalOnay").modal('hide');

    }
    function CallButtonClick(kiraciId) {
        document.getElementById('<%= paramKiraciIdLbl.ClientID%>').value = kiraciId;
            document.getElementById('<%= KiraciSecNowBtn.ClientID%>').click();
        }
</script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Ödeme Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-2 form-group">
                    <asp:Label CssClass="col-form-label text-white" runat="server" Font-Bold="True">. . . . . . . . .</asp:Label>
                    <asp:LinkButton ID="KiraciSecBtn" CssClass="btn btn-outline-primary form-control " CausesValidation="false" runat="server" Text="Kiracı Seç ..." OnClick="KiraciSecBtn_Click" />
                </div>
                <div class="col form-group">
                    <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Kiracı Adı </asp:Label>
                    <asp:Label ID="KiraciAdiLbl" runat="server" CssClass="form-control col-form-label" Enabled="True"></asp:Label>
                </div>
                <div class="col form-group">
                    <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Kiracı Adresi </asp:Label>
                    <asp:Label ID="KiraciAdresiLbl" runat="server" CssClass="form-control col-form-label" Enabled="False"></asp:Label>
                </div>
            </div>
            <div class="form-group alert-secondary p-2">
                <div class="form-group ">
                    <div class="row">
                        <div class="col-2 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Ödeme Tutarı </asp:Label>
                            <asp:TextBox ID="OdemeTutariTxt" runat="server" CssClass="form-control input-money text-right"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemeTutariTxt" ForeColor="Red" ErrorMessage="Ödeme Tutarı giriniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-2 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Ödeme Tarihi </asp:Label>
                            <asp:TextBox ID="OdemeTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" AutoPostBack="True" OnTextChanged="OdemeTarihiTxt_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemeTarihiTxt" ForeColor="Red" ErrorMessage="Ödeme tarihi seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-1 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Saati </asp:Label>
                            <asp:TextBox ID="OdemeSaatiTxt" runat="server" class="form-control input-time" placeholder="hh:mm" ></asp:TextBox>
                        </div>
                        <div class="col-4 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Sözleşme Yılı </asp:Label>
                            <asp:DropDownList ID="SozlesmeDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="SozlesmeDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="SozlesmeDDL" ForeColor="Red" ErrorMessage="Sözleşme seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-2 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Ödeme Plani Ayı</asp:Label>
                            <asp:DropDownList ID="OdemePlaniDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="OdemePlaniDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemePlaniDDL" ForeColor="Red" ErrorMessage="Ödeme ayı seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Açıklama </asp:Label>
                    <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                </div>
            </div>

        </div>
        <div class="card-footer">
            <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click" />
            <asp:LinkButton ID="SilBtn" CssClass="btn btn-outline-danger" runat="server" Text="Ödemeyi Sil" OnClick="SilBtn_Click" />
            <asp:LinkButton ID="KiraciAylikOdemeBtn" CssClass="btn btn-outline-secondary" runat="server"  CausesValidation="false" Text="Aylık Ödemeler" OnClick="KiraciAylikOdemeBtn_Click" />
            <asp:LinkButton ID="OdemePlaniBtn" CssClass="btn btn-outline-secondary float-right" runat="server"  CausesValidation="false" Text="Son Ödeme Plani" OnClick="OdemePlaniBtn_Click" />
            <asp:LinkButton ID="SozlesmeBtn" CssClass="btn btn-outline-secondary float-right" runat="server"  CausesValidation="false" Text="Sözleşme" OnClick="SozlesmeBtn_Click" />
            <asp:LinkButton ID="KiraciBtn" CssClass="btn btn-outline-secondary float-right" runat="server"  CausesValidation="false" Text="Kiraci" OnClick="KiraciBtn_Click" />
            <asp:LinkButton ID="KiraciListBtn" CssClass="btn btn-outline-secondary float-right" runat="server"  CausesValidation="false" Text="Kiraci Listesi" OnClick="KiraciListBtn_Click" />
        </div>
    </div>
</div>
<div class="modal" id="KiraciSecDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content" style="width: 1000px;">
            <div class="modal-body">
                <div style="display: none">
                    <input id="paramKiraciIdLbl" runat="server" type="text" />
                    <asp:LinkButton ID="KiraciSecNowBtn" runat="server"  CausesValidation="false" OnClientClick="{return true;};" OnClick="KiraciSecNowBtn_Click"></asp:LinkButton>
                </div>
                <div>
                    <div class="text-center">
                        <h3>
                            <asp:Label ID="Label1" class="col-form-label " runat="server" Text="Kiracı Listesi"></asp:Label></h3>
                    </div>
                    <div class="card-body">
                        <div class="table loader" id="tbl" runat="server">
                            <input id="globalFilter" placeholder="Aranacak Kelime" size="30" />
                            <div id="tblfilter" class="table"></div>
                            <div id="messages"></div>
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
<div class="modal" id="ModalOnay" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header">
                            <h3>
                                <asp:Label ID="ModalLbl" class="col-form-label text-primary font-weight-bold" Text="Ödeme Güncellenecek" runat="server"></asp:Label></h3>
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label ID="MessageLbl" runat="server" class="col-form-label"></asp:Label>
                                <asp:HiddenField ID="kaydetGuncelleSilHdn" runat="server" />
                            </div>
                        </div>
                        <div class="card-footer">
                            <asp:LinkButton ID="OnaylaBtn" Text="Onayla" runat="server" class="btn btn-outline-primary" OnClick="OnaylaBtn_Click"></asp:LinkButton>
                            <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">İptal</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
