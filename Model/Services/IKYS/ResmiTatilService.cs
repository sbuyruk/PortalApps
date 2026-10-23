using DAO.Ortak;
using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using Model.IKYS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class ResmiTatilService
    {
        private readonly ResmiTatilRepository repository;
        public ResmiTatilService() : this(new ResmiTatilRepository()) { }
        public ResmiTatilService(ResmiTatilRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        public ResmiTatil GetById(int id) { return Map(repository.SelectById(id)); }
        public ResmiTatil GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<ResmiTatil> GetAll() { return ToList(repository.SelectAll()); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        public DataTable GetAllReturnDataTable(PortalLogContext context) { return repository.SelectAllReturnDataTable(context); }
        public int Save(ResmiTatil item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return item.Id; }
        public int Save(ResmiTatil item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Resmî tatil kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "ResmiTatil", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(ResmiTatil item) { if (item == null) return false; ResmiTatil old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return ok; }
        public bool Update(ResmiTatil item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                ResmiTatil old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "ResmiTatil", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Delete(ResmiTatil item) { if (item == null || item.Id == 0) return false; ResmiTatil old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return ok; }
        public bool Delete(ResmiTatil item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                ResmiTatil old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "ResmiTatil", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public List<ResmiTatil> GetByTarih(DateTime baslangic, DateTime bitis) { return ToList(repository.SelectByTarih(baslangic, bitis)); }
        public List<ResmiTatil> GetByTarih(DateTime baslangic, DateTime bitis, PortalLogContext context) { return ToList(repository.SelectByTarih(baslangic, bitis, context)); }
        public List<ResmiTatil> GetBySonIkiYil() { DateTime baslangic = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1); return ToList(repository.SelectBySonIkiYil(baslangic, baslangic.AddYears(2).AddDays(1))); }
        public List<ResmiTatil> GetByYil(int yil) { return ToList(repository.SelectByYil(yil)); }
        public bool IsResmiTatil(DateTime tarih) { return repository.SelectByDate(tarih) != null; }
        public bool IsResmiTatil(DateTime tarih, PortalLogContext context) { return repository.SelectByDate(tarih, context) != null; }
        public bool IsResmiTatil(DateTime tarih, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return repository.SelectByDate(tarih, transaction, context) != null;
        }
        public string GetAllReturnJson(DateTime baslangic, DateTime bitis)
        {
            List<ResmiTatil> list = GetByTarih(baslangic, bitis); List<CalendarEvent> events = new List<CalendarEvent>(); Faaliyet faaliyet = new Faaliyet();
            foreach (ResmiTatil tatil in list)
            {
                int yearCount = tatil.BaslamaTarihi.Year < 1900 ? bitis.Year - baslangic.Year : 1;
                for (int i = 0; i < yearCount; i++)
                {
                    if (tatil.BaslamaTarihi.Year < 1900) { string basSaat = tatil.BaslamaTarihi.Hour + ":" + tatil.BaslamaTarihi.Minute; string bitSaat = tatil.BitisTarihi.Hour + ":" + tatil.BitisTarihi.Minute; tatil.BaslamaTarihi = UtilityHelper.TariheSaatEkle(new DateTime(baslangic.Year + i, tatil.BaslamaTarihi.Month, tatil.BaslamaTarihi.Day), basSaat); tatil.BitisTarihi = UtilityHelper.TariheSaatEkle(new DateTime(baslangic.Year + i, tatil.BitisTarihi.Month, tatil.BitisTarihi.Day), bitSaat); }
                    CalendarEvent item = new CalendarEvent { state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString(), id = 999, purpose = ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT, title = tatil.Tatil, start = string.Format("{0:s}", tatil.BaslamaTarihi), end = string.Format("{0:s}", tatil.BitisTarihi), url = "", startEditable = false };
                    if (tatil.Yil < 1900) item.allDay = true; if (tatil.BitisTarihi.Day > tatil.BaslamaTarihi.Day) item.allDay = false; faaliyet.RenkBelirle(item); events.Add(item);
                }
            }
            return faaliyet.ToJSON(events);
        }
        private static ResmiTatil Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static void RecordChanges(ResmiTatil old, ResmiTatil current, int auditId, SqlTransactionContext transaction)
        {
            if (auditId == 0) return;
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "Gun", old == null ? null : old.Gun.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Gun.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Ay", old == null ? null : old.Ay.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Ay.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Yil", old == null ? null : old.Yil.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Yil.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Tatil", old == null ? null : old.Tatil, current == null ? null : current.Tatil, transaction);
            AddChange(log, auditId, "BaslamaTarihi", old == null ? null : HolidayDateValue(old.BaslamaTarihi), current == null ? null : HolidayDateValue(current.BaslamaTarihi), transaction);
            AddChange(log, auditId, "BitisTarihi", old == null ? null : HolidayDateValue(old.BitisTarihi), current == null ? null : HolidayDateValue(current.BitisTarihi), transaction);
            AddChange(log, auditId, "IlanTarihi", old == null ? null : DateValue(old.IlanTarihi), current == null ? null : DateValue(current.IlanTarihi), transaction);
            AddChange(log, auditId, "IptalTarihi", old == null ? null : DateValue(old.IptalTarihi), current == null ? null : DateValue(current.IptalTarihi), transaction);
        }
        private static string DateValue(DateTime value)
        {
            return value < new DateTime(1901, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture);
        }
        private static string HolidayDateValue(DateTime value)
        {
            return value < new DateTime(1753, 1, 1) ? null : value.ToString("o", CultureInfo.InvariantCulture);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }
        private static List<ResmiTatil> ToList(DataTable table) { return new ResmiTatil().ToList<ResmiTatil>(table); }
    }
}
