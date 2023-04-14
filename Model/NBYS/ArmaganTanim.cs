using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

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

        public override int Save()
        {
            try
            {
                GenericEntity<ArmaganTanim> genericEntity = new GenericEntity<ArmaganTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
                }
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
                if (this != null)
                {
                    ArmaganTanim item = Select<ArmaganTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<ArmaganTanim> genericEntity = new GenericEntity<ArmaganTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
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
                    GenericEntity<ArmaganTanim> genericEntity = new GenericEntity<ArmaganTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    ArmaganTanim item = Select<ArmaganTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);
            ArmaganTanim armaganTanim = new ArmaganTanim();
            armaganTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(armaganTanim, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<ArmaganTanim> SelectAktifArmaganTanim()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM ArmaganTanim_Table
                               WHERE Aktif=1                 
                               ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);

            return (list);
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ArmaganTanim> list = ToList<ArmaganTanim>(dataTable);
            ArmaganTanim armaganTanim = new ArmaganTanim();
            armaganTanim = list.FirstOrDefault();
            return armaganTanim;
        }
    }
}
