using DAO.Ortak;
using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public enum MtsActivityResult { Completed, NotFound, InvalidInput, InUse, Changed, Duplicate, InsufficientStock, InvalidRelation, Restricted, AlreadyCompleted }

    public sealed class MtsActivityWriteService
    {
        private readonly PortalLogContext context;
        private readonly MtsActivityWriteRepository repository;
        private MtsContactWriteRepository Records { get { return repository.Records; } }
        public MtsActivityWriteService(PortalLogContext context)
        {
            if (context == null || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("MTS kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
            repository = new MtsActivityWriteRepository(context);
        }
        public MtsActivityResult Save(Faaliyet input, string version, out Faaliyet saved)
        {
            saved = null;
            if (!Valid(input)) return MtsActivityResult.InvalidInput;
            Faaliyet completed;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                if (input.Id == 0 && input.UniqueId != Guid.Empty)
                {
                    var existing = List<Faaliyet>(repository.ByUniqueId(input.UniqueId, transaction));
                    if (existing.Count > 1) return MtsActivityResult.InvalidRelation;
                    if (existing.Count == 1)
                    {
                        if (existing[0].Olusturan != context.UserLogin) return MtsActivityResult.InvalidRelation;
                        transaction.Complete(); saved = existing[0];
                        return MtsActivityResult.AlreadyCompleted;
                    }
                }
                var old = input.Id > 0 ? First<Faaliyet>(Records.Read("Faaliyet_Table", input.Id, transaction)) : null;
                if (input.Id > 0 && old == null) return MtsActivityResult.NotFound;
                if (old != null && MtsContactWriteService.Version(old) != version) return MtsActivityResult.Changed;
                if (input.DisIrtibatId < 0) return MtsActivityResult.InvalidInput;
                if (input.DisIrtibatId > 0)
                {
                    var contact = First<Kisi>(Records.Read("Kisi_Table", input.DisIrtibatId, transaction));
                    if (contact == null) return MtsActivityResult.InvalidRelation;
                    if (contact.RandevuKisiti && (old == null || old.DisIrtibatId != input.DisIrtibatId)) return MtsActivityResult.Restricted;
                }
                input.UniqueId = old == null ? (input.UniqueId == Guid.Empty ? Guid.NewGuid() : input.UniqueId)
                    : old.UniqueId == Guid.Empty ? Guid.NewGuid() : old.UniqueId;
                input.FaaliyetKonusu = input.FaaliyetKonusu.Trim(); Stamp(input, old);
                int id = Records.Save("Faaliyet_Table", input, input.Id, transaction);
                completed = First<Faaliyet>(Records.Read("Faaliyet_Table", id, transaction));
                transaction.Complete();
            }
            saved = completed;
            return MtsActivityResult.Completed;
        }
        // Used by calendar writers in the next piece; dates change without replacing unrelated fields.
        public MtsActivityResult Move(int activityId, string version, DateTime start, DateTime end, out Faaliyet saved)
        {
            saved = null;
            if (start < new DateTime(1753, 1, 1) || end <= start) return MtsActivityResult.InvalidInput;
            Faaliyet completed;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var current = First<Faaliyet>(Records.Read("Faaliyet_Table", activityId, transaction));
                if (current == null) return MtsActivityResult.NotFound;
                if (MtsContactWriteService.Version(current) != version) return MtsActivityResult.Changed;
                current.BaslangicTarihi = start; current.BitisTarihi = end;
                current.BaslangicSaati = start.ToString("HH:mm"); current.BitisSaati = end.ToString("HH:mm");
                Stamp(current, current); Records.Save("Faaliyet_Table", current, current.Id, transaction);
                completed = First<Faaliyet>(Records.Read("Faaliyet_Table", current.Id, transaction));
                transaction.Complete();
            }
            saved = completed;
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult Delete(int activityId, string version, out Faaliyet deleted)
        {
            deleted = null; Faaliyet completed;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var current = First<Faaliyet>(Records.Read("Faaliyet_Table", activityId, transaction));
                if (current == null) return MtsActivityResult.NotFound;
                if (MtsContactWriteService.Version(current) != version) return MtsActivityResult.Changed;
                if (repository.HasCalls(activityId, transaction) || repository.Participants(activityId, 0, transaction) != null
                    || repository.Distributions(activityId, 0, transaction) != null) return MtsActivityResult.InUse;
                Records.Delete("Faaliyet_Table", activityId, transaction); completed = current;
                transaction.Complete();
            }
            deleted = completed;
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult SetContact(int activityId, string version, int personId)
        {
            if (personId < 0) return MtsActivityResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var current = First<Faaliyet>(Records.Read("Faaliyet_Table", activityId, transaction));
                if (current == null) return MtsActivityResult.NotFound;
                if (MtsContactWriteService.Version(current) != version) return MtsActivityResult.Changed;
                if (personId > 0)
                {
                    var person = First<Kisi>(Records.Read("Kisi_Table", personId, transaction));
                    if (person == null) return MtsActivityResult.InvalidRelation;
                    if (person.RandevuKisiti && current.DisIrtibatId != personId) return MtsActivityResult.Restricted;
                }
                current.DisIrtibatId = personId; Stamp(current, current);
                Records.Save("Faaliyet_Table", current, current.Id, transaction);
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult AddParticipant(int activityId, int personId)
        {
            if (personId <= 0) return MtsActivityResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                if (Records.Read("Faaliyet_Table", activityId, transaction) == null) return MtsActivityResult.NotFound;
                var person = First<Kisi>(Records.Read("Kisi_Table", personId, transaction));
                if (person == null) return MtsActivityResult.InvalidRelation;
                if (person.RandevuKisiti) return MtsActivityResult.Restricted;
                if (repository.Participants(activityId, personId, transaction) != null) return MtsActivityResult.Duplicate;
                var participant = new FaaliyetKatilim { FaaliyetId = activityId, KatilimciId = personId,
                    KurumGorev = Records.Organization(personId, transaction) ?? person.Kurumu + " / " + person.Gorevi };
                Stamp(participant, null); Records.Save("FaaliyetKatilim_Table", participant, 0, transaction);
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult RemoveParticipant(int activityId, int participationId)
        {
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                if (Records.Read("Faaliyet_Table", activityId, transaction) == null) return MtsActivityResult.NotFound;
                var current = First<FaaliyetKatilim>(Records.Read("FaaliyetKatilim_Table", participationId, transaction));
                if (current == null) return MtsActivityResult.NotFound;
                if (current.FaaliyetId != activityId) return MtsActivityResult.InvalidRelation;
                if (repository.Distributions(activityId, current.KatilimciId, transaction) != null) return MtsActivityResult.InUse;
                Records.Delete("FaaliyetKatilim_Table", current.Id, transaction);
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public string StockVersion(int warehouseId, int itemId)
        {
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var stock = List<DepoStok>(repository.Stock(warehouseId, itemId, transaction));
                if (stock.Count != 1) return null;
                string result = MtsContactWriteService.Version(stock[0]);
                transaction.Complete(); return result;
            }
        }
        public MtsActivityResult Deliver(int activityId, int personId, int warehouseId, int itemId, int quantity, string stockVersion)
        {
            if (quantity <= 0 || warehouseId <= 0 || itemId <= 0) return MtsActivityResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                Faaliyet activity;
                var valid = ValidateParticipant(activityId, personId, transaction, out activity);
                if (valid != MtsActivityResult.Completed) return valid;
                var item = First<AniObjesiTanim>(repository.Item(itemId, transaction));
                if (item == null || item.StokluMu != ProjeConstants.MTS_ANIOBJESISTOKLU || !repository.WarehouseExists(warehouseId, transaction))
                    return MtsActivityResult.InvalidRelation;
                var stocks = List<DepoStok>(repository.Stock(warehouseId, itemId, transaction));
                if (stocks.Count == 0) return MtsActivityResult.InsufficientStock;
                if (stocks.Count != 1) return MtsActivityResult.InvalidRelation;
                var stock = stocks[0];
                if (MtsContactWriteService.Version(stock) != stockVersion) return MtsActivityResult.Changed;
                if (stock.SonAdet < quantity) return MtsActivityResult.InsufficientStock;
                // Distribution belongs to the original source warehouse; never move its provenance on an increment.
                var matches = List<AniObjesiDagitim>(repository.Distributions(activityId, personId, transaction))
                    .Where(x => x.AniObjesiId == itemId && x.CikisDepoId == warehouseId && x.VerilenAlinan == ProjeConstants.ANIOBJESI_VERILEN_INT).ToList();
                if (matches.Count > 1) return MtsActivityResult.InvalidRelation;
                var old = matches.FirstOrDefault();
                if (old != null && old.Adet < 0) return MtsActivityResult.InvalidRelation;
                var distribution = old ?? new AniObjesiDagitim { AniObjesiId = itemId, CikisDepoId = warehouseId,
                    FaaliyetId = activityId, KatilimciId = personId, VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT };
                if ((long)distribution.Adet + quantity > int.MaxValue) return MtsActivityResult.InvalidInput;
                distribution.Adet += quantity; distribution.VerilisTarihi = activity.BaslangicTarihi;
                Stamp(distribution, old); Records.Save("AniObjesiDagitim_Table", distribution, distribution.Id, transaction);
                stock.SonAdet -= quantity; StampStock(stock); Records.Save("DepoStok_Table", stock, stock.Id, transaction);
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult Return(int activityId, int distributionId, int quantity, string version)
        {
            if (quantity <= 0) return MtsActivityResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var distribution = First<AniObjesiDagitim>(Records.Read("AniObjesiDagitim_Table", distributionId, transaction));
                if (distribution == null) return MtsActivityResult.NotFound;
                if (distribution.FaaliyetId != activityId || distribution.VerilenAlinan != ProjeConstants.ANIOBJESI_VERILEN_INT)
                    return MtsActivityResult.InvalidRelation;
                if (MtsContactWriteService.Version(distribution) != version) return MtsActivityResult.Changed;
                Faaliyet activity;
                var valid = ValidateParticipant(activityId, distribution.KatilimciId, transaction, out activity);
                if (valid != MtsActivityResult.Completed) return valid;
                var item = First<AniObjesiTanim>(repository.Item(distribution.AniObjesiId, transaction));
                if (item == null || item.StokluMu != ProjeConstants.MTS_ANIOBJESISTOKLU || distribution.CikisDepoId <= 0
                    || !repository.WarehouseExists(distribution.CikisDepoId, transaction)) return MtsActivityResult.InvalidRelation;
                if (quantity > distribution.Adet) return MtsActivityResult.InvalidInput;
                var stocks = List<DepoStok>(repository.Stock(distribution.CikisDepoId, distribution.AniObjesiId, transaction));
                if (stocks.Count > 1) return MtsActivityResult.InvalidRelation;
                var stock = stocks.SingleOrDefault() ?? new DepoStok { DepoId = distribution.CikisDepoId, AniObjesiId = distribution.AniObjesiId };
                if (stock.SonAdet < 0 || (long)stock.SonAdet + quantity > int.MaxValue) return MtsActivityResult.InvalidInput;
                stock.SonAdet += quantity; StampStock(stock); Records.Save("DepoStok_Table", stock, stock.Id, transaction);
                distribution.Adet -= quantity;
                if (distribution.Adet == 0) Records.Delete("AniObjesiDagitim_Table", distribution.Id, transaction);
                else { Stamp(distribution, distribution); Records.Save("AniObjesiDagitim_Table", distribution, distribution.Id, transaction); }
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public string ObjectVersion(int activityId, int personId)
        {
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                string version = MtsContactWriteService.Version(List<AniObjesiDagitim>(repository.Distributions(activityId, personId, transaction)));
                transaction.Complete(); return version;
            }
        }
        public MtsActivityResult SaveObjects(int activityId, int personId, IDictionary<int, int> quantities, string version)
        {
            if (quantities == null || quantities.Count > 500 || quantities.Any(p => p.Key <= 0 || p.Value < 0)) return MtsActivityResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction); Faaliyet activity;
                var valid = ValidateParticipant(activityId, personId, transaction, out activity);
                if (valid != MtsActivityResult.Completed) return valid;
                var all = List<AniObjesiDagitim>(repository.Distributions(activityId, personId, transaction));
                if (MtsContactWriteService.Version(all) != version) return MtsActivityResult.Changed;
                // Validate the whole selection before modifying any rows.
                foreach (int id in quantities.Keys)
                {
                    var definition = First<AniObjesiTanim>(repository.Item(id, transaction));
                    if (definition == null || definition.StokluMu != ProjeConstants.MTS_ANIOBJESISTOKSUZ) return MtsActivityResult.InvalidRelation;
                }
                var existing = new List<AniObjesiDagitim>();
                foreach (var row in all.Where(x => x.VerilenAlinan == ProjeConstants.ANIOBJESI_VERILEN_INT && x.CikisDepoId == 0))
                {
                    var definition = First<AniObjesiTanim>(repository.Item(row.AniObjesiId, transaction));
                    if (definition != null && definition.StokluMu == ProjeConstants.MTS_ANIOBJESISTOKSUZ) existing.Add(row);
                }
                if (existing.GroupBy(x => x.AniObjesiId).Any(g => g.Count() > 1)) return MtsActivityResult.InvalidRelation;
                foreach (var row in existing)
                    if (!quantities.ContainsKey(row.AniObjesiId) || quantities[row.AniObjesiId] == 0) Records.Delete("AniObjesiDagitim_Table", row.Id, transaction);
                foreach (var pair in quantities.Where(x => x.Value > 0))
                {
                    var old = existing.SingleOrDefault(x => x.AniObjesiId == pair.Key);
                    if (old != null && old.Adet == pair.Value) continue;
                    var row = old ?? new AniObjesiDagitim { AniObjesiId = pair.Key, FaaliyetId = activityId, KatilimciId = personId,
                        VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT };
                    row.Adet = pair.Value; row.VerilisTarihi = activity.BaslangicTarihi; Stamp(row, old);
                    Records.Save("AniObjesiDagitim_Table", row, row.Id, transaction);
                }
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult SaveReceived(int activityId, int personId, string text, string version)
        {
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction); Faaliyet activity;
                var valid = ValidateParticipant(activityId, personId, transaction, out activity);
                if (valid != MtsActivityResult.Completed) return valid;
                var all = List<AniObjesiDagitim>(repository.Distributions(activityId, personId, transaction));
                if (MtsContactWriteService.Version(all) != version) return MtsActivityResult.Changed;
                var matches = all.Where(x => x.VerilenAlinan == ProjeConstants.ANIOBJESI_GETIRILEN_INT).ToList();
                if (matches.Count > 1) return MtsActivityResult.InvalidRelation;
                var old = matches.SingleOrDefault();
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (old != null) Records.Delete("AniObjesiDagitim_Table", old.Id, transaction);
                }
                else
                {
                    var row = old ?? new AniObjesiDagitim { AniObjesiId = ProjeConstants.GETIRILEN_ANIOBJESIID_INT,
                        FaaliyetId = activityId, KatilimciId = personId, VerilenAlinan = ProjeConstants.ANIOBJESI_GETIRILEN_INT };
                    row.GetirilenAniObjesi = text.Trim(); row.VerilisTarihi = activity.BaslangicTarihi; Stamp(row, old);
                    Records.Save("AniObjesiDagitim_Table", row, row.Id, transaction);
                }
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        public MtsActivityResult MarkInvitation(int activityId, int participationId, string version)
        {
            using (var transaction = new SqlTransactionContext())
            {
                Records.Lock(transaction);
                var current = First<FaaliyetKatilim>(Records.Read("FaaliyetKatilim_Table", participationId, transaction));
                if (current == null) return MtsActivityResult.NotFound;
                if (current.FaaliyetId != activityId) return MtsActivityResult.InvalidRelation;
                var activity = First<Faaliyet>(Records.Read("Faaliyet_Table", activityId, transaction));
                if (activity == null) return MtsActivityResult.NotFound;
                if (MtsContactWriteService.Version(activity) != version) return MtsActivityResult.Changed;
                current.TakvimDaveti = ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI; Stamp(current, current);
                Records.Save("FaaliyetKatilim_Table", current, current.Id, transaction);
                transaction.Complete();
            }
            return MtsActivityResult.Completed;
        }
        private MtsActivityResult ValidateParticipant(int activityId, int personId, SqlTransactionContext transaction, out Faaliyet activity)
        {
            activity = First<Faaliyet>(Records.Read("Faaliyet_Table", activityId, transaction));
            if (activity == null) return MtsActivityResult.NotFound;
            if (personId <= 0 || Records.Read("Kisi_Table", personId, transaction) == null
                || repository.Participants(activityId, personId, transaction) == null) return MtsActivityResult.InvalidRelation;
            return MtsActivityResult.Completed;
        }
        private static bool Valid(Faaliyet input)
        {
            return input != null && input.Id >= 0 && !string.IsNullOrWhiteSpace(input.FaaliyetKonusu)
                && input.BaslangicTarihi >= new DateTime(1753, 1, 1) && input.BitisTarihi > input.BaslangicTarihi
                && new[] { ProjeConstants.RANDEVU_ALINAN, ProjeConstants.RANDEVU_VERILEN, ProjeConstants.RANDEVU_DIGER }.Contains(input.FaaliyetTipi)
                && new[] { ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT, ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT,
                    ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT }.Contains(input.FaaliyetDurumu)
                && new[] { ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT, ProjeConstants.FAALIYET_AMACI_ZIYARET_INT,
                    ProjeConstants.FAALIYET_AMACI_GORUSME_INT, ProjeConstants.FAALIYET_AMACI_DAVET_INT, ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT,
                    ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT, ProjeConstants.FAALIYET_AMACI_IZIN_INT, ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT,
                    ProjeConstants.FAALIYET_AMACI_BILGI_INT }.Contains(input.FaaliyetAmaciId.ToString());
        }
        private void Stamp(EntityBase row, EntityBase old)
        {
            row.Olusturan = old == null ? context.UserLogin : old.Olusturan;
            row.OlusturmaTarihi = old == null ? DateTime.Now : old.OlusturmaTarihi;
            row.Degistiren = context.UserLogin; row.DegistirmeTarihi = DateTime.Now;
        }
        private void StampStock(DepoStok stock)
        {
            Stamp(stock, stock.Id > 0 ? stock : null); stock.SonIslemTarihi = DateTime.Now; stock.SonIslemYapan = context.UserLogin;
        }
        private static List<T> List<T>(DataTable rows) where T : new() { return new Kisi().ToList<T>(rows); }
        private static T First<T>(DataTable rows) where T : new() { return List<T>(rows).FirstOrDefault(); }
    }
}
