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
    public class MeslekService
    {
        private readonly MeslekRepository repository;
        public MeslekService() : this(new MeslekRepository()) { }
        public MeslekService(MeslekRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public Meslek GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Meslek> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Meslek item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
            return item.Id;
        }
        public bool Update(Meslek item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Meslek old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
            return ok;
        }
        public bool Delete(Meslek item)
        {
            if (item == null || item.Id == 0) return false;
            Meslek old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_MESLEK);
            return ok;
        }
        public List<Meslek> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static Meslek Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Meslek> ToList(DataTable table) { return new Meslek().ToList<Meslek>(table); }
    }
}
