using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Model.Services.IKYS
{
    public class IzinDonemService
    {
        private readonly IzinDonemRepository repository;
        public IzinDonemService() : this(new IzinDonemRepository()) { }
        public IzinDonemService(IzinDonemRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public IzinDonem GetById(int id) { return Map(repository.SelectById(id)); }
        public IzinDonem GetById(int id, PortalLogContext context) { return Trace(context, "IzinDonem.SelectById", () => Map(repository.SelectById(id))); }
        public IzinDonem GetById(int id, SqlTransactionContext transaction, PortalLogContext context) { return Map(repository.SelectById(id, transaction, context)); }
        public List<IzinDonem> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(IzinDonem item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM); return item.Id; }
        public int Save(IzinDonem item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("İzin dönemi oluşturulamadı.");
            new PortalLogRepository().RecordAudit(context, "INSERT", "IzinDonem", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            return item.Id;
        }
        public bool Update(IzinDonem item) { if (item == null) throw new ArgumentNullException("item"); IzinDonem old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM); return ok; }
        public bool Update(IzinDonem item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            IzinDonem old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            new PortalLogRepository().RecordAudit(context, "UPDATE", "IzinDonem", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            return true;
        }
        public bool Delete(IzinDonem item) { if (item == null || item.Id == 0) return false; IzinDonem old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM); return ok; }
        public bool Delete(IzinDonem item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null || item.Id <= 0) return false;
            IzinDonem old = Map(repository.SelectById(item.Id, transaction, context));
            if (old == null || !repository.Delete(item.Id, transaction, context)) return false;
            new PortalLogRepository().RecordAudit(context, "DELETE", "IzinDonem", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            return true;
        }
        public IzinDonem GetByIzinTarihi(int personelId, int izinTipi, DateTime tarih) { return Map(repository.SelectByIzinTarihi(personelId, izinTipi, tarih)); }
        public IzinDonem GetByIzinTarihi(int personelId, int izinTipi, DateTime tarih, PortalLogContext context) { return Trace(context, "IzinDonem.SelectByIzinTarihi", () => Map(repository.SelectByIzinTarihi(personelId, izinTipi, tarih))); }
        public IzinDonem GetByIzinTarihi(int personelId, int izinTipi, DateTime tarih, SqlTransactionContext transaction, PortalLogContext context) { return Map(repository.SelectByIzinTarihi(personelId, izinTipi, tarih, transaction, context)); }
        public List<IzinDonem> GetOncekiYillaraAit(int personelId) { return ToList(repository.SelectOncekiYillaraAit(personelId)); }
        public List<IzinDonem> GetOncekiYillaraAit(int personelId, PortalLogContext context) { return Trace(context, "IzinDonem.SelectOncekiYillaraAit", () => ToList(repository.SelectOncekiYillaraAit(personelId))); }
        public List<IzinDonem> GetByPersonelId(int personelId, int izinTipi) { return ToList(repository.SelectByPersonelId(personelId, izinTipi)); }
        public List<IzinDonem> GetByPersonelId(int personelId, int izinTipi, PortalLogContext context) { return Trace(context, "IzinDonem.SelectByPersonelId", () => ToList(repository.SelectByPersonelId(personelId, izinTipi))); }
        public DataTable GetSumKalanIzinByPersonelId(int personelId, bool sadeceEskiDonemler) { return repository.SelectSumKalanIzinByPersonelId(personelId, sadeceEskiDonemler); }
        public string GetByPersonelIdReturnJson(int personelId, int izinTipi) { return new IzinHareketService().ConvertDataTableToString(repository.SelectByPersonelId(personelId, izinTipi)); }
        public DataTable GetByPersonelIdReturnDataTable(int personelId, int izinTipi) { return repository.SelectByPersonelIdReturnDataTable(personelId, izinTipi); }
        public DataTable GetByPersonelIdReturnDataTable(int personelId, int izinTipi, PortalLogContext context) { return Trace(context, "IzinDonem.SelectByPersonelIdReturnDataTable", () => repository.SelectByPersonelIdReturnDataTable(personelId, izinTipi)); }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new DAO.Repositories.Ortak.PortalLogRepository().TryRecordWebPartOpened(context); }
        private static T Trace<T>(PortalLogContext context, string name, Func<T> action)
        {
            Stopwatch clock = Stopwatch.StartNew(); bool succeeded = false;
            try { T result = action(); succeeded = true; return result; }
            finally { PortalTechnicalLog.WriteSqlTrace(context, "SELECT", name, clock.ElapsedMilliseconds, succeeded); }
        }

        public IzinDonem CreateForPersonel(Personel personel, int izinTipi, DateTime tarih, string currentUserName)
        {
            DateTime izinDonemiBasTar; string birim; if (!GetIzinBaslangic(personel, izinTipi, out izinDonemiBasTar, out birim)) return null;
            DateTime donemBaslangici = new DateTime(tarih.Year - 1, izinDonemiBasTar.Month, izinDonemiBasTar.Day); DateTime donemSonu = donemBaslangici.AddYears(1).AddDays(-1); if (tarih > donemSonu) { donemBaslangici = donemBaslangici.AddYears(1); donemSonu = donemBaslangici.AddYears(1).AddDays(-1); }
            IzinDonem donem = GetByIzinTarihi(personel.Id, izinTipi, tarih); if (donem != null) return donem;
            donem = new IzinDonem { PersonelId = personel.Id, IzinTipi = izinTipi, BaslangicTarihi = donemBaslangici, BitisTarihi = donemSonu, Birim = birim, Adi = donemBaslangici.Year + "-" + donemSonu.Year + " İzin Dönemi", Aciklama = " Otomatik Oluşturuldu ", IzinHakki = IKYSOrtak.IzinHakkiHesapla(personel, izinTipi, donemBaslangici, izinDonemiBasTar), KullanilanIzin = "0", Olusturan = currentUserName };
            donem.KalanIzin = donem.IzinHakki; donem.Id = Save(donem); return donem;
        }
        public IzinDonem CreateForPersonel(Personel personel, int izinTipi, DateTime tarih, string currentUserName, SqlTransactionContext transaction, PortalLogContext context)
        {
            DateTime izinDonemiBasTar; string birim;
            IsBilgileri isBilgileri;
            if (!GetIzinBaslangic(personel, izinTipi, transaction, context, out izinDonemiBasTar, out birim, out isBilgileri)) return null;
            DateTime donemBaslangici = new DateTime(tarih.Year - 1, izinDonemiBasTar.Month, izinDonemiBasTar.Day);
            DateTime donemSonu = donemBaslangici.AddYears(1).AddDays(-1);
            if (tarih > donemSonu) { donemBaslangici = donemBaslangici.AddYears(1); donemSonu = donemBaslangici.AddYears(1).AddDays(-1); }
            IzinDonem donem = GetByIzinTarihi(personel.Id, izinTipi, tarih, transaction, context);
            if (donem != null) return donem;
            string izinHakki = IKYSOrtak.IzinHakkiHesapla(personel, izinTipi, donemBaslangici, izinDonemiBasTar, isBilgileri);
            donem = new IzinDonem { PersonelId = personel.Id, IzinTipi = izinTipi, BaslangicTarihi = donemBaslangici, BitisTarihi = donemSonu, Birim = birim, Adi = donemBaslangici.Year + "-" + donemSonu.Year + " İzin Dönemi", Aciklama = " Otomatik Oluşturuldu ", IzinHakki = izinHakki, KullanilanIzin = "0", Olusturan = currentUserName, KalanIzin = izinHakki };
            donem.Id = Save(donem, transaction, context);
            return donem;
        }
        public IzinDonem UpdateForPersonel(Personel personel, int izinTipi, DateTime tarih, string currentUserName)
        {
            DateTime izinDonemiBasTar; string birim; if (!GetIzinBaslangic(personel, izinTipi, out izinDonemiBasTar, out birim)) return null;
            DateTime donemBaslangici = new DateTime(tarih.Year - 1, izinDonemiBasTar.Month, izinDonemiBasTar.Day); DateTime donemSonu = donemBaslangici.AddYears(1).AddDays(-1); if (tarih > donemSonu) { donemBaslangici = donemBaslangici.AddYears(1); donemSonu = donemBaslangici.AddYears(1).AddDays(-1); }
            IzinDonem donem = GetByIzinTarihi(personel.Id, izinTipi, tarih); if (donem == null) return null; donem.BaslangicTarihi = donemBaslangici; donem.BitisTarihi = donemSonu; donem.Birim = birim; donem.Adi = donemBaslangici.Year + "-" + donemSonu.Year + " İzin Dönemi"; donem.Aciklama = " İzin Dönemi Güncellendi. "; donem.Degistiren = currentUserName;
            if (donem.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) { donem.IzinHakki = IKYSOrtak.UcretliIzinHakkiHesapla(personel, donemBaslangici, izinDonemiBasTar).ToString(); donem.KalanIzin = (donem.IzinHakki.ConvertToInt() - donem.KullanilanIzin.ConvertToInt()).ToString(); Update(donem); }
            if (donem.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) { TimeSpan kalan = donem.IzinHakki.ConvertToTimeSpan() - donem.KullanilanIzin.ConvertToTimeSpan(); donem.KalanIzin = kalan.ToString(); Update(donem); }
            return donem;
        }
        public IzinDonem UpdateForPersonel(Personel personel, int izinTipi, DateTime tarih, string currentUserName, SqlTransactionContext transaction, PortalLogContext context)
        {
            DateTime izinDonemiBasTar; string birim;
            IsBilgileri isBilgileri;
            if (!GetIzinBaslangic(personel, izinTipi, transaction, context, out izinDonemiBasTar, out birim, out isBilgileri)) return null;
            DateTime donemBaslangici = new DateTime(tarih.Year - 1, izinDonemiBasTar.Month, izinDonemiBasTar.Day);
            DateTime donemSonu = donemBaslangici.AddYears(1).AddDays(-1);
            if (tarih > donemSonu) { donemBaslangici = donemBaslangici.AddYears(1); donemSonu = donemBaslangici.AddYears(1).AddDays(-1); }
            IzinDonem donem = GetByIzinTarihi(personel.Id, izinTipi, tarih, transaction, context);
            if (donem == null) return null;
            donem.BaslangicTarihi = donemBaslangici; donem.BitisTarihi = donemSonu; donem.Birim = birim; donem.Adi = donemBaslangici.Year + "-" + donemSonu.Year + " İzin Dönemi"; donem.Aciklama = " İzin Dönemi Güncellendi. "; donem.Degistiren = currentUserName;
            if (donem.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) { donem.IzinHakki = IKYSOrtak.UcretliIzinHakkiHesapla(personel, donemBaslangici, izinDonemiBasTar, isBilgileri).ToString(CultureInfo.InvariantCulture); donem.KalanIzin = (donem.IzinHakki.ConvertToInt() - donem.KullanilanIzin.ConvertToInt()).ToString(); }
            else if (donem.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) { TimeSpan kalan = donem.IzinHakki.ConvertToTimeSpan() - donem.KullanilanIzin.ConvertToTimeSpan(); donem.KalanIzin = kalan.ToString(); }
            if (!Update(donem, transaction, context)) return null;
            return donem;
        }
        public bool UpdateUsedLeave(IzinDonem donem, string sure, bool ekle, string currentUserName)
        {
            bool isSaved = false; if (donem.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT && donem != null) { int used = donem.KullanilanIzin.ConvertToInt() + (ekle ? sure.ConvertToInt() : -sure.ConvertToInt()); donem.KullanilanIzin = used.ToString(); donem.KalanIzin = (donem.IzinHakki.ConvertToInt() - used).ToString(); Update(donem); }
            if (donem.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT && donem != null) { TimeSpan used = donem.KullanilanIzin.ConvertToTimeSpan(); TimeSpan result = used + (ekle ? sure.ConvertToTimeSpan() : -sure.ConvertToTimeSpan()); donem.KullanilanIzin = result.ToString(); donem.KalanIzin = (donem.IzinHakki.ConvertToTimeSpan() - result).ToString(); donem.Degistiren = currentUserName; Update(donem); } return isSaved;
        }
        public bool UpdateUsedLeave(IzinDonem donem, string sure, bool ekle, string currentUserName, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (donem == null) return false;
            if (donem.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
            {
                int used = donem.KullanilanIzin.ConvertToInt() + (ekle ? sure.ConvertToInt() : -sure.ConvertToInt());
                donem.KullanilanIzin = used.ToString();
                donem.KalanIzin = (donem.IzinHakki.ConvertToInt() - used).ToString();
            }
            else if (donem.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                TimeSpan used = donem.KullanilanIzin.ConvertToTimeSpan();
                TimeSpan result = used + (ekle ? sure.ConvertToTimeSpan() : -sure.ConvertToTimeSpan());
                donem.KullanilanIzin = result.ToString();
                donem.KalanIzin = (donem.IzinHakki.ConvertToTimeSpan() - result).ToString();
            }
            else return false;
            donem.Degistiren = currentUserName;
            return Update(donem, transaction, context);
        }
        private bool GetIzinBaslangic(Personel personel, int izinTipi, out DateTime baslangic, out string birim)
        {
            IsBilgileri ib = new IsBilgileriService().GetByPersonelId(personel.Id); string baslama = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull(); string izinBas = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull(); izinBas = string.IsNullOrEmpty(izinBas) ? baslama : izinBas; birim = ProjeConstants.IZIN_BIRIMI_GUN; baslangic = DateTime.MinValue; if (string.IsNullOrEmpty(izinBas)) return false; baslangic = izinBas.ConvertToDatetime(); if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) { baslangic = new DateTime(baslangic.Year, 1, 1); birim = ProjeConstants.IZIN_BIRIMI_SAAT; } return true;
        }
        private bool GetIzinBaslangic(Personel personel, int izinTipi, SqlTransactionContext transaction, PortalLogContext context, out DateTime baslangic, out string birim, out IsBilgileri isBilgileri)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            isBilgileri = new IsBilgileriService().GetByPersonelId(personel.Id, transaction, context);
            string baslama = isBilgileri == null ? "" : isBilgileri.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinBas = isBilgileri == null ? "" : isBilgileri.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();
            izinBas = string.IsNullOrEmpty(izinBas) ? baslama : izinBas;
            birim = ProjeConstants.IZIN_BIRIMI_GUN;
            baslangic = DateTime.MinValue;
            if (string.IsNullOrEmpty(izinBas)) return false;
            baslangic = izinBas.ConvertToDatetime();
            if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                baslangic = new DateTime(baslangic.Year, 1, 1);
                birim = ProjeConstants.IZIN_BIRIMI_SAAT;
            }
            return true;
        }
        private static IzinDonem Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<IzinDonem> ToList(DataTable table) { return new IzinDonem().ToList<IzinDonem>(table); }
    }
}
