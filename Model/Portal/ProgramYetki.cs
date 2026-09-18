using DAO.Ortak;
using DocumentFormat.OpenXml.Office2010.Excel;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    public class ProgramYetki : ParentClass
    {
        public int BirimId { get; set; }
        public string Program { get; set; }
        public string Modul { get; set; }
        public string Kosul { get; set; }
        public bool Deger { get; set; }
        
        public override T Select<T>(int id)
        {
            GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ProgramYetki> list = ToList<ProgramYetki>(dataTable);
            ProgramYetki programYetki = new ProgramYetki();
            programYetki = list.FirstOrDefault();
            return (T)Convert.ChangeType(programYetki, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_SELECT);
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ProgramYetki> list = ToList<ProgramYetki>(dataTable);
            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override int Save()
        {
            try
            {
                GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_INSERT);
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
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_UPDATE);
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
            GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_DELETE);
            string sqlString = genericEntity.GetQuery(this);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }

        public List<ProgramYetki> SelectByProgram(string program)
        {
            string wherestr = string.Format("WHERE Program={0}", program);
            GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_SELECT);
            string sqlString = genericEntity.GetQuery(this,wherestr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ProgramYetki> list = ToList<ProgramYetki>(dataTable);
            return (List<ProgramYetki>)Convert.ChangeType(list, typeof(List<ProgramYetki>));
        }
        public List<ProgramYetki> SelectByProgramModul(string program, string modul, string birimId)
        {
            string wherestr = string.Format("WHERE Program={0} AND Modul={1} AND BirimId IN {2}", program.ReturnQuotedValue(), modul.ReturnQuotedValue(),birimId);
            GenericEntity<ProgramYetki> genericEntity = new GenericEntity<ProgramYetki>(ProjeConstants.SQL_SELECT);
            string sqlString = genericEntity.GetQuery(this,wherestr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ProgramYetki> list = ToList<ProgramYetki>(dataTable);
            return (List<ProgramYetki>)Convert.ChangeType(list, typeof(List<ProgramYetki>));
        }
    }
}
