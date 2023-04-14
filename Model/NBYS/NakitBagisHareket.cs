using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class NakitBagisHareket : ParentClass
    {
        public DateTime BagisTarihi { get; set; }
        public int BagisciId { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public int BankaId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adresi { get; set; }
        public string Telefon { get; set; }
        public string Aciklama { get; set; }
        public int ArmaganId { get; set; }
        public bool IadeEdildiMi { get; set; }
        public decimal IadeMiktari { get; set; }
        public DateTime IadeTarihi { get; set; }
        public string IadeSebebi { get; set; }
        public string IadeEden { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        //Methods
 public override int Save()
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    NakitBagisHareket item = Select<NakitBagisHareket>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    NakitBagisHareket item = Select<NakitBagisHareket>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);
            NakitBagisHareket nakitBagisciHareket = new NakitBagisHareket();
            nakitBagisciHareket = list.FirstOrDefault();
            return (T)Convert.ChangeType(nakitBagisciHareket, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
                string sqlString = string.IsNullOrEmpty(extId)?genericEntity.GetQuery(this): genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<NakitBagisHareket> SelectByArmaganId(int armaganId)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table 
                               WHERE ArmaganId= {0}", armaganId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (list);
        }
        public DataTable SelectByArmaganIdReturnDataTable(int armaganId)
        {
            string sqlString = string.Format(@"
                SELECT A.*, A.BagisMiktari BagisTutari, A.DovizCinsi,
                    B.BagisMiktari ArmaganTutari, B.DovizCinsi, B.Durum,
                    C.Armagan,
                    D.Banka
                FROM NakitBagisHareket_Table A
                INNER JOIN Armagan_Table B ON B.Id=A.ArmaganId
                INNER JOIN ArmaganTanim_Table C ON C.Id=B.ArmaganTanimId
                LEFT JOIN BankaTanim_Table D ON D.Id=A.BankaId
                WHERE A.ArmaganId= {0}
                ORDER BY A.BagisTarihi DESC
                ", armaganId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public string SelectByBagisciIdReturnJSon(string nakitBagisciId, ref int rowCount)
        {
            string sqlString = string.Format(@"
                SELECT A.BagisciId
		                ,BagisTarihi as BagisTarihi
		                ,REPLACE(CONVERT(varchar,A.BagisMiktari),'.',',') + ' ' +A.DovizCinsi BagisTutari
                        ,REPLACE(ISNULL(CONVERT(varchar,B.BagisMiktari),''),'.',',') + ' ' +ISNULL(B.DovizCinsi,'') ArmaganTutari
		                ,Convert(nvarchar,replace (A.BagisMiktari,'.',',')) as BagisMiktari
                        ,A.DovizCinsi as DovizCinsi
		                ,ArmaganId
		                ,ISNULL(C.Armagan,'') Armagan
		                ,ISNULL(B.Durum,'') Durum
		                ,ISNULL(B.Aciklama,'') Aciklama
						,D.Banka
                FROM NakitBagisHareket_Table A
                LEFT JOIN Armagan_Table B ON B.Id= A.ArmaganId 
                LEFT JOIN ArmaganTanim_Table C ON C.Id= B.ArmaganTanimId
				LEFT JOIN BankaTanim_Table D ON D.Id=A.BankaId
                WHERE A.BagisciId={0}
				ORDER BY BagisTarihi DESC 
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }

            string json = ToJSON(dataTable);
            return json;
        }
        public List<NakitBagisHareket> SelectArmaganiOlmayanBagislarByBagisciId(int bagisciId)
        {
            string sqlString = string.Format(@"
                SELECT A.* 
                    --,A.Id NakitBagisHareketId
	                --,A.ArmaganId AArmaganId ,B.Id BArmaganId
	                --,B.BagisciId BBagisciId,A.BagisciId ABagisciId
	                --,B.ArmaganTanimId
	                --,A.BagisTarihi,A.BagisMiktari
                FROM NakitBagisHareket_Table A
	                LEFT JOIN Armagan_Table B ON B.Id=A.ArmaganId AND B.ArmaganTanimId IN (2,3,4)
                WHERE A.BagisciId = {0}
	                AND B.Id is NULL
                ORDER BY BagisMiktari DESC", bagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (list);
        }
        public List<NakitBagisHareket> SelectByBagisciId(int bagisciId)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table 
                               WHERE BagisciId= {0}", bagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (list);
        }
        public List<NakitBagisHareket> SelectByBagisciIdTarih(int nakitBagisciId, DateTime bastar,DateTime bittar)
        {
            string sqlString = string.Format(@"SELECT *
                FROM NakitBagisHareket_Table 
                WHERE BagisciId={0} and  BagisTarihi between {1} and {2}", nakitBagisciId, bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (list);
        }
        /**
         * returns Json
         * ***/
        public string SelectByBagisciId(string nakitBagisciId, ref int rowCount)
        {
            string sqlString = string.Format(@"
                    SELECT NakitBagisHareket_Table.BagisciId
				         ,BagisTarihi as BagisTarihi
                         ,FORMAT(BagisTarihi,'dd.MM.yyyy') BagisTarihiDDMMYY
						 ,Convert(nvarchar,replace (NakitBagisHareket_Table.BagisMiktari,'.',',')) as BagisMiktari
                         ,NakitBagisHareket_Table.DovizCinsi as DovizCinsi
						 ,ArmaganId
						 ,ArmaganTanim_Table.Armagan
                         ,BankaTanim_Table.Banka
						 ,Armagan_Table.Durum
						 ,Armagan_Table.Aciklama
                    FROM NakitBagisHareket_Table 
                    LEFT OUTER JOIN BankaTanim_Table ON BankaTanim_Table.Id= NakitBagisHareket_Table.BankaId
                    LEFT OUTER JOIN Armagan_Table ON Armagan_Table.Id= NakitBagisHareket_Table.ArmaganId 
                    LEFT OUTER JOIN ArmaganTanim_Table ON ArmaganTanim_Table.Id= Armagan_Table.ArmaganTanimId
                    WHERE NakitBagisHareket_Table.BagisciId={0}
					Order BY BagisTarihi DESC, BagisciId 
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }

            string json = ToJSON(dataTable);
            return json;
        }
        /**
         * returns Datatable **/
        public DataTable SelectByBagisciIdReturnDataTable(int nakitBagisciId)
        {
            string sqlString = string.Format(@"
                SELECT NakitBagisHareket_Table.BagisciId
                    ,FORMAT(BagisTarihi,'dd.MM.yyyy') BagisTarihi
					,Convert(nvarchar,replace (NakitBagisHareket_Table.BagisMiktari,'.',',')) as BagisMiktari
                    ,NakitBagisHareket_Table.DovizCinsi as DovizCinsi
					,ArmaganTanim_Table.Armagan
                    ,BankaTanim_Table.Banka
					,Armagan_Table.Aciklama
                FROM NakitBagisHareket_Table 
                    LEFT OUTER JOIN BankaTanim_Table ON BankaTanim_Table.Id= NakitBagisHareket_Table.BankaId
                    LEFT OUTER JOIN Armagan_Table ON Armagan_Table.Id= NakitBagisHareket_Table.ArmaganId 
                    LEFT OUTER JOIN ArmaganTanim_Table ON ArmaganTanim_Table.Id= Armagan_Table.ArmaganTanimId
                WHERE NakitBagisHareket_Table.BagisciId={0}
				ORDER BY NakitBagisHareket_Table.BagisTarihi DESC, BagisciId 
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public decimal GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(DateTime basTar, DateTime bitTar, int nakitBagisciId)
        {
            decimal toplam = 0;
            DateTime ilkTarih = new DateTime(basTar.Year, basTar.Month, basTar.Day);
            DateTime sonTarih = new DateTime(bitTar.Year, bitTar.Month, bitTar.Day);
            string sqlString = string.Format(@"SELECT SUM(BagisMiktari) Toplam
                                             FROM NakitBagisHareket_Table
                                             WHERE BagisciId={0} and  BagisTarihi between {1} and {2}", nakitBagisciId, ilkTarih.ReturnTRDateFormat(), sonTarih.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
                }
            }

            return toplam;
        }
        public decimal SelectSumBagisMiktariByBagisTarihiBolge(DateTime basTar, DateTime bitTar, string bolge, ref int adet)
        {
            decimal toplam = 0;
            string bolgeStr = string.IsNullOrEmpty(bolge) ? " AND Bolge is NULL " : "AND Bolge=" + bolge.ReturnQuotedValue().ToString();
            string sqlString = string.Format(@"
                    SELECT COUNT(H.Id) Adet,SUM(BagisMiktari) Toplam FROM NakitBagisHareket_Table H
                        LEFT OUTER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId
                        LEFT OUTER JOIN Il_Table B ON B.Id=H.Ili
                        WHERE BagisTarihi between {0} and {1}
	                    {2} ", basTar.ReturnQuotedValue(), bitTar.ReturnQuotedValue(), bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = Decimal.Parse(row["Toplam"].ToString());
                    adet = Int32.Parse(row["Adet"].ToString());
                }
            }

            return toplam;
        }
        public DataTable SelectByFilter(string filter, DateTime bagisTarihi)
        {
            string ilStr = string.Empty;

            string sqlString = string.Format(@"
                SELECT A.Id BagisHareketId, B.Id ArmaganId, C.Armagan, B.Durum, A.BagisciId, D.Adi BagisciAdi
				        , A.BagisTarihi ,Convert(nvarchar,replace (A.BagisMiktari,'.',',')) as BagisMiktari, A.DovizCinsi
						, D.Adres + ' ' + F.IlceAdi +' / '+ E.IlAdi Adres, F.IlAdi Ili, F.IlceAdi Ilcesi
                        , D.Telefon1 + IIF(ISNULL(D.Telefon1,'')!='' AND ISNULL(D.Telefon2,'')!='',' - ','') + D.Telefon2 Telefon
                        , A.DovizCinsi, A.ArmaganId, C.Armagan, B.Durum,B.Aciklama,A.IadeEdildiMi, ISNULL(A.IadeMiktari,0) IadeMiktari
                    FROM NakitBagisHareket_Table A
                    LEFT OUTER JOIN Armagan_Table B ON B.Id= A.ArmaganId 
                    LEFT OUTER JOIN ArmaganTanim_Table C ON C.Id= B.ArmaganTanimId
					INNER JOIN NakitBagisci_Table D ON D.Id= A.BagisciId 
					LEFT JOIN Il_Table E ON E.Id= D.Ili 
                    LEFT JOIN Ilce_Table F ON F.Id= D.Ilcesi AND F.IlId=E.Id
                WHERE A.BagisTarihi>{0} 
					AND D.Adi like '%{1}%'
	                OR D.TCKimlikNo like '%{1}%'
	                OR D.Telefon1 like '%{1}%'
	                OR D.Adres like '%{1}%'
                ORDER BY A.BagisTarihi DESC  ", bagisTarihi.ReturnTRDateFormat(), filter);
            DataTable dataTable = null;
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
        public decimal SelectSumBagisMiktariByBagisTarihiBanka(DateTime basTar, DateTime bitTar, string banka, ref int adet)
        {
            decimal toplam = 0;
            string bankaStr = string.IsNullOrEmpty(banka) ? " AND BankaGrup is NULL " : " AND BankaGrup=" + banka.ReturnQuotedValue().ToString();
            string sqlString = string.Format(@"
                    SELECT COUNT(H.Id) Adet,SUM(BagisMiktari) Toplam FROM NakitBagisHareket_Table H
                        LEFT OUTER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId
						LEFT OUTER JOIN BankaTanim_Table B ON B.Id= H.BankaId
                        WHERE BagisTarihi between {0} and {1}
                            AND BagisciId IN (Select BagisciId FROM NakitBagisHareket_Table WHERE BagisTarihi < {0} )
	                    {2} ", basTar.ReturnQuotedValue(), bitTar.ReturnQuotedValue(), bankaStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ToString().ConvertToDecimal();
                    adet = row["Adet"].ToString().ConvertToInt();
                }
            }

            return toplam;
        }
        public decimal SelectMaxBagisMiktariByBagisTarihiBanka(DateTime basTar, DateTime bitTar, string banka)
        {
            decimal toplam = 0;
            string bankaStr = string.IsNullOrEmpty(banka) ? "" : " AND BankaGrup=" + banka.ReturnQuotedValue().ToString();
            string sqlString = string.Format(@"
                    SELECT MAX(BagisMiktari) Toplam FROM NakitBagisHareket_Table H
                        LEFT OUTER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId
						LEFT OUTER JOIN BankaTanim_Table B ON B.Id= H.BankaId
                        WHERE BagisTarihi BETWEEN {0} AND {1}
	                    {2} ", basTar.ReturnQuotedValue(), bitTar.ReturnQuotedValue(), bankaStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ToString().ConvertToDecimal();
                }
            }

            return toplam;
        }
        public string SelectByDurumTarihReturnJson(string ay, string yil, int ilId)
        {
            string sqlString = GetSQLSelectByTarih(ay, yil, ilId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            string json = ToJSON(dataTable);
            return json;
        }

        private string GetSQLSelectByTarih(string ay, string yil, int ilId)
        {
            string ilStr = string.Empty;
            if (ilId < ProjeConstants.IL_HEPSI)
            {
                ilStr = " AND Il_Table.Id =" + ilId;
            }
            string ayStr;
            if (ay.Equals(ProjeConstants.HEPSI_INT.ToString()))
            {
                ayStr = " ";
            }
            else
            {
                ayStr = " AND MONTH(BagisTarihi)=" + ay.ReturnQuotedValue();
            }

            string sqlString = string.Format(@"
                SELECT  A.Id NakitBagisHareketId,
                        A.BagisciId BagisciId, A.Id BagisId,
                        B.Adi as Adi, B.Adres,
                        B.TCKimlikNo as TCKimlikNo,	 TRIM(B.Telefon1 +' ' + B.Telefon2) Telefon,               
                        Convert(nvarchar,replace (A.BagisMiktari,'.',',')) as BagisMiktari,
                        A.DovizCinsi as DovizCinsi,
	                    A.BagisTarihi as BagisTarihi,
                        A.ArmaganId as ArmaganId,
	                    ISNULL(E.IlAdi,'')  Ili,
                        D.Durum as Durum,
	                    C.Banka Banka
                FROM NakitBagisHareket_Table A 
                INNER JOIN NakitBagisci_Table B ON B.Id= A.BagisciId 
                INNER JOIN BankaTanim_Table C ON C.Id= A.BankaId
                LEFT JOIN Armagan_Table D ON D.Id= A.ArmaganId
			    LEFT JOIN Il_Table E ON E.Id= A.Ili
                WHERE YEAR(BagisTarihi)={0}  
                {1}
                {2}
            ", yil, ayStr, ilStr);
            return sqlString;
        }

        public DataTable SelectByDurumTarihReturnDataTable(string ay, string yil, int ilId)
        {
            string sqlString = GetSQLSelectByTarih(ay, yil, ilId);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectByIliAndYil(int ilId, DateTime tarih)
        {
            string sqlString = string.Empty;
            string ilStr = string.Empty;
            if (ilId < ProjeConstants.IL_HEPSI)
            {
                ilStr = " AND Il_Table.Id =" + ilId;
            }

            sqlString = string.Format(@"
                        SELECT YEAR(BagisTarihi) Yil , SUM(BagisMiktari) BagisToplam, count(BagisMiktari) BagisSayisi , Il_Table.IlAdi IlAdi from NakitBagisHareket_Table
                        LEFT OUTER JOIN IL_Table on Il_Table.Id= NakitBagisHareket_Table.Ili
                        WHERE BagisTarihi > {0} 
                        {1}
                        GROUP BY YEAR(BagisTarihi), IlAdi
                        ORDER BY Yil DESC
                        ", tarih.ReturnQuotedValue(), ilStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            ///List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);
            return dataTable;
        }
        public DataTable SelectCountByBagisTarihiBolge(int yil, int ay)
        {
            string sqlString = string.Format(@"        
                                SELECT  Convert(nvarchar,replace (SUM(A.BagisMiktari),'.',',')) as Toplam , 
										COUNT(A.Id) Adet, I.Bolge Bolge, MONTH(A.BagisTarihi) Ay 
								FROM NakitBagisHareket_Table A
                                INNER JOIN NakitBagisci_Table N on N.Id=A.BagisciId
								INNER JOIN Il_Table I on I.Id=N.Ili
                                WHERE YEAR(A.BagisTarihi)=  {0}
                                      AND  MONTH(A.BagisTarihi)= {1}
                                GROUP BY Bolge , MONTH(A.BagisTarihi) ", yil, ay);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectCountSumByBagisTarihi(DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"        
                                Select COUNT(H.Id) Adet,SUM(BagisMiktari) Toplam,B.Bolge FROM NakitBagisHareket_Table H
                                    INNER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId
                                    LEFT OUTER JOIN Il_Table B ON B.Id=H.Ili
                                WHERE BagisTarihi between {0} and {1}
	                                AND BagisciId IN (Select BagisciId FROM NakitBagisHareket_Table WHERE BagisTarihi < {0} )
                                GROUP BY B.Bolge 
                                ORDER BY B.Bolge ", bastar.ReturnQuotedValue(), bittar.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectCountSumByYil_il(int basYil, int bitYil)
        {
            string sqlString = string.Format(@"        
                SELECT C.Bolge,C.IlAdi,YEAR(A.BagisTarihi) Yil, SUM(BagisMiktari) Tutar ,COUNT(A.Id) Adet
                FROM NakitBagisHareket_Table A
	                LEFT JOIN Il_Table C ON C.Id= A.Ili
                WHERE YEAR(A.BagisTarihi) BETWEEN {0} AND {1}  -- AND A.BagisMiktari>0
                GROUP BY C.Bolge,C.IlAdi,YEAR(A.BagisTarihi)
                ORDER BY C.Bolge,C.IlAdi,YEAR(A.BagisTarihi) ", basYil, bitYil);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectCountSumByBagisBanka(DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"        
                                SELECT COUNT(H.Id) Adet,SUM(BagisMiktari) Toplam, B.BankaGrup Banka 
                                FROM NakitBagisHareket_Table H
                                    --bu sefer de Vakıfta sorun oldu INNER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId    --LEFT OUTER iş bankası toplamı hatalı çıktığı için değiştirildi
                                    INNER JOIN NakitBagisci_Table A ON A.Id= H.BagisciId --bi daa açtım bakalım hayırlısı
									INNER JOIN BankaTanim_Table B ON B.Id= H.BankaId        --LEFT OUTER 
                                WHERE BagisTarihi BETWEEN {0} AND {1}
                                GROUP BY B.BankaGrup 
                                ORDER BY Toplam DESC,B.BankaGrup ", bastar.ReturnQuotedValue(), bittar.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectByBolgeTarih(string bolge, DateTime ilkTarih, DateTime sonTarih)
        {
            string sqlString = string.Format(@"
                SELECT A.BagisTarihi, A.BagisMiktari, 
                    B.Id NakitBagisciId, B.Adi, B.Soyadi, B.Telefon1, B.Telefon2, B.Adres, B.BelgeIstemiyor,
					C.IlAdi Ili,D.IlceAdi Ilcesi,
					F.Armagan, E.Durum
                FROM NakitBagisHareket_Table A
                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                LEFT JOIN Ilce_Table D ON D.Id=B.Ilcesi
                LEFT JOIN Armagan_Table E ON E.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table F ON F.Id=E.ArmaganTanimId
                WHERE Bolge={0} AND BagisTarihi BETWEEN {1} AND {2}
                    --AND E.Durum NOT IN ('Ulaşılamıyor', 'Belge İstemiyor') --30.12.2022 Deniz Hanım aradı, Zeki Alb. ve Kemal Alb.. tarafından bu şeklde olmasının istendiğini iletti
                ORDER BY BagisMiktari DESC,Adi, BagisTarihi DESC
            ", bolge.ReturnQuotedValue(), ilkTarih.ReturnTRDateFormat(),sonTarih.ReturnTRDateFormat());
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectByNakitBagisciId(int nakitBagisciId)
        {
            string sqlString = string.Format(@"
                SELECT A.BagisTarihi, A.BagisMiktari, 
                    B.Id NakitBagisciId, B.Adi, B.Soyadi, B.Telefon1, B.Telefon2, B.Adres, B.BelgeIstemiyor,
					C.IlAdi Ili,D.IlceAdi Ilcesi,
					F.Armagan, E.Durum
                FROM NakitBagisHareket_Table A
                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                INNER JOIN Il_Table C ON C.Id=B.Ili
                LEFT JOIN Ilce_Table D ON D.Id=B.Ilcesi
                LEFT JOIN Armagan_Table E ON E.Id=A.ArmaganId
                LEFT JOIN ArmaganTanim_Table F ON F.Id=E.ArmaganTanimId
                WHERE B.Id={0}
                ORDER BY BagisTarihi DESC 
            ", nakitBagisciId);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectByBagisTarihiBankaId(DateTime bagisTarihi, int bankaId=0)
        {
            string bankaStr = bankaId == 0 ? string.Empty : string.Format(" AND BankaId={0}", bankaId);
            string sqlString = string.Format(@"
                
                SELECT BagisTarihi, SUM(BagisMiktari) ToplamBagis, B.BankaGrup Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi={0} 
                {1}
                GROUP BY BagisTarihi, B.BankaGrup
                ORDER BY BagisTarihi 
            ", bagisTarihi.ReturnTRDateFormat(), bankaStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectByTarihBankaGrup(DateTime bagisTarihi, string bankaGrup)
        {
            string bankaGrupStr = string.IsNullOrEmpty(bankaGrup) ? string.Empty : string.Format(" AND BankaGrup={0}", bankaGrup.ReturnQuotedValue());
            string sqlString = string.Format(@"             
                SELECT BagisTarihi,  SUM(BagisMiktari + ISNULL(IadeMiktari,0)) ToplamBagis, B.BankaGrup Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi = {0} 
                {1}
                GROUP BY BagisTarihi, B.BankaGrup  
            ", bagisTarihi.ReturnTRDateFormat(), bankaGrupStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectTlBagisByTarihBankaGrup2(DateTime bagisTarihi, string bankaGrup2)
        {
            string bankaGrupStr = string.IsNullOrEmpty(bankaGrup2) ? string.Empty : string.Format(" AND BankaGrup2={0}", bankaGrup2.ReturnQuotedValue());
            string sqlString = string.Format(@"             
                SELECT BagisTarihi,  SUM(BagisMiktari + ISNULL(IadeMiktari,0)) ToplamBagis, B.BankaGrup2 Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi={0} AND BagisTarihi = {1} 
                {2}
                GROUP BY BagisTarihi, B.BankaGrup2  
            ",ProjeConstants.DOVIZ_TL.ReturnQuotedValue(), bagisTarihi.ReturnTRDateFormat(), bankaGrupStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectDovizBagisByTarihBankaGrup2(DateTime bastar,DateTime bittar, string bankaGrup2,string dovizCinsi)
        {
            string bankaGrupStr = string.IsNullOrEmpty(bankaGrup2) ? string.Empty : string.Format(" AND BankaGrup2={0}", bankaGrup2.ReturnQuotedValue());
            string sqlString = string.Format(@"  
                SELECT BagisTarihi, A.DovizTutari,A.DovizKuru,A.DovizCinsi,A.BagisMiktari, B.BankaGrup2, B.Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi={0} AND (BagisTarihi >= {1} AND BagisTarihi <= {2})  
                {3}
            ", dovizCinsi.ReturnQuotedValue(), bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat(), bankaGrupStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public List<string> SelectBankaGrup2ByTarihDovizCinsi(DateTime bastar, DateTime bittar, string dovizCinsi)
        {
            string dovizCinsiStr = string.IsNullOrEmpty(dovizCinsi) ? string.Empty : string.Format(" AND DovizCinsi={0}", dovizCinsi.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT BankaGrup2
                FROM NakitBagisHareket_Table A
					INNER JOIN BankaTanim_Table B ON B.Id=A.BankaId 
				WHERE BagisTarihi >={0} AND BagisTarihi<={1}
                    {2}
                GROUP BY BankaGrup2
                ", bastar.ReturnTRDateFormat(),bittar.ReturnTRDateFormat(),dovizCinsiStr) ;

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<string> list = new List<string>();
            if (dataTable!=null)
            {
                list = dataTable.AsEnumerable()
                       .Select(r => r.Field<string>("BankaGrup2"))
                       .ToList(); 
            }
            return list;
        }
        public DataTable SelectByTarihBankaId(DateTime bagisTarihi, int bankaId)
        {
            string bankaIdStr = bankaId==0 ? string.Empty : string.Format(" AND BankaId={0}", bankaId);
            string sqlString = string.Format(@"             
                SELECT BagisTarihi,  SUM(BagisMiktari + ISNULL(IadeMiktari,0)) ToplamBagis, B.BankaGrup Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE BagisTarihi = {0} 
                {1}
                GROUP BY BagisTarihi, B.BankaGrup  
            ", bagisTarihi.ReturnTRDateFormat(), bankaIdStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
        public DataTable SelectDovizleBagisByTarihBankaId(DateTime bastar, DateTime bittar, string bankaGrup)
        {
            string bankaGrupStr = string.IsNullOrEmpty(bankaGrup)||bankaGrup.Equals("0") ? string.Empty : string.Format(" AND BankaGrup={0}", bankaGrup.ReturnQuotedValue());
            string sqlString = string.Format(@"
                
                SELECT DovizCinsi, SUM(BagisMiktari) TlKarsiligiToplamBagis, SUM(DovizTutari) ToplamDovizTutari, B.BankaGrup Banka 
                FROM NakitBagisHareket_Table A
	                LEFT JOIN BankaTanim_Table B ON B.Id=A.BankaId
                WHERE DovizCinsi!={0} AND BagisTarihi >= {1} AND BagisTarihi <= {2}
                {3}
                GROUP BY DovizCinsi, B.BankaGrup
                ORDER BY Banka 
            ", ProjeConstants.DOVIZ_TL.ReturnQuotedValue(), bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat(), bankaGrupStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("sql=" + sqlString, e);
                throw ex;
            }
            return dataTable;
        }
    }
}
