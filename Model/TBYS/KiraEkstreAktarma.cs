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
            return new KiraEkstreAktarmaService().Save(this);
        }
        public override bool Update()
        {
            return new KiraEkstreAktarmaService().Update(this);
        }
        public override bool Delete()
        {
            return new KiraEkstreAktarmaService().Delete(this);
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new KiraEkstreAktarmaService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KiraEkstreAktarmaService().GetAll(), typeof(List<T>));
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
            return new KiraEkstreAktarmaService().GetByKiraciAdi(adi);
        }
        public List<KiraEkstreAktarma> SelectByIdList(string idListStr)
        {
            if (!string.IsNullOrEmpty(idListStr))
            {
                List<int> idList = idListStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
                string inClause = string.Join(",", idList.Select((id, idx) => "@Id" + idx));
                SqlQuery query = new SqlQuery(string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi,Id DESC, OdemeTarihi desc, Adi
                ", inClause));
                for (int i = 0; i < idList.Count; i++)
                {
                    query.AddParameter("@Id" + i, idList[i]);
                }
                DataTable dataTable = dao.SelectFromDb(query, "");
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
            return new KiraEkstreAktarmaService().GetByIslemNo(islemNo);
        }
        public List<KiraEkstreAktarma> SelectByColumns(string adi, string soyadi, decimal tutar, DateTime odemeTarihi)
        {
            return new KiraEkstreAktarmaService().GetByColumns(adi, soyadi, tutar, odemeTarihi);
        }
        public List<KiraEkstreAktarma> SelectByEkstreIdList(string idListStr, ref int rowCount)
        {
            if (!string.IsNullOrEmpty(idListStr))
            {
                List<int> idList = idListStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
                string inClause = string.Join(",", idList.Select((id, idx) => "@Id" + idx));
                SqlQuery query = new SqlQuery(string.Format(@"
                SELECT *
                FROM KiraEkstreAktarma_Table
                WHERE Id in ({0})
                ORDER BY AktarildiMi, OdemeTarihi desc, Adi
                ", inClause));
                for (int i = 0; i < idList.Count; i++)
                {
                    query.AddParameter("@Id" + i, idList[i]);
                }
                DataTable dataTable = dao.SelectFromDb(query, "");
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
            DateTime basTar = new DateTime(ucAyOncesi.Year, ucAyOncesi.Month, 1);
            string aktarilanlarHaricStr = aktarilanlarHaric ? " AND AktarildiMi=@AktarildiMi " : "";
            string kiraTeminatDigerStr = kiraTeminatDiger
                ? " AND (OdemeSebebiId IS NULL OR OdemeSebebiId IN (@Diger,@Kira,@KesintiTeminat,@GeciciTeminat,@KiraTeminat)) "
                : "";
            SqlQuery query = new SqlQuery(string.Format(@"
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
                    AND OdemeTarihi >= @BasTar
                {0}
                {1}
                ORDER BY AktarildiMi,  OdemeTarihi desc, A.Id, A.Adi
                ", aktarilanlarHaricStr, kiraTeminatDigerStr));
            query.AddParameter("@BasTar", basTar);
            if (aktarilanlarHaric)
                query.AddParameter("@AktarildiMi", false);
            if (kiraTeminatDiger)
            {
                query.AddParameter("@Diger", ProjeConstants.ODEMESEBEBI_DIGER_INT);
                query.AddParameter("@Kira", ProjeConstants.ODEMESEBEBI_KIRA_INT);
                query.AddParameter("@KesintiTeminat", ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT);
                query.AddParameter("@GeciciTeminat", ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT);
                query.AddParameter("@KiraTeminat", ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT);
            }
            DataTable dataTable = dao.SelectFromDb(query, "");
            rowCount = dataTable != null ? dataTable.Rows.Count : 0;
            return dataTable;
        }
        public DataTable SelectById(int ekstreAktarmaId)
        {
            SqlQuery query = new SqlQuery(@"
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
                WHERE A.Id=@Id
                ORDER BY AktarildiMi,  OdemeTarihi desc, A.Id, A.Adi");
            query.AddParameter("@Id", ekstreAktarmaId);
            DataTable dataTable = dao.SelectFromDb(query, "");
            return dataTable;
        }

		public DataTable SelectTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
		{
			DateTime bastar = new DateTime(tarih.Year, tarih.Month, tarih.Day);
			DateTime bittar = UtilityHelper.TariheSaatEkle(bastar, "23:59");
			string odemeSebebiFilter = odemeSebebiId == ProjeConstants.HEPSI_INT
				? string.Empty
				: " AND (A.odemeSebebiId=@OdemeSebebiId OR C.odemeSebebiId=@OdemeSebebiId) ";
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT 
					A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB, A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA, C.Tutar TutarB,A.Aciklama AciklamaA ,C.Aciklama AciklamaB
				FROM KiraEkstreAktarma_Table A
					LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
					LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
					LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId
					LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
				WHERE A.OdemeTarihi BETWEEN @BasTar AND @BitTar
					{0}
				", odemeSebebiFilter));
			query.AddParameter("@BasTar", bastar);
			query.AddParameter("@BitTar", bittar);
			if (odemeSebebiId != ProjeConstants.HEPSI_INT)
				query.AddParameter("@OdemeSebebiId", odemeSebebiId);
			DataTable dataTable = dao.SelectFromDb(query, "");
			return dataTable;
		}
		public DataTable SelectSumTutarByTarihOdemeSebebi(DateTime tarih, int odemeSebebiId)
		{
			DateTime bastar = new DateTime(tarih.Year, tarih.Month, tarih.Day);
			DateTime bittar = UtilityHelper.TariheSaatEkle(bastar, "23:59");
			string odemeSebebiFilter = odemeSebebiId == ProjeConstants.HEPSI_INT
				? string.Empty
				: " AND (A.odemeSebebiId=@OdemeSebebiId OR C.odemeSebebiId=@OdemeSebebiId) ";
			SqlQuery query = new SqlQuery(string.Format(@"
				SELECT 
					A.OdemeTarihi,A.OdemeSebebiId,B.OdemeSebebi OdemeSebebiA,D.OdemeSebebi OdemeSebebiB, A.Adi AdiA,E.Adi AdiB,E.Soyadi,A.Tutar TutarA, C.Tutar TutarB,A.Aciklama AciklamaA ,C.Aciklama AciklamaB
				FROM KiraEkstreAktarma_Table A
					LEFT JOIN OdemeSebebiTanim_Table B ON B.Id=A.OdemeSebebiId
					LEFT JOIN OdemeAyristirma_Table C ON C.KiraEkstreAktarmaId=A.Id
					LEFT JOIN OdemeSebebiTanim_Table D ON D.Id=C.OdemeSebebiId
					LEFT JOIN Kiraci_Table E ON E.Id=A.KiraciId
				WHERE A.OdemeTarihi BETWEEN @BasTar AND @BitTar
					{0}
				", odemeSebebiFilter));
			query.AddParameter("@BasTar", bastar);
			query.AddParameter("@BitTar", bittar);
			if (odemeSebebiId != ProjeConstants.HEPSI_INT)
				query.AddParameter("@OdemeSebebiId", odemeSebebiId);
			DataTable dataTable = dao.SelectFromDb(query, "");
			return dataTable;
		}
    }
}
