using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysReportWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;

        public NbysReportWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "NbysReportWriteRepository");
        }

        public DataTable EligibleDonations(int donorId, SqlTransactionContext transaction)
        {
            Require(transaction);
            // Preserve the report's eligibility rule: donations already linked to medals are excluded.
            SqlQuery query = new SqlQuery(@"SELECT A.* FROM NakitBagisHareket_Table A WITH (UPDLOCK,HOLDLOCK)
                LEFT JOIN Armagan_Table B WITH (HOLDLOCK) ON B.Id=A.ArmaganId AND B.ArmaganTanimId IN (2,3,4)
                WHERE A.BagisciId=@Id AND B.Id IS NULL ORDER BY A.Id");
            query.AddParameter("@Id", donorId);
            return sql.Select(query, transaction);
        }

        public DataTable PropertyDonation(int id, SqlTransactionContext transaction)
        {
            Require(transaction);
            SqlQuery query = new SqlQuery("SELECT * FROM Bagis_Table WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public bool CanAccessPropertyDonor(int donorId, int regionId, SqlTransactionContext transaction)
        {
            Require(transaction);
            SqlQuery query = new SqlQuery(@"SELECT D.Id FROM TasinmazBagisci_Table D WITH (HOLDLOCK)
                INNER JOIN Il_Table I WITH (HOLDLOCK) ON I.Id=D.IlId
                    OR ((D.IlId IS NULL OR D.IlId<=0) AND I.IlAdi=D.Ili)
                WHERE D.Id=@Id AND I.BolgeId=@RegionId");
            query.AddParameter("@Id", donorId);
            query.AddParameter("@RegionId", regionId);
            return sql.Select(query, transaction) != null;
        }

        public void SetMagazine(int id, bool doNotSend, string actor, SqlTransactionContext transaction)
        {
            Require(transaction);
            SqlQuery query = new SqlQuery(@"UPDATE NakitBagisci_Table SET DergiGonderilmesin=@Value,
                Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Value", doNotSend);
            Stamp(query, id, actor);
            new NbysTransactionalWriter(context, "NakitBagisci_Table").Update(query, id, transaction);
        }

        public void SetPropertyGift(int id, string number, string status, DateTime date, string description,
            string actor, SqlTransactionContext transaction)
        {
            Require(transaction);
            SqlQuery query = new SqlQuery(@"UPDATE Bagis_Table SET ArmaganId=@Number,ArmaganDurumu=@Status,
                ArmaganTarihi=@Date,ArmaganAciklama=@Description,Degistiren=@Actor,DegistirmeTarihi=@Changed WHERE Id=@Id");
            query.AddParameter("@Number", number ?? string.Empty);
            query.AddParameter("@Status", status);
            query.AddParameter("@Date", date);
            query.AddParameter("@Description", description ?? string.Empty);
            Stamp(query, id, actor);
            new NbysTransactionalWriter(context, "Bagis_Table").Update(query, id, transaction);
        }

        private static void Stamp(SqlQuery query, int id, string actor)
        {
            query.AddParameter("@Id", id);
            query.AddParameter("@Actor", actor);
            query.AddParameter("@Changed", DateTime.Now);
        }

        private static void Require(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
        }
    }
}
