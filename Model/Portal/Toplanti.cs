using Model.MTS;
using Model.Ortak;
using Model.Services.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;

namespace Model.Portal
{
    public class Toplanti : EntityBase
    {
        public Guid UniqueId { get; set; }
        public string ToplantiKonusu { get; set; }
        public int ToplantiYeri { get; set; }
        public string ToplantiYeriDiger { get; set; }
        public int ToplantiYetkilisi { get; set; }
        public int Koordinator { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public string DisKatilimcilar { get; set; }
        public bool CevrimIci { get; set; }
        public bool IkramOnayi { get; set; }
        public string IkramMalzemesi { get; set; }
        public string Aciklama { get; set; }

        public int Save()
        {
            return new ToplantiService().Save(this);
        }

        public bool Update()
        {
            return new ToplantiService().Update(this);
        }

        public bool Delete()
        {
            return new ToplantiService().Delete(this);
        }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new ToplantiService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new ToplantiService().GetAll(), typeof(List<T>));
        }

        public List<Toplanti> SelectByToplantiYeri(int id)
        {
            return new ToplantiService().GetByYeri(id);
        }

        public Toplanti Select(int id)
        {
            return new ToplantiService().GetById(id);
        }

        public Toplanti SelectByBaslangicTarihi(int y, DateTime d)
        {
            return new ToplantiService().GetByStart(y, d);
        }

        public Toplanti SelectByBitisTarihi(int y, DateTime d)
        {
            return new ToplantiService().GetByEnd(y, d);
        }

        public DataTable SelectAllByKatilimciToplantiReturnDataTable(int id, string a)
        {
            return new ToplantiService().GetByParticipantMeeting(0, a, id == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ? (int?)null : id);
        }

        public DataTable SelectAllByKatilimciToplantiTarihReturnDataTable(int id, string a, DateTime f, DateTime t)
        {
            return new ToplantiService().GetByParticipantMeetingDate(0, a, id == Utility.ProjeGlobal.ProjeConstants.HEPSI_INT ? (int?)null : id, f, t);
        }

        public DataTable SelectAllByKatilimci(int id)
        {
            return new ToplantiService().GetByParticipant(id);
        }

        public List<Toplanti> SelectByKatilimciTarih(int id, DateTime d)
        {
            return new ToplantiService().GetByParticipantDate(id, d);
        }

        public string SelectAllReturnJson()
        {
            return new ToplantiService().GetCalendarJson();
        }

        public string ToJSON(List<CalendarEvent> list)
        {
            JavaScriptSerializer s = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            return s.Serialize(list);
        }
    }
}

