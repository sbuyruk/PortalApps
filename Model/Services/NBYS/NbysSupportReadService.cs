using DAO.Repositories.NBYS;
using Model.Ortak;
using Model.Portal;
using System.Collections.Generic;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public sealed class NbysSupportReadService
    {
        private readonly NbysSupportReadRepository repository;

        public NbysSupportReadService(PortalLogContext context)
        {
            repository = new NbysSupportReadRepository(context);
        }

        public List<Il> GetFtkProvinces()
        {
            return new Il().ToList<Il>(repository.FtkProvinces());
        }

        public List<Ilce> GetFtkDistricts(int provinceId)
        {
            return new Ilce().ToList<Ilce>(repository.FtkDistricts(provinceId, ProjeConstants.ILCE_MERKEZ));
        }

        public List<ProgramYetki> GetPermissionSettings()
        {
            return new ProgramYetki().ToList<ProgramYetki>(repository.PermissionSettings(ProjeConstants.NBYS, ProjeConstants.NBYS_BOLGE_NAKITBAGISCILISTESI));
        }
    }
}
