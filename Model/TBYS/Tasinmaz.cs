using Model.Ortak;
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
            "Satış",
            "Kamulaştırma",
            "Tevhit"
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
            Il il = new Il();
            il=il.Select<Il>(Id);
            return il==null?string.Empty:il.IlAdi;
        }
        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE EnvanterdeMi=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return (T)Convert.ChangeType(tasinmaz, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<Tasinmaz> genericEntity = new GenericEntity<Tasinmaz>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    Tasinmaz item = Select<Tasinmaz>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Tasinmaz> genericEntity = new GenericEntity<Tasinmaz>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<Tasinmaz> genericEntity = new GenericEntity<Tasinmaz>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Tasinmaz item = Select(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Tasinmaz Select(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
        public string SelectByIdBolumId(int tasinmazId, int bolumId)
        {
            string retVal = string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT A.Adres, A.Ili, A.Ilcesi, B.BolumNo 
                FROM Tasinmaz_Table A
                LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND B.Id=@BolumId
                WHERE A.Id=@TasinmazId");
            query.AddParameter("@BolumId", bolumId);
            query.AddParameter("@TasinmazId", tasinmazId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                string adres = row["Adres"].ReturnEmptyIfNull().ToString();
                string il = row["Ili"].ReturnEmptyIfNull().ToString();
                string ilce = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                string bolumNo = row["BolumNo"].ReturnEmptyIfNull().ToString();
                retVal = adres + " " + bolumNo + " " + ilce + "/" + il;
            }
            return retVal;

        }
        public Tasinmaz SelectById(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
        public Tasinmaz SelectEnvanterdenCikanTasinmaz(int id)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *,Convert(nvarchar,replace (EnvanterdenCikmaBedeli,'.',',')) as EnvanterdenCikmaBedeli
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=0 AND Id=@Id ");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");

            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
		public DataTable SelectByBolgeReturnJson(int bolgeId)
		{
			bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
			string bolgeStr = bolgeFiltresiVar ? " AND E.Id=@BolgeId " : string.Empty;

			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId,  D.IlceAdi +'/'+C.IlAdi IliIlcesi,E.KisaAdi Bolge,
					T.*,
					B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId, B.Sag_vefat                    
				FROM Tasinmaz_Table T
					LEFT JOIN Bagis_Table A ON A.TasinmazId=T.Id
					LEFT JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1 
					{0}", bolgeStr));
			if (bolgeFiltresiVar)
				query.AddParameter("@BolgeId", bolgeId);
			DataTable dataTable = null;
			try
			{
				dataTable = dao.SelectFromDb(query, "");
			}
			catch (Exception e)
			{
				throw;
			}
			return dataTable;
		}
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                                FROM Tasinmaz_Table
                                WHERE EnvanterdeMi=1 ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        /// <summary>
        /// Bagisçisi olmayan envanterdeki tasinmazlari getir Ortak bagislar dahil
        /// </summary>
        /// <returns></returns>
		public DataTable SelectBagiscisiOlmayanTasinmazlarByBagsciIdReturnDT()
		{
			string[] sebepler = SatisVsDahilEnvanterdenCikmaSebepleri;
			string[] sebepParamAdlari = sebepler
				.Select((sebep, index) => "@Sebep" + index)
				.ToArray();
			string sebepInClause = string.Join(",", sebepParamAdlari);

			SqlQuery query = new SqlQuery($@"
				SELECT A.Id TasinmazId,
					A.Id TasinmazId,A.MulkiyetSekli,A.KullanimSekli,A.Ili,A.Ilcesi,A.Adres,A.EnvanterdeMi
				FROM Tasinmaz_Table A
					LEFT JOIN Bagis_Table B ON B.TasinmazId= A.Id
				WHERE (B.BagisciId IS NULL OR B.BagisciId=0)
					AND (
						A.EnvanterdeMi=1
						OR (A.EnvanterdeMi=0 AND A.EnvanterdenCikmaSebebi IN ({sebepInClause}))
					)
				");
			for (int i = 0; i < sebepler.Length; i++)
			{
				query.AddParameter(sebepParamAdlari[i], sebepler[i]);
			}

			DataTable dataTable = null;
			try
			{
				dataTable = dao.SelectFromDb(query, "");
			}
			catch (Exception e)
			{
				throw;
			}
			return dataTable;
		}
        public string SelectTasinmazBolumNoReturnJson(int envanterde, string kirayaUygunluk)
        {
			SqlQuery query = new SqlQuery(@"
				SELECT 
					A.Id, A.Id TasinmazId,A.Cinsi, A.Ili, A.Ilcesi, A.Ili+'/'+A.Ilcesi IliIlcesi, 
										A.SigortaDurumu, A.Adres,A.Adres+' '+A.Ili+'/'+A.Ilcesi AdresIliIlcesi,
										A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, A.SorumluBolge, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo,
										A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi,
										A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto,
										A.Nitelik,A.BulunduguKat, A.Aciklama,A.EnvantereGirisTarihi, 
										B.BolumNo,B.Id BolumId

					FROM Tasinmaz_Table A
						LEFT JOIN BagimsizBolum_Table B On B.TasinmazId = A.Id
						LEFT JOIN KiraSozlesme_Table D ON D.Aktif=1 AND D.Id IN (SELECT SozlesmeId FROM SozlesmeTasinmaz_Table where TasinmazId= A.Id AND (BolumId IS NULL OR BolumId=0 OR BolumId=B.Id))
					WHERE A.EnvanterdeMi=@Envanterde AND A.KirayaUygunluk=@KirayaUygunluk
						AND D.Id IS NULL
					ORDER BY A.Id 
				");
			query.AddParameter("@Envanterde", envanterde);
			query.AddParameter("@KirayaUygunluk", kirayaUygunluk);
            //string sqlString = string.Format(@"
            //    SELECT ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano, A.Id, A.Id TasinmazId,A.Cinsi, A.Ili, A.Ilcesi, A.Ili+'/'+A.Ilcesi IliIlcesi, 
            //        A.SigortaDurumu, A.Adres,A.Adres+' '+A.Ili+'/'+A.Ilcesi AdresIliIlcesi,
            //        A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, A.SorumluBolge, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo,
            //        A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi,
            //        A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto,
            //        A.Nitelik,A.BulunduguKat, A.Aciklama,A.EnvantereGirisTarihi, 
            //        B.BolumNo,B.Id BolumId
            //    FROM Tasinmaz_Table A
            //        LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id
            //    WHERE A.EnvanterdeMi=1 
            //    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(query, "");
            }
            catch (Exception e)
            {
                throw;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectEnvanterdeOlmayanTasinmazReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano, A.Id, A.Id TasinmazId,A.Cinsi, A.Ili, A.Ilcesi, A.Ili+'/'+A.Ilcesi IliIlcesi, 
                    A.SigortaDurumu, A.Adres,A.Adres+' '+A.Ili+'/'+A.Ilcesi AdresIliIlcesi,
                    A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, A.SorumluBolge, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo,
                    A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi,
                    A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto,
                    A.Nitelik,A.BulunduguKat,A.Aciklama,A.EnvantereGirisTarihi, 
                    B.BolumNo,B.Id BolumId
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id
                WHERE A.EnvanterdeMi=2 
                ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectEnvanterdeOlmayanTasinmazReturnDataTable()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano, A.Id, A.Id TasinmazId,A.Cinsi, C.IlAdi Ili, D.IlceAdi Ilcesi, C.IlAdi+'/'+D.IlceAdi IliIlcesi, 
                    A.SigortaDurumu, A.Adres,A.Adres+' '+C.IlAdi+'/'+D.IlceAdi AdresIliIlcesi,
                    A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, E.KisaAdi SorumluBolge, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo,
                    A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi,
                    A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto,
                    A.Nitelik,A.BulunduguKat,A.Aciklama,A.EnvantereGirisTarihi, 
                    B.BolumNo,B.Id BolumId
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id
					LEFT JOIN IL_Table C ON C.Id=A.IlId
					LEFT JOIN ILCE_Table D ON D.Id=A.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE A.EnvanterdeMi=2 
                ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }

            return dataTable;
        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = string.Format(@"
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
            return dataTable;
        }

        public DataTable SelectAllEnvanterdenCikanReturnDataTable()
        {
            string sqlString = SelectAllEnvanterdenCikanSQL();
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            return dataTable;
        }
        private string SelectAllEnvanterdenCikanSQL()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId,T.BagisYili,
                    T.Cinsi, T.Ili, T.Ilcesi, T.Ili+'/'+T.Ilcesi IliIlcesi, T.SigortaDurumu, 
                    T.Adres,T.Adres+' '+T.Ili+'/'+T.Ilcesi AdresIlIlce,
	                T.MulkiyetSekli, T.KiraDurumu, 
                    IIF(T.KatMulkiyeti = 1, 'Kat Mülkiyeti Var', 'Kat Mülkiyeti Yok') AS KatMulkiyeti,
                    T.SorumluBolge, T.EdinmeSekli,T.BagisYili, T.EmlakSicilNo,
                    T.EmlakBeyanDegeri, T.TahminiRayicDegeri, T.TapuTarihi, T.AdaNo, T.ParselNo, T.PaftaNo, T.Yuzolcumu, T.ArsaPayi, T.VakifHissesi,
	                T.YevmiyeNo,T.CiltNo, T.SahifeNo, T.KullanimSekli, T.TasinmazFoto, T.TasinmazFoto1, T.TasinmazFoto2, T.TapuFoto, T.KrokiFoto, T.TahkikatFoto,
	                T.Nitelik,T.BulunduguKat,T.Aciklama,T.EnvantereGirisTarihi,  YEAR(T.EnvanterdenCikmaTarihi) EnvanterdenCikmaYili,
                    --B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId,
                    T.EnvantereGirisTarihi,T.EnvanterdenCikmaTarihi,T.EnvanterdenCikmaSebebi,T.EnvanterdenCikmaBedeli
                FROM Tasinmaz_Table T
	                --INNER JOIN Bagis_Table A ON A.TasinmazId=T.Id
	                --INNER JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                WHERE T.EnvanterdeMi=0 
                ");

            return sqlString;
        }
        public List<Tasinmaz> SelectByIlAdi(string ilAdi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Ili=@IlAdi");
            query.AddParameter("@IlAdi", ilAdi);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            return list;
        }
        public Tasinmaz SelectNext(int tasinmazId)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Id > @TasinmazId
                ORDER BY Id ");
            query.AddParameter("@TasinmazId", tasinmazId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
                tasinmaz = list.FirstOrDefault();
            }
            else
            {
                tasinmaz = SelectMin();

            }
            return tasinmaz;
        }
        public Tasinmaz SelectPrev(int tasinmazId)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Id < @TasinmazId
                ORDER BY Id DESC ");
            query.AddParameter("@TasinmazId", tasinmazId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
                tasinmaz = list.FirstOrDefault();
            }
            else
            {
                tasinmaz = SelectMax();

            }
            return tasinmaz;
        }
        public Tasinmaz SelectMax()
        {
            string sqlString = string.Format(@"
                SELECT MAX(Id) Id  
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int tasinmazId = row["Id"].ConvertToInt();
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(tasinmazId);
                return tasinmaz;
            }
            else
            {
                return null;
            }
        }
        public Tasinmaz SelectMin()
        {
            string sqlString = string.Format(@"
                SELECT MIN(Id) Id  
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int tasinmazId = row["Id"].ConvertToInt();
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(tasinmazId);
                return tasinmaz;
            }
            else
            {
                return null;
            }
        }
		public decimal SelectTahminiRayicToplami(int bolgeId)
		{
			bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
			string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
			decimal toplam = 0;
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT SUM(TahminiRayicDegeri) Toplam 
				FROM Tasinmaz_Table T
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1 
				 {0}", bolgeStr));
			if (bolgeFiltresiVar)
				query.AddParameter("@BolgeId", bolgeId);
			DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
		public decimal SelectEmlakBeyanDegeriToplami(int bolgeId)
		{
			bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
			string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
			decimal toplam = 0;
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT SUM(EmlakBeyanDegeri) Toplam 
				FROM Tasinmaz_Table T
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1  {0}", bolgeStr));
			if (bolgeFiltresiVar)
				query.AddParameter("@BolgeId", bolgeId);
			DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
		public decimal SelectMuhasebeyeKayitliDegerToplami(int bolgeId)
		{
			bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
			string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
			decimal toplam = 0;
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT SUM(MuhasebeyeKayitliDeger) Toplam 
				FROM Tasinmaz_Table T
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1  {0}", bolgeStr));
			if (bolgeFiltresiVar)
				query.AddParameter("@BolgeId", bolgeId);
			DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
		public decimal SelectYaklasikPiyasaToplami(int bolgeId)
		{
			bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
			string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
			decimal toplam = 0;
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT SUM(YaklasikPiyasaDegeri) Toplam 
				FROM Tasinmaz_Table T
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1  {0}", bolgeStr));
			if (bolgeFiltresiVar)
				query.AddParameter("@BolgeId", bolgeId);
			DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectEmlakBeyanDegeriToplamiBySigorta(string sigorta)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(EmlakBeyanDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1
                    AND SigortaDurumu=@Sigorta");
            query.AddParameter("@Sigorta", sigorta);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectTahminiRayicToplamiBySigorta(string sigorta)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(TahminiRayicDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1
                    AND SigortaDurumu=@Sigorta");
            query.AddParameter("@Sigorta", sigorta);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        
        public decimal SelectTahminiRayicToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(TahminiRayicDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 AND KirayaUygunluk=@KirayaUygunluk ");
            query.AddParameter("@KirayaUygunluk", kirayaUygunluk);

            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectEmlakBeyanToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(EmlakBeyanDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 AND KirayaUygunluk=@KirayaUygunluk");
            query.AddParameter("@KirayaUygunluk", kirayaUygunluk);

            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public int SelectTasinmazAdetByBolgeMulkiyetSekli(int bolgeId, string mulkiyetSekli)
        {
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            int Adet = 0;
            SqlQuery query = new SqlQuery(string.Format(@"
                SELECT COUNT(MulkiyetSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId 
                WHERE EnvanterdeMi=1 
                    {0}
                    AND MulkiyetSekli =@MulkiyetSekli", bolgeStr));
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(int bolgeId, string kullanimSekli, string kiraDurumu, string mülkiyetSekli, string kirayaUygunluk = null)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kiraDurumu))
            {
                whereStr = " AND KiraDurumu = @KiraDurumu";
                query.AddParameter("@KiraDurumu", kiraDurumu);
            }
            if (!string.IsNullOrEmpty(mülkiyetSekli))
            {
                whereStr += " AND MulkiyetSekli = @MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mülkiyetSekli);
            }
            if (!string.IsNullOrEmpty(kirayaUygunluk))
            {
                whereStr += " AND KirayaUygunluk = @KirayaUygunluk";
                query.AddParameter("@KirayaUygunluk", kirayaUygunluk);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli =@KullanimSekli
                    {1}", bolgeStr, whereStr);
            query.AddParameter("@KullanimSekli", kullanimSekli);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeCinsiKiraDurumu(int bolgeId, string cinsi, string kiraDurumu, string mülkiyetSekli, string kirayaUygunluk = null)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kiraDurumu))
            {
                whereStr = " AND KiraDurumu = @KiraDurumu";
                query.AddParameter("@KiraDurumu", kiraDurumu);
            }
            if (!string.IsNullOrEmpty(mülkiyetSekli))
            {
                whereStr += " AND MulkiyetSekli = @MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mülkiyetSekli);
            }
            if (!string.IsNullOrEmpty(kirayaUygunluk))
            {
                whereStr += " AND KirayaUygunluk = @KirayaUygunluk";
                query.AddParameter("@KirayaUygunluk", kirayaUygunluk);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND Cinsi =@Cinsi
                    {1}", bolgeStr, whereStr);
            query.AddParameter("@Cinsi", cinsi);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(int bolgeId, string kullanimSekli, string kirayaUygunluk, string mülkiyetSekli)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kirayaUygunluk))
            {
                whereStr = " AND KirayaUygunluk = @KirayaUygunluk";
                query.AddParameter("@KirayaUygunluk", kirayaUygunluk);
            }
            if (!string.IsNullOrEmpty(mülkiyetSekli))
            {
                whereStr += " AND MulkiyetSekli = @MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mülkiyetSekli);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);

            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli =@KullanimSekli
                    {1}", bolgeStr, whereStr);
            query.AddParameter("@KullanimSekli", kullanimSekli);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(int bolgeId, string cinsi, string kirayaUygunluk, string mülkiyetSekli)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kirayaUygunluk))
            {
                whereStr = " AND KirayaUygunluk = @KirayaUygunluk";
                query.AddParameter("@KirayaUygunluk", kirayaUygunluk);
            }
            if (!string.IsNullOrEmpty(mülkiyetSekli))
            {
                whereStr += " AND MulkiyetSekli = @MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mülkiyetSekli);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);

            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(Cinsi) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND Cinsi =@Cinsi
                    {1}", bolgeStr, whereStr);
            query.AddParameter("@Cinsi", cinsi);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKirayaUygunluk(int bolgeId, string kirayaUygunluk, string mulkiyetSekli)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = " AND KirayaUygunluk=@KirayaUygunluk";
            query.AddParameter("@KirayaUygunluk", kirayaUygunluk);

            string mulkiyetStr = string.Empty;
            if (!string.IsNullOrEmpty(mulkiyetSekli))
            {
                mulkiyetStr = " AND MulkiyetSekli=@MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId 
                WHERE EnvanterdeMi=1 
                    {0}
                    {1}
                    {2}", bolgeStr, whereStr, mulkiyetStr);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKirayaUygunlukCinsi(int bolgeId, string kirayaUygunluk, string mulkiyetSekli)
        {
            SqlQuery query = new SqlQuery();
            string whereStr = " AND KirayaUygunluk=@KirayaUygunluk";
            query.AddParameter("@KirayaUygunluk", kirayaUygunluk);

            string mulkiyetStr = string.Empty;
            if (!string.IsNullOrEmpty(mulkiyetSekli))
            {
                mulkiyetStr = " AND MulkiyetSekli=@MulkiyetSekli";
                query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            }
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            int Adet = 0;
            query.Sql = string.Format(@"
                SELECT COUNT(Cinsi) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId 
                WHERE EnvanterdeMi=1 
                    {0}
                    {1}
                    {2}", bolgeStr, whereStr, mulkiyetStr);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(string ilAdi, string mulkiyetSekli, string kullanimSekli)
        {
            int Adet = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 
                    AND Ili =@IlAdi
                    AND MulkiyetSekli =@MulkiyetSekli
                    AND KullanimSekli =@KullanimSekli");
            query.AddParameter("@IlAdi", ilAdi);
            query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            query.AddParameter("@KullanimSekli", kullanimSekli);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public int SelectTasinmazAdetByIliMulkiyetSekliCinsi(string ilAdi, string mulkiyetSekli, string cinsi)
        {
            int Adet = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT(Cinsi) Adet 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 
                    AND Ili =@IlAdi
                    AND MulkiyetSekli =@MulkiyetSekli
                    AND Cinsi =@Cinsi");
            query.AddParameter("@IlAdi", ilAdi);
            query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            query.AddParameter("@Cinsi", cinsi);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(int bolgeId, string mulkiyetSekli, string sigorta)
        {
            int Adet = 0;
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            SqlQuery query = new SqlQuery(string.Format(@"
                SELECT COUNT(MulkiyetSekli) Adet 
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    INNER JOIN Il_Table C ON C.Id=B.IlId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND MulkiyetSekli =@MulkiyetSekli
                    AND SigortaDurumu =@Sigorta", bolgeStr));
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            query.AddParameter("@Sigorta", sigorta);
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigorta(int bolgeId, string kullanimSekli, string sigorta)
        {
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            int Adet = 0;
            SqlQuery query = new SqlQuery(string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    INNER JOIN Il_Table C ON C.Id=B.IlId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli =@KullanimSekli
                    AND SigortaDurumu =@Sigorta", bolgeStr));
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@KullanimSekli", kullanimSekli);
            query.AddParameter("@Sigorta", sigorta);

            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigortaYeni(int bolgeId, string kullanimSekli, string sigorta)
        {
            bool bolgeFiltresiVar = bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT;
            string bolgeStr = bolgeFiltresiVar ? " AND BolgeId=@BolgeId " : string.Empty;
            int Adet = 0;
            SqlQuery query = new SqlQuery(string.Format(@"
                SELECT COUNT(A.KullanimSekli) Adet 
                FROM Sigorta_Table A
                    --INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND (B.EnvanterdeMi=1 OR B.EnvanterdeMi=2) 
                    LEFT JOIN BagimsizBolum_Table F ON F.Id=A.BolumId AND F.TasinmazId=B.Id
                    INNER JOIN Il_Table C ON C.Id=B.IlId
                WHERE 1>0 
                    {0}
                    AND A.KullanimSekli =@KullanimSekli
                    AND SigortaDurumu =@Sigorta", bolgeStr));
            if (bolgeFiltresiVar)
                query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@KullanimSekli", kullanimSekli);
            query.AddParameter("@Sigorta", sigorta);

            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
		public DataTable SelectBolumByTasinmazId(int tasinmazId)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT  A.KatMulkiyeti, A.KullanimSekli,A.Cinsi,A.MulkiyetSekli,D.Adi,D.Soyadi, A.Adres,A.Ilcesi,A.Ili, 
					A.AdaNo,A.ParselNo,A.Yuzolcumu,A.ArsaPayi ,A.EnvanterdeMi,A.KullanimSekli,
					B.Id BolumId,B.BolumNo, B.Aciklama,B.Nitelik, B.Metrekare, B.KullanimAmaci
				FROM Tasinmaz_Table A
					Left Join BagimsizBolum_Table B ON B.TasinmazId=A.Id
					Left Join Bagis_Table C ON C.TasinmazId=A.Id
					Left Join TasinmazBagisci_Table D ON D.Id=C.BagisciId
				WHERE A.Id=@TasinmazId AND EnvanterdeMi=1 AND A.KatMulkiyeti=0");
			query.AddParameter("@TasinmazId", tasinmazId);
			DataTable dataTable = dao.SelectFromDb(query, "");
			return dataTable;
		}

        public DataTable ToplamTasinmazAdediGetir()
        {
            string sqlString = string.Format(@"
                Select MulkiyetSekli, Count(Id) Adet From Tasinmaz_Table
                Where EnvanterdeMi=1
                Group By MulkiyetSekli
                ");
            int toplamTasinmazAdedi = 0;
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }

        public DataTable SelectKirayaUygunTumTasinmazlar()
        {
            string sqlString = string.Format(@"
                SELECT EnvanterdeMi,COUNT(DISTINCT(A.Id)) AnaTasinmaz, Count(B.Id) AltBolum,COUNT(A.Id) ToplamKiralanabilir
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND AltBolum=1
                WHERE EnvanterdeMi in (1,2) AND KirayaUygunluk='Kiraya Uygun'
                GROUP BY EnvanterdeMi

                ");
            int toplamTasinmazAdedi = 0;
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
    }
}
