using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.Logging;

namespace DAO.Repositories.MTS
{
    public sealed class MtsContactWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly MtsSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();
        public MtsContactWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new MtsSqlExecutor(context, "MtsContactWriteRepository");
        }
        // All migrated MTS writers must use this lock before their row locks.
        public void Lock(SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=N'PortalApps.MTS.Write',
                @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
                SELECT @result Result;");
            DataTable rows = sql.Select(query, transaction);
            if (rows == null || Convert.ToInt32(rows.Rows[0]["Result"]) < 0)
                throw new InvalidOperationException("MTS işlem kilidi alınamadı.");
        }
        public DataTable Read(string table, int id, SqlTransactionContext transaction)
        {
            Require(transaction); Fields(table);
            var query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }
        public bool DuplicateIdentity(int id, long identity, SqlTransactionContext transaction)
        {
            Require(transaction);
            if (identity <= 0) return false;
            var query = new SqlQuery("SELECT Id FROM Kisi_Table WITH (UPDLOCK,HOLDLOCK) WHERE Id<>@Id AND TCKimlikNo=@Identity");
            query.AddParameter("@Id", id); query.AddParameter("@Identity", identity);
            return sql.Select(query, transaction) != null;
        }
        public bool PersonInUse(int id, SqlTransactionContext transaction)
        {
            Require(transaction);
            // Include contacts and organization assignments as well as visible participation/history.
            var query = new SqlQuery(@"SELECT TOP (1) Id FROM FaaliyetKatilim_Table WITH (UPDLOCK,HOLDLOCK) WHERE KatilimciId=@Id
                UNION ALL SELECT TOP (1) Id FROM AramaGorusme_Table WITH (UPDLOCK,HOLDLOCK) WHERE ArayanId=@Id
                UNION ALL SELECT TOP (1) Id FROM AniObjesiDagitim_Table WITH (UPDLOCK,HOLDLOCK) WHERE KatilimciId=@Id
                UNION ALL SELECT TOP (1) Id FROM Faaliyet_Table WITH (UPDLOCK,HOLDLOCK) WHERE DisIrtibatId=@Id
                UNION ALL SELECT TOP (1) Id FROM MTSKurumGorev_Table WITH (UPDLOCK,HOLDLOCK) WHERE KisiId=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction) != null;
        }
        public bool ValidPersonReferences(int titleId, int provinceId, int districtId, SqlTransactionContext transaction)
        {
            Require(transaction);
            if (titleId < 0 || provinceId < 0 || districtId < 0) return false;
            if (titleId > 0)
            {
                var title = new SqlQuery("SELECT Id FROM MTSUnvanTanim_Table WITH (HOLDLOCK) WHERE Id=@Id");
                title.AddParameter("@Id", titleId);
                if (sql.Select(title, transaction) == null) return false;
            }
            if (provinceId > 0)
            {
                var province = new SqlQuery("SELECT Id FROM IL_Table WITH (HOLDLOCK) WHERE Id=@Id");
                province.AddParameter("@Id", provinceId);
                if (sql.Select(province, transaction) == null) return false;
            }
            if (districtId > 0)
            {
                var district = new SqlQuery("SELECT Id FROM ILCE_Table WITH (HOLDLOCK) WHERE Id=@Id AND IlId=@Province");
                district.AddParameter("@Id", districtId); district.AddParameter("@Province", provinceId);
                if (sql.Select(district, transaction) == null) return false;
            }
            return true;
        }
        public string Organization(int personId, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"SELECT TOP (1) K.Adi Kurum, G.Adi Gorev
                FROM MTSKurumGorev_Table A WITH (HOLDLOCK)
                LEFT JOIN MTSKurumTanim_Table K ON K.Id=A.MTSKurumTanimId
                LEFT JOIN MTSGorevTanim_Table G ON G.Id=A.MTSGorevTanimId
                WHERE A.KisiId=@Id AND A.Durum=@Status ORDER BY A.Id DESC");
            query.AddParameter("@Id", personId);
            query.AddParameter("@Status", Utility.ProjeGlobal.ProjeConstants.MTSGOREVDURUMU_GOREVDE);
            DataTable rows = sql.Select(query, transaction);
            return rows == null ? null : Convert.ToString(rows.Rows[0]["Kurum"]) + " / " + Convert.ToString(rows.Rows[0]["Gorev"]);
        }
        public int Save<T>(string table, T item, int id, SqlTransactionContext transaction)
        {
            Require(transaction); Fields(table);
            DataTable before = id > 0 ? Read(table, id, transaction) : null;
            if (id > 0 && before == null) throw new InvalidOperationException("Güncellenecek MTS kaydı bulunamadı.");
            string[] dates = { "Tarih", "BaslangicTarihi", "BitisTarihi", "DogumTarihi", "VerilisTarihi", "SonIslemTarihi" };
            if (id > 0)
            {
                if (!sql.Write(builder.BuildUpdate(item, table, dates), "UPDATE", transaction))
                    throw new InvalidOperationException("MTS kaydı güncellenemedi.");
            }
            else
            {
                id = sql.Insert(builder.BuildInsert(item, table, dates), transaction);
                if (id <= 0) throw new InvalidOperationException("MTS kaydı oluşturulamadı.");
            }
            DataTable after = Read(table, id, transaction);
            if (after == null) throw new InvalidOperationException("Kaydedilen MTS kaydı okunamadı.");
            Audit(table, id, before, after, transaction);
            return id;
        }
        public void Delete(string table, int id, SqlTransactionContext transaction)
        {
            DataTable before = Read(table, id, transaction);
            if (before == null) throw new InvalidOperationException("Silinecek MTS kaydı bulunamadı.");
            if (!sql.Write(builder.BuildDelete(table, id), "DELETE", transaction))
                throw new InvalidOperationException("MTS kaydı silinemedi.");
            Audit(table, id, before, null, transaction);
        }
        private void Audit(string table, int id, DataTable before, DataTable after, SqlTransactionContext transaction)
        {
            var log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, before == null ? "INSERT" : after == null ? "DELETE" : "UPDATE",
                table.Replace("_Table", ""), id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in Fields(table))
            {
                string oldValue = Value(before, field), newValue = Value(after, field);
                if (oldValue != newValue) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }
        private static string Value(DataTable rows, string field)
        {
            if (rows == null || rows.Rows[0].IsNull(field)) return null;
            object value = rows.Rows[0][field];
            return value is DateTime ? ((DateTime)value).ToString("o", CultureInfo.InvariantCulture) : Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static string[] Fields(string table)
        {
            // Do not write identities, names, phone numbers, addresses or free text to audit changes.
            switch (table)
            {
                case "Kisi_Table": return new[] { "MTSUnvanTanimId", "Ili", "Ilcesi", "Kutlama", "RandevuKisiti" };
                case "AramaGorusme_Table": return new[] { "FaaliyetId", "Tarih", "GorusmeSekli", "GorusmeSaglandi", "RandevuIstendi" };
                case "Faaliyet_Table": return new[] { "FaaliyetAmaciId", "FaaliyetDurumu", "BaslangicTarihi", "BitisTarihi", "AcikTarih", "TumGun", "TakvimeIslendi" };
                case "FaaliyetKatilim_Table": return new[] { "FaaliyetId", "KatilimciTipi", "TakvimDaveti" };
                case "AniObjesiDagitim_Table": return new[] { "AniObjesiId", "FaaliyetId", "Adet", "VerilenAlinan", "CikisDepoId", "VerilisTarihi" };
                case "DepoStok_Table": return new[] { "AniObjesiId", "DepoId", "SonAdet", "SonIslemTarihi" };
                default: throw new ArgumentException("Geçersiz MTS kayıt türü.", "table");
            }
        }
        private static void Require(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }
    }
}
