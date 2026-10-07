using DAO.Ortak;
using System;
using System.Data;
using System.Linq;

namespace DAO.Repositories.TBYS
{
    public class TasinmazRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public TasinmazRepository() : this(new DbClass()) { }

        public TasinmazRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAddressByBolumId(int tasinmazId, int bolumId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Adres, A.Ili, A.Ilcesi, B.BolumNo
                FROM Tasinmaz_Table A
                LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND B.Id=@BolumId
                WHERE A.Id=@TasinmazId");
            query.AddParameter("@BolumId", bolumId);
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBolge(int bolgeId, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND E.Id=@BolgeId ";
            SqlQuery query = new SqlQuery(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano, T.Id, T.Id TasinmazId, D.IlceAdi +'/'+C.IlAdi IliIlcesi,E.KisaAdi Bolge,
                    T.*,
                    B.Adi+' '+B.Soyadi Bagisci, B.Id BagisciId, B.Sag_vefat
                FROM Tasinmaz_Table T
                    LEFT JOIN Bagis_Table A ON A.TasinmazId=T.Id
                    LEFT JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                    LEFT JOIN IL_Table C ON C.Id=T.IlId
                    LEFT JOIN ILCE_Table D ON D.Id=T.IlceId
                    LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                WHERE T.EnvanterdeMi=1" + regionFilter);
            if (!string.IsNullOrEmpty(regionFilter)) query.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectWithoutDonor(string[] exitReasons)
        {
            string[] parameters = exitReasons.Select((reason, index) => "@Sebep" + index).ToArray();
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id TasinmazId, A.Id TasinmazId, A.MulkiyetSekli,A.KullanimSekli,A.Ili,A.Ilcesi,A.Adres,A.EnvanterdeMi
                FROM Tasinmaz_Table A
                    LEFT JOIN Bagis_Table B ON B.TasinmazId=A.Id
                WHERE (B.BagisciId IS NULL OR B.BagisciId=0)
                    AND (A.EnvanterdeMi=1 OR (A.EnvanterdeMi=0 AND A.EnvanterdenCikmaSebebi IN (" + string.Join(",", parameters) + ")))" );
            for (int i = 0; i < exitReasons.Length; i++) query.AddParameter(parameters[i], exitReasons[i]);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectSectionNumbers(int inventoryState, string rentalEligibility)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id, A.Id TasinmazId,A.Cinsi, A.Ili, A.Ilcesi, A.Ili+'/'+A.Ilcesi IliIlcesi,
                    A.SigortaDurumu, A.Adres,A.Adres+' '+A.Ili+'/'+A.Ilcesi AdresIliIlcesi,
                    A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, A.SorumluBolge, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo,
                    A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi,
                    A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto,
                    A.Nitelik,A.BulunduguKat, A.Aciklama,A.EnvantereGirisTarihi, B.BolumNo,B.Id BolumId
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId = A.Id
                    LEFT JOIN KiraSozlesme_Table D ON D.Aktif=1 AND D.Id IN (SELECT SozlesmeId FROM SozlesmeTasinmaz_Table WHERE TasinmazId=A.Id AND (BolumId IS NULL OR BolumId=0 OR BolumId=B.Id))
                WHERE A.EnvanterdeMi=@Envanterde AND A.KirayaUygunluk=@KirayaUygunluk AND D.Id IS NULL
                ORDER BY A.Id");
            query.AddParameter("@Envanterde", inventoryState);
            query.AddParameter("@KirayaUygunluk", rentalEligibility);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectCountBySigorta(int bolgeId, string countColumn, string propertyColumn, string propertyValue, string sigortaValue, bool includeOutOfInventory, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND C.BolgeId=@BolgeId";
            string inventoryFilter = includeOutOfInventory ? " AND (B.EnvanterdeMi=1 OR B.EnvanterdeMi=2)" : " AND B.EnvanterdeMi=1";
            SqlQuery query = new SqlQuery("SELECT COUNT(" + (propertyColumn == "KullanimSekli" && includeOutOfInventory ? "A." : "B.") + countColumn + ") Adet FROM Sigorta_Table A INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId INNER JOIN Il_Table C ON C.Id=B.IlId WHERE 1=1" + inventoryFilter + regionFilter + " AND " + (propertyColumn == "KullanimSekli" && includeOutOfInventory ? "A." : "B.") + propertyColumn + "=@PropertyValue AND B.SigortaDurumu=@Sigorta");
            if (!string.IsNullOrEmpty(regionFilter)) query.AddParameter("@BolgeId", bolgeId);
            query.AddParameter("@PropertyValue", propertyValue);
            query.AddParameter("@Sigorta", sigortaValue);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectSectionsByTasinmazId(int tasinmazId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.KatMulkiyeti, A.KullanimSekli,A.Cinsi,A.MulkiyetSekli,D.Adi,D.Soyadi, A.Adres,A.Ilcesi,A.Ili,
                    A.AdaNo,A.ParselNo,A.Yuzolcumu,A.ArsaPayi,A.EnvanterdeMi,A.KullanimSekli,
                    B.Id BolumId,B.BolumNo, B.Aciklama,B.Nitelik, B.Metrekare, B.KullanimAmaci
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id
                    LEFT JOIN Bagis_Table C ON C.TasinmazId=A.Id
                    LEFT JOIN TasinmazBagisci_Table D ON D.Id=C.BagisciId
                WHERE A.Id=@TasinmazId AND A.EnvanterdeMi=1 AND A.KatMulkiyeti=0");
            query.AddParameter("@TasinmazId", tasinmazId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectTotalByOwnership()
        {
            return db.SelectFromDb(new SqlQuery("SELECT MulkiyetSekli, Count(Id) Adet FROM Tasinmaz_Table WHERE EnvanterdeMi=1 GROUP BY MulkiyetSekli"), "");
        }
        public DataTable SelectRentalEligibleTotals()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT EnvanterdeMi,COUNT(DISTINCT(A.Id)) AnaTasinmaz, Count(B.Id) AltBolum,COUNT(A.Id) ToplamKiralanabilir
                FROM Tasinmaz_Table A
                    LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id AND AltBolum=1
                WHERE EnvanterdeMi in (1,2) AND KirayaUygunluk='Kiraya Uygun'
                GROUP BY EnvanterdeMi"), "");
        }
        public DataTable SelectOutOfInventoryList(bool includeRegionNames)
        {
            string regionColumns = includeRegionNames ? "C.IlAdi Ili, D.IlceAdi Ilcesi, C.IlAdi+'/'+D.IlceAdi IliIlcesi, E.KisaAdi SorumluBolge" : "A.Ili Ili, A.Ilcesi Ilcesi, A.Ili+'/'+A.Ilcesi IliIlcesi, A.SorumluBolge";
            string addressColumns = includeRegionNames ? "A.Adres+' '+C.IlAdi+'/'+D.IlceAdi AdresIliIlcesi" : "A.Adres+' '+A.Ili+'/'+A.Ilcesi AdresIliIlcesi";
            string joins = includeRegionNames ? "LEFT JOIN IL_Table C ON C.Id=A.IlId LEFT JOIN ILCE_Table D ON D.Id=A.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId" : string.Empty;
            SqlQuery query = new SqlQuery("SELECT ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano, A.Id, A.Id TasinmazId,A.Cinsi," + regionColumns + ", A.SigortaDurumu, A.Adres," + addressColumns + ", A.MulkiyetSekli, A.KiraDurumu, A.KatMulkiyeti, A.EdinmeSekli,A.BagisYili, A.EmlakSicilNo, A.EmlakBeyanDegeri, A.TahminiRayicDegeri, A.TapuTarihi, A.AdaNo, A.ParselNo, A.PaftaNo, A.Yuzolcumu, A.ArsaPayi, A.VakifHissesi, A.YevmiyeNo,A.CiltNo, A.SahifeNo, A.KullanimSekli, A.TasinmazFoto, A.TasinmazFoto1, A.TasinmazFoto2, A.TapuFoto, A.KrokiFoto, A.TahkikatFoto, A.Nitelik,A.BulunduguKat,A.Aciklama,A.EnvantereGirisTarihi, B.BolumNo,B.Id BolumId FROM Tasinmaz_Table A LEFT JOIN BagimsizBolum_Table B ON B.TasinmazId=A.Id " + joins + " WHERE A.EnvanterdeMi=2");
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllInventoryReport()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano,E.KisaAdi AS SorumluBolge,B.Adi+' '+B.Soyadi AS Bagisci,B.Sag_vefat,
                    T.Adres,D.IlceAdi AS Ilcesi,C.IlAdi AS Ili,T.Mahalle,T.Koy,T.Cadde,T.Sokak,T.Mevki,T.Giris,T.Blok,
                    T.AdaNo,T.ParselNo,T.PaftaNo,T.Yuzolcumu,T.ArsaPayi,T.VakifHissesi,T.YevmiyeNo,T.CiltNo,T.SahifeNo,
                    T.KullanimSekli,T.AnaTasinmazNitelik,T.BBNitelik,T.TapuTasinmazNo,T.Cinsi,T.MulkiyetSekli,T.KirayaUygunluk,T.KiraDurumu,
                    T.EdinmeSekli,T.BagisYili,T.Nitelik,T.BulunduguKat,T.BagimsizBolumNo,T.TamHisse,T.HisseMiktariPay,T.HisseMiktariPayda,
                    T.ToplamKatSayisi,T.InsaYili,T.Metrekare,T.ToplamMetrekare,T.ProjeM2,T.ZeminTipi,T.ZeminHisse,T.BBBrutAlan,T.BBNetAlan,T.EnvantereGirisTarihi,
                    IIF(T.KatMulkiyeti=1,'Kat Mülkiyeti Var','Kat Mülkiyeti Yok') KatMulkiyeti,
                    IIF(T.KatIrtifaki=1,'Kat Irtifaki Var','Kat Irtifaki Yok') KatIrtifaki,
                    IIF(T.AltBolum=1,'Kat Alt Bölüm Var','Kat Alt Bölüm Yok') AltBolum,
                    T.TapuTarihi,T.TapuIslemTarihi,T.EmlakSicilNo,T.SigortaDurumu,T.Aciklama,T.EmlakBeyanDegeri,T.TahminiRayicDegeri,
                    T.YaklasikPiyasaDegeri,T.MuhasebeyeKayitliDeger,T.MalikSayisi,T.BagimsizBolumSayisi,T.YapiTarzi,T.InsaatinSinifi,T.ArazininCinsi,
                    T.Id TasinmazId,G.Id SozlesmeId,H.Adi,G.IlkSozlesmeTar,G.SozBasTar BaslamaTarihi,H.KiralamaAmaci,H.Adres KiraciAdresi,H.Ili,H.Ilcesi,
                    G.OdemeSekli,G.KiraBedeli,G.ArtisAyi,YEAR(G.SozBasTar)-YEAR(G.IlkSozlesmeTar) KiraSuresi,T.SatisPlani,T.SatisPlaniAciklama
                FROM Tasinmaz_Table T
                    LEFT OUTER JOIN Bagis_Table A ON A.TasinmazId=T.Id
                    LEFT OUTER JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                    LEFT JOIN IL_Table C ON C.Id=T.IlId LEFT JOIN ILCE_Table D ON D.Id=T.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId
                    OUTER APPLY (SELECT TOP 1 * FROM SozlesmeTasinmaz_Table F WHERE F.TasinmazId=T.Id ORDER BY F.SozlesmeId DESC) F
                    LEFT JOIN KiraSozlesme_Table G ON G.Id=F.SozlesmeId LEFT JOIN Kiraci_Table H ON H.Id=G.KiraciId
                WHERE T.EnvanterdeMi=1"), "");
        }
        public DataTable SelectAllOutOfInventoryReport()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT ROW_NUMBER() OVER(ORDER BY T.Id) AS Sirano,T.Id,T.Id TasinmazId,B.Adi+' '+B.Soyadi Bagisci,B.Id BagisciId,T.BagisYili,
                    T.Cinsi,T.Ili,T.Ilcesi,T.Ili+'/'+T.Ilcesi IliIlcesi,T.SigortaDurumu,T.Adres,T.Adres+' '+T.Ili+'/'+T.Ilcesi AdresIlIlce,
                    T.MulkiyetSekli,T.KiraDurumu,IIF(T.KatMulkiyeti=1,'Kat Mülkiyeti Var','Kat Mülkiyeti Yok') KatMulkiyeti,
                    T.SorumluBolge,T.EdinmeSekli,T.BagisYili,T.EmlakSicilNo,T.EmlakBeyanDegeri,T.TahminiRayicDegeri,T.TapuTarihi,
                    T.AdaNo,T.ParselNo,T.PaftaNo,T.Yuzolcumu,T.ArsaPayi,T.VakifHissesi,T.YevmiyeNo,T.CiltNo,T.SahifeNo,T.KullanimSekli,
                    T.TasinmazFoto,T.TasinmazFoto1,T.TasinmazFoto2,T.TapuFoto,T.KrokiFoto,T.TahkikatFoto,T.Nitelik,T.BulunduguKat,T.Aciklama,
                    T.EnvantereGirisTarihi,YEAR(T.EnvanterdenCikmaTarihi) EnvanterdenCikmaYili,T.EnvanterdenCikmaTarihi,T.EnvanterdenCikmaSebebi,T.EnvanterdenCikmaBedeli
                FROM Tasinmaz_Table T LEFT JOIN Bagis_Table A ON A.TasinmazId=T.Id LEFT JOIN TasinmazBagisci_Table B ON B.Id=A.BagisciId
                WHERE T.EnvanterdeMi=0"), "");
        }

        public DataTable SelectInventoryById(int id)
        {
            SqlQuery query = new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectInventory()
        {
            return db.SelectFromDb(new SqlQuery(
                "SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), "");
        }
        public DataTable SelectInventoryByIlAdi(string ilAdi) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Ili=@IlAdi"); q.AddParameter("@IlAdi", ilAdi); return db.SelectFromDb(q, ""); }
        public DataTable SelectOutOfInventoryById(int id) { SqlQuery q = new SqlQuery("SELECT *,Convert(nvarchar,replace (EnvanterdenCikmaBedeli,'.',',')) as EnvanterdenCikmaBedeli FROM Tasinmaz_Table WHERE EnvanterdeMi=0 AND Id=@Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectInventoryValueTotal(string column, int bolgeId, int allRegion, int headquarters)
        {
            string filter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND E.Id=@BolgeId";
            SqlQuery q = new SqlQuery("SELECT SUM(T." + column + ") Toplam FROM Tasinmaz_Table T LEFT JOIN Il_Table C ON C.Id=T.IlId LEFT JOIN Ilce_Table D ON D.Id=T.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId WHERE T.EnvanterdeMi=1" + filter);
            if (!string.IsNullOrEmpty(filter)) q.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectFilteredValueTotal(string column, string filterColumn, string filterValue)
        {
            SqlQuery q = new SqlQuery("SELECT SUM(" + column + ") Toplam FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND " + filterColumn + "=@FilterValue");
            q.AddParameter("@FilterValue", filterValue);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByIl(string countColumn, string ilAdi, string mulkiyetSekli, string value, string valueColumn)
        {
            SqlQuery q = new SqlQuery("SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Ili=@IlAdi AND MulkiyetSekli=@MulkiyetSekli AND " + valueColumn + "=@Value");
            q.AddParameter("@IlAdi", ilAdi);
            q.AddParameter("@MulkiyetSekli", mulkiyetSekli);
            q.AddParameter("@Value", value);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByBolge(string countColumn, int bolgeId, string value, string valueColumn, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND D.Id=@BolgeId";
            SqlQuery q = new SqlQuery("SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table A LEFT JOIN Il_Table B ON B.Id=A.IlId LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId WHERE A.EnvanterdeMi=1" + regionFilter + " AND " + valueColumn + "=@Value");
            if (!string.IsNullOrEmpty(regionFilter)) q.AddParameter("@BolgeId", bolgeId);
            q.AddParameter("@Value", value);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectCountByBolgeFilters(string countColumn, string primaryColumn, string primaryValue, string kiraDurumu, string mulkiyetSekli, string kirayaUygunluk, int bolgeId, int allRegion, int headquarters)
        {
            string regionFilter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " AND D.Id=@BolgeId";
            string optionalFilters = string.Empty;
            SqlQuery q = new SqlQuery();
            if (!string.IsNullOrEmpty(kiraDurumu)) { optionalFilters += " AND KiraDurumu=@KiraDurumu"; q.AddParameter("@KiraDurumu", kiraDurumu); }
            if (!string.IsNullOrEmpty(mulkiyetSekli)) { optionalFilters += " AND MulkiyetSekli=@MulkiyetSekli"; q.AddParameter("@MulkiyetSekli", mulkiyetSekli); }
            if (!string.IsNullOrEmpty(kirayaUygunluk)) { optionalFilters += " AND KirayaUygunluk=@KirayaUygunluk"; q.AddParameter("@KirayaUygunluk", kirayaUygunluk); }
            if (!string.IsNullOrEmpty(regionFilter)) q.AddParameter("@BolgeId", bolgeId);
            string primaryFilter = string.IsNullOrEmpty(primaryColumn) ? string.Empty : " AND " + primaryColumn + "=@PrimaryValue";
            if (!string.IsNullOrEmpty(primaryFilter)) q.AddParameter("@PrimaryValue", primaryValue);
            q.Sql = "SELECT COUNT(" + countColumn + ") Adet FROM Tasinmaz_Table A LEFT JOIN Il_Table B ON B.Id=A.IlId LEFT JOIN Bolge_Table D ON D.Id=B.BolgeId WHERE A.EnvanterdeMi=1" + regionFilter + primaryFilter + optionalFilters;
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectNext(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id>@Id ORDER BY Id"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectPrev(int id) { SqlQuery q = new SqlQuery("SELECT * FROM Tasinmaz_Table WHERE EnvanterdeMi=1 AND Id<@Id ORDER BY Id DESC"); q.AddParameter("@Id", id); return db.SelectFromDb(q, ""); }
        public DataTable SelectExtreme(bool max) { return db.SelectFromDb(new SqlQuery("SELECT " + (max ? "MAX" : "MIN") + "(Id) Id FROM Tasinmaz_Table WHERE EnvanterdeMi=1"), ""); }

        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Tasinmaz_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Tasinmaz_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Tasinmaz_Table", id), ""); }
    }
}
