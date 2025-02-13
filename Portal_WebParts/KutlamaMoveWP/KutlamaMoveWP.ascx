<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KutlamaMoveWP.ascx.cs" Inherits="Portal_WebParts.KutlamaMoveWP.KutlamaMoveWP" %>
<style>
    /*Arka arkaya açılan popup pencerelerde modal backdrop farklı gelsin diye*/

    .modal.fade {
        background: rgba(0,0,0,0.5);
    }
</style>
<script type="text/javascript">
    /* SET COOKIE */
    function OpenKutlamaPopup(clicked, opener) {

        // ilk açılışta checkbox  checked= true gelsin
        document.getElementById('<%= KutlamayiOkudumChk.ClientID%>').checked = true;

        //cookie işlemleri, gösterildi olarak cookie at
        var aCookie = "KutlamaGosterildi";
        var kutlamaGosterildi = cookieGosterildiMi(aCookie);
        if (!kutlamaGosterildi || clicked) {
            if (clicked || kutlamaGosterildi) { // tıklandıysa veya daha önce gösterildiyse okudumDiv görünmesin
                kutlamaIconClicked = clicked;
                document.getElementById('<%= KutlamayiOkudumDiv.ClientID%>').style.display = "none";
            }
            $("#KutlamaPopupDiv").modal(
                {
                    backdrop: false,
                    keyboard: true
                });
        }

    }
    var kutlamaIconClicked = false;
    $("#KutlamaPopupDiv").draggable({
        handle: "modal-header"
    });

    function KutlamayiKapatClicked() {
        var okudumChk = document.getElementById('<%= KutlamayiOkudumChk.ClientID%>').checked;
        var kutlamaCookie = "KutlamaGosterildi";
        var expiresInSec = 30;//dakika
        var date = new Date();
        setCookieInSec(kutlamaCookie, new Date(), expiresInSec);
        if (!kutlamaIconClicked) {
            if (okudumChk) {
                setCookie(kutlamaCookie, new Date());
            }
        }
    }
</script>
<asp:HiddenField ID="hdnBirthday" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnMarriage" runat="server" ClientIDMode="Static" />
<div class="part-item celebration-container">
    <a id="openerKutlama" class="celebration_a cf" onclick=" OpenKutlamaPopup(true,'clicked');">
        <span class="icon">
            <img src="./../OrtakResimler/kutlama.png" alt="" />
        </span>
        <span class="wr">Kutlama​
        </span>
    </a>
    <br />
</div>

<div id="KutlamaPopupDiv" class="modal" role="dialog">
    <div class="modal-dialog shadow " role="document">

        <%-- Kutlama Başlangıç--%>
        <div class="modal-content">
            <div class="modal-header">
            </div>
            <div class="modal-body">
                <div class="text-danger font-weight-bold text-center" id="YokDiv" runat="server" style="display: block">
                    Bugün doğan veya evlilik yıldönümü olan personelimiz bulunmamaktadır.
                </div>
                <div class="card" id="DogumGunuDiv" runat="server" style="display: none">
                    <div class="card-header">
                        <div class="row">
                            <div class="col-8">
                                <h5>
                                    <asp:Label CssClass="col-form-label text-success font-weight-bold" runat="server" Text="Bugün Doğanlar"></asp:Label>
                                </h5>
                            </div>
                            <div class="col-4">
                                <img class="tr" src="./../OrtakResimler/balon.gif" height="60" />
                            </div>
                        </div>

                    </div>
                    <div class="card-body">
                        <asp:Repeater runat="server" ID="DogumRepeater">
                            <ItemTemplate>
                                <div class="card alert-success  mb-2" style="border-radius: 15px">
                                    <div class="row">
                                        <div class="col-md-4">
                                            <img class="rounded-circle border" id="DisplayImage" src="<%#Eval("ResimPath")%>" height="80" />
                                        </div>
                                        <div class="col-md-8">
                                            <label style="font-weight: bold; text-decoration: underline; font-style: italic"><%#Eval("Adi") %></label>
                                            <label><%#Eval("Metin") %></label>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="card mt-1" id="EvlilikDiv" runat="server" style="display: none">
                    <div class="card-header">
                        <div class="row">
                            <div class="col-8">
                                <h5>
                                    <asp:Label CssClass="col-form-label text-danger font-weight-bold" runat="server" Text="Bugün Evlenenler"></asp:Label>
                                </h5>
                            </div>
                            <div class="col-4">
                                <img class="tr" src="/../OrtakResimler/nazar.gif" height="60" />
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <asp:Repeater runat="server" ID="EvlilikRepeater">
                            <ItemTemplate>
                                <div class="card alert-danger mb-2" style="border-radius: 15px">
                                    <div class="row">
                                        <div class="col-md-4">
                                            <img class="rounded-circle border" id="DisplayImage" src="<%#Eval("ResimPath")%>" height="80" />
                                        </div>
                                        <div class="col-md-8">
                                            <label style="font-weight: bold; text-decoration: underline; font-style: italic"><%#Eval("Adi") %></label>
                                            <label><%#Eval("Metin") %></label>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="form-group float-end kutlamaGenelMudur" id="GenelMudurDiv" runat="server" style="display: none">
                    <div class="form-group m-0">
                        <asp:Label class="form-control col-form-label font-weight-bold border-0" ID="pGenelMudur" runat="server"></asp:Label>
                    </div>
                    <div class="form-group m-0">
                        <asp:Label class="form-control col-form-label font-weight-bold border-0 pt-0" ID="pGenelMudurUnvan" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="form-group" id="KutlamayiOkudumDiv" runat="server" style="display: none">
                    <asp:CheckBox ID="KutlamayiOkudumChk" runat="server" Text="Okudum, bir daha gösterme" ForeColor="Gray" Font-Size="Small" />
                </div>

                <button type="button" class="btn btn-outline-secondary " data-bs-dismiss="modal" onclick="KutlamayiKapatClicked();">Kapat</button>
            </div>
        </div>
    </div>
</div>
