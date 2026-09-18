using DAO.Ortak;
using Model.Ortak;
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
    public class Faaliyet : ParentClass
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
        public override int Save()
        {
            try
            {
                UniqueId= Guid.NewGuid();
                GenericEntity<Faaliyet> genericEntity = new GenericEntity<Faaliyet>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                }
                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    Faaliyet item = Select<Faaliyet>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Faaliyet> genericEntity = new GenericEntity<Faaliyet>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<Faaliyet> genericEntity = new GenericEntity<Faaliyet>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Faaliyet item = Select<Faaliyet>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Faaliyet Select(int id)
        {
            GenericEntity<Faaliyet> genericEntity = new GenericEntity<Faaliyet>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Faaliyet> list = ToList<Faaliyet>(dataTable);
            Faaliyet item = new Faaliyet();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<Faaliyet> genericEntity = new GenericEntity<Faaliyet>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Faaliyet> list = ToList<Faaliyet>(dataTable);
            Faaliyet item = new Faaliyet();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Faaliyet_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Faaliyet> list = ToList<Faaliyet>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string SelectAllReturnJson(string acikTarih)
        {
            string acikTarihStr = acikTarih.Equals(ProjeConstants.HEPSI) ? "" : 
                (acikTarih.Equals(ProjeConstants.FAALIYET_ACIKTARIHLI)? " WHERE AcikTarih=1" : " WHERE AcikTarih=0");
            string sqlString = SelectAllSQL(acikTarihStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }

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
            string acikTarihStr = acikTarih.Equals(ProjeConstants.HEPSI) ? "" :
                (acikTarih.Equals(ProjeConstants.FAALIYET_ACIKTARIHLI) ? " WHERE AcikTarih=1" : " WHERE AcikTarih=0");
            string sqlString = SelectAllSQL(acikTarihStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Faaliyet> list = ToList<Faaliyet>(dataTable);

            return list;
        }
        public List<Faaliyet> SelectByTarihReturnList(DateTime tarih)
        {
            DateTime bastar = new DateTime(tarih.Year,tarih.Month,tarih.Day);
            DateTime bittar = UtilityHelper.TariheSaatEkle(tarih, "23:59");
            string sqlString = string.Format(@"
                SELECT * FROM Faaliyet_Table
                WHERE (BaslangicTarihi <={0} AND BitisTarihi >= {1}) ORDER BY BaslangicTarihi 
            ", bittar.ReturnTRDateFormat(),bastar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Faaliyet> list = ToList<Faaliyet>(dataTable);

            return list;
        }
        public DataTable SelectAllByKatilimciFaaliyetReturnDataTable( int faaliyetId, int monthBefore, string acikTarihli, DateTime bastar, DateTime bittar,string faaliyetAmaci)
        {
            string bastarStr = bastar < ProjeConstants.REFERANS_TARIHI ? string.Empty : string.Format(" AND BaslangicTarihi >= {0}",bastar.ReturnTRDateFormat());
            string bittarStr = bittar < ProjeConstants.REFERANS_TARIHI ? string.Empty : string.Format(" AND BitisTarihi <= {0}",bittar.ReturnTRDateFormat());
            string acikTarililerHaric = acikTarihli.Equals(ProjeConstants.HEPSI) ? string.Empty: (acikTarihli.Equals(ProjeConstants.FAALIYET_ACIKTARIHLI) ? " AND B.AcikTarih=1 ": " AND B.AcikTarih=0 ");
            string faaliyetIdStr = faaliyetId == ProjeConstants.HEPSI_INT ? "" : " AND B.Id=" + faaliyetId;
            string monthBeforeStr = monthBefore == 0 ? string.Empty : string.Format("AND BaslangicTarihi > DateAdd(month, {0}, Convert(date, GetDate()))", monthBefore);
            string faaliyetAmaciStr = string.IsNullOrEmpty(faaliyetAmaci.Trim()) || faaliyetAmaci.Equals(ProjeConstants.HEPSI) ? "" : " AND B.FaaliyetAmaciId IN " + faaliyetAmaci;
            string sqlString = string.Format(@"
                SELECT C.KatilimciTipi,A.KatilimciId, A.Id KatilimId,A.TakvimDaveti,B.Aciklama, B.OlusturmaTarihi,
                    C.Adi, C.Soyadi, C.EPosta,C.EPosta,
	                B.Id FaaliyetId, B.BaslangicTarihi,B.BaslangicSaati, B.BitisTarihi,B.BitisSaati,
	                B.FaaliyetAmaciId,B.FaaliyetDurumu,B.FaaliyetKonusu,B.FaaliyetTipi,B.FaaliyetYeriStr FaaliyetYeri, B.TumGun,B.AcikTarih,
	                H.Adi Kurumu
                FROM Faaliyet_Table B  
	                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId=B.Id 
	                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
                    LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum={0}
					LEFT JOIN MTSKurumTanim_Table H ON H.Id = D.MTSKurumTanimId
                    LEFT JOIN IletisimBilgileri_Table G ON G.PersonelId = A.KatilimciId
                WHERE B.Id IS NOT NULL
                {1}
                {2}
                {3}
                {4}
                {5}
                {6}
                ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, Adi,Soyadi 
            ", ProjeConstants.MTSGOREVDURUMU_GOREVDE.ReturnQuotedValue(), acikTarililerHaric, faaliyetIdStr, monthBeforeStr, bastarStr, bittarStr, faaliyetAmaciStr);
            
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectByKatilimciReturnDataTable(int katilimciId, int faaliyetId)
        {
            string katilimciIdStr = katilimciId > 0 ? " AND KatilimciId=" + katilimciId : "";//" AND A.FaaliyetId in (SELECT FaaliyetId FROM FaaliyetKatilim_Table WHERE KatilimciId=" + katilimciId + ")" : "";
            string faaliyetIdStr = faaliyetId > 0 ? " AND A.FaaliyetId=" + faaliyetId : "";
            string sqlString = string.Format(@"
                SELECT C.KatilimciTipi,A.KatilimciId, A.Id KatilimId,
                    C.Adi, C.Soyadi, E.Adi Kurumu,
	                B.Id FaaliyetId, B.BaslangicTarihi,B.BaslangicSaati, B.BitisTarihi,B.BitisSaati,
	                B.FaaliyetAmaciId,B.FaaliyetDurumu,B.FaaliyetKonusu,B.FaaliyetTipi,B.FaaliyetYeriStr FaaliyetYeri,B.TumGun,B.AcikTarih
	
                FROM Faaliyet_Table B  
	                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId=B.Id {0}
	                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
	                LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum={1}
					LEFT JOIN MTSKurumTanim_Table E ON E.Id = D.MTSKurumTanimId
                WHERE A.FaaliyetId IS NOT NULL
                {2}
                ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, Adi,Soyadi 
            ", katilimciIdStr,ProjeConstants.MTSGOREVDURUMU_GOREVDE.ReturnQuotedValue(), faaliyetIdStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
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
        private string SelectAllSQL(string kriter)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Faaliyet_Table 
                {0}
                ORDER BY FaaliyetKonusu
                ", kriter);

            return sqlString;
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
            string sqlString = @"
                SELECT DISTINCT(FaaliyetYeriStr) FaaliyetYeri FROM Faaliyet_Table
            ";
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<string> list = dataTable.AsEnumerable()
                           .Select(r => r.Field<string>("FaaliyetYeri"))
                           .ToList();

            return list;
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