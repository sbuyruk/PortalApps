
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IzinHareket : ParentClass
    {
        public IzinHareket()
        {
        }

        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public int IzinTalepId { get; set; }
        public int IzinDonemId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Birim { get; set; }
        public string Adres { get; set; }
        public int VekilImza { get; set; }
        public int AmirImza { get; set; }
        public int OnayImza { get; set; }
        public bool Mahsup { get; set; }
        public string Aciklama { get; set; }
        public string OncekiIzinStr { get; set; }
        public string KullanilanIzinStr { get; set; }
        public string KalanIzinStr { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinHareket> list = ToList<IzinHareket>(dataTable);
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = list.FirstOrDefault();
            return (T)Convert.ChangeType(izinHareket, typeof(T));
        }
        public override int Save()
        {
            try
            {
                GenericEntity<IzinHareket> genericEntity = new GenericEntity<IzinHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

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
                if (Id != 0)
                {
                    GenericEntity<IzinHareket> genericEntity = new GenericEntity<IzinHareket>(ProjeConstants.SQL_UPDATE);
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
        public override bool Delete()
        {
            string sqlString = DeleteSQL();

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IzinHareket_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinHareket> list = ToList<IzinHareket>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<IzinHareket> SelectByTarihReturnList(DateTime tarih)
        {
            string sqlString = string.Format(@"
                            SELECT *
                            FROM IzinHareket_Table
                            WHERE IzinTipi!=2 AND BitisTarihi>={0}
                            ORDER BY BaslangicTarihi ", tarih.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinHareket> list = ToList<IzinHareket>(dataTable);

            return (list);
        }
        public IzinHareket SelectByIzinTalepId(int izinTalepId)
        {
            string sqlString = string.Format(@"
                            SELECT *
                            FROM IzinHareket_Table
                            WHERE  IzinTalepId={0}
                            ", izinTalepId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinHareket> list = ToList<IzinHareket>(dataTable);
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = list.FirstOrDefault<IzinHareket>();
            return izinHareket;
        }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, 
                    A.PersonelId, A.BaslangicTarihi, A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,
                    C.Adi IzinTanim, D.BirimId,E.KisaAdi GorevYeri
                FROM IzinHareket_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN IzinTanim_Table C ON C.Id= A.IzinTipi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE --Mahsup=0 AND --SB 21.09.2020 Yeşim Hanımın talebi, Deniz özkanın 21.09.2020 tarihli izni o günkü görevli izinli personel listesinde çıkmadı o yüzden mahsup=0 kapatıldı
                    BaslangicTarihi<={0} AND BitisTarihi>={1}
                ORDER BY  ProtokolSiraNo, A.IzinTipi, BaslangicTarihi ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat()); //TODO 8 saat olan Mazeret de dahil olsun

            DataTable dataTable = dao.selectFromDb(sqlString, "");


            return dataTable;
        }
        public IzinHareket SelectByPersonelTarih(int personelId,DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"

                SELECT *
                FROM IzinHareket_Table A
                WHERE PersonelId={0} 
                    AND IzinTipi!=2 AND IzinTipi!=8 -- süt izni ve mazeret haric
                    AND BaslangicTarihi<={1} AND BitisTarihi>={2}
                ORDER BY BitisTarihi DESC
            ",personelId, bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat()); 

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IzinHareket> list = ToList<IzinHareket>(dataTable);
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = list.FirstOrDefault<IzinHareket>();
            return izinHareket;

        }
        public DataTable SelectByIzinTipiTarihReturnDataTable(int izinTipi, DateTime ilkTarih, DateTime bittar)
        {
            string sqlString = string.Format(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, 
                    A.BaslangicTarihi, A.BitisTarihi,A.Sure,A.Birim,A.IzinTipi,
                    C.Adi IzinTanim, D.BirimId,E.KisaAdi GorevYeri, A.Aciklama,
					D.ProtokolSiraNo
                FROM IzinHareket_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN IzinTanim_Table C ON C.Id= A.IzinTipi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE Mahsup=0 AND IzinTipi={0} AND BaslangicTarihi BETWEEN {1} AND {2}
                ORDER BY D.ProtokolSiraNo,A.BaslangicTarihi ", izinTipi, ilkTarih.ReturnTRDateFormat(), bittar.ReturnTRDateFormat()); //TODO 8 saat olan Mazeret de dahil olsun

            DataTable dataTable = dao.selectFromDb(sqlString, "");


            return dataTable;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IzinHareket_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IzinHareket_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        //depreciated use "public DataTable SelectByIzinDonemiReturnDataTable(int izinDonemId,int personelId, int izinTipi)"
        public DataTable SelectByIzinDonemiReturnDataTable(int personelId, int izinTipi, DateTime izinDonemiBasi, DateTime izinDonemiSonu)
        {

            string sqlString = SelectByIzinDonemiSQL(personelId, izinTipi, izinDonemiBasi, izinDonemiSonu);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");

            }
            catch (Exception e)
            {
                throw e;
            }

            return (dataTable);
        }
        public DataTable SelectByIzinDonemiReturnDataTable(int izinDonemId, int personelId, int izinTipi)
        {

            //string sqlString = SelectByIzinDonemiSQL (izinDonemId,personelId, izinTipi);
            string sqlString = SelectByPersonelIdDonemIdSQL(personelId, izinDonemId, izinTipi);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");

            }
            catch (Exception e)
            {
                throw e;
            }

            return (dataTable);
        }
        public string SelectByPersonelIdDonemIdReturnJson(int personelId, int donemId, int izinTanimId)
        {
            string json = string.Empty;

            string sqlString = SelectByPersonelIdDonemIdSQL(personelId, donemId, izinTanimId);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
                json = ConvertDataTabletoString(dataTable);
                return (json);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<IzinHareket> SelectDigerIzinlerByPersonelIdReturnJson(int personelId)
        {
            string sqlString = string.Format(@" 
                SELECT *, B.Adi IzinTipiAdi FROM IzinHareket_Table A
	                INNER JOIN IzinTanim_Table B ON B.Id=A.IzinTipi                    
                WHERE IzinTipi NOT IN (1,2) 
                    AND PersonelId={0}
                ORDER BY A.BaslangicTarihi DESC", personelId);
            try
            {
                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<IzinHareket> list = ToList<IzinHareket>(dataTable);
                return (list);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string ConvertDataTabletoString(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            if (dt != null)
            {

                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }

            }

            return serializer.Serialize(rows);
        }
        ////depreciated use "private string SelectByPersonelIdDonemIdSQL(int personelId,int donemId,int izinTanimId)"
        private string SelectByIzinDonemiSQL(int personelId, int izinTipi, DateTime izinDonemiBasi, DateTime izinDonemiSonu)
        {
            string andStr = string.Empty;
            string onaySurumuStr = string.Empty;
            andStr = personelId > 0 ? string.Format(" AND PersonelId={0} ", personelId) : "";
            andStr += izinTipi > 0 ? string.Format(" AND IzinTipi={0} ", izinTipi) : "";
            string izinDonemiStr = "'" + izinDonemiBasi + "-" + izinDonemiSonu + "' IzinDonemi,";
            string sqlstr = string.Format(@" 
                    SELECT A.Id IzinHareketId, P.Adi+' '+P.Soyadi AdiSoyadi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Birim,A.IzinTalepId, {3}
                           B.Adi IzinTipi,B.Id IzinTipiId, A.VekilImza, A.AmirImza,A.OnayImza, A.Adres,A.Aciklama,A.IzinDonemId,A.Mahsup
                    FROM IzinHareket_Table A
                        INNER JOIN Personel_Table P On P.Id=A.PersonelId  
						INNER JOIN IzinTanim_Table B On B.Id=A.IzinTipi  
                    WHERE A.BaslangicTarihi BETWEEN {0} AND {1}  
                        {2}
                    ORDER BY A.Id DESC", izinDonemiBasi.ReturnTRDateFormat(), izinDonemiSonu.ReturnTRDateFormat(), andStr, izinDonemiStr);
            return sqlstr;
        }
        private string SelectByPersonelIdDonemIdSQL(int personelId, int donemId, int izinTanimId)
        {
            int sinceYear = DateTime.Today.AddYears(-30).Year;
            string perStr = personelId > 0 ? string.Format(" AND A.PersonelId={0} ", personelId) : "";
            string donemStr = donemId > 0 ? string.Format(" AND A.IzinDonemId={0} ", donemId) : "";
            string izinTipiStr = izinTanimId > 0 ? string.Format(" AND A.IzinTipi={0} ", izinTanimId) : "";
            DateTime sinceDate = new DateTime(sinceYear, 1, 1);
            string sqlstr = string.Format(@" 
                    SELECT A.Id IzinHareketId, P.Adi+' '+P.Soyadi AdiSoyadi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi, 
                           A.Sure, A.Birim, A.Sure+' '+A.Birim SureBirim, A.IzinTalepId,A.IzinDonemId,
                           CONVERT(varchar,FORMAT(C.BaslangicTarihi,'dd.MM.yyyy')) +'-'+ CONVERT(varchar,FORMAT(C.BitisTarihi,'dd.MM.yyyy')) IzinDonemi ,C.Adi IzinDonemiYil,
                           B.Adi IzinTipi,B.Id IzinTipiId, A.VekilImza, A.AmirImza,A.OnayImza, A.Adres, A.Mahsup, A.Aciklama,
                           A.OncekiIzinStr, A.KullanilanIzinStr,A.KalanIzinStr
                    FROM IzinHareket_Table A
                        INNER JOIN Personel_Table P On P.Id = A.PersonelId  
						INNER JOIN IzinTanim_Table B On B.Id = A.IzinTipi   
						LEFT OUTER JOIN IzinDonem_Table C On C.Id=A.IzinDonemId    
                    WHERE A.BaslangicTarihi >= {0} {1} {2} {3}
                    ORDER BY A.BaslangicTarihi DESC", sinceDate.ReturnTRDateFormat(), perStr, donemStr, izinTipiStr);
            return sqlstr;
        }

    }
}
