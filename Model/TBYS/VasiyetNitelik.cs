using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    public class VasiyetNitelik : ParentClass
    {
        public int VasiyetciId { get; set; }
        public string Cinsi { get; set; }
        public string Niteligi { get; set; }
        public decimal TahminiRayic { get; set; }



        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<VasiyetNitelik> genericEntity = new GenericEntity<VasiyetNitelik>(ProjeConstants.SQL_DELETE);
                    //OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    bool isDeleted = dao.DeleteFromDb(sqlString, this);
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

        public override int Save()
        {
            try
            {
                GenericEntity<VasiyetNitelik> genericEntity = new GenericEntity<VasiyetNitelik>(ProjeConstants.SQL_INSERT);
                //OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public VasiyetNitelik Select(int id)
        {
            GenericEntity<VasiyetNitelik> genericEntity = new GenericEntity<VasiyetNitelik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyetNitelik> list = ToList<VasiyetNitelik>(dataTable);
            VasiyetNitelik item = new VasiyetNitelik();
            item = list.FirstOrDefault();
            return item;
        }

        public override T Select<T>(int id)
        {
            GenericEntity<VasiyetNitelik> genericEntity = new GenericEntity<VasiyetNitelik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyetNitelik> list = ToList<VasiyetNitelik>(dataTable);
            VasiyetNitelik item = new VasiyetNitelik();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyetNitelik_Table ORDER BY VasiyetciId
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyetNitelik> list = ToList<VasiyetNitelik>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<VasiyetNitelik> SelectByVasiyetciId(int vasiyetciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyetNitelik_Table 
                WHERE VasiyetciId={0}
                ORDER BY VasiyetciId
                ", vasiyetciId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyetNitelik> list = ToList<VasiyetNitelik>(dataTable);

            return list;
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
    }
}