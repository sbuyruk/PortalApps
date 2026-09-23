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
    public class NakitBagisciService
    {
        private readonly NakitBagisciRepository repository;

        public NakitBagisciService()
            : this(new NakitBagisciRepository())
        {
        }

        public NakitBagisciService(NakitBagisciRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public int Save(NakitBagisci nakitBagisci)
        {
            if (nakitBagisci == null)
                throw new ArgumentNullException("nakitBagisci");

            nakitBagisci.OlusturmaTarihi = DateTime.Now;
            nakitBagisci.Olusturan = UtilityHelper.GetCurrentUserName();
            nakitBagisci.Id = repository.Insert(nakitBagisci);

            if (nakitBagisci.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(
                    nakitBagisci,
                    ProjeConstants.NBYS,
                    ProjeConstants.NBYS_NAKITBAGISCI);
            }

            return nakitBagisci.Id;
        }

        public NakitBagisci GetById(int id)
        {
            DataTable dataTable = repository.SelectById(id);
            NakitBagisci mapper = new NakitBagisci();
            List<NakitBagisci> list = mapper.ToList<NakitBagisci>(dataTable);
            return list.FirstOrDefault();
        }

        public bool Update(NakitBagisci nakitBagisci)
        {
            if (nakitBagisci == null)
                throw new ArgumentNullException("nakitBagisci");

            NakitBagisci eskiNakitBagisci = GetById(nakitBagisci.Id);
            bool isSaved = false;
            if (nakitBagisci.Id != 0)
            {
                nakitBagisci.DegistirmeTarihi = DateTime.Now;
                nakitBagisci.Degistiren = UtilityHelper.GetCurrentUserName();
                isSaved = repository.Update(nakitBagisci);
            }

            if (isSaved && ProjeConstants.NBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(
                    nakitBagisci,
                    eskiNakitBagisci,
                    ProjeConstants.NBYS,
                    ProjeConstants.NBYS_NAKITBAGISCI);
            }

            return isSaved;
        }

        public bool Update(NakitBagisci nakitBagisci, bool oncekiTuzelKisi, string currentUser)
        {
            bool isSaved = Update(nakitBagisci);
            if (oncekiTuzelKisi != nakitBagisci.TuzelKisi)
            {
                Armagan armagan = new Armagan();
                var armaganList = armagan.SelectByBagisciIdAndDurum(
                    nakitBagisci.Id,
                    ProjeConstants.DURUM_GONDERILMEDI);

                foreach (Armagan item in armaganList)
                {
                    EkstreAktarma.SaveArmagan(
                        item.Tarih,
                        nakitBagisci.TuzelKisi,
                        item.BagisciId,
                        0,
                        currentUser);
                }
            }

            return isSaved;
        }

        public bool Delete(NakitBagisci nakitBagisci)
        {
            if (nakitBagisci == null)
                throw new ArgumentNullException("nakitBagisci");

            if (nakitBagisci.Id == 0)
                return false;

            NakitBagisci silinecekNakitBagisci = GetById(nakitBagisci.Id);
            if (silinecekNakitBagisci == null)
                return false;

            bool isDeleted = repository.Delete(nakitBagisci.Id);
            if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(
                    silinecekNakitBagisci,
                    ProjeConstants.NBYS,
                    ProjeConstants.NBYS_NAKITBAGISCI);
            }

            return isDeleted;
        }
    }
}
