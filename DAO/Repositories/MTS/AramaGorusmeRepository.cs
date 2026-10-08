using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class AramaGorusmeRepository
    {
        private const string TableName = "AramaGorusme_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public AramaGorusmeRepository() : this(new DbClass()) { }
        public AramaGorusmeRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + TableName + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByArayanId(int arayanId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE ArayanId = @ArayanId");
            query.AddParameter("@ArayanId", arayanId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilter(int arayanId, string gorusmeSekli, DateTime basTar, DateTime bitTar, DateTime referansTarihi, string gorevde)
        {
            List<string> filters = new List<string>();
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id AramaId, C.Id ArayanId, A.GorusmeSekli, A.Tarih, A.Konu,
                       A.GorusmeSaglandi, A.RandevuIstendi, A.FaaliyetId,
                       C.Adi, C.Soyadi, H.Adi Kurumu, C.RandevuKisiti
                FROM AramaGorusme_Table A
                LEFT JOIN Kisi_Table C ON C.Id = A.ArayanId
                LEFT JOIN MTSKurumGorev_Table G ON G.KisiId = C.Id AND G.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table H ON H.Id = G.MTSKurumTanimId");
            query.AddParameter("@Gorevde", gorevde);
            if (arayanId > 0)
            {
                filters.Add("A.ArayanId = @ArayanId");
                query.AddParameter("@ArayanId", arayanId);
            }
            if (!string.IsNullOrEmpty(gorusmeSekli))
            {
                filters.Add("A.GorusmeSekli = @GorusmeSekli");
                query.AddParameter("@GorusmeSekli", gorusmeSekli);
            }
            if (basTar >= referansTarihi)
            {
                filters.Add("A.Tarih BETWEEN @BasTar AND @BitTar");
                query.AddParameter("@BasTar", basTar);
                query.AddParameter("@BitTar", bitTar);
            }
            if (filters.Count > 0)
                query.Sql += " WHERE " + string.Join(" AND ", filters);
            query.Sql += " ORDER BY A.Tarih DESC";
            return db.SelectFromDb(query, "");
        }
    }
}
