using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TasinmazTaahhutRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public TasinmazTaahhutRepository() : this(new DbClass()) { }

        public TasinmazTaahhutRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM TasinmazTaahhut_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM TasinmazTaahhut_Table"), "");
        }

        public DataTable SelectByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM TasinmazTaahhut_Table WHERE BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTcKimlikNo(long tcKimlikNo)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM TasinmazTaahhut_Table
                WHERE TCKimlikNo > 0 AND TCKimlikNo=@TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikNo);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilters(bool isSagVefat, bool isTCKimlikNoFull, bool isDogumTarihiFull, int bolgeId, string sagValue, int allRegionId, int headquartersRegionId)
        {
            string tcKimlikNoFilter = isTCKimlikNoFull ? " AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0" : string.Empty;
            string dogumTarihiFilter = isDogumTarihiFull ? " AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900'" : string.Empty;
            string sagVefatFilter = isSagVefat ? " AND Sag_vefat=@SagVefat" : string.Empty;
            string bolgeFilter = bolgeId == allRegionId || bolgeId == headquartersRegionId ? string.Empty : " AND BolgeId=@BolgeId";
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM TasinmazTaahhut_Table A
                LEFT JOIN Il_Table B ON B.Id = A.Ili
                WHERE 1>0" + tcKimlikNoFilter + dogumTarihiFilter + sagVefatFilter + bolgeFilter);

            if (isSagVefat)
                query.AddParameter("@SagVefat", sagValue);
            if (!string.IsNullOrEmpty(bolgeFilter))
                query.AddParameter("@BolgeId", bolgeId);

            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIlAdi(string ilAdi)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM TasinmazTaahhut_Table WHERE Ili=@Ili");
            query.AddParameter("@Ili", ilAdi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllCountDonationAsJson()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, Count(C.Id) ToplamBagisAdedi,
                    A.Id TasinmazTaahhutId, E.Bolge,
                    A.Adi+' '+ A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence,
                    A.Ilcesi +'-'+A.Ili IlIlce, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, FORMAT(A.vefatTarihi,'dd.MM.yyyy') VefatTarihi
                FROM TasinmazTaahhut_Table A
                LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
                LEFT JOIN Il_Table E on E.IlAdi=A.Ili
                GROUP BY C.BagisciId,
                    A.Id, A.Adi, A.Soyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence,
                    A.Ili, A.Ilcesi, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,E.Bolge,A.vefatTarihi"), "");
        }

        public DataTable SelectAllCountDonation(bool excludeDeceased, bool excludeHidden, string sagValue)
        {
            string sagFilter = excludeDeceased ? " WHERE SAG_VEFAT=@SagVefat" : string.Empty;
            string hiddenFilter = excludeHidden ? (excludeDeceased ? " AND " : " WHERE ") + " (Gizli IS NULL OR Gizli=0)" : string.Empty;
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano, A.Id TasinmazTaahhutId, Count(C.Id) ToplamBagisAdedi, SUM(D.TahminiRayicDegeri) ToplamTahminiRayic,
                    A.Id TasinmazTaahhutId, E.Bolge,
                    A.Adi+' '+A.Soyadi AdiSoyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence,
                    A.Ilcesi, A.Ili, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat, A.Gizli
                FROM TasinmazTaahhut_Table A
                LEFT JOIN Bagis_Table C on C.BagisciId=A.Id AND C.Envanterde=1
                LEFT JOIN Tasinmaz_Table D on D.Id=C.TasinmazId AND D.EnvanterdeMi=1
                LEFT JOIN Il_Table E on E.IlAdi=A.Ili
                " + sagFilter + hiddenFilter + @"
                GROUP BY C.BagisciId,
                    A.Id, A.Adi, A.Soyadi, A.TCKimlikNo, A.DogumYeri, A.DogumTarihi, A.Meslegi, A.SosyalGuvence,
                    A.Ili, A.Ilcesi, A.Adres, A.Telefon1, A.Telefon2, A.Foto, A.Sag_vefat,E.Bolge,A.Gizli");

            if (excludeDeceased)
                query.AddParameter("@SagVefat", sagValue);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "TasinmazTaahhut_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "TasinmazTaahhut_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("TasinmazTaahhut_Table", id), "");
        }
    }
}
