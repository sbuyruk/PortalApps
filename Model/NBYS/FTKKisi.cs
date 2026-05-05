using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class FTKKisi : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Unvani { get; set; }
        public bool Vali { get; set; }     
        public bool Kaymakam { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public string KartNo { get; set; }
        public int FTKGorevi { get; set; }
        public string UyelikDurumu { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<FTKKisi> genericEntity = new GenericEntity<FTKKisi>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
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
                    FTKKisi item = Select<FTKKisi>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FTKKisi> genericEntity = new GenericEntity<FTKKisi>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
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
                    GenericEntity<FTKKisi> genericEntity = new GenericEntity<FTKKisi>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    FTKKisi item = Select<FTKKisi>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKKISI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FTKKisi Select(int id)
        {
            GenericEntity<FTKKisi> genericEntity = new GenericEntity<FTKKisi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi item = new FTKKisi();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FTKKisi> genericEntity = new GenericEntity<FTKKisi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi item = new FTKKisi();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FTKKisi_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT A.*, B.IlAdi, C.IlceAdi FROM FTKKisi_Table A
                LEFT JOIN Il_Table B on A.Ili = B.Id
                LEFT JOIN Ilce_Table C on A.Ilcesi = C.Id
                ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT A.Id KisiId, A.Adi,A.Soyadi
                FROM FTKKisi_Table A");
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
        public FTKKisi SelectByAdiSoyadi(string adi, string soyadi)
        {
            adi = string.IsNullOrEmpty(adi) ? "#${}?" : adi;
            soyadi = string.IsNullOrEmpty(soyadi) ? "#${}?" : soyadi;
            string sqlString = string.Format(@"
                SELECT * 
                FROM FTKKisi_Table
                WHERE Adi ={0} AND Soyadi ={1}
                ", adi.Trim().ReturnQuotedValue(), soyadi.Trim().ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi ftkKisi = new FTKKisi();
            ftkKisi = list.FirstOrDefault();
            return ftkKisi;
        }
        public FTKKisi SelectByTCKimlikNo(long tckimlik)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM FTKKisi_Table
                WHERE TCKimlikNo={0}
                ", tckimlik);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi ftkKisi = new FTKKisi();
            ftkKisi = list.FirstOrDefault();
            return ftkKisi;
        }    
        public DataTable SelectFTKUyeleriByIliIlcesiReturnDataTable(int ili, int ilcesi, int ftkislemId, bool aktifOlmayanlariGosterme)
        {
            string aktifStr = aktifOlmayanlariGosterme ? " AND UyelikDurumu=" + ProjeConstants.FTK_UYELIK_DURUMU_AKTIF.ReturnQuotedValue() : string.Empty;
            //   (aktif.Equals(ProjeConstants.FTKKISI_DURUMU_PASIF) ? " WHERE BitisTarihi < GETDATE()" : string.Empty);

            string ftkislemIdStr = ftkislemId == ProjeConstants.HEPSI_INT ? "" :
                 string.Format(" AND A.Id={0}", ftkislemId);

            string sqlString = string.Format(@"
                SELECT  A.Id FTKIslemId, C.Id Id, C.Id FTKKisiId,
                    A.*, C.*
                FROM FTKKisi_Table C   
	                LEFT JOIN FTKIslem_Table A ON A.Ili = C.Ili AND A.Ilcesi=C.Ilcesi
                WHERE C.Ili={0} AND C.Ilcesi= {1}
                {2}
                {3}
                ORDER BY C.FTKGorevi,C.Adi,C.Soyadi 
            ", ili, ilcesi, ftkislemIdStr,aktifStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public List<FTKKisi> SelectFTKUyeleriByIliIlcesiReturnList(int ili, int ilcesi, int ftkislemId, bool aktifOlmayanlariGosterme)
        {

            DataTable dataTable = SelectFTKUyeleriByIliIlcesiReturnDataTable(ili, ilcesi, ftkislemId, aktifOlmayanlariGosterme);
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            return (list);
        }
        public FTKKisi SelectVali(int ili)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FTKKisi_Table 
                WHERE Vali=1 AND Ili={0} AND Ilcesi={1}
            ",ili,ProjeConstants.VALILIK_INT);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi ftkKisi = new FTKKisi();
            ftkKisi = list.FirstOrDefault();
            return ftkKisi;
        }
        public FTKKisi SelectKaymakam(int ili, int ilcesi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FTKKisi_Table 
                WHERE Kaymakam=1 AND Ili={0} AND Ilcesi={1}
            ", ili,ilcesi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKKisi> list = ToList<FTKKisi>(dataTable);
            FTKKisi ftkKisi = new FTKKisi();
            ftkKisi = list.FirstOrDefault();
            return ftkKisi;
        }

        public bool UpdateAktifByIdList(string idler)
        {
            string sqlString = string.Format(@"
                UPDATE FTKKisi_Table
                SET UyelikDurumu={0}
                WHERE Id in {1}", ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL.ReturnQuotedValue(), idler);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
    }
}
