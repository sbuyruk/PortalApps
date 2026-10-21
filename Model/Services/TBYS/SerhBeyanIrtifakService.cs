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

        public int Save(SerhBeyanIrtifak item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return item.Id;
        }

        public bool Update(SerhBeyanIrtifak item)
        {
            if (item == null) throw new ArgumentNullException("item");

            SerhBeyanIrtifak oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isSuccess;
        }

        public bool Delete(SerhBeyanIrtifak item)
        {
            if (item == null || item.Id == 0) return false;

            SerhBeyanIrtifak oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isDeleted;
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
    }
}
