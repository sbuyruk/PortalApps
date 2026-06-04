using DocumentFormat.OpenXml.Office2010.Excel;
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
    public class SerhBeyanIrtifak : ParentClass
    {

        public int TasinmazId { get; set; }
        public string SBI { get; set; }
        public string MalikLehtar { get; set; }
        public string TesisKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string TerkinSebebi { get; set; }
        public string Yevmiye { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SerhBeyanIrtifak_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SerhBeyanIrtifak> list = ToList<SerhBeyanIrtifak>(dataTable);
            SerhBeyanIrtifak serhBeyanIrtifak = new SerhBeyanIrtifak();
            serhBeyanIrtifak = list.FirstOrDefault();
            return (T)Convert.ChangeType(serhBeyanIrtifak, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<SerhBeyanIrtifak> genericEntity = new GenericEntity<SerhBeyanIrtifak>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
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
                    SerhBeyanIrtifak item = Select<SerhBeyanIrtifak>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<SerhBeyanIrtifak> genericEntity = new GenericEntity<SerhBeyanIrtifak>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
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
                    GenericEntity<SerhBeyanIrtifak> genericEntity = new GenericEntity<SerhBeyanIrtifak>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    SerhBeyanIrtifak item = Select<SerhBeyanIrtifak>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZTAAHHUT);
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
                               FROM SerhBeyanIrtifak_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SerhBeyanIrtifak> list = ToList<SerhBeyanIrtifak>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<SerhBeyanIrtifak> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM SerhBeyanIrtifak_Table
                WHERE TasinmazId={0}", tasinmazId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SerhBeyanIrtifak> list = ToList<SerhBeyanIrtifak>(dataTable);

            return list;
        }


    }
}
