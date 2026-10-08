using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.MTS
{
    public class FaaliyetKatilimRepository
    {
        private const string TableName = "FaaliyetKatilim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public FaaliyetKatilimRepository() : this(new DbClass()) { }
        public FaaliyetKatilimRepository(DbClass db)
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

        public DataTable SelectByFaaliyetAndKatilimci(int faaliyetId, int katilimciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM FaaliyetKatilim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByKatilimciId(int katilimciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE KatilimciId = @KatilimciId");
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return db.SelectFromDb(query, "");
        }
    }
}
