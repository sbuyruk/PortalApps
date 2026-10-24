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
    public class FTKKisiService
    {
        private readonly FTKKisiRepository repository;

        public FTKKisiService(PortalLogContext context) : this(new FTKKisiRepository(context)) { }
        public FTKKisiService() : this(new FTKKisiRepository()) { }
        public FTKKisiService(FTKKisiRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public FTKKisi GetById(int id) { return Map(repository.SelectById(id)).FirstOrDefault(); }
        public DataTable GetAllTable() { return repository.SelectAllWithLocation(); }

        public FTKKisi GetByName(string name, string surname)
        {
            name = string.IsNullOrEmpty(name) ? "#${}?" : name.Trim();
            surname = string.IsNullOrEmpty(surname) ? "#${}?" : surname.Trim();
            return Map(repository.SelectByName(name, surname)).FirstOrDefault();
        }

        public FTKKisi GetByIdentityNumber(long identityNumber)
        {
            return Map(repository.SelectByIdentityNumber(identityNumber)).FirstOrDefault();
        }

        public FTKKisi GetByName(string name, string surname, int excludedId)
        {
            name = string.IsNullOrEmpty(name) ? "#${}?" : name.Trim();
            surname = string.IsNullOrEmpty(surname) ? "#${}?" : surname.Trim();
            return Map(repository.SelectByName(name, surname, excludedId)).FirstOrDefault();
        }

        public DataTable GetMembersTable(int provinceId, int districtId, int operationId, bool onlyActive)
        {
            int repositoryOperationId = operationId == ProjeConstants.HEPSI_INT ? -1 : operationId;
            return repository.SelectMembers(provinceId, districtId, repositoryOperationId,
                onlyActive, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF);
        }

        public List<FTKKisi> GetMembers(int provinceId, int districtId, int operationId, bool onlyActive)
        {
            return Map(GetMembersTable(provinceId, districtId, operationId, onlyActive));
        }

        public FTKKisi GetGovernor(int provinceId)
        {
            return Map(repository.SelectGovernor(provinceId, ProjeConstants.VALILIK_INT)).FirstOrDefault();
        }

        public FTKKisi GetDistrictGovernor(int provinceId, int districtId)
        {
            return Map(repository.SelectDistrictGovernor(provinceId, districtId)).FirstOrDefault();
        }

        private static List<FTKKisi> Map(DataTable table)
        {
            return new FTKKisi().ToList<FTKKisi>(table);
        }
    }
}
