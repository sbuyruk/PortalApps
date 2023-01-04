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
    public class Randevu : ParentClass
    {
        public Randevu()
        {
        }

        public string RandevuTipi { get; set; }
        public int RandevuAmaci { get; set; }
        public string RandevuKonusu { get; set; }
        public int RandevuYeri { get; set; }
        public int RandevuDurumu { get; set; }
        public bool TumGun { get; set; }
        public bool AcikTarih { get; set; }
        public int IcIrtibatId { get; set; }
        public int DisIrtibatId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public string Aciklama { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as Randevu;

            if (other == null)
                return false;

            if (RandevuTipi != other.RandevuTipi 
                || RandevuAmaci != other.RandevuAmaci
                || RandevuKonusu != other.RandevuKonusu
                || RandevuYeri != other.RandevuYeri
                || TumGun != other.TumGun
                || AcikTarih != other.AcikTarih
                || IcIrtibatId != other.IcIrtibatId
                || DisIrtibatId != other.DisIrtibatId
                || BaslangicTarihi != other.BaslangicTarihi
                || BitisTarihi != other.BitisTarihi
                || BaslangicSaati != other.BaslangicSaati
                || BitisSaati != other.BitisSaati
                || Aciklama != other.Aciklama)
                return false;

            return true;
        }
        public override bool Delete()
        {
            bool deleteLog = ProjeConstants.DELETE_LOG;
            try
            {
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    if (deleteLog)
                    {
                        Randevu item = Select<Randevu>(Id);
                        if (item != null)
                        {
                            isDeleted = dao.DeleteFromDb(sqlString, "");
                        }
                        else isDeleted = false;
                        if (isDeleted)
                        {
                            OlayKayit olayKayit = new OlayKayit();
                            olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                        }
                    }
                    else
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override int Save()
        {
            bool saveLog = ProjeConstants.SAVE_LOG;
            try
            {
                GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && saveLog)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESI_DAGITIM);
                }
                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public Randevu Select(int id)
        {
            GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);
            Randevu item = new Randevu();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);
            Randevu item = new Randevu();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Randevu_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool updateLog = ProjeConstants.UPDATE_LOG;
            bool isSuccess = false;
            try
            {
                if (updateLog)
                {

                    if (this != null)
                    {
                        Randevu item = Select<Randevu>(Id);
                        if (Id != 0)
                        {
                            GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_UPDATE);
                            DegistirmeTarihi = DateTime.Now;
                            string sqlString = genericEntity.GetQuery(this);
                            isSuccess = dao.Update2Db(sqlString);
                        }
                        if (isSuccess)
                        {
                            OlayKayit olayKayit = new OlayKayit();
                            olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                        }
                    }

                }
                if (Id != 0)
                {
                    GenericEntity<Randevu> genericEntity = new GenericEntity<Randevu>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public string SelectAllReturnJson(string acikTarih)
        {
            string acikTarihStr = acikTarih.Equals(ProjeConstants.HEPSI) ? "" : 
                (acikTarih.Equals(ProjeConstants.RANDEVU_ACIKTARIHLI)? " WHERE AcikTarih=1" : " WHERE AcikTarih=0");
            string sqlString = SelectAllSQL(acikTarihStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }

            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            if (dataTable!=null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    CalendarEvent item = new CalendarEvent();
                    item.state = dataRow["RandevuDurumu"].ToString();

                    item.id = int.Parse(dataRow["Id"].ToString());
                    int randevuAmaci= dataRow["RandevuAmaci"].ReturnZeroIfNull().ConvertToInt();
                    item.purpose = randevuAmaci.ToString();
                    item.title = dataRow["RandevuKonusu"].ToString();
                    //item.description = item.title;
                    item.start = string.Format("{0:s}", dataRow["BaslangicTarihi"]);
                    item.end = string.Format("{0:s}", dataRow["BitisTarihi"]);
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_RANDEVU_GIRIS;
                    item.url = newUrl+ "?DestinationApp=Duzenle&RandevuId=" + item.id;
                    item.allDay = dataRow["TumGun"].ReturnFalseIfNull().ConvertToBool();
                    item.startEditable = true;
                    RenkBelirle(item);
                    if (item.state.Equals(ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI_INT.ToString()))
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
        public List<Randevu> SelectAllReturnList(string acikTarih)
        {
            string acikTarihStr = acikTarih.Equals(ProjeConstants.HEPSI) ? "" :
                (acikTarih.Equals(ProjeConstants.RANDEVU_ACIKTARIHLI) ? " WHERE AcikTarih=1" : " WHERE AcikTarih=0");
            string sqlString = SelectAllSQL(acikTarihStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);

            return list;
        }
        public List<Randevu> SelectByTarihReturnList(DateTime tarih)
        {
            DateTime bastar = new DateTime(tarih.Year,tarih.Month,tarih.Day);
            DateTime bittar = UtilityHelper.TariheSaatEkle(tarih, "23:59");
            string sqlString = string.Format(@"
                SELECT * FROM Randevu_Table
                WHERE (BaslangicTarihi <={0} AND BitisTarihi >= {1}) ORDER BY BaslangicTarihi 
            ", bittar.ReturnTRDateFormat(),bastar.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);

            return list;
        }
        public DataTable SelectAllByKatilimciRandevuReturnDataTable( int randevuId, int monthBefore)
        {
            string randevuIdStr = randevuId == ProjeConstants.HEPSI_INT ? "" : " AND A.RandevuId=" + randevuId;
            string monthBeforeStr = monthBefore == 0 ? string.Empty : string.Format("AND BaslangicTarihi > DateAdd(month, {0}, Convert(date, GetDate()))", monthBefore);
            string sqlString = string.Format(@"
                SELECT A.KatilimciTipi,A.KatilimciId, A.Id KatilimId,G.Deger RandevuYeri,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Adi
                        WHEN A.KatilimciTipi=2 THEN C.Adi
	                    WHEN A.KatilimciTipi=3 THEN E.Adi
                        WHEN A.KatilimciTipi=4 THEN F.Adi
                    ELSE C.Adi
                    END AS Adi,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Soyadi
                        WHEN A.KatilimciTipi=2 THEN C.Soyadi
	                    WHEN A.KatilimciTipi=3 THEN E.Soyadi
                        WHEN A.KatilimciTipi=4 THEN F.Soyadi
                    ELSE C.Soyadi
                    END AS Soyadi,
	                CASE
		                WHEN A.KatilimciTipi=1 THEN 'TSKGV'
                        WHEN A.KatilimciTipi=2 THEN C.Kurumu
		                WHEN A.KatilimciTipi=3 THEN 'Nakit Bağışçı'
		                WHEN A.KatilimciTipi=4 THEN 'Taşınmaz Bağışçı'
                    ELSE C.Kurumu
	                END AS Kurumu,
	                B.Id RandevuId, B.BaslangicTarihi,B.BaslangicSaati, B.BitisTarihi,B.BitisSaati,
	                B.RandevuAmaci,B.RandevuDurumu,B.RandevuKonusu,B.RandevuTipi,B.RandevuYeri RandevuYeriId,B.TumGun,B.AcikTarih
	
                FROM Randevu_Table B  
	                LEFT JOIN RandevuKatilim_Table A ON A.RandevuId=B.Id 
	                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
	                LEFT JOIN Personel_Table D ON D.Id = A.KatilimciId
                    LEFT JOIN NakitBagisci_Table E ON E.Id = A.KatilimciId
                    LEFT JOIN TasinmazBagisci_Table F ON F.Id = A.KatilimciId
                    LEFT JOIN RandevuParametre_Table G ON G.Id=B.RandevuYeri 
                WHERE B.Id IS NOT NULL
                {0}
                {1}
                ORDER BY B.BaslangicTarihi DESC, KatilimciTipi, Adi,Soyadi 
            ", randevuIdStr, monthBeforeStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectByKatilimciReturnDataTable(int katilimciId, int katilimciTipi, int randevuId)
        {
            string katilimciIdStr = katilimciId > 0 ? " AND A.RandevuId in (SELECT RandevuId FROM RandevuKatilim_Table WHERE KatilimciTipi=" + katilimciTipi + " AND KatilimciId=" + katilimciId + ")" : "";
            string randevuIdStr = randevuId > 0 ? " AND A.RandevuId=" + randevuId : "";
            string sqlString = string.Format(@"
                SELECT A.KatilimciTipi,A.KatilimciId, A.Id KatilimId,G.Deger RandevuYeri,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Adi
                        WHEN A.KatilimciTipi=2 THEN C.Adi
	                    WHEN A.KatilimciTipi=3 THEN E.Adi
                        WHEN A.KatilimciTipi=4 THEN F.Adi
                    ELSE C.Adi
                    END AS Adi,
                    CASE
	                    WHEN A.KatilimciTipi=1 THEN D.Soyadi
                        WHEN A.KatilimciTipi=2 THEN C.Soyadi
	                    WHEN A.KatilimciTipi=3 THEN E.Soyadi
                        WHEN A.KatilimciTipi=4 THEN F.Soyadi
                    ELSE C.Soyadi
                    END AS Soyadi,
	                CASE
		                WHEN A.KatilimciTipi=1 THEN 'TSKGV'
                        WHEN A.KatilimciTipi=2 THEN C.Kurumu
		                WHEN A.KatilimciTipi=3 THEN 'Nakit Bağışçı'
		                WHEN A.KatilimciTipi=4 THEN 'Taşınmaz Bağışçı'
                    ELSE C.Kurumu
	                END AS Kurumu,
	                B.Id RandevuId, B.BaslangicTarihi,B.BaslangicSaati, B.BitisTarihi,B.BitisSaati,
	                B.RandevuAmaci,B.RandevuDurumu,B.RandevuKonusu,B.RandevuTipi,B.RandevuYeri RandevuYeriId,B.TumGun,B.AcikTarih
	
                FROM Randevu_Table B  
	                LEFT JOIN RandevuKatilim_Table A ON A.RandevuId=B.Id {0}
	                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
	                LEFT JOIN Personel_Table D ON D.Id = A.KatilimciId
                    LEFT JOIN NakitBagisci_Table E ON E.Id = A.KatilimciId
                    LEFT JOIN TasinmazBagisci_Table F ON F.Id = A.KatilimciId
                    LEFT JOIN RandevuParametre_Table G ON G.Id=B.RandevuYeri 
                WHERE A.RandevuId IS NOT NULL
                {1}
                ORDER BY B.BaslangicTarihi DESC, KatilimciTipi, Adi,Soyadi 
            ", katilimciIdStr, randevuIdStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public List<Randevu> SelectByRandevuYeriId(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Randevu_Table 
                WHERE RandevuYeri={0}
                ",parametreId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Randevu> list = ToList<Randevu>(dataTable);

            return list;
        }

        public void RenkBelirle(CalendarEvent item)
        {

            switch (item.purpose)
            {
                case ProjeConstants.RANDEVU_AMACI_TOPLANTI_INT:
                    {
                        item.color = Color.Orange.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_ZIYARET_INT:
                    {
                        item.color = Color.Blue.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_ZIYARET;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_DAVET_INT:
                    {
                        item.color = Color.Green.Name;
                        item.textColor = Color.White.Name;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_YILDONUMU_INT:
                    {
                        item.color = Color.Aqua.Name;
                        item.textColor = Color.Black.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_DOGUMGUNU_INT:
                    {
                        item.color = Color.Aquamarine.Name;
                        item.textColor = Color.Black.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_DOGUMGUNU;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_OZELCALISMA_INT:
                    {
                        item.color = Color.LightBlue.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_IZIN_INT:
                    {
                        item.color = Color.Aqua.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_RESMITATIL_INT:
                    {
                        item.color = Color.MediumVioletRed.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_RESMITATIL;
                        break;
                    }
                default:
                    break;
            }
            if (item.state.Equals(ProjeConstants.RANDEVU_DURUMU_PLANLANDI_INT.ToString()))
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
                FROM Randevu_Table 
                {0}
                ORDER BY RandevuKonusu
                ", kriter);

            return sqlString;
        }

        public override int GetHashCode()
        {
            int hashCode = 477006145;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RandevuTipi);
            hashCode = hashCode * -1521134295 + RandevuAmaci.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RandevuKonusu);
            hashCode = hashCode * -1521134295 + RandevuYeri.GetHashCode();
            hashCode = hashCode * -1521134295 + RandevuDurumu.GetHashCode();
            hashCode = hashCode * -1521134295 + TumGun.GetHashCode();
            hashCode = hashCode * -1521134295 + AcikTarih.GetHashCode();
            hashCode = hashCode * -1521134295 + IcIrtibatId.GetHashCode();
            hashCode = hashCode * -1521134295 + DisIrtibatId.GetHashCode();
            hashCode = hashCode * -1521134295 + BaslangicTarihi.GetHashCode();
            hashCode = hashCode * -1521134295 + BitisTarihi.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaslangicSaati);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BitisSaati);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Aciklama);
            return hashCode;
        }

        public static bool operator ==(Randevu left, Randevu right)
        {
            return EqualityComparer<Randevu>.Default.Equals(left, right);
        }

        public static bool operator !=(Randevu left, Randevu right)
        {
            return !(left == right);
        }
    }
}