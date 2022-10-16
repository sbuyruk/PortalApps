using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.NBYS
{
    public class ArmaganTanim : ParentClass
    {
        public string Armagan { get; set; }
        public decimal OzelKisiAltLimit { get; set; }
        public decimal OzelKisiUstLimit { get; set; }
        public decimal TuzelKisiAltLimit { get; set; }
        public decimal TuzelKisiUstLimit { get; set; }
        public string ImzaGorevi { get; set; }
        public string ImzaAdi { get; set; }

        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM ArmaganTanim_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }

        public override int Save()
        {
            string sqlString = string.Format(@"INSERT INTO ArmaganTanim_Table
                              (Armagan,OzelKisiAltLimit,OzelKisiUstLimit,TuzelKisiAltLimit,TuzelKisiUstLimit,ImzaGorevi,ImzaAdi,Olusturan,OlusturmaTarihi)
                               VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8})",
                           Armagan.ReturnQuotedValue(), OzelKisiAltLimit.ReturnQuotedValue(), OzelKisiUstLimit.ReturnQuotedValue(), TuzelKisiAltLimit.ReturnQuotedValue(), TuzelKisiUstLimit.ReturnQuotedValue(), ImzaGorevi.ReturnQuotedValue(), ImzaAdi.ReturnQuotedValue(), Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());


            int id = dao.Insert(sqlString);

            return id;
        }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);
            ArmaganTanim armaganTanim = new ArmaganTanim();
            armaganTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(armaganTanim, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<ArmaganTanim> SelectAktifArmaganTanim()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table
                               WHERE Aktif=1                 
                               ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);

            return (list);
        }


        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = string.Format(@"UPDATE ArmaganTanim_Table 
                              SET Armagan={0},OzelKisiAltLimit={1},OzelKisiUstLimit={2},TuzelKisiAltLimit={3},TuzelKisiUstLimit={4},ImzaGorevi={5},ImzaAdi={6},Degistiren={7},DegistirmeTarihi={8}
                               WHERE Id={9}",
                              Armagan.ReturnQuotedValue(), OzelKisiAltLimit.ReturnQuotedValue(), OzelKisiUstLimit.ReturnQuotedValue(), TuzelKisiAltLimit.ReturnQuotedValue(), TuzelKisiUstLimit.ReturnQuotedValue(), ImzaGorevi.ReturnQuotedValue(), ImzaAdi.ReturnQuotedValue(), Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }

        public ArmaganTanim SelectByTutar(decimal tutar, bool tuzelKisiMi)
        {
            string sqlString = string.Empty;

            if (tuzelKisiMi)
            {
                sqlString = string.Format(@"SELECT *
                                        FROM ArmaganTanim_Table
                                        WHERE Aktif=1 
                                            AND TuzelKisiAltLimit <= {0} AND TuzelKisiUstLimit>={0}", tutar.ConvertDecimalToString());
            }
            else
            {
                sqlString = string.Format(@"SELECT *
                                        FROM ArmaganTanim_Table
                                         WHERE Aktif=1 
                                            AND OzelKisiAltLimit <= {0} AND OzelKisiUstLimit>={0}", tutar.ConvertDecimalToString());
            }
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);
            ArmaganTanim armaganTanim = new ArmaganTanim();
            armaganTanim = list.FirstOrDefault();
            return armaganTanim;
        }
        public ArmaganTanim SelectByArmagan(string armagan)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM ArmaganTanim_Table
                WHERE Aktif=1 
                    AND Armagan LIKE  '%{0}' ", armagan);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);
            ArmaganTanim armaganTanim = new ArmaganTanim();
            armaganTanim = list.FirstOrDefault();
            return armaganTanim;
        }
    }
}
