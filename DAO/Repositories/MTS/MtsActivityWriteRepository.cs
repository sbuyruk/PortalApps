using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public sealed class MtsActivityWriteRepository
    {
        private readonly MtsSqlExecutor sql;
        public MtsContactWriteRepository Records { get; private set; }
        public MtsActivityWriteRepository(PortalLogContext context)
        {
            sql = new MtsSqlExecutor(context, "MtsActivityWriteRepository");
            Records = new MtsContactWriteRepository(context);
        }
        public DataTable Participants(int activityId, int personId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"SELECT * FROM FaaliyetKatilim_Table WITH (UPDLOCK,HOLDLOCK)
                WHERE FaaliyetId=@Activity AND (@Person=0 OR KatilimciId=@Person) ORDER BY Id");
            query.AddParameter("@Activity", activityId); query.AddParameter("@Person", personId);
            return sql.Select(query, transaction);
        }
        public DataTable Distributions(int activityId, int personId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"SELECT * FROM AniObjesiDagitim_Table WITH (UPDLOCK,HOLDLOCK)
                WHERE FaaliyetId=@Activity AND (@Person=0 OR KatilimciId=@Person) ORDER BY Id");
            query.AddParameter("@Activity", activityId); query.AddParameter("@Person", personId);
            return sql.Select(query, transaction);
        }
        public bool HasCalls(int activityId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT Id FROM AramaGorusme_Table WITH (UPDLOCK,HOLDLOCK) WHERE FaaliyetId=@Id");
            query.AddParameter("@Id", activityId);
            return sql.Select(query, transaction) != null;
        }
        public DataTable ByUniqueId(Guid uniqueId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT * FROM Faaliyet_Table WITH (UPDLOCK,HOLDLOCK) WHERE UniqueId=@Id ORDER BY Id");
            query.AddParameter("@Id", uniqueId);
            return sql.Select(query, transaction);
        }
        public DataTable Stock(int warehouseId, int itemId, SqlTransactionContext transaction)
        {
            Require(transaction);
            // Zero stock is still an existing row and must be used when accepting returns.
            var query = new SqlQuery(@"SELECT * FROM DepoStok_Table WITH (UPDLOCK,HOLDLOCK)
                WHERE DepoId=@Warehouse AND AniObjesiId=@Item ORDER BY Id");
            query.AddParameter("@Warehouse", warehouseId); query.AddParameter("@Item", itemId);
            return sql.Select(query, transaction);
        }
        public DataTable Item(int itemId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT * FROM AniObjesiTanim_Table WITH (HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", itemId);
            return sql.Select(query, transaction);
        }
        public bool WarehouseExists(int warehouseId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT Id FROM DepoTanim_Table WITH (HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", warehouseId);
            return sql.Select(query, transaction) != null;
        }
        private static void Require(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }
    }
}
