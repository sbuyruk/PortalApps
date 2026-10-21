using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.TBYS
{
    public class VasiyeteKonuVarlikRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public VasiyeteKonuVarlikRepository() : this(new DbClass()) { }

        public VasiyeteKonuVarlikRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = new TbysSqlExecutor(db, "VasiyeteKonuVarlikRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public VasiyeteKonuVarlikRepository(PortalLogContext context) { db = new TbysSqlExecutor(context, "VasiyeteKonuVarlikRepository"); queryBuilder = new CrudQueryBuilder(); }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM VasiyeteKonuVarlik_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table
                ORDER BY VasiyetciId"), "");
        }

        public DataTable SelectByVasiyetciId(int vasiyetciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table
                WHERE VasiyetciId=@VasiyetciId
                ORDER BY VasiyetciId");
            query.AddParameter("@VasiyetciId", vasiyetciId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "VasiyeteKonuVarlik_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "VasiyeteKonuVarlik_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("VasiyeteKonuVarlik_Table", id), "");
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { SqlQuery q = new SqlQuery("SELECT * FROM VasiyeteKonuVarlik_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, transaction, context); }
        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Insert(queryBuilder.BuildInsert(entity, "VasiyeteKonuVarlik_Table"), transaction, context); }
        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "VasiyeteKonuVarlik_Table"), transaction, context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return db.DeleteFromDb(queryBuilder.BuildDelete("VasiyeteKonuVarlik_Table", id), "", transaction, context); }
    }
}
