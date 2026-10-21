using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class OdemeSebebiTanimService
    {
        private readonly OdemeSebebiTanimRepository repository;
        private readonly PortalLogContext context;

        public OdemeSebebiTanimService() : this(new OdemeSebebiTanimRepository()) { }

        public OdemeSebebiTanimService(OdemeSebebiTanimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public OdemeSebebiTanimService(PortalLogContext context) : this(new OdemeSebebiTanimRepository(context)) { this.context = context; }

        public OdemeSebebiTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemeSebebiTanim> GetAll() { return ToList(repository.SelectAll()); }

        public int Save(OdemeSebebiTanim item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
            }
            return item.Id;
        }
        public int Save(OdemeSebebiTanim item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0) Audit("INSERT", item.Id, tx); return item.Id; }

        public bool Update(OdemeSebebiTanim item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
            if (item == null) throw new ArgumentNullException("item");
            OdemeSebebiTanim oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
            }
            return isSuccess;
        }
        public bool Update(OdemeSebebiTanim item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok) Audit("UPDATE", item.Id, tx); return ok; }

        public bool Delete(OdemeSebebiTanim item)
        {
            if (context != null) using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
            if (item == null || item.Id == 0) return false;
            OdemeSebebiTanim oldItem = GetById(item.Id);
            if (oldItem == null) return false;
            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMESEBEBITANIM);
            }
            return isDeleted;
        }
        public bool Delete(OdemeSebebiTanim item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok) Audit("DELETE", item.Id, tx); return ok; }
        private void Audit(string operation, int id, SqlTransactionContext tx) { if (PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, operation, "OdemeSebebiTanim", id.ToString(), tx); }

        private static List<OdemeSebebiTanim> ToList(DataTable table) { return new OdemeSebebiTanim().ToList<OdemeSebebiTanim>(table); }
        private static OdemeSebebiTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
