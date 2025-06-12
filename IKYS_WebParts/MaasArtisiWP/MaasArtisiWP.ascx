<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaasArtisiWP.ascx.cs" Inherits="IKYS_WebParts.MaasArtisiWP.MaasArtisiWP" %>

<style>
    .modal {
        z-index: 1050 !important;
    }

    .modal-backdrop {
        z-index: 1040 !important;
    }

    .modal-content {
        z-index: 1060 !important;
    }
</style>

<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>

<div class="container w-25">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Maaş Artışı"></asp:Label>
                <asp:Label CssClass="form-label fw-semibold text-white" ID="GorevOnayIdLbl" runat="server"></asp:Label>
            </h3>
        </div>

        <div class="card-body">
            <div class="row">
                <div class="col">
                    <div class="form-group m-0">
                        <label class="form-label fw-semibold" for="BaslangicTarihiTxt">Başlangıç Tarihi</label>
                        <asp:TextBox
                            ID="BaslangicTarihiTxt"
                            runat="server"
                            CssClass="form-control"
                            ClientIDMode="Static"
                            placeholder="Başlangıç Tarihi"
                            Enabled="False" />
                    </div>

                    <div class="form-group m-0">
                        <label class="form-label fw-semibold" for="ArtisYuzdesiTxt">Artış Yüzdesi</label>
                        <%-- Alternatif kullanım (Yorum satırında bırakıldı) --%>
                        <%-- 
                        <asp:TextBox ID="ArtisYuzdesiTxt" runat="server" CssClass="form-control input-money" TextMode="Number" min="0" ToolTip="Artış Yüzdesi"></asp:TextBox> 
                        --%>
                        <asp:TextBox
                            ID="ArtisYuzdesiTxt"
                            runat="server"
                            CssClass="form-control input-money"
                            ToolTip="Artış Yüzdesi" />
                    </div>
                </div>

                <div class="col">
                    <div class="form-group m-0">
                        <label class="form-label fw-semibold" for="BitisTarihiTxt">Bitiş Tarihi</label>
                        <asp:TextBox
                            ID="BitisTarihiTxt"
                            runat="server"
                            CssClass="form-control input-date DateTimePickerV1"
                            ClientIDMode="Static"
                            placeholder="Bitiş Tarihi" />
                    </div>

                    <div class="form-group m-0">
                        <label class="form-label fw-semibold" for="AgiTxt">İlave Ödeme (AGİ)</label>
                        <asp:TextBox
                            ID="AgiTxt"
                            runat="server"
                            CssClass="form-control input-money"
                            ToolTip="İlave Ödeme (AGİ)" />
                    </div>
                </div>
            </div>
        </div>

        <div class="card-footer text-center">
            <asp:LinkButton
                ID="KaydetBtn"
                CssClass="btn btn-success"
                runat="server"
                Text="Artış Yap Ve Tabloları Kaydet"
                OnClick="KaydetBtn_Click"
                />
        </div>
    </div>

    <!-- Modal -->
    <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content" style="width: 550px;">
                <div class="modal-body">
                    <div class="text-center">
                        <h3>
                            <asp:Label
                                ID="MessageTitleLbl"
                                CssClass="form-label fw-semibold"
                                runat="server"
                                Text="Lütfen Dikkat: Maaş Artışı Yapılacak" />
                        </h3>
                    </div>
                    <div class="card-body">
                        <asp:Label
                            ID="MessageTextLbl"
                            CssClass="form-label fw-semibold"
                            runat="server"
                            Text="Maaş Artışını Uygulamak İstiyor musunuz?" />
                    </div>
                </div>

                <div class="text-center">
                    <label class="form-label text-danger">
                        Tablolar oluşturulduktan sonra bu işlem geri alınamaz.
                    </label>
                </div>

                <div class="modal-footer text-center">
                    <asp:LinkButton
                        CssClass="btn btn-danger"
                        ID="DeleteNowBtn"
                        runat="server"
                        CausesValidation="false"
                        Text="Görev Onayı Sil"
                        OnClientClick="{return true;};"
                        OnClick="DeleteNowBtn_Click"
                        Visible="false" />

                    <asp:LinkButton
                        CssClass="btn btn-success"
                        ID="KaydetNowBtn"
                        runat="server"
                        CausesValidation="false"
                        Text="Artış Yap Ve Tabloları Kaydet"
                        OnClientClick="{return true;};"
                        OnClick="KaydetNowBtn_Click" />

                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                </div>
            </div>
        </div>
    </div>
</div>




<%--<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaasArtisiWP.ascx.cs" Inherits="IKYS_WebParts.MaasArtisiWP.MaasArtisiWP" %>
<style>
    .modal {
        z-index: 1050 !important;
    }

    .modal-backdrop {
        z-index: 1040 !important;
    }

    .modal-content {
        z-index: 1060 !important;
    }
</style>

<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }

</script>
<div class="container w-25">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Maaş Artışı"></asp:Label>
                <asp:Label CssClass="form-label fw-semibold text-white" ID="GorevOnayIdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col">
                    <div class="form-group m-0 ">
                        <label class="form-label fw-semibold" for="BaslangicTarihiTxt">Başlangıç Tarihi</label>
                        <asp:TextBox ID="BaslangicTarihiTxt" runat="server" CssClass="form-control"
                            ClientIDMode="Static" placeholder="Başlangıç Tarihi" Enabled="False"></asp:TextBox>
                    </div>
                    <div class="form-group m-0 ">
                        <label class="form-label fw-semibold" for="ArtisYuzdesiTxt">Artış Yüzdesi</label>
                        <asp:TextBox
                            ID="ArtisYuzdesiTxt"
                            runat="server"
                            CssClass="form-control input-money"
                            ToolTip="Artış Yüzdesi">
                        </asp:TextBox>

                    </div>
                </div>
                <div class="col">
                    <div class="form-group m-0 ">
                        <label class="form-label fw-semibold" for="BitisTarihiTxt">Bitiş Tarihi</label>
                        <asp:TextBox ID="BitisTarihiTxt" runat="server" CssClass="form-control input-date DateTimePickerV1"
                            ClientIDMode="Static" placeholder="Bitiş Tarihi"></asp:TextBox>
                    </div>
                    <div class="form-group m-0 ">
                        <label class="form-label fw-semibold" for="AgiTxt">İlave Ödeme (AGİ)</label>
                        <asp:TextBox ID="AgiTxt" runat="server" class="form-control input-money" ToolTip="İlave Ödeme (AGİ)"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-success col-2 me-5" runat="server" Text="Artış Yap Ve Tabloları Kaydet" OnClick="KaydetBtn_Click" Visible="false" />
        </div>
    </div>
    <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">

                <div class="modal-body">

                    <div class="text-center">
                        <h3>
                            <asp:Label ID="MessageTitleLbl" class="form-label fw-semibold " runat="server" Text="Lütfen Dikkat: Maaş Artışı Yapılacak"></asp:Label></h3>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="MessageTextLbl" CssClass="form-label fw-semibold " runat="server" Text="Maaş Artışını Uygulamak İstiyor musunuz?"></asp:Label>
                    </div>

                </div>
                <div class="text-center">
                    <label class="form-label text-danger">Tablolar oluşturulduktan sonra bu işlem geri alınamaz.</label>
                </div>
                <div class="modal-footer text-center">

                    <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Görev Onayı Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-success col-3" ID="KaydetNowBtn" runat="server" CausesValidation="false" Text="Artış Yap Ve Tabloları Kaydet" OnClientClick="{return true;};" OnClick="KaydetNowBtn_Click" />
                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                </div>
            </div>
        </div>
    </div>




</div>--%>
