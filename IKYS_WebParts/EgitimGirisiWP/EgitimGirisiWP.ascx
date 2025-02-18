<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EgitimGirisiWP.ascx.cs" Inherits="IKYS_WebParts.EgitimGirisiWP.EgitimGirisiWP" %>
<div class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="text-success" ID="TitleLbl" runat="server" Text="Eğitim Bilgileri Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary" ID="PersonelIdLbl" Visible="false" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="PersonelAdiLbl" runat="server"></asp:Label>

            </h3>
        </div>
        <div class="card-body">

            <div class="card-body">

                <div class="card">
                    <div class="bg-secondary">
                        <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#OkulDiv" aria-expanded="false" aria-controls="OkulDiv" style="font-weight: bold">Mezun Olduğu Okullar</a>
                    </div>
                </div>
                <div class="card">
                    <div class="collapse" id="OkulDiv">
                        <asp:UpdatePanel ID="OkulTableUpdatePanel" runat="server">
                            <ContentTemplate>
                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                    <asp:Table ID="OkulTable" runat="server" CssClass="table table-sm small table-hover">
                                    </asp:Table>
                                </div>
                                <div id="EkleDiv1" class="row">
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="OkulAdiTxt" class="col-form-label">Okul</label>
                                        <div>
                                            <asp:TextBox ID="OkulTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label for="OkulSeviyeDDL" class="col-form-label">Seviye</label>
                                        <div>
                                            <asp:DropDownList ID="OkulSeviyeDDL" runat="server" class="form-control "></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label class="col-form-label">Mezuniyet Tar.</label>
                                        <div>
                                            <input runat="server" type="text" id="MezuniyetTarTxt" name="OkulMezuniyetTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="AciklamaTxt" class="col-form-label">Açıklama</label>
                                        <div>
                                            <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2">
                                        <label class="col-form-label"></label>
                                        <div>
                                            <asp:LinkButton ID="OkulEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Okul Ekle" OnClick="OkulEkleBtn_Click" />
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
                <div class="card">
                    <div class="bg-secondary">
                        <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#KursDiv" aria-expanded="false" aria-controls="KursDiv" style="font-weight: bold">Gördüğü Kurslar</a>
                    </div>
                </div>
                <div class="card">
                    <div class="collapse" id="KursDiv">
                        <asp:UpdatePanel ID="KursTableUpdatePanel" runat="server">
                            <ContentTemplate>
                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                    <asp:Table ID="KursTable" runat="server" CssClass="table table-sm small table-hover">
                                    </asp:Table>
                                </div>
                                <div id="KursEkleDiv" class="row">
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="KursAdiTxt" class="col-form-label">Kurs/Eğt/Sertifika</label>
                                        <div>
                                            <asp:TextBox ID="KursAdiTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label class="col-form-label">Tarih</label>
                                        <div>
                                            <input runat="server" type="text" id="TarihTxt" name="TarihTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label for="SureTxt" class="col-form-label">Kurs Süresi</label>
                                        <div>
                                            <asp:TextBox ID="SureTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="VerenKurumTxt" class="col-form-label">Veren Kurum</label>
                                        <div>
                                            <asp:TextBox ID="VerenKurumTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2">
                                        <label class="col-form-label"></label>
                                        <div>
                                            <asp:LinkButton ID="KursEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Kurs Ekle" OnClick="KursEkleBtn_Click" />
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="card">
                    <div class="bg-secondary">
                        <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#IsTecrubeDiv" aria-expanded="false" aria-controls="IsTecrubeDiv" style="font-weight: bold">İş Tecrübesi</a>
                    </div>
                </div>
                <div class="card">
                    <div class="collapse" id="IsTecrubeDiv">
                        <asp:UpdatePanel ID="IsyeriTableUpdatePanel" runat="server">
                            <ContentTemplate>
                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                    <asp:Table ID="IsyeriTable" runat="server" CssClass="table table-sm small table-hover">
                                    </asp:Table>
                                </div>
                                <div id="IsyeriEkleDiv" class="row">
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="IsyeriTxt" class="col-form-label">Çalıştığı İşyeri</label>
                                        <div>
                                            <asp:TextBox ID="IsyeriTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-3 nopadding">
                                        <label for="GoreviTxt" class="col-form-label">Yaptığı Görev</label>
                                        <div>
                                            <asp:TextBox ID="GoreviTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label class="col-form-label">Baş.Tarih</label>
                                        <div>
                                            <input runat="server" type="text" id="BasTarTxt" name="BasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                    </div>
                                    <div class="form-group col-sm-2 nopadding">
                                        <label class="col-form-label">Ayr.Tarih</label>
                                        <div>
                                            <input runat="server" type="text" id="BitTarTxt" name="BitTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                    </div>

                                    <div class="form-group col-sm-2">
                                        <label class="col-form-label"></label>
                                        <div>
                                            <asp:LinkButton ID="IsyeriEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="İşyeri Ekle" OnClick="IsyeriEkleBtn_Click" />
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="card">
                    <div class="bg-secondary">
                        <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#YabanciDilDiv" aria-expanded="false" aria-controls="YabanciDilDiv" style="font-weight: bold">Yabancı Dil Notları</a>
                    </div>
                </div>
                <div class="card">
                    <div class="collapse" id="YabanciDilDiv">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                    <asp:Table ID="YabanciDilTable" runat="server" CssClass="table table-sm small table-hover">
                                    </asp:Table>
                                </div>
                                <div id="YabanciDilEkleDiv" class="row">
                                    <div class="form-group col-2 nopadding">
                                        <label for="YabanciDilAdiTxt" class="col-form-label">YabanciDil</label>
                                        <div>
                                            <asp:DropDownList ID="YabanciDilDDL" runat="server" class="form-control " style="height:auto"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group col-4 nopadding">
                                        <label for="SinavAdiTxt" class="col-form-label">Sınav Adı</label>
                                        <div>
                                            <asp:TextBox ID="SinavAdiTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-2 nopadding">
                                        <label for="SinavNotuTxt" class="col-form-label">Sınav Notu</label>
                                        <div>
                                            <asp:TextBox ID="SinavNotuTxt" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group col-2 nopadding">
                                        <label class="col-form-label">Sınav Tar.</label>
                                        <div>
                                            <input runat="server" type="text" id="SinavTarihiTxt" name="SinavTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                    </div>
                                    <div class="form-group col-2">
                                        <label class="col-form-label"></label>
                                        <div>
                                            <asp:LinkButton ID="YabanciDilEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Ekle" OnClick="YabanciDilEkleBtn_Click" />
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <%--                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="OkulEkleBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="KursEkleBtn" EventName="click" />
                            </Triggers>--%>
                        </asp:UpdatePanel>
                    </div>

                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
        </div>
    </div>
</div>
