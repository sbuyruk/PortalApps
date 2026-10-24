using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisciRepository
    {
        private readonly NbysSqlExecutor sqlReader = new NbysSqlExecutor("NakitBagisciRepository");
        public bool HasLogContext { get { return readContext != null; } }
        private const string TableName = "NakitBagisci_Table";

        private readonly PortalLogContext readContext;
        private readonly CrudQueryBuilder crudQueryBuilder;

        public NakitBagisciRepository()
        {
            crudQueryBuilder = new CrudQueryBuilder();
        }

        // Explicit-context writes require a supplied transaction and use centralized audit.
        public NakitBagisciRepository(PortalLogContext context)
        {
            sqlReader = new NbysSqlExecutor(context, "NakitBagisciRepository");
            if (context == null || context.Project != "NBYS")
                throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            readContext = context;
            crudQueryBuilder = new CrudQueryBuilder();
        }

        private DataTable Select(SqlQuery query,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sqlReader.Select(query, method: method);
        }

        public int Insert<T>(T entity, SqlTransactionContext transaction)
        {
            if (readContext == null) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (transaction == null) throw new ArgumentNullException("transaction");
            return new NbysTransactionalWriter(readContext, TableName).Insert(crudQueryBuilder.BuildInsert(entity, TableName), transaction);
        }

        public bool Update<T>(T entity, SqlTransactionContext transaction)
        {
            if (readContext == null) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (transaction == null) throw new ArgumentNullException("transaction");
            return new NbysTransactionalWriter(readContext, TableName).Update(crudQueryBuilder.BuildUpdate(entity, TableName), NbysTransactionalWriter.Id(entity), transaction);
        }

        // Both commands use the same connection and transaction; the identity never leaves this operation.

        public int CreateBilinmeyenBagisci<T>(
            T entity,
            string namePrefix,
            DateTime modifiedAt,
            string modifiedBy,
            SqlTransactionContext transaction)
        {
            if (readContext == null) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            SqlQuery insert = crudQueryBuilder.BuildInsert(entity, TableName);
            NormalizeLegacyNullStrings(entity, insert);
            NbysSqlExecutor sql = new NbysSqlExecutor(readContext, "NakitBagisciRepository");
            int id = sql.Insert(insert, transaction);
            if (id <= 0)
                throw new InvalidOperationException("Bilinmeyen bagisci kimligi gecersiz.");

            SqlQuery update = new SqlQuery(@"UPDATE NakitBagisci_Table
                SET Adi=@Adi, Aciklama=@Aciklama,
                    DegistirmeTarihi=@DegistirmeTarihi, Degistiren=@Degistiren
                WHERE Id=@Id");
            string idText = id.ToString(CultureInfo.InvariantCulture);
            update.AddParameter("@Id", id);
            update.AddParameter("@Adi", namePrefix + "_" + idText);
            update.AddParameter("@Aciklama", namePrefix + " Bagisçi Id= " + idText);
            update.AddParameter("@DegistirmeTarihi", modifiedAt);
            update.AddParameter("@Degistiren", modifiedBy ?? string.Empty);
            if (!sql.Write(update, "UPDATE", transaction))
                throw new InvalidOperationException("Bilinmeyen bağışçı bilgileri güncellenemedi.");
            new NbysTransactionalWriter(readContext, TableName).RecordInserted(id, transaction);
            return id;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Id = @Id");
            query.AddParameter("@Id", id);
            return Select(query);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK)
                               WHERE Id = @Id");
            query.AddParameter("@Id", id);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectAll()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table");
            return Select(query);
        }

        public DataTable SelectByTcKimlikno(long tcKimlikno)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE TCKimlikNo != 0 AND TCKimlikNo = @TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikno);
            return Select(query);
        }

        public DataTable SelectByTcKimlikno(long tcKimlikno, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK)
                               WHERE TCKimlikNo != 0 AND TCKimlikNo = @TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikno);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectImportCandidates(string fullName)
        {
            // Exact names only; phone normalization and uniqueness are checked by the service.
            SqlQuery query = new SqlQuery(@"SELECT * FROM NakitBagisci_Table
                WHERE LTRIM(RTRIM(ISNULL(Adi,'') + ' ' + ISNULL(Soyadi,''))) = @FullName");
            query.AddParameter("@FullName", fullName);
            return Select(query);
        }

        public DataTable SelectImportCandidates(string fullName, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"SELECT * FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE LTRIM(RTRIM(ISNULL(Adi,'') + ' ' + ISNULL(Soyadi,''))) = @FullName");
            query.AddParameter("@FullName", fullName);
            return SelectInTransaction(query, transaction);
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
            return Select(query);
        }

        public DataTable SelectByAdAndTelefon(string adi, string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi = @Adi AND (Telefon1 = @Telefon OR Telefon2 = @Telefon)");
            query.AddParameter("@Adi", adi ?? string.Empty);
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return Select(query);
        }

        public DataTable SelectByAdAndTelefon(
            string adi,
            string telefon,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK)
                               WHERE Adi = @Adi AND (Telefon1 = @Telefon OR Telefon2 = @Telefon)");
            query.AddParameter("@Adi", adi ?? string.Empty);
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectByTelefon(string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Telefon1 = @Telefon OR Telefon2 = @Telefon");
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return Select(query);
        }

        public DataTable SelectByEposta(string eposta)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE EPosta = @Eposta");
            query.AddParameter("@Eposta", eposta ?? string.Empty);
            return Select(query);
        }

        public DataTable SelectByAd(string adi)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi LIKE @Adi");
            query.AddParameter("@Adi", "%" + adi + "%");
            return Select(query);
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
            return Select(query);
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
            return Select(query);
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
            return Select(query);
        }

        public DataTable SelectByFilter(string filter, int eksiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,ISNULL(Soyadi,'') Soyadi, 
                    TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
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

            return Select(query);
        }

        private static object NormalizeLegacyDateParameter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            if (value.Length >= 2 && value[0] == '\'' && value[value.Length - 1] == '\'')
                return value.Substring(1, value.Length - 2);

            return value;
        }

        private static void NormalizeLegacyNullStrings<T>(T entity, SqlQuery query)
        {
            Type entityType = entity.GetType();
            foreach (System.Data.SqlClient.SqlParameter parameter in query.Parameters)
            {
                string propertyName = parameter.ParameterName.TrimStart('@');
                System.Reflection.PropertyInfo property = entityType.GetProperty(propertyName);
                if (property != null
                    && property.PropertyType == typeof(string)
                    && property.GetValue(entity, null) == null)
                {
                    parameter.Value = string.Empty;
                }
            }
        }
        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return sqlReader.Select(query, transaction, method);
        }
    }
}
