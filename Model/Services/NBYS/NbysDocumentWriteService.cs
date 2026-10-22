using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public sealed class NbysDocumentBatch
    {
        public DataTable Rows { get; internal set; }
        public int RegionId { get; internal set; }
        public int DefinitionId { get; internal set; }
        public DateTime Start { get; internal set; }
        public DateTime End { get; internal set; }
        public bool Forbidden { get; internal set; }
        public int Count { get { return Rows == null ? 0 : Rows.Rows.Count; } }
    }

    public enum NbysDocumentWriteResult { Completed, Empty, Forbidden, Changed }

    public sealed class NbysDocumentWriteService
    {
        private readonly PortalLogContext context;
        private readonly ArmaganRepository gifts;
        private readonly NbysBagisciWriteRepository records;
        public NbysDocumentWriteService(PortalLogContext context)
        {
            this.context = context;
            gifts = new ArmaganRepository(context);
            records = new NbysBagisciWriteRepository(context);
        }

        public NbysDocumentBatch Prepare(DateTime start, DateTime end, int definitionId, int regionId)
        {
            if (start > end || start < new DateTime(1753, 1, 1)) throw new ArgumentException("Belge tarih aralığı geçersiz.");
            if (!new[] { ProjeConstants.ARMAGAN_TESEKKURID, ProjeConstants.ARMAGAN_BRONZID,
                ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.ARMAGAN_ALTINID,
                ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID }.Contains(definitionId))
                throw new ArgumentException("Belge türü geçersiz.");
            NbysDocumentBatch batch = new NbysDocumentBatch { Start = start, End = end, DefinitionId = definitionId, RegionId = regionId };
            Bolge userRegion = FirstRegion(records.SelectUserRegion(context.UserLogin));
            if (!CanAccess(userRegion, regionId)) { batch.Forbidden = true; return batch; }
            batch.Rows = ValidRows(Read(batch, null));
            return batch;
        }

        public NbysDocumentWriteResult MarkSent(NbysDocumentBatch batch)
        {
            if (batch == null || batch.Count == 0) return NbysDocumentWriteResult.Empty;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (!CanAccess(FirstRegion(records.SelectUserRegion(context.UserLogin, transaction)), batch.RegionId))
                    return NbysDocumentWriteResult.Forbidden;
                foreach (int donorId in batch.Rows.AsEnumerable().Select(row => Convert.ToInt32(row["BagisciId"])).Distinct().OrderBy(id => id))
                    records.SelectDonor(donorId, transaction);
                DataTable current = ValidRows(Read(batch, transaction));
                // The document uses this exact snapshot. Never update a newly matching record or stale document.
                if (!SameRows(batch.Rows, current)) return NbysDocumentWriteResult.Changed;
                NbysDocumentWriteRepository writer = new NbysDocumentWriteRepository(context);
                foreach (DataRow row in current.Rows)
                    writer.MarkSent(Convert.ToInt32(row["ArmaganId"]), ProjeConstants.DURUM_GONDERILDI, transaction);
                transaction.Complete();
            }
            return NbysDocumentWriteResult.Completed;
        }

        private DataTable Read(NbysDocumentBatch batch, SqlTransactionContext transaction)
        {
            int? region = IsCentral(batch.RegionId) ? (int?)null : batch.RegionId;
            return gifts.SelectByDurumTarih(ProjeConstants.DURUM_KONTROLEDILDI, batch.Start, batch.End,
                batch.DefinitionId, region, null, transaction);
        }

        private static DataTable ValidRows(DataTable source)
        {
            if (source == null) return null;
            DataTable result = source.Clone();
            foreach (DataRow row in source.Rows)
                if (row.IsNull("BelgeGecersizMi") || Convert.ToInt32(row["BelgeGecersizMi"]) != 1) result.ImportRow(row);
            return result;
        }

        private static bool SameRows(DataTable expected, DataTable current)
        {
            if (current == null || current.Rows.Count != expected.Rows.Count) return false;
            var rows = current.AsEnumerable().ToDictionary(row => Convert.ToInt32(row["ArmaganId"]));
            foreach (DataRow before in expected.Rows)
            {
                DataRow after;
                if (!rows.TryGetValue(Convert.ToInt32(before["ArmaganId"]), out after)) return false;
                foreach (DataColumn field in expected.Columns)
                    if (field.ColumnName != "Sirano" && Convert.ToString(before[field], CultureInfo.InvariantCulture)
                        != Convert.ToString(after[field.ColumnName], CultureInfo.InvariantCulture)) return false;
            }
            return true;
        }

        private static Bolge FirstRegion(DataTable rows) { return new Bolge().ToList<Bolge>(rows).FirstOrDefault(); }
        private static bool CanAccess(Bolge user, int regionId)
        {
            return user != null && (IsCentral(user.Id) || (user.Id > 0 && !IsCentral(regionId) && user.Id == regionId));
        }
        public static bool IsCentral(int regionId)
        {
            return regionId == ProjeConstants.BOLGE_HEPSI_INT || regionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT;
        }
    }
}
