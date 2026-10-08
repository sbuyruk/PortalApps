using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        public DataTable SelectByIdList(List<int> ids) { SqlQuery q = new SqlQuery(string.Format("SELECT * FROM KiraEkstreAktarma_Table WHERE Id in ({0}) ORDER BY AktarildiMi,Id DESC, OdemeTarihi desc, Adi", BuildInClause(ids))); AddIdParameters(q, ids); return db.SelectFromDb(q, ""); }
        public DataTable SelectByEkstreIdList(List<int> ids) { SqlQuery q = new SqlQuery(string.Format("SELECT * FROM KiraEkstreAktarma_Table WHERE Id in ({0}) ORDER BY AktarildiMi, OdemeTarihi desc, Adi", BuildInClause(ids))); AddIdParameters(q, ids); return db.SelectFromDb(q, ""); }
        public DataTable SelectUploadedRecords(DateTime basTar, bool aktarilanlarHaric, bool kiraTeminatDiger, int diger, int kira, int kesintiTeminat, int geciciTeminat, int kiraTeminat)
        {
            string aktarilanlarHaricStr = aktarilanlarHaric ? " AND AktarildiMi=@AktarildiMi " : "";
            string kiraTeminatDigerStr = kiraTeminatDiger ? " AND (OdemeSebebiId IS NULL OR OdemeSebebiId IN (@Diger,@Kira,@KesintiTeminat,@GeciciTeminat,@KiraTeminat)) " : "";
            SqlQuery q = new SqlQuery(string.Format(@"
                SELECT A.Id KiraEkstreAktarmaId, A.IslemTarihi, A.KiraciId,
                    B.Adi, B.Adi + ' (' + B.Adres + '' + IIF(ISNULL(D.IlceAdi,'')='','',D.IlceAdi + '/') + C.IlAdi+')' KiraciAdi,
                    A.*, ISNULL(A.Adi,'') + ' ' + ISNULL(A.Soyadi,'') AdiSoyadi,
                    A.Telefon1 + IIF(ISNULL(A.Telefon1,'')!='' AND ISNULL(A.Telefon2,'')!='',' - ','') + A.Telefon2 Telefon,
                    A.Aciklama, A.OdemeSebebiId OdemeSebebiId, E.OdemeSebebi OdemeSebebi
                FROM KiraEkstreAktarma_Table A
                    LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN Il_Table C ON C.IlAdi=B.Ili
                    LEFT JOIN Ilce_Table D ON D.IlceAdi=B.Ilcesi AND D.IlAdi=B.Ili
                    LEFT JOIN OdemeSebebiTanim_Table E ON E.Id=A.OdemeSebebiId
                WHERE 1>0 AND OdemeTarihi >= @BasTar
                    {0} {1}
                ORDER BY AktarildiMi, OdemeTarihi desc, A.Id, A.Adi", aktarilanlarHaricStr, kiraTeminatDigerStr));
            q.AddParameter("@BasTar", basTar);
            if (aktarilanlarHaric) q.AddParameter("@AktarildiMi", false);
            if (kiraTeminatDiger)
            {
                q.AddParameter("@Diger", diger); q.AddParameter("@Kira", kira);
                q.AddParameter("@KesintiTeminat", kesintiTeminat); q.AddParameter("@GeciciTeminat", geciciTeminat);
                q.AddParameter("@KiraTeminat", kiraTeminat);
            }
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectByIdDetailed(int id)
        {
            SqlQuery q = new SqlQuery(@"
                SELECT A.Id KiraEkstreAktarmaId, A.IslemTarihi, A.KiraciId,
                    B.Adi, B.Adi + ' (' + B.Adres + '' + IIF(ISNULL(D.IlceAdi,'')='','',D.IlceAdi + '/') + C.IlAdi+')' KiraciAdi,
                    A.*, ISNULL(A.Adi,'') + ' ' + ISNULL(A.Soyadi,'') AdiSoyadi,
                    A.Telefon1 + IIF(ISNULL(A.Telefon1,'')!='' AND ISNULL(A.Telefon2,'')!='',' - ','') + A.Telefon2 Telefon,
                    A.Aciklama
                FROM KiraEkstreAktarma_Table A
                    LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN Il_Table C ON C.IlAdi=B.Ili
                    LEFT JOIN Ilce_Table D ON D.IlceAdi=B.Ilcesi AND D.IlAdi=B.Ili
                WHERE A.Id=@Id
                ORDER BY AktarildiMi, OdemeTarihi desc, A.Id, A.Adi");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectByDateAndPaymentReason(DateTime basTar, DateTime bitTar, int? odemeSebebiId)
        {
            SqlQuery q = new SqlQuery(string.Format(@"
                SELECT A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB,
                    A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA,C.Tutar TutarB,
                    A.Aciklama AciklamaA,C.Aciklama AciklamaB
                FROM KiraEkstreAktarma_Table A
                    LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
                    LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
                    LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId
                    LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
                WHERE A.OdemeTarihi BETWEEN @BasTar AND @BitTar {0}", odemeSebebiId.HasValue ? "AND (A.OdemeSebebiId=@OdemeSebebiId OR C.OdemeSebebiId=@OdemeSebebiId)" : ""));
            q.AddParameter("@BasTar", basTar); q.AddParameter("@BitTar", bitTar);
            if (odemeSebebiId.HasValue) q.AddParameter("@OdemeSebebiId", odemeSebebiId.Value);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectSumByDateAndPaymentReason(DateTime basTar, DateTime bitTar, int? odemeSebebiId) { return SelectByDateAndPaymentReason(basTar, bitTar, odemeSebebiId); }
        private static string BuildInClause(List<int> ids) { if (ids == null || ids.Count == 0) throw new ArgumentException("ids"); return string.Join(",", ids.Select((id, index) => "@Id" + index)); }
        private static void AddIdParameters(SqlQuery q, List<int> ids) { for (int i = 0; i < ids.Count; i++) q.AddParameter("@Id" + i, ids[i]); }
        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "KiraEkstreAktarma_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "KiraEkstreAktarma_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("KiraEkstreAktarma_Table", id), ""); }
    }
}
