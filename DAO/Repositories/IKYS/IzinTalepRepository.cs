using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class IzinTalepRepository
    {
        private readonly DbClass db;

        public IzinTalepRepository() : this(new DbClass()) { }

        public IzinTalepRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IzinTalep_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM IzinTalep_Table"), "");
        }
    }
}
