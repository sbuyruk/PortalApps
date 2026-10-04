using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class ResmiTatilRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public ResmiTatilRepository() : this(new DbClass()) { }
        public ResmiTatilRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM ResmiTatil_Table"), ""); }
        public DataTable SelectAllReturnDataTable() { return db.SelectFromDb(new SqlQuery("SELECT Id ResimiTatilId,Gun,Ay,Yil,Tatil,BaslamaTarihi,BitisTarihi,IlanTarihi,IptalTarihi FROM ResmiTatil_Table ORDER BY BaslamaTarihi DESC"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "ResmiTatil_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "ResmiTatil_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("ResmiTatil_Table", id), ""); }
        public DataTable SelectByTarih(DateTime baslangic, DateTime bitis) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR BaslamaTarihi BETWEEN @Baslangic AND @Bitis ORDER BY BaslamaTarihi"); q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); return db.SelectFromDb(q, ""); }
        public DataTable SelectBySonIkiYil(DateTime baslangic, DateTime bitis) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR BaslamaTarihi BETWEEN @Baslangic AND @Bitis ORDER BY MONTH(BaslamaTarihi),DAY(BaslamaTarihi)"); q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); return db.SelectFromDb(q, ""); }
        public DataTable SelectByYil(int yil) { SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE Yil=0 OR Yil=@Yil ORDER BY MONTH(BaslamaTarihi),DAY(BaslamaTarihi)"); q.AddParameter("@Yil", yil); return db.SelectFromDb(q, ""); }
        public DataTable SelectByDate(DateTime tarih)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM ResmiTatil_Table WHERE ((DAY(BaslamaTarihi)<=@Gun AND DAY(BitisTarihi)>=@Gun AND Ay=@Ay AND Yil=0) AND (IlanTarihi IS NULL OR IlanTarihi<=@Tarih) AND (IptalTarihi IS NULL OR IptalTarihi>=@Tarih)) OR (BaslamaTarihi<=@Tarih AND BitisTarihi>=@Tarih)");
            q.AddParameter("@Gun", tarih.Day); q.AddParameter("@Ay", tarih.Month); q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, "");
        }
    }
}
