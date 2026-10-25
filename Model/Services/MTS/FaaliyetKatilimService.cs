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

        public FaaliyetKatilimService() : this(new FaaliyetKatilimRepository())
        {
        }

        public FaaliyetKatilimService(Utility.Logging.PortalLogContext context) : this(new FaaliyetKatilimRepository(context)) { }

        public FaaliyetKatilimService(FaaliyetKatilimRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public FaaliyetKatilim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<FaaliyetKatilim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<FaaliyetKatilim> GetByFaaliyetAndKatilimci(int faaliyetId, int katilimciId)
        {
            return ToList(repository.SelectByFaaliyetAndKatilimci(faaliyetId, katilimciId));
        }

        public List<FaaliyetKatilim> GetByKatilimciId(int katilimciId)
        {
            return ToList(repository.SelectByKatilimciId(katilimciId));
        }

        public List<FaaliyetKatilim> GetByFaaliyetId(int faaliyetId)
        {
            return ToList(repository.SelectByFaaliyetId(faaliyetId));
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
