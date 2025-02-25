<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KPSSorguListesiWP.ascx.cs" Inherits="TBYS_WebParts.KPSSorguListesiWP.KPSSorguListesiWP" %>

<script type="text/javascript">
    function CikarButtonClick(no) {
        document.getElementById('<%= paramTcKimlikNo.ClientID%>').value = no;
        document.getElementById('<%= CikarNowBtn.ClientID%>').click();
    }
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Nüfus Sorgu Listesi Hazırlama"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
            <div style="display: none">
                <input id="paramTcKimlikNo" runat="server" type="text" />
                <asp:LinkButton ID="CikarNowBtn" runat="server" OnClientClick="{return true;};" OnClick="CikarNowBtn_Click"></asp:LinkButton>
            </div>
        </div>
        <div class="card-body">
            <div class="form-group row">
                <div class="form-group col">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="TCKimlikChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="TCKimlikChk_CheckedChanged" ToolTip="TC Kimlik numarası dolu olan bağışçıları listeye eklemek için işaretleyiniz." />
                            TC Kimlik Numarası Dolu Olanlar
                        </label>
                    </div>
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="DogumTarihiChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="DogumTarihiChk_CheckedChanged" ToolTip="Doğum tarihi dolu olan bağışçıları listeye eklemek için işaretleyiniz." />
                            Doğum Tarihi Dolu Olanlar
                        </label>
                    </div>
                </div>
                <div class="form-group col">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="CiplakMukiyetChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="CiplakMukiyetChk_CheckedChanged" ToolTip="Çıplak Mülkiyet bağışlayan bağışçıları listeye eklemek için işaretleyiniz." />
                            Sadece Çıplak Mülkiyet Bağışlayanları Getir
                        </label>
                    </div>
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="SagVefatChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="SagVefatChk_CheckedChanged" ToolTip="Sadece sağ olan bağışçıları listeye eklemek için işaretleyiniz." />
                            Sadece Sağ Olanları Getir
                        </label>
                    </div>
                </div>
                <div class="form-group col">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="TaahhutChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="TaahhutChk_CheckedChanged" ToolTip="Çıplak Mülkiyet bağışlayan bağışçıları listeye eklemek için işaretleyiniz." />
                            Taahhüt Verilen Kişileri Dahil Et
                        </label>
                    </div>
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="VasiyetciChk" runat="server" Checked="True" AutoPostBack="True" CausesValidation="False" OnCheckedChanged="TaahhutChk_CheckedChanged" ToolTip="Çıplak Mülkiyet bağışlayan bağışçıları listeye eklemek için işaretleyiniz." />
                            Vasiyetçileri Dahil Et
                        </label>
                    </div>
                </div>
            </div>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Bağışçı/TaahhütVerilen</th>
                                    <th>Adı</th>
                                    <th>Soyadı</th>
                                    <th>TC Kimlik No</th>
                                    <th>Doğum Tarihi</th>
                                    <th>Çıkar</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div class="form-group">
                        <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Yazı</asp:HyperLink>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="TCKimlikChk" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DogumTarihiChk" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="CiplakMukiyetChk" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="SagVefatChk" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DosyayaKaydetBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="CikarNowBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="DosyayaKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Dosyaya Kaydet " OnClick="DosyayaKaydetBtn_Click" />
        </div>
    </div>
</div>
