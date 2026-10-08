using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class KiraBorcuTakipRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public KiraBorcuTakipRepository() : this(new DbClass()) { }
        public KiraBorcuTakipRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM KiraBorcuTakip_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM KiraBorcuTakip_Table ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), ""); }
        public DataTable SelectByKiraciIdAyYil(int kiraciId, int ay, int yil) { SqlQuery q = new SqlQuery("SELECT * FROM KiraBorcuTakip_Table WHERE KiraciId=@KiraciId AND IslemAyi=@IslemAyi AND IslemYili=@IslemYili"); q.AddParameter("@KiraciId", kiraciId); q.AddParameter("@IslemAyi", ay); q.AddParameter("@IslemYili", yil); return db.SelectFromDb(q, ""); }
        public DataTable SelectCountByFilters(string takipIslemi, string bolge, int ay, int yil)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(Id) Adet FROM KiraBorcuTakip_Table WHERE 1=1 ");
            SqlQuery q = new SqlQuery();
            if (!string.IsNullOrEmpty(takipIslemi)) { sql.Append(" AND TakipIslemi=@TakipIslemi "); q.AddParameter("@TakipIslemi", takipIslemi); }
            if (!string.IsNullOrEmpty(bolge)) { sql.Append(" AND Bolge=@Bolge "); q.AddParameter("@Bolge", bolge); }
            if (ay >= 1) { sql.Append(" AND IslemAyi=@IslemAyi "); q.AddParameter("@IslemAyi", ay); }
            if (yil >= 2005) { sql.Append(" AND IslemYili=@IslemYili "); q.AddParameter("@IslemYili", yil); }
            q.Sql = sql.ToString(); return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountBySozlesmeId(int kiraSozlesmeId, string takipIslemi)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(Id) Adet FROM KiraBorcuTakip_Table WHERE KiraSozlesmeId=@KiraSozlesmeId ");
            SqlQuery q = new SqlQuery(); q.AddParameter("@KiraSozlesmeId", kiraSozlesmeId);
            if (!string.IsNullOrEmpty(takipIslemi)) { sql.Append(" AND TakipIslemi=@TakipIslemi "); q.AddParameter("@TakipIslemi", takipIslemi); }
            q.Sql = sql.ToString(); return db.SelectFromDb(q, "");
        }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "KiraBorcuTakip_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "KiraBorcuTakip_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("KiraBorcuTakip_Table", id), ""); }
    }
}
