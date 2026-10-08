using DAO.Ortak;
using System;
using System.Data;
using System.Text;

namespace DAO.Repositories.TBYS
{
    public class OdemeRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public OdemeRepository()
            : this(new DbClass())
        {
        }

        public OdemeRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Odeme_Table"), "");
        }

        public DataTable SelectByKiraciId(int id)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE KiraciId=@KiraciId ORDER BY OdemeTarihi DESC");
            q.AddParameter("@KiraciId", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId)
        {
            SqlQuery q = new SqlQuery("SELECT * FROM Odeme_Table WHERE SozlesmeId=@SozlesmeId AND OdemePlaniId=@OdemePlaniId ORDER BY OdemeTarihi");
            q.AddParameter("@SozlesmeId", sozlesmeId);
            q.AddParameter("@OdemePlaniId", planId);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByKiraciAyYil(int kiraciId, int ay, int yil)
        {
            StringBuilder sql = new StringBuilder(@"
                SELECT A.Id, B.Adi KiraciAdiSoyadi, F.Bolge,
                    A.Id OdemeId, A.OdemeTarihi, A.OdenenTutar, A.Aciklama, A.SozlesmeId, A.KiraciId, A.OdemePlaniId, B.KiralamaAmaci
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
                    INNER JOIN SozlesmeTasinmaz_Table D ON D.Id=(Select TOP 1 Id From SozlesmeTasinmaz_Table WHERE SozlesmeId=A.SozlesmeId)
                    INNER JOIN Tasinmaz_Table E ON E.Id=D.TasinmazId
                    INNER JOIN Il_Table F ON F.IlAdi= E.Ili
                WHERE 1=1 ");
            SqlQuery query = new SqlQuery();
            if (ay != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND MONTH(A.OdemeTarihi)=@Ay AND YEAR(A.OdemeTarihi)=@Yil ");
                query.AddParameter("@Ay", ay);
                query.AddParameter("@Yil", yil);
            }
            else if (ay == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND YEAR(A.OdemeTarihi)=@Yil ");
                query.AddParameter("@Yil", yil);
            }
            else if (ay != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND MONTH(A.OdemeTarihi)=@Ay ");
                query.AddParameter("@Ay", ay);
            }
            if (kiraciId > 0)
            {
                sql.Append(" AND A.KiraciId=@KiraciId ");
                query.AddParameter("@KiraciId", kiraciId);
            }
            sql.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
            query.Sql = sql.ToString();
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByKiraciAyYilReturnDataTable(int bolgeId, int kiraciId, DateTime basTarih, DateTime bitTarih)
        {
            StringBuilder sql = new StringBuilder(@"
                SELECT A.Id, A.Id OdemeId,A.OdemePlaniId,A.SozlesmeId,A.KiraciId,
                    A.OdemeTarihi, A.OdenenTutar, A.Aciklama,
                    B.Adi, B.Soyadi, B.KiralamaAmaci,
                    C.SozBasTar, C.SozBitTar, C.IlkSozlesmeTar,C.DosyaNo, C.ArtisAyi,C.KiraBedeli,C.OdemeSekli,
                    D.VadeBitTar, E.Id TeminatId,
                    H.KisaAdi Bolge
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
                    INNER JOIN OdemePlani_Table D ON D.Id=A.OdemePlaniId
                    LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
                    LEFT JOIN Bolge_Table H ON H.Id=C.BolgeId
                WHERE 1=1 ");
            SqlQuery query = new SqlQuery();
            sql.Append(" AND A.OdemeTarihi BETWEEN @BasTarih AND @BitTarih ");
            query.AddParameter("@BasTarih", basTarih);
            query.AddParameter("@BitTarih", bitTarih);
            if (kiraciId > 0)
            {
                sql.Append(" AND A.KiraciId=@KiraciId ");
                query.AddParameter("@KiraciId", kiraciId);
            }
            if (bolgeId != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && bolgeId != Utility.ProjeGlobal.ProjeConstants.BOLGE_GENELMUDURLUK_INT)
            {
                sql.Append(" AND C.BolgeId=@BolgeId ");
                query.AddParameter("@BolgeId", bolgeId);
            }
            sql.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
            query.Sql = sql.ToString();
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByAyYilReturnDataTable(int ay, int yil)
        {
            StringBuilder sql = new StringBuilder(@"
                SELECT
                    A.OdemeTarihi, A.OdenenTutar, A.Aciklama,
                    B.Adi, B.Soyadi
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
                    LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
                WHERE 1=1 ");
            SqlQuery query = new SqlQuery();
            if (ay != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND MONTH(A.OdemeTarihi)=@Ay AND YEAR(A.OdemeTarihi)=@Yil ");
                query.AddParameter("@Ay", ay);
                query.AddParameter("@Yil", yil);
            }
            else if (ay == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND YEAR(A.OdemeTarihi)=@Yil ");
                query.AddParameter("@Yil", yil);
            }
            else if (ay != Utility.ProjeGlobal.ProjeConstants.HEPSI_INT && yil == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT)
            {
                sql.Append(" AND MONTH(A.OdemeTarihi)=@Ay ");
                query.AddParameter("@Ay", ay);
            }
            sql.Append(" ORDER BY A.OdemeTarihi DESC, B.Id, A.SozlesmeId ");
            query.Sql = sql.ToString();
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime basTarih, DateTime bitTarih)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM Odeme_Table A
                WHERE KiraciId=@KiraciId AND SozlesmeId=@SozlesmeId
                    AND OdemeTarihi BETWEEN @BasTarih AND @BitTarih
                ORDER BY A.OdemeTarihi ");
            query.AddParameter("@KiraciId", kiraciId);
            query.AddParameter("@SozlesmeId", sozlesmeId);
            query.AddParameter("@BasTarih", basTarih);
            query.AddParameter("@BitTarih", bitTarih);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(OdenenTutar) Toplam FROM Odeme_Table
                WHERE SozlesmeId=@SozlesmeId
                    AND OdemePlaniId=@OdemePlaniId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            query.AddParameter("@OdemePlaniId", odemePlaniId);
            return db.SelectFromDb(query, "");
        }

        public bool DeleteBySozlesmeId(int id)
        {
            SqlQuery q = new SqlQuery("DELETE Odeme_Table WHERE SozlesmeId=@SozlesmeId");
            q.AddParameter("@SozlesmeId", id);
            return db.DeleteFromDb(q, "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Odeme_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Odeme_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Odeme_Table", id), "");
        }
    }
}
