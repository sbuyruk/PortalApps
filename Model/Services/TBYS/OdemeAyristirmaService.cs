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
    public class OdemeAyristirmaService
    {
        private readonly OdemeAyristirmaRepository repository;

        public OdemeAyristirmaService() : this(new OdemeAyristirmaRepository()) { }
        public OdemeAyristirmaService(OdemeAyristirmaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public OdemeAyristirma GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemeAyristirma> GetAll() { return ToList(repository.SelectAll()); }
        public DataTable GetByKiraEkstreAktarmaId(int kiraEkstreAktarmaId) { return repository.SelectByKiraEkstreAktarmaId(kiraEkstreAktarmaId); }
        public int Save(OdemeAyristirma item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
            return item.Id;
        }
        public bool Update(OdemeAyristirma item)
        {
            if (item == null || item.Id == 0) return false;
            OdemeAyristirma old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
            return ok;
        }
        public bool Delete(OdemeAyristirma item)
        {
            if (item == null || item.Id == 0) return false;
            OdemeAyristirma old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
            return ok;
        }
        private static List<OdemeAyristirma> ToList(DataTable table) { return new OdemeAyristirma().ToList<OdemeAyristirma>(table); }
        private static OdemeAyristirma Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
