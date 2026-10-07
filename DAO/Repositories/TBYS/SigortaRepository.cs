using DAO.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace DAO.Repositories.TBYS
{
    public class SigortaRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public SigortaRepository() : this(new DbClass()) { }

        public SigortaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Sigorta_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Sigorta_Table"), "");
        }

        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Sigorta_Table
                WHERE TasinmazId=@TasinmazId
                ORDER BY SigortaBitTar DESC");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventoryList()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT A.Id SigortaId, E.KisaAdi SorumluBolge, A.TasinmazId,B.SorumluBolge,A.SigortaCinsi,A.AdresKodu,A.PoliceNo,A.SigortaBasTar,A.SigortaBitTar,A.YapiTarzi,A.InsaYili,
                    A.BulunduguKat,A.ToplamKatSayisi, A.BBNetAlan, A.BBBrutAlan, A.SigortaBedeli, A.Prim,A.DaskPoliceNo,
                    B.Adres+ISNULL(F.BolumNo,'') +' '+ D.IlceAdi +'-'+ C.IlAdi Adres, C.IlAdi,D.IlceAdi, D.IlceAdi +' '+ C.IlAdi IliIlcesi,
                    B.KullanimSekli, B.Cinsi, B.PaftaNo,B.AdaNo,B.ParselNo,B.SahifeNo,F.BolumNo,
                    B.TapuTasinmazNo,A.TeminatListesi,A.TeminatAciklama,A.Aciklama
                FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                LEFT JOIN BagimsizBolum_Table F ON F.Id=A.BolumId
                LEFT JOIN IL_Table C ON C.Id=B.IlId
                LEFT JOIN ILCE_Table D ON D.Id=B.IlceId
                LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE 1>0
                ORDER BY E.Id, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC"), "");
        }

        public DataTable SelectInsuranceValueTotal(string sigortaCinsi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(S.SigortaBedeli) Toplam
                FROM Sigorta_Table S
                INNER JOIN Tasinmaz_Table T ON T.Id=S.TasinmazId
                WHERE T.EnvanterdeMi=1 AND SigortaCinsi=@SigortaCinsi");
            query.AddParameter("@SigortaCinsi", sigortaCinsi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectPremiumTotal(string sigortaCinsi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT SUM(S.Prim) Toplam
                FROM Sigorta_Table S
                INNER JOIN Tasinmaz_Table T ON T.Id=S.TasinmazId
                WHERE T.EnvanterdeMi=1 AND SigortaCinsi=@SigortaCinsi");
            query.AddParameter("@SigortaCinsi", sigortaCinsi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTeminatSigortaCinsi(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000,
            bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan, int bolgeId, DateTime basTarih, DateTime bitTarih)
        {
            string tarStr = string.Format(CultureInfo.InvariantCulture, @"
                    AND A.SigortaBasTar >= {0} AND A.SigortaBasTar <= {1}", basTarih.ReturnQuotedValue(), bitTarih.ReturnQuotedValue());
            string bolgeStr = bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? string.Empty
                : string.Format(CultureInfo.InvariantCulture, "  AND E.Id={0} ", bolgeId);
            string sigortaCinsiStr = string.IsNullOrEmpty(sigortaCinsi) || sigortaCinsi.Equals(ProjeConstants.HEPSI)
                ? " AND SigortaCinsi is not null "
                : " AND SigortaCinsi = " + sigortaCinsi.ReturnQuotedValue();

            string depremStr = isDeprem ? " TeminatListesi Like '%1%'" : string.Empty;
            string yanginStr = isYangin ? " TeminatListesi Like '%2%'" : string.Empty;
            string makine100000Str = isMakine100000 ? " TeminatListesi Like '%3%'" : string.Empty;
            string makine5000Str = isMakine5000 ? " TeminatListesi Like '%4%'" : string.Empty;
            string jeneratorStr = isJenerator ? " TeminatListesi Like '%5%'" : string.Empty;
            string asansorStr = isAsansor ? " TeminatListesi Like '%6%'" : string.Empty;
            string kazanStr = isKazan ? " TeminatListesi Like '%7%'" : string.Empty;

            string teminatStr = string.Empty;
            string[] teminatlar = { depremStr, yanginStr, makine100000Str, makine5000Str, jeneratorStr, asansorStr, kazanStr };
            foreach (string teminat in teminatlar)
            {
                if (string.IsNullOrEmpty(teminat)) continue;
                teminatStr += (string.IsNullOrEmpty(teminatStr) ? string.Empty : " OR ") + teminat;
            }
            if (!string.IsNullOrEmpty(teminatStr)) teminatStr = " AND ( " + teminatStr + " ) ";

            DateTime sonGun = new DateTime(DateTime.Today.AddMonths(2).Year, DateTime.Today.AddMonths(2).Month, 1);
            string vadeStr = vadesiGelenler ? " AND SigortaBitTar <" + sonGun.ReturnTRDateFormat() : string.Empty;
            string sql = string.Format(CultureInfo.InvariantCulture, @"
                SELECT A.Id SigortaId, B.SorumluBolge,E.KisaAdi Bolge, A.TasinmazId,B.SorumluBolge,A.SigortaCinsi,A.AdresKodu,A.PoliceNo,A.SigortaBasTar,A.SigortaBitTar,
                    A.YapiTarzi,A.InsaYili, A.BulunduguKat,B.BulunduguKat, A.ToplamKatSayisi,B.ToplamKatSayisi, A.BBNetAlan, B.BBNetAlan ,A.BBBrutAlan, B.BBBrutAlan,
                    A.SigortaBedeli, A.Prim,A.DaskPoliceNo,A.BagimsizBolumNo,A.PDFDosyasi,A.Prim,
                    B.Adres+ISNULL(F.BolumNo,'') Adres, B.Ili,B.Ilcesi, B.Ilcesi +' '+ B.Ili IliIlcesi, B.KullanimSekli TasinmazKullanimSekli, B.Cinsi, B.PaftaNo,B.AdaNo,B.ParselNo,B.SahifeNo,F.BolumNo,
                    A.TeminatListesi,A.TeminatAciklama,A.Aciklama,B.EnvanterdeMi,
                    B.Adres+ISNULL(F.BolumNo,'') +' '+ B.Ilcesi+'-'+ B.Ili TamAdres,
                    B.KatMulkiyeti,A.KullanimSekli SigortaKullanimSekli,B.TapuTasinmazNo,
                    E.KisaAdi Bolge
                FROM Sigorta_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND (B.EnvanterdeMi=1 OR B.EnvanterdeMi=2)
                    LEFT JOIN BagimsizBolum_Table F ON F.Id=A.BolumId AND F.TasinmazId=B.Id
                    LEFT JOIN IL_Table C ON C.Id=B.IlId
                    LEFT JOIN ILCE_Table D ON D.Id=B.IlceId
                    LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE 1>0
                    {0}
                    {1}
                    {2}
                    {3}
                    {4}
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC
                            ", sigortaCinsiStr, teminatStr, vadeStr, bolgeStr, tarStr);
            return db.SelectFromDb(new SqlQuery(sql), "");
        }

        public DataTable SelectNavigationList()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM Sigorta_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId AND B.EnvanterdeMi=1
                ORDER BY B.SorumluBolge, B.Ili,B.Ilcesi, A.Id, SigortaBasTar DESC"), "");
        }

        public DataTable SelectMaxId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MAX(Id) Id FROM Sigorta_Table"), "");
        }

        public DataTable SelectMinId()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MIN(Id) Id FROM Sigorta_Table"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, "Sigorta_Table"));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, "Sigorta_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete("Sigorta_Table", id), "");
        }
    }
}
