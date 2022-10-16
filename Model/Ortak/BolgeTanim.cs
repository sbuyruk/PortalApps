using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class BolgeTanim : ParentClass
    {
        public string BolgeAdi { get; set; }
        public string BolgeKisaAdi { get; set; }
        public override int Save()
        {
            throw new NotImplementedException();
        }
        public override bool Update()
        {
            throw new NotImplementedException();
        }
        public override bool Delete()
        {
            throw new NotImplementedException();
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BolgeTanim_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BolgeTanim> list = ToList<BolgeTanim>(dataTable);
            BolgeTanim bolgeTanim = new BolgeTanim();
            bolgeTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(bolgeTanim, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BolgeTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BolgeTanim> list = ToList<BolgeTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public BolgeTanim Select(int id)
        {
            GenericEntity<BolgeTanim> genericEntity = new GenericEntity<BolgeTanim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BolgeTanim> list = ToList<BolgeTanim>(dataTable);
            BolgeTanim bolgeTanim = new BolgeTanim();
            bolgeTanim = list.FirstOrDefault();
            return bolgeTanim;
        }
        public BolgeTanim SelectByPersonelId(int id)
        {
            GenericEntity<BolgeTanim> genericEntity = new GenericEntity<BolgeTanim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BolgeTanim> list = ToList<BolgeTanim>(dataTable);
            BolgeTanim bolgeTanim = new BolgeTanim();
            bolgeTanim = list.FirstOrDefault();
            return bolgeTanim;
        }
    }
}
