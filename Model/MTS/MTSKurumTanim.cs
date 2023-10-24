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
    public class MTSKurumTanim : ParentClass
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
                GenericEntity<MTSKurumTanim> genericEntity = new GenericEntity<MTSKurumTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM
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
                    MTSKurumTanim item = Select<MTSKurumTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<MTSKurumTanim> genericEntity = new GenericEntity<MTSKurumTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);
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
                    GenericEntity<MTSKurumTanim> genericEntity = new GenericEntity<MTSKurumTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    MTSKurumTanim item = Select<MTSKurumTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MTSKurumTanim Select(int id)
        {
            GenericEntity<MTSKurumTanim> genericEntity = new GenericEntity<MTSKurumTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumTanim> list = ToList<MTSKurumTanim>(dataTable);
            MTSKurumTanim item = new MTSKurumTanim();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<MTSKurumTanim> genericEntity = new GenericEntity<MTSKurumTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumTanim> list = ToList<MTSKurumTanim>(dataTable);
            MTSKurumTanim item = new MTSKurumTanim();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM MTSKurumTanim_Table ORDER BY Id 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumTanim> list = ToList<MTSKurumTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}
