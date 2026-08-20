using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public string Aciklama { get; set; }
        public string KiralamaAmaci { get; set; }
        public override T Select<T>(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Kiraci_Table 
                               WHERE  Id=@Id");
            query.AddParameter("@Id", id);
            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            DataTable dataTable = dao.SelectFromDb(query, "");
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
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                    Kiraci item = Select<Kiraci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Kiraci> genericEntity = new GenericEntity<Kiraci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Kiraci item = Select<Kiraci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM Kiraci_Table");

            DataTable dataTable = dao.SelectFromDb(query, "");
            List<Kiraci> list = ToList<Kiraci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
		public List<Kiraci> SelectAktifKiracilar()
		{
			SqlQuery query = new SqlQuery(@"
				SELECT S.DosyaNo,A.*
				FROM Kiraci_Table A
					INNER JOIN KiraSozlesme_Table S On S.KiraciId=A.Id AND S.Aktif=1
					INNER JOIN OdemePlani_Table O On O.Id=(SELECT Top 1 Id FROM OdemePlani_Table WHERE SozlesmeId=S.Id)
				ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id");

			DataTable dataTable = dao.SelectFromDb(query, "");
			List<Kiraci> list = ToList<Kiraci>(dataTable);

			return (list);
		}
        public string SelectAllReturnJson()
        {
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER (ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id) AS Sirano, 
                    S.DosyaNo, A.Id KiraciId, Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo, A.Ilcesi +'-'+ A.Ili IlIlce, Semt,A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama,
                    S.Id SozlesmeId,S.Aktif
                FROM Kiraci_Table A
                    LEFT JOIN KiraSozlesme_Table S On S.KiraciId=A.Id AND S.Aktif=1
                    LEFT JOIN SozlesmeTasinmaz_Table C On C.Id=(Select top 1 Id from SozlesmeTasinmaz_Table where SozlesmeId=S.ID) 
                    LEFT JOIN Tasinmaz_Table D On D.Id=C.TasinmazId
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999), S.Id, A.Id");
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


		public DataTable SelectAllReturnDT(string secim, int bolgeId)
		{
			StringBuilder sb = new StringBuilder(@"
				SELECT 
					C.IlAdi Ili,D.IlceAdi Ilcesi,D.IlceAdi, D.IlceAdi +'-'+ C.IlAdi As IlIlce
					 ,E.KisaAdi As Bolge,
					A.Id KiraciId, A.Adi, Soyadi, MAX(SozBasTar) , COUNT(KiraciId), MAX(S.Id) SozlesmeId,S.Aktif, S.KiraBedeli,S.OdemeSekli,
					TCKimlikNo,VergiDairesi,VergiNo, Semt,
					A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama
				FROM KiraSozlesme_Table S
					RIGHT JOIN Kiraci_Table A ON A.Id=S.KiraciId
				LEFT JOIN 
					Il_Table C ON C.Id=A.IlId
				LEFT JOIN 
					Ilce_Table D ON D.Id=A.IlceId
				LEFT JOIN 
					Bolge_Table E ON E.Id=C.BolgeId
				WHERE 1=1 ");
			SqlQuery query = new SqlQuery();
			if (!secim.Equals(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString()))
			{
				sb.Append(" AND S.Aktif=@Aktif ");
				query.AddParameter("@Aktif", secim);
			}
			if (bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT)
			{
				sb.Append(" AND S.BolgeId=@BolgeId ");
				query.AddParameter("@BolgeId", bolgeId);
			}
			sb.Append(@" GROUP BY A.Id, A.Adi,Soyadi,TCKimlikNo,VergiDairesi,VergiNo, D.IlceAdi, C.IlAdi,D.IlceAdi +'-'+ C.IlAdi  ,E.KisaAdi , 
					Semt,A.Adres,A.Telefon,A.Eposta, A.KiralamaAmaci,A.Aciklama,S.Aktif, S.KiraBedeli,S.OdemeSekli
				Order BY A.Adi ");
			query.Sql = sb.ToString();
			DataTable dataTable = dao.SelectFromDb(query, "");
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
        public DataTable SelectAktifSozlesmesiOlmayanKiracilarReturnDT(int bolgeId)
        {
            StringBuilder sb = new StringBuilder(@"
                SELECT 
	                A.KiraciId, B.Adi,A.Aktif, B.Soyadi, MAX(A.Id) SozlesmeId,
                    F.IlAdi Ili,D.IlceAdi Ilcesi,D.IlceAdi, D.IlceAdi +'-'+ F.IlAdi As IlIlce,E.KisaAdi As Bolge,
	                TCKimlikNo,VergiDairesi,VergiNo,
	                B.Semt,B.Adres,B.Telefon,B.Eposta, B.KiralamaAmaci
                FROM Kiraci_Table B 
	                INNER JOIN KiraSozlesme_Table A ON  A.KiraciId=B.Id
                    LEFT JOIN 
	                    Il_Table F ON F.Id=B.IlId
                    LEFT JOIN 
	                    Ilce_Table D ON D.Id=B.IlceId
                    LEFT JOIN 
	                    Bolge_Table E ON E.Id=A.BolgeId
				WHERE not exists
				  (
					SELECT 1 FROM KiraSozlesme_Table C 
					WHERE A.KiraciId = C.KiraciId
					  AND A.Aktif=0
					  AND C.Aktif=1
				  )
				  AND A.Aktif=0 ");
			SqlQuery query = new SqlQuery();
			if (bolgeId != ProjeConstants.HEPSI_INT && bolgeId != ProjeConstants.BOLGE_GENELMUDURLUK_INT)
			{
				sb.Append(" AND A.BolgeId=@BolgeId ");
				query.AddParameter("@BolgeId", bolgeId);
			}
			sb.Append(@" GROUP BY A.KiraciId,A.Aktif,  B.Adi, B.Soyadi, 
					TCKimlikNo,VergiDairesi,VergiNo,  D.IlceAdi, F.IlAdi,D.IlceAdi +'-'+ F.IlAdi  ,E.KisaAdi , 
					B.Semt,B.Adres,B.Telefon,B.Eposta, B.KiralamaAmaci
				  ORDER BY B.Adi ");
			query.Sql = sb.ToString();
			DataTable dataTable = dao.SelectFromDb(query, "");
			return dataTable;
		}
		public DataTable SelectByFilterReturnDataTable(string filter)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT A.*, B.*,
					B.Id SozlesmeId
				FROM Kiraci_Table A
					INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id AND B.Id in (SELECT Top 1 Id FROM KiraSozlesme_Table WHERE KiraciId=A.Id ORDER BY SozBitTar DESC)
				WHERE Adi like @Filter
					OR TCKimlikNo like @Filter
					OR Telefon like @Filter
					OR Adres like @Filter
				ORDER BY SozBasTar DESC,Aktif DESC, KiraciId");
			query.AddParameter("@Filter", "%" + filter + "%");
			DataTable dataTable = null;
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
		public string SelectByFilter(string filter)
		{
			SqlQuery query = new SqlQuery(@"
				SELECT 
					Id KiraciId, Adi,Soyadi, TCKimlikNo, Ili , Ilcesi, Adres,
					Telefon
				FROM Kiraci_Table
				WHERE Adi like @Filter
					OR TCKimlikNo like @Filter
					OR Telefon like @Filter
					OR Adres like @Filter
				ORDER BY KiraciId");
			query.AddParameter("@Filter", "%" + filter + "%");
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
        public Kiraci SelectNext()
        {
            Kiraci kiraci = new Kiraci();
            SqlQuery query = new SqlQuery(@"SELECT * FROM Kiraci_Table
                    WHERE Id > @Id
                    ORDER BY Id");
            query.AddParameter("@Id", Id);

            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = new SqlQuery(@"
                    SELECT * FROM Kiraci_Table
                    WHERE Id < @Id
                    ORDER BY Id DESC");
            query.AddParameter("@Id", Id);


            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Kiraci_Table
                ORDER BY Id DESC");
            DataTable dataTable = dao.SelectFromDb(query, "");
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
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Kiraci_Table
                ORDER BY Id");
            DataTable dataTable = dao.SelectFromDb(query, "");
            if (dataTable != null)
            {
                List<Kiraci> list = ToList<Kiraci>(dataTable);
                kiraci = list.FirstOrDefault();
            }
            return kiraci;
        }

		public List<Kiraci> SelectByAdi(string adi, string soyadi = "")
		{
			StringBuilder sb = new StringBuilder(@"
				SELECT A.*
				FROM Kiraci_Table A
				INNER JOIN KiraSozlesme_Table B ON B.KiraciId=A.Id AND B.Id IN (SELECT MAX(Id) FROM KiraSozlesme_Table WHERE KiraciId=A.Id GROUP BY KiraciId) --Sözeslmesi yeni olan önce gelsin
				WHERE Adi Like @Adi ");
			SqlQuery query = new SqlQuery();
			query.AddParameter("@Adi", "%" + adi.Trim() + "%");
			if (!string.IsNullOrEmpty(soyadi))
			{
				sb.Append(" AND Soyadi LIKE @Soyadi ");
				query.AddParameter("@Soyadi", "%" + soyadi + "%");
			}
			sb.Append(" ORDER BY B.SozBasTar DESC ");
			query.Sql = sb.ToString();

			DataTable dataTable = dao.SelectFromDb(query, "");
			List<Kiraci> list = ToList<Kiraci>(dataTable);

			return (list);
		}
    }
}
