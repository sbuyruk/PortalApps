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
    public class OnarimService
    {
        private readonly OnarimRepository repository;

        public OnarimService() : this(new OnarimRepository()) { }

        public OnarimService(OnarimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Onarim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Onarim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<Onarim> GetByTasinmazId(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazId(tasinmazId));
        }

        public DataTable GetAllForDataTable()
        {
            return repository.SelectAllForDataTable();
        }

        public List<Onarim> GetByOnarimId(int onarimId)
        {
            return ToList(repository.SelectByOnarimId(onarimId));
        }

        public Onarim GetNext(int onarimId)
        {
            DataTable table = repository.SelectNext(onarimId);
            return table == null ? GetMin() : Map(table);
        }

        public Onarim GetPrevious(int onarimId)
        {
            DataTable table = repository.SelectPrevious(onarimId);
            return table == null ? GetMax() : Map(table);
        }

        public Onarim GetMax()
        {
            return GetById(ReadId(repository.SelectMaxId()));
        }

        public Onarim GetMin()
        {
            return GetById(ReadId(repository.SelectMinId()));
        }

        public List<Onarim> GetByTasinmazIdWithAddress(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazIdWithAddress(tasinmazId));
        }

        public int Save(Onarim item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return item.Id;
        }

        public bool Update(Onarim item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Onarim oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return isSuccess;
        }

        public bool Delete(Onarim item)
        {
            if (item == null || item.Id == 0) return false;

            Onarim oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return isDeleted;
        }

        private static List<Onarim> ToList(DataTable table)
        {
            return new Onarim().ToList<Onarim>(table);
        }

        private static Onarim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }

        private static int ReadId(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Id"].ConvertToInt();
        }
    }
}
