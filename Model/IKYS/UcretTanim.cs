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

        public int GrupId { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Unvan { get; set; }
        public decimal AltUcret { get; set; }
        public decimal UstUcret { get; set; }
        public decimal AskerUcret { get; set; }
        public DateTime BaslangicTarihi{ get; set; }
        public DateTime BitisTarihi{ get; set; }
        public decimal Agi{ get; set; }
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

                throw;
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

                throw;
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
        public List<UcretTanim> SelectByKademe(int grupId, int kademe)
        {
            
            string sqlString = string.Format(@"
                SELECT *
                FROM UcretTanim_Table
                WHERE GrupId={0} AND Kademe={1} 
                ORDER BY Derece,Kademe", grupId, kademe);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);

            return (list);
        } 
        public List<UcretTanim> SelectByMaxGrupId()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM UcretTanim_Table
                WHERE GrupId = (
                    SELECT MAX(GrupId) FROM UcretTanim_Table
                )
                ORDER BY Derece,Kademe");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<UcretTanim> list = ToList<UcretTanim>(dataTable);

            return (list);
        } 
        public DataTable SelectByGrup()
        {
            string sqlString = string.Format(@"
                SELECT GrupId, 
                    MAX(BaslangicTarihi) AS BaslangicTarihi,
                    MAX(BitisTarihi) AS BitisTarihi
                FROM 
                    UcretTanim_Table
                WHERE 
                    BaslangicTarihi IS NOT NULL
                GROUP BY GrupId
                ORDER BY GrupId DESC

                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return (dataTable);
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

        public DataTable SelectKademe(int derece,int grupId)
        {
            string sqlString = string.Format(@"
                SELECT DISTINCT Kademe
                FROM UcretTanim_Table
                WHERE Derece={0} AND GrupId={1}
                ORDER BY Kademe",derece,grupId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

        public DataTable SelectMaasListesi(int grupId, DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT
                    A.Id AS PersonelId,
                    A.Adi,
                    A.Soyadi,
                    B.KisaAdi Unvan,
                    E.Derece,
                    E.Kademe,
                    C.ProtokolSiraNo,
                    E.DegisimTarihi DereceKademeIlerlemeTarihi,
                    {0} GrupId,   
                    D.Agi,
                    CASE 
                        WHEN A.Asker_Sivil = 1 THEN D.AskerUcret
                        ELSE D.UstUcret
                    END AS Ucret
                FROM Personel_Table A
                LEFT JOIN GorevTanim_Table B ON B.PersonelId = A.Id
                LEFT JOIN IsBilgileri_Table C ON C.PersonelId = A.Id
                
				LEFT JOIN DereceKademeDegisim_Table E ON E.PersonelId=A.Id AND E.DegisimTarihi = (SELECT Max(DegisimTarihi) 
					FROM DereceKademeDegisim_Table
					WHERE PersonelId=A.Id AND DegisimTarihi <= {1})
                LEFT JOIN UcretTanim_Table D ON D.Derece = E.Derece AND D.Kademe = E.Kademe AND D.GrupId={0}
                WHERE C.CalismaDurumu = 1 AND Tipi=1 
                ORDER BY C.ProtokolSiraNo;

            ", grupId,tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
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

        public int SelectGrupIdByTarih(DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT MAX(GrupId) GrupId from UcretTanim_Table 
                WHERE BaslangicTarihi <={0} AND BitisTarihi>={0}
            ", tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            int grupId = dataTable.Rows[0][0].ConvertToInt();
            return grupId;
        }

        public bool DeleteByGrupId(int grupId)
        {
            //GrupId ile eslesen tüm kayitlari siler
            string sqlString = string.Format(@"
                DELETE FROM UcretTanim_Table
                WHERE GrupId={0}", grupId);
            bool isDeleted = dao.DeleteFromDb(sqlString, "");
            if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_MAASARTISI);
            }
            return isDeleted;
        }

        public decimal SelectUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe)
        {
            string sqlString =string.Empty;
            if (personel.Asker_sivil == ProjeConstants.PER_ASKER_INT)
            {
                sqlString = string.Format(@"
                SELECT AskerUcret FROM UcretTanim_Table
                WHERE GrupId={0} AND Derece={1} AND Kademe={2}", grupId, derece, kademe);
            }
            else
            {
                sqlString = string.Format(@"
                SELECT UstUcret FROM UcretTanim_Table
                WHERE GrupId={0} AND Derece={1} AND Kademe={2}", grupId, derece, kademe);
                //GrupId, Derece ve Kademe ile eslesen Ucret degerini döner
            }
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable.Rows.Count > 0)
            {
                decimal ucret = dataTable.Rows[0][0].ConvertToDecimal();
                return ucret;
            }
            else
            {
                return SqlDecimal.Null.Value;
            }
        }
    }
}
