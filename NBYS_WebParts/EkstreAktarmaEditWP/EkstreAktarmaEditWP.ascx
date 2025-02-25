<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EkstreAktarmaEditWP.ascx.cs" Inherits="NBYS_WebParts.EkstreAktarmaEditWP.EkstreAktarmaEditWP" %>
<script type="text/javascript">
    function CallButtonClick(secilenBagisci) {
        paramLbl.value = secilenBagisci;
        var button = document.getElementById("NakitBagisciSecildiBtn");
        if (button != null) {
            button.click();
        }

    }
    function Hesapla() {
<%--        dtutar = document.getElementById('<%= DovizTutariTxt.ClientID%>').value ;
        dkur = document.getElementById('<%= DovizKuruTxt.ClientID%>').value; 
        dtutar = parseFloat(dtutar.replace('.', '').replace(',', '.'));
        dkur = parseFloat(dkur.replace('.', '').replace(',', '.'));
        var tutarTl = dkur * dtutar;
        var str = tutarTl + "";
        str= parseFloat(str.replace('.', ',').replace(' ', ''));
        document.getElementById('<%= HesaplananTlLbl.ClientID%>').value = tutarTl;--%>
        document.getElementById('<%= HesaplaBtn.ClientID%>').click();
    }

    function TutaraYaz() {

        var tutarTl = document.getElementById('<%= HesaplananTlLbl.ClientID%>').value;

        document.getElementById('<%= TutarTlTxt.ClientID%>').value = tutarTl;
    }

</script>
<div class="container w-75">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label btn-outline-info" runat="server" Text="Ekstre Kaydı Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="EksterAktarmaIdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="3" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="pl-2 bg-light">
                <div class="form-group ">
                    <label class="col-form-label">İşlem tarihi</label>
                    <asp:TextBox type="text" ID="IslemTarihiTxt" name="IslemTarihiTxt" class="col-form-label" runat="server" Enabled="false" />
                </div>
            </div>
            <div class="row">
                <div class="col-4 border border-dark alert-secondary m-2">
                    <div class="form-group">
                        <label class="col-form-label">Adı</label>
                        <div class="">
                            <asp:TextBox ID="AdiTxt" runat="server" CssClass="form-control" type="text" />
                            <asp:Label Visible="false" ID="NakitBagisciIdLbl" runat="server" type="text" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                        </div>

                        <div style="display: none">
                            <input id="paramLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                            <asp:LinkButton ID="NakitBagisciSecildiBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="NakitBagisciSecildiBtn_Click" />

                           
                        </div>
                        <label class="col-form-label">Bilinmeyen</label>
                        <asp:CheckBox ID="ChkBilinmeyen" runat="server" CssClass="form-control custom-checkbox" OnCheckedChanged="ChkBilinmeyen_CheckedChanged" AutoPostBack="true" />
                    </div>
                    <div class="form-group ">
                        <label class="col-form-label">TC Kimlik No</label>
                        <asp:TextBox ID="TCKimlikNoTxt" runat="server" CssClass="form-control" type="text" />
                    </div>
                    <div class="form-group ">
                        <label class="col-form-label">Adres</label>
                        <asp:TextBox ID="AdresTxt" TextMode="MultiLine" Rows="3" runat="server" CssClass="form-control" type="text" />
                    </div>
                    <div class="form-group ">
                        <label class="col-form-label">Tüzel Kişi</label>
                        <asp:CheckBox ID="TuzelKisiChk" runat="server" CssClass="form-control custom-checkbox" type="text" />
                    </div>
                </div>
                <div class="col border border-dark alert-light m-2">

                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                            <div style="display: none">
                            <asp:LinkButton ID="HesaplaBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="HesaplaBtn_Click" />
                        </div>
                            <div class="row">
                                <div class="col">
                                    <div class="form-group ">
                                        <label class="col-form-label">Banka</label>
                                        <asp:DropDownList ID="BankaDDL" runat="server" CssClass="form-control small" Style="height: auto"></asp:DropDownList>
                                    </div>
                                    <div class="row p-1">
                                        <div class="col form-group ">
                                            <label class="col-form-label">Bağış Tarihi</label>
                                            <input type="text" id="BagisTarihiTxt" name="BagisTarihiTxt" class="form-control DateTimePickerV1 " runat="server" readonly="readonly" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="BagisTarihiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="col form-group ">
                                            <label class="col-form-label">Tutar (TL)</label>
                                            <asp:TextBox ID="TutarTlTxt" runat="server" CssClass="form-control input-money text-end" type="text" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TutarTlTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label">Para Birimi</label>
                                        <asp:DropDownList ID="DovizCinsiDDL" runat="server" CssClass="form-control small" Style="height: auto" AutoPostBack="True" OnSelectedIndexChanged="DovizCinsiDDLIli_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="form-group border border-dark p-2" id="DovizDiv" runat="server" style="display: none">
                                        <div class="row">
                                            <div class="col form-group ">
                                                <label class="col-form-label">Dözviz Tutarı</label>
                                                <asp:TextBox ID="DovizTutariTxt" runat="server" CssClass="form-control input-money text-end" AutoPostBack="true" OnTextChanged="HesaplaBtn_Click" type="text" />
                                            </div>
                                            <div class="col form-group ">
                                                <label class="col-form-label">Kur</label>
                                                <asp:TextBox ID="DovizKuruTxt" runat="server" CssClass="form-control input-money text-end" AutoPostBack="true" OnTextChanged="HesaplaBtn_Click" type="text" />
                                            </div>

                                        </div>
                                        <div class="row">
                                            <div class="col form-group ">
                                                <label class="col-form-label">Kur Tarihi</label>
                                                <asp:TextBox ID="KurTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" type="text" />
                                            </div>
                                            <div class="col form-group ">
                                                <asp:TextBox ID="HesaplananTlLbl" runat="server" CssClass="form-control input-money text-end" type="text" Enabled="False" />
                                                <asp:LinkButton ID="TutaraYazBtn" runat="server" CssClass="form-control btn btn-secondary" OnClientClick="TutaraYaz();" CausesValidation="False">Tutara Yaz</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>


                                </div>
                                <div class="col">
                                    <div class="row">
                                        <div class="col form-group ">
                                            <label class="col-form-label">İl</label>
                                            <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                        </div>
                                        <div class="col form-group ">
                                            <label class="col-form-label">İlçe</label>
                                            <%--<asp:TextBox ID="IlcesiTxt" Enabled="false" runat="server" class="form-control" type="text" />--%>
                                            <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control small" Style="height: auto"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col form-group ">
                                            <label class="col-form-label">Telefon1</label>
                                            <asp:TextBox ID="Telefon1Txt" runat="server" CssClass="form-control" type="text" />
                                        </div>
                                        <div class="col form-group ">
                                            <label class="col-form-label">Telefon2</label>
                                            <asp:TextBox ID="Telefon2Txt" runat="server" CssClass="form-control" type="text" />
                                        </div>
                                    </div>

                                    <div class="form-group ">
                                        <label class="col-form-label">Posta Kodu</label>
                                        <asp:TextBox ID="PostaKoduTxt" runat="server" CssClass="form-control" type="text" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label">EPosta</label>
                                        <asp:TextBox ID="EPostaTxt" runat="server" CssClass="form-control" type="text" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label">Belge İstemiyor</label>
                                        <asp:CheckBox ID="BelgeIstemiyorChk" runat="server" CssClass="form-control custom-checkbox" type="text" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label">Fiş No</label>
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="FisNoTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                        <asp:TextBox ID="FisNoTxt" runat="server" CssClass="form-control" type="text" />
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
                    <div class="form-group ">
                        <label class="col-form-label">Aciklama</label>
                        <asp:TextBox ID="AciklamaTxt" TextMode="MultiLine" Rows="3" runat="server" CssClass="form-control" type="text" />
                    </div>
                </div>

            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-primary m-2" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-primary m-2" ID="EslestirBtn" runat="server" Text="Eşleştir" OnClick="EslestirBtn_Click" Visible="false" />
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end m-2" ID="BackBtn" runat="server" Text="Geri..." CausesValidation="false" OnClick="BackBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end m-2" ID="EkstreListesiBtn" runat="server" Text="Ekstre Listesi" CausesValidation="false" OnClick="EkstreListesiBtn_Click" />
        </div>
    </div>
</div>
