using DAO.Ortak;
using Utility.Logging;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class SerhBeyanIrtifakRepository
    {
        private readonly TbysSqlExecutor db;
        private readonly CrudQueryBuilder queryBuilder;

        public SerhBeyanIrtifakRepository() : this(new DbClass()) { }

        public SerhBeyanIrtifakRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = new TbysSqlExecutor(db, "SerhBeyanIrtifakRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public SerhBeyanIrtifakRepository(PortalLogContext context)
        {
            db = new TbysSqlExecutor(context, "SerhBeyanIrtifakRepository");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SerhBeyanIrtifak_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM SerhBeyanIrtifak_Table"), "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM SerhBeyanIrtifak_Table
                WHERE TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "SerhBeyanIrtifak_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "SerhBeyanIrtifak_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("SerhBeyanIrtifak_Table", id), "");
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM SerhBeyanIrtifak_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, transaction, context);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "SerhBeyanIrtifak_Table"), transaction, context);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "SerhBeyanIrtifak_Table"), transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("SerhBeyanIrtifak_Table", id), "", transaction, context);
        }
    }
}
