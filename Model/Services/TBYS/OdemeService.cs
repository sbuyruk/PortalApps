using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class OdemeService
    {
        private readonly OdemeRepository repository;
        private readonly PortalLogContext context;
        public OdemeService() : this(new OdemeRepository()) { }
        public OdemeService(OdemeRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public OdemeService(PortalLogContext context) : this(new OdemeRepository(context)) { this.context = context; }
        public Odeme GetById(int id) { return Map(repository.SelectById(id)); }
        public Odeme GetById(int id, SqlTransactionContext transaction) { return Map(repository.SelectById(id, transaction, context)); }
        public List<Odeme> GetAll() { return ToList(repository.SelectAll()); }
        public List<Odeme> GetByKiraciId(int id) { return ToList(repository.SelectByKiraciId(id)); }
        public List<Odeme> GetBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId) { return ToList(repository.SelectBySozlesmeIdOdemePlaniId(sozlesmeId, planId)); }
        public DataTable GetByKiraciAyYil(int kiraciId, int ay, int yil) { return repository.SelectByKiraciAyYil(kiraciId, ay, yil); }
        public DataTable GetByKiraciAyYilReturnDataTable(int bolgeId, int kiraciId, DateTime basTarih, DateTime bitTarih) { return repository.SelectByKiraciAyYilReturnDataTable(bolgeId, kiraciId, basTarih, UtilityHelper.TariheSaatEkle(bitTarih, "23:59:59")); }
        public DataTable GetByAyYilReturnDataTable(int ay, int yil) { return repository.SelectByAyYilReturnDataTable(ay, yil); }
        public List<Odeme> GetByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
        {
            DateTime basTarih = UtilityHelper.TariheSaatEkle(new DateTime(ilkTarih.Year, ilkTarih.Month, ilkTarih.Day), "00:00");
            DateTime bitTarih = UtilityHelper.TariheSaatEkle(new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day), "23:59");
            return ToList(repository.SelectByKiraciVadeBasTarVadeBitTar(sozlesmeId, kiraciId, basTarih, bitTarih));
        }
        public List<Odeme> GetByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime ilkTarih, DateTime ikinciTarih, SqlTransactionContext transaction)
        {
            DateTime basTarih = UtilityHelper.TariheSaatEkle(new DateTime(ilkTarih.Year, ilkTarih.Month, ilkTarih.Day), "00:00");
            DateTime bitTarih = UtilityHelper.TariheSaatEkle(new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day), "23:59");
            return ToList(repository.SelectByKiraciVadeBasTarVadeBitTar(sozlesmeId, kiraciId, basTarih, bitTarih, transaction, context));
        }
        public decimal GetSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            DataTable dataTable = repository.SelectSumBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Toplam"].ToString().ConvertToDecimal();
            }
            return 0;
        }
        public decimal GetSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId, SqlTransactionContext transaction)
        {
            DataTable dataTable = repository.SelectSumBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId, transaction, context);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Toplam"].ToString().ConvertToDecimal();
            }
            return 0;
        }
        public int Save(Odeme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
        }
        public int Save(Odeme item, SqlTransactionContext tx) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, tx, context); if (item.Id > 0) Audit("INSERT", item.Id, tx); return item.Id; }
        public bool Update(Odeme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
        }
        public bool Update(Odeme item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item, tx, context); if (ok) Audit("UPDATE", item.Id, tx); return ok; }
        public bool Delete(Odeme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
        }
        public bool Delete(Odeme item, SqlTransactionContext tx) { if (item == null || item.Id == 0) return false; bool ok = repository.Delete(item.Id, tx, context); if (ok) Audit("DELETE", item.Id, tx); return ok; }
        private void Audit(string operation, int id, SqlTransactionContext tx) { if (PortalLogSettings.IsEnabled(context)) new PortalLogRepository().RecordAudit(context, operation, "Odeme", id.ToString(), tx); }
        public bool DeleteBySozlesmeId(int id)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = DeleteBySozlesmeId(id, tx); if (ok) tx.Complete(); return ok; }
        }
        public bool DeleteBySozlesmeId(int id, SqlTransactionContext tx)
        {
            DataTable rows = repository.SelectBySozlesmeId(id, tx, context);
            if (rows == null || rows.Rows.Count == 0) return true;
            if (!repository.DeleteBySozlesmeId(id, tx, context)) return false;
            foreach (DataRow row in rows.Rows)
                Audit("DELETE", Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture), tx);
            return true;
        }

        public Odeme OdemeyiKaydetOdemePlaniniGuncelle(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Odeme sonuc = OdemeyiKaydetOdemePlaniniGuncelle(kiraSozlesme, odemePlani, odemeTarihi, odenenTutar, aciklama, kullanici, transaction);
                if (sonuc != null && sonuc.Id > 0) transaction.Complete();
                return sonuc;
            }
        }

        public Odeme OdemeyiKaydetOdemePlaniniGuncelle(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici, SqlTransactionContext transaction)
        {
            Odeme odeme = new Odeme
            {
                SozlesmeId = kiraSozlesme.Id,
                KiraciId = kiraSozlesme.KiraciId,
                OdemePlaniId = odemePlani.Id,
                OdemeTarihi = odemeTarihi,
                OdenenTutar = odenenTutar,
                Aciklama = aciklama,
                Olusturan = kullanici
            };
            odeme.Id = Save(odeme, transaction);
            if (odeme.Id <= 0) throw new InvalidOperationException("Ödeme kaydedilemedi.");
            if (odeme.Id > 0)
            {
                decimal toplamOdenen = GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id, transaction);
                odemePlani.OdenenTutar = toplamOdenen;
                odemePlani.Aciklama += aciklama + Environment.NewLine;
                odemePlani.Degistiren = kullanici;
                if (!new OdemePlaniService(context).Update(odemePlani, transaction))
                {
                    throw new InvalidOperationException("Ödeme planı güncellenemedi.");
                }
            }

            return odeme;
        }

        public bool OdemeyiSilOdemePlaniniGuncelle(int odemeId, string aciklama, int odemePlaniId, string kullanici)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool result = OdemeyiSilOdemePlaniniGuncelle(odemeId, aciklama, odemePlaniId, kullanici, transaction);
                if (result) transaction.Complete();
                return result;
            }
        }

        private bool OdemeyiSilOdemePlaniniGuncelle(int odemeId, string aciklama, int odemePlaniId, string kullanici, SqlTransactionContext transaction)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Odeme odeme = GetById(odemeId, transaction);
            if (odeme == null || !Delete(odeme, transaction)) return false;

            OdemeAyrintiService odemeAyrintiService = new OdemeAyrintiService(context);
            if (odemeAyrintiService.GetByOdemeIdOdemePlaniId(odemeId, odemePlaniId, transaction).Count > 0
                && !odemeAyrintiService.DeleteByOdemeIdOdemePlaniId(odemeId, odemePlaniId, transaction)) return false;

            OdemePlaniService odemePlaniService = new OdemePlaniService(context);
            OdemePlani odemePlani = odemePlaniService.GetById(odemePlaniId, transaction);
            if (odemePlani == null) return false;
            odemePlani.OdenenTutar = GetSumBySozlesmeIdOdemePlaniId(odemePlani.SozlesmeId, odemePlani.Id, transaction);
            odemePlani.Aciklama += aciklama + Environment.NewLine +
                " *" + odeme.OdemeTarihi.ConvertToDatetimeEmptyIfNull() + " tarihli " + odeme.OdenenTutar.ToString("N", culturInfo) + " ödeme silindi." + Environment.NewLine;
            odemePlani.Degistiren = kullanici;
            return odemePlaniService.Update(odemePlani, transaction);

        }

        public bool OdemeyiVeOdemePlaniniGuncelle(int sozlesmeId, int oncekiOdemePlaniId, int yeniOdemePlaniId, int odemeId, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool result = OdemeyiVeOdemePlaniniGuncelle(sozlesmeId, oncekiOdemePlaniId, yeniOdemePlaniId, odemeId, odemeTarihi, odenenTutar, aciklama, kullanici, transaction);
                if (result) transaction.Complete();
                return result;
            }
        }

        private bool OdemeyiVeOdemePlaniniGuncelle(int sozlesmeId, int oncekiOdemePlaniId, int yeniOdemePlaniId, int odemeId, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici, SqlTransactionContext transaction)
        {
            OdemePlaniService odemePlaniService = new OdemePlaniService(context);
            OdemePlani oncekiOdemePlani = odemePlaniService.GetById(oncekiOdemePlaniId, transaction);
            OdemePlani yeniOdemePlani = odemePlaniService.GetById(yeniOdemePlaniId, transaction);
            if (yeniOdemePlani == null)
            {
                if (oncekiOdemePlani == null) return false;
                List<OdemePlani> sozlesmeOdemePlanlari = odemePlaniService.GetBySozlesmeId(sozlesmeId, transaction);
                if (odemeTarihi <= oncekiOdemePlani.OdemeBasTar) yeniOdemePlani = sozlesmeOdemePlanlari.FirstOrDefault();
                else if (odemeTarihi >= oncekiOdemePlani.OdemeBitTar) yeniOdemePlani = sozlesmeOdemePlanlari.LastOrDefault();
                else return false;
            }
            if (yeniOdemePlani == null) return false;

            Odeme odeme = GetById(odemeId, transaction);
            if (odeme == null) return false;
            odeme.OdemeTarihi = odemeTarihi;
            odeme.OdenenTutar = odenenTutar;
            odeme.Aciklama = aciklama;
            odeme.Degistiren = kullanici;
            odeme.OdemePlaniId = yeniOdemePlani.Id;
            odeme.SozlesmeId = yeniOdemePlani.SozlesmeId;
            if (!Update(odeme, transaction)) return false;

            if (oncekiOdemePlani != null && oncekiOdemePlani.Id != yeniOdemePlani.Id)
            {
                oncekiOdemePlani.OdenenTutar = GetSumBySozlesmeIdOdemePlaniId(oncekiOdemePlani.SozlesmeId, oncekiOdemePlani.Id, transaction);
                oncekiOdemePlani.Degistiren = kullanici;
                if (!odemePlaniService.Update(oncekiOdemePlani, transaction)) return false;
            }
            yeniOdemePlani.OdenenTutar = GetSumBySozlesmeIdOdemePlaniId(yeniOdemePlani.SozlesmeId, yeniOdemePlani.Id, transaction);
            yeniOdemePlani.Aciklama = aciklama;
            yeniOdemePlani.Degistiren = kullanici;
            return odemePlaniService.Update(yeniOdemePlani, transaction);

        }

        private static List<Odeme> ToList(DataTable t) { return new Odeme().ToList<Odeme>(t); }
        private static Odeme Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
