using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class FTKIslemRepository
    {
        private const string TableName = "FTKIslem_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public FTKIslemRepository() : this(new DbClass()) { }

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
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByLocation(int provinceId, int districtId)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM FTKIslem_Table WHERE Ili=@Ili AND Ilcesi=@Ilcesi");
            query.AddParameter("@Ili", provinceId);
            query.AddParameter("@Ilcesi", districtId);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
    }
}
