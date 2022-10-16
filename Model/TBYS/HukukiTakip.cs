using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);
            HukukiTakip hukukiTakip = new HukukiTakip();
            hukukiTakip = list.FirstOrDefault();
            return (T)Convert.ChangeType(hukukiTakip, typeof(T));

        }
        public override int Save()
        {
            string sqlString = string.Format(@"
                INSERT INTO HukukiTakip_Table 
                    (SozlesmeId, KiraciId,  IslemTarihi, BorcAnaPara, BorcFaiz, Aciklama , Aktif,
                    Olusturan, OlusturmaTarihi)
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8})",
                SozlesmeId.ReturnQuotedValue(), KiraciId.ReturnQuotedValue(), IslemTarihi.ReturnTRDateFormat(),
                BorcAnaPara.ConvertDecimalToString(), BorcFaiz.ConvertDecimalToString(), Aciklama.ReturnQuotedValue(), Aktif.ReturnQuotedValue(),
                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());

            int id = dao.Insert(sqlString);

            this.Id = id;
            return id;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = string.Format(@"
                    UPDATE HukukiTakip_Table 
                    SET SozlesmeId={0}, KiraciId={1}, IslemTarihi={2}, BorcAnaPara={3}, BorcFaiz={4}, Aciklama={5}, Aktif={6},
                        Degistiren={7},DegistirmeTarihi={8}
                    WHERE Id={9}",
                    SozlesmeId.ReturnQuotedValue(), KiraciId.ReturnQuotedValue(), IslemTarihi.ReturnTRDateFormat(),
                    BorcAnaPara.ConvertDecimalToString(), BorcFaiz.ConvertDecimalToString(), Aciklama.ReturnQuotedValue(), Aktif.ReturnQuotedValue(),
                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM HukukiTakip_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
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
                dataTable = dao.selectFromDb(sqlString, "");
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
                dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public HukukiTakip SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM HukukiTakip_Table
                WHERE SozlesmeId={0}
                ORDER BY IslemTarihi ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<HukukiTakip> list = ToList<HukukiTakip>(dataTable);
            HukukiTakip hukukiTakip = new HukukiTakip();
            hukukiTakip = list.FirstOrDefault<HukukiTakip>();
            return hukukiTakip;

        }
    }
}
