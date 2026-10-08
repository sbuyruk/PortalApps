using Model.Ortak;
using System;
using System.ComponentModel.DataAnnotations;

namespace Model.TBYS
{
    [Serializable]
    public class Tasinmaz : EntityBase
    {
        /// <summary>
        /// Tasinmaz envanterden ciktiginda, satis vs. kapsaminda degerlendirilecek
        /// "EnvanterdenCikmaSebebi" degerleri. Zamanla degisebileceginden tek
        /// merkezden yonetilir; kullanan sorgular buradan referans almalidir.
        /// </summary>
        public static readonly string[] SatisVsDahilEnvanterdenCikmaSebepleri =
        {
            "Satış","Taşınmaz İadesi", "Taşınmaz Satın Alma","İzale-i Şuyu ile Satış","Trampa","Takas","Kat İrtifakından Kat Mülkiyetine Geçiş",
            "Kat Mülkiyeti Terkini","Mahkeme Kararı ile Envanterden Çıkarma", "Resmi Kuruma Bedelsiz Devir", "Hisse Birleştirme Yoluyla Bağış",
            "Kat Karşılığı İnşaat","Kamulaştırma","İmar Uygulaması","İfraz","Tevhit","Resmi Kurumdan Gelen Yazı ile Düşülmesi"
        };
        public enum SatisPlaniDurumu
        {
            [Display(Name = "Envanterde Tutulacak Taşınmaz")]
            HenuzIslemiPlanlanmamis = 0,

            [Display(Name = "Satışı Yapılacak Taşınmaz")]
            SatisiYapilacak = 1,

            [Display(Name = "Satışı Planlanan Taşınmaz")]
            IkinciPlandaSatisDusunulen = 2,

            [Display(Name = "Proje Geliştirilebilecek Taşınmaz")]
            ProjeGelistirilebilecek = 3,

            [Display(Name = "Hukuki İşlem Gereken Taşınmaz")]
            HukukiIslemGereken = 4,

            [Display(Name = "Hukuki İşlemi Devam Eden Taşınmaz")]
            HukukiIslemiDevamEden = 5,

            [Display(Name = "Kamulaştırılacak Taşınmaz")]
            KamulaTasinmazlar = 6,

            [Display(Name = "Satış Kabiliyeti Olmayan Taşınmaz")]
            SatisKabiliyetiOlmayan = 7,

            [Display(Name = "Sorunlu Taşınmaz")]
            SorunluTasinmazlar = 8,

            [Display(Name = "Yeniden İnşa")]
            YenidenInsa = 9

        }
        public int SatisPlani { get; set; }
        public string SatisPlaniAciklama { get; set; }
        public string Cinsi { get; set; }
        public string Nitelik { get; set; }
        //public string Ili { get { return IliStr(); } set { Ili = value; } }
        public string Ilcesi { get; set;}
        public string Ili { get; set;}
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string SigortaDurumu { get; set; }
        public string Adres { get; set; }
        public string MulkiyetSekli { get; set; }
        public string KiraDurumu { get; set; }
        public string SorumluBolge { get; set; }
        public string EdinmeSekli { get; set; }
        public string BagisYili { get; set; }
        public string EmlakSicilNo { get; set; }
        public decimal MuhasebeyeKayitliDeger { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal YaklasikPiyasaDegeri { get; set; }
        public DateTime TapuTarihi { get; set; }
        public string AdaNo { get; set; }
        public string ParselNo { get; set; }
        public string PaftaNo { get; set; }
        public string Yuzolcumu { get; set; }
        public string ArsaPayi { get; set; }
        public string VakifHissesi { get; set; }
        public string YevmiyeNo { get; set; }
        public string CiltNo { get; set; }
        public string KullanimSekli { get; set; }
        public string SahifeNo { get; set; }
        public string TasinmazFoto { get; set; }
        public string TasinmazFoto1 { get; set; }
        public string TasinmazFoto2 { get; set; }
        public string TasinmazFoto3 { get; set; }
        public string TasinmazFoto4 { get; set; }
        public string TapuFoto { get; set; }
        public string KrokiFoto { get; set; }
        public string TahkikatFoto { get; set; }
        public string Bagisci { get; set; }
        //public string KatMulkiyeti { get; set; }
        public string BulunduguKat { get; set; }
        public string Aciklama { get; set; }
        public int EnvanterdeMi { get; set; }
        public DateTime EnvantereGirisTarihi { get; set; }
        public string EnvanterdenCikmaSebebi { get; set; }
        public DateTime EnvanterdenCikmaTarihi { get; set; }
        public decimal EnvanterdenCikmaBedeli { get; set; }
        public int BagisciId { get; set; }
        public string Mahalle { get; set; }
        public string Koy { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string Mevki { get; set; }
        public string TamHisse { get; set; }
        public string HisseMiktariPay { get; set; }
        public string HisseMiktariPayda { get; set; }
        public string ToplamKatSayisi { get; set; }
        public decimal Metrekare { get; set; }
        public string TapuTasinmazNo { get; set; }
        public string InsaYili { get; set; }
        public string KirayaUygunluk { get; set; }
        public string ProjeM2 { get; set; }
        public string Blok { get; set; }
        public string Giris { get; set; }
        public bool KatMulkiyeti { get; set; }
        public bool KatIrtifaki { get; set; }
        public bool AltBolum { get; set; }
        public decimal ToplamMetrekare { get; set; }
        public string ZeminTipi { get; set; }
        public decimal ZeminHisse { get; set; }
        public decimal BBBrutAlan { get; set; }
        public decimal BBNetAlan { get; set; }
        public DateTime TapuIslemTarihi { get; set; }
        public string BBNitelik { get; set; }
        public string AnaTasinmazNitelik { get; set; }
        public int BagimsizBolumSayisi { get; set; } = 1;
        public int MalikSayisi{ get; set; } = 1;
        public string YapiTarzi { get; set; }
        public string InsaatinSinifi { get; set; }
        public string ArazininCinsi { get; set; }

    }
}
