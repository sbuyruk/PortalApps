using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.IKYS
{
    public class PersonelRepository
    {
        private readonly DbClass db;

        public PersonelRepository() : this(new DbClass())
        {
        }

        public PersonelRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
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
    }
}
