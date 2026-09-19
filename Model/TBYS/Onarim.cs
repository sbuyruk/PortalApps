using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class Onarim : ParentClass
    {
        public int TasinmazId { get; set; }
        public string YapilanIs { get; set; }
        public string HarcamaUsulu { get; set; }
        public DateTime OnayTarihi { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Onarim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            Onarim onarim = new Onarim();
            onarim = list.FirstOrDefault();
            return (T)Convert.ChangeType(onarim, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<Onarim> genericEntity = new GenericEntity<Onarim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
                }
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
                if (this != null)
                {
                    Onarim item = Select<Onarim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Onarim> genericEntity = new GenericEntity<Onarim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
                    }
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
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<Onarim> genericEntity = new GenericEntity<Onarim>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    Onarim item = Select<Onarim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Onarim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Onarim> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT *
                    FROM Onarim_Table A
                WHERE TasinmazId={0}
                ORDER BY TasinmazId", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                return list;
            }
            else
            {
                return null;
            }

        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = SelectAllString();
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            return dataTable;

        }
        private string SelectAllString()
        {
            string sqlString = string.Format(@"
                SELECT A.Id OnarimId, A.TasinmazId,A.YapilanIs,A.HarcamaUsulu,A.OnayTarihi,
                    Convert(nvarchar,replace (A.Tutar,'.',',')) as Tutar, A.Aciklama,
                    B.Adres, B.Ili,B.Ilcesi, B.Ili+' '+B.Ilcesi IliIlcesi
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                ORDER BY OnayTarihi DESC  
                            ");
            return sqlString;
        }
        public List<Onarim> SelectByOnarimId(int onarimId)
        {
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                              WHERE Id={0}", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            return list;
        }
        public Onarim SelectNext(int onarimId)
        {
            Onarim onarim = new Onarim();
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                                            WHERE Id > {0}
                                            ORDER BY Id ", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                onarim = list.FirstOrDefault();
            }
            else
            {
                onarim = SelectMin();

            }
            return onarim;
        }
        public Onarim SelectPrev(int onarimId)
        {
            Onarim onarim = new Onarim();
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                                            WHERE Id < {0}
                                            ORDER BY Id ", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                onarim = list.FirstOrDefault();
            }
            else
            {
                onarim = SelectMax();

            }
            return onarim;
        }
        public Onarim SelectMax()
        {
            string sqlString = string.Format(@"SELECT MAX(Id) Id  FROM Onarim_Table ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int onarimId = row["Id"].ConvertToInt();
                Onarim onarim = new Onarim();
                onarim = onarim.Select<Onarim>(onarimId);
                return onarim;
            }
            else
            {
                return null;
            }
        }

        public List<Onarim> SelectOnarimByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id, A.TasinmazId, A.YapilanIs, convert(decimal(10, 2), A.Tutar) Tutar,
                    A.OnayTarihi,  A.Aciklama,A.HarcamaUsulu, B.Ili, B.Ilcesi,B.Adres
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                WHERE A.TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            return list;
        }

        public Onarim SelectMin()
        {
            string sqlString = string.Format(@"SELECT MIN(Id) Id  FROM Onarim_Table ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int onarimId = row["Id"].ConvertToInt();
                Onarim onarim = new Onarim();
                onarim = onarim.Select<Onarim>(onarimId);
                return onarim;
            }
            else
            {
                return null;
            }
        }
    }
}
