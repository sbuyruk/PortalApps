using Model.Ortak;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utility.ProjeGlobal;
using System.Data;
using System.Linq;

namespace Model.MTS
{
    public class MTSGorevTanim : ParentClass
    {
        [Required]
        [DisplayName("Kurum Adı")]
        public string Adi { get; set; }
        [DisplayName("Kısa Adı")]
        public string KisaAdi { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<MTSGorevTanim> genericEntity = new GenericEntity<MTSGorevTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM
                        );
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
                    MTSGorevTanim item = Select<MTSGorevTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<MTSGorevTanim> genericEntity = new GenericEntity<MTSGorevTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM);
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
                    GenericEntity<MTSGorevTanim> genericEntity = new GenericEntity<MTSGorevTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    MTSGorevTanim item = Select<MTSGorevTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MTSGorevTanim Select(int id)
        {
            GenericEntity<MTSGorevTanim> genericEntity = new GenericEntity<MTSGorevTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSGorevTanim> list = ToList<MTSGorevTanim>(dataTable);
            MTSGorevTanim item = new MTSGorevTanim();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<MTSGorevTanim> genericEntity = new GenericEntity<MTSGorevTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSGorevTanim> list = ToList<MTSGorevTanim>(dataTable);
            MTSGorevTanim item = new MTSGorevTanim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM MTSGorevTanim_Table ORDER BY Id 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSGorevTanim> list = ToList<MTSGorevTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}
