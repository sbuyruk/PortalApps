using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.IKYS;

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
            Id = id;
            return (T)Convert.ChangeType(new YoklamaService().GetById(id), typeof(T));
        }
        public Yoklama Select(int id)
        {
            Id = id;
            return new YoklamaService().GetById(id);
        }
        public override int Save()
        {
            try
            {

                GenericEntity<Yoklama> genericEntity = new GenericEntity<Yoklama>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
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
                    SqlQuery query = genericEntity.GetQueryParametreli(this);

                    Yoklama item = Select<Yoklama>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
            return (List<T>)Convert.ChangeType(new YoklamaService().GetAll(), typeof(List<T>));
        }

        public string SelectAllReturnJson(int personelId)
        {
            return new YoklamaService().GetAllByPersonelIdAsJson(personelId);
        }
        public DataTable SelectAllReturnDataTable(int personelId)
        {
            return new YoklamaService().GetAllByPersonelId(personelId);
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
            return new YoklamaService().GetByPersonelIdAndDate(personelId, bastar, bittar);
        }
    }
}
