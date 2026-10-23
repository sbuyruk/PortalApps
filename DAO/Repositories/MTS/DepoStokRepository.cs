using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class DepoStokRepository
    {
        private const string TableName = "DepoStok_Table";
        private readonly DbClass db;
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;

        public DepoStokRepository() : this(new DbClass()) { }

        public DepoStokRepository(PortalLogContext context) : this(new DbClass())
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
        }
        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, identifier)
                : PortalSqlReadExecutor.Select(query, context, "DepoStokRepository." + method);
        }
        private void RequireLegacyWrite()
        {
            if (context != null) throw new InvalidOperationException("MTS yazma işlemi transaction hizmeti üzerinden yapılmalıdır.");
        }

        public DepoStokRepository(DbClass db)
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

        public DataTable SelectAll()
        {
            return Read(new SqlQuery("SELECT * FROM " + TableName + " ORDER BY Id"), "");
        }

        public int Insert<T>(T entity) { RequireLegacyWrite(); return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { RequireLegacyWrite(); return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { RequireLegacyWrite(); return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }

        public DataTable SelectDepoStok(int depoId, int aniObjesiId, string stokluMu)
        {
            string depoFilter = depoId > 0 ? " AND A.DepoId = @DepoId" : "";
            string aniObjesiFilter = aniObjesiId > 0 ? " AND B.Id = @AniObjesiId" : "";
            string stokluFilter = string.IsNullOrEmpty(stokluMu) ? "" : " AND B.StokluMu = @StokluMu";
            SqlQuery query = new SqlQuery(@"
                SELECT A.*
                FROM DepoStok_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id = A.AniObjesiId
                WHERE A.SonAdet > 0" + depoFilter + aniObjesiFilter + stokluFilter + @"
                ORDER BY A.Id");
            if (depoId > 0) query.AddParameter("@DepoId", depoId);
            if (aniObjesiId > 0) query.AddParameter("@AniObjesiId", aniObjesiId);
            if (!string.IsNullOrEmpty(stokluMu)) query.AddParameter("@StokluMu", stokluMu);
            return Read(query, "");
        }
    }
}
