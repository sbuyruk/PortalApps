using DAO.Repositories.TBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class OdemePlaniService
    {
        private readonly OdemePlaniRepository repository;
        public OdemePlaniService() : this(new OdemePlaniRepository()) { }
        public OdemePlaniService(OdemePlaniRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public OdemePlani GetById(int id) { return Map(repository.SelectById(id)); }
        public List<OdemePlani> GetAll() { return ToList(repository.SelectAll()); }
        public List<OdemePlani> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public OdemePlani GetBySozlesmeIdSira(int id, int sira) { return Map(repository.SelectBySozlesmeIdSira(id, sira)); }
        public OdemePlani GetBySozlesmeIdOdemeTarihi(int id, DateTime tarih) { return Map(repository.SelectBySozlesmeIdOdemeTarihi(id, tarih)); }
        public bool Exists(int id) { return ToList(repository.SelectExists(id)).Count > 0; }
        public OdemePlani GetLastBySozlesmeId(int id) { return ToList(repository.SelectLastBySozlesmeId(id)).LastOrDefault(); }
        public OdemePlani GetFirstBySozlesmeId(int id) { return Map(repository.SelectFirstBySozlesmeId(id)); }
        public int Save(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return item.Id; }
        public bool Update(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); OdemePlani old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        public bool Delete(OdemePlani item) { if (item == null || item.Id == 0) return false; OdemePlani old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        public bool DeleteBySozlesmeId(OdemePlani item, int id) { bool ok = repository.DeleteBySozlesmeId(id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
        public bool CreatePaymentPlan(KiraSozlesme kiraSozlesme)
        {
            IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int taksitSayisi = kiraSozlesme.TaksitSayisi;
            if (taksitSayisi < 1) taksitSayisi = 1;
            else if (taksitSayisi > 12) taksitSayisi = 12;
            DateTime sozlesmeBaslangic = kiraSozlesme.SozBasTar;
            DateTime sozlesmeBitis = sozlesmeBaslangic.AddMonths(taksitSayisi);
            string bitisAyi = sozlesmeBitis.ToString("MMMM", cultureInfo);
            DateTime vadeBaslangic = sozlesmeBaslangic;
            DateTime vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
            try
            {
                decimal aylikKira = kiraSozlesme.KiraBedeli;
                if (kiraSozlesme.OdemeSekli.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK)) aylikKira = kiraSozlesme.KiraBedeli / taksitSayisi;
                SaveCarryOver(kiraSozlesme, kiraSozlesme.DevirAnaPara, kiraSozlesme.DevirFaizTutari, "Önceki Sözlesmeden devir", 0);
                for (int i = 1; i <= taksitSayisi; i++)
                {
                    int yil = vadeBaslangic.Year; string ay = vadeBaslangic.ToString("MMMM", cultureInfo);
                    if ((ay + "/" + yil).Equals(bitisAyi + "/" + sozlesmeBitis.Year)) break;
                    SaveNew(kiraSozlesme, yil, ay, vadeBaslangic, vadeBitis, sozlesmeBaslangic, sozlesmeBitis, aylikKira, i);
                    vadeBaslangic = vadeBaslangic.AddMonths(1); vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
                }
                for (int i = taksitSayisi + 1; i <= 12; i++)
                {
                    int yil = vadeBaslangic.Year; string ay = vadeBaslangic.ToString("MMMM", cultureInfo);
                    SaveNew(kiraSozlesme, yil, ay, vadeBaslangic, vadeBitis, sozlesmeBaslangic, sozlesmeBitis, 0, i);
                    vadeBaslangic = vadeBaslangic.AddMonths(1); vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
                }
                return true;
            }
            catch (Exception) { return false; }
        }
        private void SaveNew(KiraSozlesme kiraSozlesme, int yil, string ay, DateTime vadeBaslangic, DateTime vadeBitis, DateTime odemeBaslangic, DateTime odemeBitis, decimal aylikKira, int sira)
        {
            Save(new OdemePlani { SozlesmeId = kiraSozlesme.Id, Yil = yil, Ay = ay, VadeBasTar = vadeBaslangic, VadeBitTar = vadeBitis, KiraBedeli = aylikKira, AnaPara = 0, FaizTutari = 0, OdenenTutar = 0, OdemeBasTar = odemeBaslangic, OdemeBitTar = odemeBitis, Sira = sira, Aciklama = "" });
        }
        private void SaveCarryOver(KiraSozlesme kiraSozlesme, decimal anaPara, decimal faizTutari, string aciklama, int sira)
        {
            Save(new OdemePlani { SozlesmeId = kiraSozlesme.Id, VadeBitTar = kiraSozlesme.SozBasTar.AddDays(-1), AnaPara = anaPara, FaizTutari = faizTutari, FaizliBakiye = anaPara + faizTutari, OdenenTutar = 0, Aciklama = aciklama, Sira = sira });
        }
        private static List<OdemePlani> ToList(DataTable t) { return new OdemePlani().ToList<OdemePlani>(t); }
        private static OdemePlani Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
