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
    public class TasinmazTaahhut : ParentClass
    {

        public int TasinmazId { get; set; }
        public int BagisciId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string TaahhutAciklama { get; set; }
        public DateTime EvrakTarihi { get; set; }
        public string EvrakSayisi { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazTaahhut_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);
            TasinmazTaahhut tasinmazTaahhut = new TasinmazTaahhut();
            tasinmazTaahhut = list.FirstOrDefault();
            return (T)Convert.ChangeType(tasinmazTaahhut, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<TasinmazTaahhut> genericEntity = new GenericEntity<TasinmazTaahhut>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
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
                    TasinmazTaahhut item = Select<TasinmazTaahhut>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<TasinmazTaahhut> genericEntity = new GenericEntity<TasinmazTaahhut>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
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
                    GenericEntity<TasinmazTaahhut> genericEntity = new GenericEntity<TasinmazTaahhut>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    TasinmazTaahhut item = Select<TasinmazTaahhut>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazTaahhut_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<TasinmazTaahhut> SelectByBagisciId(int bagisciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazTaahhut_Table
                WHERE BagisciId={0}", bagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);

            return list;
        }
        public TasinmazTaahhut SelectByTCKimlikNo(long tcKimlikNo)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazTaahhut_Table
                WHERE TCKimlikNo > 0 AND TCKimlikNo={0}", tcKimlikNo);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);

            return list.FirstOrDefault<TasinmazTaahhut>();
        }
        public List<TasinmazTaahhut> SelectByFilters(bool isSagVefat,  bool isTCKimlikNoFull, bool isDogumTarihiFull, int bolgeId)
        {
            string TCKimlikNoStr = isTCKimlikNoFull ? string.Format(" AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0 ") : string.Empty;
            string dogumTarihiStr = isDogumTarihiFull ? string.Format(" AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900' ") : string.Empty;
            string sagVefatStr = isSagVefat ? string.Format(" AND Sag_vefat={0}", ProjeConstants.BAGISCI_SAG.ReturnQuotedValue()) : string.Empty;
            string bolgeStr = bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            string whereStr = TCKimlikNoStr + dogumTarihiStr + sagVefatStr + bolgeStr;
            string sqlString = string.Format(@"
                SELECT *
                FROM TasinmazTaahhut_Table A
                    LEFT JOIN Il_Table B ON B.Id = A.Ili
                WHERE 1>0
                {0}", whereStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);

            return (list);
        }
        public List<TasinmazTaahhut> SelectByIlAdi(string ilAdi)
        {
            string sqlString = string.Format(@"SELECT * FROM TasinmazTaahhut_Table
                              WHERE Ili={0}", ilAdi.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<TasinmazTaahhut> list = ToList<TasinmazTaahhut>(dataTable);
            return list;
        }
        public string SelectAllCountBagisAdediReturnJson()
        {
            string sqlString = string.Format(@"
                 SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, Count(C.Id) ToplamBagisAdedi, 
                    A.Id TasinmazTaahhutId, E.Bolge,
	                A.Adi+' '+ A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ilcesi +'-'+A.Ili IlIlce, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, FORMAT(A.vefatTarihi,'dd.MM.yyyy') VefatTarihi
                FROM TasinmazTaahhut_Table A 
                    LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                    LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
					LEFT JOIN Il_Table E on E.IlAdi=A.Ili 
                GROUP BY  C.BagisciId,
				A.Id, A.Adi, A.Soyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ili, A.Ilcesi, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,E.Bolge , A.vefatTarihi                                   
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
        public DataTable SelectAllCountBagisAdediReturnDataTable(bool vefatEdenBagiscilarHaric, bool gizliBagiscilarHaric)
        {
            string vefatEdenBagiscilarHaricStr = vefatEdenBagiscilarHaric? string.Format(" WHERE SAG_VEFAT={0} ",ProjeConstants.BAGISCI_SAG.ReturnQuotedValue()):string.Empty;
            string gizliBagiscilarHaricStr = gizliBagiscilarHaric ? (vefatEdenBagiscilarHaric ? " AND ": " WHERE ") + " (Gizli IS NULL OR Gizli=0) " : string.Empty;

            string sqlString = string.Format(@"
                 SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, A.Id TasinmazTaahhutId, Count(C.Id) ToplamBagisAdedi, SUM (D.TahminiRayicDegeri) ToplamTahminiRayic,
                    A.Id TasinmazTaahhutId, E.Bolge,
	                A.Adi+' '+ A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence, 
	                A.Ilcesi, A.Ili, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, A.Gizli
                FROM TasinmazTaahhut_Table A 
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
                throw;
            }
            return dataTable;
        }

    }
}
