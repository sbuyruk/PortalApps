<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IzinTalepListesiWP.ascx.cs" Inherits="IKYS_WebParts.IzinTalepListesiWP.IzinTalepListesiWP" %>
<style>
    .kayitlara-islendi {
        background-color: lightgrey !important;
        color: black !important;
    }
    .reddedildi {
        background-color: lightpink !important;
        color: black !important;
    }
</style>
<script>
    function OpenModalOnay() {
        $("#ModalOnayDiv").modal({ backdrop: false });
    }
    //ekrandan secilen onayla vb butonun serverside'da oncliclkini calistirsin
    function CallButtonClick(onay) {

        var button = document.getElementById('<%= OnaylaBtn.ClientID%>');
        if (onay == "onay") {
            button = document.getElementById('<%= OnaylaBtn.ClientID%>');
        }
        if (onay == "kabul") {
            button = document.getElementById('<%= KabuletBtn.ClientID%>');
        }
        if (onay == "reddet") {
            button = document.getElementById('<%= ReddetBtn.ClientID%>');
        }
        if (button != null) {
            button.click();
        }

    }
    function OpenModal(onay, izinTalepId, personelId) {
        document.getElementById('<%= paramIzinTalepIdLbl.ClientID%>').value = izinTalepId;
        document.getElementById('<%= paramPersonelIdLbl.ClientID%>').value = personelId;
        document.getElementById('<%= paramOnayIdLbl.ClientID%>').value = onay;

        OpenModalOnay();
        document.getElementById('<%= IzinHareketleriBtn.ClientID%>').click();
    }
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3>
                <asp:Label CssClass="col-form-label  btn-outline-primary" runat="server" Text="İzin Talepleri (Mazeret İzinleri Hariç)"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="card-body p-0">
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                        <thead>
                            <tr>
                                <th>Başlangıç (Hidden)</th>
                                <th>Adı Soyadı</th>
                                <th>İzin Tipi</th>
                                <th>İzin Başlangıcı</th>
                                <th>İzin Bitişi</th>
                                <th>Süre</th>
                                <th>Birim</th>
                                <th>Durumu</th>
                                <th>Düzenle</th>
                                <th>Kayıt/Kontrol/Red</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div style="display: none">
                    <input id="paramIzinTalepIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                    <input id="paramPersonelIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                    <input id="paramOnayIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                    <asp:LinkButton ID="KabuletBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="KabulBtn_Click" />
                    <asp:LinkButton ID="OnaylaBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="OnaylaBtn_Click" />
                    <asp:LinkButton ID="ReddetBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ReddetBtn_Click" />
                    <asp:LinkButton ID="YazdirBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="YazdirBtn_Click" />
                </div>
            </div>
        </div>
        <div class="card-footer">
        </div>

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
<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="IzinHareketleriBtn" CssClass="btn btn-info" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="IzinHareketleriBtn_Click" />
                        </div>
                        <div>
                            <div class="text-center">
                                <h3>
                                    <asp:Label ID="PersonelAdiLbl" class="text-primary " runat="server" Text="..."></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <asp:Table ID="IzinBilgileriTable" runat="server" class="table table-sm small">
                                </asp:Table>
                            </div>
                            <div class="card-body">
                                <asp:Table ID="IzinHareketTable" runat="server" class="table table-sm small">
                                </asp:Table>
                            </div>
                            <div class="card-body">
                                
                                <label class="col-form-label">İzin Onay/Kabul/Red Açıklaması</label>
                                <asp:TextBox ID="AciklamaTxt" TextMode="MultiLine" Rows="3" runat="server" class="form-control" type="text" />
                                
                                <asp:Label ID="IzinTalebiLbl" runat="server" CssClass="col-form-label text-danger"></asp:Label>
                                <div class="form-group">
                                    <asp:Label ID="AdresLbl" TextMode="MultiLine" Rows="3" runat="server" class="col-form-label" type="text" ForeColor="Blue" />
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="modal-footer">
                        <button id="OnaylaModalBtn" runat="server" class="btn btn-success" onclick="CallButtonClick('onay')" visible="False">Onayla</button>
                        <button id="KabuletModalBtn" runat="server" class="btn btn-info" onclick="CallButtonClick('kabul')" visible="False">Kabul Et</button>
                        <button id="ReddetModalBtn" runat="server" class="btn btn-danger" onclick="CallButtonClick('reddet')">Reddet</button>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
