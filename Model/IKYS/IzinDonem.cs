
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IzinDonem : ParentClass
    {
        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string IzinHakki { get; set; }
        public string KullanilanIzin { get; set; }
        public string KalanIzin { get; set; }
        public string Birim { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);
            IzinDonem izinDonem = new IzinDonem();
            izinDonem = list.FirstOrDefault();
            return (T)Convert.ChangeType(izinDonem, typeof(T));
        }
        public override int Save()
        {
            try
            {

                GenericEntity<IzinDonem> genericEntity = new GenericEntity<IzinDonem>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM);
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
                IzinDonem item = Select<IzinDonem>(Id);
                if (Id != 0)
                {
                    GenericEntity<IzinDonem> genericEntity = new GenericEntity<IzinDonem>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM);
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
                    GenericEntity<IzinDonem> genericEntity = new GenericEntity<IzinDonem>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    IzinDonem item = Select<IzinDonem>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_IZINDONEM);
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
                               FROM IzinDonem_Table
                               ORDER BY BaslangicTarihi DESC");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IzinDonem_Table 
                                        (PersonelId,IzinTipi,BaslangicTarihi,BitisTarihi,IzinHakki,KullanilanIzin,KalanIzin,Birim,Adi,Aciklama,
                                        Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}) ",
                                    PersonelId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(),
                                    BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(),
                                    IzinHakki.ReturnQuotedValue(), KullanilanIzin.ReturnQuotedValue(), KalanIzin.ReturnQuotedValue(), Birim.ReturnQuotedValue(), Adi.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Update  SQL 
            string sqlSQL = string.Format(@"
                                    UPDATE IzinDonem_Table 
                                    SET PersonelId={0}, IzinTipi={1}, BaslangicTarihi={2},BitisTarihi={3}, 
                                        IzinHakki={4}, KullanilanIzin={5}, KalanIzin={6},Birim={7}, Adi={8},Aciklama={9},
                                        Degistiren={10},DegistirmeTarihi={11}
                                    WHERE Id= {12}",
                                    PersonelId.ReturnQuotedValue(), IzinTipi.ReturnQuotedValue(), BaslangicTarihi.ReturnTRDateFormat(), BitisTarihi.ReturnTRDateFormat(),
                                    IzinHakki.ReturnQuotedValue(), KullanilanIzin.ReturnQuotedValue(), KalanIzin.ReturnQuotedValue(), Birim.ReturnQuotedValue(), Adi.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IzinDonem_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IzinDonem_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

        public IzinDonem SelectByIzinTarihi(int personelId, int izinTipi, DateTime tarih)
        {
            string sqlString = SelectByIzinTarihiSQL(personelId, izinTipi, tarih);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);
            IzinDonem izinDonemi = list.FirstOrDefault();
            return (izinDonemi);
        }
        public List<IzinDonem> SelectOncekiYillaraAitIzinDonemleri(int personelId)
        {
            string sqlString = SelectOncekiYillaraAitIzinDonemleriSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);
            return (list);
        }
        public List<IzinDonem> SelectByPersonelId(int personelId, int izinTipi)
        {
            string sqlString = SelectByPersonelSQL(personelId, izinTipi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);
            return (list);
        }
        public DataTable SelectSUMKalanIzinByPersonelId(int personelId,bool sadeceEskiDonemler)
        {
            string sadeceEskiDonemlerStr = sadeceEskiDonemler ? " AND BitisTarihi < GETDATE() " : string.Empty;
            string sqlString = string.Format(@"
                SELECT SUM(CONVERT(INT,IzinHakki)) IzinHakkiToplami,SUM(CONVERT(INT,KullanilanIzin)) KullanilanIzinToplami,SUM(CONVERT(INT,KalanIzin)) KalanIzinToplami
                FROM IzinDonem_Table
                WHERE IzinTipi={0} AND PersonelId={1}
                {2}
                GROUP BY PersonelId
            ", ProjeConstants.IZINTIPI_UCRETLI_INT,personelId, sadeceEskiDonemlerStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return (dataTable);
        }
        public string SelectByPersonelIdReturnJSon(int personelId, int izinTipi)
        {
            string sqlString = SelectByPersonelSQL(personelId, izinTipi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IzinDonem> list = ToList<IzinDonem>(dataTable);
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectByPersonelIdReturnDT(int personelId, int izinTipi)
        {
            string sqlString = SelectByPersonelSQL(personelId, izinTipi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        private string SelectByIzinTarihiSQL(int personelId, int izinTipi, DateTime tarih)
        {
            string izinTipiStr = izinTipi == 0 ? "" : string.Format(" AND IzinTipi={0}", izinTipi);
            string sqlstr = string.Format(@" 
                    SELECT * FROM IzinDonem_Table  
                    WHERE PersonelId={0} {1}
                        AND (BaslangicTarihi<={2} AND BitisTarihi>{3}) ----AND BitisTarihi>={3}) izindönemnin son gününün ertesi günü yeni izin dönemi olmali SB 01.06.2020
                    ORDER BY BaslangicTarihi", personelId, izinTipiStr, tarih.ReturnTRDateFormat(), tarih.AddDays(-1).ReturnTRDateFormat());
            return sqlstr;
        }
        private string SelectOncekiYillaraAitIzinDonemleriSQL(int personelId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IzinDonem_Table  
                    WHERE PersonelId={0} AND IzinTipi=1 AND BitisTarihi < GETDATE() AND KalanIzin!=0                     
                    ORDER BY BaslangicTarihi DESC", personelId);
            return sqlstr;
        }
        private string SelectByPersonelSQL(int personelId, int izinTipi)
        {

            string izinTipiStr = izinTipi == 0 ? "" : string.Format(" IzinTipi={0}", izinTipi);
            string perStr = personelId == 0 ? "" : string.Format(" personelId={0}", personelId);
            string whereStr = string.Empty;
            if (!string.IsNullOrEmpty(izinTipiStr) && !string.IsNullOrEmpty(perStr))
            {
                whereStr = string.Format(" WHERE {0} AND {1} ", perStr, izinTipiStr);
            }
            else if (string.IsNullOrEmpty(izinTipiStr) && !string.IsNullOrEmpty(perStr))
            {
                whereStr = string.Format(" WHERE {0}", perStr);
            }
            else if (!string.IsNullOrEmpty(izinTipiStr) && string.IsNullOrEmpty(perStr))
            {
                whereStr = string.Format(" WHERE {0} ", izinTipiStr);
            }

            string sqlstr = string.Format(@" 
                    SELECT Id IzinDonemId, Id ,PersonelId, BaslangicTarihi, BitisTarihi, Adi, IzinTipi, IzinHakki, KullanilanIzin, KalanIzin, Birim ,Aciklama, 
                        Olusturan, OlusturmaTarihi, Degistiren, DegistirmeTarihi
                    FROM IzinDonem_Table  
                    {0} 
                    ORDER BY BaslangicTarihi DESC", whereStr);
            return sqlstr;
        }

        /// <summary>
        /// personel,izinTipi ve tarih bazinda IzinDonem_Table'da kayit yoksa, IzinDonemi tipinde kayit ekler,  
        /// IzinDonemi tipinde ekledigi yeni nesneyi döndürür.
        /// </summary>
        /// <param name="personel"></param>
        /// <param name="izinTipi"></param>
        /// <param name="tarih"></param>
        /// <returns></returns>
        public IzinDonem IzinDonemiOlustur(Personel personel, int izinTipi, DateTime tarih, string currentUserName)
        {
            IzinDonem izinDonemi = null;
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {
                DateTime izinDonemiBasTar = izinDonemiBasTarStr.ConvertToDatetime();//ib.IzinDonemiBasTar;
                string birim = ProjeConstants.IZIN_BIRIMI_GUN;
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    izinDonemiBasTar = new DateTime(izinDonemiBasTar.Year, 1, 1);
                    birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                }
                int year = tarih.Year;
                int month = izinDonemiBasTar.Month;
                int day = izinDonemiBasTar.Day;
                DateTime izinDonemiBasi = new DateTime(year - 1, month, day);
                IzinHareket izinHareket = new IzinHareket();
                DateTime izinDonemiSonu = izinDonemiBasi.AddYears(1).AddDays(-1);
                if (tarih > izinDonemiSonu)
                {
                    izinDonemiBasi = izinDonemiBasi.AddYears(1);
                    izinDonemiSonu = izinDonemiBasi.AddYears(1).AddDays(-1);
                }

                izinDonemi = new IzinDonem();
                //DateTime today = DateTime.Today;
                izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinTipi, tarih);
                if (izinDonemi == null)
                {
                    izinDonemi = new IzinDonem();
                    izinDonemi.PersonelId = personel.Id;
                    izinDonemi.IzinTipi = izinTipi;
                    izinDonemi.BaslangicTarihi = izinDonemiBasi;
                    izinDonemi.BitisTarihi = izinDonemiSonu;
                    izinDonemi.Birim = birim;
                    izinDonemi.Adi = izinDonemiBasi.Year + "-" + izinDonemiSonu.Year + " İzin Dönemi";
                    izinDonemi.Aciklama = " Otomatik Oluşturuldu ";
                    izinDonemi.IzinHakki = IKYSOrtak.IzinHakkiHesapla(personel, izinTipi, izinDonemiBasi, izinDonemiBasTar);
                    string sifirIzin = "0";

                    izinDonemi.KullanilanIzin = sifirIzin;
                    izinDonemi.KalanIzin = izinDonemi.IzinHakki;
                    izinDonemi.Olusturan = currentUserName;
                    izinDonemi.Id = izinDonemi.Save();
                }

            }

            return izinDonemi;
        }
        public IzinDonem IzinDonemiGuncelle(Personel personel, int izinTipi, DateTime tarih, string currentUserName)
        {
            //bir kisinin izin dönemi belirlendikten sonra
            IzinDonem izinDonemi = null;
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {
                DateTime izinDonemiBasTar = izinDonemiBasTarStr.ConvertToDatetime();//ib.IzinDonemiBasTar;
                string birim = ProjeConstants.IZIN_BIRIMI_GUN;
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    izinDonemiBasTar = new DateTime(izinDonemiBasTar.Year, 1, 1);
                    birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                }
                int year = tarih.Year;
                int month = izinDonemiBasTar.Month;
                int day = izinDonemiBasTar.Day;
                DateTime izinDonemiBasi = new DateTime(year - 1, month, day);
                DateTime izinDonemiSonu = izinDonemiBasi.AddYears(1).AddDays(-1);
                if (tarih > izinDonemiSonu)
                {
                    izinDonemiBasi = izinDonemiBasi.AddYears(1);
                    izinDonemiSonu = izinDonemiBasi.AddYears(1).AddDays(-1);
                }

                izinDonemi = new IzinDonem();
                //DateTime today = DateTime.Today;
                izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinTipi, tarih);
                if (izinDonemi != null)
                {
                    izinDonemi.BaslangicTarihi = izinDonemiBasi;
                    izinDonemi.BitisTarihi = izinDonemiSonu;
                    izinDonemi.Birim = birim;
                    izinDonemi.Adi = izinDonemiBasi.Year + "-" + izinDonemiSonu.Year + " İzin Dönemi";
                    izinDonemi.Aciklama = " İzin Dönemi Güncellendi. ";

                    izinDonemi.Degistiren = currentUserName;

                    if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) // izin tipi ÜCRETLI izinse
                    {
                        izinDonemi.IzinHakki = IKYSOrtak.UcretliIzinHakkiHesapla(personel, izinDonemiBasi, izinDonemiBasTar).ToString();//UtilityHelper.IzinHakkiHesapla(personel, izinTipi, izinDonemiBasi);
                        int kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                        int kalanIzin = izinDonemi.KalanIzin.ConvertToInt();
                        int izinHakkiInt = izinDonemi.IzinHakki.ConvertToInt();
                        kalanIzin = izinHakkiInt - kullanilanIzin;
                        izinDonemi.KalanIzin = kalanIzin.ToString();
                        izinDonemi.Update();
                    }
                    if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        TimeSpan kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                        TimeSpan kalanIzin = new TimeSpan(0, 0, 0); ;
                        TimeSpan izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpan();
                        kalanIzin = izinHakki - kullanilanIzin;
                        izinDonemi.KalanIzin = kalanIzin.ToString();
                        izinDonemi.Update();
                    }

                }
            }
            return izinDonemi;
        }

        public bool KullanilanIzinGuncelle(IzinDonem izinDonemi, string sure, bool ekle, string currentUserName)
        {
            bool isSaved = false;


            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) //yalnizca izin tipi ÜCRETLI izinse  kalan izin süresini hesapla
            {
                if (izinDonemi != null)
                {

                    int oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = 0;
                    if (ekle)
                        sonuc = oncekiToplamIzin + sure.ConvertToInt();
                    else
                        sonuc = oncekiToplamIzin - sure.ConvertToInt();

                    izinDonemi.KullanilanIzin = sonuc.ToString();

                    int izinHakki = izinDonemi.IzinHakki.ConvertToInt();
                    int kalanIzin = izinHakki - sonuc;
                    izinDonemi.KalanIzin = kalanIzin.ToString();

                    izinDonemi.Update();
                }
            }
            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                if (izinDonemi != null)
                {

                    TimeSpan oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    TimeSpan sonuc = new TimeSpan(0, 0, 0);
                    if (ekle)
                        sonuc = oncekiToplamIzin + sure.ConvertToTimeSpan();
                    else
                        sonuc = oncekiToplamIzin - sure.ConvertToTimeSpan();

                    izinDonemi.KullanilanIzin = sonuc.ToString();
                    TimeSpan kalanIzin = new TimeSpan(0, 0, 0); ;
                    TimeSpan izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpan();
                    kalanIzin = izinHakki - sonuc;
                    izinDonemi.KalanIzin = kalanIzin.ToString();
                    izinDonemi.Degistiren = currentUserName;
                    izinDonemi.Update();
                }
            }

            return isSaved;
        }
    }
}
