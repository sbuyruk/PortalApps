using DAO.Ortak;
using System;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public sealed class SalaryScaleWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly UcretTanimRepository scales = new UcretTanimRepository();
        private readonly MaasHareketRepository salaries = new MaasHareketRepository();
        public SalaryScaleWriteRepository(PortalLogContext context)
        {
            if (context == null || context.Project != "IKYS" || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("IKYS kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
        }

        public DataTable Read()
        {
            return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM UcretTanim_Table ORDER BY GrupId,Id"), context, "SalaryScaleWriteRepository.Read");
        }

        public void Lock(SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=N'PortalApps.IKYS.SalaryScale',
                    @LockMode=N'Exclusive',@LockOwner=N'Transaction',@LockTimeout=15000;
                SELECT @result Result;");
            DataTable result = PortalSqlReadExecutor.Select(query, context, "SalaryScaleWriteRepository.Lock", transaction);
            if (result == null || Convert.ToInt32(result.Rows[0]["Result"]) < 0)
                throw new InvalidOperationException("Maaş tablosu işlem kilidi alınamadı.");
        }

        public DataTable Read(SqlTransactionContext transaction)
        {
            Require(transaction);
            return PortalSqlReadExecutor.Select(new SqlQuery("SELECT * FROM UcretTanim_Table WITH (UPDLOCK,HOLDLOCK) ORDER BY GrupId,Id"),
                context, "SalaryScaleWriteRepository.ReadLocked", transaction);
        }

        public DataTable ReadSalaries(int groupId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT Id,GrupId FROM MaasHareket_Table WITH (UPDLOCK,HOLDLOCK) WHERE GrupId=@Group ORDER BY Id");
            query.AddParameter("@Group", groupId);
            return PortalSqlReadExecutor.Select(query, context, "SalaryScaleWriteRepository.ReadSalaries", transaction);
        }

        public int Insert<T>(T item, SqlTransactionContext transaction)
        {
            Require(transaction);
            return scales.Insert(item, transaction, context);
        }

        public void Delete(int groupId, SqlTransactionContext transaction)
        {
            Require(transaction);
            // Delete dependants first; both tables and their audits share the caller's transaction.
            if (!salaries.DeleteByGrupId(groupId, transaction, context) || !scales.DeleteByGrupId(groupId, transaction, context))
                throw new InvalidOperationException("Maaş artışı ve bağlı maaş listeleri silinemedi.");
        }

        private static void Require(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
        }
    }
}
