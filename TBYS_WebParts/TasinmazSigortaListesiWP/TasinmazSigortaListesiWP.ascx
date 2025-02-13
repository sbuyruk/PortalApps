<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazSigortaListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazSigortaListesiWP.TasinmazSigortaListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    .renkli {
        color: red;
        font-weight:bold;
        background-color:beige;
    }
</style>


<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>

<div class="container col-xl">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Sigortaları"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body" id="MainCardDiv" runat="server">
                    <asp:UpdatePanel ID="upPanel" runat="server">
                        <ContentTemplate>
                            <div class="form-group border">
                                <div class="form-group">
                                    <div class="row">
                                    <div class="form-group col-2">
                                        <label class="col-form-label " for="SigortaCinsiDDL">Sigorta Cinsi</label>
                                        <asp:DropDownList ID="SigortaCinsiDDL" runat="server" class="form-control " Height="34px" OnSelectedIndexChanged="SigortaCinsiDDL_SelectedIndexChanged" AutoPostBack="True" />
                                        <div class="checkbox pt-3">
                                            <label>
                                                <asp:CheckBox ID="VadesiGelenlerChk" CssClass="font-weight-bold text-danger" runat="server" Checked="false" ToolTip="Bir Ay İçinde Sigortası Bitecek Olanlar" OnCheckedChanged="VadesiGelenlerChk_CheckedChanged" AutoPostBack="True" ForeColor="Red" TextAlign="Left" />
                                                Sadece Vadesi Gelenleri Göster 
                                            </label>
                                        </div>
                                    </div>
                                    <div class="col">
                                        <div class="form-group">
                                            <asp:CheckBox ID="DepremChk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 1. Deprem " Checked="false" TextAlign="Right" ToolTip="Deprem" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" />
                                        </div>
                                        <div class="form-group">
                                            <asp:CheckBox ID="YanginChk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 2. Yangın, yıldırım, infilak ... " Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Yangın, yıldırım, infilak, sel, su baskını, dahili su, fırtına, yer kayması, duman, cam kırılması, kar ağırlığı, kara-hava taşıtları çarpması, yangın mali sorumluluğu, grev, lokavt, kargaşa, halk hareketleri, kötü niyetli hareketler, terör, kira kaybı ve sabit tesisat (3.000 TL)." />
                                        </div>
                                    </div>
                                    <div class="col ">
                                        <div class="form-group">
                                            <asp:CheckBox ID="Makine100000Chk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 3. Makine-Tesisat (100.000 TL) ... " Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Makine-Tesisat (100.000 TL), Demirbaş (100.000 TL), Elektronik Cihaz (350.000TL), Nakit Para ve Kıymetli Evrak (20.000 TL), Taşınan Para Hırsızlık (20.000 TL), Emniyeti Suistimal (20.000 TL),  Kasa (10.000 TL)" />
                                        </div>
                                        <div class="form-group">
                                            <asp:CheckBox ID="Makine5000Chk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 4.Makine-Tesisat (5.000 TL) ... " Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Makine-Tesisat (5.000 TL), Demirbaş (10.000 TL), Elektronik Cihaz (10.000TL), Nakit Para ve Kıymetli Evrak (2.000 TL), Taşınan Para Hırsızlık (2.000 TL). Emniyeti Suistimal (2.000 TL)" />
                                        </div>
                                        
                                    </div>
                                    <div class="col ">
                                        <div class="form-group">
                                            <asp:CheckBox ID="JeneratorChk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 5. Jeneratör (30.000 TL) " Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Jeneratör (30.000 TL)" />
                                        </div>
                                        <div class="form-group">
                                            <asp:CheckBox ID="AsansorChk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 6. Asansör (50.000 TL) " Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Asansör (50.000 TL) " />
                                        </div>
                                        <div class="form-group">
                                            <asp:CheckBox ID="KazanChk" CssClass="form-control btn btn-light m-1 text-left " runat="server" Text=". 7. Kazan Dairesi Ekipmanı ..." Checked="false" TextAlign="Right" OnCheckedChanged="DepremChk_CheckedChanged" AutoPostBack="True" ToolTip="Kazan Dairesi Ekipmanı (30.000 TL)" />
                                        </div>
                                    </div>
                                </div>
                                </div>
                                
                                <div class="form-group border">
                                    <div class="form-group">
                                        <table id="CustomDataTable" class="table table-hover row-border" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>Bölge</th>
                                                    <th>Sigorta Cinsi</th>
                                                    <th>Adres Kodu</th>
                                                    <th>Poliçe No</th>
                                                    <th>Kullanım Şekli</th>
                                                    <th>Taşınmazın Adresi</th>
                                                    <th>Teminatlar</th>
                                                    <th>Sig.Bit.Tar.</th>
                                                    <th>Poliçe</th>
                                                    <th>Taşınmaz Kartı</th>
                                                    <th>Düzenle</th>
                                                </tr>
                                            </thead>
                                        </table>
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
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
