using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class Toplanti : ParentClass
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

        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Toplanti> genericEntity = new GenericEntity<Toplanti>(ProjeConstants.SQL_DELETE);
                    OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    bool isDeleted = dao.DeleteFromDb(sqlString, "");
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
            try
            {
                GenericEntity<Toplanti> genericEntity = new GenericEntity<Toplanti>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public override T Select<T>(int id)
        {
            GenericEntity<Toplanti> genericEntity = new GenericEntity<Toplanti>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);
            Toplanti item = new Toplanti();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Toplanti_Table ORDER BY ToplantiKonusu
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Toplanti> genericEntity = new GenericEntity<Toplanti>(ProjeConstants.SQL_UPDATE);
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
        public List<Toplanti> SelectByToplantiYeri(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Toplanti_Table 
                WHERE ToplantiYeri={0}
                ", parametreId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);

            return list;
        }
        public Toplanti Select(int id)
        {
            GenericEntity<Toplanti> genericEntity = new GenericEntity<Toplanti>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);
            Toplanti item = new Toplanti();
            item = list.FirstOrDefault();
            return item;
        }
        public Toplanti SelectByBaslangicTarihi(int toplantiYeri, DateTime baslangicTarihi)
        {
            string sqlString= string.Format(@"
                SELECT * FROM Toplanti_Table
                WHERE ToplantiYeri={0} AND
                    BaslangicTarihi <= {1} AND 
                BitisTarihi >= {1}  
            ", toplantiYeri, baslangicTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);
            Toplanti item = new Toplanti();
            item = list.FirstOrDefault();
            return item;
        }
        public Toplanti SelectByBitisTarihi(int toplantiYeri, DateTime bitisTarihi)
        {
            string sqlString = string.Format(@"
                SELECT * FROM Toplanti_Table
                WHERE ToplantiYeri!={0} AND
                    ToplantiYeri={1} AND
                    BitisTarihi >= {2} AND 
                    BaslangicTarihi <= {2}  
            ", ProjeConstants.PARAM_DIGER_INT, toplantiYeri, bitisTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);
            Toplanti item = new Toplanti();
            item = list.FirstOrDefault();
            return item;
        }
        public DataTable SelectAllByKatilimciToplantiReturnDataTable(int toplantiId, string aktif)
        {
            string aktifStr = aktif.Equals(ProjeConstants.TOPLANTI_AKTIF) ? " WHERE BitisTarihi > GETDATE() " :
               (aktif.Equals(ProjeConstants.TOPLANTI_PASIF) ? " WHERE BitisTarihi < GETDATE()" : string.Empty);

            string toplantiIdStr = toplantiId == ProjeConstants.HEPSI_INT ? "" : 
                (string.IsNullOrEmpty(aktifStr)? " WHERE " : " AND ") + string.Format(" B.ToplantiId={0}", toplantiId);
           
            string sqlString = string.Format(@"
                SELECT  A.Id ToplantiId, A.*,
                B.KatilimciId, B.Id KatilimId, B.Bilgi, 
                    C.Adi, C.Soyadi,
                    D.ProtokolSiraNo 
                FROM Toplanti_Table A  
	                LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId
	                LEFT JOIN Personel_Table C ON C.Id = B.KatilimciId
					LEFT JOIN IsBilgileri_Table D ON D.PersonelId = C.Id
                {0}
                {1}
                ORDER BY  A.Id, ProtokolSiraNo, Adi,Soyadi 
            ", aktifStr, toplantiIdStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectAllByKatilimciToplantiTarihReturnDataTable(int toplantiId, string aktif, DateTime ilktar, DateTime sontar)
        {
            string aktifStr = aktif.Equals(ProjeConstants.TOPLANTI_AKTIF) ? " AND BitisTarihi > GETDATE() " :
               (aktif.Equals(ProjeConstants.TOPLANTI_PASIF) ? " AND BitisTarihi < GETDATE()" : string.Empty);

            string toplantiIdStr = toplantiId == ProjeConstants.HEPSI_INT ? "" :
                 string.Format(" AND B.ToplantiId={0}", toplantiId);

            string sqlString = string.Format(@"
                SELECT C.Adi, C.Soyadi, B.KatilimciId,B.Bilgi, A.Id ToplantiId, B.Id KatilimId, D.ProtokolSiraNo , 
                    A.*         
                FROM Toplanti_Table A  
	                LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId
	                LEFT JOIN Personel_Table C ON C.Id = B.KatilimciId
					LEFT JOIN IsBilgileri_Table D ON D.PersonelId = C.Id
                WHERE A.BitisTarihi >= {0} AND A.BaslangicTarihi <= {1}
                {2}
                {3}
                ORDER BY  A.Id, ProtokolSiraNo, Adi,Soyadi 
            ", ilktar.ReturnTRDateFormat(), sontar.ReturnTRDateFormat(), aktifStr, toplantiIdStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectAllByKatilimci(int katilimciId)
        {
            string sqlString = string.Format(@"
                SELECT C.Adi, C.Soyadi, B.KatilimciId, A.Id ToplantiId, B.Id KatilimId , D.ProtokolSiraNo , 
                    A.*         
                FROM Toplanti_Table A  
	                LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId
	                LEFT JOIN Personel_Table C ON C.Id = B.KatilimciId
					LEFT JOIN IsBilgileri_Table D ON D.PersonelId = C.Id
                WHERE B.KatilimciId = {0}
                ORDER BY  A.Id, ProtokolSiraNo, Adi,Soyadi 
            ", katilimciId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public List<Toplanti> SelectByKatilimciTarih(int katilimciId, DateTime tarih)
        {
            DateTime bastar = new DateTime(tarih.Year, tarih.Month, tarih.Day);
            DateTime bittar = UtilityHelper.TariheSaatEkle(tarih, "23:59");
            string sqlString = string.Format(@"
                SELECT C.Adi, C.Soyadi, B.KatilimciId, A.Id ToplantiId, B.Id KatilimId , D.ProtokolSiraNo , 
                    A.*         
                FROM Toplanti_Table A  
	                LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId
	                LEFT JOIN Personel_Table C ON C.Id = B.KatilimciId
					LEFT JOIN IsBilgileri_Table D ON D.PersonelId = C.Id
                WHERE B.KatilimciId = {0} AND (BaslangicTarihi <={1} AND BitisTarihi >= {2}) ORDER BY BaslangicTarihi  
            ", katilimciId, bittar.ReturnTRDateFormat(), bastar.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Toplanti> list = ToList<Toplanti>(dataTable);
            return list;
        }
        public string SelectAllReturnJson()
        {

            string sqlString = @"
                SELECT * 
                FROM Toplanti_Table
                ";
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
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    CalendarEvent item = new CalendarEvent();

                    item.id = int.Parse(dataRow["Id"].ToString());

                    item.title = dataRow["ToplantiKonusu"].ToString();
                    //item.description = item.title;
                    item.start = string.Format("{0:s}", dataRow["BaslangicTarihi"]);
                    item.end = string.Format("{0:s}", dataRow["BitisTarihi"]);
                    int toplantiYeri= dataRow["ToplantiYeri"].ConvertToInt();
                    if (toplantiYeri==1) {
                        item.color = Color.Red.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                    }
                    else if(toplantiYeri == 2) {
                        
                        item.color = Color.Orange.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                    }
                    else if (toplantiYeri == 3)
                    {

                        item.color = Color.Green.Name;
                        item.textColor = Color.White.Name;
                        item.purpose = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                    }
                    eventItems.Add(item);
                }
            }
            string json = ToJSON(eventItems);
            return json;
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
    }
}
