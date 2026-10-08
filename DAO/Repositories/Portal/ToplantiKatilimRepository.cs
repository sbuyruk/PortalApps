using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class ToplantiKatilimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder b;

        public ToplantiKatilimRepository()
            : this(new DbClass())
        {
        }

        public ToplantiKatilimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            b = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM ToplantiKatilim_Table"), "");
        }

        public DataTable SelectByParticipantMeeting(int k, int t)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE KatilimciId=@KatilimciId AND ToplantiId=@ToplantiId");
            q.AddParameter("@KatilimciId", k);
            q.AddParameter("@ToplantiId", t);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByMeeting(int t)
        {
            var q = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WHERE ToplantiId=@ToplantiId");
            q.AddParameter("@ToplantiId", t);
            return db.SelectFromDb(q, "");
        }

        public int Insert<T>(T x)
        {
            return db.Insert(b.BuildInsert(x, "ToplantiKatilim_Table"));
        }

        public bool Update<T>(T x)
        {
            return db.Update2Db(b.BuildUpdate(x, "ToplantiKatilim_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(b.BuildDelete("ToplantiKatilim_Table", id), "");
        }

        public bool DeleteByMeeting(int id)
        {
            var q = new SqlQuery("DELETE ToplantiKatilim_Table WHERE ToplantiId=@ToplantiId");
            q.AddParameter("@ToplantiId", id);
            return db.DeleteFromDb(q, "");
        }
    }
}
