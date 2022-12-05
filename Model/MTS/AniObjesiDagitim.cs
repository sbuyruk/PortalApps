using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
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
        public string GetirilenAniObjesi { get; set; }
        public string Aciklama { get; set; }
        
        public override int Save()
        {
            try
            {
                GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_INSERT);
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
        public AniObjesiDagitim Select(int id)
        {
            GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return item;
        }
        public AniObjesiDagitim SelectGetirilenAniObjesi(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2} AND VerilenAlinan={3}
                ORDER BY Id
                ", randevuId, katilimciId, katilimciTipi, ProjeConstants.ANIOBJESI_GETIRILEN_INT);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return item;
        }
        public DataTable SelectReturnDT(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId, A.Sira, A.Deger, B.Id AniObjesiDagitimId, B.Adet, B.RandevuId,B.KatilimciId,B.KatilimciTipi
                FROM RandevuParametre_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                WHERE Grup= {3} 
                ORDER BY A.Id
                ", randevuId, katilimciId, katilimciTipi, ProjeConstants.PARAM_ANIOBJESI.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public List<AniObjesiDagitim> SelectReturnList(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.RandevuId, B.KatilimciId,B.KatilimciTipi,B.VerilenAlinan
                FROM RandevuParametre_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                WHERE Grup= {3} 
                ", randevuId, katilimciId, katilimciTipi, ProjeConstants.PARAM_ANIOBJESI.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public List<AniObjesiDagitim> SelectByKisiIdReturnList(int katilimciId, int katilimciTipi, string verilenGetirilen )
        {
            string verilenGetirilenStr = verilenGetirilen.Equals(ProjeConstants.ANIOBJESI_VERILENGETIRILEN) ? string.Empty :
                verilenGetirilenStr = " AND B.VerilenAlinan=" + verilenGetirilen;
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.RandevuId, B.KatilimciId,B.KatilimciTipi,B.VerilenAlinan
                FROM RandevuParametre_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND KatilimciId={0} AND KatilimciTipi={1}
                WHERE Grup= {2} 
                {3}
                ", katilimciId, katilimciTipi, ProjeConstants.PARAM_ANIOBJESI.ReturnQuotedValue(),verilenGetirilenStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public string SelectByKatilimcidKatilimciTipiRandevuId(int katilimciId, int katilimciTipi, int randevuId)
        {
            string retval = string.Empty;
            string sqlString = string.Format(@"
                SELECT  B.Deger, A.Adet  FROM AniObjesiDagitim_Table A
                INNER JOIN RandevuParametre_Table B ON B.Grup={0} AND B.Id=A.AniObjesiId
                WHERE KatilimciId={1} AND KatilimciTipi={2} AND RandevuId={3}                
                ", ProjeConstants.PARAM_ANIOBJESI.ReturnQuotedValue(), katilimciId, katilimciTipi, randevuId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null )
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string aniObjesi = row["Deger"].ReturnEmptyIfNull().ToString();
                    string adet = row["Adet"].ReturnZeroIfNull().ToString();
                    retval += !string.IsNullOrEmpty(aniObjesi) ? aniObjesi + "(" + adet + "), " : string.Empty;
                }
            }
            return retval;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_UPDATE);
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
        public List<AniObjesiDagitim> SelectByAniObjesiId(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE AniObjesiId={0}
                ", parametreId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return list;
        }
        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    WriteToTraceFile("Delete1");
                    GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_DELETE);
                    OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    bool isDeleted = dao.DeleteFromDb(sqlString, "");
                    if (isDeleted)
                    {
                        Trace.TraceInformation("Adım DELETE1 Silme başarılı mı = " + isDeleted.ConvertToBool().ToString() + " DeleteStr=" + sqlString, ProjeConstants.MESAJ_BILGI);
                        // You must close or flush the trace to empty the output buffer.
                        Trace.Flush();
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

        private void WriteToTraceFile(string adim)
        {
            Trace.TraceInformation("Tarih Saat = " + DateTime.Now.ConvertToDDMMYYYHHmmFormat());
            Trace.TraceInformation("-----------------------------------------------------------------------");
            GenericEntity<AniObjesiDagitim> genericEntitySelect = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_SELECT);
            string sqlStringSelect = genericEntitySelect.GetQuery(this);
            Trace.TraceInformation("Adım "+adim+" Sorgu =" + sqlStringSelect);
            DataTable dataTable = dao.selectFromDb(sqlStringSelect, "");
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
        public int Delete(int randevuId, int katilimciId, int katilimciTipi, string aniObjesiIdStr)
        {
            WriteToTraceFile("Delete2");
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                    AND AniObjesiId IN ({3})", randevuId, katilimciId, katilimciTipi, aniObjesiIdStr);
            int deleted = dao.DeleteFromDb(sqlString, "RandevuId="+randevuId+" KatilimciId="+katilimciId+" AniObjesiId="+aniObjesiIdStr, true);
            if (deleted > 0)
            {
                Trace.TraceInformation("Adım DELETE2 Silme başarılı mı = " + deleted.ToString() + " DeleteStr=" + sqlString, ProjeConstants.MESAJ_BILGI);
                // You must close or flush the trace to empty the output buffer.
                Trace.Flush();
            }

            return deleted;
        }
        public int Delete(int randevuId, int katilimciId, int katilimciTipi)
        {
            WriteToTraceFile("Delete3");
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
            ", randevuId, katilimciId, katilimciTipi);
            int deleted = dao.DeleteFromDb(sqlString, "", true);
            if (deleted > 0)
            {
                Trace.TraceInformation("Adım DELETE3 Silinen adet = " + deleted.ToString() + " DeleteStr=" + sqlString, ProjeConstants.MESAJ_BILGI);
                // You must close or flush the trace to empty the output buffer.
                Trace.Flush();
            }
            return deleted;
        }
    }
}
