<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DereceKademeListesiWP.ascx.cs" Inherits="IKYS_WebParts.DereceKademeListesiWP.DereceKademeListesiWP" %>


<script type="text/javascript">
    function OpenModal(id,personelId, degisim, tarih, derece, kademe, aciklama, adSoyad) {
        $('#editId').val(id);
        $('#editDegisim').val(degisim);
        $('#editTarih').val(tarih);
        $('#DereceDDL').val(derece);
        $('#KademeDDL').val(kademe);
        $('#editAciklama').val(aciklama);
        $('#editPersonelId').val(personelId);

        var ek = turkceEkGetir(adSoyad); // ← burada ek bulunuyor
        $('#ModalBaslikLbl').text(adSoyad + ek + " Ait Derece/Kademe Değişikliği");
        $('#KaydetBtn').hide();
        $('#GuncelleBtn').show();
        var modal = new bootstrap.Modal(document.getElementById('ModalOnayDiv'));
        modal.show();
    }
    
    function OpenAddModal(personelId, ad, soyad, dereceKademe) {
        // inputları temizle
        $('#editId').val('');
        $('#editDegisim').val('Kademe Yükseltme'); // varsayılan olarak Kademe Yükseltme
        // Tarih alanını bugünün tarihi olarak ayarla
        $('#editTarih').val(new Date().toISOString().split('T')[0]); // ISO formatında tarih

        // Derece dropdownına GecerliDereceTxt değerini koy
        const dereceTxt = document.getElementById('GecerliDereceTxt').value;
        const kademeTxt = document.getElementById('GecerliKademeTxt').value;
        $('#DereceDDL').val(dereceTxt);
        $('#KademeDDL').val(kademeTxt);
        if (dereceKademe === 'Derece') {
            $('#editDegisim').val('Derece Yükseltme');
            $('#DereceDDL').show();
            $('#KademeDDL').hide();
        } else if (dereceKademe === 'Kademe') {
            $('#editDegisim').val('Kademe Yükseltme');
            $('#DereceDDL').hide();
            $('#KademeDDL').show();
        } else {
            $('#editDegisim').val('Göreve Başlama'); // varsayılan olarak Göreve Başlama
            $('#DereceDDL').hide();
            $('#KademeDDL').show();
        }
        $('#editAciklama').val('');
        // personelId'yi ayarla
        $('#editPersonelId').val(personelId);

        const adSoyad = `${ad} ${soyad}`;
        const ek = turkceEkGetir(adSoyad);


        $('#ModalBaslikLbl').text(`${adSoyad}${ek} Ait Derece/Kademe Ekleme`);
        $('#KaydetBtn').show();
        $('#GuncelleBtn').hide();
        const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        modal.show();
    }

    function handleAddClick(dereceKademe) {
        const personelDDL = document.getElementById('PersonelDDL');
        const selectedPersonel = personelDDL.options[personelDDL.selectedIndex];

        const personelId = personelDDL.value;
        const adSoyad = selectedPersonel.text || '';
        const parts = adSoyad.trim().split(' ');
        const ad = parts.slice(0, -1).join(' '); // Ad (birden fazla olabilir)
        const soyad = parts.slice(-1)[0];        // Son kelime soyad



        OpenAddModal(personelId, ad, soyad, dereceKademe);
    }

    function turkceEkGetir(isim) {
        if (!isim) return "'a";

        // Son harfi al
        var sonHarf = isim.slice(-1).toLowerCase();

        // Ünlüye göre a/e ekini seç
        var aGrubu = ['a', 'ı', 'o', 'u'];
        var eGrubu = ['e', 'i', 'ö', 'ü'];
        var aMi = aGrubu.includes(sonHarf);
        var eMi = eGrubu.includes(sonHarf);

        var ek = aMi ? "'ya" : "'ye"; // varsayılan olarak ünsüz sonrası 'ya / 'ye
        // Son harf ünsüzse ondan önceki harfi kontrol et
        if (!aMi && !eMi && isim.length > 1) {
            var onceki = isim.slice(-2, -1).toLowerCase();
            aMi = aGrubu.includes(onceki);
            eMi = eGrubu.includes(onceki);
            ek = aMi ? "'a" : "'e"; // varsayılan olarak ünsüz sonrası 'ya / 'ye
        }
        return ek;
    }

</script>

<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="form-label fw-semibold text-primary fw-bold mb-1" Text="Derece Kademe Değişim Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="form-label fw-semibold text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="form-label fw-semibold" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
             <div class="form-group mb-3 row">

                 <div class="col-2">
                     <label class="form-label fw-semibold" for="PersonelDDL">Personel Seçimi</label>
                     <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control form-select form-select-lg col-6" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" ClientIDMode="Static"/>
                 </div>
                 <div class="col-2">
                     <label class="form-label fw-semibold" for="PersonelDDL">Geçerli Derece</label>
                     <asp:TextBox ID="GecerliDereceTxt" runat="server" CssClass="form-control fw-semibold" TextMode="Number" ClientIDMode="Static" Enabled="false"/>
                 </div>
                 <div class="col-2">
                     <label class="form-label fw-semibold" for="PersonelDDL">Geçerli Kademe</label>
                     <asp:TextBox ID="GecerliKademeTxt" runat="server" CssClass="form-control fw-semibold" TextMode="Number" ClientIDMode="Static"  Enabled="false"/>
                 </div>
                 <div class="col-2">
                     <asp:Panel ID="DereceYukseltPanel" runat="server" CssClass="form-froup" ClientIDMode="Static">
                         <label class="form-label fw-semibold" for="DereceYukseltBtn">Derece Yükselt</label>
                         <button id="DereceYukseltBtn" type="button" class="btn btn-success form-control" onclick="handleAddClick('Derece')">Derece Yükselt</button>
                     </asp:Panel>
                 </div>
                 <div class="col-2">
                     <asp:Panel ID="KademeYukseltPanel" runat="server" CssClass="form-group" ClientIDMode="Static">
                         <label class="form-label fw-semibold" for="EkleBtsn">Kademe Yükselt</label>
                         <button id="KademeYukseltBtn" type="button" class="btn btn-primary form-control" onclick="handleAddClick('Kademe')">Kademe Yükselt</button>
                     </asp:Panel>
                 </div>
             </div>
            <div class="form-group">
                <table id="DereceKademeDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Değişiklik</th>
                            <th>Değişim Tarihi</th>
                            <th>Derece</th>
                            <th>Kademe</th>
                            <th>Açıklama</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
        </div>
    </div>
</div>
<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content" style="width: 550px;">

            <!-- Modal Başlık -->
            <div class="modal-header ">
                <h4 class="modal-title mb-0">
                    <asp:Label ID="ModalBaslikLbl" class="form-label fw-semibold" runat="server" ClientIDMode="Static" Text="Başlık" />
                </h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
            </div>

            <!-- Modal İçerik -->
            <div class="modal-body">
                <asp:HiddenField ID="editId" runat="server" ClientIDMode="Static"/>
                <asp:HiddenField ID="editPersonelId" runat="server" />
                 <div class="form-group mb-3 row">

                     <div class="col">
                         <label for="editDegisim" class="form-label fw-semibold">Değişim</label>
                         <asp:DropDownList ID="editDegisim" runat="server" CssClass="form-control form-select form-select-lg" ClientIDMode="Static">
                             <asp:ListItem Text="Derece Yükseltme" Value="Derece Yükseltme" />
                             <asp:ListItem Text="Kademe Yükseltme" Value="Kademe Yükseltme" />
                             <asp:ListItem Text="Göreve Başlama" Value="Göreve Başlama" />
                         </asp:DropDownList>
                     </div>
                     <div class="col">
                         <label for="editTarih" class="form-label fw-semibold">Değişim Tarihi</label>
                         <asp:TextBox ID="editTarih" runat="server" CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
                     </div>
                 </div>
                <div class="form-group mb-3 row">
                    <div class="col">
                        <label for="DereceDDL" class="form-label fw-semibold">Derece</label>
                        <asp:DropDownList ID="DereceDDL" runat="server" CssClass="form-control form-select form-select-lg" ClientIDMode="Static"></asp:DropDownList>
                    </div>
                    <div class="col">
                        <label for="KademeDDL" class="form-label fw-semibold">Kademe</label>
                        <asp:DropDownList ID="KademeDDL" runat="server" CssClass="form-control form-select form-select-lg" ClientIDMode="Static"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label for="editAciklama" class="form-label fw-semibold">Açıklama</label>
                    <asp:TextBox ID="editAciklama" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" ClientIDMode="Static" />
                </div>
            </div>

            <div class="modal-footer">
                <asp:LinkButton ID="KaydetBtn" runat="server" Text="Kaydet" CssClass="btn btn-success" OnClick="KaydetBtn_Click" ClientIDMode="Static" style="display: none;"/>
                <asp:LinkButton ID="GuncelleBtn" runat="server" Text="Güncelle" CssClass="btn btn-primary" OnClick="GuncelleBtn_Click" ClientIDMode="Static" style="display: none;"/>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
            </div>

        </div>
    </div>
</div>


