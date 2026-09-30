using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class EkstreAktarmaRepository
    {
        private const string TableName = "EkstreAktarma_Table";
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public EkstreAktarmaRepository() : this(new DbClass()) { }

        public EkstreAktarmaRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EkstreAktarma_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM EkstreAktarma_Table"), "");
        }

        public DataTable SelectList(DateTime processDate, bool excludeTransferred, string bankName)
        {
            string filters = excludeTransferred ? " AND AktarildiMi=0" : string.Empty;
            if (!string.IsNullOrEmpty(bankName)) filters += " AND BankaAdi=@BankaAdi";
            SqlQuery query = new SqlQuery(@"SELECT Id EkstreAktarmaId,NakitBagisciId,BankaAdi,TCKimlikNo,Adi,Soyadi,
                    ISNULL(Adi,'')+' '+ISNULL(Soyadi,'') AdiSoyadi,BagisTarihi,Tutar,DovizCinsi,
                    DovizTutari,DovizKuru,KurTarihi,AktarildiMi,Adres,Aciklama,Telefon1,Telefon2,
                    Telefon1+IIF(ISNULL(Telefon1,'')<>'' AND ISNULL(Telefon2,'')<>'',' - ','')+Telefon2 Telefon
                FROM EkstreAktarma_Table WHERE IslemTarihi=@IslemTarihi" + filters + @"
                ORDER BY AktarildiMi,BagisTarihi DESC,Id,Adi");
            query.AddParameter("@IslemTarihi", processDate);
            if (!string.IsNullOrEmpty(bankName)) query.AddParameter("@BankaAdi", bankName);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBankAndReceipt(string bankNamePart, string receiptNumber)
        {
            SqlQuery query = new SqlQuery(@"SELECT * FROM EkstreAktarma_Table
                WHERE BankaAdi LIKE @BankaAdi AND FisNo=@FisNo");
            query.AddParameter("@BankaAdi", "%" + bankNamePart + "%");
            query.AddParameter("@FisNo", receiptNumber);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByIds(IList<int> ids, bool includeIdDescending)
        {
            if (ids == null || ids.Count == 0) return new DataTable();
            List<string> parameters = new List<string>();
            for (int i = 0; i < ids.Count; i++) parameters.Add("@Id" + i);
            string orderBy = includeIdDescending
                ? "ORDER BY AktarildiMi,Id DESC,BagisTarihi DESC,Adi"
                : "ORDER BY AktarildiMi,BagisTarihi DESC,Adi";
            SqlQuery query = new SqlQuery("SELECT * FROM EkstreAktarma_Table WHERE Id IN (" +
                string.Join(",", parameters) + ") " + orderBy);
            for (int i = 0; i < ids.Count; i++) query.AddParameter(parameters[i], ids[i]);
            return db.SelectFromDb(query, "");
        }

        public bool ExistsByBankAndProcessDate(string bankName, DateTime processDate)
        {
            SqlQuery query = new SqlQuery(@"SELECT TOP 1 Id FROM EkstreAktarma_Table
                WHERE BankaAdi=@BankaAdi AND IslemTarihi=@IslemTarihi AND ElleKayit<>1");
            query.AddParameter("@BankaAdi", bankName);
            query.AddParameter("@IslemTarihi", processDate);
            DataTable table = db.SelectFromDb(query, "");
            return table != null && table.Rows.Count > 0;
        }

        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, TableName)); }
        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, TableName)); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete(TableName, id), ""); }
    }
}
