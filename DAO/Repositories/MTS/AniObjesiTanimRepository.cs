using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class AniObjesiTanimRepository
    {
        private const string TableName = "AniObjesiTanim_Table";
        private readonly DbClass db;
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;

        public AniObjesiTanimRepository() : this(new DbClass()) { }

        public AniObjesiTanimRepository(PortalLogContext context) : this(new DbClass())
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
        }
        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, identifier)
                : PortalSqlReadExecutor.Select(query, context, "AniObjesiTanimRepository." + method);
        }
        private void RequireLegacyWrite()
        {
            if (context != null) throw new InvalidOperationException("MTS yazma işlemi transaction hizmeti üzerinden yapılmalıdır.");
        }

        public AniObjesiTanimRepository(DbClass db)
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

        public DataTable SelectStokluAniObjeleri(string stokluMu)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id AniObjesiId, A.Adi, SUM(B.SonAdet) Toplam
                FROM AniObjesiTanim_Table A
                INNER JOIN DepoStok_Table B ON B.AniObjesiId = A.Id AND B.SonAdet > 0
                WHERE A.StokluMu = @StokluMu
                GROUP BY A.Id, A.Adi
                ORDER BY A.Adi");
            query.AddParameter("@StokluMu", stokluMu);
            return Read(query, "");
        }
    }
}
