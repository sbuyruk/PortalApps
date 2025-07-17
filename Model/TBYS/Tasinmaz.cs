using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class Tasinmaz : ParentClass
    {
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
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
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
        public string KatMulkiyeti { get; set; }
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
        public bool KatMulkiyetiChk { get; set; }
        public bool KatIrtifaki { get; set; }
        public bool AltBolum { get; set; }
        public decimal ToplamMetrekare { get; set; }
        public string ZeminTipi { get; set; }
        public decimal ZeminHisse { get; set; }
        public decimal BBBrutAlan { get; set; }
        public decimal BBNetAlan { get; set; }
        public DateTime TapuIslemTarihi { get; set; }

        private string IliStr() 
        {
            Il il = new Il();
            il=il.Select<Il>(Id);
            return il==null?string.Empty:il.IlAdi;
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE EnvanterdeMi=1 AND Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

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
                throw ex;
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
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
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
                    string sqlString = genericEntity.GetQuery(this);
                    Tasinmaz item = Select(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
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
                throw ex;
            }
        }
        public Tasinmaz Select(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
        public string SelectByIdBolumId(int tasinmazId, int bolumId)
        {
            string retVal = string.Empty;
            string sqlString = string.Format(@"
                SELECT A.Adres, A.Ili, A.Ilcesi, B.BolumNo 
                FROM Tasinmaz_Table A
                LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND B.Id={0}
                WHERE A.Id={1}", bolumId, tasinmazId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"SELECT *
                               FROM Tasinmaz_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
        public Tasinmaz SelectEnvanterdenCikanTasinmaz(int id)
        {
            string sqlString = string.Format(@"
                SELECT *,Convert(nvarchar,replace (EnvanterdenCikmaBedeli,'.',',')) as EnvanterdenCikmaBedeli
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=0 AND Id={0} ", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = list.FirstOrDefault();
            return tasinmaz;

        }
        public DataTable SelectByBolgeReturnJson(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND E.Id={0} ", bolgeId);

            string sqlString = string.Format(@"
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
				    {0}", bolgeStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
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
        /// Bağışçısı olmayan envanterdeki taşınmazları getir Ortak bağışlar dahil
        /// </summary>
        /// <returns></returns>
        public DataTable SelectBagiscisiOlmayanTasinmazlarByBagsciIdReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT A.Id TasinmazId,
                    A.Id TasinmazId,A.MulkiyetSekli,A.KullanimSekli,A.Ili,A.Ilcesi,A.Adres
	            FROM Tasinmaz_Table A
		            LEFT JOIN Bagis_Table B ON B.TasinmazId= A.Id
	            WHERE A.EnvanterdeMi=1 AND (B.BagisciId IS NULL OR B.BagisciId=0)
                ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public string SelectTasinmazBolumNoReturnJson(int envanterde, string kirayaUygunluk)
        {
            string sqlString = string.Format(@"
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
                    WHERE A.EnvanterdeMi={0} AND A.KirayaUygunluk={1}
	                    AND D.Id IS NULL
                    ORDER BY A.Id 
                ", envanterde,kirayaUygunluk.ReturnQuotedValue());
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
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
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
                throw e;
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
                throw e;
            }

            return dataTable;
        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId, D.IlceAdi+'/'+C.IlAdi IliIlcesi,E.Adi Bolge,
                    T.*,
                    B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId, B.Sag_vefat                    
                FROM Tasinmaz_Table T
	                LEFT OUTER JOIN Bagis_Table A ON A.TasinmazId=T.Id
	                LEFT OUTER JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                
					LEFT JOIN IL_Table C ON C.Id=T.IlId
					LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
					LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
				WHERE T.EnvanterdeMi=1 

                ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
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
                throw e;
            }
            return dataTable;
        }
        private string SelectAllEnvanterdenCikanSQL()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId,T.Cinsi, T.Ili, T.Ilcesi, T.Ili+'/'+T.Ilcesi IliIlcesi, T.SigortaDurumu, 
                    T.Adres,T.Adres+' '+T.Ili+'/'+T.Ilcesi AdresIlIlce,
	                T.MulkiyetSekli, T.KiraDurumu, T.KatMulkiyeti, T.SorumluBolge, T.EdinmeSekli,T.BagisYili, T.EmlakSicilNo,
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
            string sqlString = string.Format(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Ili={0}", ilAdi.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Tasinmaz> list = ToList<Tasinmaz>(dataTable);
            return list;
        }
        public Tasinmaz SelectNext(int tasinmazId)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            string sqlString = string.Format(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Id > {0}
                ORDER BY Id ", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"
                SELECT * FROM Tasinmaz_Table
                WHERE EnvanterdeMi=1 AND Id < {0}
                ORDER BY Id DESC ", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(TahminiRayicDegeri) Toplam 
                FROM Tasinmaz_Table T
                    LEFT JOIN IL_Table C ON C.Id=T.IlId
	                LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
	                LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE T.EnvanterdeMi=1 
                 {0}", bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectEmlakBeyanDegeriToplami(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(EmlakBeyanDegeri) Toplam 
                FROM Tasinmaz_Table T
                    LEFT JOIN IL_Table C ON C.Id=T.IlId
	                LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
	                LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE T.EnvanterdeMi=1  {0}", bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"
                SELECT SUM(EmlakBeyanDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1
                    AND SigortaDurumu={0}", sigorta.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"
                SELECT SUM(TahminiRayicDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1
                    AND SigortaDurumu={0}", sigorta.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        
        public decimal SelectTahminiRayicToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            string whereStr = " AND KirayaUygunluk = " + kirayaUygunluk.ReturnQuotedValue();
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(TahminiRayicDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 {0} ", whereStr);


            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public decimal SelectEmlakBeyanToplamiByKirayaUygunluk(string kirayaUygunluk)
        {
            string whereStr = " AND KirayaUygunluk=" + kirayaUygunluk.ReturnQuotedValue();


            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(EmlakBeyanDegeri) Toplam 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 {0}", whereStr);


            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                toplam = row["Toplam"].ConvertToDecimal();

            }
            return toplam;
        }
        public int SelectTasinmazAdetByBolgeMulkiyetSekli(int bolgeId, string mulkiyetSekli)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            int Adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(MulkiyetSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId 
                WHERE EnvanterdeMi=1 
                    {0}
                    AND MulkiyetSekli ={1}", bolgeStr, mulkiyetSekli.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(int bolgeId, string kullanimSekli, string kiraDurumu, string mülkiyetSekli, string kirayaUygunluk = null)
        {
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kiraDurumu))
                whereStr = " AND KiraDurumu = " + kiraDurumu.ReturnQuotedValue();
            if (!string.IsNullOrEmpty(mülkiyetSekli))
                whereStr += " AND MulkiyetSekli = " + mülkiyetSekli.ReturnQuotedValue();
            if (!string.IsNullOrEmpty(kirayaUygunluk))
                whereStr += " AND KirayaUygunluk = " + kirayaUygunluk.ReturnQuotedValue();
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            int Adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli ={1}
                    {2}", bolgeStr, kullanimSekli.ReturnQuotedValue(), whereStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliKirayaUygunluk(int bolgeId, string kullanimSekli, string kirayaUygunluk, string mülkiyetSekli)
        {
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(kirayaUygunluk))
                whereStr = " AND KirayaUygunluk = " + kirayaUygunluk.ReturnQuotedValue();
            if (!string.IsNullOrEmpty(mülkiyetSekli))
                whereStr += " AND MulkiyetSekli = " + mülkiyetSekli.ReturnQuotedValue();
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);

            int Adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli ={1}
                    {2}", bolgeStr, kullanimSekli.ReturnQuotedValue(), whereStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();

            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKirayaUygunluk(int bolgeId, string kirayaUygunluk, string mulkiyetSekli)
        {
            string whereStr = " AND KirayaUygunluk=" + kirayaUygunluk.ReturnQuotedValue();

            string mulkiyetStr = string.IsNullOrEmpty(mulkiyetSekli) ? string.Empty : " AND MulkiyetSekli=" + mulkiyetSekli.ReturnQuotedValue();
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            int Adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table A
                LEFT JOIN Il_Table B ON B.Id=A.IlId
                LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId 
                WHERE EnvanterdeMi=1 
                    {0}
                    {1}
                    {2}", bolgeStr, whereStr, mulkiyetStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string sqlString = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Tasinmaz_Table 
                WHERE EnvanterdeMi=1 
                    AND Ili ={0}
                    AND MulkiyetSekli ={1}
                    AND KullanimSekli ={2}", ilAdi.ReturnQuotedValue(), mulkiyetSekli.ReturnQuotedValue(), kullanimSekli.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            string sqlString = string.Format(@"
                SELECT COUNT(MulkiyetSekli) Adet 
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    INNER JOIN Il_Table C ON C.Id=B.IlId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND MulkiyetSekli ={1}
                    AND SigortaDurumu ={2}", bolgeStr, mulkiyetSekli.ReturnQuotedValue(), sigorta.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public int SelectTasinmazAdetByBolgeKullanimSekliSigorta(int bolgeId, string kullanimSekli, string sigorta)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            int Adet = 0;
            string sqlString = string.Format(@"
                SELECT COUNT(KullanimSekli) Adet 
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    INNER JOIN Il_Table C ON C.Id=B.IlId
                WHERE EnvanterdeMi=1 
                    {0}
                    AND KullanimSekli ={1}
                    AND SigortaDurumu ={2}", bolgeStr, kullanimSekli.ReturnQuotedValue(), sigorta.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                Adet = row["Adet"].ConvertToInt();
            }
            return Adet;
        }
        public DataTable SelectBolumByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT  A.KatMulkiyeti, A.KullanimSekli,A.Cinsi,A.MulkiyetSekli,D.Adi,D.Soyadi, A.Adres,A.Ilcesi,A.Ili, 
                    A.AdaNo,A.ParselNo,A.Yuzolcumu,A.ArsaPayi ,A.EnvanterdeMi,A.KullanimSekli,
					B.Id BolumId,B.BolumNo, B.Aciklama,B.Nitelik, B.Metrekare, B.KullanimAmaci
                FROM Tasinmaz_Table A
	                Left Join BagimsizBolum_Table B ON B.TasinmazId=A.Id
	                Left Join Bagis_Table C ON C.TasinmazId=A.Id
	                Left Join TasinmazBagisci_Table D ON D.Id=C.BagisciId
                WHERE A.Id={0} AND EnvanterdeMi=1 AND A.KatMulkiyeti={1}", tasinmazId,ProjeConstants.KAT_MULKIYETI_YOK.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
    }
}
