using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciTalepleri : ParentClass
    {
        public string Talep { get; set; }
        public string Irtibat { get; set; }
        public string Tarih { get; set; }
        public string Aciklama { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciTalepleri_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);
            BagisciTalepleri bagisciTalepleri = new BagisciTalepleri();
            bagisciTalepleri = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisciTalepleri, typeof(T));

        }
        public override int Save()
        {
            string sqlString = string.Format(@"
                                            INSERT INTO BagisciTalepleri_Table 
                                                (Talep,Irtibat,Tarih, BagisciId,Aciklama,Olusturan, OlusturmaTarihi)
                                            VALUES ({0},{1},{2},{3},{4},{5},{6})",
                                Talep.ReturnQuotedValue(), Irtibat.ReturnQuotedValue(), Tarih.ReturnQuotedValue(),
                                BagisciId.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
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
                                        UPDATE BagisciTalepleri_Table 
                                        SET Talep={0},Irtibat={1},Tarih={2},BagisciId={3},Aciklama={4},
                                            Degistiren={5},DegistirmeTarihi={6}
                                        WHERE Id={7}",
                              Talep.ReturnQuotedValue(), Irtibat.ReturnQuotedValue(), Tarih.ReturnQuotedValue(),
                              BagisciId.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                              Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM BagisciTalepleri_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisciTalepleri_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagisciTalepleri> SelectByBagisciId(int bagisciId)
        {

            string sqlString = string.Format(@"SELECT * FROM BagisciTalepleri_Table
                              WHERE BagisciId={0}", bagisciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagisciTalepleri> list = ToList<BagisciTalepleri>(dataTable);
            return list;
        }


    }
}
