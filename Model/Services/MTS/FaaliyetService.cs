using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class FaaliyetService
    {
        private readonly FaaliyetRepository repository;

        public FaaliyetService() : this(new FaaliyetRepository())
        {
        }

        public FaaliyetService(FaaliyetRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public Faaliyet GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Faaliyet> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<Faaliyet> GetAll(string acikTarih)
        {
            return ToList(repository.SelectByAcikTarih(acikTarih));
        }

        public DataTable GetAllData(string acikTarih)
        {
            return repository.SelectByAcikTarih(acikTarih);
        }

        public string GetCalendarJson(string acikTarih)
        {
            DataTable dataTable = GetAllData(acikTarih);
            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    CalendarEvent item = new CalendarEvent
                    {
                        state = dataRow["FaaliyetDurumu"].ToString(),
                        id = int.Parse(dataRow["Id"].ToString()),
                        purpose = dataRow["FaaliyetAmaciId"].ReturnZeroIfNull().ConvertToInt().ToString(),
                        title = dataRow["FaaliyetKonusu"].ToString(),
                        start = string.Format("{0:s}", dataRow["BaslangicTarihi"]),
                        end = string.Format("{0:s}", dataRow["BitisTarihi"]),
                        allDay = dataRow["TumGun"].ReturnFalseIfNull().ConvertToBool(),
                        startEditable = true
                    };

                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_FAALIYET_GIRIS;
                    item.url = newUrl + "?DestinationApp=Duzenle&FaaliyetId=" + item.id;
                    new Faaliyet().RenkBelirle(item);

                    if (item.state.Equals(ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT.ToString()))
                    {
                        item.color = Color.Red.Name;
                        item.textColor = Color.Black.Name;
                        item.className = "iptal-edildi";
                    }

                    eventItems.Add(item);
                }
            }

            return new Faaliyet().ToJSON(eventItems);
        }

        public List<Faaliyet> GetByDate(DateTime tarih)
        {
            DateTime baslangic = new DateTime(tarih.Year, tarih.Month, tarih.Day);
            DateTime bitis = UtilityHelper.TariheSaatEkle(tarih, "23:59");
            return ToList(repository.SelectByDate(baslangic, bitis));
        }

        public DataTable GetParticipantList(int faaliyetId, int monthBefore, string acikTarihli, DateTime basTar, DateTime bitTar, string faaliyetAmaci)
        {
            return repository.SelectParticipants(
                faaliyetId,
                monthBefore,
                acikTarihli,
                basTar,
                bitTar,
                faaliyetAmaci,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE,
                ProjeConstants.REFERANS_TARIHI);
        }

        public DataTable GetByParticipant(int katilimciId, int faaliyetId)
        {
            return repository.SelectByParticipant(
                katilimciId,
                faaliyetId,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE);
        }

        public List<string> GetDistinctPlaces()
        {
            return repository.SelectDistinctPlaces()
                .AsEnumerable()
                .Select(row => row.Field<string>("FaaliyetYeri"))
                .ToList();
        }

        public int Save(Faaliyet item)
        {
            item.UniqueId = Guid.NewGuid();
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return item.Id;
        }

        public bool Update(Faaliyet item)
        {
            Faaliyet old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return updated;
        }

        public bool Delete(Faaliyet item)
        {
            Faaliyet old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return deleted;
        }

        private static List<Faaliyet> ToList(DataTable table)
        {
            return new Faaliyet().ToList<Faaliyet>(table);
        }

        private static Faaliyet Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
