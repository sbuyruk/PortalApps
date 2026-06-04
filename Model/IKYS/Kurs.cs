
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Kurs : ParentClass
    {
        public int PersonelId { get; set; }
        public string KursAdi { get; set; }
        public string VerenKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string Sure { get; set; }
        public string Adres { get; set; }


        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kurs> list = ToList<Kurs>(dataTable);
            Kurs kurs = new Kurs();
            kurs = list.FirstOrDefault();
            return (T)Convert.ChangeType(kurs, typeof(T));
        }

        public override int Save()
        {
            try
            {

                GenericEntity<Kurs> genericEntity = new GenericEntity<Kurs>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_KURS);
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
                Kurs item = Select<Kurs>(Id);
                if (Id != 0)
                {
                    GenericEntity<Kurs> genericEntity = new GenericEntity<Kurs>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_KURS);
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
                    GenericEntity<Kurs> genericEntity = new GenericEntity<Kurs>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    Kurs item = Select<Kurs>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_KURS);
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
            string sqlString = string.Format(@"SELECT *
                               FROM Kurs_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kurs> list = ToList<Kurs>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Kurs_Table 
                                        (PersonelId, Sure, KursAdi,VerenKurum, Adres,Tarih,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7}) ",
                                        PersonelId.ReturnQuotedValue(), Sure.ReturnQuotedValue(), KursAdi.ReturnQuotedValue(), VerenKurum.ReturnQuotedValue(),
                                        Adres.ReturnQuotedValue(), Tarih.ReturnTRDateFormat(), Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Kurs_Table 
                                    SET PersonelId = {0},Sure={1}, KursAdi={2}, VerenKurum={3}, Adres={4}, Tarih={5},
                                        Degistiren={6},DegistirmeTarihi={7}
                                        WHERE Id= {8}", PersonelId.ReturnQuotedValue(), Sure.ReturnQuotedValue(), KursAdi.ReturnQuotedValue(),
                                            VerenKurum.ReturnQuotedValue(), Adres.ReturnQuotedValue(), Tarih.ReturnTRDateFormat(), Degistiren.ReturnQuotedValue(),
                                        DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Kurs_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Kurs_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public List<Kurs> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kurs> list = ToList<Kurs>(dataTable);

            return (list);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Kurs_Table  
                    WHERE PersonelId={0}
                    ORDER BY Tarih DESC", pId);
            return sqlstr;
        }

    }
}
