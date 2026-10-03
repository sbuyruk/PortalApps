using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Data;
using System.Collections.Generic;
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

        public List<Ilce> GetByProvinceId(int provinceId)
        {
            return new Ilce().ToList<Ilce>(repository.SelectByProvinceId(provinceId));
        }

        public List<Ilce> GetAll()
        {
            return new Ilce().ToList<Ilce>(repository.SelectAll());
        }

        public List<Ilce> GetByProvinceName(string provinceName)
        {
            return new Ilce().ToList<Ilce>(repository.SelectByProvinceName(provinceName));
        }

        public DataTable GetDistrictFromLegacyTable(int districtId)
        {
            return repository.SelectDistrictFromLegacyTable(districtId);
        }

        public Ilce GetByProvinceAndDistrictName(string provinceName, string districtName)
        {
            return Map(repository.SelectByProvinceAndDistrictName(provinceName, districtName));
        }

        public Ilce GetByProvinceAndDistrictContains(string provinceName, string districtName)
        {
            return Map(repository.SelectByProvinceAndDistrictContains(provinceName, districtName));
        }

        public int CountByRegion(int regionId)
        {
            DataTable table = repository.SelectCountByRegion(regionId);
            return table == null || table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0]["Adet"]);
        }

        public System.Collections.Generic.List<Ilce> GetWithFTK(int provinceId)
        {
            return new Ilce().ToList<Ilce>(repository.SelectWithFTK(provinceId));
        }

        private static Ilce Map(DataTable table)
        {
            return new Ilce().ToList<Ilce>(table).FirstOrDefault();
        }
    }
}
