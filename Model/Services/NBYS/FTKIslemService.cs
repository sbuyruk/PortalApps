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

        public int Save(FTKIslem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
            return item.Id;
        }

        public bool Update(FTKIslem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            FTKIslem previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
            return updated;
        }

        public bool Delete(FTKIslem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (item.Id == 0) return false;
            FTKIslem previous = GetById(item.Id);
            if (previous == null) return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(previous, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
            return deleted;
        }

        private static List<FTKIslem> Map(DataTable table)
        {
            return new FTKIslem().ToList<FTKIslem>(table);
        }
    }
}
