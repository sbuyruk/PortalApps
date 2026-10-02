using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Data;
using System.Linq;

namespace Model.Services.Ortak
{
    public class IlceService
    {
        private readonly IlceRepository repository;

        public IlceService() : this(new IlceRepository())
        {
        }

        public IlceService(IlceRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Ilce GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public Ilce GetByProvinceAndDistrictName(string provinceName, string districtName)
        {
            return Map(repository.SelectByProvinceAndDistrictName(provinceName, districtName));
        }

        private static Ilce Map(DataTable table)
        {
            return new Ilce().ToList<Ilce>(table).FirstOrDefault();
        }
    }
}
