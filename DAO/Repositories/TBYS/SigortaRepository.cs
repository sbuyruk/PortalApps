using DAO.Ortak;
using System;
using System.Data;

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
