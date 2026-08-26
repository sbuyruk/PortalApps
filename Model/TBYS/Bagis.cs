using DAO.Ortak;
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
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Bagis_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            DataTable dataTable = dao.SelectFromDb(query, "");
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
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                    Bagis item = Select<Bagis>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Bagis> genericEntity = new GenericEntity<Bagis>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Bagis item = Select<Bagis>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Bagis_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Bagis> list = ToList<Bagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Bagis> SelectByBagisciId(int bagisciId)
        {

            SqlQuery query = new SqlQuery(@"SELECT * FROM Bagis_Table
                              WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Bagis> list = ToList<Bagis>(dataTable);
            return list;
        }
        public DataTable SelectByBagisciIdGroupByKullanimSekli(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.KullanimSekli,COUNT(A.Id) Adet,B.Ili, SUM(B.TahminiRayicDegeri) TahminiRayic
                FROM Bagis_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                WHERE A.BagisciId=@BagisciId
                GROUP BY B.KullanimSekli,B.Ili
            ");
            query.AddParameter("@BagisciId", bagisciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            return dataTable;
        }
		public string SelectByBagisciIdReturnJson(int bagisciId)
		{
			SqlQuery query = new SqlQuery(@"
			SELECT 
				A.Id BagisciId, A.Adi, A.Soyadi,
				C.Id TasinmazId, C.Adres
			FROM TasinmazBagisci_Table A
				INNER JOIN  Bagis_Table B ON B.BagisciId=A.Id 
				INNER JOIN Tasinmaz_Table C ON C.Id=B.TasinmazId AND C.EnvanterdeMi=1
				WHERE B.BagisciId=@BagisciId");
			query.AddParameter("@BagisciId", bagisciId);
			DataTable dataTable = dao.SelectFromDb(query, "");
			string json = ToJSON(dataTable);
			return json;
		}
		public Bagis SelectByTasinmazId(int tasinmazId)
		{
			SqlQuery query = new SqlQuery(@"SELECT * FROM Bagis_Table
							  WHERE TasinmazId=@TasinmazId");
			query.AddParameter("@TasinmazId", tasinmazId);
			DataTable dataTable = dao.SelectFromDb(query, "");
			List<Bagis> list = ToList<Bagis>(dataTable);
			Bagis bagis = new Bagis();
			bagis = list.FirstOrDefault();
			return bagis;
		}
        public string SelectTasinmazByBagisciIdReturnJson(int bagisciId)
        {
            SqlQuery query = SelectTasinmazBagisByBagisciIdSQL(bagisciId);
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
        public DataTable SelectTasinmazByBagisciIdReturnDT(int bagisciId)
        {
            SqlQuery query = SelectTasinmazBagisByBagisciIdSQL(bagisciId);
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
        public DataTable SelectSatisVsDahilTasinmazByBagisciIdReturnDT(int bagisciId)
        {
            SqlQuery query = SelectSatisVsDahilTasinmazByBagisciIdSQL(bagisciId);
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
        private SqlQuery SelectSatisVsDahilTasinmazByBagisciIdSQL(int bagisciId)
        {
            string[] sebepler = Tasinmaz.SatisVsDahilEnvanterdenCikmaSebepleri;
            string[] sebepParamAdlari = sebepler
                .Select((sebep, index) => "@Sebep" + index)
                .ToArray();
            string sebepInClause = string.Join(",", sebepParamAdlari);

            SqlQuery query = new SqlQuery($@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id,B.BagisTarihi) AS Sirano,
                    A.Id TasinmazId,A.TahminiRayicDegeri, A.Cinsi, A.KullanimSekli, A.Adres,
                    A.MulkiyetSekli,A.KiraDurumu, A.EmlakBeyanDegeri,A.TahminiRayicDegeri,A.EnvanterdeMi,A.EnvanterdenCikmaSebebi,
                    B.Id BagisId,B.BagisYili, 
                    D.IlceAdi +'-'+C.IlAdi IlIlce                    
                FROM Bagis_Table B
                INNER JOIN Tasinmaz_Table A on A.Id=B.TasinmazId
                LEFT JOIN Il_Table C ON C.IlAdi=A.Ili
                LEFT JOIN Ilce_Table D ON D.IlceAdi=A.Ilcesi AND D.IlId=C.Id
                WHERE B.BagisciId=@BagisciId
                    AND (
                        A.EnvanterdeMi=1
                        OR (A.EnvanterdeMi=0 AND A.EnvanterdenCikmaSebebi IN ({sebepInClause}))
                    )
                ORDER BY A.Id,B.BagisTarihi
                                    ");
            query.AddParameter("@BagisciId", bagisciId);
            for (int i = 0; i < sebepler.Length; i++)
            {
                query.AddParameter(sebepParamAdlari[i], sebepler[i]);
            }
            return query;
        }
        public decimal SelectSumTahminiRayicByBagisciId(int bagisciId)
        {
            decimal toplam = 0;
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(TahminiRayicDegeri) Toplam
                FROM Bagis_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId AND B.EnvanterdeMi=1
                WHERE A.BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
        private SqlQuery SelectTasinmazBagisByBagisciIdSQL(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id,B.BagisTarihi) AS Sirano,
                    A.Id TasinmazId,A.TahminiRayicDegeri, A.Cinsi, A.KullanimSekli, A.Adres,
                    A.MulkiyetSekli,A.KiraDurumu, A.EmlakBeyanDegeri,A.TahminiRayicDegeri,
                    B.Id BagisId,B.BagisYili, 
                    D.IlceAdi +'-'+C.IlAdi IlIlce                    
                FROM Bagis_Table B
                INNER JOIN Tasinmaz_Table A on A.Id=B.TasinmazId
                LEFT JOIN Il_Table C ON C.IlAdi=A.Ili
                LEFT JOIN Ilce_Table D ON D.IlceAdi=A.Ilcesi AND D.IlId=C.Id
                WHERE A.EnvanterdeMi=1 AND B.BagisciId=@BagisciId
                ORDER BY A.Id,B.BagisTarihi
                                    ");
            query.AddParameter("@BagisciId", bagisciId);
            return query;
        }
    }
}
