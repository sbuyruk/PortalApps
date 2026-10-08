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
    public class FaaliyetKatilimService
    {
        private readonly FaaliyetKatilimRepository repository;
        private readonly MtsLookupRepository lookupRepository;

        public FaaliyetKatilimService() : this(new FaaliyetKatilimRepository(), new MtsLookupRepository())
        {
        }

        public FaaliyetKatilimService(FaaliyetKatilimRepository repository, MtsLookupRepository lookupRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
            this.lookupRepository = lookupRepository ?? throw new ArgumentNullException("lookupRepository");
        }

        public FaaliyetKatilimService(MtsLookupRepository lookupRepository)
            : this(new FaaliyetKatilimRepository(), lookupRepository)
        {
        }

        public FaaliyetKatilim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<FaaliyetKatilim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public int Save(FaaliyetKatilim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
            return item.Id;
        }

        public bool Update(FaaliyetKatilim item)
        {
            FaaliyetKatilim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
            return updated;
        }

        public bool Delete(FaaliyetKatilim item)
        {
            FaaliyetKatilim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETKATILIM);
            return deleted;
        }

        public List<FaaliyetKatilim> GetByFaaliyetAndKatilimci(int faaliyetId, int katilimciId)
        {
            return ToList(lookupRepository.SelectFaaliyetKatilim(faaliyetId, katilimciId));
        }

        public List<FaaliyetKatilim> GetByKatilimciId(int katilimciId)
        {
            return ToList(lookupRepository.SelectFaaliyetKatilimByKatilimciId(katilimciId));
        }

        public List<FaaliyetKatilim> GetByFaaliyetId(int faaliyetId)
        {
            return ToList(lookupRepository.SelectFaaliyetKatilimByFaaliyetId(faaliyetId));
        }

        private static List<FaaliyetKatilim> ToList(DataTable table)
        {
            return new FaaliyetKatilim().ToList<FaaliyetKatilim>(table);
        }

        private static FaaliyetKatilim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
