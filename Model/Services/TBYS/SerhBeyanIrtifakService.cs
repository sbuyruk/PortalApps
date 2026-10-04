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
    public class SerhBeyanIrtifakService
    {
        private readonly SerhBeyanIrtifakRepository repository;

        public SerhBeyanIrtifakService() : this(new SerhBeyanIrtifakRepository()) { }

        public SerhBeyanIrtifakService(SerhBeyanIrtifakRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SerhBeyanIrtifak GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<SerhBeyanIrtifak> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<SerhBeyanIrtifak> GetByTasinmazId(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazId(tasinmazId));
        }

        public int Save(SerhBeyanIrtifak item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return item.Id;
        }

        public bool Update(SerhBeyanIrtifak item)
        {
            if (item == null) throw new ArgumentNullException("item");

            SerhBeyanIrtifak oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isSuccess;
        }

        public bool Delete(SerhBeyanIrtifak item)
        {
            if (item == null || item.Id == 0) return false;

            SerhBeyanIrtifak oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
            }

            return isDeleted;
        }

        private static List<SerhBeyanIrtifak> ToList(DataTable table)
        {
            return new SerhBeyanIrtifak().ToList<SerhBeyanIrtifak>(table);
        }

        private static SerhBeyanIrtifak Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
