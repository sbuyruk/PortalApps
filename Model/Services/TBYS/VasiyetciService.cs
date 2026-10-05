using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class VasiyetciService
    {
        private readonly VasiyetciRepository repository;

        public VasiyetciService() : this(new VasiyetciRepository()) { }

        public VasiyetciService(VasiyetciRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Vasiyetci GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Vasiyetci> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetByRegion(int bolgeId)
        {
            return repository.SelectByRegion(bolgeId, ProjeConstants.HEPSI_INT, ProjeConstants.BOLGE_GENELMUDURLUK_INT);
        }

        public string GetByIdAsJson(int id, ref int rowCount)
        {
            DataTable table = repository.SelectByIdForJson(id);
            rowCount = table == null ? 0 : table.Rows.Count;
            return new Vasiyetci().ToJSON(table);
        }

        public DataTable GetAllForDataTable(bool excludeDeceased, ref int rowCount)
        {
            DataTable table = repository.SelectAllForDataTable(excludeDeceased, int.Parse(ProjeConstants.BAGISCI_SAG_INT));
            rowCount = table == null ? 0 : table.Rows.Count;
            return table;
        }

        public List<Vasiyetci> GetByFilters(bool onlyAlive, bool fullTcKimlikNo, bool fullBirthDate)
        {
            return ToList(repository.SelectByFilters(onlyAlive, fullTcKimlikNo, fullBirthDate, int.Parse(ProjeConstants.BAGISCI_VEFAT_INT)));
        }

        public int Save(Vasiyetci item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
            }

            return item.Id;
        }

        public bool Update(Vasiyetci item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Vasiyetci oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
            }

            return isSuccess;
        }

        public bool Delete(Vasiyetci item)
        {
            if (item == null || item.Id == 0) return false;

            Vasiyetci oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
            }

            return isDeleted;
        }

        private static List<Vasiyetci> ToList(DataTable table)
        {
            return new Vasiyetci().ToList<Vasiyetci>(table);
        }

        private static Vasiyetci Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
