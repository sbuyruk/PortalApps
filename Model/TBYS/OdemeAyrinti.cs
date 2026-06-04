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
    public class OdemeAyrinti : ParentClass
    {

        public int KiraciId { get; set; }
        public int SozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public int OdemeId { get; set; }
        public int GecikmeZammiId { get; set; }
        public int OdemePlaniSirasi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime GecikmeZammiDegisimTar { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public DateTime IlkTarih { get; set; }
        public DateTime SonTarih { get; set; }
        public int AySayisi { get; set; }
        public int GunSayisi { get; set; }
        public decimal GecikmeZammiOrani { get; set; }
        public decimal AnaPara { get; set; }
        public decimal OdenenTutar { get; set; }
        public decimal KalanAnaPara { get; set; }
        public decimal GecikmeZammiTutari { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeAyrinti_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
            odemeAyrinti = list.FirstOrDefault();
            return (T)Convert.ChangeType(odemeAyrinti, typeof(T));

        }
        public OdemeAyrinti Select(int id)
        {
            GenericEntity<OdemeAyrinti> genericEntity = new GenericEntity<OdemeAyrinti>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
            odemeAyrinti = list.FirstOrDefault();
            return odemeAyrinti;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemeAyrinti> genericEntity = new GenericEntity<OdemeAyrinti>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI);
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
                    OdemeAyrinti item = Select<OdemeAyrinti>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<OdemeAyrinti> genericEntity = new GenericEntity<OdemeAyrinti>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI);
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
                    GenericEntity<OdemeAyrinti> genericEntity = new GenericEntity<OdemeAyrinti>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    OdemeAyrinti item = Select<OdemeAyrinti>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                DELETE OdemeAyrinti_Table 
                WHERE SozlesmeId={0}", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int odemePlaniId)
        {
            string sqlString = string.Format(@"
                DELETE OdemeAyrinti_Table 
                WHERE OdemeId={0} AND OdemePlaniId={1}", odemeId, odemePlaniId);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeAyrinti_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<OdemeAyrinti> SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemeAyrinti_Table
                WHERE SozlesmeId={0}
                ORDER BY OdemePlaniSirasi,OdemeTarihi,Id ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            return list;

        }

        public List<OdemeAyrinti> SelectByOdemeIdOdemePlaniId(int odemeId, int odemePlaniId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemeAyrinti_Table
                WHERE OdemeId={0} AND OdemePlaniId={1}
                ORDER BY OdemePlaniSirasi,OdemeTarihi,Id ", odemeId.ReturnQuotedValue(), odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            return list;

        }
        public OdemeAyrinti Select(KiraSozlesme kiraSozlesme, int odemePlaniId, int gecikmeZammiId, int odemeId)
        {

            //string odemeIdStr= odemeId==0?"":" AND OdemeId=" + odemeId;
            string sqlString = string.Format(@"
                SELECT * FROM OdemeAyrinti_Table
                WHERE 
                    KiraciId={0} AND SozlesmeId={1} AND OdemePlaniId={2} AND GecikmeZammiId={3}
                    AND OdemeId={4}
                ORDER BY OdemePlaniSirasi,OdemeTarihi,OdemeId,Id ", kiraSozlesme.KiraciId, kiraSozlesme.Id, odemePlaniId, gecikmeZammiId, odemeId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable == null)
            {
                return null;
            }
            else
            {
                List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
                OdemeAyrinti odemeAyrinti = new OdemeAyrinti();
                odemeAyrinti = list.FirstOrDefault();
                return odemeAyrinti;
            }
        }
        public List<OdemeAyrinti> Select(KiraSozlesme kiraSozlesme)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemeAyrinti_Table
                WHERE 
                    SozlesmeId={0} 
                ORDER BY OdemePlaniSirasi,OdemeTarihi,Id ", kiraSozlesme.Id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            return list;

        }
        public List<OdemeAyrinti> Select(OdemePlani odemePlani)
        {
            string sqlString = string.Format(@"
                SELECT * FROM OdemeAyrinti_Table
                WHERE 
                    OdemePlaniId={0} 
                ORDER BY IlkTarih,Id ", odemePlani.Id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            return list;

        }

        public decimal SelectLastAnaParaByOdemePlaniId(int odemePlaniId)
        {
            decimal anaPara = 0;
            string sqlString = string.Format(@"
                SELECT AnaPara FROM OdemeAyrinti_Table
                WHERE 
                    OdemePlaniId={0} 
                ORDER BY Id DESC
                ", odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    anaPara = Decimal.Parse(row["AnaPara"].ToString());
                }
            }
            return anaPara;
        }

        public decimal SelectSumGecikmeZammiTutariByOdemePlaniId(int odemePlaniId)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT ISNULL(SUM(GecikmeZammiTutari),0) Toplam FROM OdemeAyrinti_Table
                WHERE 
                    OdemePlaniId={0} 
                ", odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = Decimal.Parse(row["Toplam"].ToString());
                }
            }
            return toplam;
        }

        public decimal SelectSonGecikmeZammiTutariByOdemePlaniId(int odemePlaniId)
        {
            decimal oran = 0;
            string sqlString = string.Format(@"
                SELECT GecikmeZammiOrani FROM OdemeAyrinti_Table
                
                WHERE 
                    OdemePlaniId={0} 
            ORDER BY IlkTarih
                ", odemePlaniId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyrinti> list = ToList<OdemeAyrinti>(dataTable);
            if (list.Count > 0)
                oran = list.LastOrDefault().GecikmeZammiOrani;
            return oran;
        }
    }
}
