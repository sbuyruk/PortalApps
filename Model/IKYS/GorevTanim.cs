
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

namespace Model.IKYS
{
    public class GorevTanim : ParentClass
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }
        public int HarcirahGrupId { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            GorevTanim gorevTanim = new GorevTanim();
            gorevTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(gorevTanim, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<GorevTanim> genericEntity = new GenericEntity<GorevTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
                }
                this.Id = id;
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
                GorevTanim item = Select<GorevTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<GorevTanim> genericEntity = new GenericEntity<GorevTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
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
                    GenericEntity<GorevTanim> genericEntity = new GenericEntity<GorevTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    GorevTanim item = Select<GorevTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVTANIM);
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

                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM GorevTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM GorevTanim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM GorevTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

        public GorevTanim SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            GorevTanim gorevTanim = new GorevTanim();
            gorevTanim = list.FirstOrDefault();
            return gorevTanim;
        }
        public List<GorevTanim> SelectByBirimId(int birimId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM GorevTanim_Table  
                    WHERE BirimId={0}
                    ORDER BY Id", birimId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            return (list);
        }
        public GorevTanim SelectByGorevId(int gorevId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM GorevTanim_Table  
                    WHERE Id={0}", gorevId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            GorevTanim gorev = new GorevTanim();
            gorev = list.FirstOrDefault<GorevTanim>();
            return (gorev);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM GorevTanim_Table  
                    WHERE PersonelId={0}
                    ORDER BY Id", pId);
            return sqlstr;
        }
        
        public DataTable SelectAllReturnDataTable(PersonelTipi personelTipi = PersonelTipi.Kadrolu)
        {
            int personelTipiInt = (int)personelTipi;
            string personelTipiStr = personelTipi == PersonelTipi.Tumu ? string.Empty : string.Format(" AND Tipi={0}", personelTipiInt);
            string sqlString = string.Format(@"
                SELECT
	                A.Id GorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
	                ISNULL(B.Adi,'') + ' ' +ISNULL(B.Soyadi,'') Personel, 
	                C.Adi BirimAdi  
                FROM GorevTanim_Table A
                LEFT JOIN Personel_Table B on B.Id=A.PersonelId {0}
                LEFT JOIN BirimTanim_Table C on C.Id=A.BirimId
                WHERE A.Id>0
                Order BY C.Sira,A.Id
             ", personelTipiStr);
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
    }
}
