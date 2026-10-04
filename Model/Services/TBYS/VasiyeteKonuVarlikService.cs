using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class VasiyeteKonuVarlikService
    {
        private readonly VasiyeteKonuVarlikRepository repository;

        public VasiyeteKonuVarlikService() : this(new VasiyeteKonuVarlikRepository()) { }

        public VasiyeteKonuVarlikService(VasiyeteKonuVarlikRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public VasiyeteKonuVarlik GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<VasiyeteKonuVarlik> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<VasiyeteKonuVarlik> GetByVasiyetciId(int vasiyetciId)
        {
            return ToList(repository.SelectByVasiyetciId(vasiyetciId));
        }

        public int Save(VasiyeteKonuVarlik item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return item.Id;
        }

        public bool Update(VasiyeteKonuVarlik item)
        {
            if (item == null) throw new ArgumentNullException("item");

            VasiyeteKonuVarlik oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return isSuccess;
        }

        public bool Delete(VasiyeteKonuVarlik item)
        {
            if (item == null || item.Id == 0) return false;

            VasiyeteKonuVarlik oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
            }

            return isDeleted;
        }

        private static List<VasiyeteKonuVarlik> ToList(DataTable table)
        {
            return new VasiyeteKonuVarlik().ToList<VasiyeteKonuVarlik>(table);
        }

        private static VasiyeteKonuVarlik Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
