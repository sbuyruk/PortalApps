using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    public class Vasiyetci : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string SagVefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public int IkametIli { get; set; }
        public int IkametIlcesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string DogumYeri { get; set; }
        public string IkametAdresi { get; set; }
        public string VasiyetTipi { get; set; }
        //public string SorumluBolge { get; set; }
        public string VasiyetinDurumu { get; set; }
        public string Noter { get; set; }
        public DateTime VasiyetTarihi { get; set; }
        public string YevmiyeNumarasi { get; set; }
        public string VasiyetcininTalebi { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<Vasiyetci> genericEntity = new GenericEntity<Vasiyetci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
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
                    Vasiyetci item = Select<Vasiyetci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Vasiyetci> genericEntity = new GenericEntity<Vasiyetci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
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
                    GenericEntity<Vasiyetci> genericEntity = new GenericEntity<Vasiyetci>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Vasiyetci item = Select<Vasiyetci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETCI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Vasiyetci Select(int id)
        {
            GenericEntity<Vasiyetci> genericEntity = new GenericEntity<Vasiyetci>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Vasiyetci> list = ToList<Vasiyetci>(dataTable);
            Vasiyetci item = new Vasiyetci();
            item = list.FirstOrDefault();
            return item;
        }

        public override T Select<T>(int id)
        {
            GenericEntity<Vasiyetci> genericEntity = new GenericEntity<Vasiyetci>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Vasiyetci> list = ToList<Vasiyetci>(dataTable);
            Vasiyetci item = new Vasiyetci();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Vasiyetci_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Vasiyetci> list = ToList<Vasiyetci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectByBolgeReturnDataTable(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND B.BolgeId={0} ", bolgeId);
            string sqlString = string.Format(@"
                SELECT A.*, B.IlAdi, C.IlceAdi, D.KisaAdi Bolge
                FROM Vasiyetci_Table A
                INNER JOIN Il_Table B on A.IkametIli = B.Id
                LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                INNER JOIN Bolge_Table D on D.Id = B.BolgeId
                {0}
                ORDER BY Adi
                ", bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

        public string SelectVasiyetciByIdReturnJson(int vasiyetciId, ref int rowCount)
        {
            string sqlString = string.Format(@"
                SELECT A.*,A.Id VasiyetciId, B.IlAdi, C.IlceAdi FROM Vasiyetci_Table A
                LEFT JOIN Il_Table B on A.IkametIli = B.Id
                LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                WHERE A.Id={0} 
                ORDER BY A.Id
                ", vasiyetciId);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
                rowCount = dataTable.Rows.Count;
            }
            catch (Exception e)
            {
                throw;
            }

            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectAllVasiyetciReturnDataTable(bool VefatEdenVasiyetcilerHaric, ref int rowCount)
        {
            string VefatEdenVasiyetcilerHaricStr = string.Empty;
            if (VefatEdenVasiyetcilerHaric)
            {
                VefatEdenVasiyetcilerHaricStr = string.Format(@" WHERE SAGVEFAT={0} ", ProjeConstants.BAGISCI_SAG_INT);
            }
            string sqlString = string.Format(@"
                SELECT A.*,A.Id VasiyetciId, B.IlAdi, C.IlceAdi, D.KisaAdi Bolge FROM Vasiyetci_Table A
                    LEFT JOIN Il_Table B on A.IkametIli = B.Id
                    LEFT JOIN Ilce_Table C on A.IkametIlcesi = C.Id
                    LEFT JOIN Bolge_Table D on D.Id = B.BolgeId
                {0}                                                    
                ", VefatEdenVasiyetcilerHaricStr);

            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
                rowCount = dataTable.Rows.Count;
            }
            catch (Exception e)
            {
                throw;
            }
            return dataTable;
        }
        public List<Vasiyetci> SelectByFilters(bool SadeceSagOlanlar, bool isTCKimlikNoFull, bool isDogumTarihiFull)
        {
            string TCKimlikNoStr = isTCKimlikNoFull ? string.Format(" AND TCKimlikNo IS NOT NULL AND TCKimlikNo > 0 ") : string.Empty;
            string dogumTarihiStr = isDogumTarihiFull ? string.Format(" AND DogumTarihi IS NOT NULL AND DogumTarihi!='' AND DogumTarihi>'01.01.1900' ") : string.Empty;
            string sagVefatStr = SadeceSagOlanlar ? string.Format(" AND SagVefat!={0}", ProjeConstants.BAGISCI_VEFAT_INT) : string.Empty;
            string whereStr = TCKimlikNoStr + dogumTarihiStr + sagVefatStr;
            string sqlString = string.Format(@"
                SELECT *
                FROM Vasiyetci_Table A
                WHERE 1>0
                {0}", whereStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Vasiyetci> list = ToList<Vasiyetci>(dataTable);

            return (list);
        }
    }
}