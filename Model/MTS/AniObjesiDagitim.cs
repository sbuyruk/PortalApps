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
        public int FaaliyetId { get; set; }
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
                throw;
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
        public int Delete(int faaliyetId, int katilimciId, string aniObjesiIdList="")
        {
            int deleted;
            string aniObjesiIdListStr = string.IsNullOrEmpty(aniObjesiIdList) ? string.Empty : string.Format(" AND AniObjesiId IN ({0})", aniObjesiIdList);
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE FaaliyetId={0} AND KatilimciId={1}
                {2}
            ", faaliyetId, katilimciId, aniObjesiIdListStr);


            string wherestr = string.Format("WHERE FaaliyetId={0} AND KatilimciId={1} {2}", faaliyetId, katilimciId, aniObjesiIdListStr);
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
                FROM AniObjesiDagitim_Table ORDER BY FaaliyetId 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public AniObjesiDagitim SelectGetirilenAniObjesi(int faaliyetId, int katilimciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE FaaliyetId={0} AND KatilimciId={1} AND VerilenAlinan={2}
                ORDER BY Id
                ", faaliyetId, katilimciId, ProjeConstants.ANIOBJESI_GETIRILEN_INT);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            AniObjesiDagitim item = new AniObjesiDagitim();
            item = list.FirstOrDefault();
            return item;
        }
        public DataTable SelectReturnDT(int faaliyetId, int katilimciId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId, B.Adet, B.FaaliyetId,B.KatilimciId
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND FaaliyetId={0} AND KatilimciId={1}
                ORDER BY A.Id
                ", faaliyetId, katilimciId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        
        public DataTable SelectByFaaliyetIdKatilimciIdStokReturnDT(int faaliyetId, int katilimciId, string stokluMu)
        {
            string stokStr=string.IsNullOrEmpty(stokluMu)?string.Empty:" WHERE A.StokluMu="+stokluMu.ReturnQuotedValue();
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId, B.Adet, B.FaaliyetId,B.KatilimciId
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND FaaliyetId={0} AND KatilimciId={1} 
                {2}
                ORDER BY A.Id
                ", faaliyetId, katilimciId,stokStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public List<AniObjesiDagitim> SelectStoksuzAniObjeleriReturnList(int faaliyetId, int katilimciId)
        {
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.FaaliyetId, B.KatilimciId
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND FaaliyetId={0} AND KatilimciId={1} 
                WHERE A.StokluMu={2}
                ", faaliyetId, katilimciId, ProjeConstants.MTS_ANIOBJESISTOKSUZ.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public AniObjesiDagitim Select(int faaliyetId, int katilimciId, int aniObjesiId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table  
                WHERE AniObjesiId={0} AND FaaliyetId={1} AND KatilimciId={2} 
                ", aniObjesiId,faaliyetId, katilimciId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list.FirstOrDefault();
        }
        public List<AniObjesiDagitim> SelectByKisiIdReturnList(int katilimciId, string verilenGetirilen )
        {
            string verilenGetirilenStr = verilenGetirilen.Equals(ProjeConstants.ANIOBJESI_VERILENGETIRILEN) ? string.Empty :
                verilenGetirilenStr = " AND B.VerilenAlinan=" + verilenGetirilen;
            string sqlString = string.Format(@"
                SELECT B.Id, A.Id AniObjesiId, IsNull(B.Adet,0) Adet ,b.FaaliyetId, B.KatilimciId,B.VerilenAlinan
                FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id AND KatilimciId={0} 
                WHERE 1>0 
                {1}
                ", katilimciId, verilenGetirilenStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);
            return list;
        }
        public string SelectGetirilenByKatilimcidFaaliyetId(int katilimciId, int faaliyetId)
        {
            string retval = string.Empty;
            string sqlString = string.Format(@"
                SELECT  GetirilenAniObjesi  FROM AniObjesiDagitim_Table A
                WHERE KatilimciId={0} AND FaaliyetId={1} AND AniObjesiId={2}
                ", katilimciId, faaliyetId, ProjeConstants.GETIRILEN_ANIOBJESIID_INT);
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
        public DataTable SelectByKatilimcidFaaliyetId(int katilimciId, int faaliyetId,string stokluMu)
        {
            string retval = string.Empty;
            string stokStr = string.IsNullOrEmpty(stokluMu) ? string.Empty :" AND StokluMu="+stokluMu.ReturnQuotedValue();
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiDagitimId, B.Id AniObjesiId, B.Adi, A.Adet, A.CikisDepoId  FROM AniObjesiDagitim_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id=A.AniObjesiId
                WHERE KatilimciId={0} AND FaaliyetId={1}
                    {2}
                ", katilimciId, faaliyetId,stokStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public List<AniObjesiDagitim> SelectByFaaliyetId(int parametreId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiDagitim_Table 
                WHERE FaaliyetId={0}
                ", parametreId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiDagitim> list = ToList<AniObjesiDagitim>(dataTable);

            return list;
        }
    }
}
