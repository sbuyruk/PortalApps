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
    public class IzinTalepService
    {
        private readonly IzinTalepRepository repository;

        public IzinTalepService() : this(new IzinTalepRepository()) { }

        public IzinTalepService(IzinTalepRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public IzinTalep GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<IzinTalep> GetAll()
        {
            return new IzinTalep().ToList<IzinTalep>(repository.SelectAll());
        }

        public int Save(IzinTalep item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return item.Id;
        }

        public bool Update(IzinTalep item)
        {
            if (item == null) throw new ArgumentNullException("item");
            IzinTalep old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return ok;
        }

        public bool Delete(IzinTalep item)
        {
            if (item == null || item.Id == 0) return false;
            IzinTalep old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINTALEP);
            return ok;
        }

        public string GetIzinTalepleriReturnJson(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            return new IzinTalep().ToJSON(GetIzinTalepleri(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri));
        }

        public DataTable GetIzinTalepleriReturnDataTable(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            return GetIzinTalepleri(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri);
        }

        public IzinTalep GetByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar) { return Map(repository.SelectByPersonelIdBasBitTar(personelId, basTar, bitTar)); }
        public IzinTalep GetIslemiDevamEden(int personelId, int izinTipi) { return Map(repository.SelectIslemiDevamEden(personelId, izinTipi)); }
        public IzinTalep GetSonByPersonel(int izinTipi, int personelId) { return Map(repository.SelectSonByPersonel(izinTipi, personelId)); }

        private DataTable GetIzinTalepleri(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri)
        {
            List<int> izinDonemIds = null;
            if (sadeceGecerliDonemTalepleri)
            {
                DateTime today = DateTime.Today;
                IzinDonemService izinDonemService = new IzinDonemService();
                IzinDonem ucretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                IzinDonem mazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, today);
                if (ucretli != null && mazeret != null)
                {
                    izinDonemIds = new List<int> { ucretli.Id, mazeret.Id };
                    IzinDonem gelecekUcretli = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, ucretli.BitisTarihi.AddDays(1));
                    IzinDonem gelecekMazeret = izinDonemService.GetByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, mazeret.BitisTarihi.AddDays(1));
                    if (gelecekUcretli != null) izinDonemIds.Add(gelecekUcretli.Id);
                    if (gelecekMazeret != null) izinDonemIds.Add(gelecekMazeret.Id);
                }
            }
            return repository.SelectIzinTalepleri(personelId, izinTipi, mazeretHaric, ProjeConstants.IZINTIPI_MAZERET_INT, izinDonemIds);
        }

        private static IzinTalep Map(DataTable table)
        {
            return new IzinTalep().ToList<IzinTalep>(table).FirstOrDefault();
        }
    }
}
