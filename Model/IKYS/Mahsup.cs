
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Mahsup : ParentClass
    {
        public int PersonelId { get; set; }
        public int IzinHareketId { get; set; }
        public int IzinTipi { get; set; }
        public int KullanildigiDonemId { get; set; }
        public int MahsupDonemId { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Mahsup> list = ToList<Mahsup>(dataTable);
            Mahsup mahsup = new Mahsup();
            mahsup = list.FirstOrDefault();
            return (T)Convert.ChangeType(mahsup, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<Mahsup> genericEntity = new GenericEntity<Mahsup>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
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
                Mahsup item = Select<Mahsup>(Id);
                if (Id != 0)
                {
                    GenericEntity<Mahsup> genericEntity = new GenericEntity<Mahsup>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
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
                    GenericEntity<Mahsup> genericEntity = new GenericEntity<Mahsup>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    Mahsup item = Select<Mahsup>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_MAHSUP);
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
                               FROM Mahsup_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Mahsup> list = ToList<Mahsup>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Mahsup_Table 
                                        (PersonelId,IzinHareketId,IzinTipi,KullanildigiDonemId,MahsupDonemId,Aciklama,
                                        Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7}) ",
                                    PersonelId.ReturnQuotedValue(), IzinHareketId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(),
                                    KullanildigiDonemId.ReturnQuotedValue(), MahsupDonemId.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE Mahsup_Table 
                                    SET PersonelId={0}, IzinHareketId={1}, IzinTipi={2},KullanildigiDonemId={3}, 
                                        MahsupDonemId={4}, Aciklama={5},
                                        Degistiren={6},DegistirmeTarihi={7}
                                    WHERE Id= {8}",
                                    PersonelId.ReturnQuotedValue(), IzinHareketId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(),
                                    KullanildigiDonemId.ReturnQuotedValue(), MahsupDonemId.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Mahsup_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Mahsup_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

        public List<Mahsup> SelectByDonemId(int donemId)
        {
            string sqlString = SelectByDonemSQL(donemId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Mahsup> list = ToList<Mahsup>(dataTable);
            return (list);
        }
        public List<Mahsup> SelectByPersonelId(int personelId, int izinTipi)
        {
            string sqlString = SelectByPersonelSQL(personelId, izinTipi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Mahsup> list = ToList<Mahsup>(dataTable);
            return (list);
        }
        private string SelectByPersonelSQL(int personelId, int izinTipi)
        {
            string andStr = izinTipi == 0 ? "" : izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT ? string.Format(" AND IzinTipi={0}", izinTipi) : " AND IzinTipi!=2 ";//sadece mazeret iznini ayrı göster

            string sqlstr = string.Format(@" 
                    SELECT * FROM Mahsup_Table  
                    WHERE PersonelId={0} {1} ", personelId, andStr);
            return sqlstr;
        }
        private string SelectByDonemSQL(int donemId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Mahsup_Table  
                    WHERE KullanildigiDonemId={0} OR MahsupDonemId={0}", donemId);
            return sqlstr;
        }
    }
}
