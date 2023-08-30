using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class AniObjesiDagitim : ParentClass
    {
        public int AniObjesiId { get; set; }
        public int Adet { get; set; }
        public int KatilimciId { get; set; }
        public int KatilimciTipi { get; set; }
        public int RandevuId { get; set; }
        public int VerilenAlinan { get; set; } //verilen 0; alinan 1
        public int DagitimYeriTanimId { get; set; } = 0;
        public int CikisDepoId { get; set; } = 0;
        public string GetirilenAniObjesi { get; set; }
        public string Aciklama { get; set; }
        public DateTime VerilisTarihi { get; set; }
        
        public override int Save()
        {
            try
            {
                GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
                }
                this.Id = id;
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
                    AniObjesiDagitim item = Select<AniObjesiDagitim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
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
                bool isDeleted=false;
                if (Id != 0)
                {
                    GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    AniObjesiDagitim item = Select<AniObjesiDagitim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AniObjesiDagitim Select(int id)
        {
            GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return item;
        }
        public int Delete(int randevuId, int katilimciId, int katilimciTipi, string aniObjesiIdList="")
        {
            int deleted;
            string aniObjesiIdListStr = string.IsNullOrEmpty(aniObjesiIdList) ? string.Empty : string.Format(" AND AniObjesiId IN ({0})", aniObjesiIdList);
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                {3}
            ", randevuId, katilimciId, katilimciTipi, aniObjesiIdListStr);


            string wherestr = string.Format("WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2} {3}", randevuId, katilimciId, katilimciTipi, aniObjesiIdListStr);
            GenericEntity<AniObjesiDagitim> genericEntitySelect = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            string sqlStringSelect = genericEntitySelect.GetQuery(this, wherestr);
            DataTable dataTable = dao.SelectFromDb(sqlStringSelect, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            
            if (list.Count > 0)
                deleted = dao.DeleteFromDb(sqlString, "", true);
            else deleted = 0;
            if (deleted > 0 && ProjeConstants.MTS_DELETE_LOG)
            {
                foreach (AniObjesiDagitim item in list)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
                }
            }

            return deleted;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table ORDER BY RandevuId 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public AniObjesiDagitim SelectGetirilenAniObjesi(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2} AND VerilenAlinan={3}
                ORDER BY Id
                ", randevuId, katilimciId, katilimciTipi, ProjeConstants.ANIOBJESI_GETIRILEN_INT);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return item;
        }
        public DataTable SelectReturnDT(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId, B.Adet, B.RandevuId,B.KatilimciId,B.KatilimciTipi
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                
                ORDER BY A.Id
                ", randevuId, katilimciId, katilimciTipi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectReturnDT(int randevuId, int katilimciId, int katilimciTipi,string stokluMu)
        {
            string stokStr=string.IsNullOrEmpty(stokluMu)?string.Empty:" WHERE A.StokluMu="+stokluMu.ReturnQuotedValue();
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId, B.Adet, B.RandevuId,B.KatilimciId,B.KatilimciTipi
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                {3}
                ORDER BY A.Id
                ", randevuId, katilimciId, katilimciTipi,stokStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public List<AniObjesiDagitim> SelectStoksuzAniObjeleriReturnList(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.RandevuId, B.KatilimciId,B.KatilimciTipi,B.VerilenAlinan
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                WHERE A.StokluMu={3}
                ", randevuId, katilimciId, katilimciTipi,ProjeConstants.MTS_ANIOBJESISTOKSUZ.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public AniObjesiDagitim Select(int randevuId, int katilimciId, int katilimciTipi, int aniObjesiId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table  
                WHERE AniObjesiId={0} AND RandevuId={1} AND KatilimciId={2} AND KatilimciTipi={3}
                ", aniObjesiId,randevuId, katilimciId, katilimciTipi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list.FirstOrDefault();
        }
        public List<AniObjesiDagitim> SelectByKisiIdReturnList(int katilimciId, int katilimciTipi, string verilenGetirilen )
        {
            string verilenGetirilenStr = verilenGetirilen.Equals(ProjeConstants.ANIOBJESI_VERILENGETIRILEN) ? string.Empty :
                verilenGetirilenStr = " AND B.VerilenAlinan=" + verilenGetirilen;
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.RandevuId, B.KatilimciId,B.KatilimciTipi,B.VerilenAlinan
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND KatilimciId={0} AND KatilimciTipi={1}
                WHERE 1>0 
                {2}
                ", katilimciId, katilimciTipi, verilenGetirilenStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public string SelectByKatilimcidKatilimciTipiRandevuId(int katilimciId, int katilimciTipi, int randevuId)
        {
            string retval = string.Empty;
            string sqlString = string.Format(@"
                SELECT  B.Adi, A.Adet  FROM AniObjesiDagitim_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id=A.AniObjesiId
                WHERE KatilimciId={0} AND KatilimciTipi={1} AND RandevuId={2}                
                ", katilimciId, katilimciTipi, randevuId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null )
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string aniObjesi = row["Adi"].ReturnEmptyIfNull().ToString();
                    string adet = row["Adet"].ReturnZeroIfNull().ToString();
                    retval += !string.IsNullOrEmpty(aniObjesi) ? aniObjesi + "(" + adet + "), " : string.Empty;
                }
            }
            return retval;
        }
        public string SelectGetirilenByKatilimcidKatilimciTipiRandevuId(int katilimciId, int katilimciTipi, int randevuId)
        {
            string retval = string.Empty;
            string sqlString = string.Format(@"
                SELECT  GetirilenAniObjesi  FROM AniObjesiDagitim_Table A
                WHERE KatilimciId={0} AND KatilimciTipi={1} AND RandevuId={2} AND AniObjesiId={3}
                ", katilimciId, katilimciTipi, randevuId, ProjeConstants.GETIRILEN_ANIOBJESIID_INT);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string aniObjesi = row["GetirilenAniObjesi"].ReturnEmptyIfNull().ToString();
                    retval += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi  : string.Empty;
                }
            }
            return retval;
        }
        public DataTable SelectByKatilimcidKatilimciTipiRandevuId(int katilimciId, int katilimciTipi, int randevuId,string stokluMu)
        {
            string retval = string.Empty;
            string stokStr = string.IsNullOrEmpty(stokluMu) ? string.Empty :" AND StokluMu="+stokluMu.ReturnQuotedValue();
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiDagitimId, B.Id AniObjesiId, B.Adi, A.Adet, A.CikisDepoId  FROM AniObjesiDagitim_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id=A.AniObjesiId
                WHERE KatilimciId={0} AND KatilimciTipi={1} AND RandevuId={2}
                    {3}
                ", katilimciId, katilimciTipi, randevuId,stokStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            //if (dataTable != null)
            //{
            //    foreach (DataRow row in dataTable.Rows)
            //    {
            //        string aniObjesi = row["Adi"].ReturnEmptyIfNull().ToString();
            //        string adet = row["Adet"].ReturnZeroIfNull().ToString();
            //        retval += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi + "(" + adet + "), " : string.Empty;
            //    }
            //}
            return dataTable;
        }
        public List<AniObjesiDagitim> SelectByAniObjesiId(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE AniObjesiId={0}
                ", parametreId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return list;
        }
        public List<AniObjesiDagitim> SelectByRandevuId(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE RandevuId={0}
                ", parametreId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return list;
        }
        private void WriteToTraceFile(string adim)
        {
            Trace.TraceInformation("Tarih Saat = " + DateTime.Now.ConvertToDDMMYYYHHmmFormat());
            Trace.TraceInformation("-----------------------------------------------------------------------");
            GenericEntity<AniObjesiDagitim> genericEntitySelect = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            string sqlStringSelect = genericEntitySelect.GetQuery(this);
            Trace.TraceInformation("Adım "+adim+" Sorgu =" + sqlStringSelect);
            DataTable dataTable = dao.SelectFromDb(sqlStringSelect, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            string json = ToJSON(list);
            Trace.TraceInformation("Adım "+adim+"  json =" + json);
            // You must close or flush the trace to empty the output buffer.
            Trace.Flush();
        }
        public string ToJSON(List<AniObjesiDagitim> list)
        {
            string json = "[]";
            try
            {
                if (list != null)
                {

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    foreach (var item in list)
                    {
                        childRow = new Dictionary<string, object>();
                        childRow.Add("id", item.Id);
                        childRow.Add("Aciklama", item.Aciklama);
                        childRow.Add("Adet", item.Adet);
                        childRow.Add("VerilisTarihi", item.VerilisTarihi);
                        childRow.Add("KatilimciId", item.KatilimciId);
                        childRow.Add("KatilimciTipi", item.KatilimciTipi);
                        childRow.Add("Olusturan", item.Olusturan);
                        childRow.Add("OlusturmaTarihi", item.OlusturmaTarihi);
                        childRow.Add("RandevuId", item.RandevuId);
                        childRow.Add("VerilenAlinan", item.VerilenAlinan);
                        childRow.Add("Degistiren", item.Degistiren);
                        childRow.Add("DegistirmeTarihi", item.DegistirmeTarihi);
                        childRow.Add("VerilenAlinan", item.VerilenAlinan);

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
