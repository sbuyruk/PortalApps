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
    public class HukukiTakip : ParentClass
    {
        public int SozlesmeId { get; set; }
        public int KiraciId { get; set; }
        public decimal BorcAnaPara { get; set; }
        public decimal BorcFaiz { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM HukukiTakip_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);
            HukukiTakip hukukiTakip = new HukukiTakip();
            hukukiTakip = list.FirstOrDefault();
            return (T)Convert.ChangeType(hukukiTakip, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<HukukiTakip> genericEntity = new GenericEntity<HukukiTakip>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
                }
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
                    HukukiTakip item = Select<HukukiTakip>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<HukukiTakip> genericEntity = new GenericEntity<HukukiTakip>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
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
                    GenericEntity<HukukiTakip> genericEntity = new GenericEntity<HukukiTakip>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    HukukiTakip item = Select<HukukiTakip>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                DELETE HukukiTakip_Table 
                WHERE SozlesmeId={0}", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT A.Id, A.SozlesmeId,B.DosyaNo,A.KiraciId,A.Aciklama,
	                FORMAT(A.BorcAnaPara,'###.00') BorcAnaPara,
	                FORMAT(A.BorcFaiz,'###.00') BorcFaiz,
	                FORMAT(A.IslemTarihi,'dd/MM/yyyy') IslemTarihi,
	                FORMAT(B.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
	                FORMAT(B.SozBasTar,'dd/MM/yyyy') SozBasTar,
	                FORMAT(B.SozBitTar,'dd/MM/yyyy') SozBitTar,
	                C.Adi,C.Soyadi, C.Adi+' '+C.Soyadi KiraciAdiSoyadi
                FROM HukukiTakip_Table A
                INNER JOIN KiraSozlesme_Table B ON B.Id=A.SozlesmeId
                INNER JOIN Kiraci_Table C ON C.Id=A.KiraciId
                WHERE A.Aktif=1 
                ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectAllReturnDataTable()
        {
            DataTable dataTable = null;
            string sqlString = string.Format(@"
                SELECT A.Id, A.SozlesmeId,A.KiraciId,A.BorcAnaPara,A.BorcFaiz,A.Aciklama,
	                FORMAT(A.IslemTarihi,'dd/MM/yyyy') IslemTarihi,
	                FORMAT(B.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
	                FORMAT(B.SozBasTar,'dd/MM/yyyy') SozBasTar,
	                FORMAT(B.SozBitTar,'dd/MM/yyyy') SozBitTar,
	                C.Adi,C.Soyadi, C.Adi+' '+C.Soyadi KiraciAdiSoyadi
                FROM HukukiTakip_Table A
                INNER JOIN KiraSozlesme_Table B ON B.Id=A.SozlesmeId
                INNER JOIN Kiraci_Table C ON C.Id=A.KiraciId
                WHERE A.Aktif=1");

            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }

            return dataTable;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM HukukiTakip_Table
                                WHERE Aktif=1");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public HukukiTakip SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM HukukiTakip_Table
                WHERE SozlesmeId={0}
                ORDER BY IslemTarihi ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);
            HukukiTakip hukukiTakip = new HukukiTakip();
            hukukiTakip = list.FirstOrDefault<HukukiTakip>();
            return hukukiTakip;

        }
    }
}
