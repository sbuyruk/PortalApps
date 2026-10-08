using DAO.Repositories.TBYS;
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
    public class KiraEkstreAktarmaService
    {
        private readonly KiraEkstreAktarmaRepository repository;
        public KiraEkstreAktarmaService() : this(new KiraEkstreAktarmaRepository()) { }
        public KiraEkstreAktarmaService(KiraEkstreAktarmaRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public KiraEkstreAktarma GetById(int id) { return Map(repository.SelectById(id)); }
        public List<KiraEkstreAktarma> GetAll() { return ToList(repository.SelectAll()); }
        public List<KiraEkstreAktarma> GetByKiraciAdi(string adi) { return ToList(repository.SelectByKiraciAdi(adi)); }
        public List<KiraEkstreAktarma> GetByIslemNo(string islemNo) { return ToList(repository.SelectByIslemNo(islemNo)); }
        public List<KiraEkstreAktarma> GetByColumns(string adi, string soyadi, decimal tutar, DateTime odemeTarihi) { return ToList(repository.SelectByColumns(adi, soyadi, tutar, odemeTarihi)); }
        public List<KiraEkstreAktarma> GetByIdList(string idListStr) { List<int> ids = ParseIds(idListStr); return ids.Count == 0 ? new List<KiraEkstreAktarma>() : ToList(repository.SelectByIdList(ids)); }
        public List<KiraEkstreAktarma> GetByEkstreIdList(string idListStr, ref int rowCount) { List<int> ids = ParseIds(idListStr); if (ids.Count == 0) return new List<KiraEkstreAktarma>(); DataTable table = repository.SelectByEkstreIdList(ids); rowCount = table == null ? 0 : table.Rows.Count; return ToList(table); }
        public DataTable GetUploadedRecords(ref int rowCount, bool aktarilanlarHaric, bool kiraTeminatDiger)
        {
            DateTime threeMonthsAgo = DateTime.Today.AddMonths(-3);
            DateTime basTar = new DateTime(threeMonthsAgo.Year, threeMonthsAgo.Month, 1);
            DataTable table = repository.SelectUploadedRecords(basTar, aktarilanlarHaric, kiraTeminatDiger,
                ProjeConstants.ODEMESEBEBI_DIGER_INT, ProjeConstants.ODEMESEBEBI_KIRA_INT,
                ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT, ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT,
                ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT);
            rowCount = table == null ? 0 : table.Rows.Count;
            return table;
        }
        public DataTable GetByIdDetailed(int id) { return repository.SelectByIdDetailed(id); }
        public DataTable GetByDateAndPaymentReason(DateTime tarih, int odemeSebebiId) { return repository.SelectByDateAndPaymentReason(StartOfDay(tarih), UtilityHelper.TariheSaatEkle(StartOfDay(tarih), "23:59"), odemeSebebiId == ProjeConstants.HEPSI_INT ? (int?)null : odemeSebebiId); }
        public DataTable GetSumByDateAndPaymentReason(DateTime tarih, int odemeSebebiId) { return repository.SelectSumByDateAndPaymentReason(StartOfDay(tarih), UtilityHelper.TariheSaatEkle(StartOfDay(tarih), "23:59"), odemeSebebiId == ProjeConstants.HEPSI_INT ? (int?)null : odemeSebebiId); }
        public int Save(KiraEkstreAktarma item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return item.Id; }
        public bool Update(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
        public bool Delete(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
        private static List<KiraEkstreAktarma> ToList(DataTable t) { return new KiraEkstreAktarma().ToList<KiraEkstreAktarma>(t); }
        private static KiraEkstreAktarma Map(DataTable t) { return ToList(t).FirstOrDefault(); }
        private static DateTime StartOfDay(DateTime value) { return new DateTime(value.Year, value.Month, value.Day); }
        private static List<int> ParseIds(string value) { return string.IsNullOrEmpty(value) ? new List<int>() : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())).ToList(); }
    }
}
