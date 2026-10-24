using Utility.Logging;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class FTKService
    {
        private readonly FTKRepository repository;
        public FTKService(PortalLogContext context) : this(new FTKRepository(context)) { }
        public FTKService() : this(new FTKRepository()) { }
        public FTKService(FTKRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public FTK GetById(int id) { return Map(repository.SelectById(id)).FirstOrDefault(); }

        public DataTable GetLatestTable(int regionId, int provinceId, int districtId,
            string establishmentDate, string updateDate)
        {
            int? region = regionId == ProjeConstants.HEPSI_INT || regionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? (int?)null : regionId;
            int? province = provinceId > 0 ? (int?)provinceId : null;
            bool districtsOnly = districtId == ProjeConstants.SADECE_ILCELER_INT;
            int? district = districtId == 0 || districtsOnly ? (int?)null : districtId;
            DateTime? establishedSince = string.IsNullOrEmpty(establishmentDate)
                ? (DateTime?)null : establishmentDate.ConvertToDatetime();
            DateTime? updatedSince = string.IsNullOrEmpty(updateDate.ConvertToDatetimeEmptyIfNull())
                ? (DateTime?)null : updateDate.ConvertToDatetime();
            return repository.SelectLatest(region, province, district, districtsOnly,
                ProjeConstants.VALILIK_INT, establishedSince, updatedSince);
        }

        public List<FTK> GetLatest(int provinceId, int districtId)
        {
            return Map(GetLatestTable(ProjeConstants.HEPSI_INT, provinceId, districtId, string.Empty, string.Empty));
        }

        public FTK GetByLocationAndUpdateDate(int provinceId, int districtId, DateTime updateDate)
        {
            return Map(repository.SelectByLocationAndUpdateDate(provinceId, districtId, updateDate)).FirstOrDefault();
        }

        public int GetMaxCounter(int provinceId, int districtId)
        {
            DataTable table = repository.SelectMaxCounter(provinceId, districtId);
            return table.Rows[0]["Sayac"].ConvertToInt();
        }

        public int GetEstablishedProvinceCount(int regionId)
        {
            DataTable table = repository.SelectEstablishedProvinceCount(regionId, ProjeConstants.VALILIK_INT);
            return table == null ? 0 : table.Rows.Count;
        }

        public int GetEstablishedDistrictCount(int regionId)
        {
            DataTable table = repository.SelectEstablishedDistrictCount(regionId);
            return table == null ? 0 : table.Rows.Count;
        }

        public int GetUpdatedLocationCount(int regionId, DateTime updatedSince)
        {
            DataTable table = repository.SelectUpdatedLocationCount(regionId, updatedSince);
            return table == null ? 0 : table.Rows.Count;
        }

        public int GetEstablishedLocationCount(int regionId, DateTime establishedSince)
        {
            DataTable table = repository.SelectEstablishedLocationCount(regionId, establishedSince);
            return table == null ? 0 : table.Rows.Count;
        }

        public DataTable GetProvincesWithoutFTK(int regionId, int provinceId)
        {
            int? region = regionId == ProjeConstants.HEPSI_INT || regionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? (int?)null : regionId;
            int? province = provinceId < 1 ? (int?)null : provinceId;
            return repository.SelectProvincesWithoutFTK(region, province, ProjeConstants.VALILIK_INT);
        }

        public DataTable GetDistrictsWithoutFTK(int regionId, int provinceId)
        {
            int? region = regionId == ProjeConstants.HEPSI_INT || regionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? (int?)null : regionId;
            int? province = provinceId < 1 ? (int?)null : provinceId;
            return repository.SelectDistrictsWithoutFTK(region, province, ProjeConstants.ILCE_MERKEZ);
        }

        private static List<FTK> Map(DataTable table) { return new FTK().ToList<FTK>(table); }
    }
}
