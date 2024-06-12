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
    public class TasinmazBagisci : ParentClass
    {

        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string EPosta { get; set; }
        public string Meslegi { get; set; }
        public string SosyalGuvence { get; set; }
        //public string SorumluBolge { get; set; }
        public string Foto { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public string DefinYeri { get; set; }
        public string DefinIli { get; set; }
        public string DefinIlcesi { get; set; }
        public string DefinAciklama { get; set; }
        //public bool TuzelKisi{ get; set; }                      
        //public bool Sag { get; set; }                           
        //public string Eposta { get; set; }                      
        //public string PostaKodu { get; set; }                  
        public string Aciklama { get; set; }
        public bool Gizli { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazBagisci_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisci, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<TasinmazBagisci> genericEntity = new GenericEntity<TasinmazBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZBAGISCI);
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
                    TasinmazBagisci item = Select<TasinmazBagisci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<TasinmazBagisci> genericEntity = new GenericEntity<TasinmazBagisci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZBAGISCI);
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
                    GenericEntity<TasinmazBagisci> genericEntity = new GenericEntity<TasinmazBagisci>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    TasinmazBagisci item = Select<TasinmazBagisci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZBAGISCI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazBagisci_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<TasinmazBagisci> SelectAllSagBagiscilar(string sag)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazBagisci_Table
                WHERE Sag_vefat={0}",sag.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);

            return (list);
        }        
        public List<TasinmazBagisci> SelectByBolge(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" WHERE BolgeId={0} ", bolgeId);
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazBagisci_Table A
                    LEFT JOIN Il_Table B ON B.IlAdi = A.Ili
                {0}", bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);

            return (list);
        }
        public List<TasinmazBagisci> SelectByFilters(bool isSagVefat, bool isCiplakMulkiyet, bool isTCKimlikNoFull, bool isDogumTarihiFull)
        {
            string TCKimlikNoStr = isTCKimlikNoFull ? string.Format(" AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0 ") : string.Empty;
            string dogumTarihiStr = isDogumTarihiFull ? string.Format(" AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900' ") : string.Empty;
            string sagVefatStr = isSagVefat ? string.Format(" AND Sag_vefat={0}", ProjeConstants.BAGISCI_SAG.ReturnQuotedValue()) : string.Empty;
            string ciplakMulkiyetStr = isCiplakMulkiyet ? string.Format("AND A.Id in (SELECT B.BagisciId FROM Bagis_Table B INNER JOIN Tasinmaz_Table C ON C.Id= B.TasinmazId AND C.MulkiyetSekli={0} )", ProjeConstants.MULKIYETSEKLI_CM.ReturnQuotedValue()) : string.Empty;//string.Format(" AND MulkiyetSekli={0}", ProjeConstants.MULKIYETSEKLI_CM.ReturnQuotedValue()) : string.Empty;
            string whereStr = TCKimlikNoStr + dogumTarihiStr + sagVefatStr + ciplakMulkiyetStr;
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazBagisci_Table A
                WHERE 1>0
                {0}", whereStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);

            return (list);
        }
        public List<TasinmazBagisci> SelectByIlAdi(string ilAdi)
        {
            string sqlString = string.Format(@"SELECT * FROM TasinmazBagisci_Table
                              WHERE Ili={0}", ilAdi.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazBagisci> list = ToList<TasinmazBagisci>(dataTable);
            return list;
        }
        public DataTable SelectAllCountBagisAdediReturnDataTable(bool vefatEdenBagiscilarHaric, bool gizliBagiscilarHaric)
        {
            string vefatEdenBagiscilarHaricStr = vefatEdenBagiscilarHaric? string.Format(" WHERE SAG_VEFAT={0} ",ProjeConstants.BAGISCI_SAG.ReturnQuotedValue()):string.Empty;
            string gizliBagiscilarHaricStr = gizliBagiscilarHaric ? (vefatEdenBagiscilarHaric ? " AND ": " WHERE ") + " (Gizli IS NULL OR Gizli=0) " : string.Empty;

            string sqlString = string.Format(@"
                 SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, A.Id TasinmazBagisciId, Count(C.Id) ToplamBagisAdedi, SUM (D.TahminiRayicDegeri) ToplamTahminiRayic,
                    A.Id TasinmazBagisciId, E.Bolge,
	                A.Adi+' '+ A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ilcesi, A.Ili, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, A.Gizli
                FROM TasinmazBagisci_Table A 
                    LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                    LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
					LEFT JOIN Il_Table E on E.IlAdi=A.Ili 
                {0} {1}
                GROUP BY  C.BagisciId,
				A.Id, A.Adi, A.Soyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ili, A.Ilcesi, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,E.Bolge,A.Gizli                                    
                ", vefatEdenBagiscilarHaricStr, gizliBagiscilarHaricStr);

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
        public DataTable SelectAllCountBagisAdediReturnDataTable_Deprecated(string bolge)
        {
            string bolgeStr = string.IsNullOrEmpty(bolge) ||
                bolge.Equals(ProjeConstants.BOLGE_HEPSI) ||
                bolge.Equals(ProjeConstants.TBYS_YETKILI_BIRIM) ? string.Empty : string.Format(" WHERE E.Bolge={0}", bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                 SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, Count(C.Id) ToplamBagisAdedi, 
                    A.Id TasinmazBagisciId, E.Bolge,
	                A.Adi+' '+ A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ilcesi +'-'+ A.Ili IlIlce, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, FORMAT(A.vefatTarihi,'dd.MM.yyyy') VefatTarihi, DefinYeri,DefinIli,DefinIlcesi,DefinAciklama
                FROM TasinmazBagisci_Table A 
                    LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                    LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
					LEFT JOIN Il_Table E on E.IlAdi=A.Ili
                {0}
                GROUP BY  C.BagisciId,
				    A.Id, A.Adi, A.Soyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ili, A.Ilcesi, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,E.Bolge  , A.VefatTarihi, DefinYeri,DefinIli,DefinIlcesi,DefinAciklama                         
                ", bolgeStr);

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
        public DataTable SelectAllCountBagisAdediReturnDataTable(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" WHERE E.Id={0} ", bolgeId);
            string sqlString = string.Format(@"
                 SELECT 
	                 A.Id As TasinmazBagisciId
	                 ,A.Adi,A.Soyadi,A.Sag_vefat, A.Adi+' '+ A.Soyadi AdiSoyadi
	                 ,COUNT(B.Id) As ToplamBagisAdedi
	                 ,C.IlAdi
	                 ,D.IlceAdi,D.IlceAdi, D.IlceAdi +'-'+ C.IlAdi As IlIlce
	                 ,E.Adi As Bolge
                FROM 
	                TasinmazBagisci_Table A
                LEFT JOIN 
	                Bagis_Table B ON B.BagisciId=A.Id 
                LEFT JOIN 
	                Il_Table C ON C.Id=A.IlId
                LEFT JOIN 
	                Ilce_Table D ON D.Id=A.IlceId
                LEFT JOIN 
	                Bolge_Table E ON E.Id=C.BolgeId
                LEFT JOIN 
	                Tasinmaz_Table F on F.Id=B.TasinmazId AND F.EnvanterdeMi=1
                {0}
                GROUP BY 
	                A.Id
	                ,A.Adi,A.Soyadi,A.Sag_vefat
	                ,C.IlAdi
	                ,D.IlceAdi
	                ,E.Adi
                 ORDER BY A.Adi,A.Soyadi                      
                ", bolgeStr);

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
        public DataTable SelectTasinmazBagisciReturnDataTable(bool gizliBagiscilarHaric)
        {
            string gizliBagiscilarHaricStr = gizliBagiscilarHaric ? " AND Gizli IS NULL OR Gizli=0 " : string.Empty;
            string sqlString = string.Format(@"
                SELECT 
                    A.Id TasinmazBagisciId, E.Bolge,
	                A.Adi+' '+ A.Soyadi AdiSoyadi,Sag_vefat,
                    A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
                    C.TasinmazId, C.BagisTarihi,C.ArmaganId, C.ArmaganDurumu, C.ArmaganTarihi, C.Id BagisId,
	                A.Ili,A.Ilcesi, A.Adres,  
                    A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,A.Gizli
                FROM TasinmazBagisci_Table A 
                    LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                    LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
					LEFT JOIN Il_Table E on E.IlAdi=A.Ili 
					LEFT JOIN Armagan_Table F on F.Id=A.Ili 
                WHERE D.EdinmeSekli='Bağış'
                {0}
				ORDER BY C.BagisTarihi DESC                               
                ", gizliBagiscilarHaricStr);

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
        public DataTable SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT A.Id KatilimciId, A.Adi, A.Soyadi, 
                    A.Adres,A.Telefon1 Telefon,A.Sag_vefat,A.Ilcesi Ilce,A.Ili Il
                FROM TasinmazBagisci_Table A
                WHERE Sag_vefat='Sağ' 
                ORDER BY A.Adi");
            DataTable dataTable;
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
    }
}
