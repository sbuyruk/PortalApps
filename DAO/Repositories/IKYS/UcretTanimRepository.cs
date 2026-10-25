using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

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

        public DataTable SelectUcretByGrupDereceKademe(int grupId, int derece, int kademe, bool asker) { string column = asker ? "AskerUcret" : "UstUcret"; SqlQuery q = new SqlQuery("SELECT " + column + " FROM UcretTanim_Table WHERE GrupId=@GrupId AND Derece=@Derece AND Kademe=@Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Derece", derece); q.AddParameter("@Kademe", kademe); return db.SelectFromDb(q, ""); }
        public DataTable SelectById(int id, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM UcretTanim_Table WHERE Id=@Id"); q.AddParameter("@Id", id); return TraceSelect(q, "UcretTanim.SelectById", context); }

        public DataTable SelectAll(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT * FROM UcretTanim_Table"), "UcretTanim.SelectAll", context); }

        public DataTable SelectByKademe(int grupId, int kademe, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM UcretTanim_Table WHERE GrupId=@GrupId AND Kademe=@Kademe ORDER BY Derece,Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Kademe", kademe); return TraceSelect(q, "UcretTanim.SelectByKademe", context); }

        public DataTable SelectByMaxGrupId(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT * FROM UcretTanim_Table WHERE GrupId=(SELECT MAX(GrupId) FROM UcretTanim_Table) ORDER BY Derece,Kademe"), "UcretTanim.SelectByMaxGrupId", context); }

        public DataTable SelectByGrup(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT GrupId,MAX(BaslangicTarihi) AS BaslangicTarihi,MAX(BitisTarihi) AS BitisTarihi FROM UcretTanim_Table WHERE BaslangicTarihi IS NOT NULL GROUP BY GrupId ORDER BY GrupId DESC"), "UcretTanim.SelectByGrup", context); }

        public DataTable SelectDerece(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT DISTINCT Derece, Unvan FROM UcretTanim_Table ORDER BY Derece"), "UcretTanim.SelectDerece", context); }

        public DataTable SelectKademe(int derece, int grupId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT DISTINCT Kademe FROM UcretTanim_Table WHERE Derece=@Derece AND GrupId=@GrupId ORDER BY Kademe"); q.AddParameter("@Derece", derece); q.AddParameter("@GrupId", grupId); return TraceSelect(q, "UcretTanim.SelectKademe", context); }

        public DataTable SelectMaasListesi(int grupId, DateTime tarih, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery(@"SELECT A.Id AS PersonelId,A.Adi,A.Soyadi,B.KisaAdi Unvan,E.Derece,E.Kademe,C.ProtokolSiraNo,E.DegisimTarihi DereceKademeIlerlemeTarihi,@GrupId GrupId,D.Agi,CASE WHEN A.Asker_Sivil=1 THEN D.AskerUcret ELSE D.UstUcret END AS Ucret
                FROM Personel_Table A LEFT JOIN GorevTanim_Table B ON B.PersonelId=A.Id LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id
                LEFT JOIN DereceKademeDegisim_Table E ON E.PersonelId=A.Id AND E.DegisimTarihi=(SELECT Max(DegisimTarihi) FROM DereceKademeDegisim_Table WHERE PersonelId=A.Id AND DegisimTarihi<=@Tarih)
                LEFT JOIN UcretTanim_Table D ON D.Derece=E.Derece AND D.Kademe=E.Kademe AND D.GrupId=@GrupId WHERE C.CalismaDurumu=1 AND Tipi=1 ORDER BY C.ProtokolSiraNo");
            q.AddParameter("@GrupId", grupId); q.AddParameter("@Tarih", tarih); return TraceSelect(q, "UcretTanim.SelectMaasListesi", context);
        }

        public DataTable SelectMaxBitisTarihi(PortalLogContext context)
        { return TraceSelect(new SqlQuery("SELECT MAX(BitisTarihi) FROM UcretTanim_Table"), "UcretTanim.SelectMaxBitisTarihi", context); }

        public DataTable SelectAgi(DateTime tarih, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT TOP 1 Agi FROM UcretTanim_Table WHERE BitisTarihi=@BitisTarihi"); q.AddParameter("@BitisTarihi", tarih); return TraceSelect(q, "UcretTanim.SelectAgi", context); }

        public DataTable SelectGrupIdByTarih(DateTime tarih, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT MAX(GrupId) GrupId FROM UcretTanim_Table WHERE BaslangicTarihi<=@Tarih AND BitisTarihi>=@Tarih"); q.AddParameter("@Tarih", tarih); return TraceSelect(q, "UcretTanim.SelectGrupIdByTarih", context); }

        public DataTable SelectUcretByGrupDereceKademe(int grupId, int derece, int kademe, bool asker, PortalLogContext context)
        { string column = asker ? "AskerUcret" : "UstUcret"; SqlQuery q = new SqlQuery("SELECT " + column + " FROM UcretTanim_Table WHERE GrupId=@GrupId AND Derece=@Derece AND Kademe=@Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Derece", derece); q.AddParameter("@Kademe", kademe); return TraceSelect(q, "UcretTanim.SelectUcretByGrupDereceKademe", context); }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM UcretTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { transaction.DeferSqlTrace(context, "SELECT", "UcretTanim.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "UcretTanim_Table")); succeeded = id > 0; return id; }
            finally { transaction.DeferSqlTrace(context, "INSERT", "UcretTanim.Insert", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "UcretTanim_Table"), "UPDATE", "UcretTanim.Update", transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("UcretTanim_Table", id), "DELETE", "UcretTanim.Delete", transaction, context);
        }

        public DataTable SelectByGrupId(int grupId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM UcretTanim_Table WHERE GrupId=@GrupId ORDER BY Derece,Kademe");
            query.AddParameter("@GrupId", grupId);
            return TraceSelect(query, "UcretTanim.SelectByGrupId", context);
        }

        public DataTable SelectByGrupId(int grupId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM UcretTanim_Table WITH (UPDLOCK, HOLDLOCK) WHERE GrupId=@GrupId ORDER BY Id");
            query.AddParameter("@GrupId", grupId);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { transaction.DeferSqlTrace(context, "SELECT", "UcretTanim.SelectByGrupId", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool DeleteByGrupId(int grupId, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("DELETE FROM UcretTanim_Table WHERE GrupId=@GrupId");
            query.AddParameter("@GrupId", grupId);
            return TraceWrite(query, "DELETE", "UcretTanim.DeleteByGrupId", transaction, context);
        }

        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(query, context, name);
        }

        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { transaction.DeferSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }

        public DataTable SelectMaasListesi(int grupId, DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery q = new SqlQuery(@"SELECT A.Id AS PersonelId,A.Adi,A.Soyadi,B.KisaAdi Unvan,E.Derece,E.Kademe,C.ProtokolSiraNo,E.DegisimTarihi DereceKademeIlerlemeTarihi,@GrupId GrupId,D.Agi,CASE WHEN A.Asker_Sivil=1 THEN D.AskerUcret ELSE D.UstUcret END AS Ucret
                FROM Personel_Table A LEFT JOIN GorevTanim_Table B ON B.PersonelId=A.Id LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id
                LEFT JOIN DereceKademeDegisim_Table E ON E.PersonelId=A.Id AND E.DegisimTarihi=(SELECT Max(DegisimTarihi) FROM DereceKademeDegisim_Table WHERE PersonelId=A.Id AND DegisimTarihi<=@Tarih)
                LEFT JOIN UcretTanim_Table D ON D.Derece=E.Derece AND D.Kademe=E.Kademe AND D.GrupId=@GrupId WHERE C.CalismaDurumu=1 AND Tipi=1 ORDER BY C.ProtokolSiraNo");
            q.AddParameter("@GrupId", grupId); q.AddParameter("@Tarih", tarih); return PortalSqlReadExecutor.Select(q, context, "UcretTanim.SelectMaasListesi", transaction);
        }

        public DataTable SelectGrupIdByTarih(DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction"); SqlQuery q = new SqlQuery("SELECT MAX(GrupId) GrupId FROM UcretTanim_Table WHERE BaslangicTarihi<=@Tarih AND BitisTarihi>=@Tarih"); q.AddParameter("@Tarih", tarih); return PortalSqlReadExecutor.Select(q, context, "UcretTanim.SelectGrupIdByTarih", transaction); }

        public DataTable SelectUcretByGrupDereceKademe(int grupId, int derece, int kademe, bool asker, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction"); string column = asker ? "AskerUcret" : "UstUcret"; SqlQuery q = new SqlQuery("SELECT " + column + " FROM UcretTanim_Table WHERE GrupId=@GrupId AND Derece=@Derece AND Kademe=@Kademe"); q.AddParameter("@GrupId", grupId); q.AddParameter("@Derece", derece); q.AddParameter("@Kademe", kademe); return PortalSqlReadExecutor.Select(q, context, "UcretTanim.SelectUcretByGrupDereceKademe", transaction); }
    }
}
