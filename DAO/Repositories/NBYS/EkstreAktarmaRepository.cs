using Utility.Logging;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class EkstreAktarmaRepository
    {
        public bool HasLogContext { get { return logContext != null; } }
        private const string TableName = "EkstreAktarma_Table";
        private readonly DbClass db;
        private readonly PortalLogContext logContext;
        private readonly CrudQueryBuilder queryBuilder;

        public EkstreAktarmaRepository() : this(new DbClass()) { }

        public EkstreAktarmaRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "NBYS") throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
            logContext = context;
            queryBuilder = new CrudQueryBuilder();
        }

        public EkstreAktarmaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public EkstreAktarmaRepository(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EkstreAktarma_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
        }

        public DataTable SelectById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            SqlQuery query = new SqlQuery(
                "SELECT * FROM EkstreAktarma_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM EkstreAktarma_Table"), "");
        }

        public DataTable SelectList(DateTime processDate, bool excludeTransferred, string bankName)
        {
            string filters = excludeTransferred ? " AND AktarildiMi=0" : string.Empty;
            if (!string.IsNullOrEmpty(bankName)) filters += " AND BankaAdi=@BankaAdi";
            SqlQuery query = new SqlQuery(@"SELECT Id EkstreAktarmaId,NakitBagisciId,BankaAdi,TCKimlikNo,Adi,Soyadi,
                    ISNULL(Adi,'')+' '+ISNULL(Soyadi,'') AdiSoyadi,BagisTarihi,Tutar,DovizCinsi,
                    DovizTutari,DovizKuru,KurTarihi,AktarildiMi,Adres,Aciklama,Telefon1,Telefon2,
                    Telefon1+IIF(ISNULL(Telefon1,'')<>'' AND ISNULL(Telefon2,'')<>'',' - ','')+Telefon2 Telefon
                FROM EkstreAktarma_Table WHERE IslemTarihi=@IslemTarihi" + filters + @"
                ORDER BY AktarildiMi,BagisTarihi DESC,Id,Adi");
            query.AddParameter("@IslemTarihi", processDate);
            if (!string.IsNullOrEmpty(bankName)) query.AddParameter("@BankaAdi", bankName);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
        }

        public DataTable SelectByBankAndReceipt(string bankNamePart, string receiptNumber)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM EkstreAktarma_Table
                WHERE BankaAdi LIKE @BankaAdi AND FisNo=@FisNo");
            query.AddParameter("@BankaAdi", "%" + bankNamePart + "%");
            query.AddParameter("@FisNo", receiptNumber);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
        }

        public DataTable SelectByBankAndReceipt(string bankNamePart, string receiptNumber,
            SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"SELECT * FROM EkstreAktarma_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BankaAdi LIKE @BankaAdi AND FisNo=@FisNo");
            query.AddParameter("@BankaAdi", "%" + bankNamePart + "%");
            query.AddParameter("@FisNo", receiptNumber);
            return SelectInTransaction(query, transaction);
        }

        public DataTable SelectByIds(IList<int> ids, bool includeIdDescending)
        {
            if (ids == null || ids.Count == 0) return new DataTable();
            List<string> parameters = new List<string>();
            for (int i = 0; i < ids.Count; i++) parameters.Add("@Id" + i);
            string orderBy = includeIdDescending
                ? "ORDER BY AktarildiMi,Id DESC,BagisTarihi DESC,Adi"
                : "ORDER BY AktarildiMi,BagisTarihi DESC,Adi";
            SqlQuery query = new SqlQuery("SELECT * FROM EkstreAktarma_Table WHERE Id IN (" +
                string.Join(",", parameters) + ") " + orderBy);
            for (int i = 0; i < ids.Count; i++) query.AddParameter(parameters[i], ids[i]);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
        }

        public bool ExistsByBankAndProcessDate(string bankName, DateTime processDate)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP 1 Id FROM EkstreAktarma_Table
                WHERE BankaAdi=@BankaAdi AND IslemTarihi=@IslemTarihi AND ElleKayit<>1");
            query.AddParameter("@BankaAdi", bankName);
            query.AddParameter("@IslemTarihi", processDate);
            DataTable table = logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
            return table != null && table.Rows.Count > 0;
        }

        public DataTable SelectProcessDatesWithUploads(DateTime startDate, DateTime endDate)
        {
            SqlQuery query = new SqlQuery(@"SELECT DISTINCT IslemTarihi
                FROM EkstreAktarma_Table
                WHERE IslemTarihi >= @StartDate AND IslemTarihi < @EndDate
                    AND ElleKayit<>1
                ORDER BY IslemTarihi");
            query.AddParameter("@StartDate", startDate);
            query.AddParameter("@EndDate", endDate);
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query);
        }

        public int Insert<T>(T entity) { return LegacyWriteDb.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public int Insert<T>(T entity, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Insert(queryBuilder.BuildInsert(entity, TableName))
                : new NbysTransactionalWriter(logContext, TableName).Insert(queryBuilder.BuildInsert(entity, TableName), transaction);
        }
        public bool Update<T>(T entity) { return LegacyWriteDb.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Update<T>(T entity, SqlTransactionContext transaction)
        {
            return logContext == null ? transaction.Update(queryBuilder.BuildUpdate(entity, TableName))
                : new NbysTransactionalWriter(logContext, TableName).Update(queryBuilder.BuildUpdate(entity, TableName), NbysTransactionalWriter.Id(entity), transaction);
        }
        public bool Delete(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Update(queryBuilder.BuildDelete(TableName, id))
                : new NbysTransactionalWriter(logContext, TableName).Delete(id, transaction);
        }
        public bool Delete(int id) { return LegacyWriteDb.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
        private DataTable Select(SqlQuery query, string ignored,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return logContext == null ? db.SelectFromDb(query, "") : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query, method: method);
        }
        private DbClass LegacyWriteDb
        {
            get
            {
                if (logContext != null) throw new InvalidOperationException("Yazma işlemi için transaction gereklidir.");
                return db;
            }
        }
        private DataTable SelectInTransaction(SqlQuery query, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return logContext == null ? transaction.Select(query) : new NbysSqlExecutor(logContext, "EkstreAktarmaRepository").Select(query, transaction, method);
        }
    }
}
