
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IsBilgileri : ParentClass
    {
        public int PersonelId { get; set; }
        public int BolgeId { get; set; }
        public int UnvanId { get; set; }
        public int GorevId { get; set; }
        public int BirimId { get; set; }
        public DateTime BaslamaTar { get; set; }
        public DateTime IzinDonemiBasTar { get; set; }
        public int CalismaDurumu { get; set; }
        public DateTime AyrilmaTar { get; set; }
        public string AyrilmaSebebi { get; set; }
        public int ProtokolSiraNo { get; set; }
        public string SGKSicilNo { get; set; }
        public DateTime SGKBasTar { get; set; }
        public int VakifOncesiPrimGunSayisi { get; set; }
        public bool SGKDestekPrimi { get; set; }
        public DateTime EmeklilikTarihi { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri isBilgileri = new IsBilgileri();
            isBilgileri = list.FirstOrDefault();
            return (T)Convert.ChangeType(isBilgileri, typeof(T));
        }
        public IsBilgileri Select(int id)
        {
            GenericEntity<IsBilgileri> genericEntity = new GenericEntity<IsBilgileri>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri isBilgileri = new IsBilgileri();
            isBilgileri = list.FirstOrDefault();
            return isBilgileri;
        }
        public override int Save()
        {
            string sqlString = saveSQL();
            int id = dao.Insert(sqlString);
            this.Id = id;
            return id;
        }

        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = UpdateSQL();
                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }

        public override bool Delete()
        {
            string sqlString = DeleteSQL();

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IsBilgileri_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IsBilgileri_Table 
                                        (PersonelId, UnvanId, GorevId, BirimId, BaslamaTar,IzinDonemiBasTar, CalismaDurumu, AyrilmaTar, AyrilmaSebebi, 
                                         ProtokolSiraNo,SGKSicilNo,SGKBasTar,VakifOncesiPrimGunSayisi,SGKDestekPrimi,EmeklilikTarihi,
                                         Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}) ",
                                    PersonelId, UnvanId.ReturnQuotedValue(), GorevId.ReturnQuotedValue(), BirimId.ReturnQuotedValue(),
                                    BaslamaTar.ReturnTRDateFormat(), IzinDonemiBasTar.ReturnTRDateFormat(), CalismaDurumu.ReturnQuotedValue(), AyrilmaTar.ReturnTRDateFormat(),
                                    AyrilmaSebebi.ReturnQuotedValue(), ProtokolSiraNo.ReturnQuotedValue(), SGKSicilNo.ReturnQuotedValue(),
                                    SGKBasTar.ReturnQuotedValue(), VakifOncesiPrimGunSayisi.ReturnQuotedValue(), SGKDestekPrimi.ReturnEmptyIfNull().ReturnQuotedValue(),
                                    EmeklilikTarihi.ReturnTRDateFormat(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE IsBilgileri_Table 
                                    SET PersonelId = {0}, UnvanId= {1}, GorevId= {2}, BirimId= {3}, BaslamaTar= {4},IzinDonemiBasTar={5},
                                        CalismaDurumu= {6}, AyrilmaTar= {7}, AyrilmaSebebi= {8}, ProtokolSiraNo= {9},SGKSicilNo= {10},
                                        SGKBasTar= {11},VakifOncesiPrimGunSayisi= {12},SGKDestekPrimi={13},EmeklilikTarihi={14},
                                        Degistiren= {15},Degistirmetarihi= {16}
                                    WHERE Id= {17}",
                                    PersonelId, UnvanId.ReturnQuotedValue(), GorevId.ReturnQuotedValue(), BirimId.ReturnQuotedValue(),
                                    BaslamaTar.ReturnTRDateFormat(), IzinDonemiBasTar.ReturnTRDateFormat(), CalismaDurumu.ReturnQuotedValue(), AyrilmaTar.ReturnTRDateFormat(),
                                    AyrilmaSebebi.ReturnQuotedValue(), ProtokolSiraNo.ReturnQuotedValue(), SGKSicilNo.ReturnQuotedValue(),
                                    SGKBasTar.ReturnQuotedValue(), VakifOncesiPrimGunSayisi.ReturnQuotedValue(), SGKDestekPrimi.ReturnQuotedValue(),
                                    EmeklilikTarihi.ReturnTRDateFormat(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        public IsBilgileri SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsBilgileri> list = ToList<IsBilgileri>(dataTable);
            IsBilgileri ib = list.FirstOrDefault();
            return (ib);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IsBilgileri_Table  
                    WHERE PersonelId={0}
                    ORDER BY UnvanId", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IsBilgileri_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IsBilgileri_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public DataTable SelectAllFromIS_YERI_BILGILERI()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IS_YERI_BILGILERI");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return (dataTable);
        }
    }
}
