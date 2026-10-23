using System.Runtime.CompilerServices;
using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class MTSKurumGorevRepository
    {
        private readonly DbClass db;
        private readonly PortalLogContext context;
        private readonly CrudQueryBuilder queryBuilder;
        public MTSKurumGorevRepository() : this(new DbClass())
        {
        }

        public MTSKurumGorevRepository(PortalLogContext context) : this(new DbClass())
        {
            if (context == null || context.Project != "MTS") throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
        }

        private DataTable Read(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            return context == null ? db.SelectFromDb(query, identifier)
                : PortalSqlReadExecutor.Select(query, context, "MTSKurumGorevRepository." + method);
        }

        private void RequireLegacyWrite()
        {
            if (context != null) throw new InvalidOperationException("MTS yazma işlemi transaction hizmeti üzerinden yapılmalıdır.");
        }

        public MTSKurumGorevRepository(DbClass db)
        {
            this.db = db ?? throw new ArgumentNullException("db");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM MTSKurumGorev_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Read(query, "");
        }

        public DataTable SelectAll()
        {
            return Read(new SqlQuery("SELECT * FROM MTSKurumGorev_Table ORDER BY Id"), "");
        }
        public DataTable SelectByKisiId(int kisiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT B.Adi Kurum, C.Adi Gorev
                FROM MTSKurumGorev_Table A
                LEFT JOIN MTSKurumTanim_Table B ON B.Id = A.MTSKurumTanimId
                LEFT JOIN MTSGorevTanim_Table C ON C.Id = A.MTSGorevTanimId
                WHERE A.KisiId = @KisiId AND A.Durum = @Durum");
            query.AddParameter("@KisiId", kisiId);
            query.AddParameter("@Durum", ProjeConstants.MTSGOREVDURUMU_GOREVDE);
            return Read(query, "");
        }

        public int Insert<T>(T item)
        { RequireLegacyWrite();
            return db.Insert(queryBuilder.BuildInsert(item, "MTSKurumGorev_Table"));
        }

        public bool Update<T>(T item)
        { RequireLegacyWrite();
            return db.Update2Db(queryBuilder.BuildUpdate(item, "MTSKurumGorev_Table"));
        }

        public bool Delete(int id)
        { RequireLegacyWrite();
            return db.DeleteFromDb(queryBuilder.BuildDelete("MTSKurumGorev_Table", id), "");
        }
    }
}
