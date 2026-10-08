using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class KiraEkstreAktarmaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public KiraEkstreAktarmaRepository() : this(new DbClass()) { }
        public KiraEkstreAktarmaRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM KiraEkstreAktarma_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM KiraEkstreAktarma_Table"), ""); }
        public DataTable SelectByKiraciAdi(string adi) { SqlQuery q = new SqlQuery("SELECT * FROM KiraEkstreAktarma_Table WHERE KiraciId > 0 AND Adi=@Adi"); q.AddParameter("@Adi", adi); return db.SelectFromDb(q, ""); }
        public DataTable SelectByIslemNo(string islemNo) { SqlQuery q = new SqlQuery("SELECT * FROM KiraEkstreAktarma_Table WHERE IslemNo=@IslemNo"); q.AddParameter("@IslemNo", islemNo); return db.SelectFromDb(q, ""); }
        public DataTable SelectByColumns(string adi, string soyadi, decimal tutar, DateTime odemeTarihi) { SqlQuery q = new SqlQuery("SELECT * FROM KiraEkstreAktarma_Table WHERE Adi=@Adi AND Soyadi=@Soyadi AND Tutar=@Tutar AND OdemeTarihi=@OdemeTarihi"); q.AddParameter("@Adi", adi); q.AddParameter("@Soyadi", soyadi); q.AddParameter("@Tutar", tutar); q.AddParameter("@OdemeTarihi", odemeTarihi); return db.SelectFromDb(q, ""); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "KiraEkstreAktarma_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "KiraEkstreAktarma_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("KiraEkstreAktarma_Table", id), ""); }
    }
}
