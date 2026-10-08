using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class DuyuruOkumaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder b;

        public DuyuruOkumaRepository()
            : this(new DbClass())
        {
        }

        public DuyuruOkumaRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            b = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruOkuma_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM DuyuruOkuma_Table"), "");
        }

        public DataTable SelectByDuyuruId(int id)
        {
            var q = new SqlQuery("SELECT A.Id,B.Adi,B.Soyadi,A.OkumaTarihi FROM DuyuruOkuma_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId WHERE A.DuyuruId=@DuyuruId ORDER BY OkumaTarihi DESC");
            q.AddParameter("@DuyuruId", id);
            return db.SelectFromDb(q, "");
        }

        public int Insert<T>(T x)
        {
            return db.Insert(b.BuildInsert(x, "DuyuruOkuma_Table"));
        }

        public bool Update<T>(T x)
        {
            return db.Update2Db(b.BuildUpdate(x, "DuyuruOkuma_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(b.BuildDelete("DuyuruOkuma_Table", id), "");
        }
    }
}
















































































































n
