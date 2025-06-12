using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class UcretTanim : ParentClass
    {

        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Unvan { get; set; }
        public decimal AltUcret { get; set; }
        public decimal UstUcret { get; set; }
        public decimal AskerUcret { get; set; }
        public DateTime BaslangicTarihi{ get; set; }
        public DateTime BitisTarihi{ get; set; }
        public override T Select<T>(int id)
        {
            GenericEntity<UcretTanim> genericEntity = new GenericEntity<UcretTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);
            UcretTanim ucretTanim = new UcretTanim();
            ucretTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(ucretTanim, typeof(T));
        }
       
        public UcretTanim Select(int id)
        {
            GenericEntity<UcretTanim> genericEntity = new GenericEntity<UcretTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);
            UcretTanim ucretTanim = new UcretTanim();
            ucretTanim = list.FirstOrDefault();
            return ucretTanim;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<UcretTanim> genericEntity = new GenericEntity<UcretTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
                }
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
                UcretTanim item = Select<UcretTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<UcretTanim> genericEntity = new GenericEntity<UcretTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<UcretTanim> genericEntity = new GenericEntity<UcretTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    UcretTanim item = Select<UcretTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
                    }
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM UcretTanim_Table 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        } 
        public List<UcretTanim> SelectByKademe(DateTime tarih, int kademe)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM UcretTanim_Table
                WHERE BaslangicTarihi<={0} AND BitisTarihi>={0} AND Kademe={1} 
                ORDER BY Derece,Kademe", tarih.ReturnTRDateFormat(), kademe);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);

            return (list);
        } 
        public List<UcretTanim> SelectByBaslangicTarihiBitistarihi(DateTime baslangicTarihi,DateTime bitisTarihi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM UcretTanim_Table
                WHERE  BaslangicTarihi>={0} AND BitisTarihi<={1} 
                ORDER BY Derece,Kademe", baslangicTarihi.ReturnTRDateFormat(),bitisTarihi.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);

            return (list);
        }
        public DataTable SelectDerece()
        {
            string sqlString = string.Format(@"
                SELECT DISTINCT Derece, Unvan
                FROM UcretTanim_Table
                ORDER BY Derece");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

        public DataTable SelectKademe(int derece)
        {
            string sqlString = string.Format(@"
                SELECT DISTINCT Kademe
                FROM UcretTanim_Table
                WHERE Derece={0}
                ORDER BY Kademe",derece);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

        public DataTable SelectMaasListesi()
        {
            string sqlString = string.Format(@"
                SELECT 
                    A.Adi,
                    A.Soyadi,
                    B.KisaAdi Unvan,
                    C.Derece,
                    C.ProtokolSiraNo,
                    C.Kademe,
                    C.DereceKademeIlerlemeTarihi,
                    CASE 
                        WHEN A.Asker_Sivil = 1 THEN D.AskerUcret
                        ELSE D.UstUcret
                    END AS Ucret
                FROM Personel_Table A
                LEFT JOIN GorevTanim_Table B ON B.PersonelId = A.Id
                LEFT JOIN IsBilgileri_Table C ON C.PersonelId = A.Id
                LEFT JOIN UcretTanim_Table D ON D.Derece = C.Derece AND D.Kademe = C.Kademe
                WHERE C.CalismaDurumu = 1 AND Tipi=1 
                ORDER BY C.ProtokolSiraNo;

            ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }

        public DateTime SelectMaxBaslangicTarihi()
        {
            string sqlString = string.Format(@"
                SELECT MAX(BaslangicTarihi) from UcretTanim_Table
            ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            DateTime maxTarih = dataTable.Rows[0][0].ConvertToDatetime();
            return maxTarih;
        }        
        public DateTime SelectMaxBitisTarihi()
        {
            string sqlString = string.Format(@"
                SELECT MAX(BitisTarihi) from UcretTanim_Table
            ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            DateTime maxTarih = dataTable.Rows[0][0].ConvertToDatetime();
            return maxTarih;
        }
        public decimal SelectAgi(DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT Top 1 Agi from UcretTanim_Table where BitisTarihi={0}
            ",tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            decimal agi = dataTable.Rows[0][0].ConvertToDecimal();
            return agi;
        }

        public List<Array> SelectArtisTarihleri()
        {
            throw new NotImplementedException();
        }
    }
}
