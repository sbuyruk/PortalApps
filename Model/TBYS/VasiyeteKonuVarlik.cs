using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    public class VasiyeteKonuVarlik : ParentClass
    {
        public int VasiyetciId { get; set; }
        public string Konusu { get; set; }
        public string Cinsi { get; set; }
        public string AdetMiktar { get; set; }
        public decimal TahminiRayic { get; set; }
        public string Aciklama { get; set; }



        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_DELETE);
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
                GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_INSERT);
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

        public VasiyeteKonuVarlik Select(int id)
        {
            GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);
            VasiyeteKonuVarlik item = new VasiyeteKonuVarlik();
            item = list.FirstOrDefault();
            return item;
        }

        public override T Select<T>(int id)
        {
            GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);
            VasiyeteKonuVarlik item = new VasiyeteKonuVarlik();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table ORDER BY VasiyetciId
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<VasiyeteKonuVarlik> SelectByVasiyetciId(int vasiyetciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table 
                WHERE VasiyetciId={0}
                ORDER BY VasiyetciId
                ", vasiyetciId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);

            return list;
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
    }
}