using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using DAO.Ortak;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class Tasinmaz : ParentClass
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

        private string IliStr() 
        {
            Il il = new IlService().GetById(Id);
            return il==null?string.Empty:il.IlAdi;
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new TasinmazService().GetInventoryById(id), typeof(T));

        }
        public override int Save()
        {
            return new TasinmazService().Save(this);
        }
        public override bool Update()
        {
            return new TasinmazService().Update(this);
        }
        public override bool Delete()
        {
            return new TasinmazService().Delete(this);
        }
        public Tasinmaz Select(int id)
        {
            return new TasinmazService().GetById(id);
        }
        public string SelectByIdBolumId(int tasinmazId, int bolumId)
        {
            return new TasinmazService().GetAddressByBolumId(tasinmazId, bolumId);
        }
        public Tasinmaz SelectById(int id)
        {
            return new TasinmazService().GetById(id);
        }
        public Tasinmaz SelectEnvanterdenCikanTasinmaz(int id)
        {
            return new TasinmazService().GetOutOfInventoryById(id);
        }
		public DataTable SelectByBolgeReturnJson(int bolgeId)
		{
			return new TasinmazService().GetByBolge(bolgeId);
		}
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new TasinmazService().GetInventory(),
                typeof(List<T>));
        }
        /// <summary>
        /// Bagisçisi olmayan envanterdeki tasinmazlari getir Ortak bagislar dahil
        /// </summary>
        /// <returns></returns>
		public DataTable SelectBagiscisiOlmayanTasinmazlarByBagsciIdReturnDT()
		{
			return new TasinmazService().GetWithoutDonor(SatisVsDahilEnvanterdenCikmaSebepleri);
		}
        public string SelectTasinmazBolumNoReturnJson(int envanterde, string kirayaUygunluk)
        {
            return new TasinmazService().GetSectionNumbersAsJson(envanterde, kirayaUygunluk);
        }
        public string SelectEnvanterdeOlmayanTasinmazReturnJson()
        {
            return new TasinmazService().GetOutOfInventoryListAsJson();
        }
        public DataTable SelectEnvanterdeOlmayanTasinmazReturnDataTable()
        {
            return new TasinmazService().GetOutOfInventoryList();
        }
        public DataTable SelectAllReturnDataTable()
        {
            return new TasinmazService().GetAllInventoryReport();
            /*string sqlString = string.Format(@"
                SELECT 
                    ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano,
                    E.KisaAdi AS SorumluBolge,
                    B.Adi + ' ' + B.Soyadi AS Bagisci,
                    B.Sag_vefat,
                    T.Adres,
                    D.IlceAdi AS Ilcesi,
                    C.IlAdi AS Ili,
                    T.Mahalle, T.Koy, T.Cadde, T.Sokak, T.Mevki, T.Giris, T.Blok, 
                    T.AdaNo, T.ParselNo, T.PaftaNo, T.Yuzolcumu, T.ArsaPayi, T.VakifHissesi, T.YevmiyeNo, T.CiltNo, T.SahifeNo,
                    T.KullanimSekli, T.AnaTasinmazNitelik, T.BBNitelik,
                    T.TapuTasinmazNo, T.Cinsi, T.MulkiyetSekli, T.KirayaUygunluk,T.KiraDurumu, T.EdinmeSekli, T.BagisYili, T.Nitelik,
                    T.BulunduguKat, T.BagimsizBolumNo, T.TamHisse, T.HisseMiktariPay, T.HisseMiktariPayda, T.ToplamKatSayisi, T.InsaYili,
                    T.Metrekare, T.ToplamMetrekare, T.ProjeM2, T.ZeminTipi, T.ZeminHisse, T.BBBrutAlan, T.BBNetAlan, T.EnvantereGirisTarihi,
                    IIF(T.KatMulkiyeti = 1, 'Kat Mülkiyeti Var', 'Kat Mülkiyeti Yok') AS KatMulkiyeti,
                    IIF(T.KatIrtifaki = 1, 'Kat Irtifaki Var', 'Kat Irtifaki Yok') AS KatIrtifaki,
                    IIF(T.AltBolum = 1, 'Kat Alt Bölüm Var', 'Kat Alt Bölüm Yok') AS AltBolum,
                    T.TapuTarihi, T.TapuIslemTarihi, T.EmlakSicilNo, T.SigortaDurumu, T.Aciklama,
                    T.EmlakBeyanDegeri, T.TahminiRayicDegeri,T.YaklasikPiyasaDegeri,T.MuhasebeyeKayitliDeger,  
                    T. MalikSayisi,T.BagimsizBolumSayisi, T.YapiTarzi, T.InsaatinSinifi, T.ArazininCinsi,
	                T.Id TasinmazId, G.Id SozlesmeId,H.Adi,G.IlkSozlesmeTar, G.SozBasTar BaslamaTarihi,H.KiralamaAmaci,H.Adres KiraciAdresi,
	                H.Ili,H.Ilcesi,G.OdemeSekli, G.KiraBedeli, G.ArtisAyi,YEAR(G.SozBasTar)-YEAR(G.IlkSozlesmeTar) KiraSuresi,
                    T.SatisPlani, T.SatisPlaniAciklama
                FROM Tasinmaz_Table T
                    LEFT OUTER JOIN Bagis_Table A ON A.TasinmazId = T.Id
                    LEFT OUTER JOIN TasinmazBagisci_Table B ON B.Id = A.BagisciId
                    LEFT JOIN IL_Table C ON C.Id = T.IlId
                    LEFT JOIN ILCE_Table D ON D.Id = T.IlceId
                    LEFT JOIN Bolge_Table E ON E.Id = C.BolgeId
                    OUTER APPLY (
                        SELECT TOP 1 * 
                        FROM SozlesmeTasinmaz_Table F 
                        WHERE F.TasinmazId = T.Id 
                        ORDER BY F.SozlesmeId DESC
                    ) F
	                LEFT JOIN KiraSozlesme_Table G ON G.Id=F.SozlesmeId --AND G.SozlesmeDurumu='Devam Ediyor'
	                LEFT JOIN Kiraci_Table H ON H.Id=G.KiraciId 
                WHERE T.EnvanterdeMi = 1
                ");

            //string sqlString = string.Format(@"
            //    SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano,E.KisaAdi SorumluBolge,B.Adi+' '+B.Soyadi Bagisci, B.Sag_vefat,
            //     T.Adres,D.IlceAdi Ilcesi, C.IlAdi Ili, T.Mahalle,T.Koy,T.Cadde,T.Sokak,T.Mevki,T.Giris,T.Blok, 
            //     T.AdaNo,T.ParselNo,T.PaftaNo,T.Yuzolcumu,T.ArsaPayi,T.VakifHissesi,T.YevmiyeNo,T.CiltNo,T.SahifeNo,T.KullanimSekli,T.AnaTasinmazNitelik,T.BBNitelik,
            //     T.TapuTasinmazNo,T.Cinsi, T.MulkiyetSekli,T.KirayaUygunluk,T.EdinmeSekli,T.BagisYili,T.Nitelik,
            //     T.BulunduguKat, T.BagimsizBolumNo,T.TamHisse,T.HisseMiktariPay,T.HisseMiktariPayda,T.ToplamKatSayisi,T.InsaYili,
            //     T.Metrekare,T.ToplamMetrekare,T.ProjeM2,T.ZeminTipi,T.ZeminHisse, T.BBBrutAlan,T.BBNetAlan,T.EnvantereGirisTarihi,
            //        IIF(T.KatMulkiyeti=1,'Kat Mülkiyeti Var','Kat Mülkiyeti Yok') KatMulkiyeti,
            //     IIF(T.KatIrtifaki=1,'Kat Irtifaki Var','Kat Irtifaki Yok') KatIrtifaki,
            //        IIF(T.AltBolum=1,'Kat Alt Bölüm Var','Kat Alt Bölüm Yok') AltBolum,
            //     T.TapuTarihi,T.TapuIslemTarihi,T.EmlakSicilNo, T.EmlakBeyanDegeri,T.TahminiRayicDegeri,T.SigortaDurumu,T.Aciklama
            //    FROM Tasinmaz_Table T
            //        LEFT OUTER JOIN Bagis_Table A ON A.TasinmazId=T.Id
            //        LEFT OUTER JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
            //        LEFT JOIN IL_Table C ON C.Id=T.IlId
            //        LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
            //        LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
            //    WHERE T.EnvanterdeMi=1 

            //    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            return dataTable;*/
        }

        public DataTable SelectAllEnvanterdenCikanReturnDataTable()
        {
            return new TasinmazService().GetAllOutOfInventoryReport();
        }
        private string SelectAllEnvanterdenCikanSQL()
        {
            //string sqlString = string.Format(@"
            //    SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId,T.BagisYili,
            //        T.Cinsi, T.Ili, T.Ilcesi, T.Ili+'/'+T.Ilcesi IliIlcesi, T.SigortaDurumu, 
            //        T.Adres,T.Adres+' '+T.Ili+'/'+T.Ilcesi AdresIlIlce,
	           //     T.MulkiyetSekli, T.KiraDurumu, 
            //        IIF(T.KatMulkiyeti = 1, 'Kat Mülkiyeti Var', 'Kat Mülkiyeti Yok') AS KatMulkiyeti,
            //        T.SorumluBolge, T.EdinmeSekli,T.BagisYili, T.EmlakSicilNo,
            //        T.EmlakBeyanDegeri, T.TahminiRayicDegeri, T.TapuTarihi, T.AdaNo, T.ParselNo, T.PaftaNo, T.Yuzolcumu, T.ArsaPayi, T.VakifHissesi,
	           //     T.YevmiyeNo,T.CiltNo, T.SahifeNo, T.KullanimSekli, T.TasinmazFoto, T.TasinmazFoto1, T.TasinmazFoto2, T.TapuFoto, T.KrokiFoto, T.TahkikatFoto,
	           //     T.Nitelik,T.BulunduguKat,T.Aciklama,T.EnvantereGirisTarihi,  YEAR(T.EnvanterdenCikmaTarihi) EnvanterdenCikmaYili,
            //        --B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId,
            //        T.EnvantereGirisTarihi,T.EnvanterdenCikmaTarihi,T.EnvanterdenCikmaSebebi,T.EnvanterdenCikmaBedeli
            //    FROM Tasinmaz_Table T
	           //     --INNER JOIN Bagis_Table A ON A.TasinmazId=T.Id
	           //     --INNER JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
            //    WHERE T.EnvanterdeMi=0 
            //    ");
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId,
	                B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId,
	                T.BagisYili,
                    T.Cinsi, T.Ili, T.Ilcesi, T.Ili+'/'+T.Ilcesi IliIlcesi, T.SigortaDurumu, 
                    T.Adres,T.Adres+' '+T.Ili+'/'+T.Ilcesi AdresIlIlce,
                    T.MulkiyetSekli, T.KiraDurumu, 
                    IIF(T.KatMulkiyeti = 1, 'Kat Mülkiyeti Var', 'Kat Mülkiyeti Yok') AS KatMulkiyeti,
                    T.SorumluBolge, T.EdinmeSekli,T.BagisYili, T.EmlakSicilNo,
                    T.EmlakBeyanDegeri, T.TahminiRayicDegeri, T.TapuTarihi, T.AdaNo, T.ParselNo, T.PaftaNo, T.Yuzolcumu, T.ArsaPayi, T.VakifHissesi,
                    T.YevmiyeNo,T.CiltNo, T.SahifeNo, T.KullanimSekli, T.TasinmazFoto, T.TasinmazFoto1, T.TasinmazFoto2, T.TapuFoto, T.KrokiFoto, T.TahkikatFoto,
                    T.Nitelik,T.BulunduguKat,T.Aciklama,T.EnvantereGirisTarihi,  YEAR(T.EnvanterdenCikmaTarihi) EnvanterdenCikmaYili,
    
                    T.EnvantereGirisTarihi,T.EnvanterdenCikmaTarihi,T.EnvanterdenCikmaSebebi,T.EnvanterdenCikmaBedeli
                FROM Tasinmaz_Table T
                    LEFT JOIN Bagis_Table A ON A.TasinmazId=T.Id
                    LEFT JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                WHERE T.EnvanterdeMi=0 
                ");

            return sqlString;
        }
        public List<Tasinmaz> SelectByIlAdi(string ilAdi)
        {
            return new TasinmazService().GetInventoryByIlAdi(ilAdi);
        }
        public Tasinmaz SelectNext(int tasinmazId)
        {
            return new TasinmazService().GetNext(tasinmazId);
        }
        public Tasinmaz SelectPrev(int tasinmazId)
        {
            return new TasinmazService().GetPrev(tasinmazId);
        }
        public Tasinmaz SelectMax()
        {
            return new TasinmazService().GetMax();
        }
        public Tasinmaz SelectMin()
        {
            return new TasinmazService().GetMin();
        }
		public decimal SelectTahminiRayicToplami(int bolgeId)
		{
			return new TasinmazService().GetTahminiRayicToplami(bolgeId);
		}
		public decimal SelectEmlakBeyanDegeriToplami(int bolgeId)
		{
			return new TasinmazService().GetEmlakBeyanDegeriToplami(bolgeId);
		}
		public decimal SelectMuhasebeyeKayitliDegerToplami(int bolgeId)
		{
			return new TasinmazService().GetMuhasebeyeKayitliDegerToplami(bolgeId);
		}
		public decimal SelectYaklasikPiyasaToplami(int bolgeId)
		{
			return new TasinmazService().GetYaklasikPiyasaToplami(bolgeId);
		}

		public decimal SelectEmlakBeyanDegeriToplamiBySigorta(string sigorta)
        {
            return new TasinmazService().GetEmlakBeyanBySigorta(sigorta);
        }
        public decimal SelectTahminiRayicToplamiBySigorta(string sigorta)
        {
            return new TasinmazService().GetTahminiRayicBySigorta(sigorta);
        }
        
        public decimal SelectTahminiRayicToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            return new TasinmazService().GetTahminiRayicByKirayaUygunluk(kirayaUygunluk);
        }
        public decimal SelectEmlakBeyanToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            return new TasinmazService().GetEmlakBeyanByKirayaUygunluk(kirayaUygunluk);
        }
        public int SelectTasinmazAdetByBolgeMulkiyetSekli(int bolgeId, string mulkiyetSekli)
        {
            return new TasinmazService().GetCountByBolgeMulkiyet(bolgeId, mulkiyetSekli);
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(int bolgeId, string kullanimSekli, string kiraDurumu, string mülkiyetSekli, string kirayaUygunluk = null)
        {
            return new TasinmazService().GetCountByBolgeFilters("KullanimSekli", "KullanimSekli", kullanimSekli, kiraDurumu, mülkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByBolgeCinsiKiraDurumu(int bolgeId, string cinsi, string kiraDurumu, string mülkiyetSekli, string kirayaUygunluk = null)
        {
            return new TasinmazService().GetCountByBolgeFilters("KullanimSekli", "Cinsi", cinsi, kiraDurumu, mülkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(int bolgeId, string kullanimSekli, string kirayaUygunluk, string mülkiyetSekli)
        {
            return new TasinmazService().GetCountByBolgeFilters("KullanimSekli", "KullanimSekli", kullanimSekli, null, mülkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(int bolgeId, string cinsi, string kirayaUygunluk, string mülkiyetSekli)
        {
            return new TasinmazService().GetCountByBolgeFilters("Cinsi", "Cinsi", cinsi, null, mülkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByBolgeKirayaUygunluk(int bolgeId, string kirayaUygunluk, string mulkiyetSekli)
        {
            return new TasinmazService().GetCountByBolgeFilters("KullanimSekli", null, null, null, mulkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByBolgeKirayaUygunlukCinsi(int bolgeId, string kirayaUygunluk, string mulkiyetSekli)
        {
            return new TasinmazService().GetCountByBolgeFilters("Cinsi", null, null, null, mulkiyetSekli, kirayaUygunluk, bolgeId);
        }
        public int SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(string ilAdi, string mulkiyetSekli, string kullanimSekli)
        {
            return new TasinmazService().GetCountByIlKullanim(ilAdi, mulkiyetSekli, kullanimSekli);
        }
        public int SelectTasinmazAdetByIliMulkiyetSekliCinsi(string ilAdi, string mulkiyetSekli, string cinsi)
        {
            return new TasinmazService().GetCountByIlCinsi(ilAdi, mulkiyetSekli, cinsi);
        }
        public int SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(int bolgeId, string mulkiyetSekli, string sigorta)
        {
            return new TasinmazService().GetCountBySigorta(bolgeId, "MulkiyetSekli", "MulkiyetSekli", mulkiyetSekli, sigorta, false);
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigorta(int bolgeId, string kullanimSekli, string sigorta)
        {
            return new TasinmazService().GetCountBySigorta(bolgeId, "KullanimSekli", "KullanimSekli", kullanimSekli, sigorta, false);
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigortaYeni(int bolgeId, string kullanimSekli, string sigorta)
        {
            return new TasinmazService().GetCountBySigorta(bolgeId, "KullanimSekli", "KullanimSekli", kullanimSekli, sigorta, true);
        }
		public DataTable SelectBolumByTasinmazId(int tasinmazId)
		{
			return new TasinmazService().GetSectionsByTasinmazId(tasinmazId);
		}

        public DataTable ToplamTasinmazAdediGetir()
        {
            return new TasinmazService().GetTotalByOwnership();
        }

        public DataTable SelectKirayaUygunTumTasinmazlar()
        {
            return new TasinmazService().GetRentalEligibleTotals();
        }
    }
}
