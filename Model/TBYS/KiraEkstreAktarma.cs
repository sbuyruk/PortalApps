using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class KiraEkstreAktarma : ParentClass
    {
        public DateTime IslemTarihi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime OdemeTarihi { get; set; }
        
        public decimal Tutar { get; set; }
        public string DovizCinsi { get; set; }
        public string BankaAdi { get; set; }
        public string Telefon1 { get; set; }
        public string Adres { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Aciklama { get; set; }
        public bool AktarildiMi { get; set; }
        public bool ElleKayit { get; set; }
        public int OdemeId { get; set; }
        public int KiraciId { get; set; }
        public string IslemNo { get; set; }
        public bool Uyari { get; set; }
        public int OdemeSebebiId { get; set; }

        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM KiraEkstreAktarma_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraEkstreAktarma_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = list.FirstOrDefault();
            return (T)Convert.ChangeType(kiraEkstreAktarma, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraEkstreAktarma_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public List<KiraEkstreAktarma> SelectKiraciIdByAdi(string adi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table 
                WHERE  KiraciId >0 AND Adi = {0}", adi.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
            return (list);
        }    
        public List<KiraEkstreAktarma> SelectByIdList(string idListStr)
        {
            if (!string.IsNullOrEmpty(idListStr))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi,Id DESC, OdemeTarihi desc, Adi
                ", idListStr);
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);

                return list;
            }
            else
            {
                return new List<KiraEkstreAktarma>();
            }
        }
        public List<KiraEkstreAktarma> SelectByIslemNo(string islemNo)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraEkstreAktarma_Table 
                               WHERE  IslemNo={0}", islemNo.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
            return (list);
        }
        public List<KiraEkstreAktarma> SelectByColumns(string adi, string soyadi, decimal tutar, DateTime odemeTarihi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table 
                WHERE  Adi={0} 
                    AND Soyadi={1}
                    AND Tutar={2}
                    AND OdemeTarihi={3}                    
                ", adi.ReturnQuotedValue(), soyadi.ReturnQuotedValue(), tutar.ReturnQuotedValue(), OdemeTarihi.ReturnTRDateFormat());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
            return (list);
        }
        public List<KiraEkstreAktarma> SelectByEkstreIdList(string idListStr, ref int rowCount)
        {
            if (!string.IsNullOrEmpty(idListStr))
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi, OdemeTarihi desc, Adi
                ", idListStr);
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
                rowCount = dataTable != null ? dataTable.Rows.Count : 0;
                return list;
            }
            else
            {
                return new List<KiraEkstreAktarma>();
            }
        }


        public DataTable SelectYuklenenKayit(ref int rowCount, bool aktarilanlarHaric)
        {
            string aktarilanlarHaricStr = aktarilanlarHaric ? " WHERE AktarildiMi=0 " : "";
            string sqlString = string.Format(@"
                SELECT 
                    A.Id KiraEkstreAktarmaId, A.IslemTarihi,
                    A.KiraciId, B.Adi, B.Adi + ' (' + B.Adres + '' + IIF(ISNULL(D.IlceAdi,'')='','',D.IlceAdi + '/') +C.IlAdi+')' KiraciAdi, 
                    A.*, ISNULL(A.Adi,'')  +' ' +ISNULL(A.Soyadi,'') AdiSoyadi, 
                    A.Telefon1 + IIF(ISNULL(A.Telefon1,'')!='' AND ISNULL(A.Telefon2,'')!='',' - ','') + A.Telefon2 Telefon,
                    A.Aciklama,A.OdemeSebebiId OdemeSebebiId,E.OdemeSebebi OdemeSebebi
                FROM KiraEkstreAktarma_Table A
                    LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN Il_Table C ON C.IlAdi=B.Ili
                    LEFT JOIN Ilce_Table D ON (D.IlceAdi=B.Ilcesi AND D.IlAdi=B.Ili)
                    LEFT JOIN OdemeSebebiTanim_Table E ON E.Id=A.OdemeSebebiId
                {0}
                ORDER BY AktarildiMi,  OdemeTarihi desc, A.Id, A.Adi
                ", aktarilanlarHaricStr);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            //List<EkstreAktarma> list = ToList<EkstreAktarma>(dataTable);
            rowCount = dataTable != null ? dataTable.Rows.Count : 0;
            return dataTable;
        }
        public DataTable SelectById(int ekstreAktarmaId)
        {
            string sqlString = string.Format(@"
                SELECT 
                    A.Id KiraEkstreAktarmaId, A.IslemTarihi,
                    A.KiraciId, B.Adi, B.Adi + ' (' + B.Adres + '' + IIF(ISNULL(D.IlceAdi,'')='','',D.IlceAdi + '/') +C.IlAdi+')' KiraciAdi, 
                    A.*, ISNULL(A.Adi,'')  +' ' +ISNULL(A.Soyadi,'') AdiSoyadi, 
                    A.Telefon1 + IIF(ISNULL(A.Telefon1,'')!='' AND ISNULL(A.Telefon2,'')!='',' - ','') + A.Telefon2 Telefon,
                    A.Aciklama
                FROM KiraEkstreAktarma_Table A
                    LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    LEFT JOIN Il_Table C ON C.IlAdi=B.Ili
                    LEFT JOIN Ilce_Table D ON (D.IlceAdi=B.Ilcesi AND D.IlAdi=B.Ili)
                WHERE A.Id={0}
                ORDER BY AktarildiMi,  OdemeTarihi desc, A.Id, A.Adi
                ", ekstreAktarmaId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return dataTable;
        }
    }
}
