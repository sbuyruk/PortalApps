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
    public class EkstreAktarmaService
    {
        private readonly EkstreAktarmaRepository repository;
        public EkstreAktarmaService() : this(new EkstreAktarmaRepository()) { }
        public EkstreAktarmaService(EkstreAktarmaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public EkstreAktarma GetById(int id) { return Map(repository.SelectById(id)).FirstOrDefault(); }
        public List<EkstreAktarma> GetAll() { return Map(repository.SelectAll()); }

        public DataTable GetListTable(DateTime processDate, bool excludeTransferred, string bankName,
            ref int rowCount)
        {
            string filter = bankName == ProjeConstants.HEPSI ? string.Empty : bankName;
            DataTable table = repository.SelectList(processDate, excludeTransferred, filter);
            rowCount = table == null ? 0 : table.Rows.Count;
            return table;
        }

        public List<EkstreAktarma> GetByBankAndReceipt(string bankNamePart, string receiptNumber)
        {
            return Map(repository.SelectByBankAndReceipt(bankNamePart, receiptNumber));
        }

        public List<EkstreAktarma> GetByIdList(string idList, bool includeIdDescending,
            ref int rowCount)
        {
            List<int> ids = ParseIds(idList);
            if (ids.Count == 0)
            {
                rowCount = 0;
                return new List<EkstreAktarma>();
            }
            DataTable table = repository.SelectByIds(ids, includeIdDescending);
            rowCount = table == null ? 0 : table.Rows.Count;
            return Map(table);
        }

        public bool ExistsByBankAndProcessDate(string bankName, DateTime processDate)
        {
            return repository.ExistsByBankAndProcessDate(bankName, processDate);
        }

        public bool CheckIsExistByBankaAdiAndIslemTarihi(string bankName, DateTime processDate)
        {
            return ExistsByBankAndProcessDate(bankName, processDate);
        }

        public int Save(EkstreAktarma item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_EKSTREAKTARMA);
            return item.Id;
        }

        public bool Update(EkstreAktarma item)
        {
            if (item == null) throw new ArgumentNullException("item");
            EkstreAktarma previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_EKSTREAKTARMA);
            return updated;
        }

        public bool Delete(EkstreAktarma item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (item.Id == 0) return false;
            EkstreAktarma previous = GetById(item.Id);
            if (previous == null) return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(previous, ProjeConstants.NBYS, ProjeConstants.NBYS_EKSTREAKTARMA);
            return deleted;
        }

        private static List<int> ParseIds(string idList)
        {
            List<int> ids = new List<int>();
            if (string.IsNullOrWhiteSpace(idList)) return ids;
            string value = idList.Trim().Trim('(', ')');
            string[] parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                int id;
                if (!int.TryParse(parts[i].Trim(), out id))
                    throw new ArgumentException("ID listesi yalnızca geçerli tam sayılar içermelidir.", "idList");
                ids.Add(id);
            }
            return ids;
        }

        private static List<EkstreAktarma> Map(DataTable table)
        {
            return new EkstreAktarma().ToList<EkstreAktarma>(table);
        }
    }
}
