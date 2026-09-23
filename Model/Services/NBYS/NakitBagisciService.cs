using System;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class NakitBagisciService
    {
        public bool Save(Model.NBYS.NakitBagisci nakitBagisci)
        {
            if (nakitBagisci == null)
            {
                throw new ArgumentNullException("nakitBagisci");
            }

            nakitBagisci.Id = nakitBagisci.Save();
            return nakitBagisci.Id > 0;
        }

        public bool Update(Model.NBYS.NakitBagisci nakitBagisci, bool oncekiTuzelKisi, string currentUser)
        {
            if (nakitBagisci == null)
            {
                throw new ArgumentNullException("nakitBagisci");
            }

            bool isSaved = nakitBagisci.Update();
            if (oncekiTuzelKisi != nakitBagisci.TuzelKisi)
            {
                Model.NBYS.Armagan armagan = new Model.NBYS.Armagan();
                var armaganList = armagan.SelectByBagisciIdAndDurum(
                    nakitBagisci.Id,
                    ProjeConstants.DURUM_GONDERILMEDI);

                foreach (Model.NBYS.Armagan item in armaganList)
                {
                    Model.NBYS.EkstreAktarma.SaveArmagan(
                        item.Tarih,
                        nakitBagisci.TuzelKisi,
                        item.BagisciId,
                        0,
                        currentUser);
                }
            }

            return isSaved;
        }
    }
}
