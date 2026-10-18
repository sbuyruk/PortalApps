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
using static Model.IKYS.Personel;

namespace Model.Services.IKYS
{
    public class EskiPersonelGorevTanimService
    {
        private readonly EskiPersonelGorevTanimRepository repository;
        public EskiPersonelGorevTanimService() : this(new EskiPersonelGorevTanimRepository()) { }
        public EskiPersonelGorevTanimService(EskiPersonelGorevTanimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public EskiPersonelGorevTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public EskiPersonelGorevTanim GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<EskiPersonelGorevTanim> GetAll() { return ToList(repository.SelectAll()); }
        public List<EskiPersonelGorevTanim> GetAll(PortalLogContext context) { return ToList(repository.SelectAll(context)); }
        public int Save(EskiPersonelGorevTanim item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
            return item.Id;
        }
        public int Save(EskiPersonelGorevTanim item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(!PortalLogSettings.IsEnabled(context)) return Save(item); if(item==null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); using(SqlTransactionContext transaction=new SqlTransactionContext()) { item.Id=repository.Insert(item,transaction,context); int auditId=new PortalLogRepository().RecordAudit(context,"INSERT","EskiPersonelGorevTanim",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(null,item,auditId,transaction); transaction.Complete(); } return item.Id; }
        public bool Update(EskiPersonelGorevTanim item)
        {
            if (item == null) throw new ArgumentNullException("item");
            EskiPersonelGorevTanim old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
            return ok;
        }
        public bool Update(EskiPersonelGorevTanim item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(!PortalLogSettings.IsEnabled(context)) return Update(item); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { EskiPersonelGorevTanim old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); if(!repository.Update(item,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"UPDATE","EskiPersonelGorevTanim",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,item,auditId,transaction); transaction.Complete(); return true; } }
        public bool Delete(EskiPersonelGorevTanim item)
        {
            if (item == null || item.Id == 0) return false;
            EskiPersonelGorevTanim old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
            return ok;
        }
        public bool Delete(EskiPersonelGorevTanim item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(!PortalLogSettings.IsEnabled(context)) return Delete(item); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { EskiPersonelGorevTanim old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null||!repository.Delete(item.Id,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"DELETE","EskiPersonelGorevTanim",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,null,auditId,transaction); transaction.Complete(); return true; } }
        public EskiPersonelGorevTanim GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public EskiPersonelGorevTanim GetByPersonelId(int personelId, PortalLogContext context) { return Map(repository.SelectByPersonelId(personelId,context)); }
        public List<EskiPersonelGorevTanim> GetByBirimId(int birimId) { return ToList(repository.SelectByBirimId(birimId)); }
        public List<EskiPersonelGorevTanim> GetByBirimId(int birimId, PortalLogContext context) { return ToList(repository.SelectByBirimId(birimId,context)); }
        public EskiPersonelGorevTanim GetByGorevId(int gorevId) { return GetById(gorevId); }
        public DataTable GetAllReturnDataTable(PersonelTipi personelTipi) { return repository.SelectAllReturnDataTable(personelTipi == PersonelTipi.Tumu ? (int?)null : (int)personelTipi); }
        public DataTable GetAllReturnDataTable(PersonelTipi personelTipi, PortalLogContext context) { return repository.SelectAllReturnDataTable(personelTipi == PersonelTipi.Tumu ? (int?)null : (int)personelTipi,context); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        private static void RecordChanges(EskiPersonelGorevTanim old,EskiPersonelGorevTanim current,int auditId,SqlTransactionContext transaction) { PortalLogRepository log=new PortalLogRepository(); Add(log,auditId,"BirimId",old==null?(object)null:old.BirimId,current==null?(object)null:current.BirimId,transaction); Add(log,auditId,"PersonelId",old==null?(object)null:old.PersonelId,current==null?(object)null:current.PersonelId,transaction); Add(log,auditId,"Adi",old==null?null:old.Adi,current==null?null:current.Adi,transaction); Add(log,auditId,"KisaAdi",old==null?null:old.KisaAdi,current==null?null:current.KisaAdi,transaction); Add(log,auditId,"Vekil",old==null?(object)null:old.Vekil,current==null?(object)null:current.Vekil,transaction); Add(log,auditId,"Aktif",old==null?(object)null:old.Aktif,current==null?(object)null:current.Aktif,transaction); }
        private static void Add(PortalLogRepository log,int id,string field,object oldValue,object newValue,SqlTransactionContext transaction) { string a=oldValue==null?null:Convert.ToString(oldValue,CultureInfo.InvariantCulture); string b=newValue==null?null:Convert.ToString(newValue,CultureInfo.InvariantCulture); if(!string.Equals(a,b,StringComparison.Ordinal)) log.RecordAuditChange(id,field,a,b,transaction); }
        private static EskiPersonelGorevTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<EskiPersonelGorevTanim> ToList(DataTable table) { return new EskiPersonelGorevTanim().ToList<EskiPersonelGorevTanim>(table); }
    }
}
