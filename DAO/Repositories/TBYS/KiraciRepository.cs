using DAO.Ortak;
using System;
using System.Data;
using System.Text;
using Utility.ProjeGlobal;

namespace DAO.Repositories.TBYS
{
    public class KiraciRepository
    {
        private readonly DbClass db;

        public KiraciRepository() : this(new DbClass()) { }

        public KiraciRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Kiraci_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Kiraci_Table"), "");
        }

        public DataTable SelectActiveTenants()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT S.DosyaNo,A.*
                FROM Kiraci_Table A
                INNER JOIN KiraSozlesme_Table S ON S.KiraciId=A.Id AND S.Aktif=1
                INNER JOIN OdemePlani_Table O ON O.Id=(SELECT TOP 1 Id FROM OdemePlani_Table WHERE SozlesmeId=S.Id)
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id"), "");
        }
        public DataTable SelectAllReturnJson()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id) AS Sirano,
                    S.DosyaNo, A.Id KiraciId, Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo, A.Ilcesi +'-'+ A.Ili IlIlce, Semt,A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama,
                    S.Id SozlesmeId,S.Aktif
                FROM Kiraci_Table A
                    LEFT JOIN KiraSozlesme_Table S On S.KiraciId=A.Id AND S.Aktif=1
                    LEFT JOIN SozlesmeTasinmaz_Table C On C.Id=(Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId=S.ID)
                    LEFT JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id"), "");
        }
        public DataTable SelectAllReturnDataTable(string secim, int bolgeId)
        {
            StringBuilder sql = new StringBuilder(@"
                SELECT C.IlAdi Ili,D.IlceAdi Ilcesi,D.IlceAdi, D.IlceAdi +'-'+ C.IlAdi As IlIlce,
                    E.KisaAdi As Bolge,A.Id KiraciId, A.Adi, Soyadi, MAX(SozBasTar), COUNT(KiraciId),
                    MAX(S.Id) SozlesmeId,S.Aktif, S.KiraBedeli,S.OdemeSekli,TCKimlikNo,VergiDairesi,VergiNo, Semt,
                    A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama
                FROM KiraSozlesme_Table S RIGHT JOIN Kiraci_Table A ON A.Id=S.KiraciId
                LEFT JOIN Il_Table C ON C.Id=A.IlId LEFT JOIN Ilce_Table D ON D.Id=A.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE 1=1 ");
            SqlQuery query = new SqlQuery();
            if (!secim.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString())) { sql.Append(" AND S.Aktif=@Aktif "); query.AddParameter("@Aktif", secim); }
            if (bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT) { sql.Append(" AND S.BolgeId=@BolgeId "); query.AddParameter("@BolgeId", bolgeId); }
            sql.Append(@" GROUP BY A.Id,A.Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo,D.IlceAdi,C.IlAdi,D.IlceAdi +'-'+ C.IlAdi,E.KisaAdi,
                Semt,A.Adres,A.Telefon,A.Eposta,A.KiralamaAmaci,A.Aciklama,S.Aktif,S.KiraBedeli,S.OdemeSekli ORDER BY A.Adi ");
            query.Sql = sql.ToString();
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBolge(string aktif, string bolge)
        {
            string activeFilter = aktif.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString()) ? "" : " WHERE A.Aktif=@Aktif ";
            string regionFilter = bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? "" : (string.IsNullOrEmpty(activeFilter) ? " WHERE A.Bolge=@Bolge " : " AND A.Bolge=@Bolge ");
            SqlQuery query = new SqlQuery(string.Format(@"
                SELECT A.Id SozlesmeId,A.IlkSozlesmeTar,A.SozBasTar,A.SozBitTar,A.SozlesmeDurumu,A.KiraBedeli,A.OdemeSekli,A.TeminatTutari,A.Aktif,
                    B.Id KiraciId,B.Adi,B.Soyadi,B.Adres KiraciAdresi,B.Ili KiraciIli,B.Ilcesi KiraciIlcesi
                FROM KiraSozlesme_Table A LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId {0}{1}", activeFilter, regionFilter));
            if (!aktif.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString())) query.AddParameter("@Aktif", aktif);
            if (!bolge.Equals(ProjeConstants.BOLGE_HEPSI)) query.AddParameter("@Bolge", bolge);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectWithoutActiveContract(int bolgeId)
        {
            StringBuilder sql = new StringBuilder(@"
                SELECT A.KiraciId,B.Adi,A.Aktif,B.Soyadi,MAX(A.Id) SozlesmeId,F.IlAdi Ili,D.IlceAdi Ilcesi,D.IlceAdi,D.IlceAdi +'-'+ F.IlAdi As IlIlce,E.KisaAdi As Bolge,
                    TCKimlikNo,VergiDairesi,VergiNo,B.Semt,B.Adres,B.Telefon,B.Eposta,B.KiralamaAmaci
                FROM Kiraci_Table B INNER JOIN KiraSozlesme_Table A ON A.KiraciId=B.Id
                    LEFT JOIN Il_Table F ON F.Id=B.IlId LEFT JOIN Ilce_Table D ON D.Id=B.IlceId LEFT JOIN Bolge_Table E ON E.Id=A.BolgeId
                WHERE NOT EXISTS (SELECT 1 FROM KiraSozlesme_Table C WHERE A.KiraciId=C.KiraciId AND A.Aktif=0 AND C.Aktif=1) AND A.Aktif=0 ");
            SqlQuery query = new SqlQuery();
            if (bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT) { sql.Append(" AND A.BolgeId=@BolgeId "); query.AddParameter("@BolgeId", bolgeId); }
            sql.Append(@" GROUP BY A.KiraciId,A.Aktif,B.Adi,B.Soyadi,TCKimlikNo,VergiDairesi,VergiNo,D.IlceAdi,F.IlAdi,D.IlceAdi +'-'+ F.IlAdi,E.KisaAdi,
                B.Semt,B.Adres,B.Telefon,B.Eposta,B.KiralamaAmaci ORDER BY B.Adi ");
            query.Sql = sql.ToString();
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByFilter(string filter)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.*,B.*,B.Id SozlesmeId FROM Kiraci_Table A
                INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id AND B.Id in (SELECT Top 1 Id FROM KiraSozlesme_Table WHERE KiraciId=A.Id ORDER BY SozBitTar DESC)
                WHERE Adi like @Filter OR TCKimlikNo like @Filter OR Telefon like @Filter OR Adres like @Filter
                ORDER BY SozBasTar DESC,Aktif DESC,KiraciId");
            query.AddParameter("@Filter", "%" + filter + "%");
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByFilterSimple(string filter)
        {
            SqlQuery query = new SqlQuery(@"SELECT Id KiraciId,Adi,Soyadi,TCKimlikNo,Ili,Ilcesi,Adres,Telefon FROM Kiraci_Table
                WHERE Adi like @Filter OR TCKimlikNo like @Filter OR Telefon like @Filter OR Adres like @Filter ORDER BY KiraciId");
            query.AddParameter("@Filter", "%" + filter + "%");
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectNext(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Kiraci_Table WHERE Id > @Id ORDER BY Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectPrevious(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Kiraci_Table WHERE Id < @Id ORDER BY Id DESC"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectMax() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Kiraci_Table ORDER BY Id DESC"), ""); }
        public DataTable SelectMin() { return db.SelectFromDb(new SqlQuery("SELECT * FROM Kiraci_Table ORDER BY Id"), ""); }
        public DataTable SelectByName(string adi, string soyadi)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.* FROM Kiraci_Table A INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id
                AND B.Id IN (SELECT MAX(Id) FROM KiraSozlesme_Table WHERE KiraciId=A.Id GROUP BY KiraciId) WHERE Adi Like @Adi");
            query.AddParameter("@Adi", "%" + adi.Trim() + "%");
            if (!string.IsNullOrEmpty(soyadi)) { query.Sql += " AND Soyadi LIKE @Soyadi"; query.AddParameter("@Soyadi", "%" + soyadi + "%"); }
            query.Sql += " ORDER BY B.SozBasTar DESC";
            return db.SelectFromDb(query, "");
        }
        public int Insert<T>(T entity) { return db.Insert(new CrudQueryBuilder().BuildInsert(entity, "Kiraci_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(new CrudQueryBuilder().BuildUpdate(entity, "Kiraci_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(new CrudQueryBuilder().BuildDelete("Kiraci_Table", id), ""); }
    }
}
