
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class BirimTanim : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public int ParentId { get; set; }
        public int AmirId { get; set; }
        public int Sira { get; set; }
        public bool Aktif { get; set; }
        public bool BirimKaldirildi { get; set; } = false;
        public int BolgeId { get; set; } = ProjeConstants.BOLGE_GENELMUDURLUK_INT;
        public override T Select<T>(int id)
        {
            GenericEntity<BirimTanim> genericEntity = new GenericEntity<BirimTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);
            BirimTanim birimTanim = new BirimTanim();
            birimTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(birimTanim, typeof(T));
        }
        public BirimTanim Select(int id)
        {
            GenericEntity<BirimTanim> genericEntity = new GenericEntity<BirimTanim>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);
            BirimTanim duyuru = new BirimTanim();
            duyuru = list.FirstOrDefault();
            return duyuru;
        }
        public override int Save()
        {
            try
            {               
                GenericEntity<BirimTanim> genericEntity = new GenericEntity<BirimTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM);
                }
                this.Id = id;
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
                BirimTanim item = Select<BirimTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<BirimTanim> genericEntity = new GenericEntity<BirimTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM);
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
                    GenericEntity<BirimTanim> genericEntity = new GenericEntity<BirimTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    BirimTanim item = Select<BirimTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM);
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

                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BirimTanim_Table ORDER BY ParentId, Sira");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<BirimTanim> SelectByBirimKaldirildi(bool birimKaldirildiMi)
        {
            string birimKaldirildiMiStr= string.Format(" WHERE BirimKaldirildi={0}", birimKaldirildiMi ? 1 : 0); 
            string sqlString = string.Format(@"SELECT *
                               FROM BirimTanim_Table 
                               {0}
                               ORDER BY ParentId, Sira",birimKaldirildiMiStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);

            return list;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM BirimTanim_Table 
                               WHERE Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM BirimTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT 
	                A.Id BirimId, A.Adi BirimAdi, A.KisaAdi BirimKisaAdi, 
	                B.Adi UstBirim,
	                ISNULL(C.Adi,'') + ' ' +ISNULL(C.Soyadi,'') BirimAmiri 
                FROM BirimTanim_Table A
                    LEFT JOIN BirimTanim_Table B on B.Id=A.ParentId
                    LEFT JOIN Personel_Table C on C.Id=A.AmirId 
                ORDER BY A.Sira
                                    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = string.Format(@"
                SELECT 
	                A.Id BirimId, A.Adi BirimAdi, A.KisaAdi BirimKisaAdi, A.Sira,
	                B.Adi UstBirim,
	                ISNULL(C.Adi,'') + ' ' +ISNULL(C.Soyadi,'') BirimAmiri 
                FROM BirimTanim_Table A
                    LEFT JOIN BirimTanim_Table B on B.Id=A.ParentId
                    LEFT JOIN Personel_Table C on C.Id=A.AmirId 
                ORDER BY A.Sira
                                    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
        public List<BirimTanim> SelectByParentId(int parentId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM BirimTanim_Table  
                    WHERE ParentId={0}
                    ORDER BY Sira", parentId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);

            return (list);
        }
        public BirimTanim SelectRoot()
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM BirimTanim_Table  
                    WHERE ParentId=0
                    ORDER BY Sira");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BirimTanim> list = ToList<BirimTanim>(dataTable);

            return (list.FirstOrDefault<BirimTanim>());
        }
    }
}
