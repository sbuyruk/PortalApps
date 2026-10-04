using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO.Repositories.IKYS
{
    public class PersonelRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public PersonelRepository() : this(new DbClass())
        {
        }

        public PersonelRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Personel_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Personel_Table"), "");
        }

        public DataTable SelectByUserName(string userName)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Personel_Table WHERE KullaniciAdi=@KullaniciAdi ORDER BY Id");
            query.AddParameter("@KullaniciAdi", userName);
            return db.SelectFromDb(query, "");
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "Personel_Table")); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "Personel_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Personel_Table", id), ""); }

        public DataTable SelectActiveEmployees(int personelTipi)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId, P.SicilNo, P.Tahsili, P.KullaniciAdi, P.Asker_sivil,
                       U.Adi Unvan, G.Adi Gorev, B.Adi BirimSube,I.BirimId
                FROM Personel_Table P
                INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId
                LEFT OUTER JOIN UnvanTanim_Table U ON I.UnvanId=U.Id
                LEFT OUTER JOIN BirimTanim_Table B ON I.BirimId=B.Id
                LEFT OUTER JOIN GorevTanim_Table G ON I.GorevId=G.Id
                WHERE CalismaDurumu=1 AND Tipi=@Tipi
                ORDER BY I.ProtokolSiraNo");
            query.AddParameter("@Tipi", personelTipi);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectActiveEmployee(int personelId)
        {
            SqlQuery query = new SqlQuery(@"SELECT P.* FROM Personel_Table P
                INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId
                LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id
                WHERE I.CalismaDurumu=1 AND P.Id=@PersonelId ORDER BY I.ProtokolSiraNo");
            query.AddParameter("@PersonelId", personelId); return db.SelectFromDb(query, "");
        }

        public DataTable SelectActiveEmployeesByUnit(int birimId, int personelTipi, bool includeAllTypes)
        {
            string typeFilter = includeAllTypes ? "" : " AND P.Tipi=@Tipi";
            SqlQuery query = new SqlQuery(@"SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,
                U.Adi Unvan,G.Adi Gorev,B.Adi BirimSube,B.Id BirimId,I.IzinDonemiBasTar,I.ProtokolSiraNo
                FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id
                LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id
                WHERE I.CalismaDurumu=1 AND I.BirimId=@BirimId" + typeFilter + " ORDER BY I.ProtokolSiraNo");
            query.AddParameter("@BirimId", birimId); if (!includeAllTypes) query.AddParameter("@Tipi", personelTipi); return db.SelectFromDb(query, "");
        }

        public DataTable SelectActiveEmployeesByRegion(int bolgeId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Personel_Table A INNER JOIN IsBilgileri_Table B ON B.PersonelId=A.Id INNER JOIN BirimTanim_Table C ON C.Id=B.BirimId WHERE B.CalismaDurumu=1 AND C.BolgeId=@BolgeId ORDER BY B.ProtokolSiraNo"); query.AddParameter("@BolgeId", bolgeId); return db.SelectFromDb(query, "");
        }

        public DataTable SelectActiveEmployeesReturnDataTable(int personelTipi, bool includeAllTypes)
        {
            string typeFilter = includeAllTypes ? "" : " AND P.Tipi=@Tipi";
            SqlQuery query = new SqlQuery(@"SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,
                U.Adi Unvan,G.Adi Gorev,B.Adi BirimSube,B.Id BirimId,I.IzinDonemiBasTar,I.ProtokolSiraNo
                FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id
                LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id
                WHERE I.CalismaDurumu=1" + typeFilter + " ORDER BY I.ProtokolSiraNo"); if (!includeAllTypes) query.AddParameter("@Tipi", personelTipi); return db.SelectFromDb(query, "");
        }

        public DataTable SelectPersonelReturnDataTable(int personelId)
        {
            SqlQuery query = new SqlQuery(@"SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,
                U.Adi Unvan,G.Adi Gorev,B.Adi BirimSube,B.Id BirimId,I.IzinDonemiBasTar,I.ProtokolSiraNo,H.CepTelefonu,H.InternetEPosta
                FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id
                LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id LEFT JOIN IletisimBilgileri_Table H ON H.PersonelId=P.Id
                WHERE P.Id=@PersonelId ORDER BY I.ProtokolSiraNo"); query.AddParameter("@PersonelId", personelId); return db.SelectFromDb(query, "");
        }

        public DataTable SelectEmployeeListReturnDataTable(int personelTipi, bool includeAllTypes)
        {
            string typeFilter = includeAllTypes ? "" : " AND A.Tipi=@Tipi";
            SqlQuery query = new SqlQuery(@"SELECT A.Id PersonelId,B.TCKimlikNo,A.SicilNo,A.Adi,A.Soyadi,IIF(A.Asker_sivil=0,'Sivil','(E) Asker') Asker_Sivil,A.KullaniciAdi,A.Tipi PersonelTipi,
                B.AnneAdi,B.BabaAdi,FORMAT(B.DogumTar,'dd.MM.yyyy') DogumTarihi,B.MedeniHali,B.EvlilikTar,B.Cinsiyet,B.KanGrubu,B.DogumTar,B.DogumGunuKutlama,B.EvlilikTar,B.EvlilikKutlama,
                IIF(FORMAT(C.BaslamaTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.BaslamaTar,'dd.MM.yyyy')) BaslamaTar,C.CalismaDurumu,
                IIF(FORMAT(C.AyrilmaTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.AyrilmaTar,'dd.MM.yyyy')) AyrilmaTar,C.AyrilmaSebebi,C.ProtokolSiraNo,C.SGKSicilNo,C.SGKBasTar,C.VakifOncesiPrimGunSayisi,
                IIF(FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.EmeklilikTarihi,'dd.MM.yyyy')) EmeklilikTarihi,IIF(FORMAT(C.IzinDonemiBasTar,'dd.MM.yyyy')='01.01.1900','',FORMAT(C.IzinDonemiBasTar,'dd.MM.yyyy')) IzinDonemiBasTar,
                D.Adi Unvan,F.Adi Gorev,E.Adi BirimSube,G.CepTelefonu,G.Adres,G.InternetEPosta,K.IlceAdi IkametIlcesi,K.IlAdi IkametIli,H.TahsilDurumu,I.IlAdi+' - '+I.IlceAdi DogumYeri,J.Adi+' '+J.Soyadi Esi,J.TcKimlikNo EsTcKimlikNo,J.Telefon EsTelefon,Plaka
                FROM Personel_Table A INNER JOIN Kimlik_Table B ON B.PersonelId=A.Id INNER JOIN IsBilgileri_Table C ON A.Id=C.PersonelId LEFT JOIN UnvanTanim_Table D ON C.UnvanId=D.Id LEFT JOIN BirimTanim_Table E ON C.BirimId=E.Id LEFT JOIN GorevTanim_Table F ON C.GorevId=F.Id LEFT JOIN IletisimBilgileri_Table G ON G.PersonelId=A.Id LEFT JOIN TahsilTanim_Table H ON H.Id=A.Tahsili LEFT JOIN Ilce_Table I ON I.Id=B.DogumYeri LEFT JOIN Aile_Table J ON J.PersonelId=A.Id AND YakinlikDerecesi=1 LEFT JOIN Ilce_Table K ON K.Id=G.Ilcesi WHERE C.CalismaDurumu=1" + typeFilter + " ORDER BY C.ProtokolSiraNo"); if (!includeAllTypes) query.AddParameter("@Tipi", personelTipi); return db.SelectFromDb(query, "");
        }

        public DataTable SelectFormerEmployeeListReturnDataTable()
        {
            return db.SelectFromDb(new SqlQuery(@"SELECT A.Id PersonelId,B.TCKimlikNo,A.SicilNo,A.Adi,A.Soyadi,IIF(A.Asker_sivil=0,'Sivil','(E) Asker') Asker_Sivil,D.Adi Unvan,F.Adi Gorev,E.Adi BirimSube,A.KullaniciAdi,FORMAT(B.DogumTar,'dd.MM.yyyy') DogumTarihi,B.MedeniHali,B.EvlilikTar,B.KanGrubu,G.CepTelefonu,G.Adres,G.InternetEPosta,B.DogumTar,B.EvlilikTar,B.EvlilikKutlama,C.ProtokolSiraNo,C.BaslamaTar,C.AyrilmaTar,C.AyrilmaSebebi FROM Personel_Table A INNER JOIN Kimlik_Table B ON B.PersonelId=A.Id INNER JOIN IsBilgileri_Table C ON A.Id=C.PersonelId LEFT JOIN UnvanTanim_Table D ON C.UnvanId=D.Id LEFT JOIN BirimTanim_Table E ON C.BirimId=E.Id LEFT JOIN EskiPersonelGorevTanim_Table F ON C.GorevId=F.Id LEFT JOIN IletisimBilgileri_Table G ON G.PersonelId=A.Id WHERE C.CalismaDurumu=0 ORDER BY C.ProtokolSiraNo"), "");
        }

        public DataTable SelectManagers() { return db.SelectFromDb(new SqlQuery("SELECT P.Id PersonelId,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,B.Adi BirimSube,B.Id BirimId FROM BirimTanim_Table B INNER JOIN Personel_Table P ON P.Id=B.AmirId INNER JOIN IsBilgileri_Table I ON I.PersonelId=B.AmirId WHERE I.CalismaDurumu=1 ORDER BY I.ProtokolSiraNo"), ""); }
        public DataTable SelectActiveEmployeesReturnDT() { return db.SelectFromDb(new SqlQuery("SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,U.Adi Unvan,G.Adi Gorev,B.Id BirimId,B.Adi Birim,I.IzinDonemiBasTar FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON I.PersonelId=P.Id LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id WHERE I.CalismaDurumu=1 ORDER BY B.Sira,I.ProtokolSiraNo"), ""); }

        public DataTable SelectActiveEmployeesByUnitReturnDataTable(IList<int> birimIds, bool allUnits)
        {
            SqlQuery query = new SqlQuery(@"SELECT P.Id PersonelId,P.Id Id,P.Adi,Soyadi,P.PerId,P.SicilNo,P.Tahsili,P.KullaniciAdi,P.Asker_sivil,
                U.Adi Unvan,U.KisaAdi UnvanKisa,G.Adi Gorev,G.KisaAdi GorevKisa,B.Id BirimId,B.Adi Birim,I.IzinDonemiBasTar,I.BaslamaTar IseBaslamaTar,
                L.DahiliTelefonu,L.EvTelefonu,L.CepTelefonu,L.InternetEPosta,L.Plaka
                FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON P.Id=I.PersonelId LEFT JOIN UnvanTanim_Table U ON I.UnvanId=U.Id
                LEFT JOIN BirimTanim_Table B ON I.BirimId=B.Id LEFT JOIN GorevTanim_Table G ON I.GorevId=G.Id LEFT JOIN IletisimBilgileri_Table L ON L.PersonelId=P.Id
                WHERE I.CalismaDurumu=1");
            query.Sql += allUnits ? "" : (birimIds.Count == 0 ? " AND I.BirimId IN ('')" : " AND I.BirimId IN (" + AddIds(query, birimIds, "@BirimId") + ")"); query.Sql += " ORDER BY B.Sira,I.ProtokolSiraNo"; return db.SelectFromDb(query, "");
        }

        public DataTable SelectActiveEmployeesByUnitReturnList(IList<int> birimIds, bool allUnits)
        {
            SqlQuery query = new SqlQuery(@"SELECT P.* FROM Personel_Table P INNER JOIN IsBilgileri_Table I ON I.PersonelId=P.Id
                LEFT JOIN BirimTanim_Table B ON B.Id=I.BirimId LEFT JOIN GorevTanim_Table G ON G.Id=I.GorevId LEFT JOIN IletisimBilgileri_Table L ON L.PersonelId=P.Id
                WHERE I.CalismaDurumu=1"); query.Sql += allUnits ? "" : (birimIds.Count == 0 ? " AND I.BirimId IN ('')" : " AND I.BirimId IN (" + AddIds(query, birimIds, "@BirimId") + ")"); query.Sql += " ORDER BY B.Sira,I.ProtokolSiraNo"; return db.SelectFromDb(query, "");
        }

        public DataTable SelectByDogumGunu(int gun, int ay) { SqlQuery q = new SqlQuery("SELECT * FROM Personel_Table A INNER JOIN Kimlik_Table B ON B.PersonelId=A.Id INNER JOIN IsBilgileri_Table C ON C.PersonelId=A.Id WHERE DATEPART(D,B.DogumTar)=@Gun AND DATEPART(M,B.DogumTar)=@Ay AND C.CalismaDurumu=1 AND B.DogumGunuKutlama=1 ORDER BY C.ProtokolSiraNo"); q.AddParameter("@Gun", gun); q.AddParameter("@Ay", ay); return db.SelectFromDb(q, ""); }
        public DataTable SelectByEvlilikTarihi(int gun, int ay) { SqlQuery q = new SqlQuery("SELECT * FROM Personel_Table A INNER JOIN Kimlik_Table B ON B.PersonelId=A.Id INNER JOIN IsBilgileri_Table C ON C.PersonelId=A.Id WHERE B.EvlilikTar>'01.01.1900' AND DATEPART(D,B.EvlilikTar)=@Gun AND DATEPART(M,B.EvlilikTar)=@Ay AND C.CalismaDurumu=1 AND B.EvlilikKutlama=1 ORDER BY C.ProtokolSiraNo"); q.AddParameter("@Gun", gun); q.AddParameter("@Ay", ay); return db.SelectFromDb(q, ""); }
        public DataTable SelectUnselectedMeetingParticipants(int toplantiId) { return db.SelectFromDb(new SqlQuery("SELECT A.Id KatilimciId,A.Adi,A.Soyadi FROM Personel_Table A INNER JOIN IsBilgileri_Table B ON B.PersonelId=A.Id WHERE B.CalismaDurumu=1 ORDER BY B.ProtokolSiraNo,A.Adi,A.Soyadi"), ""); }
        public DataTable SelectMeetingParticipants(int toplantiId, bool bilgiVerildi) { SqlQuery q = new SqlQuery("SELECT A.* FROM Personel_Table A INNER JOIN ToplantiKatilim_Table B ON B.KatilimciId=A.Id LEFT JOIN IsBilgileri_Table C ON C.PersonelId=A.Id WHERE B.ToplantiId=@ToplantiId AND B.Bilgi=@Bilgi ORDER BY C.ProtokolSiraNo,A.Adi"); q.AddParameter("@ToplantiId", toplantiId); q.AddParameter("@Bilgi", bilgiVerildi); return db.SelectFromDb(q, ""); }
        private static string AddIds(SqlQuery query, IList<int> ids, string prefix) { List<string> names = new List<string>(); for (int i = 0; i < ids.Count; i++) { string name = prefix + i; names.Add(name); query.Parameters.Add(new SqlParameter(name, SqlDbType.Int) { Value = ids[i] }); } return string.Join(",", names); }
    }
}
