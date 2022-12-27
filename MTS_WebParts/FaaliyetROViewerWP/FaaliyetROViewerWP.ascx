<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaaliyetROViewerWP.ascx.cs" Inherits="MTS_WebParts.FaaliyetROViewerWP.FaaliyetROViewerWP" %>

<link rel="stylesheet" href="/Style%20Library/tskgv/js/fullcalendar/main.css">
<script type="text/javascript" src="/Style%20Library/tskgv/js/fullcalendar/main.js"></script>
<script type="text/javascript" src="/Style%20Library/tskgv/js/fullcalendar/locales/tr.js"></script>



<style>
  body {
    /*margin-top: 40px;*/
    font-size: 14px;
    font-family: Arial, Helvetica Neue, Helvetica, sans-serif;
  }

  .fc-daygrid-dot-event .fc-event-title {
    white-space: break-spaces;
    word-break: break-word;
    font-weight:normal;
    border:solid;
    border-width:thin;
  }

  #calendar-wrap {
    /*margin-left: 200px;*/
  }

  #calendar {
      font-size:small;
      height:1116px;
      width:1600px;
/*    max-width: 1100px;
    margin: 0 auto;
    margin-left:220px; */
  }
  .title-wrap {
    white-space: normal !important;
    }
    .iptal-edildi {
        /*text-decoration: line-through !important;*/
            background-image: url("data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPScxMCcgaGVpZ2h0PScxMCc+CiAgPHJlY3Qgd2lkdGg9JzEwJyBoZWlnaHQ9JzEwJyBmaWxsPSd3aGl0ZScvPgogIDxwYXRoIGQ9J00tMSwxIGwyLC0yCiAgICAgICAgICAgTTAsMTAgbDEwLC0xMAogICAgICAgICAgIE05LDExIGwyLC0yJyBzdHJva2U9J2JsYWNrJyBzdHJva2Utd2lkdGg9JzEnLz4KPC9zdmc+Cg=="); 
            background-repeat: repeat;    
    }

  #globalWrapper * {
    z-index: auto;
  }
</style>
<script type="text/javascript">
    function OpenToplantiModal() {
        $("#ToplantiDetaylariModal").modal({ backdrop: true });
    }
    function OpenFaaliyetModal() {
        $("#FaaliyetDetaylariModal").modal({ backdrop: true });
    }
    function ToplantiDetaylariModal(toplantiId) {
        document.getElementById('<%= paramToplantiIdLbl.ClientID%>').value = toplantiId;
        document.getElementById('<%= ToplantiDetaylariBtn.ClientID%>').click();
    }
    function FaaliyetDetaylariModal(faaliyetId) {
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= FaaliyetDetaylariBtn.ClientID%>').click();
    }

</script>
<script type="text/javascript">
    function DoIt() {
        var element = document.getElementById('calendar');
        var fileName = $("h2").html();
        var opt = {
            margin: [0, 0],
            filename: fileName,
            enableLinks: false,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'in', format: 'A3', orientation: 'landscape' }
        };

        // New Promise-based usage:
        html2pdf().set(opt).from(element).save();

        // Old monolithic-style usage:
        //html2pdf(element, opt);
    }


</script>
<script src="/Style Library/tskgv/js/jspdf.js"></script>
<script src="/Style Library/tskgv/js/jspdf.plugin.addimage.js"></script>
<script src="/Style Library/tskgv/js/html2canvas.min.js"></script>
<script src="/Style Library/tskgv/js/html2pdf.bundle.min.js"></script>
<div class="col-xl" style="background-color: aliceblue;">
    <div class="form-group row">
        <div class="form-group col">
            <div class="checkbox">
                <label>
                    <asp:CheckBox ID="VakifIciKutlamaChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="VakifIciKutlamaChk_CheckedChanged" ToolTip="TC Kimlik numarası dolu olan bağışçıları listeye eklemek için işaretleyiniz." />
                    Doğum Günü / Evlenme Yıldönümü Kutlamalarını Göster (Vakıf İçi)
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <asp:CheckBox ID="VakifDisiKutlamaChk" runat="server" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="VakifDisiKutlamaChk_CheckedChanged" ToolTip="Doğum tarihi dolu olan bağışçıları listeye eklemek için işaretleyiniz." Enabled="False" />
                    Doğum Günü Kutlamalarını Göster (Vakıf Dışı)
                </label>
            </div>
        </div>
        <div class="form-group col">
            <div class="checkbox">
                <label>
                    <asp:CheckBox ID="ResmiTatilChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="ResmiTatilChk_CheckedChanged" ToolTip="Çıplak Mülkiyet bağışlayan bağışçıları listeye eklemek için işaretleyiniz." />
                    Resmi Talilleri Göster
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <asp:CheckBox ID="ToplantiChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="ToplantiChk_CheckedChanged" ToolTip="TYS Toplantilarını Göster" />
                    TYS Toplantılarını Göster
                </label>
            </div>
        </div>
        <div class="form-group col ">
            <a href="#" class="btn btn-info  m-2 float-right" id="downloadPDF" onclick="DoIt();">Takvimi PDF'e Aktar</a>
        </div>
    </div>
    <div id='wrap'>
        <div id='calendar-wrap'>
            <div id='calendar'></div>
        </div>
    </div>
    <div class='acik-tarihli'>
        <h4>Açık Tarihli Faaliyetler</h4>
        <div id="AcikTarihliRandevuListDiv" runat="server" clientidmode="Static">
        </div>
    </div>
    <div id="RandevuHiddenDiv" style="display: none">
        <input id="paramRandevuId" runat="server" type="text" />
        <input id="paramBasTar" runat="server" type="text" />
        <input id="paramBitTar" runat="server" type="text" />
        <input id="paramView" runat="server" type="text" />
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
<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal " id="FaaliyetDetaylariModal" role="dialog" style="z-index : 111111">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-body ">
                        <div class="HiddenDiv" style="display: none">
                            <asp:TextBox ID="paramFaaliyetIdLbl" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:LinkButton ID="FaaliyetDetaylariBtn" runat="server" CausesValidation="false" Text="Faaliyet bilgilerini modala doldur" OnClick="FaaliyetDetaylariBtn_Click" />
                        </div>
                        <div class="card">
                            <div class="card-header">
                                <h3>
                                    <asp:Label CssClass="font-weight-bold text-danger" ID="Label1" runat="server">Faaliyet Detayları</asp:Label>
                                    <asp:Label ID="FaaliyetIdLbl" runat="server"></asp:Label>
                                </h3>
                            </div>
                            <div class="card-body">
                                <div class="card-body p-0" id="Div2" runat="server">
                                    <div class="form-group">
                                        <asp:Table id="Table1" runat="server" CssClass="table table-striped table-bordered" width="100%">
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Faaliyet Konusu
                                                </asp:TableCell>
                                                <asp:TableCell  ID="FaaliyetKonusuCell"></asp:TableCell>
                                            </asp:TableRow>
                                             <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Başlangıç Zamanı
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetBaslangicZamaniCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Bitiş Zamanı
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetBitisZamaniCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Faaliyet Yeri
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetYeriCell"></asp:TableCell>
                                            </asp:TableRow>               
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Faaliyet Tipi
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetTipiCell"></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Faaliyet Amacı
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetAmaciCell"></asp:TableCell>
                                            </asp:TableRow>                       
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Katilimcilar
                                                </asp:TableCell>
                                                <asp:TableCell ID="KatilimcilarCell">
                                                <div class="card" id="KatilimciBilgileriDiv" runat="server" style="display: none">
                                                    <div class="card-body mb-5">
                                                        <div class="form-group">
                                                            <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Adı</th>
                                                                        <th>Soyadı</th>
                                                                        <th>Kurumu</th>
                                                                        <th>Katilimci int</th>
                                                                        <th>Katilimci Tipi</th>
                                                                    </tr>
                                                                </thead>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                                    </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                    Faaliyet Durumu
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetDurumuCell"></asp:TableCell>
                                            </asp:TableRow>                                            
                                            <asp:TableRow>
                                                <asp:TableCell CssClass="font-weight-bold">
                                                   Açıklama
                                                </asp:TableCell>
                                                <asp:TableCell ID="FaaliyetAciklamaCell"></asp:TableCell>
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