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
    public class Kiraci : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TCKimlikNo { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public string Aciklama { get; set; }
        public string KiralamaAmaci { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Kiraci_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);
            Kiraci kiraci = new Kiraci();
            kiraci = list.FirstOrDefault();
            return (T)Convert.ChangeType(kiraci, typeof(T));

        }
        public Kiraci Select(int id)
        {
            GenericEntity<Kiraci> genericEntity = new GenericEntity<Kiraci>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);
            Kiraci kiraci = new Kiraci();
            kiraci = list.FirstOrDefault();
            return kiraci;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Kiraci> genericEntity = new GenericEntity<Kiraci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
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
                    Kiraci item = Select<Kiraci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Kiraci> genericEntity = new GenericEntity<Kiraci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
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
                    GenericEntity<Kiraci> genericEntity = new GenericEntity<Kiraci>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Kiraci item = Select<Kiraci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Kiraci_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Kiraci> SelectAktifKiracilar()
        {
            string sqlString = string.Format(@"
                SELECT S.DosyaNo,A.*
                FROM Kiraci_Table A
                    INNER JOIN KiraSozlesme_Table S On S.KiraciId=A.Id AND S.Aktif=1
	                INNER JOIN OdemePlani_Table O On O.Id=(SELECT Top 1 Id FROM OdemePlani_Table WHERE SozlesmeId=S.Id)
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);

            return (list);
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER (ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id) AS Sirano, 
                    S.DosyaNo, A.Id KiraciId, Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo, A.Ilcesi +'-'+ A.Ili IlIlce, Semt,A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama,
                    S.Id SozlesmeId
                FROM Kiraci_Table A
                    LEFT JOIN KiraSozlesme_Table S On S.KiraciId=A.Id AND S.Aktif=1
                    LEFT JOIN SozlesmeTasinmaz_Table C On C.Id=(Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId=S.ID) 
                    LEFT JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id");
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


        public DataTable SelectAllReturnDT(string secim, string bolge)
        {
            string aktifStr = secim.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString()) ? "" : " AND S.Aktif=" + secim;
            string bolgeStr = bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? string.Empty : string.Format(" AND S.Bolge={0}", bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT 
	                ROW_NUMBER() OVER (ORDER BY MAX(DosyaNo)) AS Sirano, 
	                A.Id KiraciId, S.Bolge, A.Adi, Soyadi, MAX(SozBasTar) , COUNT(KiraciId), MAX(S.Id) SozlesmeId,S.Aktif, S.KiraBedeli,S.OdemeSekli,
	                TCKimlikNo,VergiDairesi,VergiNo, A.Ilcesi, A.Ili, A.Ilcesi +'-'+ A.Ili IlIlce, Semt,
                    A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama
                FROM KiraSozlesme_Table S
	                RIGHT JOIN Kiraci_Table A ON A.Id=S.KiraciId
                WHERE 1>0
                {0}
                {1}
                GROUP BY A.Id, A.Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo, A.Ilcesi, A.Ili,A.Ilcesi +'-'+ A.Ili , 
				S.Bolge, Semt,A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama,S.Aktif, S.KiraBedeli,S.OdemeSekli
                Order BY A.Adi --MAX(DosyaNo),KiraciId", aktifStr,bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectByByBolgeReturnDT(string aktif, string bolge)
        {
            string aktifStr = aktif.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString()) ? string.Empty : string.Format(" WHERE A.Aktif={0} ",  aktif);
            string bolgeStr = bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? string.Empty :
                (string.IsNullOrEmpty(aktifStr) ? string.Format(" WHERE A.Bolge={0}", bolge.ReturnQuotedValue()) : string.Format(" AND A.Bolge={0}", bolge.ReturnQuotedValue()));
            string whereStr = aktifStr + bolgeStr;
            string sqlString = string.Format(@"
                SELECT 
	                A.Id SozlesmeId, A.IlkSozlesmeTar,A.SozBasTar,A.SozBitTar,A.SozlesmeDurumu,A.KiraBedeli,A.OdemeSekli,A.TeminatTutari,A.Aktif,
	                B.Id KiraciId, B.Adi,B.Soyadi,B.Adres KiraciAdresi,B.Ili KiraciIli,B.Ilcesi KiraciIlcesi
                        --,E.IlAdi KiraciIli,F.IlceAdi KiraciIlcesi,
	                    --D.Adres+ISNULL(G.BolumNo,'') TasinmazAdresi, D.Ili TasinmazIli, D.Ilcesi TasinmazIlcesi
                FROM KiraSozlesme_Table A
                LEFT JOIN Kiraci_Table B ON B.Id=A.KiraciId
                --INNER JOIN SozlesmeTasinmaz_Table C ON C.SozlesmeId =A.Id
                --LEFT JOIN Tasinmaz_Table D ON D.Id =C.TasinmazId
                --LEFT JOIN Il_Table E ON E.IlAdi=B.Ili
                --LEFT JOIN Ilce_Table F ON F.IlceAdi=B.Ilcesi AND F.IlAdi=E.IlAdi
                --LEFT JOIN BagimsizBolum_Table G ON G.Id=C.BolumId
                {0}", whereStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectAktifSozlesmesiOlmayanKiracilarReturnDT(string bolge)
        {
            string bolgeStr = bolge.Equals(ProjeConstants.BOLGE_HEPSI) ? string.Empty : string.Format(" AND A.Bolge={0}", bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT 
	                A.KiraciId, A.Bolge, B.Adi,A.Aktif, B.Soyadi, MAX(A.Id) SozlesmeId,B.Ili,B.Ilcesi,
	                TCKimlikNo,VergiDairesi,VergiNo,B.Ilcesi +'-'+ B.Ili IlIlce, 
	                B.Semt,B.Adres,B.Telefon,B.Eposta, B.KiralamaAmaci
                FROM Kiraci_Table B 
	                INNER JOIN KiraSozlesme_Table A ON  A.KiraciId=B.Id
                WHERE not exists
                  (
                    SELECT 1 FROM KiraSozlesme_Table C 
                    WHERE A.KiraciId = C.KiraciId
                      AND A.Aktif=0
                      AND C.Aktif=1
                  )
                  AND Aktif=0
                  {0}
                GROUP BY --A.KiraciId,A.Aktif,B.Adi
                  A.KiraciId,A.Aktif, A.Bolge, B.Adi, B.Soyadi, 
	                TCKimlikNo,VergiDairesi,VergiNo, B.Ilcesi ,B.Ili , 
	                B.Semt,B.Adres,B.Telefon,B.Eposta, B.KiralamaAmaci
                  ORDER BY B.Adi --A.DosyaNo,A.KiraciId,A.Aktif	
                ",bolgeStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectByFilterReturnDataTable(string filter)
        {
            string sqlString = string.Format(@"
                SELECT A.*, B.*,
                    B.Id SozlesmeId
                FROM Kiraci_Table A
				    INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id AND B.Id in (SELECT Top 1 Id FROM KiraSozlesme_Table WHERE KiraciId=A.Id ORDER BY SozBitTar DESC)
                WHERE Adi like '%{0}%'
	                OR TCKimlikNo like '%{0}%'
	                OR Telefon like '%{0}%'
	                OR Adres like '%{0}%'
                ORDER BY SozBasTar DESC,Aktif DESC, KiraciId ",  filter);
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
        public string SelectByFilter(string filter)
        {
            string sqlString = string.Format(@"
                SELECT 
                    Id KiraciId, Adi,Soyadi, TCKimlikNo, Ili , Ilcesi, Adres,
                    Telefon
                FROM Kiraci_Table
                WHERE Adi like '%{0}%'
	                OR TCKimlikNo like '%{0}%'
	                OR Telefon like '%{0}%'
	                OR Adres like '%{0}%'
                ORDER BY KiraciId ",  filter);
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
        public Kiraci SelectNext()
        {
            Kiraci kiraci = new Kiraci();
            string sqlString = string.Format(@"SELECT * FROM Kiraci_Table
                    WHERE Id > {0}
                    ORDER BY Id ", Id);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Kiraci> list = ToList<Kiraci>(dataTable);
                kiraci = list.FirstOrDefault();
            }
            else
            {
                kiraci = SelectMin();

            }
            return kiraci;
        }
        public Kiraci SelectPrev()
        {
            Kiraci kiraci = new Kiraci();
            string sqlString = string.Format(@"
                    SELECT * FROM Kiraci_Table
                    WHERE Id < {0}
                    ORDER BY Id DESC", Id);


            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Kiraci> list = ToList<Kiraci>(dataTable);
                kiraci = list.FirstOrDefault();
            }
            else
            {
                kiraci = SelectMax();

            }
            return kiraci;
        }
        public Kiraci SelectMax()
        {
            Kiraci kiraci = null;
            string sqlString = string.Format(@"
                SELECT * FROM Kiraci_Table
                ORDER BY Id DESC ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Kiraci> list = ToList<Kiraci>(dataTable);
                kiraci = list.FirstOrDefault();
            }
            return kiraci;
        }
        public Kiraci SelectMin()
        {
            Kiraci kiraci = null;
            string sqlString = string.Format(@"
                SELECT * FROM Kiraci_Table
                ORDER BY Id ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Kiraci> list = ToList<Kiraci>(dataTable);
                kiraci = list.FirstOrDefault();
            }
            return kiraci;
        }

        public List<Kiraci> SelectByAdi(string adi, string soyadi = "")
        {
            string soyadiStr = string.IsNullOrEmpty(soyadi) ? string.Empty : string.Format(" AND Soyadi LIKE '%{0}%'", soyadi);
            string sqlString = string.Format(@"
                SELECT A.*
                FROM Kiraci_Table A
				INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id AND B.Id IN (SELECT MAX(Id) FROM KiraSozlesme_Table WHERE KiraciId=A.Id GROUP BY KiraciId) --Sözeşlmesi yeni olan önce gelsin
                WHERE Adi Like '%{0}%' 
                     {1}
                ORDER BY B.SozBasTar DESC
                ", adi.Trim(), soyadiStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);

            return (list);
        }
    }
}
