using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciYakinlari : ParentClass
    {
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string YakinlikDerecesi { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciYakinlari_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);
            BagisciYakinlari bagisciYakinlari = new BagisciYakinlari();
            bagisciYakinlari = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisciYakinlari, typeof(T));

        }
        public override int Save()
        {
            string sqlString = string.Format(@"
                                            INSERT INTO BagisciYakinlari_Table 
                                                (AdSoyad,Telefon, YakinlikDerecesi, BagisciId,Olusturan, OlusturmaTarihi)
                                            VALUES ({0},{1},{2},{3},{4},{5})",
                                AdSoyad.ReturnQuotedValue(), Telefon.ReturnQuotedValue(), YakinlikDerecesi.ReturnQuotedValue(),
                                BagisciId.ReturnQuotedValue(),
                                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());

            int id = dao.Insert(sqlString);

            this.Id = id;
            return id;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = string.Format(@"
                                        UPDATE BagisciYakinlari_Table 
                                        SET AdSoyad={0},Telefon={1},Tarih={2},BagisciId={3},
                                            Degistiren={4},DegistirmeTarihi={5}
                                        WHERE Id={6}",
                              AdSoyad.ReturnQuotedValue(), Telefon.ReturnQuotedValue(),
                              YakinlikDerecesi.ReturnQuotedValue(), BagisciId.ReturnQuotedValue(),
                              Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM BagisciYakinlari_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciYakinlari_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciYakinlari> SelectByBagisciId(int bagisciId)
        {

            string sqlString = string.Format(@"SELECT * FROM BagisciYakinlari_Table
                              WHERE BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciYakinlari> list = ToList<BagisciYakinlari>(dataTable);
            return list;
        }


    }
}
