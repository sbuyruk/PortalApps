using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public class OrtakParametreRepository
    {
        private readonly DbClass db;

        public OrtakParametreRepository()
            : this(new DbClass())
        {
        }

        public OrtakParametreRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OrtakParametre_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM OrtakParametre_Table ORDER BY Sira, Deger"), "");
        }

        public DataTable SelectByKey(string key)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OrtakParametre_Table WHERE Anahtar=@Anahtar ORDER BY Sira, Deger");
            query.AddParameter("@Anahtar", key);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByKey(string key, Utility.Logging.PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OrtakParametre_Table WHERE Anahtar=@Anahtar ORDER BY Sira, Deger");
            query.AddParameter("@Anahtar", key);
            return PortalSqlReadExecutor.Select(query, context, "OrtakParametreRepository.SelectByKey");
        }

        public DataTable SelectByGroup(string group)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OrtakParametre_Table WHERE Grup=@Grup ORDER BY Sira, Deger");
            query.AddParameter("@Grup", group);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAllData()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM OrtakParametre_Table ORDER BY Sira, Deger"), "");
        }

        public DataTable SelectByGroupValue(string group, string value)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM OrtakParametre_Table WHERE Grup=@Grup AND Deger=@Deger ORDER BY Sira, Deger");
            query.AddParameter("@Grup", group);
            query.AddParameter("@Deger", value);
            return db.SelectFromDb(query, "");
        }

    }
}
