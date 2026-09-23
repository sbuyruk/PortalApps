using DAO.Ortak;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisciSearchRepository
    {
        private readonly DbClass db;

        public NakitBagisciSearchRepository()
            : this(new DbClass())
        {
        }

        public NakitBagisciSearchRepository(DbClass db)
        {
            this.db = db;
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
