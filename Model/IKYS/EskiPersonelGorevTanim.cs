
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

namespace Model.IKYS
{
    public class EskiPersonelGorevTanim : ParentClass
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<EskiPersonelGorevTanim> list = ToList<EskiPersonelGorevTanim>(dataTable);
            EskiPersonelGorevTanim EskiPersonelGorevTanim = new EskiPersonelGorevTanim();
            EskiPersonelGorevTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(EskiPersonelGorevTanim, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<EskiPersonelGorevTanim> genericEntity = new GenericEntity<EskiPersonelGorevTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
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
                EskiPersonelGorevTanim item = Select<EskiPersonelGorevTanim>(Id);
                if (Id != 0)
                {
                    GenericEntity<EskiPersonelGorevTanim> genericEntity = new GenericEntity<EskiPersonelGorevTanim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
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
                    GenericEntity<EskiPersonelGorevTanim> genericEntity = new GenericEntity<EskiPersonelGorevTanim>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);

                    EskiPersonelGorevTanim item = Select<EskiPersonelGorevTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ESKIPERSONELGOREVTANIM);
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
                               FROM EskiPersonelGorevTanim_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<EskiPersonelGorevTanim> list = ToList<EskiPersonelGorevTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM EskiPersonelGorevTanim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM EskiPersonelGorevTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

        public EskiPersonelGorevTanim SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<EskiPersonelGorevTanim> list = ToList<EskiPersonelGorevTanim>(dataTable);
            EskiPersonelGorevTanim EskiPersonelGorevTanim = new EskiPersonelGorevTanim();
            EskiPersonelGorevTanim = list.FirstOrDefault();
            return EskiPersonelGorevTanim;
        }
        public List<EskiPersonelGorevTanim> SelectByBirimId(int birimId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM EskiPersonelGorevTanim_Table  
                    WHERE BirimId={0}
                    ORDER BY Id", birimId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<EskiPersonelGorevTanim> list = ToList<EskiPersonelGorevTanim>(dataTable);
            return (list);
        }
        public EskiPersonelGorevTanim SelectByGorevId(int gorevId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM EskiPersonelGorevTanim_Table  
                    WHERE Id={0}", gorevId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<EskiPersonelGorevTanim> list = ToList<EskiPersonelGorevTanim>(dataTable);
            EskiPersonelGorevTanim gorev = new EskiPersonelGorevTanim();
            gorev = list.FirstOrDefault<EskiPersonelGorevTanim>();
            return (gorev);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM EskiPersonelGorevTanim_Table  
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
	                A.Id EskiPersonelGorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
	                ISNULL(B.Adi,'') + ' ' +ISNULL(B.Soyadi,'') Personel, 
	                C.Adi BirimAdi  
                FROM EskiPersonelGorevTanim_Table A
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
                throw;
            }
            return dataTable;
        }
    }
}
