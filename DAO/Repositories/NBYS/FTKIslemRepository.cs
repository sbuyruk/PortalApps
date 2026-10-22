using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKIslemRepository
    {
        private const string TableName = "FTKIslem_Table";
        private readonly DbClass db;
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;

        public FTKIslemRepository() : this(new DbClass()) { }

        public FTKIslemRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            this.context = context;
            queryBuilder = new CrudQueryBuilder();
        }

        public FTKIslemRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FTKIslem_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query);
        }

        public DataTable SelectByLocation(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKIslem_Table WHERE Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return Select(query);
        }

        public int Insert<T>(T entity) { return LegacyWriteDb.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return LegacyWriteDb.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return LegacyWriteDb.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
        private DataTable Select(SqlQuery query,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(context, "FTKIslemRepository").Select(query, method: method);
        }

        private DbClass LegacyWriteDb
        {
            get
            {
                if (context != null) throw new InvalidOperationException("FTK yazmaları transaction destekli işlem servisini kullanmalıdır.");
                return db;
            }
        }
    }
}
