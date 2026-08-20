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
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM HukukiTakip_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                    HukukiTakip item = Select<HukukiTakip>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<HukukiTakip> genericEntity = new GenericEntity<HukukiTakip>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    HukukiTakip item = Select<HukukiTakip>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
                throw;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                DELETE HukukiTakip_Table 
                WHERE SozlesmeId=@SozlesmeId");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(query, "");
            return isSuccess;
        }
		public string SelectAllReturnJson()
		{
			SqlQuery query = new SqlQuery(@"
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
				WHERE A.Aktif=1");
			DataTable dataTable = null;
			try
			{
				dataTable = dao.SelectFromDb(query, "");
			}
			catch (Exception e)
			{
				throw;
			}
			string json = ToJSON(dataTable);
			return json;
		}
		public DataTable SelectAllReturnDataTable()
		{
			DataTable dataTable = null;
			SqlQuery query = new SqlQuery(@"
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
				dataTable = dao.SelectFromDb(query, "");
            }
            catch (Exception e)
            {
                throw;
            }

            return dataTable;
        }
        public override List<T> SelectAll<T>()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM HukukiTakip_Table
                                WHERE Aktif=1");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public HukukiTakip SelectBySozlesmeId(int sozlesmeId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM HukukiTakip_Table
                WHERE SozlesmeId=@SozlesmeId
                ORDER BY IslemTarihi ");
            query.AddParameter("@SozlesmeId", sozlesmeId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);
            HukukiTakip hukukiTakip = new HukukiTakip();
            hukukiTakip = list.FirstOrDefault<HukukiTakip>();
            return hukukiTakip;

        }
    }
}
