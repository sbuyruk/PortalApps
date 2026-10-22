using DAO.Ortak;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysFtkWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();

        public NbysFtkWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "NbysFtkWriteRepository");
        }

        public void LockWrites(SqlTransactionContext transaction)
        {
            DataTable result = Select(new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=N'PortalApps.NBYS.FTK.Write',
                    @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
                SELECT @result Result;"), transaction);
            if (result == null || Convert.ToInt32(result.Rows[0]["Result"]) < 0)
                throw new InvalidOperationException("FTK kaydı başka bir işlem nedeniyle bekliyor. Lütfen tekrar deneyin.");
        }

        public DataTable Person(int id, SqlTransactionContext transaction)
        {
            return ById("FTKKisi_Table", id, transaction);
        }

        public DataTable Province(int id, SqlTransactionContext transaction)
        {
            return ById("Il_Table", id, transaction);
        }

        public DataTable District(int id, SqlTransactionContext transaction)
        {
            return ById("Ilce_Table", id, transaction);
        }

        public bool DuplicateIdentity(long identity, int excludedId, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery(@"SELECT Id FROM FTKKisi_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE TCKimlikNo=@Identity AND Id<>@Id");
            query.AddParameter("@Identity", identity);
            query.AddParameter("@Id", excludedId);
            return Select(query, transaction) != null;
        }

        public bool PersonHasHistory(int id, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery("SELECT Id FROM FTK_Table WITH (UPDLOCK, HOLDLOCK) WHERE KisiId=@Id");
            query.AddParameter("@Id", id);
            return Select(query, transaction) != null;
        }

        public DataTable Operations(int province, int district, SqlTransactionContext transaction)
        {
            return AtLocation("FTKIslem_Table", province, district, transaction);
        }

        public DataTable History(int province, int district, SqlTransactionContext transaction)
        {
            return AtLocation("FTK_Table", province, district, transaction);
        }

        public DataTable ActiveMembers(int province, int district, string status, SqlTransactionContext transaction)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM FTKKisi_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE Ili=@Province AND Ilcesi=@District AND UyelikDurumu=@Status ORDER BY Id");
            query.AddParameter("@Province", province);
            query.AddParameter("@District", district);
            query.AddParameter("@Status", status);
            return Select(query, transaction);
        }

        public int Save<T>(T item, string table, int id, SqlTransactionContext transaction)
        {
            NbysTransactionalWriter writer = Writer(table);
            if (id > 0) writer.Update(builder.BuildUpdate(item, table), id, transaction);
            else id = writer.Insert(builder.BuildInsert(item, table), transaction);
            return id;
        }

        public void Delete(string table, int id, SqlTransactionContext transaction)
        {
            if (!Writer(table).Delete(id, transaction)) throw new InvalidOperationException("Silinecek FTK kaydı bulunamadı.");
        }

        private NbysTransactionalWriter Writer(string table)
        {
            if (table != "FTKKisi_Table" && table != "FTKIslem_Table" && table != "FTK_Table")
                throw new ArgumentException("Geçersiz FTK kayıt türü.", "table");
            return new NbysTransactionalWriter(context, table);
        }

        private DataTable ById(string table, int id, SqlTransactionContext transaction, [CallerMemberName] string method = "")
        {
            // Table identifiers only come from the three fixed callers above.
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return Select(query, transaction, method);
        }

        private DataTable AtLocation(string table, int province, int district, SqlTransactionContext transaction, [CallerMemberName] string method = "")
        {
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE Ili=@Province AND Ilcesi=@District ORDER BY Id");
            query.AddParameter("@Province", province);
            query.AddParameter("@District", district);
            return Select(query, transaction, method);
        }

        private DataTable Select(SqlQuery query, SqlTransactionContext transaction, [CallerMemberName] string method = "")
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return sql.Select(query, transaction, method);
        }
    }
}
