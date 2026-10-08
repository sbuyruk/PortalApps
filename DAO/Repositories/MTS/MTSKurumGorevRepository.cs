using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class MTSKurumGorevRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public MTSKurumGorevRepository() : this(new DbClass())
        {
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
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM MTSKurumGorev_Table ORDER BY Id"), "");
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
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T item)
        {
            return db.Insert(queryBuilder.BuildInsert(item, "MTSKurumGorev_Table"));
        }

        public bool Update<T>(T item)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(item, "MTSKurumGorev_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("MTSKurumGorev_Table", id), "");
        }
    }
}
