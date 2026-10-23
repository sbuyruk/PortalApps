using System.Runtime.CompilerServices;
using Utility.Logging;
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
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;

        public AramaGorusmeRepository() : this(new DbClass()) { }
        public AramaGorusmeRepository(PortalLogContext context) : this(new DbClass())
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
        }

        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, identifier)
                : PortalSqlReadExecutor.Select(query, context, "AramaGorusmeRepository." + method);
        }

        private void RequireLegacyWrite()
        {
            if (context != null) throw new InvalidOperationException("MTS yazma işlemi transaction hizmeti üzerinden yapılmalıdır.");
        }

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
            return Read(query, "");
        }
        public DataTable SelectAll() { return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), ""); }
        public int Insert<T>(T entity) { RequireLegacyWrite(); return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { RequireLegacyWrite(); return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { RequireLegacyWrite(); return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return Read(query, "");
        }

        public DataTable SelectByArayanId(int arayanId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE ArayanId = @ArayanId");
            query.AddParameter("@ArayanId", arayanId);
            return Read(query, "");
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
            return Read(query, "");
        }
    }
}
