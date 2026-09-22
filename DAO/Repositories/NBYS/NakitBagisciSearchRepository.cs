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
            string sqlString = string.Format(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM NakitBagisci_Table N 
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili 
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                    ---INNER JOIN NakitBagisHareket_Table C on C.BagisciId=N.Id
                WHERE N.Id!= {0} AND N.Adi like '%{1}%'
	                OR N.TCKimlikNo like '%{1}%'
	                OR N.Telefon1 like '%{1}%'
	                OR N.Adres like '%{1}%'
                ORDER BY NakitBagisciId ", eksiId, filter);

            return db.SelectFromDb(sqlString, "");
        }
    }
}
