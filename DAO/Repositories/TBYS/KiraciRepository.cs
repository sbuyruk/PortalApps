using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class KiraciRepository
    {
        private readonly DbClass db;

        public KiraciRepository() : this(new DbClass()) { }

        public KiraciRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kiraci_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Kiraci_Table"), "");
        }

        public DataTable SelectActiveTenants()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT S.DosyaNo,A.*
                FROM Kiraci_Table A
                INNER JOIN KiraSozlesme_Table S ON S.KiraciId=A.Id AND S.Aktif=1
                INNER JOIN OdemePlani_Table O ON O.Id=(SELECT TOP 1 Id FROM OdemePlani_Table WHERE SozlesmeId=S.Id)
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id"), "");
        }
    }
}
