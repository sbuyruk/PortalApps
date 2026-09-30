using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class BankaTanimRepository
    {
        private const string TableName = "BankaTanim_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public BankaTanimRepository() : this(new DbClass())
        {
        }

        public BankaTanimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM BankaTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM BankaTanim_Table ORDER BY Banka"), "");
        }

        public DataTable SelectByName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM BankaTanim_Table WHERE Banka=@Banka");
            query.AddParameter("@Banka", name);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGroups()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT BankaGrup FROM BankaTanim_Table GROUP BY BankaGrup"), "");
        }

        public DataTable SelectGroups2()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT BankaGrup2 FROM BankaTanim_Table GROUP BY BankaGrup2"), "");
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
