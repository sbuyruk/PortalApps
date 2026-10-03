using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class KiraSozlesmeRepository
    {
        private readonly DbClass db;

        public KiraSozlesmeRepository() : this(new DbClass()) { }

        public KiraSozlesmeRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }

        public DataTable SelectAllActive()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE Aktif=1
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }
    }
}
