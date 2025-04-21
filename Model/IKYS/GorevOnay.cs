using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class GorevOnay : ParentClass
    {

        public int PersonelId { get; set; }
        public string GorevinSebebi { get; set; }
        public string GorevinYeri { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Avans { get; set; }
        public string Yevmiye { get; set; }
        public string ParaBirimi { get; set; }
        public string UlasimAraci { get; set; }
        public bool AracTahsisi { get; set; }
        public string AracPlakasi { get; set; }
        public int PerSubeImza { get; set; }
        public bool PerSubeVekil { get; set; }
        public int OnayImza { get; set; }
        public int OnayMakam { get; set; }
        public bool OnayMakamVekil { get; set; }
        public int GMImza { get; set; }
        public bool GMVekil { get; set; }
        public string Aciklama { get; set; }
        public bool Secildi { get; set; }
        public override T Select<T>(int id)
        {
            GenericEntity<GorevOnay> genericEntity = new GenericEntity<GorevOnay>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevOnay> list = ToList<GorevOnay>(dataTable);
            GorevOnay yoklama = new GorevOnay();
            yoklama = list.FirstOrDefault();
            return (T)Convert.ChangeType(yoklama, typeof(T));
        }
       
        public GorevOnay Select(int id)
        {
            GenericEntity<GorevOnay> genericEntity = new GenericEntity<GorevOnay>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevOnay> list = ToList<GorevOnay>(dataTable);
            GorevOnay gorevOnay = new GorevOnay();
            gorevOnay = list.FirstOrDefault();
            return gorevOnay;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<GorevOnay> genericEntity = new GenericEntity<GorevOnay>(ProjeConstants.SQL_INSERT);
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
                GorevOnay item = Select<GorevOnay>(Id);
                if (Id != 0)
                {
                    GenericEntity<GorevOnay> genericEntity = new GenericEntity<GorevOnay>(ProjeConstants.SQL_UPDATE);
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
                    GenericEntity<GorevOnay> genericEntity = new GenericEntity<GorevOnay>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    GorevOnay item = Select<GorevOnay>(Id);
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
            string sqlString = string.Format(@"SELECT *
                               FROM GorevOnay_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevOnay> list = ToList<GorevOnay>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string SaveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO GorevOnay_Table 
                                        (PersonelId,GorevinSebebi,GorevinYeri,BaslangicTarihi,BitisTarihi,Sure,Avans,Yevmiye,ParaBirimi,
                                        AracTahsisi,AracPlakasi,PerSubeImza,PerSubeVekil,OnayImza,OnayMakam,OnayMakamVekil,GMImza,GMVekil, Aciklama,
                                        Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}) ",
                                    PersonelId.ReturnQuotedValue(), GorevinSebebi.ReturnQuotedValue(), GorevinYeri.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(),
                                    Sure.ReturnQuotedValue(), Avans.ReturnQuotedValue(), Yevmiye.ReturnQuotedValue(), ParaBirimi.ReturnQuotedValue(),
                                    AracTahsisi.ReturnQuotedValue(), AracPlakasi.ReturnQuotedValue(), PerSubeImza.ReturnQuotedValue(),
                                    PerSubeVekil.ReturnQuotedValue(), OnayImza.ReturnQuotedValue(), OnayMakam.ReturnQuotedValue(), OnayMakamVekil.ReturnQuotedValue(),
                                    GMImza.ReturnQuotedValue(), GMVekil.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }

        public bool UpdateAllSecildiToFalse()
        {
            string sqlString = string.Format(@"
                    UPDATE GorevOnay_Table
                    SET Secildi=0
                ");
            return dao.Update2Db(sqlString); ;
        }
        public bool UpdateAllSecildiToTrue(string idString)
        {
            string sqlString = string.Format(@"
                    UPDATE GorevOnay_Table
                    SET Secildi=1
                    WHERE ID IN ({0})
                ", idString);
            return dao.Update2Db(sqlString);
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM GorevOnay_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM GorevOnay_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public string SelectAllReturnJson(int personelId)
        {

            string sqlString = SelectAllSQL(personelId);
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
        public DataTable SelectAllReturnDT(int personelId)
        {
            string sqlString = SelectAllSQL(personelId);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");

            }
            catch (Exception e)
            {
                throw e;
            }

            return (dataTable);
        }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, 
	                A.PersonelId, A.BaslangicTarihi, A.BitisTarihi,A.Sure,A.GorevinYeri GidilecekYer,A.GorevinSebebi,
                    C.BirimId,D.KisaAdi GorevYeri, D.Vekil
                FROM GorevOnay_Table A
	                INNER JOIN Personel_Table B ON B.Id= A.PersonelId
				    LEFT JOIN IsBilgileri_Table C ON C.PersonelId= A.PersonelId
				    INNER JOIN GorevTanim_Table D ON D.Id= C.GorevId
                WHERE --BitisTarihi>GETDATE() AND 
                    BaslangicTarihi<={0} AND BitisTarihi>={1}
                ORDER BY ProtokolSiraNo,BitisTarihi, BaslangicTarihi ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");


            return dataTable;
        }
        public GorevOnay SelectByPersonelTarih(int personelId, DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"

                SELECT *
                FROM GorevOnay_Table A
                WHERE PersonelId={0} 
                    AND BaslangicTarihi<={1} AND BitisTarihi>={2}
                ORDER BY BitisTarihi DESC
            ", personelId, bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevOnay> list = ToList<GorevOnay>(dataTable);
            GorevOnay gorevOnay = new GorevOnay();
            gorevOnay = list.FirstOrDefault<GorevOnay>();
            return gorevOnay;

        }
        private string SelectAllSQL(int personelId)
        {
            string personelIdStr = personelId > 0 ? string.Format(" WHERE PersonelId={0}",personelId):string.Empty;
            string sqlstr = string.Format(@" 
                    SELECT A.Id GorevOnayId, P.Adi+' '+P.Soyadi AdiSoyadi, A.Secildi,A.UlasimAraci,
                        A.PersonelId,A.GorevinSebebi,A.GorevinYeri,A.BaslangicTarihi,A.BitisTarihi,A.Sure,A.Avans,A.Yevmiye,A.ParaBirimi,A.Sure,
                        A.AracTahsisi,A.AracPlakasi,A.PerSubeImza,A.PerSubeVekil,A.OnayImza,A.OnayMakam,A.OnayMakamVekil,A.GMImza,A.GMVekil, A.Aciklama
                    FROM GorevOnay_Table A
                        INNER JOIN Personel_Table P On A.PersonelId=P.Id 
                    {0}
                    ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC ", personelIdStr);
            return sqlstr;
        }

        public List<GorevOnay> SelectAllBySecildi(bool secildi)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM GorevOnay_Table
                WHERE Secildi={0}",secildi.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevOnay> list = ToList<GorevOnay>(dataTable);

            return list;
        }
    }
}
