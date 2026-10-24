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
    public class FTKIslemService
    {
        private readonly FTKIslemRepository repository;
        public FTKIslemService(PortalLogContext context) : this(new FTKIslemRepository(context)) { }
        public FTKIslemService() : this(new FTKIslemRepository()) { }
        public FTKIslemService(FTKIslemRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public FTKIslem GetById(int id) { return Map(repository.SelectById(id)).FirstOrDefault(); }
        public FTKIslem GetByLocation(int provinceId, int districtId)
        {
            return Map(repository.SelectByLocation(provinceId, districtId)).FirstOrDefault();
        }

        private static List<FTKIslem> Map(DataTable table)
        {
            return new FTKIslem().ToList<FTKIslem>(table);
        }
    }
}
