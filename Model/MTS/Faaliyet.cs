using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    [Serializable]
    public class Faaliyet : EntityBase
    {
        public Faaliyet()
        {
        }
        public Guid UniqueId { get; set; }
        public string FaaliyetTipi { get; set; }
        public int FaaliyetAmaciId { get; set; }
        public string FaaliyetKonusu { get; set; }
        public string FaaliyetYeriStr { get; set; }
        public int FaaliyetDurumu { get; set; }
        public bool TumGun { get; set; }
        public bool AcikTarih { get; set; }
        public int DisIrtibatId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public string Aciklama { get; set; }
        public string YoneticiNotu { get; set; }
        public bool TakvimeIslendi { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as Faaliyet;

            if (other == null)
                return false;

            if (FaaliyetTipi != other.FaaliyetTipi
                || FaaliyetAmaciId != other.FaaliyetAmaciId
                || !FaaliyetKonusu.Equals(other.FaaliyetKonusu)
                || !FaaliyetYeriStr.Equals(other.FaaliyetYeriStr)
                || TumGun != other.TumGun
                || AcikTarih != other.AcikTarih
                || DisIrtibatId != other.DisIrtibatId
                || BaslangicTarihi != other.BaslangicTarihi
                || BitisTarihi != other.BitisTarihi
                || BaslangicSaati != other.BaslangicSaati
                || BitisSaati != other.BitisSaati
                || Aciklama != other.Aciklama
                || YoneticiNotu != other.YoneticiNotu)
                return false;

            return true;
        }
        public int Save()
        {
            return new FaaliyetService().Save(this);
        }
        public bool Update()
        {
            return new FaaliyetService().Update(this);
        }
        public bool Delete()
        {
            return new FaaliyetService().Delete(this);
        }
        public Faaliyet Select(int id)
        {
            Id = id;
            return new FaaliyetService().GetById(id);
        }
        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new FaaliyetService().GetById(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new FaaliyetService().GetAll(), typeof(List<T>));
        }
        public string SelectAllReturnJson(string acikTarih)
        {
            DataTable dataTable = new FaaliyetService().GetAllData(acikTarih);

            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            if (dataTable!=null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    CalendarEvent item = new CalendarEvent();
                    item.state = dataRow["FaaliyetDurumu"].ToString();

                    item.id = int.Parse(dataRow["Id"].ToString());
                    int faaliyetAmaci= dataRow["FaaliyetAmaciId"].ReturnZeroIfNull().ConvertToInt();
                    item.purpose = faaliyetAmaci.ToString();
                    item.title = dataRow["FaaliyetKonusu"].ToString();
                    //item.description = item.title;
                    item.start = string.Format("{0:s}", dataRow["BaslangicTarihi"]);
                    item.end = string.Format("{0:s}", dataRow["BitisTarihi"]);
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_FAALIYET_GIRIS;
                    item.url = newUrl+ "?DestinationApp=Duzenle&FaaliyetId=" + item.id;
                    item.allDay = dataRow["TumGun"].ReturnFalseIfNull().ConvertToBool();
                    item.startEditable = true;
                    RenkBelirle(item);
                    if (item.state.Equals(ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT.ToString()))
                    {
                        item.color = Color.Red.Name;
                        item.textColor = Color.Black.Name;
                        item.className = "iptal-edildi";
                    }
                    //item.className = "iptal-edildi";
                    eventItems.Add(item);
                }
            }
            string json = ToJSON(eventItems);
            return json;
        }
        public List<Faaliyet> SelectAllReturnList(string acikTarih)
        {
            return new FaaliyetService().GetAll(acikTarih);
        }
        public List<Faaliyet> SelectByTarihReturnList(DateTime tarih)
        {
            return new FaaliyetService().GetByDate(tarih);
        }
        public DataTable SelectAllByKatilimciFaaliyetReturnDataTable( int faaliyetId, int monthBefore, string acikTarihli, DateTime bastar, DateTime bittar,string faaliyetAmaci)
        {
            return new FaaliyetService().GetParticipantList(faaliyetId, monthBefore, acikTarihli, bastar, bittar, faaliyetAmaci);
        }
        public DataTable SelectByKatilimciReturnDataTable(int katilimciId, int faaliyetId)
        {
            return new FaaliyetService().GetByParticipant(katilimciId, faaliyetId);
        }
        public void RenkBelirle(CalendarEvent item)
        {

            switch (item.purpose)
            {
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        item.color = Color.Orange.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_ZIYARET_INT:
                    {
                        item.color = Color.Blue.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_ZIYARET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        item.color = Color.Green.Name;
                        item.textColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        item.color = Color.Aqua.Name;
                        item.textColor = Color.Black.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT:
                    {
                        item.color = Color.Aquamarine.Name;
                        item.textColor = Color.Black.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_DOGUMGUNU;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        item.color = Color.LightBlue.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        item.color = Color.Yellow.Name;
                        item.textColor = Color.Black.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        item.color = Color.MediumVioletRed.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_GORUSME_INT:
                    {
                        item.color = Color.DeepSkyBlue.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_GORUSME;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT:
                    {
                        item.color = Color.Coral.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_SEYAHAT;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_BILGI_INT:
                    {
                        item.color = Color.DimGray.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_BILGI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_VAKIF_TOPLANISI_INT:
                    {
                        item.color = Color.Red.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.FAALIYET_AMACI_VAKIF_TOPLANISI;
                        break;
                    }
                default:
                    break;
            }
            if (item.state.Equals(ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT.ToString()))
            {
                item.color = Color.LightGray.Name;
                item.textColor = Color.Black.Name;
            }

        }
        public string ToJSON(List<CalendarEvent> eventList)
        {
            string json = "[]";
            try
            {
                if (eventList != null)
                {

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    foreach (var item in eventList)
                    {
                        childRow = new Dictionary<string, object>();
                        childRow.Add("id", item.id);
                        childRow.Add("title", item.title);
                        //childRow.Add("description", item.description);
                        childRow.Add("start", item.start);
                        childRow.Add("end", item.end);
                        childRow.Add("color", item.color);
                        childRow.Add("textColor", item.textColor);
                        childRow.Add("allDay", item.allDay);
                        childRow.Add("url", item.url);
                        childRow.Add("className", item.className);

                        parentRow.Add(childRow);
                    }
                    jsSerializer.MaxJsonLength = Int32.MaxValue;
                    json = jsSerializer.Serialize(parentRow);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
                throw;
            }
            return json;
        }
        public override int GetHashCode()
        {
            int hashCode = 477006145;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetTipi);
            hashCode = hashCode * -1521134295 + FaaliyetAmaciId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetKonusu);
            hashCode = hashCode * -1521134295 + FaaliyetYeriStr.GetHashCode();
            hashCode = hashCode * -1521134295 + FaaliyetDurumu.GetHashCode();
            hashCode = hashCode * -1521134295 + TumGun.GetHashCode();
            hashCode = hashCode * -1521134295 + AcikTarih.GetHashCode();
            hashCode = hashCode * -1521134295 + DisIrtibatId.GetHashCode();
            hashCode = hashCode * -1521134295 + BaslangicTarihi.GetHashCode();
            hashCode = hashCode * -1521134295 + BitisTarihi.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaslangicSaati);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BitisSaati);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Aciklama);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(YoneticiNotu);
            return hashCode;
        }

        public List<string> SelectAllDistinctFaaliyetYeri()
        {
            return new FaaliyetService().GetDistinctPlaces();
        }

        public static bool operator ==(Faaliyet left, Faaliyet right)
        {
            return EqualityComparer<Faaliyet>.Default.Equals(left, right);
        }

        public static bool operator !=(Faaliyet left, Faaliyet right)
        {
            return !(left == right);
        }
    }
}
