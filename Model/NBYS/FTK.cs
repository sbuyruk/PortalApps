using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class FTK : ParentClass
    {

        public int FTKIslemId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public int BolgeId { get; set; }
        public DateTime KurulusTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string FTKGorevi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvani { get; set; }
        public string Telefon { get; set; }
        public string KartNo { get; set; }
        public int Sayac { get; set; }
        public int KisiId { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<FTK> genericEntity = new GenericEntity<FTK>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_FTK);
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
                    FTK item = Select<FTK>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FTK> genericEntity = new GenericEntity<FTK>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTK);
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
                    GenericEntity<FTK> genericEntity = new GenericEntity<FTK>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    FTK item = Select<FTK>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTK);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public FTK Select(int id)
        {
            GenericEntity<FTK> genericEntity = new GenericEntity<FTK>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTK> list = ToList<FTK>(dataTable);
            FTK item = new FTK();
            item = list.FirstOrDefault();
            return item;
        }
        public DataTable SelectSonFTKListesiByIliIlcesiReturnDataTable(int bolgeId, int ili, int ilcesi, string kurulusTarihi, string guncellemeTarihi)
        {
            string kurulusTarihiStr = string.IsNullOrEmpty(kurulusTarihi) ? string.Empty : string.Format(" AND A.KurulusTarihi>={0} ", kurulusTarihi.ConvertToDatetime().ReturnTRDateFormat());
            string guncellemeTarihiStr = string.IsNullOrEmpty(guncellemeTarihi.ConvertToDatetimeEmptyIfNull()) ? string.Empty : string.Format(" AND A.GuncellemeTarihi>={0} ", guncellemeTarihi.ConvertToDatetime().ReturnTRDateFormat());
            string bolgeStr = bolgeId==ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND B.BolgeId={0} ", bolgeId);
            string iliStr = ili > 0 ? string.Format(" AND A.Ili={0} ", ili) : string.Empty;
            string ilcesiStr = ilcesi == 0 ? string.Empty :
                (ilcesi == ProjeConstants.SADECE_ILCELER_INT ? " AND Ilcesi!=" + ProjeConstants.VALILIK_INT : string.Format(" AND Ilcesi={0} ", ilcesi));

            string sqlString = string.Format(@"
                SELECT A.Id FTKId,D.KisaAdi Bolge, * FROM FTK_Table A
                    LEFT JOIN Il_Table B ON B.Id = A.Ili
                    LEFT JOIN Ilce_Table C ON C.Id = A.Ilcesi AND C.IlId=A.Ili
                    LEFT JOIN Bolge_Table D ON D.Id = B.BolgeId
                WHERE Sayac= (SELECT MAX(Sayac) FROM FTK_Table WHERE FTKIslemId=A.FtkIslemId) --birden fazla guncellenen FTKlarin son guncellemesini dikkate alsin diye
                    {0}
                    {1}
                    {2}
                    {3}
                    {4}
                ORDER BY Ili,Ilcesi,FTKIslemId, KartNo
            ", kurulusTarihiStr, guncellemeTarihiStr, bolgeStr, iliStr, ilcesiStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public FTK SelectByIliIlcesi(int ili,int ilcesi, DateTime guncellemeTarihi)
        {

            string sqlString = string.Format(@"
                SELECT * FROM FTK_Table
                WHERE Ili={0} AND Ilcesi={1} AND GuncellemeTarihi={2}        
            
            ",ili,ilcesi,guncellemeTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTK> list = ToList<FTK>(dataTable);
            FTK item = new FTK();
            item = list.FirstOrDefault();
            return item;
        }
        public List<FTK> SelectSonFTKListesiByIliIlcesiReturnList(int ili, int ilcesi)
        {
            DataTable dataTable = SelectSonFTKListesiByIliIlcesiReturnDataTable(ProjeConstants.HEPSI_INT,ili, ilcesi, string.Empty, string.Empty);
            List<FTK> list = ToList<FTK>(dataTable);
            return list;
        }
        public int SelectMaxSayac(int ili, int ilcesi)
        {

            string sqlString = string.Format(@"
                select MAX(Sayac) Sayac from FTK_Table
                WHERE Ili={0} AND Ilcesi={1}        
            
            ", ili, ilcesi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int sayac = dataTable.Rows[0]["Sayac"].ConvertToInt();
            return sayac;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FTK> genericEntity = new GenericEntity<FTK>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTK> list = ToList<FTK>(dataTable);
            FTK item = new FTK();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public int SelectKuruluOlanIlSayisiByBolgeId(int bolgeId)
        {
            string sqlString = string.Format(@"
                SELECT BolgeId,Ili,Ilcesi
                FROM FTK_Table A
                WHERE  BolgeId={0} AND Ilcesi = {1}  
                GROUP BY BolgeId,Ili,Ilcesi", bolgeId,ProjeConstants.VALILIK_INT);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int adet = dataTable!=null ? dataTable.Rows.Count : 0;
            return adet;
            
        }
        public int SelectKuruluOlanIlceSayisiByBolgeId(int bolgeId)
        {
            string sqlString = string.Format(@"
                SELECT BolgeId,Ili,Ilcesi
                FROM FTK_Table A
                WHERE  Ilcesi > 0 AND BolgeId={0} 
                GROUP BY BolgeId,Ili,Ilcesi", bolgeId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int adet = dataTable != null ? dataTable.Rows.Count : 0;
            return adet;

        }
        public int SelectGuncellenenIlIlceSayisiByBolgeId(int bolgeId, DateTime guncellemeTarihi)
        {
            string sqlString = string.Format(@"
                SELECT BolgeId,Ili,Ilcesi
                FROM FTK_Table A
                WHERE A.Sayac= (SELECT MAX(Sayac) FROM FTK_Table WHERE FTKIslemId=A.FtkIslemId)
                    AND (BolgeId={0} AND GuncellemeTarihi >= {1})
                GROUP BY BolgeId,Ili,Ilcesi", bolgeId, guncellemeTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int adet = dataTable != null ? dataTable.Rows.Count : 0;
            return adet;
        }
        public int SelectKuruluIlIlceSayisiByBolgeId(int bolgeId, DateTime kurulusTarihi)
        {
            string sqlString = string.Format(@"
                SELECT BolgeId,Ili,Ilcesi
                FROM FTK_Table A
                WHERE BolgeId={0} AND KurulusTarihi >= {1}
                GROUP BY BolgeId,Ili,Ilcesi", bolgeId, kurulusTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int adet = dataTable != null ? dataTable.Rows.Count : 0;            
            return adet;

        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FTK_Table 
                ORDER BY Ili,Ilcesi
                ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTK> list = ToList<FTK>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public bool DeleteByIslemIdSayac(int ftkIslemId,int sayac)
        {
            string sqlString = string.Format(@"
                DELETE FROM FTK_Table
                WHERE  FTKIslemId = {0} AND Sayac = {1}", ftkIslemId,sayac);

            bool isDeleted = dao.DeleteFromDb(sqlString, "");
            return isDeleted;
        }
        public DataTable SelectFTKKuruluOlmayanIller(int bolgeId, int ilId)
        {
            string bolgeStr = bolgeId==ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            string iliStr = ilId < 1 ? string.Empty : string.Format(" AND A.Id={0}", ilId);
            string sqlString = string.Format(@"
                SELECT A.Id IlId,A.BolgeId, C.Adi Bolge, A.IlAdi 
                FROM Il_Table A 
                LEFT JOIN Bolge_Table C ON C.Id= A.BolgeId
                WHERE (A.Id BETWEEN 0 AND 81 AND A.Id>0 AND A.IlAdi != 'Bos') 
                    {0}
                    {1}
                    AND  A.Id NOT IN (SELECT Ili FROM FTK_Table WHERE Ilcesi={2}) 
                ORDER BY A.BolgeId, A.Id    
            ", bolgeStr, iliStr, ProjeConstants.VALILIK_INT);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectFTKKuruluOlmayanIlceler(int bolgeId, int ilId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            string iliStr = ilId < 1 ? string.Empty : string.Format(" AND B.Id={0}", ilId);
            string sqlString = string.Format(@"
                SELECT A.Id IlceId,A.IlceAdi, C.Adi Bolge, B.Id IlId, B.IlAdi 
                FROM Ilce_Table A 
	                INNER JOIN Il_Table B ON B.Id= A.IlId
                    LEFT JOIN Bolge_Table C ON C.Id= B.BolgeId
                WHERE A.IlceAdi!= {0} 
                    AND (B.Id BETWEEN 0 AND 81 AND B.IlAdi != 'Bos') 
                    AND  A.Id NOT IN (SELECT Ilcesi FROM FTK_Table WHERE Ilcesi > 0) 
                    {1}
                    {2}
                    
                ORDER BY B.BolgeId, B.Id, A.Id     
            ", ProjeConstants.ILCE_MERKEZ.ReturnQuotedValue(), bolgeStr, iliStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public DataTable SelectFTKUyeleriByIliIlcesiReturnDataTable(int ili, int ilcesi)
        {
            string sqlString = string.Format(@"
                SELECT  Id FTKId, KisiId FTKKisiId,
                    *
                FROM FTK_Table    
                WHERE Ili={0} AND Ilcesi= {1}
                ORDER BY FTKGorevi,Adi,Soyadi 
            ", ili, ilcesi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

    }
}
