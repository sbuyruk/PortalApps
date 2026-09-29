using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NBYSParametreRepository
    {
        private const string TableName = "NBYSParametre_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public NBYSParametreRepository() : this(new DbClass())
        {
        }

        public NBYSParametreRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NBYSParametre_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM NBYSParametre_Table ORDER BY Sira,Deger"), "");
        }

        public DataTable SelectByGroup(string group)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup ORDER BY Sira,Deger");
            query.AddParameter("@Grup", group);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByGroupAndKey(string group, string key)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup AND Anahtar=@Anahtar ORDER BY Sira,Anahtar");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Anahtar", key);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByGroupKeyAndValue(string group, string key, string value)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM NBYSParametre_Table
                WHERE Grup=@Grup AND Anahtar=@Anahtar AND Deger=@Deger
                ORDER BY Sira,Anahtar");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Anahtar", key);
            query.AddParameter("@Deger", value);
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
