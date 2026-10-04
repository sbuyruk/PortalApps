using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class UcretTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public UcretTanimRepository() : this(new DbClass()) { }
        public UcretTanimRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; queryBuilder = new CrudQueryBuilder(); }
        public DataTable SelectById(int id) { SqlQuery q = new SqlQuery("SELECT * FROM UcretTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM UcretTanim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "UcretTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "UcretTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("UcretTanim_Table", id), ""); }
        public DataTable SelectByKademe(int grupId, int kademe) { SqlQuery q = new SqlQuery("SELECT * FROM UcretTanim_Table WHERE GrupId=@GrupId AND Kademe=@Kademe ORDER BY Derece,Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Kademe", kademe); return db.SelectFromDb(q, ""); }
        public DataTable SelectByMaxGrupId() { return db.SelectFromDb(new SqlQuery("SELECT * FROM UcretTanim_Table WHERE GrupId=(SELECT MAX(GrupId) FROM UcretTanim_Table) ORDER BY Derece,Kademe"), ""); }
        public DataTable SelectByGrup() { return db.SelectFromDb(new SqlQuery("SELECT GrupId,MAX(BaslangicTarihi) AS BaslangicTarihi,MAX(BitisTarihi) AS BitisTarihi FROM UcretTanim_Table WHERE BaslangicTarihi IS NOT NULL GROUP BY GrupId ORDER BY GrupId DESC"), ""); }
        public DataTable SelectDerece() { return db.SelectFromDb(new SqlQuery("SELECT DISTINCT Derece, Unvan FROM UcretTanim_Table ORDER BY Derece"), ""); }
        public DataTable SelectKademe(int derece, int grupId) { SqlQuery q = new SqlQuery("SELECT DISTINCT Kademe FROM UcretTanim_Table WHERE Derece=@Derece AND GrupId=@GrupId ORDER BY Kademe"); q.AddParameter("@Derece", derece); q.AddParameter("@GrupId", grupId); return db.SelectFromDb(q, ""); }
        public DataTable SelectMaasListesi(int grupId, DateTime tarih)
        {
            SqlQuery q = new SqlQuery(@"SELECT A.Id AS PersonelId,A.Adi,A.Soyadi,B.KisaAdi Unvan,E.Derece,E.Kademe,C.ProtokolSiraNo,E.DegisimTarihi DereceKademeIlerlemeTarihi,@GrupId GrupId,D.Agi,CASE WHEN A.Asker_Sivil=1 THEN D.AskerUcret ELSE D.UstUcret END AS Ucret
                FROM Personel_Table A LEFT JOIN GorevTanim_Table B ON B.PersonelId=A.Id LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id
                LEFT JOIN DereceKademeDegisim_Table E ON E.PersonelId=A.Id AND E.DegisimTarihi=(SELECT Max(DegisimTarihi) FROM DereceKademeDegisim_Table WHERE PersonelId=A.Id AND DegisimTarihi<=@Tarih)
                LEFT JOIN UcretTanim_Table D ON D.Derece=E.Derece AND D.Kademe=E.Kademe AND D.GrupId=@GrupId WHERE C.CalismaDurumu=1 AND Tipi=1 ORDER BY C.ProtokolSiraNo");
            q.AddParameter("@GrupId", grupId); q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, "");
        }
        public DataTable SelectMaxBitisTarihi() { return db.SelectFromDb(new SqlQuery("SELECT MAX(BitisTarihi) FROM UcretTanim_Table"), ""); }
        public DataTable SelectAgi(DateTime tarih) { SqlQuery q = new SqlQuery("SELECT TOP 1 Agi FROM UcretTanim_Table WHERE BitisTarihi=@BitisTarihi"); q.AddParameter("@BitisTarihi", tarih); return db.SelectFromDb(q, ""); }
        public DataTable SelectGrupIdByTarih(DateTime tarih) { SqlQuery q = new SqlQuery("SELECT MAX(GrupId) GrupId FROM UcretTanim_Table WHERE BaslangicTarihi<=@Tarih AND BitisTarihi>=@Tarih"); q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, ""); }
        public bool DeleteByGrupId(int grupId) { SqlQuery q = new SqlQuery("DELETE FROM UcretTanim_Table WHERE GrupId=@GrupId"); q.AddParameter("@GrupId", grupId); return db.DeleteFromDb(q, ""); }
        public DataTable SelectUcretByGrupDereceKademe(int grupId, int derece, int kademe, bool asker) { string column = asker ? "AskerUcret" : "UstUcret"; SqlQuery q = new SqlQuery("SELECT " + column + " FROM UcretTanim_Table WHERE GrupId=@GrupId AND Derece=@Derece AND Kademe=@Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Derece", derece); q.AddParameter("@Kademe", kademe); return db.SelectFromDb(q, ""); }
    }
}
