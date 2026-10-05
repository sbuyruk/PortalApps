using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class MahsupService
    {
        private readonly MahsupRepository repository;
        public MahsupService() : this(new MahsupRepository()) { }
        public MahsupService(MahsupRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public Mahsup GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Mahsup> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Mahsup item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return item.Id;
        }
        public bool Update(Mahsup item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Mahsup old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return ok;
        }
        public bool Delete(Mahsup item)
        {
            if (item == null || item.Id == 0) return false;
            Mahsup old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
            return ok;
        }
        public List<Mahsup> GetByDonemId(int donemId) { return ToList(repository.SelectByDonemId(donemId)); }
        public List<Mahsup> GetByPersonelId(int personelId, int izinTipi) { return ToList(repository.SelectByPersonelId(personelId, izinTipi, ProjeConstants.IZINTIPI_MAZERET_INT)); }
        private static Mahsup Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Mahsup> ToList(DataTable table) { return new Mahsup().ToList<Mahsup>(table); }
    }
}
