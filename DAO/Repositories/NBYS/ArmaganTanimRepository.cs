using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class ArmaganTanimRepository
    {
        private const string TableName = "ArmaganTanim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public ArmaganTanimRepository() : this(new DbClass())
        {
        }

        public ArmaganTanimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM ArmaganTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM ArmaganTanim_Table"), "");
        }

        public DataTable SelectActive()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM ArmaganTanim_Table WHERE Aktif=1"), "");
        }

        public DataTable SelectByAmount(decimal amount, bool isCorporate)
        {
            string limits = isCorporate
                ? "TuzelKisiAltLimit<=@Amount AND TuzelKisiUstLimit>=@Amount"
                : "OzelKisiAltLimit<=@Amount AND OzelKisiUstLimit>=@Amount";
            SqlQuery query = new SqlQuery(
                "SELECT * FROM ArmaganTanim_Table WHERE Aktif=1 AND " + limits);
            query.AddParameter("@Amount", amount);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }
    }
}
