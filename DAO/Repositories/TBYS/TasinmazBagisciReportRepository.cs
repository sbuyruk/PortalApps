using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class TasinmazBagisciReportRepository
    {
        private readonly DbClass db;
        public TasinmazBagisciReportRepository() : this(new DbClass()) { }
        public TasinmazBagisciReportRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; }
        public DataTable SelectAllCountBagisAdedi(bool excludeDeceased, bool excludeHidden, string sagValue)
        {
            string deceased = excludeDeceased ? " WHERE SAG_VEFAT=@SagVefat " : string.Empty;
            string hidden = excludeHidden ? (excludeDeceased ? " AND " : " WHERE ") + " (Gizli IS NULL OR Gizli=0) " : string.Empty;
            SqlQuery q = new SqlQuery(@"SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS Sirano,A.Id TasinmazBagisciId,Count(C.Id) ToplamBagisAdedi,SUM(D.TahminiRayicDegeri) ToplamTahminiRayic,A.Id TasinmazBagisciId,E.Bolge,A.Adi+' '+A.Soyadi AdiSoyadi,A.TCKimlikNo,A.DogumYeri,A.DogumTarihi,A.Meslegi,A.SosyalGuvence,A.Ilcesi,A.Ili,A.Adres,A.Telefon1,A.Telefon2,A.Foto,A.Sag_vefat,A.Gizli FROM TasinmazBagisci_Table A LEFT JOIN Bagis_Table C ON C.BagisciId=A.Id AND C.Envanterde=1 LEFT JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId AND D.EnvanterdeMi=1 LEFT JOIN Il_Table E ON E.IlAdi=A.Ili " + deceased + hidden + @" GROUP BY C.BagisciId,A.Id,A.Adi,A.Soyadi,A.TCKimlikNo,A.DogumYeri,A.DogumTarihi,A.Meslegi,A.SosyalGuvence,A.Ili,A.Ilcesi,A.Adres,A.Telefon1,A.Telefon2,A.Foto,A.Sag_vefat,E.Bolge,A.Gizli");
            if (excludeDeceased) q.AddParameter("@SagVefat", sagValue);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectAllCountBagisAdediByBolge(int bolgeId, int allRegion, int headquarters)
        {
            string filter = bolgeId == allRegion || bolgeId == headquarters ? string.Empty : " WHERE E.Id=@BolgeId ";
            SqlQuery q = new SqlQuery(@"SELECT A.Id TasinmazBagisciId,A.Adi,A.Soyadi,A.Sag_vefat,A.Adi+' '+A.Soyadi AdiSoyadi,COUNT(B.Id) ToplamBagisAdedi,C.IlAdi,D.IlceAdi,D.IlceAdi+'-'+C.IlAdi IlIlce,E.KisaAdi Bolge,E.Id BagisciBolgeId FROM TasinmazBagisci_Table A LEFT JOIN Bagis_Table B ON B.BagisciId=A.Id AND B.Envanterde=1 LEFT JOIN Il_Table C ON C.Id=A.IlId LEFT JOIN Ilce_Table D ON D.Id=A.IlceId LEFT JOIN Bolge_Table E ON E.Id=C.BolgeId LEFT JOIN Tasinmaz_Table F ON F.Id=B.TasinmazId AND F.EnvanterdeMi=1 " + filter + @" GROUP BY A.Id,A.Adi,A.Soyadi,A.Sag_vefat,C.IlAdi,D.IlceAdi,E.KisaAdi,E.Id ORDER BY A.Adi,A.Soyadi");
            if (!string.IsNullOrEmpty(filter)) q.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectTasinmazBagisci(bool excludeHidden, string edinmeSekli)
        {
            string hidden = excludeHidden ? " AND (A.Gizli IS NULL OR A.Gizli=0) " : string.Empty;
            SqlQuery q = new SqlQuery(@"SELECT A.Id TasinmazBagisciId,E.Bolge,A.Adi+' '+A.Soyadi AdiSoyadi,A.Sag_vefat,A.TCKimlikNo,A.DogumYeri,A.DogumTarihi,A.Meslegi,A.SosyalGuvence,C.TasinmazId,C.BagisTarihi,C.ArmaganId,C.ArmaganDurumu,C.ArmaganTarihi,C.Id BagisId,A.Ili,A.Ilcesi,A.Adres,A.Telefon1,A.Telefon2,A.Foto,A.Sag_vefat,A.Gizli FROM TasinmazBagisci_Table A LEFT JOIN Bagis_Table C ON C.BagisciId=A.Id AND C.Envanterde=1 LEFT JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId AND D.EnvanterdeMi=1 LEFT JOIN Il_Table E ON E.IlAdi=A.Ili LEFT JOIN Armagan_Table F ON F.Id=A.Ili WHERE D.EdinmeSekli=@EdinmeSekli " + hidden + " ORDER BY C.BagisTarihi DESC");
            q.AddParameter("@EdinmeSekli", edinmeSekli);
            return db.SelectFromDb(q, "");
        }
        public DataTable SelectUnselectedParticipants() { return db.SelectFromDb(new SqlQuery("SELECT A.Id KatilimciId,A.Adi,A.Soyadi,A.Adres,A.Telefon1 Telefon,A.Sag_vefat,A.Ilcesi Ilce,A.Ili Il FROM TasinmazBagisci_Table A WHERE Sag_vefat='Sag' ORDER BY A.Adi"), ""); }
        public DataTable SelectDeprecatedByBolge(string bolge, string allRegion, string authorizedUnit)
        {
            string filter = string.IsNullOrEmpty(bolge) || bolge.Equals(allRegion) || bolge.Equals(authorizedUnit) ? string.Empty : " WHERE E.Bolge=@Bolge ";
            SqlQuery q = new SqlQuery(@"SELECT ROW_NUMBER() OVER (ORDER BY A.Id) Sirano,Count(C.Id) ToplamBagisAdedi,A.Id TasinmazBagisciId,E.Bolge,A.Adi+' '+A.Soyadi AdiSoyadi,A.TCKimlikNo,A.DogumYeri,A.DogumTarihi,A.Meslegi,A.SosyalGuvence,A.Ilcesi+'-'+A.Ili IlIlce,A.Adres,A.Telefon1,A.Telefon2,A.Foto,A.Sag_vefat,FORMAT(A.vefatTarihi,'dd.MM.yyyy') VefatTarihi,DefinYeri,DefinIli,DefinIlcesi,DefinAciklama FROM TasinmazBagisci_Table A LEFT JOIN Bagis_Table C ON C.BagisciId=A.Id AND C.Envanterde=1 LEFT JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId AND D.EnvanterdeMi=1 LEFT JOIN Il_Table E ON E.IlAdi=A.Ili " + filter + @" GROUP BY C.BagisciId,A.Id,A.Adi,A.Soyadi,A.TCKimlikNo,A.DogumYeri,A.DogumTarihi,A.Meslegi,A.SosyalGuvence,A.Ili,A.Ilcesi,A.Adres,A.Telefon1,A.Telefon2,A.Foto,A.Sag_vefat,E.Bolge,A.VefatTarihi,DefinYeri,DefinIli,DefinIlcesi,DefinAciklama");
            if (!string.IsNullOrEmpty(filter)) q.AddParameter("@Bolge", bolge);
            return db.SelectFromDb(q, "");
        }
    }
}
