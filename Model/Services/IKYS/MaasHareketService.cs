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
    public class MaasHareketService
    {
        private readonly MaasHareketRepository repository;
        public MaasHareketService() : this(new MaasHareketRepository()) { }
        public MaasHareketService(MaasHareketRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public MaasHareket GetById(int id) { return Map(repository.SelectById(id)); }
        public List<MaasHareket> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(MaasHareket item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return item.Id;
        }
        public bool Update(MaasHareket item)
        {
            if (item == null) throw new ArgumentNullException("item");
            MaasHareket old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        public bool Delete(MaasHareket item)
        {
            if (item == null || item.Id == 0) return false;
            MaasHareket old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        public MaasHareket GetByTarih(DateTime tarih) { return Map(repository.SelectByTarih(tarih)); }
        public List<MaasHareket> GetMaasListesiByTarih(DateTime tarih) { return ToList(repository.SelectByTarih(tarih)); }
        public bool DeleteByGrupId(MaasHareket item, int grupId)
        {
            bool ok = repository.DeleteByGrupId(grupId);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return ok;
        }
        private static MaasHareket Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<MaasHareket> ToList(DataTable table) { return new MaasHareket().ToList<MaasHareket>(table); }
    }
}
