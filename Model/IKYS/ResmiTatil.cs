using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class ResmiTatil : ParentClass
    {
        public int Gun { get; set; }
        public int Ay { get; set; }
        public int Yil { get; set; }
        public string Tatil { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime IlanTarihi { get; set; }
        public DateTime IptalTarihi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ResmiTatil_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ResmiTatil> list = ToList<ResmiTatil>(dataTable);
            ResmiTatil il = new ResmiTatil();
            il = list.FirstOrDefault();
            return (T)Convert.ChangeType(il, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ResmiTatil_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ResmiTatil> list = ToList<ResmiTatil>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = string.Format(@"
                SELECT Id ResimiTatilId, Gun,Ay,Yil,Tatil,BaslamaTarihi,BitisTarihi,IlanTarihi,IptalTarihi 
                FROM ResmiTatil_Table
                ORDER BY BaslamaTarihi DESC");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<ResmiTatil> genericEntity = new GenericEntity<ResmiTatil>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    ResmiTatil item = Select<ResmiTatil>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<ResmiTatil> genericEntity = new GenericEntity<ResmiTatil>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL);
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
                    GenericEntity<ResmiTatil> genericEntity = new GenericEntity<ResmiTatil>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    ResmiTatil item = Select<ResmiTatil>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_RESMITATIL);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO ResmiTatil_Table 
                                        (Gun,Ay,Yil,Tatil,BaslamaTarihi,BitisTarihi,IlanTarihi,IptalTarihi, Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9}) ",
                                    Gun.ReturnQuotedValue(), Ay.ReturnQuotedValue(), Yil.ReturnQuotedValue(), Tatil.ReturnQuotedValue(),
                                    BaslamaTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(),
                                    string.IsNullOrEmpty(IlanTarihi.ConvertToDatetimeEmptyIfNull()) ? "null" : IlanTarihi.ReturnTRDateFormat(),
                                    string.IsNullOrEmpty(IptalTarihi.ConvertToDatetimeEmptyIfNull()) ? "null" : IptalTarihi.ReturnTRDateFormat(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE ResmiTatil_Table  
                                    SET Gun={0}, Ay={1}, Yil={2}, Tatil={3}, BaslamaTarihi={4}, BitisTarihi={5}, IlanTarihi={6}, IptalTarihi={7}, 
                                        Degistiren={8},DegistirmeTarihi={9}
                                    WHERE Id= {10}",
                                    Gun.ReturnQuotedValue(), Ay.ReturnQuotedValue(), Yil.ReturnQuotedValue(), Tatil.ReturnQuotedValue(),
                                    BaslamaTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(),
                                    string.IsNullOrEmpty(IlanTarihi.ConvertToDatetimeEmptyIfNull()) ? "null" : IlanTarihi.ReturnTRDateFormat(),
                                    string.IsNullOrEmpty(IptalTarihi.ConvertToDatetimeEmptyIfNull()) ? "null" : IptalTarihi.ReturnTRDateFormat(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        public List<ResmiTatil> SelectByTarih(DateTime basTar, DateTime bitTar)
        {

            string sqlString = string.Format(@"
                SELECT *
                FROM ResmiTatil_Table
                WHERE Yil=0 OR BaslamaTarihi BETWEEN {0} AND {1} 
                ORDER BY BaslamaTarihi --, MONTH(BaslamaTarihi), DAY(BaslamaTarihi)
                ", basTar.ReturnTRDateFormat(),bitTar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ResmiTatil> list = ToList<ResmiTatil>(dataTable);
            return list;
        }
        
        public List<ResmiTatil> SelectBySonIkiYil()
        {
            DateTime basTar = new DateTime(DateTime.Today.AddYears(-1).Year,1,1);//geçen yılbaşı
            DateTime bitTar = basTar.AddYears(2).AddDays(1);
            string sqlString = string.Format(@"
                SELECT *
                FROM ResmiTatil_Table
                WHERE Yil=0 OR BaslamaTarihi BETWEEN {0} AND {1} 
                ORDER BY MONTH(BaslamaTarihi), DAY(BaslamaTarihi)", basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ResmiTatil> list = ToList<ResmiTatil>(dataTable);
            return list;
        }

        public List<ResmiTatil> SelectByYil(int yil)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM ResmiTatil_Table
                WHERE Yil=0 OR Yil={0} 
                ORDER BY MONTH(BaslamaTarihi), DAY(BaslamaTarihi)", yil);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ResmiTatil> list = ToList<ResmiTatil>(dataTable);
            return list;
        }
        public bool ResmiTatilMi(DateTime tarih)
        {
            int gun = tarih.Day;
            int ay = tarih.Month;

            //string sqlString = string.Format(@"SELECT *
            //                                    FROM ResmiTatil_Table
            //                                    WHERE (Gun={0} AND Ay={1} AND Yil=0)
            //                                    OR (BaslamaTarihi<={2} AND BitisTarihi>={2})", gun,ay, tarih.ReturnTRDateFormat());
            string sqlString = string.Format(@"SELECT *
                                                FROM ResmiTatil_Table
                                                WHERE ((DAY(BaslamaTarihi)<={0} AND DAY(BitisTarihi)>={0} AND Ay={1} AND Yil=0) --Gun={0}
                                                        AND (IlanTarihi is null OR IlanTarihi<={2}) AND (IptalTarihi is null OR IptalTarihi>={2}))
                                                        OR ((BaslamaTarihi<={2} AND BitisTarihi>={2}))", gun, ay, tarih.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
                return true;
            return false;
        }

        public string SelectAllReturnJson(DateTime basTar, DateTime bitTar)
        {

            ResmiTatil resmiTatilDao = new ResmiTatil();
            List<ResmiTatil> list = resmiTatilDao.SelectByTarih(basTar, bitTar);
            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            Randevu randevu = new Randevu();

            foreach (ResmiTatil resmiTatil in list)
            {

               

                if (resmiTatil.BaslamaTarihi.Year < 1900)
                {
                    for (int i = 0; i < (bitTar.Year - basTar.Year); i++)
                    {
                        string bassaat = resmiTatil.BaslamaTarihi.Hour + ":" + resmiTatil.BaslamaTarihi.Minute;
                        string bitsaat = resmiTatil.BitisTarihi.Hour + ":" + resmiTatil.BitisTarihi.Minute;

                        resmiTatil.BaslamaTarihi = new DateTime(basTar.Year +i , resmiTatil.BaslamaTarihi.Month, resmiTatil.BaslamaTarihi.Day);
                        resmiTatil.BaslamaTarihi = UtilityHelper.TariheSaatEkle(resmiTatil.BaslamaTarihi, bassaat);

                        resmiTatil.BitisTarihi = new DateTime(basTar.Year + i, resmiTatil.BitisTarihi.Month, resmiTatil.BitisTarihi.Day);
                        resmiTatil.BitisTarihi = UtilityHelper.TariheSaatEkle(resmiTatil.BitisTarihi, bitsaat);

                        CalendarEvent item = new CalendarEvent();
                        item.state = ProjeConstants.RANDEVU_DURUMU_ONAYLANDI_INT.ToString();
                        item.id = 999;//999 önemli taşınamayan event
                        item.purpose = ProjeConstants.RANDEVU_AMACI_RESMITATIL_INT;
                        item.title = resmiTatil.Tatil;
                        //item.description = resmiTatil.Tatil;
                        item.start = string.Format("{0:s}", resmiTatil.BaslamaTarihi);
                        item.end = string.Format("{0:s}", resmiTatil.BitisTarihi);
                        item.url = "";
                        if (resmiTatil.Yil < 1900)
                            item.allDay = true;
                        if (resmiTatil.BitisTarihi.Day > resmiTatil.BaslamaTarihi.Day)
                        {
                            item.allDay = false;
                        }
                            
                        item.startEditable = false;

                        randevu.RenkBelirle(item);
                        eventItems.Add(item);
                    }
                }
                else
                {
                    CalendarEvent item = new CalendarEvent();
                    item.state = ProjeConstants.RANDEVU_DURUMU_ONAYLANDI_INT.ToString();
                    item.id = 999;//999 önemli taşınamayan event
                    item.purpose = ProjeConstants.RANDEVU_AMACI_RESMITATIL_INT;
                    item.title = resmiTatil.Tatil;
                    //item.description = resmiTatil.Tatil;
                    item.start = string.Format("{0:s}", resmiTatil.BaslamaTarihi);
                    item.end = string.Format("{0:s}", resmiTatil.BitisTarihi);
                    item.url = "";
                    if (resmiTatil.Yil < 1900)
                        item.allDay = true;
                    item.startEditable = false;

                    randevu.RenkBelirle(item);
                    eventItems.Add(item);
                }

                
            }
            string json = randevu.ToJSON(eventItems);
            return json;
        }
    }
}
