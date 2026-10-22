using DAO.Ortak;
using Utility.Logging;
using System.Runtime.CompilerServices;
using System;
using System.Data;

namespace DAO.Repositories.Portal
{
    public class DuyuruGosterimRepository
    {
        private readonly PortalSqlExecutor sql;
        public DuyuruGosterimRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "DuyuruGosterimRepository");
        }
        private DataTable Select(SqlQuery query, [CallerMemberName] string method = "")
        {
            return sql == null ? db.SelectFromDb(query, "") : sql.Select(query, method: method);
        }
        private void RequireLegacyWrite()
        {
            if (sql != null) throw new InvalidOperationException("Duyuru yazmaları transaction servisi üzerinden yapılmalıdır.");
        }
        private readonly DbClass db;
        private readonly CrudQueryBuilder b;

        public DuyuruGosterimRepository()
            : this(new DbClass())
        {
        }

        public DuyuruGosterimRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            b = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruGosterim_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM DuyuruGosterim_Table"));
        }

        public DataTable SelectByDuyuruId(int id)
        {
            var q = new SqlQuery("SELECT * FROM DuyuruGosterim_Table WHERE DuyuruId=@DuyuruId ORDER BY Id DESC");
            q.AddParameter("@DuyuruId", id);
            return Select(q);
        }

        public int Insert<T>(T x)
        {
            RequireLegacyWrite();
            return db.Insert(b.BuildInsert(x, "DuyuruGosterim_Table"));
        }

        public bool Update<T>(T x)
        {
            RequireLegacyWrite();
            return db.Update2Db(b.BuildUpdate(x, "DuyuruGosterim_Table"));
        }

        public bool Delete(int id)
        {
            RequireLegacyWrite();
            return db.DeleteFromDb(b.BuildDelete("DuyuruGosterim_Table", id), "");
        }
    }
}

