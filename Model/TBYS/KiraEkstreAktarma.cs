using DAO.Ortak;
using Model.NBYS;
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

        public override int Save()
        {
            try
            {
                GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA);
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
                    KiraEkstreAktarma item = Select<KiraEkstreAktarma>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA);
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
                    GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    KiraEkstreAktarma item = Select<KiraEkstreAktarma>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraEkstreAktarma_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
            KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
            kiraEkstreAktarma = list.FirstOrDefault();
            return (T)Convert.ChangeType(kiraEkstreAktarma, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraEkstreAktarma_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<KiraEkstreAktarma> genericEntity = new GenericEntity<KiraEkstreAktarma>(ProjeConstants.SQL_DELETE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<KiraEkstreAktarma> SelectKiraciIdByAdi(string adi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table 
                WHERE  KiraciId >0 AND Adi = {0}", adi.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                ", adi.ReturnQuotedValue(), soyadi.ReturnQuotedValue(), tutar.ReturnQuotedValue(), odemeTarihi.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                DataTable dataTable = dao.SelectFromDb(sqlString, "");
                List<KiraEkstreAktarma> list = ToList<KiraEkstreAktarma>(dataTable);
                rowCount = dataTable != null ? dataTable.Rows.Count : 0;
                return list;
            }
            else
            {
                return new List<KiraEkstreAktarma>();
            }
        }


        public DataTable SelectYuklenenKayit(ref int rowCount, bool aktarilanlarHaric, bool kiraTeminatDiger)
        {
            DateTime ucAyOncesi = DateTime.Today.AddMonths(-3);
            string tarih =string.Format(" AND OdemeTarihi >={0}", new DateTime(ucAyOncesi.Year, ucAyOncesi.Month, 1).ReturnTRDateFormat());
            string aktarilanlarHaricStr = aktarilanlarHaric ? " AND AktarildiMi=0 " : "";
            string kiraTeminatDigerStr = kiraTeminatDiger ? string.Format(" AND (OdemeSebebiId IS NULL OR OdemeSebebiId IN ({0})) ",
                ProjeConstants.ODEMESEBEBI_DIGER_INT+","+
                ProjeConstants.ODEMESEBEBI_KIRA_INT + ","+
                ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT + ","+
                ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT + ","+
                ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT ) : "";
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
                WHERE 1>0
                {0}
                {1}
                {2}
                ORDER BY AktarildiMi,  OdemeTarihi desc, A.Id, A.Adi
                ", aktarilanlarHaricStr,kiraTeminatDigerStr,tarih);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }

        public DataTable SelectTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
        {
            DateTime bastar = new DateTime(tarih.Year, tarih.Month, tarih.Day);
            DateTime bittar = UtilityHelper.TariheSaatEkle(bastar, "23:59");
            string odemeSebebiIdStr = odemeSebebiId == ProjeConstants.HEPSI_INT ? string.Empty : string.Format(" AND (A.odemeSebebiId={0} OR C.odemeSebebiId={0}) ", odemeSebebiId);
            string sqlString = string.Format(@"
                SELECT 
	                A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB, A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA, C.Tutar TutarB,A.Aciklama AciklamaA ,C.Aciklama AciklamaB
	            FROM KiraEkstreAktarma_Table A
                    LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
                    LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
                    LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId
                    LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
                WHERE A.OdemeTarihi BETWEEN {0} AND {1}
                    {2}
                ", bastar.ReturnTRDateFormat(),bittar.ReturnTRDateFormat(), odemeSebebiIdStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectSumTutarByTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
        {
            DateTime bastar = new DateTime(tarih.Year, tarih.Month, tarih.Day);
            DateTime bittar = UtilityHelper.TariheSaatEkle(bastar, "23:59");
            string odemeSebebiIdStr = odemeSebebiId == ProjeConstants.HEPSI_INT ? string.Empty : string.Format(" AND (A.odemeSebebiId={0} OR C.odemeSebebiId={0}) ", odemeSebebiId);
            string sqlString = string.Format(@"
                SELECT 
	                A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB, A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA, C.Tutar TutarB,A.Aciklama AciklamaA ,C.Aciklama AciklamaB
	            FROM KiraEkstreAktarma_Table A
                    LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
                    LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
                    LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId
                    LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
                WHERE A.OdemeTarihi BETWEEN {0} AND {1}
                    {2}
                ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat(), odemeSebebiIdStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
    }
}
