using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Yoklama : ParentClass
    {

        public int PersonelId { get; set; }
        public int BulunmamaSebebi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Aciklama { get; set; }
        public string Adres { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Yoklama> list = ToList<Yoklama>(dataTable);
            Yoklama yoklama = new Yoklama();
            yoklama = list.FirstOrDefault();
            return (T)Convert.ChangeType(yoklama, typeof(T));
        }
        public Yoklama Select(int id)
        {
            GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Yoklama> list = ToList<Yoklama>(dataTable);
            Yoklama yoklama = new Yoklama();
            yoklama = list.FirstOrDefault();
            return yoklama;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
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
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_UPDATE);
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
        //public override int Save()
        //{
        //    string sqlString = saveSQL();
        //    int id = dao.Insert(sqlString);
        //    this.Id = id;
        //    return id;
        //}
        //public override bool Update()
        //{
        //    bool isSuccess = false;
        //    if (Id != 0)
        //    {
        //        string sqlString = UpdateSQL();
        //        isSuccess = dao.Update2Db(sqlString);
        //    }
        //    return isSuccess;
        //}
        public override bool Delete()
        {
            string sqlString = DeleteSQL();

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Yoklama_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Yoklama> list = ToList<Yoklama>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Yoklama_Table 
                                        (PersonelId,BulunmamaSebebi,BaslangicTarihi,BitisTarihi,Aciklama,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6}) ",
                                    PersonelId.ReturnQuotedValue(), BulunmamaSebebi.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(), Aciklama.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE Yoklama_Table 
                                    SET PersonelId={0}, BulunmamaSebebi={1}, BaslangicTarihi={2},  BitisTarihi={3}, Aciklama={4},
                                        Degistiren={5},DegistirmeTarihi={6}
                                    WHERE Id= {7}",
                                    PersonelId.ReturnQuotedValue(), BulunmamaSebebi.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(), Aciklama.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Yoklama_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Yoklama_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public string SelectAllReturnJson(int personelId)
        {
            string sqlString = SelectAllSQL(personelId);
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
        public DataTable SelectAllReturnDataTable(int personelId)
        {
            string sqlString = SelectAllSQL(personelId);
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
        private string SelectAllSQL(int personelId)
        {
            string personelIdStr = personelId > 0 ? " WHERE PersonelId=" + personelId : "";
            string sqlstr = string.Format(@" 
                    SELECT A.Id YoklamaId, P.Adi+' '+P.Soyadi AdiSoyadi, B.Adi BulunmamaSebebi, A.PersonelId,A.BaslangicTarihi,A.BitisTarihi,
                           A.Aciklama
                    FROM Yoklama_Table A
                        INNER JOIN Personel_Table P On A.PersonelId=P.Id     
						INNER JOIN BulunmamaSebebi_Table B On B.Id=A.BulunmamaSebebi  
                    {0}   
                    ORDER BY A.BaslangicTarihi DESC, A.BitisTarihi DESC ", personelIdStr);
            return sqlstr;
        }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, A.Aciklama, 
                    A.PersonelId, A.BaslangicTarihi, A.BitisTarihi,A.BulunmamaSebebi BulunmamaSebebiId ,C.Adi BulunmamaSebebi,C.Id BulunmamaSebebiInt,
                    D.BirimId,E.KisaAdi GorevYeri
                FROM Yoklama_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN BulunmamaSebebi_Table C ON C.Id= A.BulunmamaSebebi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE BaslangicTarihi<={0} AND BitisTarihi>={1}
                ORDER BY BaslangicTarihi, D.ProtokolSiraNo ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public DataTable SelectByTarihReturnDataTable(string bulunmamaSbebiIds, DateTime bastar, DateTime bittar)
        {
            string sqlString = string.Format(@"
                SELECT B.Adi+' ' + B.Soyadi AdiSoyadi, A.Aciklama, 
                    A.BaslangicTarihi, A.BitisTarihi,A.BulunmamaSebebi BulunmamaSebebiId ,C.Adi BulunmamaSebebi,C.Id BulunmamaSebebiInt,
                    D.BirimId,E.KisaAdi GorevYeri,D.ProtokolSiraNo
                FROM Yoklama_Table A
                    INNER JOIN Personel_Table B ON B.Id= A.PersonelId
                    INNER JOIN BulunmamaSebebi_Table C ON C.Id= A.BulunmamaSebebi
                    LEFT JOIN IsBilgileri_Table D ON D.PersonelId= A.PersonelId
                    INNER JOIN BirimTanim_Table E ON E.Id= D.BirimId
                WHERE A.BulunmamaSebebi in ({0}) AND BaslangicTarihi BETWEEN {1} AND {2}
                ORDER BY D.ProtokolSiraNo, BaslangicTarihi ", bulunmamaSbebiIds, bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public List<Yoklama> SelectByPersonelIdTarih(int personelId, DateTime bastar, DateTime bittar)
        {
            try
            {
                string sqlString = string.Format(@"
                SELECT *
                FROM Yoklama_Table
                WHERE PersonelId={0}
                    AND BulunmamaSebebi=3 --Görevli
					AND  (BitisTarihi >= {1} AND BaslangicTarihi <= {2}) ", personelId.ToString(), bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

                DataTable dataTable = dao.selectFromDb(sqlString, "");
                List<Yoklama> list = ToList<Yoklama>(dataTable);

                return list;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
