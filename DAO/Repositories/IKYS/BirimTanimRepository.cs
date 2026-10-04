using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class BirimTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public BirimTanimRepository() : this(new DbClass()) { }
        public BirimTanimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BirimTanim_Table ORDER BY ParentId,Sira"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BirimTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BirimTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BirimTanim_Table", id), ""); }
        public DataTable SelectByAmirId(int amirId) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE AmirId=@AmirId ORDER BY ParentId,Sira"); q.AddParameter("@AmirId", amirId); return db.SelectFromDb(q, ""); }
        public DataTable SelectByBirimKaldirildi(bool kaldirildi) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE BirimKaldirildi=@Kaldirildi ORDER BY ParentId,Sira"); q.AddParameter("@Kaldirildi", kaldirildi); return db.SelectFromDb(q, ""); }
        public DataTable SelectAllReturnDataTable() { return db.SelectFromDb(new SqlQuery("SELECT A.Id BirimId,A.Adi BirimAdi,A.KisaAdi BirimKisaAdi,A.Sira,B.Adi UstBirim,ISNULL(C.Adi,'')+' '+ISNULL(C.Soyadi,'') BirimAmiri FROM BirimTanim_Table A LEFT JOIN BirimTanim_Table B ON B.Id=A.ParentId LEFT JOIN Personel_Table C ON C.Id=A.AmirId ORDER BY A.Sira"), ""); }
        public DataTable SelectByParentId(int parentId) { SqlQuery q = new SqlQuery("SELECT * FROM BirimTanim_Table WHERE ParentId=@ParentId ORDER BY Sira"); q.AddParameter("@ParentId", parentId); return db.SelectFromDb(q, ""); }
        public DataTable SelectRoot() { return db.SelectFromDb(new SqlQuery("SELECT * FROM BirimTanim_Table WHERE ParentId=0 ORDER BY Sira"), ""); }
    }
}
