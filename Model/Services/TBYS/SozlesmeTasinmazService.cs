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
    public class SozlesmeTasinmazService
    {
        private readonly SozlesmeTasinmazRepository repository;

        public SozlesmeTasinmazService() : this(new SozlesmeTasinmazRepository()) { }

        public SozlesmeTasinmazService(SozlesmeTasinmazRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SozlesmeTasinmaz GetById(int id) { return Map(repository.SelectById(id)); }
        public List<SozlesmeTasinmaz> GetAll() { return ToList(repository.SelectAll()); }
        public List<SozlesmeTasinmaz> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public List<SozlesmeTasinmaz> GetByTasinmazId(int id) { return ToList(repository.SelectByTasinmazId(id)); }
        public List<SozlesmeTasinmaz> GetBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId) { return ToList(repository.SelectBySozlesmeIdTasinmazId(sozlesmeId, tasinmazId, bolumId)); }
        public List<SozlesmeTasinmaz> GetByBolumId(int bolumId) { return ToList(repository.SelectByBolumId(bolumId)); }
        public DataTable GetBySozlesmeIdReturnDataTable(int id) { return repository.SelectBySozlesmeIdReturnDataTable(id); }
        public DataTable GetBySozlesmeIdReturnList(int id) { return repository.SelectBySozlesmeIdReturnList(id); }
        public DataTable GetBySozlesmeIdReturnDT(int id) { return repository.SelectBySozlesmeIdReturnDT(id); }

        public string GetBySozlesmeIdReturnJson(int id)
        {
            return new SozlesmeTasinmaz().ToJSON(repository.SelectBySozlesmeIdReturnList(id));
        }

        public decimal GetSumMetrekareBySozlesmeId(int id)
        {
            DataTable table = repository.SelectSumMetrekareBySozlesmeId(id);
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Metrekare"].ReturnZeroIfNull().ConvertToDecimal();
        }

        public bool DeleteBySozlesmeId(SozlesmeTasinmaz item, int sozlesmeId)
        {
            bool isDeleted = repository.DeleteBySozlesmeId(sozlesmeId);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
            }
            return isDeleted;
        }

        public int Save(SozlesmeTasinmaz item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
            }
            return item.Id;
        }

        public bool Update(SozlesmeTasinmaz item)
        {
            if (item == null) throw new ArgumentNullException("item");
            SozlesmeTasinmaz oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
            }
            return isSuccess;
        }

        public bool Delete(SozlesmeTasinmaz item)
        {
            if (item == null || item.Id == 0) return false;
            SozlesmeTasinmaz oldItem = GetById(item.Id);
            if (oldItem == null) return false;
            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
            }
            return isDeleted;
        }

        private static List<SozlesmeTasinmaz> ToList(DataTable table) { return new SozlesmeTasinmaz().ToList<SozlesmeTasinmaz>(table); }
        private static SozlesmeTasinmaz Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
