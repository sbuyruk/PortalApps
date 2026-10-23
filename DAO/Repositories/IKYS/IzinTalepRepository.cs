using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class IzinTalepRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public IzinTalepRepository() : this(new DbClass()) { }

        public IzinTalepRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IzinTalep_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IzinTalep_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return TraceSelect(query, "IzinTalep.SelectById", context);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM IzinTalep_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { DataTable result = transaction.Select(query); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", "IzinTalep.SelectById", clock.ElapsedMilliseconds, succeeded); }
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM IzinTalep_Table"), "");
        }

        public DataTable SelectAll(PortalLogContext context)
        {
            return TraceSelect(new SqlQuery("SELECT * FROM IzinTalep_Table"), "IzinTalep.SelectAll", context);
        }


        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try { int id = transaction.Insert(queryBuilder.BuildInsert(item, "IzinTalep_Table")); succeeded = id > 0; return id; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "INSERT", "IzinTalep.Insert", clock.ElapsedMilliseconds, succeeded); }
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildUpdate(item, "IzinTalep_Table"), "UPDATE", "IzinTalep.Update", transaction, context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return TraceWrite(queryBuilder.BuildDelete("IzinTalep_Table", id), "DELETE", "IzinTalep.Delete", transaction, context);
        }

        public DataTable SelectIzinTalepleri(int personelId, int izinTipi, bool mazeretHaric, int mazeretIzinTipi, List<int> izinDonemIds)
        {
            string sql = @"
                SELECT A.Id IzinTalepId, P.Adi+' '+P.Soyadi AdiSoyadi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,
                       B.Adi IzinTipi,A.IzinTipi IzinTipiId, A.IzinDonemId, A.OnayDurumu OnayDurumuId,
                       C.Adi OnayDurumu, A.VekilImza, A.AmirImza,A.OnayImza, A.Adres,A.Aciklama
                FROM IzinTalep_Table A
                INNER JOIN Personel_Table P ON A.PersonelId=P.Id
                INNER JOIN IzinTanim_Table B ON A.IzinTipi=B.Id
                INNER JOIN OnayTanim_Table C ON A.OnayDurumu=C.Id
                WHERE A.Aktif=1";
            SqlQuery query = new SqlQuery();
            if (personelId > 0)
            {
                sql += " AND A.PersonelId=@PersonelId";
                query.AddParameter("@PersonelId", personelId);
            }
            if (mazeretHaric)
            {
                sql += " AND A.IzinTipi<>@MazeretIzinTipi";
                query.AddParameter("@MazeretIzinTipi", mazeretIzinTipi);
            }
            else if (izinTipi > 0)
            {
                sql += " AND A.IzinTipi=@IzinTipi";
                query.AddParameter("@IzinTipi", izinTipi);
            }
            if (izinDonemIds != null && izinDonemIds.Count > 0)
            {
                List<string> parameters = new List<string>();
                for (int i = 0; i < izinDonemIds.Count; i++)
                {
                    string parameterName = "@IzinDonemId" + i;
                    parameters.Add("A.IzinDonemId=" + parameterName);
                    query.AddParameter(parameterName, izinDonemIds[i]);
                }
                sql += " AND (" + string.Join(" OR ", parameters) + " OR A.IzinDonemId=0)";
            }
            query.Sql = sql + " ORDER BY A.Id DESC";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectIzinTalepleri(int personelId, int izinTipi, bool mazeretHaric, int mazeretIzinTipi, List<int> izinDonemIds, PortalLogContext context)
        {
            return TraceSelect(BuildIzinTalepleriQuery(personelId, izinTipi, mazeretHaric, mazeretIzinTipi, izinDonemIds), "IzinTalep.SelectIzinTalepleri", context);
        }

        public DataTable SelectByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM IzinTalep_Table
                WHERE OnayDurumu NOT IN (3,4) AND PersonelId=@PersonelId
                  AND (BaslangicTarihi<=@BitTar AND BitisTarihi>=@BasTar)");
            query.AddParameter("@PersonelId", personelId);
            query.AddParameter("@BasTar", basTar);
            query.AddParameter("@BitTar", bitTar);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM IzinTalep_Table WHERE OnayDurumu NOT IN (3,4) AND PersonelId=@PersonelId AND (BaslangicTarihi<=@BitTar AND BitisTarihi>=@BasTar)");
            query.AddParameter("@PersonelId", personelId); query.AddParameter("@BasTar", basTar); query.AddParameter("@BitTar", bitTar);
            return TraceSelect(query, "IzinTalep.SelectByPersonelIdBasBitTar", context);
        }

        public DataTable SelectIslemiDevamEden(int personelId, int izinTipi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM IzinTalep_Table
                WHERE OnayDurumu NOT IN (2,3,4) AND PersonelId=@PersonelId AND IzinTipi=@IzinTipi");
            query.AddParameter("@PersonelId", personelId);
            query.AddParameter("@IzinTipi", izinTipi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectIslemiDevamEden(int personelId, int izinTipi, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM IzinTalep_Table WHERE OnayDurumu NOT IN (2,3,4) AND PersonelId=@PersonelId AND IzinTipi=@IzinTipi");
            query.AddParameter("@PersonelId", personelId); query.AddParameter("@IzinTipi", izinTipi);
            return TraceSelect(query, "IzinTalep.SelectIslemiDevamEden", context);
        }

        public DataTable SelectSonByPersonel(int izinTipi, int personelId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM IzinTalep_Table
                WHERE IzinTipi=@IzinTipi AND PersonelId=@PersonelId
                ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@IzinTipi", izinTipi);
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSonByPersonel(int izinTipi, int personelId, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM IzinTalep_Table WHERE IzinTipi=@IzinTipi AND PersonelId=@PersonelId ORDER BY BaslangicTarihi DESC");
            query.AddParameter("@IzinTipi", izinTipi); query.AddParameter("@PersonelId", personelId);
            return TraceSelect(query, "IzinTalep.SelectSonByPersonel", context);
        }

        private SqlQuery BuildIzinTalepleriQuery(int personelId, int izinTipi, bool mazeretHaric, int mazeretIzinTipi, List<int> izinDonemIds)
        {
            string sql = @"SELECT A.Id IzinTalepId, P.Adi+' '+P.Soyadi AdiSoyadi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,
                       B.Adi IzinTipi,A.IzinTipi IzinTipiId, A.IzinDonemId, A.OnayDurumu OnayDurumuId,
                       C.Adi OnayDurumu, A.VekilImza, A.AmirImza,A.OnayImza, A.Adres,A.Aciklama
                FROM IzinTalep_Table A INNER JOIN Personel_Table P ON A.PersonelId=P.Id
                INNER JOIN IzinTanim_Table B ON A.IzinTipi=B.Id INNER JOIN OnayTanim_Table C ON A.OnayDurumu=C.Id WHERE A.Aktif=1";
            SqlQuery query = new SqlQuery();
            if (personelId > 0) { sql += " AND A.PersonelId=@PersonelId"; query.AddParameter("@PersonelId", personelId); }
            if (mazeretHaric) { sql += " AND A.IzinTipi<>@MazeretIzinTipi"; query.AddParameter("@MazeretIzinTipi", mazeretIzinTipi); }
            else if (izinTipi > 0) { sql += " AND A.IzinTipi=@IzinTipi"; query.AddParameter("@IzinTipi", izinTipi); }
            if (izinDonemIds != null && izinDonemIds.Count > 0)
            {
                List<string> parameters = new List<string>();
                for (int i = 0; i < izinDonemIds.Count; i++) { string parameterName = "@IzinDonemId" + i; parameters.Add("A.IzinDonemId=" + parameterName); query.AddParameter(parameterName, izinDonemIds[i]); }
                sql += " AND (" + string.Join(" OR ", parameters) + " OR A.IzinDonemId=0)";
            }
            query.Sql = sql + " ORDER BY A.Id DESC";
            return query;
        }

        private DataTable TraceSelect(SqlQuery query, string name, PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(query, context, name);
        }

        private static bool TraceWrite(SqlQuery query, string operation, string name, SqlTransactionContext transaction, PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { succeeded = transaction.Update(query); return succeeded; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, operation, name, clock.ElapsedMilliseconds, succeeded); }
        }
    }
}
