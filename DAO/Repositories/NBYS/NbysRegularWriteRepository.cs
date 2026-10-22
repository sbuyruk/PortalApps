using DAO.Ortak;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysRegularWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();
        public NbysRegularWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "NbysRegularWriteRepository");
        }

        public DataTable Donor(int id, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query, transaction);
        }

        public DataTable Regular(int id, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM DuzenliNakitBagisci_Table WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query, transaction);
        }

        public DataTable ActiveForDonor(int id, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM DuzenliNakitBagisci_Table WITH (UPDLOCK, HOLDLOCK) WHERE Aktif=1 AND BagisciId=@Id ORDER BY Id");
            query.AddParameter("@Id", id);
            return Select(query, transaction);
        }

        public DataTable Candidates(string kind, long identity, string name, string phone, string email, SqlTransactionContext transaction)
        {
            string filter;
            switch (kind)
            {
                case "Identity": filter = "TCKimlikNo=@Identity"; break;
                case "NamePhone": filter = "Adi=@Name AND (Telefon1=@Phone OR Telefon2=@Phone)"; break;
                case "Phone": filter = "Telefon1=@Phone OR Telefon2=@Phone"; break;
                case "Email": filter = "EPosta=@Email"; break;
                default: throw new ArgumentException("Geçersiz eşleştirme türü.", "kind");
            }
            SqlQuery query = new SqlQuery("SELECT * FROM NakitBagisci_Table WITH (UPDLOCK, HOLDLOCK) WHERE " + filter + " ORDER BY Id");
            query.AddParameter("@Identity", identity);
            query.AddParameter("@Name", name);
            query.AddParameter("@Phone", phone);
            query.AddParameter("@Email", email);
            return Select(query, transaction);
        }

        public DataTable Certificate(int donorId, int definitionId, int number, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP (1) * FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@DonorId AND ArmaganTanimId=@DefinitionId AND ISNULL(BelgeGecersizMi,0)!=1
                    AND ISNULL(NULLIF(KacinciBelge,0),1)=@Number ORDER BY Id DESC");
            query.AddParameter("@DonorId", donorId);
            query.AddParameter("@DefinitionId", definitionId);
            query.AddParameter("@Number", number);
            return Select(query, transaction);
        }

        public DataTable LastDonation(int donorId, DateTime start, SqlTransactionContext transaction)
        {
            // Preserve the existing inclusive donation-year interval.
            SqlQuery query = new SqlQuery(@"SELECT TOP (1) * FROM NakitBagisHareket_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@DonorId AND BagisTarihi>=@Start AND BagisTarihi<=@End ORDER BY BagisTarihi DESC,Id DESC");
            query.AddParameter("@DonorId", donorId);
            query.AddParameter("@Start", start);
            query.AddParameter("@End", start.AddYears(1));
            return Select(query, transaction);
        }

        public DataTable DonationForCertificate(int donorId, int giftId, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP (1) * FROM NakitBagisHareket_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@DonorId AND ArmaganId=@GiftId ORDER BY BagisTarihi DESC,Id DESC");
            query.AddParameter("@DonorId", donorId);
            query.AddParameter("@GiftId", giftId);
            return Select(query, transaction);
        }

        public bool HasRefundGift(int donorId, string status, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery("SELECT Id FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK) WHERE BagisciId=@DonorId AND Durum=@Status");
            query.AddParameter("@DonorId", donorId);
            query.AddParameter("@Status", status);
            return Select(query, transaction) != null;
        }

        public int Save<T>(T item, string table, int id, SqlTransactionContext transaction)
        {
            if (table != "DuzenliNakitBagisci_Table" && table != "NakitBagisHareket_Table" && table != "Armagan_Table")
                throw new ArgumentException("Geçersiz düzenli bağış kayıt türü.", "table");
            NbysTransactionalWriter writer = new NbysTransactionalWriter(context, table);
            if (id > 0) writer.Update(builder.BuildUpdate(item, table), id, transaction);
            else id = writer.Insert(builder.BuildInsert(item, table), transaction);
            return id;
        }

        private DataTable Select(SqlQuery query, SqlTransactionContext transaction, [CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return sql.Select(query, transaction, method);
        }
    }
}
