<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisaVesileOlanArmaganBelgesiWP.ascx.cs" Inherits="NBYS_WebParts.BagisaVesileOlanArmaganBelgesiWP.BagisaVesileOlanArmaganBelgesiWP" %>

<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text=" Bağışa Vesile Olanlara Teşekkür Belgesi Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row p-2">
                        <div class="form-group col-3 border border-dark  border-right-0">

                            <div class="form-group">
                                <label for="AdiTxt" class="col-form-label">Adı </label>
                                <asp:TextBox ID="AdiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="SoyadiTxt" class="col-form-label">Soyadı </label>
                                <asp:TextBox ID="SoyadiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="UnvanTxt" class="col-form-label">Ünvan </label>
                                <asp:TextBox ID="UnvanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="TCKimlikNoTxt" class="col-form-label">TC Kimlik No </label>
                                <asp:TextBox ID="TCKimlikNoTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>
                        <div class="form-group col-5 border border-dark  border-right-0">
                            <div class="form-group ">
                                <label for="BelgeMetni1Txt" class="col-form-label">Belge Metni (1nci Paragraf)</label>
                                <asp:TextBox ID="BelgeMetni1Txt" runat="server" TextMode="MultiLine" Rows="9" class="form-control" type="text" Text="    TSK Güçlendirme Vakfı, ülkemize kazandırdığı ASELSAN, TUSAŞ, ROKETSAN, HAVELSAN, İŞBİR ve ASPİLSAN gibi şirketler ile Türk Silahlı Kuvvetlerinin güçlendirilmesi ve Milli Savunma Sanayiimizin geliştirilmesine öncülük etmenin haklı gururunu yaşamaktadır. Vakıf kurulduğu 1987 yılından beri, Türk Milleti ile TSK arasında var olan gönül bağını güçlendirmek amacıyla yüce milletinden aldığı maddi ve manevi destek sayesinde faaliyetlerini sürdürmektedir.

                                    " />
                            </div>
                            <div class="form-group  ">
                                <label for="BelgeMetni2Txt" class="col-form-label">Belge Metni (2nci Paragraf)</label>
                                <asp:TextBox ID="BelgeMetni2Txt" runat="server" TextMode="MultiLine" Rows="6" class="form-control" type="text" Text="    Bu desteğin sürmesi, Vakfımızın görünürlüğünün artırılması ve bağışların TSK Güçlendirme Vakfı’na yönlendirilmesi amacıyla verdiğiniz destek için teşekkür ediyor, şükranlarımı sunuyorum.

                                    " />
                            </div>
                        </div>
                        <div class="form-group col-4 border border-dark">
                            <div class="row">


                                <div class="form-group col">
                                    <div class="form-group">
                                        <label for="BelgeTarihiTxt" class="col-form-label">Belge Tarihi</label>
                                        <asp:TextBox ID="BelgeTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="BelgeNoTxt" class="col-form-label">Belge No</label>
                                        <asp:TextBox ID="BelgeNoTxt" runat="server" CssClass="form-control text-bg-secondary" ReadOnly="true" ToolTip="Belge Numarası Otomatik verilecektir" placeholder="Belge Numarası"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="VerilmeSebebiTxt" class="col-form-label">Verilme Sebebi </label>
                                        <asp:TextBox ID="VerilmeSebebiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group col">
                                    <div class="form-group">
                                        <label for="ImzalayanTxt" class="col-form-label">İmza (Adi Soyadı)</label>
                                        <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="ImzalayanUnvanTxt" class="col-form-label">İmza (Ünvan)</label>
                                        <asp:TextBox ID="ImzalayanUnvanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="ImzalayanMakamTxt" class="col-form-label">İmza (Makam)</label>
                                        <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="6" class="form-control" type="text" />
                            </div>
                        </div>
                    </div>
                    

                    <div class="card-footer">
                        <asp:LinkButton ID="DosyaOlusturBtn" runat="server" CssClass="btn btn-success" OnClick="DosyaOlusturBtn_Click">Kaydet ve Teşekkür Belgesini Oluştur</asp:LinkButton>
                        <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Teşekkür Belgeleri</asp:HyperLink>
                    </div>
                    <hr />
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-hover small" width="100%">
                            <thead>
                                <tr>
                                    <th>Adı</th>
                                    <th>Soyadı</th>
                                    <th>Verilme Sebebi</th>
                                    <th>Belge Tarihi</th>
                                    <th>İmzalayan</th>
                                    <th>Açıklama</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                 
                    <div class="form-group">
                        <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                    </div>
                </div>

                <div class="card-footer">
                    
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
