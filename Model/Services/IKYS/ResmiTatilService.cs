using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class ResmiTatilService
    {
        private readonly ResmiTatilRepository repository;
        public ResmiTatilService() : this(new ResmiTatilRepository()) { }
        public ResmiTatilService(ResmiTatilRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public ResmiTatil GetById(int id) { return Map(repository.SelectById(id)); }
        public List<ResmiTatil> GetAll() { return ToList(repository.SelectAll()); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        public int Save(ResmiTatil item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return item.Id; }
        public bool Update(ResmiTatil item) { if (item == null) return false; ResmiTatil old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return ok; }
        public bool Delete(ResmiTatil item) { if (item == null || item.Id == 0) return false; ResmiTatil old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL); return ok; }
        public List<ResmiTatil> GetByTarih(DateTime baslangic, DateTime bitis) { return ToList(repository.SelectByTarih(baslangic, bitis)); }
        public List<ResmiTatil> GetBySonIkiYil() { DateTime baslangic = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1); return ToList(repository.SelectBySonIkiYil(baslangic, baslangic.AddYears(2).AddDays(1))); }
        public List<ResmiTatil> GetByYil(int yil) { return ToList(repository.SelectByYil(yil)); }
        public bool IsResmiTatil(DateTime tarih) { return repository.SelectByDate(tarih) != null; }
        public string GetAllReturnJson(DateTime baslangic, DateTime bitis)
        {
            List<ResmiTatil> list = GetByTarih(baslangic, bitis); List<CalendarEvent> events = new List<CalendarEvent>(); Faaliyet faaliyet = new Faaliyet();
            foreach (ResmiTatil tatil in list)
            {
                int yearCount = tatil.BaslamaTarihi.Year < 1900 ? bitis.Year - baslangic.Year : 1;
                for (int i = 0; i < yearCount; i++)
                {
                    if (tatil.BaslamaTarihi.Year < 1900) { string basSaat = tatil.BaslamaTarihi.Hour + ":" + tatil.BaslamaTarihi.Minute; string bitSaat = tatil.BitisTarihi.Hour + ":" + tatil.BitisTarihi.Minute; tatil.BaslamaTarihi = UtilityHelper.TariheSaatEkle(new DateTime(baslangic.Year + i, tatil.BaslamaTarihi.Month, tatil.BaslamaTarihi.Day), basSaat); tatil.BitisTarihi = UtilityHelper.TariheSaatEkle(new DateTime(baslangic.Year + i, tatil.BitisTarihi.Month, tatil.BitisTarihi.Day), bitSaat); }
                    CalendarEvent item = new CalendarEvent { state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString(), id = 999, purpose = ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT, title = tatil.Tatil, start = string.Format("{0:s}", tatil.BaslamaTarihi), end = string.Format("{0:s}", tatil.BitisTarihi), url = "", startEditable = false };
                    if (tatil.Yil < 1900) item.allDay = true; if (tatil.BitisTarihi.Day > tatil.BaslamaTarihi.Day) item.allDay = false; faaliyet.RenkBelirle(item); events.Add(item);
                }
            }
            return faaliyet.ToJSON(events);
        }
        private static ResmiTatil Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<ResmiTatil> ToList(DataTable table) { return new ResmiTatil().ToList<ResmiTatil>(table); }
    }
}
