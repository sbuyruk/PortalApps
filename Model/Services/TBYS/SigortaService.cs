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
    public class SigortaService
    {
        private readonly SigortaRepository repository;

        public SigortaService() : this(new SigortaRepository()) { }

        public SigortaService(SigortaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Sigorta GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Sigorta> GetAll()
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectAll());
        }

        public List<Sigorta> GetByTasinmazId(int tasinmazId)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectByTasinmazId(tasinmazId));
        }

        public List<Sigorta> GetByIdList(int id)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectById(id));
        }

        public DataTable GetInventoryList()
        {
            return repository.SelectInventoryList();
        }

        public decimal GetInsuranceValueTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectInsuranceValueTotal(sigortaCinsi));
        }

        public decimal GetPremiumTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectPremiumTotal(sigortaCinsi));
        }

        public string GetInventoryListAsJson()
        {
            return new Sigorta().ToJSON(GetInventoryList());
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId)
        {
            return GetByTasinmazId(tasinmazId).FirstOrDefault();
        }

        public int Save(Sigorta item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return item.Id;
        }

        public bool Update(Sigorta item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Sigorta oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return isSuccess;
        }

        public bool Delete(Sigorta item)
        {
            if (item == null || item.Id == 0) return false;

            Sigorta oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SIGORTA);
            }

            return isDeleted;
        }

        private static Sigorta Map(DataTable table)
        {
            return new Sigorta().ToList<Sigorta>(table).FirstOrDefault();
        }

        private static decimal ReadTotal(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }
    }
}
