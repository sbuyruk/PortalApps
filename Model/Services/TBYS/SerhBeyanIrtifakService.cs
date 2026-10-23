using DAO.Repositories.TBYS;
using DAO.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class SerhBeyanIrtifakService
    {
        private readonly SerhBeyanIrtifakRepository repository;
        private readonly PortalLogContext context;

        public SerhBeyanIrtifakService() : this(new SerhBeyanIrtifakRepository()) { }
        public SerhBeyanIrtifakService(PortalLogContext context) : this(new SerhBeyanIrtifakRepository(context)) { this.context = context; }

        public SerhBeyanIrtifakService(SerhBeyanIrtifakRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SerhBeyanIrtifak GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<SerhBeyanIrtifak> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<SerhBeyanIrtifak> GetByTasinmazId(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazId(tasinmazId));
        }

        public SerhBeyanIrtifak GetById(int id, SqlTransactionContext transaction, PortalLogContext context) { RequireTransaction(transaction, context); return Map(repository.SelectById(id, transaction, context)); }
        public int Save(SerhBeyanIrtifak item, SqlTransactionContext transaction, PortalLogContext context) { RequireTransaction(transaction, context); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, transaction, context); if (item.Id > 0) RecordAudit("INSERT", item.Id, transaction, context); return item.Id; }
        public bool Update(SerhBeyanIrtifak item, SqlTransactionContext transaction, PortalLogContext context) { RequireTransaction(transaction, context); if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, transaction, context); if (ok) RecordAudit("UPDATE", item.Id, transaction, context); return ok; }
        public bool Delete(SerhBeyanIrtifak item, SqlTransactionContext transaction, PortalLogContext context) { RequireTransaction(transaction, context); if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, transaction, context); if (ok) RecordAudit("DELETE", item.Id, transaction, context); return ok; }
        private static void RequireTransaction(SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir."); if (context == null) throw new ArgumentNullException("context", "TBYS log bağlamı gereklidir."); }
        private static void RecordAudit(string operation, int id, SqlTransactionContext transaction, PortalLogContext context) { new DAO.Repositories.Ortak.PortalLogRepository().RecordAudit(context, operation, "SerhBeyanIrtifak", id.ToString(System.Globalization.CultureInfo.InvariantCulture), transaction); }

        private static List<SerhBeyanIrtifak> ToList(DataTable table)
        {
            return new SerhBeyanIrtifak().ToList<SerhBeyanIrtifak>(table);
        }

        private static SerhBeyanIrtifak Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
        public int Save(SerhBeyanIrtifak item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int result = Save(item, transaction, context);
                if (result > 0) transaction.Complete();
                return result;
            }
        }

        public bool Update(SerhBeyanIrtifak item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool result = Update(item, transaction, context);
                if (result) transaction.Complete();
                return result;
            }
        }

        public bool Delete(SerhBeyanIrtifak item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool result = Delete(item, transaction, context);
                if (result) transaction.Complete();
                return result;
            }
        }

    }
}
