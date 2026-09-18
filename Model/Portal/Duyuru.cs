using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    [Serializable]
    public class Duyuru : ParentClass
    {
        public string Baslik { get; set; }
        public string Metin { get; set; }
        public DateTime YayinBasTar { get; set; }
        public DateTime YayinBitTar { get; set; }
        public string Tekrar { get; set; }
        public string DuyuruAlicilari { get; set; }
        public string Resim { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
        public bool Popup { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Duyuru_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);
            Duyuru duyuru = new Duyuru();
            duyuru = list.FirstOrDefault();
            return (T)Convert.ChangeType(duyuru, typeof(T));

        }
        public Duyuru Select(int id)
        {
            GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);
            Duyuru duyuru = new Duyuru();
            duyuru = list.FirstOrDefault();
            return duyuru;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_INSERT);
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
        public List<Duyuru> SelectByTarihReturnList(DateTime now)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Duyuru_Table 
                WHERE  YayinBasTar <= {0} AND YayinBitTar >={0}
                ORDER BY YayinBasTar,YayinBitTar DESC", now.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);

            return list;
        }
		public List<Duyuru> SelectByTarihReturnList(DateTime now, string tekrar)
		{
			if (string.IsNullOrWhiteSpace(tekrar))
			{
				return new List<Duyuru>();
			}

			tekrar = tekrar.Trim();
			string sqlString = string.Empty;

			if (tekrar.Equals(ProjeConstants.DUYURU_TEKRAR_YOK))
			{
				sqlString = string.Format(@"
					SELECT *
					FROM Duyuru_Table 
					WHERE Aktif=1 AND Tekrar= {0} AND YayinBasTar <= {1} AND YayinBitTar >={1}
					ORDER BY YayinBasTar,YayinBitTar DESC", tekrar.ReturnQuotedValue(), now.ReturnTRDateFormat());
			}
			else if (tekrar.Equals(ProjeConstants.DUYURU_TEKRARLA_YIL))
			{
				string saatDakika = now.ToString("HH:MM:SS");
				string gun = now.Day.ToString();
				string ay = now.Month.ToString();
				sqlString = string.Format(@"
					SELECT *
					FROM Duyuru_Table 
					WHERE Aktif=1 AND Tekrar= {0}
							AND CONVERT(nvarchar,YayinBasTar,108)<={1} 
							AND CONVERT(nvarchar,YayinBitTar,108)>={1}  
							AND DAY(YayinBasTar) <=  {2}  AND DAY(YayinBitTar) >= {2} 
							AND MONTH(YayinBasTar) <={3} AND MONTH(YayinBitTar) >={3} 
					ORDER BY YayinBasTar,YayinBitTar DESC"
				, tekrar.ReturnQuotedValue(), saatDakika.ReturnQuotedValue(), gun.ReturnQuotedValue(), ay.ReturnQuotedValue());
			}
			else if (tekrar.Equals(ProjeConstants.DUYURU_TEKRARLA_AY))
			{
				string saatDakika = now.ToString("HH:MM:SS");
				string gun = now.Day.ToString();
				string ay = now.Month.ToString();
				sqlString = string.Format(@"
					SELECT *
					FROM Duyuru_Table 
					WHERE Aktif=1 AND Tekrar= {0}
							AND	CONVERT(nvarchar,YayinBasTar,108)<={1} 
							AND CONVERT(nvarchar,YayinBitTar,108)>={1} 
							AND DAY(YayinBasTar) <=  {2} AND DAY(YayinBitTar) >=  {2} 
					ORDER BY YayinBasTar,YayinBitTar DESC"
				, tekrar.ReturnQuotedValue(), saatDakika.ReturnQuotedValue(), gun.ReturnQuotedValue());
			}
			else if (tekrar.Equals(ProjeConstants.DUYURU_TEKRARLA_HAFTA))
			{
				string saatDakika = now.ToString("HH:MM:SS");
				sqlString = string.Format(@"
					SELECT*
					FROM Duyuru_Table 
					WHERE Aktif=1 AND Tekrar= {0}
							AND	CONVERT(nvarchar,YayinBasTar,108)<={1} 
							AND CONVERT(nvarchar,YayinBitTar,108)>={1} 
							AND DATEPART(dw,YayinBasTar) = DATEPART(dw,GETDATE())
					ORDER BY YayinBasTar,YayinBitTar DESC"
				, tekrar.ReturnQuotedValue(), saatDakika.ReturnQuotedValue());
			}
			else
			{
				return new List<Duyuru>();
			}

			DataTable dataTable = dao.SelectFromDb(sqlString, "");
			List<Duyuru> list = ToList<Duyuru>(dataTable);

			return list;
		}
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
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
            string sqlString = string.Format(@"DELETE 
                               FROM Duyuru_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Duyuru_Table
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Duyuru> SelectDuyuruListesi()
        {
            string sqlString = string.Format(
                @"
                SELECT  ROW_NUMBER() OVER (ORDER BY Id) AS Sirano,
                    Id ,Baslik, Metin, YayinBasTar, YayinBitTar, Tekrar, Popup,Aktif,
                    FORMAT(YayinBasTar,'dd.MM.yyyy hh:mm') BaslamaTarihi,
                    FORMAT(YayinBitTar,'dd.MM.yyyy hh:mm') BitisTarihi,
                    DuyuruAlicilari, Aciklama, Resim,
                    Olusturan, OlusturmaTarihi, Degistiren, DegistirmeTarihi
                FROM Duyuru_Table
                ORDER BY Aktif Desc, YayinBasTar DESC, Popup 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            List<Duyuru> list = ToList<Duyuru>(dataTable);

            return (list);
        }
    }
}
