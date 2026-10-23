using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Model.Services.IKYS
{
    public class IzinHareketService
    {
        private readonly IzinHareketRepository repository;
        public IzinHareketService() : this(new IzinHareketRepository())
        {
        }

        public IzinHareketService(IzinHareketRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.repository = repository;
        }

        public IzinHareket GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public IzinHareket GetById(int id, PortalLogContext context)
        {
            return Map(repository.SelectById(id, context));
        }

        public IzinHareket GetById(
            int id,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return Map(repository.SelectById(id, transaction, context));
        }

        public List<IzinHareket> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<IzinHareket> GetAll(PortalLogContext context)
        {
            return ToList(repository.SelectAll(context));
        }
        public int Save(IzinHareket item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    item,
                    ProjeConstants.IKYS,
                    ProjeConstants.IKYS_IZINHAREKET);
            }

            return item.Id;
        }
        public int Save(IzinHareket item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); using(SqlTransactionContext transaction=new SqlTransactionContext()) { item.Id=repository.Insert(item,transaction,context); int auditId=new PortalLogRepository().RecordAudit(context,"INSERT","IzinHareket",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(null,item,auditId,transaction); transaction.Complete(); } return item.Id; }
        public int Save(
            IzinHareket item,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);

            if (item.Id <= 0)
            {
                throw new InvalidOperationException("İzin hareketi oluşturulamadı.");
            }

            int auditId = new PortalLogRepository().RecordAudit(
                context,
                "INSERT",
                "IzinHareket",
                item.Id.ToString(CultureInfo.InvariantCulture),
                transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }
        public bool Update(IzinHareket item) { if (item == null) throw new ArgumentNullException("item"); IzinHareket old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINHAREKET); return ok; }
        public bool Update(IzinHareket item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { IzinHareket old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); if(!repository.Update(item,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"UPDATE","IzinHareket",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,item,auditId,transaction); transaction.Complete(); return true; } }
        public bool Update(IzinHareket item, SqlTransactionContext transaction, PortalLogContext context) { if(transaction==null) throw new ArgumentNullException("transaction"); if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; IzinHareket old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); if(!repository.Update(item,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"UPDATE","IzinHareket",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,item,auditId,transaction); return true; }
        public bool Delete(IzinHareket item) { if (item == null || item.Id == 0) return false; IzinHareket old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINHAREKET); return ok; }
        public bool Delete(IzinHareket item, PortalLogContext context) { if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; using(SqlTransactionContext transaction=new SqlTransactionContext()) { IzinHareket old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null||!repository.Delete(item.Id,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"DELETE","IzinHareket",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,null,auditId,transaction); transaction.Complete(); return true; } }
        public bool Delete(IzinHareket item, SqlTransactionContext transaction, PortalLogContext context) { if(transaction==null) throw new ArgumentNullException("transaction"); if(context==null) throw new ArgumentNullException("context"); if(item==null||item.Id<=0) return false; IzinHareket old=Map(repository.SelectById(item.Id,transaction,context)); if(old==null||!repository.Delete(item.Id,transaction,context)) return false; int auditId=new PortalLogRepository().RecordAudit(context,"DELETE","IzinHareket",item.Id.ToString(CultureInfo.InvariantCulture),transaction); RecordChanges(old,null,auditId,transaction); return true; }
        public List<IzinHareket> GetByTarih(DateTime tarih) { return ToList(repository.SelectByTarih(tarih)); }
        public List<IzinHareket> GetByTarih(DateTime tarih, PortalLogContext context) { return ToList(repository.SelectByTarih(tarih,context)); }
        public IzinHareket GetByIzinTalepId(int id)
        {
            return Map(repository.SelectByIzinTalepId(id));
        }

        public IzinHareket GetByIzinTalepId(int id, PortalLogContext context)
        {
            return Map(repository.SelectByIzinTalepId(id, context));
        }

        public IzinHareket GetByIzinTalepId(
            int id,
            SqlTransactionContext transaction,
            PortalLogContext context)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return Map(repository.SelectByIzinTalepId(id, transaction, context));
        }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis)
        {
            return repository.SelectByTarihReturnDataTable(baslangic, bitis);
        }

        public DataTable GetByTarihReturnDataTable(
            DateTime baslangic,
            DateTime bitis,
            PortalLogContext context)
        {
            return Trace(
                context,
                "IzinHareket.SelectByTarih",
                () => repository.SelectByTarihReturnDataTable(baslangic, bitis));
        }

        public IzinHareket GetByPersonelTarih(
            int personelId,
            DateTime baslangic,
            DateTime bitis)
        {
            return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis));
        }

        public IzinHareket GetByPersonelTarih(
            int personelId,
            DateTime baslangic,
            DateTime bitis,
            PortalLogContext context)
        {
            return Map(repository.SelectByPersonelTarih(
                personelId,
                baslangic,
                bitis,
                context));
        }

        public DataTable GetByIzinTipiTarihReturnDataTable(
            int izinTipi,
            DateTime ilkTarih,
            DateTime bitis)
        {
            return repository.SelectByIzinTipiTarih(izinTipi, ilkTarih, bitis);
        }

        public DataTable GetByIzinTipiTarihReturnDataTable(
            int izinTipi,
            DateTime ilkTarih,
            DateTime bitis,
            PortalLogContext context)
        {
            return Trace(
                context,
                "IzinHareket.SelectByIzinTipiTarih",
                () => repository.SelectByIzinTipiTarih(izinTipi, ilkTarih, bitis));
        }

        public DataTable GetByIzinDonemiReturnDataTable(
            int personelId,
            int izinTipi,
            DateTime baslangic,
            DateTime bitis)
        {
            return repository.SelectByIzinDonemi(personelId, izinTipi, baslangic, bitis);
        }

        public DataTable GetByIzinDonemiReturnDataTable(
            int izinDonemId,
            int personelId,
            int izinTipi)
        {
            return repository.SelectByPersonelIdDonemId(personelId, izinDonemId, izinTipi);
        }

        public DataTable GetByIzinDonemiReturnDataTable(
            int izinDonemId,
            int personelId,
            int izinTipi,
            PortalLogContext context)
        {
            return repository.SelectByPersonelIdDonemId(personelId, izinDonemId, izinTipi, context);
        }

        public string GetByPersonelIdDonemIdReturnJson(
            int personelId,
            int donemId,
            int izinTanimId)
        {
            return ConvertDataTableToString(
                repository.SelectByPersonelIdDonemId(personelId, donemId, izinTanimId));
        }

        public List<IzinHareket> GetDigerByPersonelId(int personelId)
        {
            return ToList(repository.SelectDigerByPersonelId(personelId));
        }

        public List<IzinHareket> GetDigerByPersonelId(
            int personelId,
            PortalLogContext context)
        {
            return Trace(
                context,
                "IzinHareket.SelectDigerByPersonelId",
                () => ToList(repository.SelectDigerByPersonelId(personelId)));
        }
        private static void RecordChanges(IzinHareket old,IzinHareket current,int auditId,SqlTransactionContext transaction) { if (auditId == 0) return; PortalLogRepository log=new PortalLogRepository(); Add(log,auditId,"PersonelId",old==null?(object)null:old.PersonelId,current==null?(object)null:current.PersonelId,transaction); Add(log,auditId,"IzinTipi",old==null?(object)null:old.IzinTipi,current==null?(object)null:current.IzinTipi,transaction); Add(log,auditId,"IzinTalepId",old==null?(object)null:old.IzinTalepId,current==null?(object)null:current.IzinTalepId,transaction); Add(log,auditId,"IzinDonemId",old==null?(object)null:old.IzinDonemId,current==null?(object)null:current.IzinDonemId,transaction); Add(log,auditId,"BaslangicTarihi",old==null?(object)null:old.BaslangicTarihi,current==null?(object)null:current.BaslangicTarihi,transaction); Add(log,auditId,"BitisTarihi",old==null?(object)null:old.BitisTarihi,current==null?(object)null:current.BitisTarihi,transaction); Add(log,auditId,"Mahsup",old==null?(object)null:old.Mahsup,current==null?(object)null:current.Mahsup,transaction); }
        private static void Add(PortalLogRepository log,int id,string field,object oldValue,object newValue,SqlTransactionContext transaction) { string a=oldValue==null?null:Convert.ToString(oldValue,CultureInfo.InvariantCulture); string b=newValue==null?null:Convert.ToString(newValue,CultureInfo.InvariantCulture); if(!string.Equals(a,b,StringComparison.Ordinal)) log.RecordAuditChange(id,field,a,b,transaction); }
        public bool TryRecordWebPartOpened(PortalLogContext context)
        {
            return new DAO.Repositories.Ortak.PortalLogRepository()
                .TryRecordWebPartOpened(context);
        }
        private static T Trace<T>(PortalLogContext context, string name, Func<T> action)
        {
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { T result = action(); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }
        public string ConvertDataTableToString(DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(); List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            if (table != null) foreach (DataRow dataRow in table.Rows) { Dictionary<string, object> row = new Dictionary<string, object>(); foreach (DataColumn column in table.Columns) row.Add(column.ColumnName, dataRow[column]); rows.Add(row); }
            return serializer.Serialize(rows);
        }
        private static IzinHareket Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IzinHareket> ToList(DataTable table) { return new IzinHareket().ToList<IzinHareket>(table); }
    }
}
