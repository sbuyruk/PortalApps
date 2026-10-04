using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class IzinDonemRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public IzinDonemRepository() : this(new DbClass()) { }
        public IzinDonemRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM IzinDonem_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM IzinDonem_Table ORDER BY BaslangicTarihi DESC"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "IzinDonem_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "IzinDonem_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("IzinDonem_Table", id), ""); }
        public DataTable SelectByIzinTarihi(int personelId, int izinTipi, DateTime tarih)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM IzinDonem_Table WHERE PersonelId=@PersonelId AND (IzinTipi=@IzinTipi OR @IzinTipi=0) AND BaslangicTarihi<=@Tarih AND BitisTarihi>@OncekiTarih ORDER BY BaslangicTarihi");
            q.AddParameter("@PersonelId", personelId); q.AddParameter("@IzinTipi", izinTipi); q.AddParameter("@Tarih", tarih); q.AddParameter("@OncekiTarih", tarih.AddDays(-1)); return db.SelectFromDb(q, "");
        }
        public DataTable SelectOncekiYillaraAit(int personelId)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM IzinDonem_Table WHERE PersonelId=@PersonelId AND IzinTipi=1 AND BitisTarihi < GETDATE() AND KalanIzin!=0 ORDER BY BaslangicTarihi DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPersonelId(int personelId, int izinTipi)
        {
            string where = ""; if (personelId != 0) where += " PersonelId=@PersonelId"; if (izinTipi != 0) where += (where.Length == 0 ? "" : " AND ") + "IzinTipi=@IzinTipi"; if (where.Length > 0) where = " WHERE " + where;
            SqlQuery q = new SqlQuery("SELECT Id IzinDonemId, Id,PersonelId, BaslangicTarihi, BitisTarihi, Adi, IzinTipi, IzinHakki, KullanilanIzin, KalanIzin, Birim,Aciklama, Olusturan, OlusturmaTarihi, Degistiren, DegistirmeTarihi FROM IzinDonem_Table" + where + " ORDER BY BaslangicTarihi DESC");
            if (personelId != 0) q.AddParameter("@PersonelId", personelId); if (izinTipi != 0) q.AddParameter("@IzinTipi", izinTipi); return db.SelectFromDb(q, "");
        }
        public DataTable SelectSumKalanIzinByPersonelId(int personelId, bool sadeceEskiDonemler)
        {
            string oldFilter = sadeceEskiDonemler ? " AND BitisTarihi < GETDATE() " : "";
            SqlQuery q = new SqlQuery("SELECT SUM(CONVERT(INT,IzinHakki)) IzinHakkiToplami,SUM(CONVERT(INT,KullanilanIzin)) KullanilanIzinToplami,SUM(CONVERT(INT,KalanIzin)) KalanIzinToplami FROM IzinDonem_Table WHERE IzinTipi=@IzinTipi AND PersonelId=@PersonelId" + oldFilter + " GROUP BY PersonelId"); q.AddParameter("@IzinTipi", 1); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPersonelIdReturnDataTable(int personelId, int izinTipi) { return SelectByPersonelId(personelId, izinTipi); }
    }
}
