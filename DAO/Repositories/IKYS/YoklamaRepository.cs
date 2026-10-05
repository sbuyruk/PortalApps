using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class YoklamaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public YoklamaRepository() : this(new DbClass()) { }

        public YoklamaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Yoklama_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Yoklama_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Yoklama_Table", id), ""); }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Yoklama_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Yoklama_Table"), "");
        }

        public DataTable SelectByPersonelIdAndDate(int personelId, DateTime start, DateTime end)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Yoklama_Table
                WHERE PersonelId=@PersonelId
                  AND BulunmamaSebebi=3
                  AND BitisTarihi >= @StartDate
                  AND BaslangicTarihi <= @EndDate");
            query.AddParameter("@PersonelId", personelId);
            query.AddParameter("@StartDate", start);
            query.AddParameter("@EndDate", end);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllByPersonelId(int personelId)
        {
            string personelFilter = personelId > 0 ? " WHERE A.PersonelId=@PersonelId" : "";
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id YoklamaId, P.Adi+' '+P.Soyadi AdiSoyadi,
                       B.Adi BulunmamaSebebi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Aciklama
                FROM Yoklama_Table A
                INNER JOIN Personel_Table P ON A.PersonelId=P.Id
                INNER JOIN BulunmamaSebebi_Table B ON B.Id=A.BulunmamaSebebi
                " + personelFilter + @"
                ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC");
            if (personelId > 0)
                query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTarih(DateTime start, DateTime end)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, A.Aciklama,
                    A.PersonelId, A.BaslangicTarihi, A.BitisTarihi,A.BulunmamaSebebi BulunmamaSebebiId ,C.Adi BulunmamaSebebi,C.Id BulunmamaSebebiInt,
                    D.BirimId,E.KisaAdi GorevYeri
                FROM Yoklama_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN BulunmamaSebebi_Table C ON C.Id= A.BulunmamaSebebi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE A.BaslangicTarihi<=@StartDate AND A.BitisTarihi>=@EndDate
                ORDER BY D.ProtokolSiraNo,A.BaslangicTarihi");
            query.AddParameter("@StartDate", start); query.AddParameter("@EndDate", end);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTarihAndReasons(string reasonIds, DateTime start, DateTime end)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, A.Aciklama,
                    A.BaslangicTarihi, A.BitisTarihi,A.BulunmamaSebebi BulunmamaSebebiId ,C.Adi BulunmamaSebebi,C.Id BulunmamaSebebiInt,
                    D.BirimId,E.KisaAdi GorevYeri,D.ProtokolSiraNo
                FROM Yoklama_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN BulunmamaSebebi_Table C ON C.Id= A.BulunmamaSebebi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE A.BulunmamaSebebi in (" + reasonIds + @") AND A.BaslangicTarihi BETWEEN @StartDate AND @EndDate
                ORDER BY D.ProtokolSiraNo,A.BaslangicTarihi");
            query.AddParameter("@StartDate", start); query.AddParameter("@EndDate", end);
            return db.SelectFromDb(query, "");
        }
    }
}
