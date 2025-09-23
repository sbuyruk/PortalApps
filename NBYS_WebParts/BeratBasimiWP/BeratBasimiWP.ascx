<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BeratBasimiWP.ascx.cs" Inherits="NBYS_WebParts.BeratBasimiWP.BeratBasimiWP" %>
<style>
    /*@media all and (min-width:576px)*/
    .card-columns {
        -webkit-column-count: 4;
        -moz-column-count: 4;
        column-count: 4;
        -webkit-column-gap: 1.25rem;
        -moz-column-gap: 1.25rem;
        column-gap: 1.25rem;
    }

    .durumTable {
        height: 230px;
    }

    input[type=button], input[type=reset], input[type=submit], button {
        font-size: 1.2rem !important;
    }
</style>
<div class="container shadow">
    <div class="card-header ">
        <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
        <h3 class="mb-1">
            <asp:Label CssClass="form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Berat Basımı"></asp:Label>
            <asp:Label ID="IdLbl" runat="server" CssClass="form-label text-white" Visible="false"></asp:Label>
            <asp:Label ID="AdiLbl" runat="server" CssClass="form-label"></asp:Label>
        </h3>
    </div>
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <div class="row mt-2">
                <div class="form-group col-2">
                    <div class="row">
                        <label for="AyDDL" class="col-4 form-label text-end ">Ay</label>
                        <div class="col-8">
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold form-select form-select-lg fw-bold" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="form-group col-2">
                    <div class="row">
                        <label for="YilDDL" class="col-4 form-label text-end">Yıl</label>
                        <div class="col-8">
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold form-select form-select-lg fw-bold" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-columns  text-center">
                <div class="card bg-warning">
                    <h3 class="card-title">Ankara Bölge</h3>
                    <div class="card">
                        <h5 class="card-title">Altın Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="AnkATable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="AnkAltinDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="AnkAltinBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Altın Madalya Beratları" OnClick="AnkAltinBtn_Click" />
                            <asp:Button ID="AnkAltinEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="AnkAltinEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Gümüş Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="AnkGTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="AnkGumusDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="AnkGumusBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Gümüş Madalya Beratları" OnClick="AnkGumusBtn_Click" />
                            <asp:Button ID="AnkGumusEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="AnkGumusEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Bronz Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="AnkBTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="AnkBronzDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="AnkBronzBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Bronz Madalya Beratları" OnClick="AnkBronzBtn_Click" />
                            <asp:Button ID="AnkBronzEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="AnkBronzEtiketBtn_Click" />
                        </div>
                    </div>
                </div>
                <div class="card bg-success">
                    <h3 class="card-title">İstanbul Bölge</h3>
                    <div class="card">
                        <h5 class="card-title">Altın Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IstATable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IstAltinDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IstAltinBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Altın Madalya Beratları" OnClick="IstAltinBtn_Click" />
                            <asp:Button ID="IstAltinEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IstAltinEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Gümüş Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IstGTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IstGumusDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IstGumusBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Gümüş Madalya Beratları" OnClick="IstGumusBtn_Click" />
                            <asp:Button ID="IstGumusEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IstGumusEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Bronz Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IstBTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IstBronzDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IstBronzBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Bronz Madalya Beratları" OnClick="IstBronzBtn_Click" />
                            <asp:Button ID="IstBronzEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IstBronzEtiketBtn_Click" />
                        </div>
                    </div>
                </div>
                <div class="card bg-danger">
                    <h3 class="card-title">İzmir Bölge</h3>
                    <div class="card">
                        <h5 class="card-title">Altın Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IzmATable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IzmAltinDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IzmAltinBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Altın Madalya Beratları" OnClick="IzmAltinBtn_Click" />
                            <asp:Button ID="IzmAltinEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IzmAltinEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Gümüş Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IzmGTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IzmGumusDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IzmGumusBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Gümüş Madalya Beratları" OnClick="IzmGumusBtn_Click" />
                            <asp:Button ID="IzmGumusEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IzmGumusEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Bronz Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="IzmBTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="IzmBronzDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="IzmBronzBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Bronz Madalya Beratları" OnClick="IzmBronzBtn_Click" />
                            <asp:Button ID="IzmBronzEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="IzmBronzEtiketBtn_Click" />
                        </div>
                    </div>
                </div>
                <div class="card bg-info">
                    <h3 class="card-title">Mersin Bölge</h3>
                    <div class="card">
                        <h5 class="card-title">Altın Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="MerATable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="MerAltinDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="MerAltinBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Altın Madalya Beratları" OnClick="MerAltinBtn_Click" />
                            <asp:Button ID="MerAltinEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="MerAltinEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Gümüş Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="MerGTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="MerGumusDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="MerGumusBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Gümüş Madalya Beratları" OnClick="MerGumusBtn_Click" />
                            <asp:Button ID="MerGumusEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="MerGumusEtiketBtn_Click" />
                        </div>
                    </div>
                    <div class="card">
                        <h5 class="card-title">Bronz Madalya</h5>
                        <div class="card-body durumTable">
                            <asp:Table ID="MerBTable" runat="server" class="table table-sm table-striped">
                            </asp:Table>
                        </div>
                        <div class="card-footer">
                            <asp:CheckBox ID="MerBronzDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                            <asp:Button ID="MerBronzBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Bronz Madalya Beratları" OnClick="MerBronzBtn_Click" />
                            <asp:Button ID="MerBronzEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="MerBronzEtiketBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AyDDL" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="YilDDL" EventName="SelectedIndexChanged" />
        </Triggers>
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
