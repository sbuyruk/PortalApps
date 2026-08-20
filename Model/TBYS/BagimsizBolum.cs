using DAO.Ortak;
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
        public decimal BBBrutAlan { get; set; }
        public decimal BBNetAlan { get; set; }
        public string KullanimAmaci { get; set; }
        public decimal MuhasebeyeKayitliDeger { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal YaklasikPiyasaDegeri { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagimsizBolum_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    BagimsizBolum item = Select<BagimsizBolum>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM BagimsizBolum_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagimsizBolum> SelectByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM BagimsizBolum_Table
                              WHERE TasinmazId=@TasinmazId");
            query.AddParameter("@TasinmazId", tasinmazId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public List<BagimsizBolum> SelectByBolumNO(string bolumNo)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumNo=@BolumNo");
            query.AddParameter("@BolumNo", bolumNo);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public BagimsizBolum SelectByBolumId(int bolumId)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumId=@BolumId");
            query.AddParameter("@BolumId", bolumId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            BagimsizBolum bolum = new BagimsizBolum();
            bolum = list.FirstOrDefault();
            return bolum;
        }
    }
}
