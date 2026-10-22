using DAO.Ortak;
using System;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysDocumentWriteRepository
    {
        private readonly PortalLogContext context;
        public NbysDocumentWriteRepository(PortalLogContext context) { this.context = context; }

        public void MarkSent(int id, string status, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery(@"UPDATE Armagan_Table SET Durum=@Status,
                Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Id", id);
            query.AddParameter("@Status", status);
            query.AddParameter("@Actor", context.UserLogin);
            query.AddParameter("@Changed", DateTime.Now);
            new NbysTransactionalWriter(context, "Armagan_Table").Update(query, id, transaction);
        }
    }
}
