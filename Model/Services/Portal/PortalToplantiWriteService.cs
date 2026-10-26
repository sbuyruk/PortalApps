using DAO.Ortak;
using DAO.Repositories.Portal;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    public enum PortalToplantiResult { Completed, NotFound, InvalidInput, Forbidden, Changed, Overlap, Duplicate, InUse, Unchanged }

    public sealed class PortalToplantiChange
    {
        public Toplanti Before { get; set; }
        public Toplanti After { get; set; }
        public List<ToplantiKatilim> PreviousParticipants { get; set; }
        public List<ToplantiKatilim> Participants { get; set; }
        public string Version { get; set; }
        public bool CanEdit { get; set; }
    }

    public sealed class PortalToplantiWriteService
    {
        private readonly PortalLogContext context;
        private readonly PortalToplantiWriteRepository repository;
        public PortalToplantiWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new PortalToplantiWriteRepository(context);
        }

        public PortalToplantiChange Load(int id)
        {
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                Toplanti current = First<Toplanti>(repository.Read("Toplanti_Table", id, transaction));
                if (current == null) return null;
                var participants = List<ToplantiKatilim>(repository.Participants(id, transaction));
                var result = new PortalToplantiChange { After = current, Participants = participants,
                    Version = Version(current, participants), CanEdit = CanEdit(Actor(transaction), current, transaction) };
                transaction.Complete();
                return result;
            }
        }

        public bool CanCreate()
        {
            return GetCurrentPersonId().HasValue;
        }

        // Listing hint only; Save/Delete recheck the persisted meeting and caller in their own transaction.
        public bool CanManageMeetingsOf(int ownerId)
        {
            if (ownerId < 0) return false;
            using (var transaction = new SqlTransactionContext())
            {
                bool allowed = CanEdit(Actor(transaction), new Toplanti { ToplantiYetkilisi = ownerId }, transaction);
                transaction.Complete();
                return allowed;
            }
        }

        public int? GetCurrentPersonId()
        {
            using (var transaction = new SqlTransactionContext())
            {
                DataRow actor = string.IsNullOrWhiteSpace(context.UserLogin) ? null : Actor(transaction);
                int? personId = actor == null ? (int?)null : Convert.ToInt32(actor["Id"]);
                transaction.Complete();
                return personId;
            }
        }

        public PortalToplantiResult Save(Toplanti input, IEnumerable<int> selected, IEnumerable<int> informed,
            string expectedVersion, out PortalToplantiChange change)
        {
            change = null;
            int[] people = (selected ?? Enumerable.Empty<int>()).Distinct().ToArray();
            int[] information = (informed ?? Enumerable.Empty<int>()).Distinct().ToArray();
            if (input == null || input.Id < 0 || string.IsNullOrWhiteSpace(input.ToplantiKonusu)
                || input.ToplantiYeri <= 0 || input.BaslangicTarihi < new DateTime(1753, 1, 1)
                || input.BitisTarihi <= input.BaslangicTarihi || people.Any(id => id <= 0)
                || information.Except(people).Any()) return PortalToplantiResult.InvalidInput;
            PortalToplantiChange completed;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                Toplanti current = input.Id > 0 ? First<Toplanti>(repository.Read("Toplanti_Table", input.Id, transaction)) : null;
                if (input.Id > 0 && current == null) return PortalToplantiResult.NotFound;
                DataRow actor = Actor(transaction);
                if (!CanEdit(actor, current, transaction)) return PortalToplantiResult.Forbidden;
                var previous = current == null ? new List<ToplantiKatilim>() : List<ToplantiKatilim>(repository.Participants(current.Id, transaction));
                if (current != null && Version(current, previous) != expectedVersion) return PortalToplantiResult.Changed;
                var place = First<ToplantiParametre>(repository.Read("ToplantiParametre_Table", input.ToplantiYeri, transaction));
                if (place == null || place.Grup != ProjeConstants.PARAM_TOPLANTIYERI
                    || (input.ToplantiYeri == ProjeConstants.PARAM_DIGER_INT && string.IsNullOrWhiteSpace(input.ToplantiYeriDiger)))
                    return PortalToplantiResult.InvalidInput;
                if (repository.Overlaps(input.Id, input.ToplantiYeri, input.BaslangicTarihi, input.BitisTarihi, transaction))
                    return PortalToplantiResult.Overlap;
                foreach (int person in people)
                    if (!repository.PersonExists(person, transaction)) return PortalToplantiResult.InvalidInput;
                input.ToplantiYetkilisi = current == null ? Convert.ToInt32(actor["Id"]) : current.ToplantiYetkilisi;
                input.Koordinator = current == null ? (actor.IsNull("BirimId") ? 0 : Convert.ToInt32(actor["BirimId"])) : current.Koordinator;
                input.UniqueId = current == null ? Guid.NewGuid() : current.UniqueId;
                if (current != null && SameContent(current, input) && previous.Count == people.Length
                    && previous.All(p => people.Contains(p.KatilimciId) && p.Bilgi == information.Contains(p.KatilimciId))
                    && previous.Select(p => p.KatilimciId).Distinct().Count() == people.Length)
                    return PortalToplantiResult.Unchanged;
                Stamp(input, current);
                input.Id = repository.Save("Toplanti_Table", input, input.Id, transaction);
                foreach (var removed in previous.Where(p => !people.Contains(p.KatilimciId)))
                    repository.Delete("ToplantiKatilim_Table", removed.Id, transaction);
                foreach (int person in people)
                {
                    var matches = previous.Where(p => p.KatilimciId == person).OrderBy(p => p.Id).ToList();
                    var old = matches.FirstOrDefault();
                    // Consolidate legacy duplicates while keeping one authoritative participant row.
                    foreach (var duplicate in matches.Skip(1)) repository.Delete("ToplantiKatilim_Table", duplicate.Id, transaction);
                    bool info = information.Contains(person);
                    if (old != null && old.Bilgi == info) continue;
                    var participant = new ToplantiKatilim { Id = old == null ? 0 : old.Id, ToplantiId = input.Id,
                        KatilimciId = person, Bilgi = info, Aciklama = old == null ? null : old.Aciklama };
                    Stamp(participant, old);
                    repository.Save("ToplantiKatilim_Table", participant, participant.Id, transaction);
                }
                Toplanti saved = First<Toplanti>(repository.Read("Toplanti_Table", input.Id, transaction));
                var participants = List<ToplantiKatilim>(repository.Participants(input.Id, transaction));
                completed = new PortalToplantiChange { Before = current, After = saved, PreviousParticipants = previous,
                    Participants = participants, Version = Version(saved, participants) };
                transaction.Complete();
            }
            change = completed;
            return PortalToplantiResult.Completed;
        }

        public PortalToplantiResult Delete(int id, string expectedVersion, out PortalToplantiChange change)
        {
            change = null;
            PortalToplantiChange completed;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var current = First<Toplanti>(repository.Read("Toplanti_Table", id, transaction));
                if (current == null) return PortalToplantiResult.NotFound;
                if (!CanEdit(Actor(transaction), current, transaction)) return PortalToplantiResult.Forbidden;
                var previous = List<ToplantiKatilim>(repository.Participants(id, transaction));
                if (Version(current, previous) != expectedVersion) return PortalToplantiResult.Changed;
                foreach (var participant in previous) repository.Delete("ToplantiKatilim_Table", participant.Id, transaction);
                repository.Delete("Toplanti_Table", id, transaction);
                completed = new PortalToplantiChange { Before = current, PreviousParticipants = previous, Participants = new List<ToplantiKatilim>() };
                transaction.Complete();
            }
            change = completed;
            return PortalToplantiResult.Completed;
        }

        public PortalToplantiResult SaveParameter(ToplantiParametre input, bool delete = false)
        {
            if (input == null || input.Id < 0) return PortalToplantiResult.InvalidInput;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                // Page access remains governed by the SharePoint page permissions.
                if (string.IsNullOrWhiteSpace(context.UserLogin)) return PortalToplantiResult.Forbidden;
                var current = input.Id > 0 ? First<ToplantiParametre>(repository.Read("ToplantiParametre_Table", input.Id, transaction)) : null;
                if (input.Id > 0 && current == null) return PortalToplantiResult.NotFound;
                if (delete)
                {
                    if (current == null) return PortalToplantiResult.NotFound;
                    if (current.Id == ProjeConstants.PARAM_DIGER_INT || repository.ParameterInUse(current.Id, transaction))
                        return PortalToplantiResult.InUse;
                    repository.Delete("ToplantiParametre_Table", current.Id, transaction);
                }
                else
                {
                    input.Grup = current == null ? input.Grup : current.Grup;
                    input.Deger = (input.Deger ?? string.Empty).Trim();
                    if (input.Deger.Length == 0 || !new[] { ProjeConstants.PARAM_TOPLANTIYERI,
                        ProjeConstants.PARAM_TOPLANTIYETKILISI, ProjeConstants.PARAM_TOPLANTIYONETICISI }.Contains(input.Grup))
                        return PortalToplantiResult.InvalidInput;
                    if (repository.DuplicateParameter(input.Id, input.Grup, input.Deger, transaction)) return PortalToplantiResult.Duplicate;
                    if (current != null && current.Id == ProjeConstants.PARAM_DIGER_INT && input.Deger != current.Deger)
                        return PortalToplantiResult.InUse;
                    Stamp(input, current);
                    input.Id = repository.Save("ToplantiParametre_Table", input, input.Id, transaction);
                }
                transaction.Complete();
            }
            return PortalToplantiResult.Completed;
        }

        private DataRow Actor(SqlTransactionContext transaction)
        {
            DataTable rows = repository.CurrentPerson(transaction);
            return rows == null || rows.Rows.Count != 1 ? null : rows.Rows[0];
        }
        private bool CanEdit(DataRow actor, Toplanti current, SqlTransactionContext transaction)
        {
            if (actor == null || string.IsNullOrWhiteSpace(context.UserLogin)) return false;
            // Employees can manage their own meetings and those owned by their current unit.
            if (current == null || current.ToplantiYetkilisi == Convert.ToInt32(actor["Id"])) return true;
            if (!actor.IsNull("BirimId")
                && repository.SameUnit(current.ToplantiYetkilisi, Convert.ToInt32(actor["BirimId"]), transaction)) return true;
            string name = Convert.ToString(actor["Adi"]) + " " + Convert.ToString(actor["Soyadi"]);
            return repository.HasRole(ProjeConstants.PARAM_TOPLANTIYONETICISI, name, transaction);
        }
        private void Stamp(EntityBase item, EntityBase current)
        {
            item.Olusturan = current == null ? context.UserLogin : current.Olusturan;
            item.OlusturmaTarihi = current == null ? DateTime.Now : current.OlusturmaTarihi;
            item.Degistiren = context.UserLogin;
            item.DegistirmeTarihi = DateTime.Now;
        }
        private static string Version(Toplanti item, List<ToplantiKatilim> participants)
        {
            string json = new JavaScriptSerializer().Serialize(new { Meeting = item, Participants = participants.OrderBy(p => p.Id).ToArray() });
            using (SHA256 hash = SHA256.Create()) return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(json)));
        }
        private static bool SameContent(Toplanti a, Toplanti b)
        {
            return a.ToplantiKonusu == b.ToplantiKonusu && a.ToplantiYeri == b.ToplantiYeri
                && (a.ToplantiYeriDiger ?? "") == (b.ToplantiYeriDiger ?? "")
                && a.BaslangicTarihi == b.BaslangicTarihi && a.BitisTarihi == b.BitisTarihi
                && a.BaslangicSaati == b.BaslangicSaati && a.BitisSaati == b.BitisSaati
                && a.CevrimIci == b.CevrimIci && a.IkramOnayi == b.IkramOnayi
                && (a.IkramMalzemesi ?? "") == (b.IkramMalzemesi ?? "")
                && (a.DisKatilimcilar ?? "") == (b.DisKatilimcilar ?? "")
                && (a.Aciklama ?? "") == (b.Aciklama ?? "");
        }
        private static List<T> List<T>(DataTable rows) where T : new() { return new Toplanti().ToList<T>(rows); }
        private static T First<T>(DataTable rows) where T : new() { return List<T>(rows).FirstOrDefault(); }
    }
}
