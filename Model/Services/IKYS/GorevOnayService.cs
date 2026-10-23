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
using Utility.ProjeGlobal;
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
        public GorevOnay GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<GorevOnay> GetAll() { return ToList(repository.SelectAll()); }
        public List<GorevOnay> GetAll(PortalLogContext context) { return ToList(repository.SelectAll(context)); }

        public int Save(GorevOnay item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return item.Id;
        }
        public int Save(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); using(SqlTransactionContext transaction=new SqlTransactionContext()) { item.Id=repository.Insert(item,transaction,context); int auditId=new PortalLogRepository().RecordAudit(context,"INSERT","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(null,item,auditId,transaction); transaction.Complete(); } return item.Id; }

        public bool Update(GorevOnay item)
        {
            if (item == null) throw new ArgumentNullException("item");
            GorevOnay old = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }
            if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return isSuccess;
        }
        public bool Update(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { GorevOnay old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); if(!repository.Update(item,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"UPDATE","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,item,auditId,transaction); transaction.Complete(); return true; } }

        public bool Delete(GorevOnay item)
        {
            if (item == null || item.Id == 0) return false;
            GorevOnay old = GetById(item.Id);
            if (old == null) return false;
            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return isDeleted;
        }
        public bool Delete(GorevOnay item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { GorevOnay old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null||!repository.Delete(item.Id,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"DELETE","GorevOnay",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,null,auditId,transaction); transaction.Complete(); return true; } }

        public bool UpdateAllSecildiToFalse() { return repository.UpdateAllSecildiToFalse(); }
        public bool UpdateAllSecildiToTrue(string idString) { return repository.UpdateAllSecildiToTrue(ParseIdList(idString)); }
        public bool UpdateAllOdendiToTrue(string idString) { return repository.UpdateAllOdendiToTrue(ParseIdList(idString)); }
        public DataTable GetAllReturnDataTable(int personelId, DateTime since) { return repository.SelectAllReturnDataTable(personelId, since); }
        public DataTable GetAllReturnDataTable(int personelId, DateTime since, PortalLogContext context) { return repository.SelectAllReturnDataTable(personelId,since,context); }
        public DataTable GetAllByPersonelReturnDataTable(int personelId)
        {
            if (personelId <= 0) throw new ArgumentOutOfRangeException("personelId");
            return repository.SelectAllByPersonelReturnDataTable(personelId);
        }
        public DataTable GetAllByPersonelReturnDataTable(int personelId, PortalLogContext context) { return repository.SelectAllByPersonelReturnDataTable(personelId,context); }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis) { return repository.SelectByTarihReturnDataTable(baslangic, bitis); }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis, PortalLogContext context) { return repository.SelectByTarihReturnDataTable(baslangic,bitis,context); }
        public GorevOnay GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis) { return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis)); }
        public GorevOnay GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis, PortalLogContext context) { return Map(repository.SelectByPersonelTarih(personelId,baslangic,bitis,context)); }
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
        private static void RecordChanges(GorevOnay old,GorevOnay current,int auditId,SqlTransactionContext transaction) { if (auditId == 0) return; PortalLogRepository log=new PortalLogRepository(); Add(log,auditId,"PersonelId",old==null?(object)null:old.PersonelId,current==null?(object)null:current.PersonelId,transaction); Add(log,auditId,"BaslangicTarihi",old==null?(object)null:old.BaslangicTarihi,current==null?(object)null:current.BaslangicTarihi,transaction); Add(log,auditId,"BitisTarihi",old==null?(object)null:old.BitisTarihi,current==null?(object)null:current.BitisTarihi,transaction); Add(log,auditId,"AmirOnayi",old==null?(object)null:old.AmirOnayi,current==null?(object)null:current.AmirOnayi,transaction); Add(log,auditId,"Secildi",old==null?(object)null:old.Secildi,current==null?(object)null:current.Secildi,transaction); Add(log,auditId,"Odendi",old==null?(object)null:old.Odendi,current==null?(object)null:current.Odendi,transaction); }
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
        public List<GorevOnay> SelectByBirimIdAndDurum(int birimId, GorevOnay.AmirOnayDurumu onayBekliyor, PortalLogContext context) { return ToList(repository.SelectByBirimIdAndDurum(birimId, (int)onayBekliyor, context)); }
    }
}
