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
    public class FTKKisiService
    {
        private readonly FTKKisiRepository repository;
        public FTKKisiService() : this(new FTKKisiRepository()) { }
        public FTKKisiService(FTKKisiRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public FTKKisi GetById(int id) { return Map(repository.SelectById(id)).FirstOrDefault(); }
        public DataTable GetAllTable() { return repository.SelectAllWithLocation(); }

        public FTKKisi GetByName(string name, string surname)
        {
            name = string.IsNullOrEmpty(name) ? "#${}?" : name.Trim();
            surname = string.IsNullOrEmpty(surname) ? "#${}?" : surname.Trim();
            return Map(repository.SelectByName(name, surname)).FirstOrDefault();
        }

        public FTKKisi GetByIdentityNumber(long identityNumber)
        {
            return Map(repository.SelectByIdentityNumber(identityNumber)).FirstOrDefault();
        }

        public DataTable GetMembersTable(int provinceId, int districtId, int operationId, bool onlyActive)
        {
            int repositoryOperationId = operationId == ProjeConstants.HEPSI_INT ? -1 : operationId;
            return repository.SelectMembers(provinceId, districtId, repositoryOperationId,
                onlyActive, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF);
        }

        public List<FTKKisi> GetMembers(int provinceId, int districtId, int operationId, bool onlyActive)
        {
            return Map(GetMembersTable(provinceId, districtId, operationId, onlyActive));
        }

        public FTKKisi GetGovernor(int provinceId)
        {
            return Map(repository.SelectGovernor(provinceId, ProjeConstants.VALILIK_INT)).FirstOrDefault();
        }

        public FTKKisi GetDistrictGovernor(int provinceId, int districtId)
        {
            return Map(repository.SelectDistrictGovernor(provinceId, districtId)).FirstOrDefault();
        }

        public bool DeactivateByIdList(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return false;
            string value = ids.Trim();
            if (value.StartsWith("(") && value.EndsWith(")"))
                value = value.Substring(1, value.Length - 2);
            if (string.IsNullOrWhiteSpace(value)) return false;
            string[] parts = value.Split(',');
            List<int> parsedIds = new List<int>();
            for (int i = 0; i < parts.Length; i++)
            {
                int id;
                if (!int.TryParse(parts[i].Trim(), out id))
                    throw new ArgumentException("ID listesi yalnızca geçerli tam sayılar içermelidir.", "ids");
                parsedIds.Add(id);
            }
            return repository.UpdateMembershipStatus(parsedIds, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL);
        }

        public int Save(FTKKisi item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
            return item.Id;
        }

        public bool Update(FTKKisi item)
        {
            if (item == null) throw new ArgumentNullException("item");
            FTKKisi previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
            return updated;
        }

        public bool Delete(FTKKisi item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (item.Id == 0) return false;
            FTKKisi previous = GetById(item.Id);
            if (previous == null) return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(previous, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
            return deleted;
        }

        private static List<FTKKisi> Map(DataTable table)
        {
            return new FTKKisi().ToList<FTKKisi>(table);
        }
    }
}
