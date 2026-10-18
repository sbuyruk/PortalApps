using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class IzinTalepService
    {
        private readonly IzinTalepRepository repository;

        public IzinTalepService() : this(new IzinTalepRepository()) { }

        public IzinTalepService(IzinTalepRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public IzinTalep GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public IzinTalep GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public IzinTalep GetById(int id, SqlTransactionContext transaction, PortalLogContext context) { if (transaction == null) throw new ArgumentNullException("transaction"); if (context == null) throw new ArgumentNullException("context"); return Map(repository.SelectById(id, transaction, context)); }

        public List<IzinTalep> GetAll()
        {
            return new IzinTalep().ToList<IzinTalep>(repository.SelectAll());
        }

        public List<IzinTalep> GetAll(PortalLogContext context) { return new IzinTalep().ToList<IzinTalep>(repository.SelectAll(context)); }

        public int Save(IzinTalep item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return item.Id;
        }

        public int Save(IzinTalep item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("İzin talebi oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "IzinTalep", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }

        public bool Update(IzinTalep item)
        {
            if (item == null) throw new ArgumentNullException("item");
            IzinTalep old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return ok;
        }

        public bool Update(IzinTalep item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinTalep old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IzinTalep", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Update(IzinTalep item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            IzinTalep old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "IzinTalep", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }

        public bool Delete(IzinTalep item)
        {
            if (item == null || item.Id == 0) return false;
            IzinTalep old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return ok;
        }

        public bool Delete(IzinTalep item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Delete(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinTalep old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "IzinTalep", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }

        public string GetIzinTalepleriReturnJson(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            return new IzinTalep().ToJSON(GetIzinTalepleri(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri));
        }

        public DataTable GetIzinTalepleriReturnDataTable(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            return GetIzinTalepleri(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri);
        }

        public DataTable GetIzinTalepleriReturnDataTable(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri, PortalLogContext context)
        {
            return GetIzinTalepleri(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri, context);
        }

        public IzinTalep GetByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar) { return Map(repository.SelectByPersonelIdBasBitTar(personelId, basTar, bitTar)); }
        public IzinTalep GetByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar, PortalLogContext context) { return Map(repository.SelectByPersonelIdBasBitTar(personelId, basTar, bitTar, context)); }
        public IzinTalep GetIslemiDevamEden(int personelId, int izinTipi) { return Map(repository.SelectIslemiDevamEden(personelId, izinTipi)); }
        public IzinTalep GetIslemiDevamEden(int personelId, int izinTipi, PortalLogContext context) { return Map(repository.SelectIslemiDevamEden(personelId, izinTipi, context)); }
        public IzinTalep GetSonByPersonel(int izinTipi, int personelId) { return Map(repository.SelectSonByPersonel(izinTipi, personelId)); }
        public IzinTalep GetSonByPersonel(int izinTipi, int personelId, PortalLogContext context) { return Map(repository.SelectSonByPersonel(izinTipi, personelId, context)); }

        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }

        private DataTable GetIzinTalepleri(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            List<int> izinDonemIds = null;
            if (sadeceGecerliDonemTalepleri)
            {
                DateTime today = DateTime.Today;
                IzinDonemService izinDonemService = new IzinDonemService();
                IzinDonem ucretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                IzinDonem mazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, today);
                if (ucretli != null && mazeret != null)
                {
                    izinDonemIds = new List<int> { ucretli.Id, mazeret.Id };
                    IzinDonem gelecekUcretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, ucretli.BitisTarihi.AddDays(1));
                    IzinDonem gelecekMazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, mazeret.BitisTarihi.AddDays(1));
                    if (gelecekUcretli != null) izinDonemIds.Add(gelecekUcretli.Id);
                    if (gelecekMazeret != null) izinDonemIds.Add(gelecekMazeret.Id);
                }
            }
            return repository.SelectIzinTalepleri(personelId, izinTipi, mazeretHaric, ProjeConstants.IZINTIPI_MAZERET_INT, izinDonemIds);
        }

        private DataTable GetIzinTalepleri(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri, PortalLogContext context)
        {
            List<int> izinDonemIds = null;
            if (sadeceGecerliDonemTalepleri)
            {
                DateTime today = DateTime.Today;
                IzinDonemService izinDonemService = new IzinDonemService();
                IzinDonem ucretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, today, context);
                IzinDonem mazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, today, context);
                if (ucretli != null && mazeret != null)
                {
                    izinDonemIds = new List<int> { ucretli.Id, mazeret.Id };
                    IzinDonem gelecekUcretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, ucretli.BitisTarihi.AddDays(1), context);
                    IzinDonem gelecekMazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, mazeret.BitisTarihi.AddDays(1), context);
                    if (gelecekUcretli != null) izinDonemIds.Add(gelecekUcretli.Id);
                    if (gelecekMazeret != null) izinDonemIds.Add(gelecekMazeret.Id);
                }
            }
            return repository.SelectIzinTalepleri(personelId, izinTipi, mazeretHaric, ProjeConstants.IZINTIPI_MAZERET_INT, izinDonemIds, context);
        }

        private static void RecordChanges(IzinTalep old, IzinTalep current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? (int?)null : old.PersonelId, current == null ? (int?)null : current.PersonelId, transaction);
            AddChange(log, auditId, "IzinTipi", old == null ? (int?)null : old.IzinTipi, current == null ? (int?)null : current.IzinTipi, transaction);
            AddChange(log, auditId, "IzinDonemId", old == null ? (int?)null : old.IzinDonemId, current == null ? (int?)null : current.IzinDonemId, transaction);
            AddChange(log, auditId, "BaslangicTarihi", old == null ? (DateTime?)null : old.BaslangicTarihi, current == null ? (DateTime?)null : current.BaslangicTarihi, transaction);
            AddChange(log, auditId, "BitisTarihi", old == null ? (DateTime?)null : old.BitisTarihi, current == null ? (DateTime?)null : current.BitisTarihi, transaction);
            AddChange(log, auditId, "Sure", old == null ? null : old.Sure, current == null ? null : current.Sure, transaction);
            AddChange(log, auditId, "OnayDurumu", old == null ? (int?)null : old.OnayDurumu, current == null ? (int?)null : current.OnayDurumu, transaction);
            AddChange(log, auditId, "Aktif", old == null ? (bool?)null : old.Aktif, current == null ? (bool?)null : current.Aktif, transaction);
        }

        private static void AddChange(PortalLogRepository log, int auditId, string field, object oldValue, object newValue, SqlTransactionContext transaction)
        {
            string oldText = oldValue == null ? null : Convert.ToString(oldValue, CultureInfo.InvariantCulture);
            string newText = newValue == null ? null : Convert.ToString(newValue, CultureInfo.InvariantCulture);
            if (!string.Equals(oldText, newText, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldText, newText, transaction);
        }

        private static IzinTalep Map(DataTable table)
        {
            return new IzinTalep().ToList<IzinTalep>(table).FirstOrDefault();
        }
    }
}
