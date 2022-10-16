
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

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
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            GorevTanim gorevTanim = new GorevTanim();
            gorevTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(gorevTanim, typeof(T));
        }
        public override int Save()
        {
            GenericEntity<GorevTanim> genericEntity = new GenericEntity<GorevTanim>(ProjeConstants.SQL_INSERT);
            string sqlString = genericEntity.GetQuery(this);
            int id = dao.Insert(sqlString);

            this.Id = id;
            return id;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            GenericEntity<GorevTanim> genericEntity = new GenericEntity<GorevTanim>(ProjeConstants.SQL_UPDATE);
            string sqlString = genericEntity.GetQuery(this);
            isSuccess = dao.Update2Db(sqlString);
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
                               FROM GorevTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<GorevTanim> list = ToList<GorevTanim>(dataTable);
            return (list);
        }
        public GorevTanim SelectByGorevId(int gorevId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM GorevTanim_Table  
                    WHERE Id={0}", gorevId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
				SELECT G.Id GorevTanimId, G.Adi GorevAdi, G.KisaAdi GorevKisaAdi, ISNULL(P.Adi,'') + ' ' +ISNULL(P.Soyadi,'') Personel, B.Adi BirimAdi  
				FROM GorevTanim_Table G
				LEFT OUTER JOIN Personel_Table P on P.Id=G.PersonelId AND G.PersonelId = (SELECT TOP 1 PersonelId FROM IsBilgileri_Table WHERE PersonelId=G.PersonelId AND CalismaDurumu=1 )
				LEFT OUTER JOIN BirimTanim_Table B on B.Id=G.BirimId
				WHERE G.Id>0
                                    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
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
	                A.Id GorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
	                ISNULL(B.Adi,'') + ' ' +ISNULL(B.Soyadi,'') Personel, 
	                C.Adi BirimAdi  
                FROM GorevTanim_Table A
                LEFT JOIN Personel_Table B on B.Id=A.PersonelId
                LEFT JOIN BirimTanim_Table C on C.Id=A.BirimId
                WHERE A.Id>0
             ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }
    }
}
