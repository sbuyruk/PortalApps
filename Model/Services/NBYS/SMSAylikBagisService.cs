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
    public class SMSAylikBagisService
    {
        private readonly SMSAylikBagisRepository repository;

        public SMSAylikBagisService() : this(new SMSAylikBagisRepository())
        {
        }

        public SMSAylikBagisService(SMSAylikBagisRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SMSAylikBagis GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<SMSAylikBagis> GetByYear(int year)
        {
            return Map(repository.SelectByYear(year));
        }

        public int Save(SMSAylikBagis item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
            return item.Id;
        }

        public bool Update(SMSAylikBagis item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            SMSAylikBagis previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
            return updated;
        }

        public bool Delete(SMSAylikBagis item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;
            SMSAylikBagis previous = GetById(item.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(previous, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
            return deleted;
        }

        private static List<SMSAylikBagis> Map(DataTable table)
        {
            return new SMSAylikBagis().ToList<SMSAylikBagis>(table);
        }
    }
}
