using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;
using DAO.Ortak;
using Model.Ortak;
using System.Collections.Generic;
using System.Linq;
using System;
using Utility.ProjeGlobal;
using System.Data;
using Utility.HelperClasses;

namespace Model.MTS
{
    public class AniObjesiTanim : ParentClass
    {
       
        [Required]
        [DisplayName("Ani Objesinin Adi")]
        public string Adi { get; set; }
        public string StokluMu { get; set; }
        public int Sira { get; set; }
        [Required]
        public int KaynakId { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<AniObjesiTanim> genericEntity = new GenericEntity<AniObjesiTanim>(ProjeConstants.SQL_INSERT);
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
                    AniObjesiTanim item = Select<AniObjesiTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<AniObjesiTanim> genericEntity = new GenericEntity<AniObjesiTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
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
                    GenericEntity<AniObjesiTanim> genericEntity = new GenericEntity<AniObjesiTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    AniObjesiTanim item = Select<AniObjesiTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
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
        public AniObjesiTanim Select(int id)
        {
            GenericEntity<AniObjesiTanim> genericEntity = new GenericEntity<AniObjesiTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiTanim> list = ToList<AniObjesiTanim>(dataTable);
            AniObjesiTanim item = new AniObjesiTanim();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<AniObjesiTanim> genericEntity = new GenericEntity<AniObjesiTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiTanim> list = ToList<AniObjesiTanim>(dataTable);
            AniObjesiTanim item = new AniObjesiTanim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM AniObjesiTanim_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<AniObjesiTanim> list = ToList<AniObjesiTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectStokluAniObjesiList()
        {
            string sqlString = string.Format(@"
                SELECT A.Id AniObjesiId,A.Adi,SUM(B.SonAdet) Toplam FROM AniObjesiTanim_Table A
	                INNER JOIN DepoStok_Table B ON B.AniObjesiId=A.Id AND B.SonAdet > 0
                WHERE A.StokluMu={0} 
                GROUP BY A.Id,A.Adi
                ORDER BY A.Adi
                ", ProjeConstants.MTS_ANIOBJESISTOKLU.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return (dataTable);
        }
    }
}
