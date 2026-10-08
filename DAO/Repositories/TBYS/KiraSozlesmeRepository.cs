using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.TBYS
{
    public class KiraSozlesmeRepository
    {
        private readonly DbClass db;

        public KiraSozlesmeRepository() : this(new DbClass()) { }

        public KiraSozlesmeRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }

        public DataTable SelectAllActive()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE Aktif=1
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }
        public DataTable SelectListReturnDataTable(int kiraciId, int aktif, int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND S.BolgeId={0} ", bolgeId);
            string aktifStr = aktif == ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT ? "" : " AND S.Aktif = " + aktif;
            string kiraciStr = kiraciId < 1 ? "" : " AND S.KiraciId=" + kiraciId;
            string sql = string.Format(@"
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, H.KisaAdi Bolge,S.BolgeId, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,ArtisAyi,
                    IlkSozlesmeTar,SozBasTar, SozBitTar,DATEDIFF(dd,S.SozBasTar,GETDATE()) SozlesmeBasladi,
                    S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi,
                    S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi,
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
                    S.TeminatAciklama, S.TeminatOdemeTarihi,
                    K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar, S.Aktif, S.SozlesmePDFDosyasi,K.KiralamaAmaci
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    INNER JOIN Bolge_Table H ON H.Id=S.BolgeId
                WHERE 1 > 0
                {0} {1} {2}
                ORDER BY DosyaNo, S.Id", aktifStr, kiraciStr, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectList(int kiraciId, int aktif, int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND S.BolgeId={0} ", bolgeId);
            string aktifStr = aktif == ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT ? "" : " AND S.Aktif = " + aktif;
            string kiraciStr = kiraciId < 1 ? "" : " AND S.KiraciId=" + kiraciId;
            string sql = string.Format(@"
                SELECT S.*
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    INNER JOIN Bolge_Table H ON H.Id=S.BolgeId
                WHERE 1 > 0
                {0} {1} {2}
                ORDER BY DosyaNo, S.Id", aktifStr, kiraciStr, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
    }
}
