using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class Armagan : ParentClass
    {
        public int BagisciId { get; set; }
        public int ArmaganTanimId { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public string BelgedeYazanIsim { get; set; }
        public int BelgeGecersizMi { get; set; }
        public int GecersizNBHareketId { get; set; }
        public string GecersizYapan { get; set; }
        public DateTime GecersizYapmaTarihi { get; set; }
        public decimal ArmaganBagisMiktari { get; set; }
        public decimal IadeMiktari { get; set; }
        public bool BagisMiktariYazmasin { get; set; }
        public bool CokluBagis { get; set; }
        public bool DuzenliBagis { get; set; }= false;
        public int KacinciBelge { get; set; }= 0;
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new ArmaganService().GetById(id), typeof(T));
        }

        public Armagan SelectByBagisciIdAndBagisTarihi(DateTime basTar, DateTime bitTar, int nakitBagisciId)
        {
            return new ArmaganService().GetByBagisciIdDateRange(nakitBagisciId, basTar, bitTar);
        }
        public override int Save()
        {
            return new ArmaganService().Save(this);
        }
        public override bool Update()
        {
            return new ArmaganService().Update(this);
        }
        public override bool Delete()
        {
            return new ArmaganService().Delete(this);
        }
        
        public Armagan SelectByBagisciIdBagisTarihi(int nakitBagisciId, int armaganId, DateTime basTar, DateTime bitTar)
        {
            return new ArmaganService().GetByBagisciIdTanimIdDateRange(
                nakitBagisciId, armaganId, basTar, bitTar);
        }
        public List<Armagan> SelectByBagisciId(int nakitBagisciId)
        {
            return new ArmaganService().GetByBagisciId(nakitBagisciId);
        }
        private string DurumGetir(int nakitBagisciId)
        {
            string durum = string.Empty;
            NakitBagisci nakitBagisci = new NakitBagisciService().GetById(nakitBagisciId);
            if (nakitBagisci != null)
            {
                if (nakitBagisci.Ulasilamiyor)
                {
                    durum = ProjeConstants.DURUM_ULASILAMADI;
                }
                if (nakitBagisci.BelgeIstemiyor)
                {
                    durum = ProjeConstants.DURUM_BELGE_ISTEMIYOR;
                }
            }
            return durum;
        }
        //public bool SaveOrUpdate(DateTime bagisTarihi, int nakitBagisciId, int nakikbagisHareketId)
        //{
        //    bool isSaved = false;

        //    Armagan armagan = SelectByBagisciIdAndBagisTarihi(bagisTarihi, nakitBagisciId);
        //    int armaganId = 0;
        //    try
        //    {
        //        string durum = DurumGetir(nakitBagisciId);
        //        if (armagan != null)
        //        {
        //            if (!string.IsNullOrEmpty(durum))
        //            {
        //                Durum = durum;
        //            }
        //            Update(); //TODO burasi armagan.Update() olmali veya üstsatirda Durum=durum; olmali
        //            armaganId = armagan.Id;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(durum))
        //            {
        //                Durum = durum;
        //            }
        //            armaganId = Save();
        //        }
        //        isSaved = true;

        //        UpdateArmaganIdAtNakitBagisHareket(nakitBagisciId,bagisTarihi, nakikbagisHareketId, armaganId);

        //    }
        //    catch (Exception)
        //    {
        //        isSaved = false;
        //    }
        //    return isSaved;
        //}
        public int SaveOrUpdate(DateTime bastar, DateTime bittar, DateTime bagisTarihi, int nakitBagisciId, List<NakitBagisHareket> nakitBagisHareketListesi)
        {
            Armagan armagan = SelectByBagisciIdAndBagisTarihi(bastar, bittar, nakitBagisciId);
            int armaganId = 0;
            try
            {
                string durum = DurumGetir(nakitBagisciId);
                if (armagan != null)
                {
                    if (!string.IsNullOrEmpty(durum))
                    {
                        Durum = durum;
                    }
                    Update(); //TODO burasi armagan.Update() olmali veya üstsatirda Durum=durum; olmali
                    armaganId = armagan.Id;
                }
                else
                {
                    if (!string.IsNullOrEmpty(durum))
                    {
                        Durum = durum;
                    }
                    armaganId = Save();
                }

                UpdateArmaganIdAtNakitBagisHareket(nakitBagisciId, bastar, bittar, nakitBagisHareketListesi, armaganId);

            }
            catch (Exception)
            {
                armaganId = 0;
            }
            return armaganId;
        }
        private void UpdateArmaganIdAtNakitBagisHareket(int nakitBagisciId, DateTime bastar,DateTime bittar, List<NakitBagisHareket> nakitBagisHareketListesi, int armaganId)
        {
            //if (nakikbagisHareketId != 0)
            //{
            //    NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            //    nakitBagisHareket = nakitBagisHareket.Select<NakitBagisHareket>(nakikbagisHareketId);
            //    if (nakitBagisHareket != null)
            //    {
            //        nakitBagisHareket.ArmaganId = armaganId;
            //        nakitBagisHareket.Update();
            //    }
            //}
            if (nakitBagisHareketListesi.Count> 0)
            {
                NakitBagisHareketService nakitBagisHareketService = new NakitBagisHareketService();
                foreach (NakitBagisHareket item in nakitBagisHareketListesi)
                {
                    item.ArmaganId = armaganId;
                    try
                    {
                        nakitBagisHareketService.Update(item);
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper ex = new ExceptionHelper(e);
                        ex.PublishException();
                    }
                }
            }
        }
        public bool UpdateDurumByBolge(string fromdurum, string todurum, string bastar, string bittar, int armaganTanimId, int bolgeId)
        {
            return new ArmaganService().UpdateDurumByBolge(
                fromdurum, todurum, bastar, bittar, armaganTanimId, bolgeId);
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new ArmaganService().GetAll(), typeof(List<T>));
        }
        public List<Armagan> SelectByBagisciIdAndDurum(int nakitBagisciId, string durum)
        {
            return new ArmaganService().GetByBagisciIdAndDurum(nakitBagisciId, durum);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="durum"></param>
        /// <param name="bastar"></param>
        /// <param name="bittar"></param>
        /// <param name="armaganTanimId"></param>
        /// <param name="rowCount"></param>
        /// Parasi iade edilen armaganlari da göstersin diye BelgeGecersizMi kontrolu burada yok
        /// <returns></returns>
        public string SelectByDurumTarih(string durum, DateTime bastar, DateTime bittar, string armaganTanimId, ref int rowCount, int bolgeId, int ili)
        {
            return new ArmaganService().ListByDurumTarihJson(
                durum, bastar, bittar, armaganTanimId, ref rowCount, bolgeId, ili);
        }
        public DataTable SelectByDurumTarihReturnDT(string durum, DateTime bastar, DateTime bittar, string armaganTanimId, int bolgeId, int ili)
        {
            return new ArmaganService().ListByDurumTarih(
                durum, bastar, bittar, armaganTanimId, bolgeId, ili);
        }
        public DataTable SelectCountDurumByBolgeBasTarBitTar(DateTime basTar, DateTime bitTar, int armaganTanimId, int bolgeId)
        {
            return new ArmaganService().CountDurumByBolge(basTar, bitTar, armaganTanimId, bolgeId);
        }
        public DataTable SelectCountDurumByBolgeTarih(int armaganTanimId, int bolgeId, DateTime bastar, DateTime bittar)
        {
            return new ArmaganService().CountDurumByBolge(bastar, bittar, armaganTanimId, bolgeId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basTar"></param>
        /// <param name="bitTar"></param>
        /// <returns></returns>
        public DataTable SelectCountByBagisTarihiBolge(DateTime basTar, DateTime bitTar)
        {
            return new ArmaganService().CountByBagisTarihiBolge(basTar, bitTar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="eksiId"></param>
        /// Parasi iade edilen armaganlari da göstermesin
        /// <returns></returns>
        public string SelectByFilter(string filter, int eksiId)
        {
            return new ArmaganService().SearchJson(filter, eksiId);
        }

        public DataTable SelectVerilenArmaganlarGroupByBagisciReturnList()
        {
            return new ArmaganService().ListVerilenArmaganlarGroupByBagisci();

        }

        public Armagan SelectByBagisciIdAndArmaganTanimId(int nakitBagisciId, int armaganTanimId)
        {
            return new ArmaganService().GetByBagisciIdAndTanimId(nakitBagisciId, armaganTanimId);
        }
        public int SelectCountByBagisciIdAndArmaganTanimId(int nakitBagisciId, int armaganTanimId)
        {
            return new ArmaganService().CountByBagisciIdAndTanimId(nakitBagisciId, armaganTanimId);
        }
    }
}
