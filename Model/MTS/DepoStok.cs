using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using DAO.Ortak;
using Model.Ortak;
using Model.MTS;
using System.Linq;
using Utility.ProjeGlobal;
using System.Data;
using Utility.HelperClasses;

namespace Model.MTS
{
    public class DepoStok : ParentClass
    {
        public int AniObjesiId { get; set; }
        public int DepoId { get; set; }
        [Required]
        public int SonAdet { get; set; } = 0;
        [DataType(DataType.DateTime)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SonIslemTarihi { get; set; } = DateTime.Now;
        public string SonIslemYapan { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<DepoStok> genericEntity = new GenericEntity<DepoStok>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
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
                    DepoStok item = Select<DepoStok>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<DepoStok> genericEntity = new GenericEntity<DepoStok>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
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
                    GenericEntity<DepoStok> genericEntity = new GenericEntity<DepoStok>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    DepoStok item = Select<DepoStok>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DepoStok Select(int id)
        {
            GenericEntity<DepoStok> genericEntity = new GenericEntity<DepoStok>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoStok> list = ToList<DepoStok>(dataTable);
            DepoStok item = new DepoStok();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<DepoStok> genericEntity = new GenericEntity<DepoStok>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoStok> list = ToList<DepoStok>(dataTable);
            DepoStok item = new DepoStok();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM DepoStok_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoStok> list = ToList<DepoStok>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DepoStok SelectByDepoIdAniObjesiId(int depoId, int aniObjesiId, string stokluMu)
        {
            string depoIdStr = depoId > 0 ? " AND A.DepoId=" + depoId : string.Empty;
            string aniObjesiIdStr = aniObjesiId > 0 ? " AND B.Id=" + aniObjesiId : string.Empty;
            string stokluMuStr = !string.IsNullOrEmpty(stokluMu) ? " AND B.StokluMu =" + stokluMu.ReturnQuotedValue() : string.Empty;
            string sqlString = string.Format(@"
                SELECT A.* FROM DepoStok_Table A
	                INNER JOIN AniObjesiTanim_Table B ON B.Id=A.AniObjesiId 
                WHERE A.SonAdet > 0 
                    {0} 
                    {1}
                    {2}
                ORDER BY A.Id
                ", stokluMuStr,depoIdStr, aniObjesiIdStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoStok> list = ToList<DepoStok>(dataTable);
            return list.FirstOrDefault();
        }
    }
}
