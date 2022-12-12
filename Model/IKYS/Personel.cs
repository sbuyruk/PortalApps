
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    [Serializable]
    public class Personel : ParentClass
    {
        public Personel()
        {

        }

        public int PerId { get; set; }
        public int SicilNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public int Tahsili { get; set; }
        public string KullaniciAdi { get; set; }
        public int Asker_sivil { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);
            Personel personel = new Personel();
            personel = list.FirstOrDefault();
            return (T)Convert.ChangeType(personel, typeof(T));
        }
        public Personel Select(int id)
        {
            GenericEntity<Personel> genericEntity = new GenericEntity<Personel>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);
            Personel personel = new Personel();
            personel = list.FirstOrDefault();
            return personel;
        }
        public override int Save()
        {
            string sqlString = saveSQL();
            int id = dao.Insert(sqlString);
            this.Id = id;
            return id;
        }
        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = UpdateSQL();
                isSuccess = dao.Update2Db(sqlString);
            }
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
            string sqlString = string.Format(
                @"SELECT *
                FROM Personel_Table ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Personel_Table 
                                        (Adi,Soyadi,PerId, SicilNo, Tahsili, KullaniciAdi, Asker_sivil,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8}) ",
                                    Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(), PerId.ReturnQuotedValue(), SicilNo.ReturnQuotedValue(),
                                    Tahsili.ReturnQuotedValue(), KullaniciAdi.ReturnQuotedValue(), Asker_sivil.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Personel_Table 
                                    SET Adi= {0},Soyadi= {1},PerId= {2}, SicilNo= {3}, Tahsili= {4}, KullaniciAdi= {5}, Asker_sivil= {6},
                                        Degistiren={7},DegistirmeTarihi={8}
                                    WHERE Id= {9}",
                                    Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(), PerId.ReturnQuotedValue(), SicilNo.ReturnQuotedValue(),
                                    Tahsili.ReturnQuotedValue(), KullaniciAdi.ReturnQuotedValue(), Asker_sivil.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        public Personel SelectByUserName(string userName)
        {
            string sqlString = SelectByUserNameSQL(userName);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);
            Personel personel = list.FirstOrDefault();
            return (personel);
        }
        private string SelectByUserNameSQL(string userName)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Personel_Table  
                    WHERE KullaniciAdi={0}
                    ORDER BY Id", userName.ReturnQuotedValue());
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Personel_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Personel_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public Personel SelectCalisanPersonel(int personelId)
        {

            string sqlString = string.Format(@"
                    SELECT P.* 
                        --P.Id PersonelId,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
						--U.Adi Unvan, G.Adi Gorev, B.Adi BirimSube
					FROM Personel_Table P
                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
					Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
					Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
					Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                    WHERE CalismaDurumu=1 And P.Id={0}
					ORDER BY I.ProtokolSiraNo
                                    ",personelId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);
            _ = new Personel();
            Personel personel = list.FirstOrDefault();
            return personel;
        }
        public List<Personel> SelectCalisanPersonelByBirimId(int birimId)
        {

            string sqlString = string.Format(@"
                    SELECT P.* 
					FROM Personel_Table P
                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
					Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
					Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
					Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                    WHERE CalismaDurumu=1 And I.BirimId={0}
					ORDER BY I.ProtokolSiraNo
                                    ", birimId);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Personel> list = ToList<Personel>(dataTable);

            return list;
        }
        public DataTable SelectCalisanPersonelReturnDataTable()
        {

            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        U.Adi Unvan, G.Adi Gorev, B.Adi BirimSube, B.Id BirimId, I.IzinDonemiBasTar,I.ProtokolSiraNo
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    WHERE CalismaDurumu=1
								    ORDER BY I.ProtokolSiraNo
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
        public DataTable SelectPersonelReturnDataTable(int personelId)
        {

            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        U.Adi Unvan, G.Adi Gorev, B.Adi BirimSube, B.Id BirimId, I.IzinDonemiBasTar,I.ProtokolSiraNo,
                                        H.CepTelefonu,H.InternetEPosta
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    Left Outer Join  IletisimBilgileri_Table H on H.PersonelId=P.Id
                                    WHERE P.Id={0}
								    ORDER BY I.ProtokolSiraNo
                                    ", personelId);
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
        public DataTable SelectCalisanPersonelListesiReturnDataTable()
        {

            string sqlString = string.Format(@"
                SELECT A.Id PersonelId, B.TCKimlikNo, A.SicilNo,A.Adi,A.Soyadi, IIF (A.Asker_sivil=0,'Sivil','(E) Asker') as Asker_Sivil,A.KullaniciAdi,
					B.AnneAdi,B.BabaAdi, FORMAT(B.DogumTar,'dd.MM.yyyy') DogumTarihi,B.MedeniHali,B.EvlilikTar,B.Cinsiyet, B.KanGrubu,
	                B.DogumTar,B.DogumGunuKutlama,B.EvlilikTar, B.EvlilikKutlama, 
					IIF(FORMAT(C.BaslamaTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.BaslamaTar,'dd.MM.yyyy')) BaslamaTar,
					C.CalismaDurumu,
					IIF(FORMAT(C.AyrilmaTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.AyrilmaTar,'dd.MM.yyyy')) AyrilmaTar,
					C.AyrilmaSebebi,C.ProtokolSiraNo,C.SGKSicilNo,C.SGKBasTar,C.SGKDestekPrimi,C.VakifOncesiPrimGunSayisi,
					IIF(FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')) EmeklilikTarihi,
					IIF(FORMAT(C.IzinDonemiBasTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.IzinDonemiBasTar,'dd.MM.yyyy')) IzinDonemiBasTar,                    
					D.Adi Unvan, F.Adi Gorev, E.Adi BirimSube, 
                    G.CepTelefonu,G.Adres,G.InternetEPosta,K.IlceAdi IkametIlcesi,K.IlAdi IkametIli,
					H.TahsilDurumu,
                    I.IlAdi + ' - '+ I.IlceAdi DogumYeri,
                    J.Adi +' '+ J.Soyadi Esi, J.TcKimlikNo EsTcKimlikNo,J.Telefon EsTelefon
                FROM Personel_Table A
                INNER JOIN Kimlik_Table B on B.PersonelId=A.Id
                INNER JOIN IsBilgileri_Table C on A.Id=C.PersonelId
                Left Outer Join UnvanTanim_Table D on C.UnvanId=D.Id
                Left Outer Join BirimTanim_Table E on C.BirimId=E.Id
                Left Outer Join GorevTanim_Table F on C.GorevId=F.Id
                Left Outer Join IletisimBilgileri_Table G on G.PersonelId=A.Id
                Left Join TahsilTanim_Table H on H.Id=A.Tahsili
                Left Join Ilce_Table I on I.Id=B.DogumYeri
                Left Join Aile_Table J on J.PersonelId=A.Id AND YakinlikDerecesi=1
				Left Join Ilce_Table K on K.Id=G.Ilcesi
                WHERE CalismaDurumu=1
                ORDER BY C.ProtokolSiraNo                               ");
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
        public DataTable SelectAyrilanPersonelListesiReturnDataTable()
        {

            string sqlString = string.Format(@"
                SELECT A.Id PersonelId, B.TCKimlikNo, A.SicilNo,A.Adi,Soyadi, IIF (A.Asker_sivil=0,'Sivil','(E) Asker') as Asker_Sivil,
	                D.Adi Unvan, F.Adi Gorev, E.Adi BirimSube, A.KullaniciAdi,
                    FORMAT(B.DogumTar,'dd.MM.yyyy') DogumTarihi,B.MedeniHali,B.EvlilikTar, B.KanGrubu,G.CepTelefonu,G.Adres,G.InternetEPosta,
                    B.DogumTar,B.EvlilikTar, B.EvlilikKutlama, 
                    IIF(FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')),
                    C.ProtokolSiraNo
                FROM Personel_Table A
                INNER JOIN Kimlik_Table B on B.PersonelId=A.Id
                INNER JOIN IsBilgileri_Table C on A.Id=C.PersonelId
                Left Outer Join UnvanTanim_Table D on C.UnvanId=D.Id
                Left Outer Join BirimTanim_Table E on C.BirimId=E.Id
                Left Outer Join GorevTanim_Table F on C.GorevId=F.Id
                Left Outer Join IletisimBilgileri_Table G on G.PersonelId=A.Id
                WHERE CalismaDurumu=0
                ORDER BY C.ProtokolSiraNo
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
        public DataTable SelectAmirReturnDataTable()
        {
            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        B.Adi BirimSube, B.Id BirimId
								    FROM BirimTanim_Table B 
									INNER JOIN  Personel_Table P on P.Id=B.AmirId
                                    INNER JOIN IsBilgileri_Table I on I.PersonelId=B.AmirId
									WHERE I.CalismaDurumu=1
								    ORDER BY I.ProtokolSiraNo
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
        public List<Personel> SelectCalisanPersonel()
        {

            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        U.Adi Unvan, G.Adi Gorev, B.Adi BirimSube,I.BirimId
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    WHERE CalismaDurumu=1
								    ORDER BY I.ProtokolSiraNo
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
            List<Personel> list = ToList<Personel>(dataTable);

            return list;
        }

        public DataTable SelectCalisanPersonelReturnDT()
        {
            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        U.Adi Unvan, G.Adi Gorev,  B.Id BirimId,B.Adi Birim, I.IzinDonemiBasTar
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    WHERE CalismaDurumu=1
								    ORDER BY B.Sira,I.ProtokolSiraNo
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
        public DataTable SelectCalisanPersonelByBirimReturnDT(string birimListesiStr)
        {
            string birimStr = string.IsNullOrEmpty(birimListesiStr) ? " AND I.BirimId in ('') " : birimListesiStr.Equals("0") ? "" : string.Format(" AND I.BirimId in ({0}) ", birimListesiStr);
            string sqlString = string.Format(@"
                                    SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
								        U.Adi Unvan, U.KisaAdi UnvanKisa,G.Adi Gorev,G.KisaAdi GorevKisa,  B.Id BirimId,B.Adi Birim, I.IzinDonemiBasTar,I.BaslamaTar IseBaslamaTar,
                                        L.DahiliTelefonu, L.EvTelefonu, L.CepTelefonu, L.InternetEPosta
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    Left Outer Join  IletisimBilgileri_Table L on L.PersonelId=P.Id
                                    WHERE CalismaDurumu=1 {0}
								    ORDER BY B.Sira,I.ProtokolSiraNo
                                    ", birimStr);
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
        public List<Personel> SelectCalisanPersonelByBirimReturnList(string birimListesiStr)
        {
            string birimStr = string.IsNullOrEmpty(birimListesiStr) ? " AND I.BirimId in ('') " : birimListesiStr.Equals("0") ? "" : string.Format(" AND I.BirimId in ({0}) ", birimListesiStr);
            string sqlString = string.Format(@"
                                    SELECT P.*
								    FROM Personel_Table P
                                    INNER JOIN IsBilgileri_Table I on P.Id=I.PersonelId
								    Left Outer Join  UnvanTanim_Table U on I.UnvanId=U.Id
								    Left Outer Join  BirimTanim_Table B on I.BirimId=B.Id
								    Left Outer Join  GorevTanim_Table G on I.GorevId=G.Id
                                    Left Outer Join  IletisimBilgileri_Table L on L.PersonelId=P.Id
                                    WHERE CalismaDurumu=1 {0}
								    ORDER BY B.Sira,I.ProtokolSiraNo
                                    ", birimStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            List<Personel> list = ToList<Personel>(dataTable);

            return list;
        }
        //DogumGunuKutlama=1 olanları döndürür
        public List<Personel> SelectByDogumGunu(int gun, int ay)
        {

            string sqlString = string.Format(@"
                SELECT * FROM Personel_Table A
                    INNER JOIN Kimlik_Table B ON B.PersonelId= A.Id
                    INNER JOIN IsBilgileri_Table C ON C.PersonelId = A.Id
                WHERE DATEPART (D, B.DogumTar) = {0} AND 
                    DATEPART (M, B.DogumTar) = {1} AND 
                    C.CalismaDurumu =1 AND B.DogumGunuKutlama=1
                ORDER BY C.ProtokolSiraNo
            ", gun.ReturnQuotedValue(), ay.ReturnQuotedValue());
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            List<Personel> list = ToList<Personel>(dataTable);

            return list;
        }
        //EvlilikKutlama=1 olanları döndürür
        public List<Personel> SelectByEvlilikTar(int gun, int ay)
        {

            string sqlString = string.Format(@"
                SELECT * FROM Personel_Table A
                    INNER JOIN Kimlik_Table B ON B.PersonelId= A.Id
                    INNER JOIN IsBilgileri_Table C ON C.PersonelId = A.Id
                WHERE B.EvlilikTar> '01.01.1900' AND 
                    DATEPART (D, B.EvlilikTar) = {0} AND 
                    DATEPART (M, B.EvlilikTar) = {1} AND 
                    C.CalismaDurumu =1 AND B.EvlilikKutlama=1
                ORDER BY C.ProtokolSiraNo
            ", gun.ReturnQuotedValue(), ay.ReturnQuotedValue());
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            List<Personel> list = ToList<Personel>(dataTable);

            return list;
        }

        public DataTable SelectSecilmemisIcKatilimcilarByRandevuIdReturnDT(int randevuId)
        {
            string randevuIdStr = randevuId > 0 ? string.Format(" AND A.Id Not in (SELECT KatilimciId FROM RandevuKatilim_Table WHERE KatilimciTipi={0} AND RandevuId={1} )", ProjeConstants.RANDEVU_KATILIMCI_IC_INT, randevuId) : string.Empty;
            string sqlString = string.Format(@"
                SELECT A.Id KatilimciId, A.Adi,A.Soyadi, {0} KatilimciTipi
                FROM Personel_Table A
	                INNER JOIN IsBilgileri_Table B ON B.PersonelId=A.Id
                WHERE B.CalismaDurumu=1 
                    {1}
                ORDER BY A.Adi", ProjeConstants.RANDEVU_KATILIMCI_IC_INT, randevuIdStr);
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
        public DataTable SelectSecilmemisIcKatilimcilarByToplantiIdReturnDT(int toplantiId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id KatilimciId, A.Adi,A.Soyadi
                FROM Personel_Table A
	                INNER JOIN IsBilgileri_Table B ON B.PersonelId=A.Id
                WHERE B.CalismaDurumu=1 -- AND A.Id Not in (select KatilimciId FROM ToplantiKatilim_Table WHERE ToplantiId={0})
                ORDER BY B.ProtokolSiraNo, A.Adi, A.Soyadi", toplantiId);
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
        public List<Personel> SelectKatilimcilarByToplantiIdList(int toplantiId)
        {
            string sqlString = string.Format(@"
                SELECT A.*
                FROM Personel_Table A
	                INNER JOIN ToplantiKatilim_Table B ON B.KatilimciId=A.Id
                    LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id
                WHERE ToplantiId={0} AND B.Bilgi=0
                ORDER BY ProtokolSiraNo, Adi", toplantiId);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            List<Personel> list = ToList<Personel>(dataTable);
            return list;
        }
        public List<Personel> SelectBilgiVerilenlerByToplantiIdList(int toplantiId)
        {
            string sqlString = string.Format(@"
                SELECT A.*
                FROM Personel_Table A
	                INNER JOIN ToplantiKatilim_Table B ON B.KatilimciId=A.Id
                    LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id
                WHERE ToplantiId={0} AND B.Bilgi=1
                ORDER BY ProtokolSiraNo, Adi", toplantiId);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            List<Personel> list = ToList<Personel>(dataTable);
            return list;
        }
    }
}
