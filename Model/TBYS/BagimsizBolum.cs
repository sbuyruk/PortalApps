using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class BagimsizBolum : ParentClass
    {
        public int TasinmazId { get; set; }
        public string BolumNo { get; set; }
        public string Nitelik{ get; set; }
        public decimal Metrekare { get; set; }
        public string KullanimAmaci { get; set; }
        public decimal MuhasebeyeKayitliDeger { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal YaklasikPiyasaDegeri { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagimsizBolum_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            BagimsizBolum bagimsizBolum = new BagimsizBolum();
            bagimsizBolum = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagimsizBolum, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<BagimsizBolum> genericEntity = new GenericEntity<BagimsizBolum>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM);
                }
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
                    BagimsizBolum item = Select<BagimsizBolum>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BagimsizBolum> genericEntity = new GenericEntity<BagimsizBolum>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM);
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
                    GenericEntity<BagimsizBolum> genericEntity = new GenericEntity<BagimsizBolum>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    BagimsizBolum item = Select<BagimsizBolum>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagimsizBolum_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagimsizBolum> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public List<BagimsizBolum> SelectByBolumNO(string bolumNo)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumNo={0}", bolumNo.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public BagimsizBolum SelectByBolumId(int bolumId)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumId={0}", bolumId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            BagimsizBolum bolum = new BagimsizBolum();
            bolum = list.FirstOrDefault();
            return bolum;
        }
    }
}
