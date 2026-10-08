using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class AramaGorusmeService
    {
        private readonly AramaGorusmeRepository repository;
        private readonly MtsLookupRepository lookupRepository;

        public AramaGorusmeService() : this(new AramaGorusmeRepository(), new MtsLookupRepository())
        {
        }

        public AramaGorusmeService(AramaGorusmeRepository repository, MtsLookupRepository lookupRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
            this.lookupRepository = lookupRepository ?? throw new ArgumentNullException("lookupRepository");
        }

        public AramaGorusmeService(MtsLookupRepository lookupRepository)
            : this(new AramaGorusmeRepository(), lookupRepository)
        {
        }

        public AramaGorusme GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<AramaGorusme> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public AramaGorusme GetByFaaliyetId(int faaliyetId)
        {
            return Map(lookupRepository.SelectAramaGorusmeByFaaliyetId(faaliyetId));
        }

        public List<AramaGorusme> GetByArayanId(int arayanId)
        {
            return ToList(lookupRepository.SelectAramaGorusmeByArayanId(arayanId));
        }

        public DataTable GetByFilter(int arayanId, string gorusmeSekli, DateTime basTar, DateTime bitTar)
        {
            if (bitTar < ProjeConstants.REFERANS_TARIHI)
                bitTar = DateTime.Today;

            if (string.IsNullOrEmpty(gorusmeSekli) || gorusmeSekli.Equals(ProjeConstants.HEPSI))
                gorusmeSekli = string.Empty;

            return lookupRepository.SelectAramaGorusmeByFilter(
                arayanId,
                gorusmeSekli,
                basTar,
                bitTar,
                ProjeConstants.REFERANS_TARIHI,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE);
        }

        public int Save(AramaGorusme item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
            return item.Id;
        }

        public bool Update(AramaGorusme item)
        {
            AramaGorusme old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
            return updated;
        }

        public bool Delete(AramaGorusme item)
        {
            AramaGorusme old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_ARAMAGORUSME);
            return deleted;
        }

        private static List<AramaGorusme> ToList(DataTable table)
        {
            return new AramaGorusme().ToList<AramaGorusme>(table);
        }

        private static AramaGorusme Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
