using Model.IKYS;
using Model.MTS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace MTS_WebParts.Logging
{
    internal static class MtsCalendar
    {
        internal static string Json(object value)
        {
            return new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(value).Replace("<", "\\u003c")
                .Replace(">", "\\u003e").Replace("&", "\\u0026")
                .Replace("\u2028", "\\u2028").Replace("\u2029", "\\u2029");
        }

        internal static string Merge(params string[] sources)
        {
            var events = new List<object>();
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            foreach (string source in sources)
                if (!string.IsNullOrWhiteSpace(source)) events.AddRange(serializer.Deserialize<object[]>(source));
            return Json(events);
        }

        internal static string View(string value, bool weekly = false)
        {
            switch (value)
            {
                case "dayGridMonth": case "timeGridWeek": case "timeGridDay": case "listWeek": return Json(value);
                case "timeGridFourDay": return Json(weekly ? value : "dayGridMonth");
                default: return Json(weekly ? "timeGridWeek" : "dayGridMonth");
            }
        }

        internal static string InitialDate(string value)
        {
            DateTime date;
            if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) date = DateTime.Today;
            return Json(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        private static DateTime Anniversary(DateTime source, int year)
        {
            return new DateTime(year, source.Month, Math.Min(source.Day, DateTime.DaysInMonth(year, source.Month)));
        }

        private static CalendarEvent Celebration(string title, DateTime date, string purpose, string url)
        {
            var item = new CalendarEvent { id = 999, state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString(),
                title = title, start = date.ToString("s"), end = date.AddDays(1).ToString("s"),
                purpose = purpose, allDay = true, startEditable = false, url = url };
            new Faaliyet().RenkBelirle(item);
            return item;
        }

        internal static string ContactBirthdays(PortalLogContext context)
        {
            var events = new List<CalendarEvent>();
            string current = HttpContext.Current.Request.Url.AbsolutePath;
            foreach (var person in new Model.Services.MTS.KisiService(context).GetBirthdayCelebrations())
            {
                if (person.DogumTarihi <= ProjeConstants.NULL_TARIH) continue;
                for (int year = DateTime.Today.Year - 1; year <= DateTime.Today.Year + 1; year++)
                    events.Add(Celebration("D. Günü: " + person.Adi + " " + person.Soyadi, Anniversary(person.DogumTarihi, year),
                        ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT, current.Substring(0, current.LastIndexOf('/')) + "/" + ProjeConstants.PAGE_KISI_KARTI + "?KisiId=" + person.Id));
            }
            return Json(events);
        }

        internal static string EmployeeCelebrations(PortalLogContext context)
        {
            var events = new List<CalendarEvent>();
            DataTable table = new PersonelService().GetCalendarCelebrations(context);
            if (table != null) foreach (DataRow row in table.Rows)
            {
                string name = row["Adi"] + " " + row["Soyadi"];
                DateTime birth = row["DogumTar"].ConvertToDatetime(), marriage = row["EvlilikTar"].ConvertToDatetime();
                for (int year = DateTime.Today.Year - 1; year <= DateTime.Today.Year + 1; year++)
                {
                    if (birth > ProjeConstants.NULL_TARIH && row["DogumGunuKutlama"].ConvertToBool())
                        events.Add(Celebration("D. Günü: " + name, Anniversary(birth, year), ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT, ""));
                    if (marriage > ProjeConstants.NULL_TARIH && row["MedeniHali"].ConvertToInt() > 0 && row["EvlilikKutlama"].ConvertToBool())
                        events.Add(Celebration("Evl. Yıld.: " + name, Anniversary(marriage, year), ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT, ""));
                }
            }
            return Json(events);
        }

        internal static string Holidays(PortalLogContext context)
        {
            var events = new List<CalendarEvent>();
            DateTime from = new DateTime(DateTime.Today.Year - 1, 1, 1), to = from.AddYears(3).AddMilliseconds(-3);
            foreach (var holiday in new ResmiTatilService().GetByTarih(from, to, context))
            {
                bool recurring = holiday.Yil < 1900;
                int first = recurring ? from.Year : holiday.BaslamaTarihi.Year;
                int last = recurring ? to.Year : first;
                for (int year = first; year <= last; year++)
                {
                    DateTime start = recurring ? Anniversary(holiday.BaslamaTarihi, year).Add(holiday.BaslamaTarihi.TimeOfDay) : holiday.BaslamaTarihi;
                    DateTime end = recurring ? Anniversary(holiday.BitisTarihi, year + (holiday.BitisTarihi.Year - holiday.BaslamaTarihi.Year)).Add(holiday.BitisTarihi.TimeOfDay) : holiday.BitisTarihi;
                    if (end < start) end = end.AddYears(1);
                    bool allDay = start.TimeOfDay == TimeSpan.Zero && (end.TimeOfDay == TimeSpan.Zero || end.Hour == 23);
                    var item = new CalendarEvent { id = 999, state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString(),
                        title = holiday.Tatil, start = start.ToString("s"), end = (allDay ? end.Date.AddDays(1) : end).ToString("s"),
                        allDay = allDay, startEditable = false, purpose = ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT, url = "" };
                    new Faaliyet().RenkBelirle(item); events.Add(item);
                }
            }
            return Json(events);
        }

        internal static DateTime ReadInstant(string value)
        {
            DateTimeOffset date;
            // Browser uses toISOString(), so convert UTC explicitly to the institution's time zone.
            if (!DateTimeOffset.TryParseExact(value, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date))
                throw new MtsInputException("Tarih bilgisi geçersiz. Sayfayı yenileyip yeniden deneyin.");
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")).DateTime;
        }

        internal static void CompleteMove(PortalLogContext context, Faaliyet saved, Action refresh)
        {
            try
            {
                if (saved.TakvimeIslendi)
                    Model.Services.MTS.MtsActivityNotificationService.Send(saved, ProjeConstants.PARAM_OZELKALEMMAILADRESI,
                        saved.FaaliyetDurumu == ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT);
            }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(context, "MTS." + context.WebPart + ".CalendarNotification", error);
                try { refresh(); }
                catch (Exception refreshError) { PortalTechnicalLog.WriteError(context, "MTS." + context.WebPart + ".AfterCommit", refreshError); }
                Messages.Warning("Faaliyetin tarihi güncellendi. Özel kalem takvimine bildirim gönderilemedi. Kaydı tekrar değiştirmeden bildirimi kontrol edin. Başvuru kodu: " + context.OperationId.ToString("N"));
                return;
            }
            MtsWebPartLog.FinishCommitted(context, "Faaliyetin tarih ve saati güncellendi.", refresh);
        }
    }
}
