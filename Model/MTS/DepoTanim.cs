using Model.Ortak;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utility.ProjeGlobal;
using System.Linq;
using Utility.HelperClasses;
using System.Data;

namespace Model.MTS
{
    public class DepoTanim : ParentClass
    {
        [Required]
        [DisplayName("Depo Adı")]
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<DepoTanim> genericEntity = new GenericEntity<DepoTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
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
                    DepoTanim item = Select<DepoTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<DepoTanim> genericEntity = new GenericEntity<DepoTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
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
                    GenericEntity<DepoTanim> genericEntity = new GenericEntity<DepoTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    DepoTanim item = Select<DepoTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DepoTanim Select(int id)
        {
            GenericEntity<DepoTanim> genericEntity = new GenericEntity<DepoTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoTanim> list = ToList<DepoTanim>(dataTable);
            DepoTanim item = new DepoTanim();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<DepoTanim> genericEntity = new GenericEntity<DepoTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoTanim> list = ToList<DepoTanim>(dataTable);
            DepoTanim item = new DepoTanim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM DepoTanim_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DepoTanim> list = ToList<DepoTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectStokluAniObjesiList(int secilenAniObjesiId)
        {
            string secilenAniObjesiIdStr =secilenAniObjesiId>0?" AND C.Id=" + secilenAniObjesiId:string.Empty;
            string sqlString = string.Format(@"
                SELECT C.Id AniObjesiId,C.Adi AniObjesiAdi,A.Id DepoId,A.Adi DepoAdi,SUM(B.SonAdet) Adet FROM DepoTanim_Table A
	                INNER JOIN DepoStok_Table B ON B.DepoId=A.Id AND B.SonAdet > 0
	                INNER JOIN AniObjesiTanim_Table C ON C.Id=B.AniObjesiId 
                WHERE C.StokluMu={0} 
                    {1}
                GROUP BY A.Id,A.Adi,C.Id,C.Adi
                ORDER BY A.Id
                ", ProjeConstants.MTS_ANIOBJESISTOKLU.ReturnQuotedValue(),secilenAniObjesiIdStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return (dataTable);
        }
    }
}