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
    public class Bagis : ParentClass
    {
        public int BagisciId { get; set; }
        public int TasinmazId { get; set; }
        public DateTime BagisTarihi { get; set; }
        public int BagisYili { get; set; }
        public bool Envanterde { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Bagis_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bagis> list = ToList<Bagis>(dataTable);
            Bagis bagis = new Bagis();
            bagis = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagis, typeof(T));

        }
        public Bagis Select(int id)
        {
            GenericEntity<Bagis> genericEntity = new GenericEntity<Bagis>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bagis> list = ToList<Bagis>(dataTable);
            Bagis bagis = new Bagis();
            bagis = list.FirstOrDefault();
            return bagis;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Bagis> genericEntity = new GenericEntity<Bagis>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
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
                    Bagis item = Select<Bagis>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Bagis> genericEntity = new GenericEntity<Bagis>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
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
                    GenericEntity<Bagis> genericEntity = new GenericEntity<Bagis>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Bagis item = Select<Bagis>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
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
                               FROM Bagis_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bagis> list = ToList<Bagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Bagis> SelectByBagisciId(int bagisciId)
        {

            string sqlString = string.Format(@"SELECT * FROM Bagis_Table
                              WHERE BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bagis> list = ToList<Bagis>(dataTable);
            return list;
        }
        public DataTable SelectByBagisciIdGroupByKullanimSekli(int bagisciId)
        {
            string sqlString = string.Format(@"
                SELECT B.KullanimSekli,COUNT(A.Id) Adet,B.Ili, SUM(B.TahminiRayicDegeri) TahminiRayic
                FROM Bagis_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                WHERE A.BagisciId={0}
                GROUP BY B.KullanimSekli,B.Ili
            ", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public string SelectByBagisciIdReturnJson(int bagisciId)
        {
            string sqlString = string.Format(@"
            SELECT 
	            A.Id BagisciId, A.Adi, A.Soyadi,
	            C.Id TasinmazId, C.Adres
            FROM TasinmazBagisci_Table A
	            INNER JOIN  Bagis_Table B ON B.BagisciId=A.Id 
	            INNER JOIN Tasinmaz_Table C ON C.Id=B.TasinmazId AND C.EnvanterdeMi=1
                              WHERE BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            string json = ToJSON(dataTable);
            return json;
        }
        public Bagis SelectByTasinmazId(int tasinmazId)
        {

            string sqlString = string.Format(@"SELECT * FROM Bagis_Table
                              WHERE TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bagis> list = ToList<Bagis>(dataTable);
            Bagis bagis = new Bagis();
            bagis = list.FirstOrDefault();
            return bagis;
        }
        public string SelectTasinmazByBagisciIdReturnJson(int bagisciId)
        {
            string sqlString = SelectTasinmazBagisByBagisciIdSQL(bagisciId);
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
        public DataTable SelectTasinmazByBagisciIdReturnDT(int bagisciId)
        {
            string sqlString = SelectTasinmazBagisByBagisciIdSQL(bagisciId);
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
        public decimal SelectSumTahminiRayicByBagisciId(int bagisciId)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(TahminiRayicDegeri) Toplam
                FROM Bagis_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId AND B.EnvanterdeMi=1
                WHERE A.BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ToString().ConvertToDecimal();
                }
            }
            return toplam;
        }
        private string SelectTasinmazBagisByBagisciIdSQL(int bagisciId)
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id,B.BagisTarihi) AS Sirano,
                    A.Id TasinmazId,A.TahminiRayicDegeri, A.Cinsi, A.KullanimSekli, A.Adres,
                    A.MulkiyetSekli,A.KiraDurumu, A.EmlakBeyanDegeri,A.TahminiRayicDegeri,
                    B.Id BagisId,B.BagisYili, 
                    D.IlceAdi +'-'+C.IlAdi IlIlce                    
                FROM Bagis_Table B
                INNER JOIN Tasinmaz_Table A on A.Id=B.TasinmazId
                LEFT JOIN Il_Table C ON C.IlAdi=A.Ili
                LEFT JOIN Ilce_Table D ON D.IlceAdi=A.Ilcesi AND D.IlId=C.Id
                WHERE A.EnvanterdeMi=1 AND B.BagisciId={0}
                ORDER BY A.Id,B.BagisTarihi
                                    ", bagisciId);
            return sqlString;
        }
    }
}
