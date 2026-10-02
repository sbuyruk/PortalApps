using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class IlRepository
    {
        private readonly DbClass db;

        public IlRepository() : this(new DbClass())
        {
        }

        public IlRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Il_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Il_Table"), "");
        }

        public DataTable SelectByName(string name)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Il_Table WHERE LOWER(IlAdi)=LOWER(@IlAdi)");
            query.AddParameter("@IlAdi", name);
            return db.SelectFromDb(query, "");
        }
    }
}
