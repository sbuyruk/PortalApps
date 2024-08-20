<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisHaritasiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBagisHaritasiWP.TasinmazBagisHaritasiWP" %>

<link rel="stylesheet" href="/Style%20Library/tskgv/css/normalize.css">
<link rel="stylesheet" type="text/css" href="/Style%20Library/tskgv/css/jquery.qtip.min.css">

<script type="text/javascript" src="/Style%20Library/tskgv/js/raphael-min.js"></script>
<script type="text/javascript" src="/Style%20Library/tskgv/js/paths.js"></script>
<script type="text/javascript" src="/Style%20Library/tskgv/js/turkiye.js?v=123"></script>
<script type="text/javascript" src="/Style%20Library/tskgv/js/jquery.qtip.min.js"></script>

<script type="text/javascript">
    $(function () {
        $("#map svg path").hover(
		  function () {
		      var id = $(this).attr("id");
		      $("#sehir").text(id.toUpperCase());
		  });
        $("#map svg path").click(function () {//tıklayınca
            var id = $(this).attr("id");
            var plaka = $(this).attr("county");
            $("#sehir").text(id.toUpperCase());
            document.getElementById('<%= paramLbl.ClientID%>').value = id;
            document.getElementById('<%= SelectedIlBtn.ClientID%>').click();
            //reloadPage(id);
            OpenIlInfoModal();
        });
        $("#ilInfoModal").draggable({
            handle: ".modal-header"
        });
        $(window).on('load', function () {

            if ($("#loaderMainContainer").length) {
                $("#loaderMainContainer").fadeOut("slow");
            }

        })
    });

    function OpenIlInfoModal() {
        $("#ilInfoModal").modal({ backdrop: false });
    }

</script>
<style type="text/css">
    html, body {
        height: 100%;
    }


    /*map*/
    body {
        background: #fff;
    }

    #map {
        width: 1050px;
        height: 600px;
        position: relative;
        margin: auto;
    }

        #map svg {
            position: relative;
            top: -80px !important;
        }

    svg > a {
        cursor: pointer;
        display: block;
    }

    #sehir {
        font-size: 25px;
        text-align: center;
        margin-top: 10px;
        color: #666;
    }

    .table {
        margin-left: auto;
        margin-right: auto;
        margin-bottom: -150px !important;
    }

    table.colors td {
        font-size: 18px;
        color: #666;
    }
</style>
<div class="container shadow">
    <div class="card">
        <div class="card-header">
            <h3>
                <asp:Label runat="server" class="h1 btn-outline-danger">Tasinmaz Bağış Haritası</asp:Label>
                <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            </h3>
            <div id="sehir" class="text-primary"></div>
        </div>
        <div class="card-body">

            <div class="table" id="MapDiv">
                <div id="map" style="left: 0px; top: -80px; overflow: hidden; position: relative; width: 1050px; height: 600px;">
                </div>
            </div>
        </div>
        <div class="card-footer">
            <div>
                <table class="colors">
                    <tr>
                        <td style="background-color: tomato; width: 5%;"></td>
                        <td style="width: 20%">Ankara Bölge Md. </td>

                        <td style="background-color: mediumseagreen; width: 5%;"></td>
                        <td style="width: 20%">İstanbul Bölge Md. </td>

                        <td style="background-color: orange; width: 5%;"></td>
                        <td style="width: 20%">İzmir Bölge Md. </td>

                        <td style="background-color: dodgerblue; width: 5%;"></td>
                        <td style="width: 20%">Mersin Bölge Md. </td>
                    </tr>
                </table>
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
    <!-- Modal Message-->
    <!-- Modal -->
<div class="modal" id="ilInfoModal" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content" style="width: 700px;">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-body" >
                            <div style="display: none">
                                <input id="paramLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                                <asp:LinkButton ID="SelectedIlBtn" CssClass="btn btn-danger" ClientIDMode="Static" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="SelectedIlBtn_Click" />
                            </div>
                            <div>
                                <div class="mt-3">
                                    <h4>
                                        <asp:Label ID="TasinmazTitleLbl" class="text-primary " runat="server" Text="..."></asp:Label>
                                    </h4>
                                </div>
                                <div id="TasinmazTableContainer" class="border" style="height: 200px; overflow: auto;">
                                    <asp:Table ID="TasinmazTable" runat="server" class="table table-bordered table-hover table-sm">
                                        <asp:TableHeaderRow ID="TasinmazTableHeader">
                                        </asp:TableHeaderRow>
                                    </asp:Table>
                                </div>
                            </div>
                            <div>
                                <div class="alignCenter">
                                    <h4>
                                        <asp:Label ID="BagisciTitleLbl" class="text-primary " runat="server" Text="..."></asp:Label>
                                    </h4>
                                </div>
                                <div id="TableContainer" class="border" style="height: 200px; overflow: auto;">
                                    <asp:Table ID="BagisciTable" runat="server" class="table table-bordered table-hover table-sm">
                                        <asp:TableHeaderRow  ID="BagisciTableHeader">                                           
                                        </asp:TableHeaderRow>
                                    </asp:Table>
                                </div>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="BagiscilarBtn" class="btn" runat="server" Text="Bağışçılar" OnClick="BagiscilarBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="BagiscilarBtn" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="SelectedIlBtn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</div>
