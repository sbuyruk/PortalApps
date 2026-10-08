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
    public class OdemeService
    {
        private readonly OdemeRepository repository;
        public OdemeService() : this(new OdemeRepository()) { }
        public OdemeService(OdemeRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Odeme GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Odeme> GetAll() { return ToList(repository.SelectAll()); }
        public List<Odeme> GetByKiraciId(int id) { return ToList(repository.SelectByKiraciId(id)); }
        public List<Odeme> GetBySozlesmeIdOdemePlaniId(int sozlesmeId, int planId) { return ToList(repository.SelectBySozlesmeIdOdemePlaniId(sozlesmeId, planId)); }
        public DataTable GetByKiraciAyYil(int kiraciId, int ay, int yil) { return repository.SelectByKiraciAyYil(kiraciId, ay, yil); }
        public DataTable GetByKiraciAyYilReturnDataTable(int bolgeId, int kiraciId, DateTime basTarih, DateTime bitTarih) { return repository.SelectByKiraciAyYilReturnDataTable(bolgeId, kiraciId, basTarih, UtilityHelper.TariheSaatEkle(bitTarih, "23:59:59")); }
        public DataTable GetByAyYilReturnDataTable(int ay, int yil) { return repository.SelectByAyYilReturnDataTable(ay, yil); }
        public List<Odeme> GetByKiraciVadeBasTarVadeBitTar(int sozlesmeId, int kiraciId, DateTime ilkTarih, DateTime ikinciTarih)
        {
            DateTime basTarih = UtilityHelper.TariheSaatEkle(new DateTime(ilkTarih.Year, ilkTarih.Month, ilkTarih.Day), "00:00");
            DateTime bitTarih = UtilityHelper.TariheSaatEkle(new DateTime(ikinciTarih.Year, ikinciTarih.Month, ikinciTarih.Day), "23:59");
            return ToList(repository.SelectByKiraciVadeBasTarVadeBitTar(sozlesmeId, kiraciId, basTarih, bitTarih));
        }
        public decimal GetSumBySozlesmeIdOdemePlaniId(int sozlesmeId, int odemePlaniId)
        {
            DataTable dataTable = repository.SelectSumBySozlesmeIdOdemePlaniId(sozlesmeId, odemePlaniId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Toplam"].ToString().ConvertToDecimal();
            }
            return 0;
        }
        public int Save(Odeme item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return item.Id; }
        public bool Update(Odeme item) { if (item == null) throw new ArgumentNullException("item"); Odeme old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool Delete(Odeme item) { if (item == null || item.Id == 0) return false; Odeme old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }

        public Odeme OdemeyiKaydetOdemePlaniniGuncelle(KiraSozlesme kiraSozlesme, OdemePlani odemePlani, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            Odeme odeme = null;
            try
            {
                odeme = new Odeme
                {
                    SozlesmeId = kiraSozlesme.Id,
                    KiraciId = kiraSozlesme.KiraciId,
                    OdemePlaniId = odemePlani.Id,
                    OdemeTarihi = odemeTarihi,
                    OdenenTutar = odenenTutar,
                    Aciklama = aciklama,
                    Olusturan = kullanici
                };
                odeme.Id = Save(odeme);
                if (odeme.Id > 0)
                {
                    decimal toplamOdenen = GetSumBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id);
                    odemePlani.OdenenTutar = toplamOdenen;
                    odemePlani.Aciklama += aciklama + Environment.NewLine;
                    odemePlani.Degistiren = kullanici;
                    new OdemePlaniService().Update(odemePlani);
                }
            }
            catch (Exception exception1)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(new Exception("Ödeme Kaydedilemedi"));
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return odeme;
        }

        public bool OdemeyiSilOdemePlaniniGuncelle(int odemeId, string aciklama, int odemePlaniId, string kullanici)
        {
            bool silindiMi = false;
            try
            {
                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                Odeme odeme = GetById(odemeId);
                if (odeme != null)
                {
                    silindiMi = Delete(odeme);
                }
                if (silindiMi)
                {
                    List<OdemeAyrinti> odemeAyrintiListesi = new OdemeAyrintiService().GetByOdemeIdOdemePlaniId(odemeId, odemePlaniId);
                    if (odemeAyrintiListesi.Count > 0)
                    {
                        new OdemeAyrintiService().DeleteByOdemeIdOdemePlaniId(odemeId, odemePlaniId);
                    }

                    OdemePlani odemePlani = new OdemePlaniService().GetById(odemePlaniId);
                    decimal toplamOdenen = GetSumBySozlesmeIdOdemePlaniId(odemePlani.SozlesmeId, odemePlani.Id);
                    odemePlani.OdenenTutar = toplamOdenen;
                    odemePlani.Aciklama += aciklama + Environment.NewLine +
                        " *" + odeme.OdemeTarihi.ConvertToDatetimeEmptyIfNull() + " tarihli " + odeme.OdenenTutar.ToString("N", culturInfo) + " ödeme silindi." + Environment.NewLine;
                    odemePlani.Degistiren = kullanici;
                    new OdemePlaniService().Update(odemePlani);
                }
            }
            catch (Exception exception1)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(new Exception("Ödeme Silinemedi"));
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return silindiMi;
        }

        public bool OdemeyiVeOdemePlaniniGuncelle(int sozlesmeId, int oncekiOdemePlaniId, int yeniOdemePlaniId, int odemeId, DateTime odemeTarihi, decimal odenenTutar, string aciklama, string kullanici)
        {
            bool guncellendiMi = false;
            try
            {
                OdemePlani oncekiOdemePlani = new OdemePlaniService().GetById(oncekiOdemePlaniId);
                OdemePlani yeniOdemePlani = new OdemePlaniService().GetById(yeniOdemePlaniId);

                if (yeniOdemePlani == null)
                {
                    yeniOdemePlani = new OdemePlani();
                    if (odemeTarihi <= oncekiOdemePlani.OdemeBasTar)
                    {
                        yeniOdemePlani = new OdemePlaniService().GetFirstBySozlesmeId(sozlesmeId);
                    }
                    else if (odemeTarihi >= oncekiOdemePlani.OdemeBitTar)
                    {
                        yeniOdemePlani = new OdemePlaniService().GetLastBySozlesmeId(sozlesmeId);
                    }
                    else
                    {
                        new ExceptionHelper(new Exception("Ödeme Tablosunda OdemePlaniId=0 oldugundan kayit yapilamadi")).PublishException();
                    }
                }

                if (yeniOdemePlani != null)
                {
                    Odeme odeme = GetById(odemeId);
                    if (odeme != null)
                    {
                        odeme.OdemeTarihi = odemeTarihi;
                        odeme.OdenenTutar = odenenTutar;
                        odeme.Aciklama = aciklama;
                        odeme.Degistiren = kullanici;
                        odeme.OdemePlaniId = yeniOdemePlani.Id;
                        odeme.SozlesmeId = yeniOdemePlani.SozlesmeId;
                        guncellendiMi = Update(odeme);
                    }
                }

                if (guncellendiMi)
                {
                    if (oncekiOdemePlani != null)
                    {
                        oncekiOdemePlani.OdenenTutar = GetSumBySozlesmeIdOdemePlaniId(oncekiOdemePlani.SozlesmeId, oncekiOdemePlani.Id);
                        oncekiOdemePlani.Degistiren = kullanici;
                        new OdemePlaniService().Update(oncekiOdemePlani);
                    }
                    if (yeniOdemePlani != null)
                    {
                        yeniOdemePlani.OdenenTutar = GetSumBySozlesmeIdOdemePlaniId(yeniOdemePlani.SozlesmeId, yeniOdemePlani.Id);
                        yeniOdemePlani.Aciklama = aciklama;
                        yeniOdemePlani.Degistiren = kullanici;
                        guncellendiMi = new OdemePlaniService().Update(yeniOdemePlani);
                    }
                }
            }
            catch (Exception exception1)
            {
                guncellendiMi = false;
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(new Exception("Ödeme Kaydedilemedi"));
                exHelper.Exceptions.Add(exception1);
                exHelper.PublishException();
            }
            return guncellendiMi;
        }

        private static List<Odeme> ToList(DataTable t) { return new Odeme().ToList<Odeme>(t); }
        private static Odeme Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
