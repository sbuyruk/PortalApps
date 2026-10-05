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
    public class IsTecrubeService
    {
        private readonly IsTecrubeRepository repository;
        public IsTecrubeService() : this(new IsTecrubeRepository()) { }
        public IsTecrubeService(IsTecrubeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public IsTecrube GetById(int id) { return Map(repository.SelectById(id)); }
        public List<IsTecrube> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IsTecrube item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ISTECRUBE);
            return item.Id;
        }
        public bool Update(IsTecrube item)
        {
            if (item == null) throw new ArgumentNullException("item");
            IsTecrube old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISTECRUBE);
            return ok;
        }
        public bool Delete(IsTecrube item)
        {
            if (item == null || item.Id == 0) return false;
            IsTecrube old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_ISTECRUBE);
            return ok;
        }
        public List<IsTecrube> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static IsTecrube Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IsTecrube> ToList(DataTable table) { return new IsTecrube().ToList<IsTecrube>(table); }
    }
}
