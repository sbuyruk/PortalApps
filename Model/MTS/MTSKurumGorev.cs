using Model.Ortak;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utility.ProjeGlobal;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.MTS
{
    public class MTSKurumGorev : ParentClass
    {
        [Required]
        public int MTSKurumTanimId { get; set; }
        public int MTSGorevTanimId { get; set; }
        public int KisiId { get; set; }
        [DisplayName("Görev Durumu")]
        [Required(ErrorMessage = "Görev Durumu boş olamaz.")]
        public string Durum { get; set; } = ProjeConstants.MTSGOREVDURUMU_GOREVDE;
        [DisplayName("Başlama Tarihi")]
        public DateTime BaslamaTarihi { get; set; }

        [DisplayName("Ayrılma Tarihi")]
        public DateTime? AyrilmaTarihi { get; set; } = null;

        public string AyrilmaSebebi { get; set; } = ProjeConstants.MTSAYRILMASEBEBI_BOS;
        public string KisaAdi { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_KURUMGOREV
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
                    MTSKurumGorev item = Select<MTSKurumGorev>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_KURUMGOREV);
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
                    GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    MTSKurumGorev item = Select<MTSKurumGorev>(Id);
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
        public MTSKurumGorev Select(int id)
        {
            GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumGorev> list = ToList<MTSKurumGorev>(dataTable);
            MTSKurumGorev item = new MTSKurumGorev();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumGorev> list = ToList<MTSKurumGorev>(dataTable);
            MTSKurumGorev item = new MTSKurumGorev();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM MTSKurumGorev_Table ORDER BY Id 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MTSKurumGorev> list = ToList<MTSKurumGorev>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        //public string SelectByKisiIdReturnKurumGorev(int kisiId)
        //{
        //    GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_SELECT);
        //    string sqlString = string.Format(@"
        //    SELECT B.Adi + ' ' C.Adi KurumGorev FROM  MTSKurumGorev_Table A
        //        Left Join MTSKurumTanim_Table B ON B.Id=A.MTSKurumTanimId
        //        Left Join MTSGorevTanim_Table C ON B.Id=A.MTSGorevTanimId
        //    WHERE A.KisiId={0} AND A.Durum={1}
        //    ",kisiId,ProjeConstants.MTSGOREVDURUMU_GOREVDE);

        //    DataTable dataTable = dao.SelectFromDb(sqlString, "");
        //    string kurumGorev =string.Empty;
        //    if (dataTable != null)
        //    {
        //        kurumGorev = dataTable.Rows[0]["KurumGorev"].ToString();
        //    }
            
        //    return kurumGorev;
        //}
        public string SelectByKisiIdReturnKurumGorev(int kisiId, ref string kurum, ref string gorev)
        {
            GenericEntity<MTSKurumGorev> genericEntity = new GenericEntity<MTSKurumGorev>(ProjeConstants.SQL_SELECT);
            string sqlString = string.Format(@"
            SELECT B.Adi Kurum, C.Adi Gorev FROM  MTSKurumGorev_Table A
                Left Join MTSKurumTanim_Table B ON B.Id=A.MTSKurumTanimId
                Left Join MTSGorevTanim_Table C ON C.Id=A.MTSGorevTanimId
            WHERE A.KisiId={0} AND A.Durum={1}
            ", kisiId, ProjeConstants.MTSGOREVDURUMU_GOREVDE.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            if (dataTable != null)
            {
                kurum = dataTable.Rows[0]["Kurum"].ToString();
                gorev = dataTable.Rows[0]["Gorev"].ToString();
            }

            return kurum+' '+ gorev;
        }
    }
}
