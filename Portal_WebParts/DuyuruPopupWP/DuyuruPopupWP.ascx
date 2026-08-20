<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuyuruPopupWP.ascx.cs" Inherits="Portal_WebParts.DuyuruPopupWP.DuyuruPopupWP" %>
<style>
    /* Bu kural sadece DuyuruPopupWP'nin kendi modallarını (duyuru-modal
       class'ıyla işaretlenmiş) hedefler. Daha önce ".modal", ".modal-dialog"
       ve ".fade" için sayfa genelinde (global) tanımlanmıştı; bu haliyle
       Bootstrap'in modal-backdrop elementini (class="modal-backdrop fade show")
       ve sayfadaki diğer web part'ların (örn. BildirimWP) modallarını da
       etkileyip opak beyaz bir overlay'e dönüştürüyordu. */
    .duyuru-modal,
    .duyuru-modal .modal-dialog {
    opacity: 1 !important;
    filter: alpha(opacity=100) !important;
    background: #fff;
}
</style>
<script type="text/javascript">
    function OpenDuyuruPopupModal(divId, duyuruId, duyuruGosterimId, clicked, chkDivId) {

        var duyuruCookie = "DuyuruGosterildi" + duyuruGosterimId;
        var duyuruGosterildi = cookieGosterildiMi(duyuruCookie);
        if (!duyuruGosterildi|| clicked) {
            if (clicked ) {
                document.getElementById(chkDivId).style.display = "none";
            }
            
            var myModal = new bootstrap.Modal(divId);
            myModal.show();
            // Açıldıktan sonra backdrop'ı kaldır
            setTimeout(() => {
                document.querySelectorAll(".modal-backdrop").forEach(el => el.remove());
            }, 100);
            if (IlkGosterilenDuyuruId == 0)
                IlkGosterilenDuyuruId = duyuruGosterimId;
        }

    }
    /**
        Okudum olarak işaretlenen duyuruları code behind içinde veri tabanında güncelleme ihtiyacı var, ama;
        document.getElementById(DuyuruyuOkudumBtn.ClientID).click(); satırı ilk popup gösteriminden sonra postback yaratıyor 
        ve bu sıradaki duyuru modalların düzgün biçimde işlemesini engelliyor.
        Çözüm olarak; 
            1. Her okudum butonuna basıldığında duyuruId'yi bir listeye ekle,
            2. En son görüntülenen modalda "okudum" veya "kapat" butonuna basıldığında, code behind içine bu listeyi gönderip, veritabanında insert/update işlemi yapma yoluna gittim. 
            3. Çözüm işe yaradı... :) 29/08/2019
    **/
    var okunanDuyuruListesi = '';
    var IlkGosterilenDuyuruId = 0;
    function DuyuruyuKapatClicked(duyuruId, duyuruGosterimId, divId, chkId,  startup) {
        var okudumChk = document.getElementById(chkId).checked;
        
        var duyuruCookie = "DuyuruGosterildi" + duyuruGosterimId;
        var expiresInSec = 30;//saniye
        var date = new Date();
        setCookieInSec(duyuruCookie, date, expiresInSec);
        if (startup) {//Tıklayarak açıldıya kapanırken okudum işlemi yapmasın
            if (okudumChk) {
                if (duyuruGosterimId > 0) {
                    okunanDuyuruListesi += duyuruGosterimId + ',';
                    setCookie(duyuruCookie, new Date());
                }
            }
        }
        

        if (IlkGosterilenDuyuruId == duyuruGosterimId) {
            document.getElementById('<%= paramOkunanDuyuruListesiLbl.ClientID%>').value = okunanDuyuruListesi;
            document.getElementById('<%= DuyuruyuOkudumBtn.ClientID%>').click();
        }
    }
    $(function () {
        $('.simple-marquee-container').SimpleMarquee({ speed: 5, direction: top });
    });
    function CallButtonClick(duyuruId) {
        document.getElementById('<%= paramDuyuruIdLbl.ClientID%>').value = duyuruId;
        document.getElementById('<%= OpenPopupBtn.ClientID%>').click();
    }

    //var seconds=30;
    //function countdown(){
    //    if (seconds > 0) {
    //        //document.getElementById("countdownTxt").innerHTML = ‘ (refresh in ‘ + seconds + ‘ seconds)';} //You do not need this one if you do not want to display the count down text
    //    }
    //    else {
    //        // document.getElementById("countdownTxt").innerHTML = ‘&nbsp;<strong>refreshing now…</strong>';
    //        location.reload(true);
    //    }
    //    seconds-=1;
    //    setTimeout("countdown()",1000);
    //}
    //countdown();

</script>
<style>
    .FontSmall {
        font-size   : small;
        color       : gray;
    } 
</style>
<div class="duyuruContent" style="border-radius: 10px">
    <div class="simple-marquee-container">
        <div class="marquee-sibling-start">
            Duyuru
        </div>
        <div class="marquee">
            <ul class="marquee-content-items" runat="server" id="DuyuruListUL">
            </ul>
        </div>
        <div class="marquee-sibling-end">
            DuyuruX
        </div>
    </div>
</div>
<asp:UpdatePanel ID="TableUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="ModalPlaceHolder" runat="server"></asp:PlaceHolder>

        <div style="display: none">
            <input id="paramDuyuruIdLbl" runat="server" type="text" />
            <input id="paramOkunanDuyuruListesiLbl" runat="server" type="text" />
            <asp:LinkButton ID="OpenPopupBtn" runat="server" OnClientClick="{return true;};" OnClick="OpenPopupBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="DuyuruyuOkudumBtn" runat="server" OnClientClick="{return true;};" OnClick="DuyuruyuOkudumBtn_Click"></asp:LinkButton>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

