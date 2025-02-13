<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GunlukYemekListesiMoveWP.ascx.cs" Inherits="Portal_WebParts.GunlukYemekListesiMoveWP.GunlukYemekListesiMoveWP" %>
<script type="text/javascript">
    function OpenYemekPopup(clicked) {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('YemekPopupDiv'));
        myModalInstance.show();
    }

</script>

<div class="meat-menu-container">
    <a id="openerYemek1" class="meat-menu cf" onclick=" OpenYemekPopup(true);">
        <span class="icon">
            <img src="./../OrtakResimler/meat-icon.png" alt="">
        </span>
        <span class="wr">Günlük Yemek Listesi</span>
    </a>
</div>

<div class="modal " id="YemekPopupDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body ">
                <div class="card">
                    <div class="card-header row">
                        <div class="col-4">
                            <img class="tr" id="YemekImg" src="../OrtakResimler/yemek.gif" height="60" runat="server" />
                        </div>
                        <div class="col-4">
                            <img class="tr" id="YemekImg1" src="../OrtakResimler/yemek1.gif" height="60" runat="server" />
                        </div>
                        <div class="col-4">
                            <img class="tr" id="YemekImg2" src="../OrtakResimler/yemek2.gif" height="60" runat="server" />
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="text-danger font-weight-bold" id="YokDiv" runat="server" style="display: none">
                            <asp:Label ID="YokLbl" runat="server" Text="Yemek Listesi bulunmamaktadır."></asp:Label>
                        </div>
                        <div class="table loader" id="TabloDiv" runat="server" style="display: block">
                            <asp:Table ID="YemekTable" runat="server" class="table table-striped table-bordered">
                            </asp:Table>
                        </div>
                        <div id="Div1" runat="server" style="display: none">
                            <asp:Button ID="YemekListesiBtn" runat="server" Text="" OnClick="YemekListesiBtn_Click"></asp:Button>
                        </div>
                    </div>
                    <div class="card-footer">
                        <asp:Label ID="ToplamLbl" runat="server" Text="" Font-Bold="True"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>

    </div>
</div>
