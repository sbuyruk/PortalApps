using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.MTS
{
    public class MtsLookupRepository
    {
        private readonly DbClass db;

        public MtsLookupRepository()
            : this(new DbClass())
        {
        }

        public MtsLookupRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
        }

        public DataTable SelectStokluAniObjeleri(string stokluMu)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id AniObjesiId, A.Adi, SUM(B.SonAdet) Toplam
                FROM AniObjesiTanim_Table A
                INNER JOIN DepoStok_Table B ON B.AniObjesiId = A.Id AND B.SonAdet > 0
                WHERE A.StokluMu = @StokluMu
                GROUP BY A.Id, A.Adi
                ORDER BY A.Adi");
            query.AddParameter("@StokluMu", stokluMu);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectStokluAniObjesiDepolari(string stokluMu, int aniObjesiId)
        {
            string filter = aniObjesiId > 0 ? " AND C.Id = @AniObjesiId" : "";
            SqlQuery query = new SqlQuery(@"
                SELECT C.Id AniObjesiId, C.Adi AniObjesiAdi,
                       A.Id DepoId, A.Adi DepoAdi, SUM(B.SonAdet) Adet
                FROM DepoTanim_Table A
                INNER JOIN DepoStok_Table B ON B.DepoId = A.Id AND B.SonAdet > 0
                INNER JOIN AniObjesiTanim_Table C ON C.Id = B.AniObjesiId
                WHERE C.StokluMu = @StokluMu" + filter + @"
                GROUP BY A.Id, A.Adi, C.Id, C.Adi
                ORDER BY A.Id");
            query.AddParameter("@StokluMu", stokluMu);
            if (aniObjesiId > 0)
                query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDepoStok(int depoId, int aniObjesiId, string stokluMu)
        {
            string depoFilter = depoId > 0 ? " AND A.DepoId = @DepoId" : "";
            string aniObjesiFilter = aniObjesiId > 0 ? " AND B.Id = @AniObjesiId" : "";
            string stokluFilter = string.IsNullOrEmpty(stokluMu) ? "" : " AND B.StokluMu = @StokluMu";
            SqlQuery query = new SqlQuery(@"
                SELECT A.*
                FROM DepoStok_Table A
                INNER JOIN AniObjesiTanim_Table B ON B.Id = A.AniObjesiId
                WHERE A.SonAdet > 0" + depoFilter + aniObjesiFilter + stokluFilter + @"
                ORDER BY A.Id");
            if (depoId > 0)
                query.AddParameter("@DepoId", depoId);
            if (aniObjesiId > 0)
                query.AddParameter("@AniObjesiId", aniObjesiId);
            if (!string.IsNullOrEmpty(stokluMu))
                query.AddParameter("@StokluMu", stokluMu);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilim(int faaliyetId, int katilimciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT *
                FROM FaaliyetKatilim_Table
                WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilimByKatilimciId(int katilimciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE KatilimciId = @KatilimciId");
            query.AddParameter("@KatilimciId", katilimciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetKatilimByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM FaaliyetKatilim_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAramaGorusmeByFaaliyetId(int faaliyetId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE FaaliyetId = @FaaliyetId");
            query.AddParameter("@FaaliyetId", faaliyetId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAramaGorusmeByArayanId(int arayanId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM AramaGorusme_Table WHERE ArayanId = @ArayanId");
            query.AddParameter("@ArayanId", arayanId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAramaGorusmeByFilter(int arayanId, string gorusmeSekli, DateTime basTar, DateTime bitTar, DateTime referansTarihi, string gorevde)
        {
            List<string> filters = new List<string>();
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id AramaId, C.Id ArayanId, A.GorusmeSekli, A.Tarih, A.Konu,
                       A.GorusmeSaglandi, A.RandevuIstendi, A.FaaliyetId,
                       C.Adi, C.Soyadi, H.Adi Kurumu, C.RandevuKisiti
                FROM AramaGorusme_Table A
                LEFT JOIN Kisi_Table C ON C.Id = A.ArayanId
                LEFT JOIN MTSKurumGorev_Table G ON G.KisiId = C.Id AND G.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table H ON H.Id = G.MTSKurumTanimId");
            query.AddParameter("@Gorevde", gorevde);

            if (arayanId > 0)
            {
                filters.Add("A.ArayanId = @ArayanId");
                query.AddParameter("@ArayanId", arayanId);
            }

            if (!string.IsNullOrEmpty(gorusmeSekli))
            {
                filters.Add("A.GorusmeSekli = @GorusmeSekli");
                query.AddParameter("@GorusmeSekli", gorusmeSekli);
            }

            if (basTar >= referansTarihi)
            {
                filters.Add("A.Tarih BETWEEN @BasTar AND @BitTar");
                query.AddParameter("@BasTar", basTar);
                query.AddParameter("@BitTar", bitTar);
            }

            if (filters.Count > 0)
                query.Sql += " WHERE " + string.Join(" AND ", filters);

            query.Sql += " ORDER BY A.Tarih DESC";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAniObjesiDagitimByFilter(int faaliyetId, int katilimciId, int aniObjesiId, string stokluMu, string verilenGetirilen, string mode)
        {
            SqlQuery query;
            if (mode == "return")
            {
                query = new SqlQuery(@"
                    SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId,
                           B.Adet, B.FaaliyetId, B.KatilimciId
                    FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId
                    ORDER BY A.Id");
            }
            else if (mode == "stok")
            {
                query = new SqlQuery(@"
                    SELECT A.Id AniObjesiId, A.Sira, A.Adi, B.Id AniObjesiDagitimId,
                           B.Adet, B.FaaliyetId, B.KatilimciId
                    FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId
                    WHERE A.StokluMu = @StokluMu
                    ORDER BY A.Id");
            }
            else if (mode == "stoksuz")
            {
                query = new SqlQuery(@"
                    SELECT B.Id, A.Id AniObjesiId, ISNULL(B.Adet, 0) Adet,
                           B.FaaliyetId, B.KatilimciId
                    FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.FaaliyetId = @FaaliyetId AND B.KatilimciId = @KatilimciId
                    WHERE A.StokluMu = @StokluMu");
            }
            else if (mode == "kisi")
            {
                query = new SqlQuery(@"
                    SELECT B.Id, A.Id AniObjesiId, ISNULL(B.Adet, 0) Adet,
                           B.FaaliyetId, B.KatilimciId, B.VerilenAlinan
                    FROM AniObjesiTanim_Table A
                    LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId = A.Id
                        AND B.KatilimciId = @KatilimciId
                    WHERE 1 > 0");
                if (!string.Equals(verilenGetirilen, ProjeConstants.ANIOBJESI_VERILENGETIRILEN))
                    query.Sql += " AND B.VerilenAlinan = @VerilenAlinan";
            }
            else if (mode == "katilimciFaaliyet")
            {
                query = new SqlQuery(@"
                    SELECT A.Id AniObjesiDagitimId, B.Id AniObjesiId, B.Adi,
                           A.Adet, A.CikisDepoId
                    FROM AniObjesiDagitim_Table A
                    INNER JOIN AniObjesiTanim_Table B ON B.Id = A.AniObjesiId
                    WHERE A.KatilimciId = @KatilimciId AND A.FaaliyetId = @FaaliyetId");
                if (!string.IsNullOrEmpty(stokluMu))
                    query.Sql += " AND B.StokluMu = @StokluMu";
            }
            else if (mode == "activity")
            {
                query = new SqlQuery("SELECT * FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId");
            }
            else
            {
                query = new SqlQuery(@"
                    SELECT * FROM AniObjesiDagitim_Table
                    WHERE AniObjesiId = @AniObjesiId
                      AND FaaliyetId = @FaaliyetId
                      AND KatilimciId = @KatilimciId");
            }

            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            if (mode == "stok" || mode == "stoksuz" || mode == "katilimciFaaliyet")
                query.AddParameter("@StokluMu", stokluMu);
            if (mode == "kisi" && !string.Equals(verilenGetirilen, ProjeConstants.ANIOBJESI_VERILENGETIRILEN))
                query.AddParameter("@VerilenAlinan", verilenGetirilen);
            if (mode == "katilimciFaaliyet")
                query.AddParameter("@StokluMu", stokluMu);
            if (mode == "single")
                query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGetirilenAniObjesi(int faaliyetId, int katilimciId, int getirilenAniObjesiId, int getirilenValue)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM AniObjesiDagitim_Table
                WHERE FaaliyetId = @FaaliyetId
                  AND KatilimciId = @KatilimciId
                  AND VerilenAlinan = @VerilenAlinan
                ORDER BY Id");
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@VerilenAlinan", getirilenValue);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectGetirilenAniObjesiText(int katilimciId, int faaliyetId, int aniObjesiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT GetirilenAniObjesi
                FROM AniObjesiDagitim_Table
                WHERE KatilimciId = @KatilimciId
                  AND FaaliyetId = @FaaliyetId
                  AND AniObjesiId = @AniObjesiId");
            query.AddParameter("@KatilimciId", katilimciId);
            query.AddParameter("@FaaliyetId", faaliyetId);
            query.AddParameter("@AniObjesiId", aniObjesiId);
            return db.SelectFromDb(query, "");
        }

        public int DeleteAniObjesiDagitim(int faaliyetId, int katilimciId, List<int> aniObjesiIds, out DataTable deletedItems)
        {
            List<string> parameters = new List<string>();
            SqlQuery selectQuery = new SqlQuery("SELECT * FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId");
            selectQuery.AddParameter("@FaaliyetId", faaliyetId);
            selectQuery.AddParameter("@KatilimciId", katilimciId);
            for (int i = 0; i < aniObjesiIds.Count; i++)
            {
                string parameter = "@AniObjesiId" + i;
                parameters.Add(parameter);
                selectQuery.Sql += (i == 0 ? " AND AniObjesiId IN (" : ", ") + parameter;
                selectQuery.AddParameter(parameter, aniObjesiIds[i]);
            }
            selectQuery.Sql += ")";
            deletedItems = db.SelectFromDb(selectQuery, "");
            if (deletedItems == null || deletedItems.Rows.Count == 0)
                return 0;

            SqlQuery deleteQuery = new SqlQuery("DELETE FROM AniObjesiDagitim_Table WHERE FaaliyetId = @FaaliyetId AND KatilimciId = @KatilimciId AND AniObjesiId IN (" + string.Join(", ", parameters) + ")");
            deleteQuery.AddParameter("@FaaliyetId", faaliyetId);
            deleteQuery.AddParameter("@KatilimciId", katilimciId);
            for (int i = 0; i < aniObjesiIds.Count; i++)
                deleteQuery.AddParameter("@AniObjesiId" + i, aniObjesiIds[i]);
            return db.DeleteFromDb(deleteQuery, "", true);
        }

        public DataTable SelectFaaliyetByAcikTarih(string acikTarih)
        {
            string filter = string.Empty;
            if (acikTarih == ProjeConstants.FAALIYET_ACIKTARIHLI)
                filter = " WHERE AcikTarih = @AcikTarih";
            else if (acikTarih != ProjeConstants.HEPSI)
                filter = " WHERE AcikTarih = @AcikTarih";

            SqlQuery query = new SqlQuery("SELECT * FROM Faaliyet_Table" + filter + " ORDER BY FaaliyetKonusu");
            if (!string.IsNullOrEmpty(filter))
                query.AddParameter("@AcikTarih", acikTarih == ProjeConstants.FAALIYET_ACIKTARIHLI);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetByDate(DateTime start, DateTime end)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT * FROM Faaliyet_Table
                WHERE BaslangicTarihi <= @BitisTarihi AND BitisTarihi >= @BaslangicTarihi
                ORDER BY BaslangicTarihi");
            query.AddParameter("@BaslangicTarihi", start);
            query.AddParameter("@BitisTarihi", end);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetParticipants(int faaliyetId, int monthBefore, string acikTarihli, DateTime basTar, DateTime bitTar, string faaliyetAmaci, string gorevde, DateTime referansTarihi)
        {
            List<string> filters = new List<string> { "B.Id IS NOT NULL" };
            SqlQuery query = new SqlQuery(@"
                SELECT C.KatilimciTipi, A.KatilimciId, A.Id KatilimId, A.TakvimDaveti,
                       B.Aciklama, B.OlusturmaTarihi, C.Adi, C.Soyadi, C.EPosta,
                       B.Id FaaliyetId, B.BaslangicTarihi, B.BaslangicSaati,
                       B.BitisTarihi, B.BitisSaati, B.FaaliyetAmaciId,
                       B.FaaliyetDurumu, B.FaaliyetKonusu, B.FaaliyetTipi,
                       B.FaaliyetYeriStr FaaliyetYeri, B.TumGun, B.AcikTarih, H.Adi Kurumu
                FROM Faaliyet_Table B
                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId = B.Id
                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
                LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table H ON H.Id = D.MTSKurumTanimId
                LEFT JOIN IletisimBilgileri_Table G ON G.PersonelId = A.KatilimciId");
            query.AddParameter("@Gorevde", gorevde);

            if (acikTarihli != ProjeConstants.HEPSI)
            {
                filters.Add("B.AcikTarih = @AcikTarih");
                query.AddParameter("@AcikTarih", acikTarihli == ProjeConstants.FAALIYET_ACIKTARIHLI);
            }
            if (faaliyetId != ProjeConstants.HEPSI_INT)
            {
                filters.Add("B.Id = @FaaliyetId");
                query.AddParameter("@FaaliyetId", faaliyetId);
            }
            if (monthBefore != 0)
            {
                filters.Add("B.BaslangicTarihi > DATEADD(month, @MonthBefore, CONVERT(date, GETDATE()))");
                query.AddParameter("@MonthBefore", monthBefore);
            }
            if (basTar >= referansTarihi)
            {
                filters.Add("B.BaslangicTarihi >= @BasTar");
                query.AddParameter("@BasTar", basTar);
            }
            if (bitTar >= referansTarihi)
            {
                filters.Add("B.BitisTarihi <= @BitTar");
                query.AddParameter("@BitTar", bitTar);
            }
            if (!string.IsNullOrEmpty(faaliyetAmaci) && faaliyetAmaci != ProjeConstants.HEPSI)
                query.Sql += " WHERE " + string.Join(" AND ", filters) + " AND B.FaaliyetAmaciId IN " + faaliyetAmaci;
            else
                query.Sql += " WHERE " + string.Join(" AND ", filters);
            query.Sql += " ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, C.Adi, C.Soyadi";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectFaaliyetByParticipant(int katilimciId, int faaliyetId, string gorevde)
        {
            List<string> filters = new List<string> { "A.FaaliyetId IS NOT NULL" };
            SqlQuery query = new SqlQuery(@"
                SELECT C.KatilimciTipi, A.KatilimciId, A.Id KatilimId,
                       C.Adi, C.Soyadi, E.Adi Kurumu, B.Id FaaliyetId,
                       B.BaslangicTarihi, B.BaslangicSaati, B.BitisTarihi,
                       B.BitisSaati, B.FaaliyetAmaciId, B.FaaliyetDurumu,
                       B.FaaliyetKonusu, B.FaaliyetTipi, B.FaaliyetYeriStr FaaliyetYeri,
                       B.TumGun, B.AcikTarih
                FROM Faaliyet_Table B
                LEFT JOIN FaaliyetKatilim_Table A ON A.FaaliyetId = B.Id
                LEFT JOIN Kisi_Table C ON C.Id = A.KatilimciId
                LEFT JOIN MTSKurumGorev_Table D ON D.KisiId = C.Id AND D.Durum = @Gorevde
                LEFT JOIN MTSKurumTanim_Table E ON E.Id = D.MTSKurumTanimId");
            query.AddParameter("@Gorevde", gorevde);
            if (katilimciId > 0)
            {
                filters.Add("A.KatilimciId = @KatilimciId");
                query.AddParameter("@KatilimciId", katilimciId);
            }
            if (faaliyetId > 0)
            {
                filters.Add("A.FaaliyetId = @FaaliyetId");
                query.AddParameter("@FaaliyetId", faaliyetId);
            }
            query.Sql += " WHERE " + string.Join(" AND ", filters);
            query.Sql += " ORDER BY B.BaslangicTarihi DESC, C.KatilimciTipi, C.Adi, C.Soyadi";
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDistinctFaaliyetYeri()
        {
            return db.SelectFromDb(new SqlQuery("SELECT DISTINCT(FaaliyetYeriStr) FaaliyetYeri FROM Faaliyet_Table"), "");
        }
    }
}
