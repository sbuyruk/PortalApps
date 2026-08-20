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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Yoklama> list = ToList<Yoklama>(dataTable);
            Yoklama yoklama = new Yoklama();
            yoklama = list.FirstOrDefault();
            return (T)Convert.ChangeType(yoklama, typeof(T));
        }
        public Yoklama Select(int id)
        {
            GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
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
                Yoklama item = Select<Yoklama>(Id);
                if (Id != 0)
                {
                    GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
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
                    GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    Yoklama item = Select<Yoklama>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
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
                               FROM Yoklama_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Yoklama> list = ToList<Yoklama>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
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
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
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
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
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
                ORDER BY ProtokolSiraNo,BaslangicTarihi ", bastar.ReturnTRDateFormat(), bittar.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

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

                DataTable dataTable = dao.SelectFromDb(sqlString, "");
                List<Yoklama> list = ToList<Yoklama>(dataTable);

                return list;
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
