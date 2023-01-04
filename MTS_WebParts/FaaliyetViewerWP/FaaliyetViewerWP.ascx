<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaaliyetViewerWP.ascx.cs" Inherits="MTS_WebParts.FaaliyetViewerWP.FaaliyetViewerWP" %>

<script type="text/javascript" src="/Style%20Library/lib/fullcalendar/index.global.js"></script>
<script type="text/javascript" src="/Style%20Library/tskgv/js/fullcalendar/locales/tr.js"></script>
<style>
    <%-- scroll için --%>
    #AcikTarihliRandevuListDiv {
      background-color: lightblue;
      height: 550px;
      width: auto;
      overflow-y: scroll;
    }
  body {
    /*margin-top: 40px;*/
    font-size: 14px;
    font-family: Arial, Helvetica Neue, Helvetica, sans-serif;
  }

  #external-events {
    position: fixed;
/*    left: 20px;
    top: 20px;*/
    width: 200px;
    padding: 0;
    border: 1px solid #ccc;
    background: #eee;
    text-align: left;
  }

  #external-events h4 {
    font-size: 16px;
    margin-top: 0;
    padding-top: 1em;
  }

  #external-events .fc-event {
    margin: 3px 0;
    cursor: move;
  }

  #external-events p {
    margin: 1.5em 0;
    font-size: 11px;
    color: #666;
  }

  #external-events p input {
    margin: 0;
    vertical-align: middle;
  }
  #calendar {
/*    max-width: 1100px;
    margin: 0 auto;*/
    margin-left:220px;
  }
  .title-wrap {
    white-space: normal !important;
  }
    .iptal-edildi {
        /*text-decoration: line-through !important;*/
          background-image: url("data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPScxMCcgaGVpZ2h0PScxMCc+CiAgPHJlY3Qgd2lkdGg9JzEwJyBoZWlnaHQ9JzEwJyBmaWxsPSd3aGl0ZScvPgogIDxwYXRoIGQ9J00tMSwxIGwyLC0yCiAgICAgICAgICAgTTAsMTAgbDEwLC0xMAogICAgICAgICAgIE05LDExIGwyLC0yJyBzdHJva2U9J2JsYWNrJyBzdHJva2Utd2lkdGg9JzEnLz4KPC9zdmc+Cg=="); 
          background-repeat: repeat;    }

  #globalWrapper * {
    z-index: auto;
  }

</style>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenToplantiModal() {
        $("#ToplantiDetaylariModal").modal({ backdrop: true });
    }
    function ToplantiDetaylariModal(toplantiId) {
        document.getElementById('<%= paramToplantiIdLbl.ClientID%>').value = toplantiId;
        document.getElementById('<%= ToplantiDetaylariBtn.ClientID%>').click();
    }
    function RandevuKaydet(randevuId, basTar, endTar, newView) {
        document.getElementById('<%= paramRandevuId.ClientID%>').value = randevuId;
        document.getElementById('<%= paramBasTar.ClientID%>').value = basTar;
        document.getElementById('<%= paramBitTar.ClientID%>').value = endTar;
        document.getElementById('<%= paramView.ClientID%>').value = newView;
        document.getElementById('<%= RandevuKaydetNowBtn.ClientID%>').click();
    }
</script>
<script type="text/javascript">

    function ExportToExcel() {
        window.open('data:application/vnd.ms-excel,' + encodeURIComponent($('div[id=calendar]').html()));
        e.preventDefault();
    }
    function DoIt () {
        var element = document.getElementById('calendar');
        var fileName = $("h2").html();
        var opt = {
            margin: [0,-1,0,0],
            filename: fileName,
            enableLinks: false,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'A3', orientation: 'landscape' }
        };

        // New Promise-based usage:
        html2pdf().set(opt).from(element).save();
    }  

</script>

<script type="text/javascript">
    
    function fnExcelReport() {
        var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
        var textRange; var j = 0;
        tab = document.getElementsByClassName('fc-scrollgrid')[0];

        for (j = 0; j < tab.rows.length; j++) {
            tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
            //tab_text=tab_text+"</tr>";
        }

        tab_text = tab_text + "</table>";
        tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
        tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
        tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");

        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
        {
            txtArea1.document.open("txt/html", "replace");
            txtArea1.document.write(tab_text);
            txtArea1.document.close();
            txtArea1.focus();
            sa = txtArea1.document.execCommand("SaveAs", true, "Say Thanks to Sumit.xls");
        }
        else                 //other browser not tested on IE 11
            sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));

        return (sa);
    }
</script>
<script src="/Style Library/tskgv/js/jspdf.js"></script>
<script src="/Style Library/tskgv/js/jspdf.plugin.addimage.js"></script>
<script src="/Style Library/tskgv/js/html2canvas.min.js"></script>
<script src="/Style Library/tskgv/js/html2pdf.bundle.min.js"></script>

<div class="col-xl">

    <div id='wrap'>
     
        <div id='external-events'>
            <div class="form-group" >
                <a href="#" class="btn btn-info" id="downloadPDF" onclick="DoIt();">Takvimi PDF'e Aktar</a>
                <a href="#" id="btnExport" onclick="fnExcelReport();"> EXPORT </a>
                <iframe id="txtArea1" style="display:none"></iframe>
            </div>
            <h4>Açık Tarihli Faaliyetler</h4>
            <div id="AcikTarihliRandevuListDiv" runat="server" ClientIDMode="Static">

            </div>
        </div>
        <div id="calendar-wrap">
            
            <div id="calendar"></div>
        </div>
    </div>
    <div id="RandevuHiddenDiv" style="display: none">
        <input id="paramRandevuId" runat="server" type="text" />
        <input id="paramBasTar" runat="server" type="text" />
        <input id="paramBitTar" runat="server" type="text" />
        <input id="paramView" runat="server" type="text" />
        <asp:LinkButton ID="RandevuKaydetNowBtn" runat="server" CausesValidation="false" Text="Faaliyete Ekle" OnClientClick="{return true;};" OnClick="RandevuKaydetNowBtn_Click" />
    </div>
</div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal " id="ToplantiDetaylariModal" role="dialog" style="z-index : 111111">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-body ">
                        <div class="HiddenDiv" style="display: none">
                            <asp:TextBox ID="paramToplantiIdLbl" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:LinkButton ID="ToplantiDetaylariBtn" runat="server" CausesValidation="false" Text="Toplantı bilgilerini modala doldur" OnClick="ToplantiDetaylariBtn_Click" />
                        </div>
                        <div class="card">
                            <div class="card-header">
                                <h3>
                                    <asp:Label CssClass="font-weight-bold text-danger" ID="ToplantiDetaylariHeaderLbl" runat="server">Toplantı Detayları</asp:Label>
                                    <asp:Label ID="IdLbl" runat="server"></asp:Label>
                                </h3>
                            </div>
                            <div class="card-body">
                                <div class="card-body p-0" id="Div1" runat="server">
                                    <div class="form-group">
                                        <asp:Table id="ToplantiDetaylariTable" runat="server" CssClass="table table-striped table-bordered" width="100%">
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Toplantı Konusu
                                                </asp:TableCell>
                                                <asp:TableCell  ID="ToplantiKonusuCell"></asp:TableCell>
                                            </asp:TableRow>
                                             <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Başlangıç Zamanı
                                                </asp:TableCell>
                                                <asp:TableCell ID="BaslangicZamaniCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Bitiş Zamanı
                                                </asp:TableCell>
                                                <asp:TableCell ID="BitisZamaniCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Toplantı Yeri
                                                </asp:TableCell>
                                                <asp:TableCell ID="ToplantiYeriCell"></asp:TableCell>
                                            </asp:TableRow>               
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Toplantı Yetkilisi
                                                </asp:TableCell>
                                                <asp:TableCell ID="ToplantiYetkilisiCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Koordinatör
                                                </asp:TableCell>
                                                <asp:TableCell ID="KoordinatorCell"></asp:TableCell>
                                            </asp:TableRow>                       
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    İç Katılımcılar
                                                </asp:TableCell>
                                                <asp:TableCell ID="IcKatilimcilarCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Dış Katılımcılar
                                                </asp:TableCell>
                                                <asp:TableCell ID="DisKatilimcilarCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Bilgi
                                                </asp:TableCell>
                                                <asp:TableCell ID="BilgiCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                   Çevrim İçi
                                                </asp:TableCell>
                                                <asp:TableCell ID="CevrimIciCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                   İkram Onayı
                                                </asp:TableCell>
                                                <asp:TableCell ID="IkramOnayiCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                   İkram Malzemesi
                                                </asp:TableCell>
                                                <asp:TableCell ID="IkramMalzemesiCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                   Açıklama
                                                </asp:TableCell>
                                                <asp:TableCell ID="AciklamaCell"></asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
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

</asp:UpdatePanel>