using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class KisiService
    {
        private readonly KisiRepository repository;

        public KisiService()
            : this(new KisiRepository())
        {
        }

        public KisiService(KisiRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public Kisi GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Kisi> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetAllData()
        {
            return repository.SelectAllData();
        }

        public List<Kisi> GetBirthdayCelebrations()
        {
            return ToList(repository.SelectBirthdayCelebrations());
        }

        public string GetAllJson()
        {
            return new Kisi().ToJSON(repository.SelectAllJsonData());
        }

        public DataTable GetUnselectedParticipants(int faaliyetId)
        {
            return repository.SelectUnselectedParticipants(faaliyetId);
        }

        public Kisi GetByName(string adi, string soyadi)
        {
            return Map(repository.SelectByName(adi, soyadi));
        }

        public Kisi GetByTcKimlikNo(string tcKimlikNo)
        {
            return Map(repository.SelectByTcKimlikNo(tcKimlikNo));
        }

        public int Save(Kisi item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KISI);

            return item.Id;
        }

        public bool Update(Kisi item)
        {
            if (item == null || item.Id == 0)
                return false;

            Kisi old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = repository.Update(item);

            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_KISI);

            return updated;
        }

        public bool Delete(Kisi item)
        {
            if (item == null || item.Id == 0)
                return false;

            Kisi old = GetById(item.Id);
            if (old == null)
                return false;

            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_KISI);

            return deleted;
        }

        private static List<Kisi> ToList(DataTable table)
        {
            return new Kisi().ToList<Kisi>(table);
        }

        private static Kisi Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
