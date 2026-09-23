using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisciRepository
    {
        private const string TableName = "NakitBagisci_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder crudQueryBuilder;

        public NakitBagisciRepository()
            : this(new DbClass())
        {
        }

        public NakitBagisciRepository(DbClass db)
        {
            this.db = db;
            crudQueryBuilder = new CrudQueryBuilder();
        }

        public int Insert<T>(T entity)
        {
            SqlQuery query = crudQueryBuilder.BuildInsert(entity, TableName);
            return db.Insert(query);
        }

        public bool Update<T>(T entity)
        {
            SqlQuery query = crudQueryBuilder.BuildUpdate(entity, TableName);
            return db.Update2Db(query);
        }

        public bool Delete(int id)
        {
            SqlQuery query = crudQueryBuilder.BuildDelete(TableName, id);
            return db.DeleteFromDb(query, "");
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Id = @Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTcKimlikno(long tcKimlikno)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE TCKimlikNo != 0 AND TCKimlikNo = @TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikno);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBagisiOlmayanBagisciById(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id, A.Adi, B.BagisTarihi, B.BagisMiktari
                FROM NakitBagisci_Table A
                LEFT JOIN NakitBagisHareket_Table B ON B.BagisciId = A.Id
                WHERE A.Id = @BagisciId
                    AND B.BagisciId IS NULL
                ORDER BY BagisTarihi DESC");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByAdAndTelefon(string adi, string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi = @Adi AND (Telefon1 = @Telefon OR Telefon2 = @Telefon)");
            query.AddParameter("@Adi", adi ?? string.Empty);
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTelefon(string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Telefon1 = @Telefon OR Telefon2 = @Telefon");
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByEposta(string eposta)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE EPosta = @Eposta");
            query.AddParameter("@Eposta", eposta ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByAd(string adi)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi LIKE @Adi");
            query.AddParameter("@Adi", "%" + adi + "%");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIl(int? ilId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id NakitBagisciId
	                ,ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano
                    ,Adi
                    ,Soyadi
                    ,TCKimlikNo
                    ,B.IlAdi Ili
                    ,C.IlceAdi Ilcesi
                    ,Adres
                    ,Telefon1
                    ,Telefon2
                    ,TuzelKisi
                    ,Sag
                    ,Eposta
                    ,PostaKodu
                    ,A.Aciklama
                    ,Ulasilamiyor,BelgeIstemiyor
                FROM NakitBagisci_Table A
	                LEFT OUTER JOIN Il_Table B ON B.Id = A.Ili
	                LEFT OUTER JOIN Ilce_Table C ON C.Id = A.Ilcesi AND C.IlId = B.Id
                WHERE @IlId IS NULL OR A.Ili = @IlId");
            query.AddParameter("@IlId", ilId.HasValue ? (object)ilId.Value : DBNull.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIlBagisTarihi(int? ilId, string baslangicTarihi, string bitisTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT DISTINCT(A.Id) NakitBagisciId
                    ,Adi,Soyadi,TCKimlikNo,Adres,Telefon1,Telefon2,TuzelKisi
                    ,Ulasilamiyor,BelgeIstemiyor,Sag,Eposta,PostaKodu,A.Aciklama
                    ,B.IlAdi Ili
                    ,C.IlceAdi Ilcesi
                FROM NakitBagisci_Table A
                    LEFT JOIN Il_Table B ON B.Id = A.Ili
                    LEFT JOIN Ilce_Table C ON C.Id = A.Ilcesi AND C.IlId = B.Id
                    INNER JOIN NakitBagisHareket_Table D ON D.BagisciId = A.Id
                WHERE D.BagisTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi
                    AND (@IlId IS NULL OR A.Ili = @IlId)
                ORDER BY NakitBagisciId");
            query.AddParameter("@BaslangicTarihi", NormalizeLegacyDateParameter(baslangicTarihi));
            query.AddParameter("@BitisTarihi", NormalizeLegacyDateParameter(bitisTarihi));
            query.AddParameter("@IlId", ilId.HasValue ? (object)ilId.Value : DBNull.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIlBagisTarihiYeni(int? ilId, string baslangicTarihi, string bitisTarihi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT distinct(N.Id) NakitBagisciId
                    ,Adi
                    ,Soyadi
                    ,TCKimlikNo
                    ,Ili
                    ,Ilcesi
                    ,Adres
                    ,Telefon1
                    ,Telefon2
                    ,TuzelKisi
                    ,N.OlusturmaTarihi
                    ,N.Olusturan
                    ,N.DegistirmeTarihi
                    ,N.Degistiren
                    ,Sag
                    ,Eposta
                    ,PostaKodu
                    ,N.Aciklama
                    ,Ulasilamiyor,BelgeIstemiyor
                FROM NakitBagisci_Table N
	                LEFT OUTER JOIN Il_Table ON Il_Table.Id = N.Ili
	                LEFT OUTER JOIN Ilce_Table ON Ilce_Table.Id = N.Ilcesi AND Ilce_Table.IlId = Il_Table.Id
                WHERE N.Id IN
                    (SELECT BagisciId FROM NakitBagisHareket_Table
                     WHERE BagisTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi)
                AND N.Id NOT IN
                    (SELECT A.BagisciId
                     FROM NakitBagisHareket_Table B, NakitBagisHareket_Table A
                     WHERE A.BagisTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi
                        AND B.BagisTarihi < @BaslangicTarihi
                        AND A.BagisciId = B.BagisciId)
                AND (@IlId IS NULL OR N.Ili = @IlId)
                ORDER BY N.Id");
            query.AddParameter("@BaslangicTarihi", NormalizeLegacyDateParameter(baslangicTarihi));
            query.AddParameter("@BitisTarihi", NormalizeLegacyDateParameter(bitisTarihi));
            query.AddParameter("@IlId", ilId.HasValue ? (object)ilId.Value : DBNull.Value);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilter(string filter, int eksiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon,
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor
                FROM NakitBagisci_Table N
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                WHERE N.Id != @ExcludedId
                    AND (
                        N.Adi LIKE @Filter
	                    OR N.TCKimlikNo LIKE @Filter
	                    OR N.Telefon1 LIKE @Filter
	                    OR N.Adres LIKE @Filter
                    )
                ORDER BY NakitBagisciId ");

            query.AddParameter("@ExcludedId", eksiId);
            query.AddParameter("@Filter", "%" + filter + "%");

            return db.SelectFromDb(query, "");
        }

        private static object NormalizeLegacyDateParameter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            if (value.Length >= 2 && value[0] == '\'' && value[value.Length - 1] == '\'')
                return value.Substring(1, value.Length - 2);

            return value;
        }
    }
}
