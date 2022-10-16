using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<AniObjesiDagitim> genericEntity = new GenericEntity<AniObjesiDagitim>(ProjeConstants.SQL_DELETE);
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

        public bool Delete(int randevuId, int katilimciId, int katilimciTipi, string aniObjesiIdStr)
        {
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
                    AND AniObjesiId IN ({3})", randevuId, katilimciId, katilimciTipi, aniObjesiIdStr);
            bool isDeleted = dao.DeleteFromDb(sqlString, "RandevuId="+randevuId+" KatilimciId="+katilimciId+" AniObjesiId="+aniObjesiIdStr);
            return isDeleted;
        }

        public bool Delete(int randevuId, int katilimciId, int katilimciTipi)
        {
            string sqlString = string.Format(@"
                DELETE FROM AniObjesiDagitim_Table
                WHERE RandevuId={0} AND KatilimciId={1} AND KatilimciTipi={2}
            ", randevuId, katilimciId, katilimciTipi);
            bool isDeleted = dao.DeleteFromDb(sqlString, "");
            return isDeleted;
        }
    }
}
