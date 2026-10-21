using Model.Ortak;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.TBYS
{
    /// <summary>
    /// Kira sözleşmelerinde bakiye (devir) işlemini otomatik yapan modül.
    /// Aktif sözleşmelerden, önceki sözleşmesinin son ödeme planı satırı ile
    /// kendi devir alanları arasında fark bulunanları işler.
    /// Bu sınıf UI bağımlılığı içermez; ekrandan veya zamanlanmış bir işten çağrılabilir.
    /// </summary>
    public class KiraBakiyeDevriOtomatik
    {
        private readonly PortalLogContext context;

        public KiraBakiyeDevriOtomatik() { }

        public KiraBakiyeDevriOtomatik(PortalLogContext context)
        {
            this.context = context;
        }

        private Model.Services.TBYS.KiraSozlesmeService KiraSozlesmeService
        {
            get { return context == null ? new Model.Services.TBYS.KiraSozlesmeService() : new Model.Services.TBYS.KiraSozlesmeService(context); }
        }

        private Model.Services.TBYS.OdemePlaniService OdemePlaniService
        {
            get { return context == null ? new Model.Services.TBYS.OdemePlaniService() : new Model.Services.TBYS.OdemePlaniService(context); }
        }
        /// <summary>
        /// Otomatik devir işlemi için kullanılacak işlem yapan adı.
        /// </summary>
        public const string ISLEM_YAPAN = "TBYS";

        /// <summary>
        /// Tüm aktif sözleşmeleri tarar, önceki sözleşmesiyle arasında fark olanlarda
        /// bakiye devrini otomatik uygular.
        /// </summary>
        /// <returns>İşlemin özetini ve işlenen/atlanan/hatalı sözleşme bilgilerini döner.</returns>
        public KiraBakiyeDevriSonuc TumAktifSozlesmeleriIsle()
        {
            KiraBakiyeDevriSonuc sonuc = new KiraBakiyeDevriSonuc();

            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> aktifSozlesmeler = KiraSozlesmeService.GetAllActive();

            foreach (KiraSozlesme kiraSozlesme in aktifSozlesmeler)
            {
                if (kiraSozlesme == null)
                {
                    continue;
                }

                try
                {
                    bool islendi = DevirAl(kiraSozlesme);
                    if (islendi)
                    {
                        sonuc.IslenenSozlesmeIdleri.Add(kiraSozlesme.Id);
                    }
                    else
                    {
                        sonuc.AtlananSozlesmeIdleri.Add(kiraSozlesme.Id);
                    }
                }
                catch (Exception ex)
                {
                    sonuc.HataliSozlesmeler.Add(kiraSozlesme.Id, ex.Message);
                }
            }

            return sonuc;
        }

        /// <summary>
        /// Tek bir sözleşme için, önceki sözleşmesiyle fark varsa bakiye devrini uygular.
        /// <paramref name="atlaBayrakKontrolu"/> false (varsayılan) iken, sözleşmenin
        /// "Otomatik Bakiye Devir Al" alanı işaretli değilse işlem yapılmaz.
        /// UI butonlarından manuel çağrılırken true geçilir; bu durumda bayrak denetlenmez.
        /// </summary>
        /// <param name="kiraSozlesme">İşlenecek kira sözleşmesi.</param>
        /// <param name="atlaBayrakKontrolu">true ise OtomatikBakiyeDevirAl bayrağı denetlenmez.</param>
        /// <returns>Devir uygulandıysa true, atlandıysa false.</returns>
        public bool DevirAl(KiraSozlesme kiraSozlesme, bool atlaBayrakKontrolu = false)
        {
            if (context != null)
            {
                if (kiraSozlesme == null)
                {
                    return false;
                }

                decimal eskiDevirAnaPara = kiraSozlesme.DevirAnaPara;
                decimal eskiDevirFaizTutari = kiraSozlesme.DevirFaizTutari;
                decimal eskiDevirFaizliBakiye = kiraSozlesme.DevirFaizliBakiye;
                bool result;
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    result = DevirAl(kiraSozlesme, atlaBayrakKontrolu, transaction);
                    if (result) transaction.Complete();
                }

                if (result)
                {
                    OdemeleriHesaplaOdemePlaniniGuncelle(kiraSozlesme.KiraciId);
                    OtomatikDevirOlayKaydet(kiraSozlesme,
                        eskiDevirAnaPara, eskiDevirFaizTutari, eskiDevirFaizliBakiye);
                }

                return result;
            }
            return DevirAlLegacy(kiraSozlesme, atlaBayrakKontrolu);
        }

        private bool DevirAl(KiraSozlesme kiraSozlesme, bool atlaBayrakKontrolu, SqlTransactionContext transaction)
        {
            if (kiraSozlesme == null)
            {
                return false;
            }

            if (atlaBayrakKontrolu)
            {
                return false;
            }

            KiraSozlesme oncekiKiraSozlesmesi = KiraSozlesmeService.GetPreviousByTenant(kiraSozlesme.KiraciId, kiraSozlesme.SozBasTar, transaction);
            if (oncekiKiraSozlesmesi == null)
            {
                return false;
            }

            OdemePlani odemePlaniDao = new OdemePlani();

            List<OdemePlani> oncekiOdemePlaniList = OdemePlaniService.GetBySozlesmeId(oncekiKiraSozlesmesi.Id, transaction);
            if (oncekiOdemePlaniList == null || oncekiOdemePlaniList.Count == 0)
            {
                return false;
            }

            OdemePlani oncekiOdemePlani = oncekiOdemePlaniList[oncekiOdemePlaniList.Count - 1];

            decimal yeniDevirAnaPara = oncekiOdemePlani.AnaPara;
            decimal yeniDevirFaizliBakiye = oncekiOdemePlani.FaizliBakiye;
            decimal yeniDevirFaizTutari = yeniDevirFaizliBakiye - yeniDevirAnaPara;

            bool farkVarMi = (yeniDevirAnaPara != kiraSozlesme.DevirAnaPara)
                          || (yeniDevirFaizliBakiye != kiraSozlesme.DevirFaizliBakiye);
            if (!farkVarMi)
            {
                return false;
            }

            kiraSozlesme.DevirAnaPara = yeniDevirAnaPara;
            kiraSozlesme.DevirFaizliBakiye = yeniDevirFaizliBakiye;
            kiraSozlesme.DevirFaizTutari = yeniDevirFaizTutari;

            bool devirAlanlariGuncellendi = KiraSozlesmeService.Update(kiraSozlesme, transaction);
            if (!devirAlanlariGuncellendi)
            {
                return false;
            }

            List<OdemePlani> odemePlaniList = OdemePlaniService.GetBySozlesmeId(kiraSozlesme.Id, transaction);
            if (odemePlaniList == null || odemePlaniList.Count == 0)
            {
                return false;
            }

            OdemePlani odemePlani = odemePlaniList[0]; //devir satırı (0. sıra)
            if (odemePlani == null)
            {
                return false;
            }

            bool odemePlaniDevirSatiriDegisti = (odemePlani.AnaPara != kiraSozlesme.DevirAnaPara)
                || (odemePlani.FaizTutari != kiraSozlesme.DevirFaizTutari)
                || (odemePlani.FaizliBakiye != kiraSozlesme.DevirFaizliBakiye);
            if (!odemePlaniDevirSatiriDegisti)
            {
                return false;
            }

            odemePlani.AnaPara = kiraSozlesme.DevirAnaPara;
            odemePlani.FaizTutari = kiraSozlesme.DevirFaizTutari;
            odemePlani.FaizliBakiye = kiraSozlesme.DevirFaizliBakiye;

            if (!OdemePlaniService.Update(odemePlani, transaction))
            {
                return false;
            }

            return true;
        }

        private bool DevirAlLegacy(KiraSozlesme kiraSozlesme, bool atlaBayrakKontrolu)
        {
            if (kiraSozlesme == null) return false;
            if (atlaBayrakKontrolu) return false;
            KiraSozlesme oncekiKiraSozlesmesi = KiraSozlesmeService.GetPreviousByTenant(kiraSozlesme.KiraciId, kiraSozlesme.SozBasTar);
            if (oncekiKiraSozlesmesi == null) return false;
            List<OdemePlani> oncekiOdemePlaniList = OdemePlaniService.GetBySozlesmeId(oncekiKiraSozlesmesi.Id);
            if (oncekiOdemePlaniList == null || oncekiOdemePlaniList.Count == 0) return false;
            OdemePlani oncekiOdemePlani = oncekiOdemePlaniList[oncekiOdemePlaniList.Count - 1];
            decimal yeniDevirAnaPara = oncekiOdemePlani.AnaPara;
            decimal yeniDevirFaizliBakiye = oncekiOdemePlani.FaizliBakiye;
            decimal yeniDevirFaizTutari = yeniDevirFaizliBakiye - yeniDevirAnaPara;
            if (yeniDevirAnaPara == kiraSozlesme.DevirAnaPara && yeniDevirFaizliBakiye == kiraSozlesme.DevirFaizliBakiye) return false;
            decimal eskiDevirAnaPara = kiraSozlesme.DevirAnaPara;
            decimal eskiDevirFaizTutari = kiraSozlesme.DevirFaizTutari;
            decimal eskiDevirFaizliBakiye = kiraSozlesme.DevirFaizliBakiye;
            kiraSozlesme.DevirAnaPara = yeniDevirAnaPara;
            kiraSozlesme.DevirFaizliBakiye = yeniDevirFaizliBakiye;
            kiraSozlesme.DevirFaizTutari = yeniDevirFaizTutari;
            if (!KiraSozlesmeService.Update(kiraSozlesme)) return false;
            List<OdemePlani> odemePlaniList = OdemePlaniService.GetBySozlesmeId(kiraSozlesme.Id);
            if (odemePlaniList == null || odemePlaniList.Count == 0) return false;
            OdemePlani odemePlani = odemePlaniList[0];
            if (odemePlani == null) return false;
            if (odemePlani.AnaPara == kiraSozlesme.DevirAnaPara && odemePlani.FaizTutari == kiraSozlesme.DevirFaizTutari && odemePlani.FaizliBakiye == kiraSozlesme.DevirFaizliBakiye) return false;
            odemePlani.AnaPara = kiraSozlesme.DevirAnaPara;
            odemePlani.FaizTutari = kiraSozlesme.DevirFaizTutari;
            odemePlani.FaizliBakiye = kiraSozlesme.DevirFaizliBakiye;
            if (!OdemePlaniService.Update(odemePlani)) return false;
            OdemeleriHesaplaOdemePlaniniGuncelle(kiraSozlesme.KiraciId);
            OtomatikDevirOlayKaydet(kiraSozlesme, eskiDevirAnaPara, eskiDevirFaizTutari, eskiDevirFaizliBakiye);
            return true;
        }

        private void OdemeleriHesaplaOdemePlaniniGuncelle(int kiraciId)
        {
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            List<KiraSozlesme> sozlesmeListesi = KiraSozlesmeService.GetAllByKiraciId(kiraciId);
            sozlesmeListesi = sozlesmeListesi.OrderBy(x => x.SozBasTar).ToList();
            foreach (KiraSozlesme kiraSozlesme in sozlesmeListesi)
            {
                TBYSOrtak.BakiyeBorcHesapla(kiraSozlesme);
            }
        }

        private void OtomatikDevirOlayKaydet(KiraSozlesme kiraSozlesme,
            decimal eskiDevirAnaPara, decimal eskiDevirFaizTutari, decimal eskiDevirFaizliBakiye)
        {
            try
            {
                Olay olay = new Olay();
                olay.Program = ProjeConstants.TBYS;
                olay.IslemKonusu = ProjeConstants.TBYS_KIRASOZLESME;
                olay.IslemTipi = "Otomatik Devir";
                olay.IslemTarihi = DateTime.Now;
                olay.IslemYapan = ISLEM_YAPAN;
                olay.Olusturan = ISLEM_YAPAN;
                olay.OlusturmaTarihi = DateTime.Now;
                olay.Aciklama =
                    "KiraSozlesmeId=" + kiraSozlesme.Id + ProjeConstants.DELIMITER +
                    "DevirAnaPara=" + eskiDevirAnaPara + "#" + kiraSozlesme.DevirAnaPara + ProjeConstants.DELIMITER +
                    "DevirFaizTutari=" + eskiDevirFaizTutari + "#" + kiraSozlesme.DevirFaizTutari + ProjeConstants.DELIMITER +
                    "DevirFaizliBakiye=" + eskiDevirFaizliBakiye + "#" + kiraSozlesme.DevirFaizliBakiye + ProjeConstants.DELIMITER;
                new Model.Services.Ortak.OlayService().Save(olay);
            }
            catch
            {
                // Loglama hatası devir işlemini bozmamalı.
            }
        }
    }

    /// <summary>
    /// Otomatik bakiye devri işleminin sonucunu taşıyan sınıf.
    /// </summary>
    public class KiraBakiyeDevriSonuc
    {
        public KiraBakiyeDevriSonuc()
        {
            IslenenSozlesmeIdleri = new List<int>();
            AtlananSozlesmeIdleri = new List<int>();
            HataliSozlesmeler = new Dictionary<int, string>();
        }

        /// <summary>Bakiye devri uygulanan sözleşmeler.</summary>
        public List<int> IslenenSozlesmeIdleri { get; set; }

        /// <summary>Fark olmadığı için işlem yapılmayan sözleşmeler.</summary>
        public List<int> AtlananSozlesmeIdleri { get; set; }

        /// <summary>İşlem sırasında hata oluşan sözleşmeler (Id ve hata mesajı).</summary>
        public Dictionary<int, string> HataliSozlesmeler { get; set; }

        public int IslenenSayisi => IslenenSozlesmeIdleri.Count;
        public int AtlananSayisi => AtlananSozlesmeIdleri.Count;
        public int HataliSayisi => HataliSozlesmeler.Count;
    }
}
