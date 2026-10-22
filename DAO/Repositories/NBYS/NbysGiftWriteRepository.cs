using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysGiftWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;

        public NbysGiftWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "NbysGiftWriteRepository");
        }

        public DataTable Owner(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            // Read the owner first, then lock donor and gift in the same order as other NBYS writers.
            SqlQuery query = new SqlQuery("SELECT BagisciId FROM Armagan_Table WHERE Id=@Id AND BelgeGecersizMi!=1");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable Definition(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT Id FROM ArmaganTanim_Table WITH (HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public void Update(int id, int definitionId, decimal amount, string status, DateTime date,
            string description, string certificateName, bool hideAmount, string actor, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"UPDATE Armagan_Table SET ArmaganTanimId=@DefinitionId,
                BagisMiktari=@Amount,Durum=@Status,Tarih=@Date,Aciklama=@Description,
                BelgedeYazanIsim=@CertificateName,BagisMiktariYazmasin=@HideAmount,
                Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Id", id);
            query.AddParameter("@DefinitionId", definitionId);
            query.AddParameter("@Amount", amount);
            query.AddParameter("@Status", status);
            query.AddParameter("@Date", date);
            query.AddParameter("@Description", description ?? string.Empty);
            query.AddParameter("@CertificateName", certificateName ?? string.Empty);
            query.AddParameter("@HideAmount", hideAmount);
            query.AddParameter("@Actor", actor);
            query.AddParameter("@Changed", DateTime.Now);
            new NbysTransactionalWriter(context, "Armagan_Table").Update(query, id, transaction);
        }

        public void SetReturned(int id, string status, string actor, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"UPDATE Armagan_Table SET Durum=@Status,
                Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Id", id);
            query.AddParameter("@Status", status);
            query.AddParameter("@Actor", actor);
            query.AddParameter("@Changed", DateTime.Now);
            new NbysTransactionalWriter(context, "Armagan_Table").Update(query, id, transaction);
        }
    }
}
