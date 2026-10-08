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
        public int Save(KiraEkstreAktarma item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return item.Id; }
        public bool Update(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
        public bool Delete(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
        private static List<KiraEkstreAktarma> ToList(DataTable t) { return new KiraEkstreAktarma().ToList<KiraEkstreAktarma>(t); }
        private static KiraEkstreAktarma Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
