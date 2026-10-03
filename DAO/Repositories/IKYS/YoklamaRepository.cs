using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class YoklamaRepository
    {
        private readonly DbClass db;

        public YoklamaRepository() : this(new DbClass()) { }

        public YoklamaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

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
    }
}
