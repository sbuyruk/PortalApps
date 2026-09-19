using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    [Serializable]
    public class Olay : ParentClass
    {
        public string Program { get; set; }
        public string IslemTipi { get; set; } //Giris-düzeltme-silme
        public string IslemKonusu { get; set; } //faaliyet-tasinmaz-kiraci, sözlesme, nakitbagis vs
        public DateTime IslemTarihi { get; set; }
        public string IslemYapan { get; set; }
        public string Aciklama { get; set; }
        
        public override T Select<T>(int id)
        {
            GenericEntity<Olay> genericEntity = new GenericEntity<Olay>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            //string sqlString = string.Format(@"SELECT *
            //                   FROM Olay_Table 
            //                   WHERE  Id={0}", id);
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Olay> list = ToList<Olay>(dataTable);
            _ = new Olay();
            Olay olay = list.FirstOrDefault();
            return (T)Convert.ChangeType(olay, typeof(T));

        }
        public Olay Select(int id)
        {
            GenericEntity<Olay> genericEntity = new GenericEntity<Olay>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Olay> list = ToList<Olay>(dataTable);
            Olay olay = new Olay();
            olay = list.FirstOrDefault();
            return olay;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Olay> genericEntity = new GenericEntity<Olay>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        
        public List<Olay> SelectByTarihReturnList(DateTime islemTarihi, string program)
        {
            string sqlString;
            if (program.Equals(ProjeConstants.HEPSI) || string.IsNullOrEmpty(program))
            {
                sqlString = string.Format(@"
                    SELECT *
                    FROM Olay_Table 
                    WHERE IslemTarihi >={0} 
                    ORDER BY IslemTarihi DESC", islemTarihi.ConvertToDDMMYYYHHmmFormat().ReturnQuotedValue());
            }
            else 
            {
                sqlString = string.Format(@"
                    SELECT *
                    FROM Olay_Table 
                    WHERE program= {0} AND IslemTarihi>={1} 
                    ORDER BY IslemTarihi DESC", program.ReturnQuotedValue(), islemTarihi.ConvertToDDMMYYYHHmmFormat().ReturnQuotedValue());
            }
            

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Olay> list = ToList<Olay>(dataTable);

            return list;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Olay> genericEntity = new GenericEntity<Olay>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
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
            GenericEntity<Olay> genericEntity = new GenericEntity<Olay>(ProjeConstants.SQL_DELETE);
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            bool isSuccess = dao.DeleteFromDb(query, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Olay_Table
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Olay> list = ToList<Olay>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Olay> SelectOlayListesi()
        {
            string sqlString = string.Format(
                @"
                SELECT  ROW_NUMBER() OVER (ORDER BY Id) AS Sirano,
                    Id ,Baslik, Metin, YayinBasTar, YayinBitTar, Tekrar, Popup,Aktif,
                    FORMAT(YayinBasTar,'dd.MM.yyyy hh:mm') BaslamaTarihi,
                    FORMAT(YayinBitTar,'dd.MM.yyyy hh:mm') BitisTarihi,
                    OlayAlicilari, Aciklama, Resim,
                    Olusturan, OlusturmaTarihi, Degistiren, DegistirmeTarihi
                FROM Olay_Table
                ORDER BY Aktif Desc, YayinBasTar DESC, Popup 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            List<Olay> list = ToList<Olay>(dataTable);

            return (list);
        }
    }
}
