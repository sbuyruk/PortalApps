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

        public AramaGorusmeService() : this(new AramaGorusmeRepository())
        {
        }

        public AramaGorusmeService(Utility.Logging.PortalLogContext context) : this(new AramaGorusmeRepository(context)) { }

        public AramaGorusmeService(AramaGorusmeRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
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
            return Map(repository.SelectByFaaliyetId(faaliyetId));
        }

        public List<AramaGorusme> GetByArayanId(int arayanId)
        {
            return ToList(repository.SelectByArayanId(arayanId));
        }

        public DataTable GetByFilter(int arayanId, string gorusmeSekli, DateTime basTar, DateTime bitTar)
        {
            if (bitTar < ProjeConstants.REFERANS_TARIHI)
                bitTar = DateTime.Today;

            if (string.IsNullOrEmpty(gorusmeSekli) || gorusmeSekli.Equals(ProjeConstants.HEPSI))
                gorusmeSekli = string.Empty;

            return repository.SelectByFilter(
                arayanId,
                gorusmeSekli,
                basTar,
                bitTar,
                ProjeConstants.REFERANS_TARIHI,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE);
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
