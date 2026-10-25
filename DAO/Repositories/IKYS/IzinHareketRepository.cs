using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class IzinHareketRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;
        public IzinHareketRepository() : this(new DbClass())
        {
        }

        public IzinHareketRepository(DbClass db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }
        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM IzinHareket_Table"), "");
        }


        public DataTable SelectById(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(query, context, "IzinHareket.SelectById");
        }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            SqlQuery query = new SqlQuery("SELECT * FROM IzinHareket_Table WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);

            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                DataTable result = transaction.Select(query);
                succeeded = true;
                return result;
            }
            finally
            {
                transaction.DeferSqlTrace(
                    context,
                    "SELECT",
                    "IzinHareket.SelectById",
                    clock.ElapsedMilliseconds,
                    succeeded);
            }
        }
        public DataTable SelectAll(PortalLogContext context)
        {
            return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM IzinHareket_Table"), context, "IzinHareket.SelectAll");
        }

        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            return TraceInsert(
                queryBuilder.BuildInsert(item, "IzinHareket_Table"),
                transaction,
                context);
        }

        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context)
        {
            return TraceWrite(
                queryBuilder.BuildUpdate(item, "IzinHareket_Table"),
                "UPDATE",
                "IzinHareket.Update",
                transaction,
                context);
        }

        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context)
        {
            return TraceWrite(
                queryBuilder.BuildDelete("IzinHareket_Table", id),
                "DELETE",
                "IzinHareket.Delete",
                transaction,
                context);
        }

        public DataTable SelectByTarih(DateTime tarih, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE IzinTipi!=2 AND BitisTarihi>=@Tarih ORDER BY BaslangicTarihi"); q.AddParameter("@Tarih", tarih); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByTarih"); }
        public DataTable SelectByIzinTalepId(int id, PortalLogContext context)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM IzinHareket_Table WHERE IzinTalepId=@IzinTalepId");
            query.AddParameter("@IzinTalepId", id);
            return PortalSqlReadExecutor.Select(query, context, "IzinHareket.SelectByIzinTalepId");
        }

        public DataTable SelectByIzinTalepId(
            int id,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            SqlQuery query = new SqlQuery(
                "SELECT * FROM IzinHareket_Table WHERE IzinTalepId=@IzinTalepId");
            query.AddParameter("@IzinTalepId", id);

            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                DataTable result = transaction.Select(query);
                succeeded = true;
                return result;
            }
            finally
            {
                transaction.DeferSqlTrace(
                    context,
                    "SELECT",
                    "IzinHareket.SelectByIzinTalepId",
                    clock.ElapsedMilliseconds,
                    succeeded);
            }
        }
        public DataTable SelectByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE PersonelId=@PersonelId AND IzinTipi!=2 AND IzinTipi!=8 AND BaslangicTarihi<=@BaslangicTarihi AND BitisTarihi>=@BitisTarihi ORDER BY BitisTarihi DESC"); q.AddParameter("@PersonelId", personelId); q.AddParameter("@BaslangicTarihi", baslangic); q.AddParameter("@BitisTarihi", bitis); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByPersonelTarih");
        }

        public DataTable SelectDigerByPersonelId(int personelId, PortalLogContext context)
        { SqlQuery q = new SqlQuery("SELECT *, B.Adi IzinTipiAdi FROM IzinHareket_Table A INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi WHERE IzinTipi NOT IN (1,2) AND PersonelId=@PersonelId ORDER BY A.BaslangicTarihi DESC"); q.AddParameter("@PersonelId", personelId); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectDigerByPersonelId"); }

        private DataTable TraceSelect(
            string name,
            Func<DataTable> action,
            PortalLogContext context)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                DataTable result = action();
                succeeded = true;
                return result;
            }
            finally
            {
                PortalTechnicalLog.WriteSqlTrace(
                    context,
                    "SELECT",
                    name,
                    clock.ElapsedMilliseconds,
                    succeeded);
            }
        }

        private static int TraceInsert(
            SqlQuery query,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                int id = transaction.Insert(query);
                succeeded = id > 0;
                return id;
            }
            finally
            {
                transaction.DeferSqlTrace(
                    context,
                    "INSERT",
                    "IzinHareket.Insert",
                    clock.ElapsedMilliseconds,
                    succeeded);
            }
        }

        private static bool TraceWrite(
            SqlQuery query,
            string operation,
            string name,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                succeeded = transaction.Update(query);
                return succeeded;
            }
            finally
            {
                transaction.DeferSqlTrace(
                    context,
                    operation,
                    name,
                    clock.ElapsedMilliseconds,
                    succeeded);
            }
        }
        public DataTable SelectByTarih(DateTime tarih) { SqlQuery q = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE IzinTipi!=2 AND BitisTarihi>=@Tarih ORDER BY BaslangicTarihi"); q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, ""); }
        public DataTable SelectByIzinTalepId(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM IzinHareket_Table WHERE IzinTalepId=@IzinTalepId");
            query.AddParameter("@IzinTalepId", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByTarihReturnDataTable(DateTime baslangic, DateTime bitis)
        {
            SqlQuery q = new SqlQuery(@"SELECT B.Adi+' ' + B.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,C.Adi IzinTanim,D.BirimId,E.KisaAdi GorevYeri
                FROM IzinHareket_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId INNER JOIN IzinTanim_Table C ON C.Id=A.IzinTipi LEFT JOIN IsBilgileri_Table D ON D.PersonelId=A.PersonelId INNER JOIN BirimTanim_Table E ON E.Id=D.BirimId
                WHERE A.BaslangicTarihi<=@BaslangicTarihi AND A.BitisTarihi>=@BitisTarihi ORDER BY ProtokolSiraNo,A.IzinTipi,A.BaslangicTarihi");
            q.AddParameter("@BaslangicTarihi", baslangic); q.AddParameter("@BitisTarihi", bitis); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM IzinHareket_Table WHERE PersonelId=@PersonelId AND IzinTipi!=2 AND IzinTipi!=8 AND BaslangicTarihi<=@BaslangicTarihi AND BitisTarihi>=@BitisTarihi ORDER BY BitisTarihi DESC"); q.AddParameter("@PersonelId", personelId); q.AddParameter("@BaslangicTarihi", baslangic); q.AddParameter("@BitisTarihi", bitis); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByIzinTipiTarih(int izinTipi, DateTime ilkTarih, DateTime bitis)
        {
            SqlQuery q = new SqlQuery(@"SELECT B.Adi+' ' + B.Soyadi AdiSoyadi,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,C.Adi IzinTanim,D.BirimId,E.KisaAdi GorevYeri,A.Aciklama,D.ProtokolSiraNo
                FROM IzinHareket_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId INNER JOIN IzinTanim_Table C ON C.Id=A.IzinTipi LEFT JOIN IsBilgileri_Table D ON D.PersonelId=A.PersonelId INNER JOIN BirimTanim_Table E ON E.Id=D.BirimId
                WHERE A.Mahsup=0 AND A.IzinTipi=@IzinTipi AND A.BaslangicTarihi BETWEEN @IlkTarih AND @BitisTarihi ORDER BY D.ProtokolSiraNo,A.BaslangicTarihi");
            q.AddParameter("@IzinTipi", izinTipi); q.AddParameter("@IlkTarih", ilkTarih); q.AddParameter("@BitisTarihi", bitis); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByIzinDonemi(int personelId, int izinTipi, DateTime baslangic, DateTime bitis)
        {
            string filters = personelId > 0 ? " AND A.PersonelId=@PersonelId" : ""; filters += izinTipi > 0 ? " AND A.IzinTipi=@IzinTipi" : "";
            SqlQuery q = new SqlQuery("SELECT A.Id IzinHareketId,P.Adi+' '+P.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTalepId,CONVERT(varchar(10),@DonemBaslangic,104)+'-'+CONVERT(varchar(10),@DonemBitis,104) IzinDonemi,B.Adi IzinTipi,B.Id IzinTipiId,A.VekilImza,A.AmirImza,A.OnayImza,A.Adres,A.Aciklama,A.IzinDonemId,A.Mahsup FROM IzinHareket_Table A INNER JOIN Personel_Table P ON P.Id=A.PersonelId INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi WHERE A.BaslangicTarihi BETWEEN @Baslangic AND @Bitis" + filters + " ORDER BY A.Id DESC");
            q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); q.AddParameter("@DonemBaslangic", baslangic); q.AddParameter("@DonemBitis", bitis); if (personelId > 0) q.AddParameter("@PersonelId", personelId); if (izinTipi > 0) q.AddParameter("@IzinTipi", izinTipi); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPersonelIdDonemId(int personelId, int donemId, int izinTipi)
        {
            int sinceYear = DateTime.Today.AddYears(-30).Year; string filters = personelId > 0 ? " AND A.PersonelId=@PersonelId" : ""; filters += donemId > 0 ? " AND A.IzinDonemId=@DonemId" : ""; filters += izinTipi > 0 ? " AND A.IzinTipi=@IzinTipi" : "";
            SqlQuery q = new SqlQuery(@"SELECT A.Id IzinHareketId,P.Adi+' '+P.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.Sure+' '+A.Birim SureBirim,A.IzinTalepId,A.IzinDonemId,CONVERT(varchar,FORMAT(C.BaslangicTarihi,'dd.MM.yyyy')) +'-'+CONVERT(varchar,FORMAT(C.BitisTarihi,'dd.MM.yyyy')) IzinDonemi,C.Adi IzinDonemiYil,B.Adi IzinTipi,B.Id IzinTipiId,A.VekilImza,A.AmirImza,A.OnayImza,A.Adres,A.Mahsup,A.Aciklama,A.OncekiIzinStr,A.KullanilanIzinStr,A.KalanIzinStr
                FROM IzinHareket_Table A INNER JOIN Personel_Table P ON P.Id=A.PersonelId INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi LEFT OUTER JOIN IzinDonem_Table C ON C.Id=A.IzinDonemId
                WHERE A.BaslangicTarihi>=@Since" + filters + " ORDER BY A.BaslangicTarihi DESC");
            q.AddParameter("@Since", new DateTime(sinceYear, 1, 1)); if (personelId > 0) q.AddParameter("@PersonelId", personelId); if (donemId > 0) q.AddParameter("@DonemId", donemId); if (izinTipi > 0) q.AddParameter("@IzinTipi", izinTipi); return db.SelectFromDb(q, "");
        }
        public DataTable SelectByPersonelIdDonemId(int personelId, int donemId, int izinTipi, PortalLogContext context)
        {
            int sinceYear = DateTime.Today.AddYears(-30).Year; string filters = personelId > 0 ? " AND A.PersonelId=@PersonelId" : ""; filters += donemId > 0 ? " AND A.IzinDonemId=@DonemId" : ""; filters += izinTipi > 0 ? " AND A.IzinTipi=@IzinTipi" : "";
            SqlQuery q = new SqlQuery(@"SELECT A.Id IzinHareketId,P.Adi+' '+P.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.Sure+' '+A.Birim SureBirim,A.IzinTalepId,A.IzinDonemId,CONVERT(varchar,FORMAT(C.BaslangicTarihi,'dd.MM.yyyy')) +'-'+CONVERT(varchar,FORMAT(C.BitisTarihi,'dd.MM.yyyy')) IzinDonemi,C.Adi IzinDonemiYil,B.Adi IzinTipi,B.Id IzinTipiId,A.VekilImza,A.AmirImza,A.OnayImza,A.Adres,A.Mahsup,A.Aciklama,A.OncekiIzinStr,A.KullanilanIzinStr,A.KalanIzinStr
                FROM IzinHareket_Table A INNER JOIN Personel_Table P ON P.Id=A.PersonelId INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi LEFT OUTER JOIN IzinDonem_Table C ON C.Id=A.IzinDonemId
                WHERE A.BaslangicTarihi>=@Since" + filters + " ORDER BY A.BaslangicTarihi DESC");
            q.AddParameter("@Since", new DateTime(sinceYear, 1, 1)); if (personelId > 0) q.AddParameter("@PersonelId", personelId); if (donemId > 0) q.AddParameter("@DonemId", donemId); if (izinTipi > 0) q.AddParameter("@IzinTipi", izinTipi); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByPersonelIdDonemId");
        }
        public DataTable SelectDigerByPersonelId(int personelId) { SqlQuery q = new SqlQuery("SELECT *, B.Adi IzinTipiAdi FROM IzinHareket_Table A INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi WHERE IzinTipi NOT IN (1,2) AND PersonelId=@PersonelId ORDER BY A.BaslangicTarihi DESC"); q.AddParameter("@PersonelId", personelId); return db.SelectFromDb(q, ""); }

        public DataTable SelectByTarihReturnDataTable(DateTime baslangic, DateTime bitis, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery(@"SELECT B.Adi+' ' + B.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,C.Adi IzinTanim,D.BirimId,E.KisaAdi GorevYeri
                FROM IzinHareket_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId INNER JOIN IzinTanim_Table C ON C.Id=A.IzinTipi LEFT JOIN IsBilgileri_Table D ON D.PersonelId=A.PersonelId INNER JOIN BirimTanim_Table E ON E.Id=D.BirimId
                WHERE A.BaslangicTarihi<=@BaslangicTarihi AND A.BitisTarihi>=@BitisTarihi ORDER BY ProtokolSiraNo,A.IzinTipi,A.BaslangicTarihi");
            q.AddParameter("@BaslangicTarihi", baslangic); q.AddParameter("@BitisTarihi", bitis); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByTarihReturnDataTable");
        }

        public DataTable SelectByIzinTipiTarih(int izinTipi, DateTime ilkTarih, DateTime bitis, PortalLogContext context)
        {
            SqlQuery q = new SqlQuery(@"SELECT B.Adi+' ' + B.Soyadi AdiSoyadi,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,C.Adi IzinTanim,D.BirimId,E.KisaAdi GorevYeri,A.Aciklama,D.ProtokolSiraNo
                FROM IzinHareket_Table A INNER JOIN Personel_Table B ON B.Id=A.PersonelId INNER JOIN IzinTanim_Table C ON C.Id=A.IzinTipi LEFT JOIN IsBilgileri_Table D ON D.PersonelId=A.PersonelId INNER JOIN BirimTanim_Table E ON E.Id=D.BirimId
                WHERE A.Mahsup=0 AND A.IzinTipi=@IzinTipi AND A.BaslangicTarihi BETWEEN @IlkTarih AND @BitisTarihi ORDER BY D.ProtokolSiraNo,A.BaslangicTarihi");
            q.AddParameter("@IzinTipi", izinTipi); q.AddParameter("@IlkTarih", ilkTarih); q.AddParameter("@BitisTarihi", bitis); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByIzinTipiTarih");
        }

        public DataTable SelectByIzinDonemi(int personelId, int izinTipi, DateTime baslangic, DateTime bitis, PortalLogContext context)
        {
            string filters = personelId > 0 ? " AND A.PersonelId=@PersonelId" : ""; filters += izinTipi > 0 ? " AND A.IzinTipi=@IzinTipi" : "";
            SqlQuery q = new SqlQuery("SELECT A.Id IzinHareketId,P.Adi+' '+P.Soyadi AdiSoyadi,A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTalepId,CONVERT(varchar(10),@DonemBaslangic,104)+'-'+CONVERT(varchar(10),@DonemBitis,104) IzinDonemi,B.Adi IzinTipi,B.Id IzinTipiId,A.VekilImza,A.AmirImza,A.OnayImza,A.Adres,A.Aciklama,A.IzinDonemId,A.Mahsup FROM IzinHareket_Table A INNER JOIN Personel_Table P ON P.Id=A.PersonelId INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi WHERE A.BaslangicTarihi BETWEEN @Baslangic AND @Bitis" + filters + " ORDER BY A.Id DESC");
            q.AddParameter("@Baslangic", baslangic); q.AddParameter("@Bitis", bitis); q.AddParameter("@DonemBaslangic", baslangic); q.AddParameter("@DonemBitis", bitis); if (personelId > 0) q.AddParameter("@PersonelId", personelId); if (izinTipi > 0) q.AddParameter("@IzinTipi", izinTipi); return PortalSqlReadExecutor.Select(q, context, "IzinHareket.SelectByIzinDonemi");
        }
    }
}
