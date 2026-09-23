using DAO.Ortak;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisciRepository
    {
        private readonly DbClass db;

        public NakitBagisciRepository()
            : this(new DbClass())
        {
        }

        public NakitBagisciRepository(DbClass db)
        {
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Id = @Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTcKimlikno(long tcKimlikno)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE TCKimlikNo != 0 AND TCKimlikNo = @TCKimlikNo");
            query.AddParameter("@TCKimlikNo", tcKimlikno);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectBagisiOlmayanBagisciById(int bagisciId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.Id, A.Adi, B.BagisTarihi, B.BagisMiktari
                FROM NakitBagisci_Table A
                LEFT JOIN NakitBagisHareket_Table B ON B.BagisciId = A.Id
                WHERE A.Id = @BagisciId
                    AND B.BagisciId IS NULL
                ORDER BY BagisTarihi DESC");
            query.AddParameter("@BagisciId", bagisciId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByAdAndTelefon(string adi, string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi = @Adi AND (Telefon1 = @Telefon OR Telefon2 = @Telefon)");
            query.AddParameter("@Adi", adi ?? string.Empty);
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByTelefon(string telefon)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Telefon1 = @Telefon OR Telefon2 = @Telefon");
            query.AddParameter("@Telefon", telefon ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByEposta(string eposta)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE EPosta = @Eposta");
            query.AddParameter("@Eposta", eposta ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByAd(string adi)
        {
            SqlQuery query = new SqlQuery(@"SELECT *
                               FROM NakitBagisci_Table
                               WHERE Adi LIKE @Adi");
            query.AddParameter("@Adi", "%" + adi + "%");
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByFilter(string filter, int eksiId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon,
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor
                FROM NakitBagisci_Table N
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                WHERE N.Id != @ExcludedId
                    AND (
                        N.Adi LIKE @Filter
	                    OR N.TCKimlikNo LIKE @Filter
	                    OR N.Telefon1 LIKE @Filter
	                    OR N.Adres LIKE @Filter
                    )
                ORDER BY NakitBagisciId ");

            query.AddParameter("@ExcludedId", eksiId);
            query.AddParameter("@Filter", "%" + filter + "%");

            return db.SelectFromDb(query, "");
        }
    }
}
