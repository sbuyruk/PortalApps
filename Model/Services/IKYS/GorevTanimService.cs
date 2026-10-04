using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

namespace Model.Services.IKYS
{
    public class GorevTanimService
    {
        private readonly GorevTanimRepository repository;
        public GorevTanimService() : this(new GorevTanimRepository()) { }
        public GorevTanimService(GorevTanimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public GorevTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<GorevTanim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(GorevTanim item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
            return item.Id;
        }
        public bool Update(GorevTanim item)
        {
            if (item == null) throw new ArgumentNullException("item");
            GorevTanim old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
            return ok;
        }
        public bool Delete(GorevTanim item)
        {
            if (item == null || item.Id == 0) return false;
            GorevTanim old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
            return ok;
        }
        public GorevTanim GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public List<GorevTanim> GetByBirimId(int birimId) { return ToList(repository.SelectByBirimId(birimId)); }
        public GorevTanim GetByGorevId(int gorevId) { return GetById(gorevId); }
        public DataTable GetAllReturnDataTable(PersonelTipi personelTipi) { return repository.SelectAllReturnDataTable(personelTipi == PersonelTipi.Tumu ? (int?)null : (int)personelTipi); }
        private static GorevTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<GorevTanim> ToList(DataTable table) { return new GorevTanim().ToList<GorevTanim>(table); }
    }
}
