using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class ArmaganRepository
    {
        private const string TableName = "Armagan_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public ArmaganRepository() : this(new DbClass())
        {
        }

        public ArmaganRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Armagan_Table WHERE BelgeGecersizMi!=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Armagan_Table WHERE BelgeGecersizMi!=1"), "");
        }

        public DataTable SelectByBagisciIdDateRange(int bagisciId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId
                    AND Tarih BETWEEN @Baslangic AND @Bitis");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdTanimIdDateRange(
            int bagisciId, int armaganTanimId, DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId
                    AND ArmaganTanimId=@ArmaganTanimId
                    AND Tarih BETWEEN @Baslangic AND @Bitis");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@ArmaganTanimId", armaganTanimId);
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciId(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdAndDurum(int bagisciId, string durum)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table
                WHERE BagisciId=@BagisciId AND Durum=@Durum");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@Durum", durum);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId
                    AND ArmaganTanimId=@ArmaganTanimId");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@ArmaganTanimId", armaganTanimId);
            return db.SelectFromDb(query, "");
        }

        public int CountByBagisciIdAndTanimId(int bagisciId, int armaganTanimId)
        {
            SqlQuery query = new SqlQuery(@"SELECT COUNT(*) FROM Armagan_Table
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId
                    AND ArmaganTanimId=@ArmaganTanimId");
            query.AddParameter("@BagisciId", bagisciId);
            query.AddParameter("@ArmaganTanimId", armaganTanimId);
            DataTable table = db.SelectFromDb(query, "");
            return table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0][0]);
        }

        public bool UpdateDurumByBolge(
            string fromDurum, string toDurum, string baslangic, string bitis,
            int armaganTanimId, int? bolgeId)
        {
            string bolgeKosulu = bolgeId.HasValue ? " AND Il_Table.BolgeId=@BolgeId" : string.Empty;
            SqlQuery query = new SqlQuery(@"UPDATE A SET Durum=@ToDurum
                FROM Armagan_Table A
                INNER JOIN ArmaganTanim_Table ON A.ArmaganTanimId=ArmaganTanim_Table.Id
                INNER JOIN NakitBagisci_Table ON A.BagisciId=NakitBagisci_Table.Id
                INNER JOIN Il_Table ON Il_Table.Id=NakitBagisci_Table.Ili
                WHERE A.BelgeGecersizMi!=1 AND A.Durum=@FromDurum
                    AND A.Tarih BETWEEN @Baslangic AND @Bitis
                    AND A.ArmaganTanimId=@ArmaganTanimId" + bolgeKosulu);
            query.AddParameter("@ToDurum", toDurum);
            query.AddParameter("@FromDurum", fromDurum);
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            query.AddParameter("@ArmaganTanimId", armaganTanimId);
            if (bolgeId.HasValue)
                query.AddParameter("@BolgeId", bolgeId.Value);
            return db.Update2Db(query);
        }

        public DataTable SelectByDurumTarih(
            string durum, DateTime baslangic, DateTime bitis, int? armaganTanimId,
            int? bolgeId, int? ilId)
        {
            string durumKosulu = durum == null ? string.Empty : " AND A.Durum=@Durum";
            string tanimKosulu = armaganTanimId.HasValue ? " AND A.ArmaganTanimId=@ArmaganTanimId" : string.Empty;
            string bolgeKosulu = bolgeId.HasValue
                ? " AND B.Ili IN (SELECT Id FROM Il_Table WHERE BolgeId=@BolgeId)" : string.Empty;
            string ilKosulu = ilId.HasValue ? " AND B.Ili=@IlId" : string.Empty;
            SqlQuery query = new SqlQuery(@"SELECT DISTINCT(A.Id) ArmaganId,
                    ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano,
                    B.Id NakitBagisciId, B.Adi NakitBagisciAdi,
                    B.TCKimlikNo NakitBagisciTC, B.Adres,
                    IIF(ISNULL(Telefon1,'')!='',Telefon1,IIF(ISNULL(Telefon2,'')!='',Telefon2,'')) Telefon,
                    E.IlAdi, E.Id IlId, F.IlceAdi,
                    CONVERT(nvarchar,REPLACE(A.BagisMiktari,'.',',')) Tutar,
                    D.Armagan ArmaganBaslik, A.BagisciId, ArmaganTanimId, Tarih,
                    CONVERT(varchar,FORMAT(Tarih,'dd.MM.yyyy')) ArmaganTarihi,
                    FORMAT(A.BagisMiktari,'N2','tr-TR') ArmaganTutari,
                    A.Durum, ISNULL(BelgedeYazanIsim,'') BelgedeYazanIsim,
                    A.BelgeGecersizMi, A.IadeMiktari, A.DovizCinsi,
                    A.BagisMiktariYazmasin,
                    IIF(A.DuzenliBagis=1,'Düzenli Bağış',IIF(A.CokluBagis=1,'Çoklu Bağış','Bağış')) CokluBagis
                FROM Armagan_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                LEFT JOIN ArmaganTanim_Table D ON D.Id=A.ArmaganTanimId
                LEFT JOIN Il_Table E ON E.Id=B.Ili
                LEFT JOIN Ilce_Table F ON F.Id=B.Ilcesi AND F.IlId=E.Id
                WHERE A.Tarih BETWEEN @Baslangic AND @Bitis" + durumKosulu + tanimKosulu + bolgeKosulu + ilKosulu +
                " ORDER BY A.Id");
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            if (durum != null) query.AddParameter("@Durum", durum);
            if (armaganTanimId.HasValue) query.AddParameter("@ArmaganTanimId", armaganTanimId.Value);
            if (bolgeId.HasValue) query.AddParameter("@BolgeId", bolgeId.Value);
            if (ilId.HasValue) query.AddParameter("@IlId", ilId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountDurumByBolge(
            DateTime baslangic, DateTime bitis, int armaganTanimId, int? bolgeId)
        {
            string bolgeKosulu = bolgeId.HasValue
                ? " AND N.Ili IN (SELECT Id FROM Il_Table WHERE BolgeId=@BolgeId)" : string.Empty;
            SqlQuery query = new SqlQuery(@"SELECT A.Durum,COUNT(A.Durum) Adet
                FROM Armagan_Table A
                INNER JOIN NakitBagisci_Table N ON N.Id=A.BagisciId
                WHERE A.BelgeGecersizMi!=1 AND A.ArmaganTanimId=@ArmaganTanimId
                    AND A.Tarih BETWEEN @Baslangic AND @Bitis" + bolgeKosulu +
                " GROUP BY A.Durum");
            query.AddParameter("@ArmaganTanimId", armaganTanimId);
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            if (bolgeId.HasValue) query.AddParameter("@BolgeId", bolgeId.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectCountByBagisTarihiBolge(DateTime baslangic, DateTime bitis)
        {
            SqlQuery query = new SqlQuery(@"SELECT COUNT(A.Id) Adet,D.Id BolgeId,
                    A.ArmaganTanimId ArmaganTanimId
                FROM Armagan_Table A
                INNER JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                LEFT JOIN Bolge_Table D ON D.Id=C.BolgeId
                WHERE A.Tarih BETWEEN @Baslangic AND @Bitis
                GROUP BY D.Id,A.ArmaganTanimId");
            query.AddParameter("@Baslangic", baslangic);
            query.AddParameter("@Bitis", bitis);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilter(string filter, int excludedDonorId)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.Id ArmaganId,B.Armagan,A.Durum,A.Tarih,
                    CONVERT(varchar,FORMAT(A.Tarih,'dd.MM.yyyy')) BelgeTarihi,
                    A.BagisMiktari,C.Id NakitBagisciId,C.Adi,C.Soyadi,C.TCKimlikNo,
                    D.IlAdi Ili,E.IlceAdi Ilcesi,Adres,Telefon1,Telefon2,
                    Telefon1+IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','')+Telefon2 Telefon,
                    C.OlusturmaTarihi,C.DegistirmeTarihi,C.Degistiren,Sag,Eposta,
                    PostaKodu,Ulasilamiyor,BelgeIstemiyor
                FROM Armagan_Table A
                LEFT JOIN ArmaganTanim_Table B ON B.Id=A.ArmaganTanimId
                INNER JOIN NakitBagisci_Table C ON C.Id=A.BagisciId
                LEFT JOIN Il_Table D ON D.Id=C.Ili
                LEFT JOIN Ilce_Table E ON E.Id=C.Ilcesi AND E.IlId=D.Id
                WHERE A.BelgeGecersizMi!=1 AND C.Id!=@ExcludedDonorId AND A.Id=@Filter
                ORDER BY A.Tarih DESC");
            query.AddParameter("@ExcludedDonorId", excludedDonorId);
            query.AddParameter("@Filter", filter);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectVerilenArmaganlarGroupByBagisci()
        {
            return db.SelectFromDb(new SqlQuery(@"SELECT A.BagisciId,B.Adi,B.Soyadi,
                    D.Armagan,COUNT(C.Id) BagisAdedi,SUM(C.BagisMiktari) ToplamBagis,A.Tarih
                FROM Armagan_Table A
                INNER JOIN NakitBagisci_Table B ON A.BagisciId=B.Id
                INNER JOIN NakitBagisHareket_Table C ON C.ArmaganId=A.Id
                INNER JOIN ArmaganTanim_Table D ON D.Id=A.ArmaganTanimId
                WHERE A.BelgeGecersizMi!=1 AND A.CokluBagis=1
                GROUP BY A.BagisciId,B.Adi,B.Soyadi,C.ArmaganId,A.Tarih,D.Armagan
                ORDER BY A.Tarih DESC"), "");
        }

        public int Insert<T>(T entity)
        {
            return db.Insert(queryBuilder.BuildInsert(entity, TableName));
        }

        public bool Update<T>(T entity)
        {
            return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), "");
        }
    }
}
