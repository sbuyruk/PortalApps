using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class MTSUnvanTanimRepository
    {
        private const string TableName = "MTSUnvanTanim_Table";
        private readonly DbClass db;
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;

        public MTSUnvanTanimRepository() : this(new DbClass()) { }

        public MTSUnvanTanimRepository(PortalLogContext context) : this(new DbClass())
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
        }
        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, identifier)
                : PortalSqlReadExecutor.Select(query, context, "MTSUnvanTanimRepository." + method);
        }
        private void RequireLegacyWrite()
        {
            if (context != null) throw new InvalidOperationException("MTS yazma işlemi transaction hizmeti üzerinden yapılmalıdır.");
        }

        public MTSUnvanTanimRepository(DbClass db)
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
    }
}
