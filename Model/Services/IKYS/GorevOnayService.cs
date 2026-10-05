using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class GorevOnayService
    {
        private readonly GorevOnayRepository repository;

        public GorevOnayService() : this(new GorevOnayRepository()) { }

        public GorevOnayService(GorevOnayRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public GorevOnay GetById(int id) { return Map(repository.SelectById(id)); }
        public List<GorevOnay> GetAll() { return ToList(repository.SelectAll()); }

        public int Save(GorevOnay item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return item.Id;
        }

        public bool Update(GorevOnay item)
        {
            if (item == null) throw new ArgumentNullException("item");
            GorevOnay old = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }
            if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return isSuccess;
        }

        public bool Delete(GorevOnay item)
        {
            if (item == null || item.Id == 0) return false;
            GorevOnay old = GetById(item.Id);
            if (old == null) return false;
            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            return isDeleted;
        }

        public bool UpdateAllSecildiToFalse() { return repository.UpdateAllSecildiToFalse(); }
        public bool UpdateAllSecildiToTrue(string idString) { return repository.UpdateAllSecildiToTrue(ParseIdList(idString)); }
        public bool UpdateAllOdendiToTrue(string idString) { return repository.UpdateAllOdendiToTrue(ParseIdList(idString)); }
        public DataTable GetAllReturnDataTable(int personelId, DateTime since) { return repository.SelectAllReturnDataTable(personelId, since); }
        public DataTable GetAllByPersonelReturnDataTable(int personelId)
        {
            if (personelId <= 0) throw new ArgumentOutOfRangeException("personelId");
            return repository.SelectAllByPersonelReturnDataTable(personelId);
        }
        public DataTable GetByTarihReturnDataTable(DateTime baslangic, DateTime bitis) { return repository.SelectByTarihReturnDataTable(baslangic, bitis); }
        public GorevOnay GetByPersonelTarih(int personelId, DateTime baslangic, DateTime bitis) { return Map(repository.SelectByPersonelTarih(personelId, baslangic, bitis)); }
        public List<GorevOnay> GetAllBySecildi(bool secildi) { return ToList(repository.SelectAllBySecildi(secildi)); }
        public DataTable GetBekleyenAmirOnayi()
        {
            return repository.SelectBekleyenAmirOnayi();
        }
        public DataTable GetBekleyenAmirOnayiByBirimIds(string birimIdListStr)
        {
            List<int> ids = ParseIdList(birimIdListStr);
            return ids.Count == 0 ? new DataTable() : repository.SelectBekleyenAmirOnayiByBirimIds(ids);
        }
        public bool HasDateConflict(int personelId, DateTime baslangic, DateTime bitis, int gorevOnayId)
        {
            return repository.SelectForDateConflict(personelId, baslangic, bitis, gorevOnayId) != null;
        }

        private static GorevOnay Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<GorevOnay> ToList(DataTable table) { return new GorevOnay().ToList<GorevOnay>(table); }

        private static List<int> ParseIdList(string idString)
        {
            List<int> ids = new List<int>();
            if (string.IsNullOrWhiteSpace(idString)) return ids;
            string value = idString.Trim();
            if (value.StartsWith("(") && value.EndsWith(")")) value = value.Substring(1, value.Length - 2);
            foreach (string part in value.Split(','))
            {
                int id;
                if (!int.TryParse(part.Trim(), out id))
                    throw new ArgumentException("Görev onay ID listesi geçersiz bir değer içeriyor.", "idString");
                ids.Add(id);
            }
            return ids;
        }

        public List<GorevOnay> SelectByBirimIdAndDurum(int birimId, GorevOnay.AmirOnayDurumu onayBekliyor)
        {
            if (birimId <= 0)
            {
                return new List<GorevOnay>();
            }

            DataTable dataTable = repository.SelectByBirimIdAndDurum(birimId, (int)onayBekliyor);
            return dataTable == null ? new List<GorevOnay>() : ToList(dataTable);
        }
    }
}
