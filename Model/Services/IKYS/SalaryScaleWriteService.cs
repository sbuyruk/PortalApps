using DAO.Ortak;
using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public enum SalaryScaleResult { Completed, Changed, NotFound, LastGroup, InvalidInput, OverlappingPeriod }

    public sealed class SalaryScaleSnapshot
    {
        public List<UcretTanim> Rows { get; internal set; }
        public string Version { get; internal set; }
        public bool CanDelete { get; internal set; }
    }

    public sealed class SalaryScaleWriteService
    {
        private readonly PortalLogContext context;
        private readonly SalaryScaleWriteRepository repository;
        public SalaryScaleWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new SalaryScaleWriteRepository(context);
        }

        public SalaryScaleSnapshot Load() { return Snapshot(Rows(repository.Read())); }

        public SalaryScaleResult Create(DateTime from, DateTime to, decimal increase, decimal allowance, string version, out SalaryScaleSnapshot saved)
        {
            saved = null;
            if (from < new DateTime(1753, 1, 1) || to < from || increase < 0 || allowance < 0) return SalaryScaleResult.InvalidInput;
            SalaryScaleSnapshot completed;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var all = Rows(repository.Read(transaction));
                var current = Snapshot(all);
                if (current.Rows.Count == 0) return SalaryScaleResult.NotFound;
                if (current.Version != version) return SalaryScaleResult.Changed;
                if (all.Any(row => row.BaslangicTarihi.Date <= to.Date && row.BitisTarihi.Date >= from.Date)) return SalaryScaleResult.OverlappingPeriod;
                if (current.Rows[0].GrupId == int.MaxValue) return SalaryScaleResult.InvalidInput;
                var pending = new List<UcretTanim>();
                try
                {
                    foreach (var old in current.Rows)
                        pending.Add(new UcretTanim { GrupId = old.GrupId + 1, Derece = old.Derece, Kademe = old.Kademe, Unvan = old.Unvan,
                            AltUcret = old.AltUcret + old.AltUcret * increase / 100, UstUcret = old.UstUcret + old.UstUcret * increase / 100,
                            AskerUcret = old.AskerUcret + old.AskerUcret * increase / 100, Agi = allowance,
                            BaslangicTarihi = from.Date, BitisTarihi = to.Date, Olusturan = context.UserLogin, OlusturmaTarihi = DateTime.Now });
                }
                catch (OverflowException) { return SalaryScaleResult.InvalidInput; }
                foreach (var row in pending)
                {
                    row.Id = repository.Insert(row, transaction);
                    if (row.Id <= 0) throw new InvalidOperationException("Maaş tablosu satırı kaydedilemedi.");
                    AuditScale(row, "INSERT", transaction);
                }
                completed = Snapshot(Rows(repository.Read(transaction)));
                transaction.Complete();
            }
            saved = completed;
            return SalaryScaleResult.Completed;
        }

        public SalaryScaleResult Delete(string version, out SalaryScaleSnapshot saved)
        {
            saved = null;
            SalaryScaleSnapshot completed;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var current = Snapshot(Rows(repository.Read(transaction)));
                if (current.Rows.Count == 0) return SalaryScaleResult.NotFound;
                if (current.Version != version) return SalaryScaleResult.Changed;
                if (!current.CanDelete) return SalaryScaleResult.LastGroup;
                int groupId = current.Rows[0].GrupId;
                DataTable salaries = repository.ReadSalaries(groupId, transaction);
                repository.Delete(groupId, transaction);
                foreach (var row in current.Rows) AuditScale(row, "DELETE", transaction);
                if (salaries != null) foreach (DataRow row in salaries.Rows)
                    new PortalLogRepository().RecordAudit(context, "DELETE", "MaasHareket", Convert.ToString(row["Id"], CultureInfo.InvariantCulture), transaction);
                completed = Snapshot(Rows(repository.Read(transaction)));
                transaction.Complete();
            }
            saved = completed;
            return SalaryScaleResult.Completed;
        }

        private void AuditScale(UcretTanim row, string operation, SqlTransactionContext transaction)
        {
            var log = new PortalLogRepository();
            int id = log.RecordAudit(context, operation, "UcretTanim", row.Id.ToString(CultureInfo.InvariantCulture), transaction);
            if (id <= 0) return;
            // Retain the existing metadata-only audit policy; salary amounts are not copied into the log.
            var fields = new Dictionary<string, string> { { "GrupId", row.GrupId.ToString(CultureInfo.InvariantCulture) },
                { "Derece", row.Derece.ToString(CultureInfo.InvariantCulture) }, { "Kademe", row.Kademe.ToString(CultureInfo.InvariantCulture) },
                { "BaslangicTarihi", row.BaslangicTarihi.ToString("o", CultureInfo.InvariantCulture) }, { "BitisTarihi", row.BitisTarihi.ToString("o", CultureInfo.InvariantCulture) } };
            foreach (var field in fields)
                log.RecordAuditChange(id, field.Key, operation == "DELETE" ? field.Value : null, operation == "INSERT" ? field.Value : null, transaction);
        }

        private static List<UcretTanim> Rows(DataTable table) { return new UcretTanim().ToList<UcretTanim>(table); }

        private static SalaryScaleSnapshot Snapshot(List<UcretTanim> all)
        {
            int latest = all.Count == 0 ? 0 : all.Max(row => row.GrupId);
            var rows = all.Where(row => row.GrupId == latest).OrderBy(row => row.Id).ToList();
            bool canDelete = all.Select(row => row.GrupId).Distinct().Count() > 1;
            string json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(new { Rows = rows, CanDelete = canDelete });
            string version;
            using (var hash = SHA256.Create()) version = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(json)));
            return new SalaryScaleSnapshot { Rows = rows, CanDelete = canDelete, Version = version };
        }
    }
}
