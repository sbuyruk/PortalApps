using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.TBYS
{
    [Serializable]
    public class BagimsizBolum : ParentClass
    {
        public int TasinmazId { get; set; }
        public string BolumNo { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagimsizBolum_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            BagimsizBolum bagimsizBolum = new BagimsizBolum();
            bagimsizBolum = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagimsizBolum, typeof(T));

        }
        public override int Save()
        {
            string sqlString = string.Format(@"
                                            INSERT INTO BagimsizBolum_Table 
                                                (TasinmazId, BolumNo, Aciklama,Olusturan, OlusturmaTarihi)
                                            VALUES ({0},{1},{2},{3},{4})",
                                            TasinmazId.ReturnQuotedValue(), BolumNo.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
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
                                        UPDATE BagimsizBolum_Table 
                                        SET TasinmazId={0}, BolumNo={1}, Aciklama={2},Degistiren={3},DegistirmeTarihi={4}
                                        WHERE Id={5}",
                                            TasinmazId.ReturnQuotedValue(), BolumNo.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                            Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM BagimsizBolum_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagimsizBolum_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BagimsizBolum> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public List<BagimsizBolum> SelectByBolumNO(string bolumNo)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumNo={0}", bolumNo.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            return list;
        }
        public BagimsizBolum SelectByBolumId(int bolumId)
        {
            string sqlString = string.Format(@"SELECT * FROM BagimsizBolum_Table
                              WHERE bolumId={0}", bolumId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BagimsizBolum> list = ToList<BagimsizBolum>(dataTable);
            BagimsizBolum bolum = new BagimsizBolum();
            bolum = list.FirstOrDefault();
            return bolum;
        }
    }
}
