using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;
using System.Globalization;

namespace Model.Services.IKYS
{
    public class GorevOnayService
    {
        private readonly GorevOnayRepository repository;

        public GorevOnayService() : this(new GorevOnayRepository()) { }

        public GorevOnayService(GorevOnayRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public GorevOnay GetById(int id) { return Map(repository.SelectById(id)); }
        public GorevOnay GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }
        public List<GorevOnay> GetAll() { return ToList(repository.SelectAll()); }
        public List<GorevOnay> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }


        public int Save(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); using(SqlTransactionContext transaction=new SqlTransactionContext()) { item.Id=repository.Insert(item,transaction,context); int auditId=new PortalLogRepository().RecordAudit(context,"INSERT","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(null,item,auditId,transaction); transaction.Complete(); } return item.Id; }


        public bool Update(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { GorevOnay old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); if(!repository.Update(item,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"UPDATE","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,item,auditId,transaction); transaction.Complete(); return true; } }


        public bool Delete(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { GorevOnay old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null||!repository.Delete(item.Id,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"DELETE","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,null,auditId,transaction); transaction.Complete(); return true; } }


        public DataTable GetAllReturnDataTable(int personelId, DateTime since) { return repository.SelectAllReturnDataTable(personelId, since); }
        public DataTable GetAllReturnDataTable(int personelId, DateTime since, PortalLogContext context)
        { return repository.SelectAllReturnDataTable(personelId, since, context); }
        public DataTable GetAllByPersonelReturnDataTable(int personelId)
        {
            if (personelId <= 0) throw new ArgumentOutOfRangeException("personelId");
            return repository.SelectAllByPersonelReturnDataTable(personelId);
        }
        public DataTable GetAllByPersonelReturnDataTable(int personelId, PortalLogContext context)
        {
            if (personelId <= 0) throw new ArgumentOutOfRangeException("personelId");
            return repository.SelectAllByPersonelReturnDataTable(personelId, context);
        }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis) { return repository.SelectByTarihReturnDataTable(baslangic, bitis); }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis, PortalLogContext context)
        { return repository.SelectByTarihReturnDataTable(baslangic, bitis, context); }
        public GorevOnay GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis) { return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis)); }
        public GorevOnay GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis, PortalLogContext context)
        { return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis, context)); }
        public List<GorevOnay> GetAllBySecildi(bool secildi) { return ToList(repository.SelectAllBySecildi(secildi)); }
        public DataTable GetBekleyenAmirOnayi()
        {
            return repository.SelectBekleyenAmirOnayi();
        }
        public DataTable GetBekleyenAmirOnayiByBirimIds(string birimIdListStr)
        {
            List<int> ids = ParseIdList(birimIdListStr);
            return ids.Count == 0 ? new DataTable() : repository.SelectBekleyenAmirOnayiByBirimIds(ids);
        }
        public bool HasDateConflict(int personelId, DateTime baslangic, DateTime bitis, int gorevOnayId)
        {
            return repository.SelectForDateConflict(personelId, baslangic, bitis, gorevOnayId) != null;
        }
        public bool HasDateConflict(int personelId, DateTime baslangic, DateTime bitis, int gorevOnayId, PortalLogContext context) { return repository.SelectForDateConflict(personelId,baslangic,bitis,gorevOnayId,context)!=null; }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        private static void RecordChanges(GorevOnay old,GorevOnay current,int auditId,SqlTransactionContext transaction) { if (auditId == 0) return; PortalLogRepository log=new PortalLogRepository(); Add(log,auditId,"PersonelId",old==null?(object)null:old.PersonelId,current==null?(object)null:current.PersonelId,transaction); Add(log,auditId,"BaslangicTarihi",old==null?(object)null:old.BaslangicTarihi,current==null?(object)null:current.BaslangicTarihi,transaction); Add(log,auditId,"BitisTarihi",old==null?(object)null:old.BitisTarihi,current==null?(object)null:current.BitisTarihi,transaction); Add(log,auditId,"AmirOnayi",old==null?(object)null:old.AmirOnayi,current==null?(object)null:current.AmirOnayi,transaction); Add(log,auditId,"Secildi",old==null?(object)null:old.Secildi,current==null?(object)null:current.Secildi,transaction); Add(log,auditId,"Odendi",old==null?(object)null:old.Odendi,current==null?(object)null:current.Odendi,transaction);
            Add(log, auditId, "GorevinSebebi", old == null ? null : Convert.ToString(old.GorevinSebebi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.GorevinSebebi, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "GorevinYeri", old == null ? null : Convert.ToString(old.GorevinYeri, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.GorevinYeri, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Sure", old == null ? null : Convert.ToString(old.Sure, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Sure, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Avans", old == null ? null : Convert.ToString(old.Avans, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Avans, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Yevmiye", old == null ? null : Convert.ToString(old.Yevmiye, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Yevmiye, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "GunlukYevmiye", old == null ? null : Convert.ToString(old.GunlukYevmiye, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.GunlukYevmiye, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "ParaBirimi", old == null ? null : Convert.ToString(old.ParaBirimi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.ParaBirimi, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "UlasimAraci", old == null ? null : Convert.ToString(old.UlasimAraci, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.UlasimAraci, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "AracTahsisi", old == null ? null : Convert.ToString(old.AracTahsisi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.AracTahsisi, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "AracPlakasi", old == null ? null : Convert.ToString(old.AracPlakasi, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.AracPlakasi, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "PerSubeImza", old == null ? null : Convert.ToString(old.PerSubeImza, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.PerSubeImza, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "PerSubeVekil", old == null ? null : Convert.ToString(old.PerSubeVekil, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.PerSubeVekil, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "OnayImza", old == null ? null : Convert.ToString(old.OnayImza, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OnayImza, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "OnayMakam", old == null ? null : Convert.ToString(old.OnayMakam, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OnayMakam, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "OnayMakamVekil", old == null ? null : Convert.ToString(old.OnayMakamVekil, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OnayMakamVekil, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "GMImza", old == null ? null : Convert.ToString(old.GMImza, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.GMImza, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "GMVekil", old == null ? null : Convert.ToString(old.GMVekil, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.GMVekil, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Aciklama", old == null ? null : Convert.ToString(old.Aciklama, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Aciklama, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Transfer", old == null ? null : Convert.ToString(old.Transfer, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Transfer, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "Konaklama", old == null ? null : Convert.ToString(old.Konaklama, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.Konaklama, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "OnayRedAciklama", old == null ? null : Convert.ToString(old.OnayRedAciklama, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OnayRedAciklama, CultureInfo.InvariantCulture), transaction);
            Add(log, auditId, "OncekiId", old == null ? null : Convert.ToString(old.OncekiId, CultureInfo.InvariantCulture), current == null ? null : Convert.ToString(current.OncekiId, CultureInfo.InvariantCulture), transaction);
        }
        private static void Add(PortalLogRepository log,int id,string field,object oldValue,object newValue,SqlTransactionContext transaction) { string a=oldValue==null?null:Convert.ToString(oldValue,CultureInfo.InvariantCulture); string b=newValue==null?null:Convert.ToString(newValue,CultureInfo.InvariantCulture); if(!string.Equals(a,b,StringComparison.Ordinal)) log.RecordAuditChange(id,field,a,b,transaction); }

        private static GorevOnay Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<GorevOnay> ToList(DataTable table) { return new GorevOnay().ToList<GorevOnay>(table); }

        private static List<int> ParseIdList(string idString)
        {
            List<int> ids = new List<int>();
            if (string.IsNullOrWhiteSpace(idString)) return ids;
            string value = idString.Trim();
            if (value.StartsWith("(") && value.EndsWith(")")) value = value.Substring(1, value.Length - 2);
            foreach (string part in value.Split(','))
            {
                int id;
                if (!int.TryParse(part.Trim(), out id))
                    throw new ArgumentException("Görev onay ID listesi geçersiz bir değer içeriyor.", "idString");
                ids.Add(id);
            }
            return ids;
        }

        public List<GorevOnay> SelectByBirimIdAndDurum(int birimId, GorevOnay.AmirOnayDurumu onayBekliyor)
        {
            if (birimId <= 0)
            {
                return new List<GorevOnay>();
            }

            DataTable dataTable = repository.SelectByBirimIdAndDurum(birimId, (int)onayBekliyor);
            return dataTable == null ? new List<GorevOnay>() : ToList(dataTable);
        }
        public List<GorevOnay> SelectByBirimIdAndDurum(int birimId, GorevOnay.AmirOnayDurumu onayBekliyor, PortalLogContext context)
        {
            if (birimId <= 0)
            {
                return new List<GorevOnay>();
            }

            DataTable dataTable = repository.SelectByBirimIdAndDurum(birimId, (int)onayBekliyor, context);
            return dataTable == null ? new List<GorevOnay>() : ToList(dataTable);
        }

        public List<GorevOnay> GetAllBySecildi(bool secildi, PortalLogContext context)
        { return ToList(repository.SelectAllBySecildi(secildi, context)); }

        public DataTable GetBekleyenAmirOnayi(PortalLogContext context)
        {
            return repository.SelectBekleyenAmirOnayi(context);
        }

        public DataTable GetBekleyenAmirOnayiByBirimIds(string birimIdListStr, PortalLogContext context)
        {
            List<int> ids = ParseIdList(birimIdListStr);
            return ids.Count == 0 ? new DataTable() : repository.SelectBekleyenAmirOnayiByBirimIds(ids, context);
        }
        public bool Update(GorevOnay item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            GorevOnay old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "GorevOnay", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(old, item, auditId, transaction);
            return true;
        }

        public bool SetSelectedIds(string ids, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            HashSet<int> selected = new HashSet<int>(ParseIdList(ids));
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                List<GorevOnay> rows = ToList(repository.SelectForFlagUpdate(transaction, context));
                foreach (GorevOnay item in rows)
                {
                    bool value = selected.Contains(item.Id);
                    if (item.Secildi == value) continue;
                    item.Secildi = value;
                    if (!Update(item, transaction, context)) throw new InvalidOperationException("Görev seçimi güncellenemedi.");
                }
                transaction.Complete();
                return true;
            }
        }

        public bool SetPaidIds(string ids, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                foreach (int id in ParseIdList(ids).Distinct())
                {
                    GorevOnay item = Map(repository.SelectById(id, transaction, context));
                    if (item == null || item.Odendi) continue;
                    item.Odendi = true;
                    if (!Update(item, transaction, context)) throw new InvalidOperationException("Görev ödeme bilgisi güncellenemedi.");
                }
                transaction.Complete();
                return true;
            }
        }
    }
}
