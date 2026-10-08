using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.Services.TBYS
{
    public class KiraSozlesmeService
    {
        private readonly KiraSozlesmeRepository repository;

        public KiraSozlesmeService() : this(new KiraSozlesmeRepository()) { }

        public KiraSozlesmeService(KiraSozlesmeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public KiraSozlesme GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<KiraSozlesme> GetAll()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAll());
        }

        public List<KiraSozlesme> GetAllActive()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAllActive());
        }
        public DataTable GetListReturnDataTable(int kiraciId, int aktif, int bolgeId)
        {
            return repository.SelectListReturnDataTable(kiraciId, aktif, bolgeId);
        }
        public List<KiraSozlesme> GetList(int kiraciId, int aktif, int bolgeId)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectList(kiraciId, aktif, bolgeId));
        }
        public DataTable GetRentIncreaseDue(int bolgeId, DateTime tarih)
        {
            return repository.SelectRentIncreaseDue(bolgeId, tarih.ReturnTRDateFormat());
        }
        public DataTable GetRealizedRentIncreases(int bolgeId)
        {
            DateTime baslangic = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime bitis = new DateTime(DateTime.Today.AddYears(1).Year, 12, 31);
            return repository.SelectRealizedRentIncreases(bolgeId, baslangic.ReturnTRDateFormat(), bitis.ReturnTRDateFormat());
        }

        private static KiraSozlesme Map(DataTable table)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(table).FirstOrDefault();
        }
    }
}
