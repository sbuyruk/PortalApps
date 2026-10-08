using DAO.Repositories.Portal;
using Model.MTS;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    public class ToplantiService
    {
        private readonly ToplantiRepository r;

        public ToplantiService()
            : this(new ToplantiRepository())
        {
        }

        public ToplantiService(ToplantiRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public Toplanti GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<Toplanti> GetAll()
        {
            return List(r.SelectAll());
        }

        public List<Toplanti> GetByYeri(int id)
        {
            return List(r.SelectByYeri(id));
        }

        public Toplanti GetByStart(int y, DateTime d)
        {
            return Map(r.SelectByStart(y, d));
        }

        public Toplanti GetByEnd(int y, DateTime d)
        {
            return Map(r.SelectByEnd(y, d));
        }

        public DataTable GetByParticipantMeeting(int id, string a, int? m)
        {
            return r.SelectByParticipantMeeting(id, a, m);
        }

        public DataTable GetByParticipantMeetingDate(int id, string a, int? m, DateTime f, DateTime t)
        {
            return r.SelectByParticipantMeetingDate(id, a, m, f, t);
        }

        public DataTable GetByParticipant(int id)
        {
            return r.SelectByParticipant(id);
        }

        public List<Toplanti> GetByParticipantDate(int id, DateTime d)
        {
            DateTime f = new DateTime(d.Year, d.Month, d.Day);
            return List(r.SelectByParticipantDate(id, f, UtilityHelper.TariheSaatEkle(f, "23:59")));
        }

        public string GetCalendarJson()
        {
            List<CalendarEvent> e = new List<CalendarEvent>();
            DataTable t = r.SelectAll();

            if (t != null)
                foreach (DataRow x in t.Rows)
                {
                    int y = x["ToplantiYeri"].ConvertToInt();
                    e.Add(new CalendarEvent
                    {
                        id = int.Parse(x["Id"].ToString()),
                        title = x["ToplantiKonusu"].ToString(),
                        start = string.Format("{0:s}", x["BaslangicTarihi"]),
                        end = string.Format("{0:s}", x["BitisTarihi"]),
                        color = y == 1 ? Color.Red.Name : y == 2 ? Color.Orange.Name : y == 3 ? Color.Green.Name : null,
                        textColor = Color.White.Name,
                        purpose = ProjeConstants.FAALIYET_AMACI_TOPLANTI
                    });
                }

            return ToJson(e);
        }

        public int Save(Toplanti x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            x.OlusturmaTarihi = DateTime.Now;
            x.Olusturan = UtilityHelper.GetCurrentUserName();
            x.Id = r.Insert(x);

            if (x.Id > 0 && ProjeConstants.PORTAL_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(x, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTI);

            return x.Id;
        }

        public bool Update(Toplanti x)
        {
            if (x == null || x.Id == 0)
                return false;

            Toplanti old = GetById(x.Id);
            x.DegistirmeTarihi = DateTime.Now;
            x.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = r.Update(x);

            if (ok && ProjeConstants.PORTAL_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(x, old, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTI);

            return ok;
        }

        public bool Delete(Toplanti x)
        {
            if (x == null || x.Id == 0)
                return false;

            Toplanti old = GetById(x.Id);
            if (old == null)
                return false;

            bool ok = r.Delete(x.Id);
            if (ok && ProjeConstants.PORTAL_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTI);

            return ok;
        }

        private static List<Toplanti> List(DataTable t)
        {
            return new Toplanti().ToList<Toplanti>(t);
        }

        private static Toplanti Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }

        private static string ToJson(List<CalendarEvent> list)
        {
            JavaScriptSerializer s = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            return s.Serialize(list.Select(x => new Dictionary<string, object>
            {
                { "id", x.id },
                { "title", x.title },
                { "start", x.start },
                { "end", x.end },
                { "color", x.color },
                { "textColor", x.textColor },
                { "allDay", x.allDay },
                { "url", x.url },
                { "className", x.className }
            }).ToList());
        }
    }
}
