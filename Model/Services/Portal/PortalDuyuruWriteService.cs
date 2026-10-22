using DAO.Ortak;
using DAO.Repositories.Portal;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.Portal
{
    public enum PortalDuyuruResult { Completed, NotFound, InvalidInput, Forbidden, Changed, Unchanged }

    public sealed class PortalDuyuruWriteService
    {
        private readonly PortalDuyuruWriteRepository repository;
        private readonly PortalLogContext context;
        public PortalDuyuruWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new PortalDuyuruWriteRepository(context);
        }

        public PortalDuyuruResult Save(Duyuru input, bool replaceImage, out int savedId)
        {
            savedId = 0;
            if (!Valid(input)) return PortalDuyuruResult.InvalidInput;
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return PortalDuyuruResult.Forbidden;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Duyuru current = null;
                if (input.Id > 0)
                {
                    repository.LockAnnouncement(input.Id, transaction);
                    current = First<Duyuru>(repository.Read("Duyuru_Table", input.Id, transaction));
                    if (current == null) return PortalDuyuruResult.NotFound;
                }
                input.Resim = replaceImage ? input.Resim : current == null ? null : current.Resim;
                input.Aciklama = current == null ? input.Aciklama : current.Aciklama;
                input.Olusturan = current == null ? context.UserLogin : current.Olusturan;
                input.OlusturmaTarihi = current == null ? DateTime.Now : current.OlusturmaTarihi;
                input.Degistiren = context.UserLogin;
                input.DegistirmeTarihi = DateTime.Now;
                input.Id = repository.Save("Duyuru_Table", input, input.Id, transaction);
                SaveVersion(input, transaction);
                transaction.Complete();
            }
            savedId = input.Id;
            return PortalDuyuruResult.Completed;
        }

        public PortalDuyuruResult Delete(int id)
        {
            if (id <= 0) return PortalDuyuruResult.NotFound;
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return PortalDuyuruResult.Forbidden;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockAnnouncement(id, transaction);
                if (repository.Read("Duyuru_Table", id, transaction) == null) return PortalDuyuruResult.NotFound;
                repository.Delete(id, transaction);
                transaction.Complete();
            }
            return PortalDuyuruResult.Completed;
        }

        public PortalDuyuruResult ClearImage(int id)
        {
            if (id <= 0) return PortalDuyuruResult.NotFound;
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return PortalDuyuruResult.Forbidden;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockAnnouncement(id, transaction);
                Duyuru current = First<Duyuru>(repository.Read("Duyuru_Table", id, transaction));
                if (current == null) return PortalDuyuruResult.NotFound;
                if (string.IsNullOrEmpty(current.Resim)) return PortalDuyuruResult.Unchanged;
                current.Resim = null;
                current.Degistiren = context.UserLogin;
                current.DegistirmeTarihi = DateTime.Now;
                repository.Save("Duyuru_Table", current, id, transaction);
                SaveVersion(current, transaction);
                transaction.Complete();
            }
            return PortalDuyuruResult.Completed;
        }

        public PortalDuyuruResult MarkRead(IEnumerable<int> selection)
        {
            int[] ids = selection == null ? new int[0] : selection.Distinct().OrderBy(id => id).ToArray();
            if (ids.Length == 0 || ids.Length > 200 || ids.Any(id => id <= 0)) return PortalDuyuruResult.InvalidInput;
            if (string.IsNullOrWhiteSpace(context.UserLogin)) return PortalDuyuruResult.Forbidden;
            bool changed = false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                DataTable person = repository.CurrentPerson(context.UserLogin, transaction);
                if (person == null) return PortalDuyuruResult.Forbidden;
                int personId = Convert.ToInt32(person.Rows[0]["Id"]);
                // Lock announcements in a stable order before locking their version rows.
                var versions = new List<DuyuruGosterim>();
                foreach (int id in ids)
                {
                    // This first lookup stays in the same transaction; authoritative re-read follows the application lock.
                    DuyuruGosterim version = First<DuyuruGosterim>(repository.VersionOwner(id, transaction));
                    if (version == null) return PortalDuyuruResult.NotFound;
                    versions.Add(version);
                }
                foreach (int announcementId in versions.Select(v => v.DuyuruId).Distinct().OrderBy(id => id))
                    repository.LockAnnouncement(announcementId, transaction);
                foreach (DuyuruGosterim requested in versions)
                {
                    Duyuru announcement = First<Duyuru>(repository.Read("Duyuru_Table", requested.DuyuruId, transaction));
                    DuyuruGosterim version = First<DuyuruGosterim>(repository.LatestVersion(requested.DuyuruId, transaction));
                    if (announcement == null || version == null) return PortalDuyuruResult.NotFound;
                    if (version.Id != requested.Id) return PortalDuyuruResult.Changed;
                    if (!announcement.Aktif || !IsRecipient(version.DuyuruAlicilari, personId)
                        || !IsRecipient(announcement.DuyuruAlicilari, personId)) return PortalDuyuruResult.Forbidden;
                    if (repository.AlreadyRead(version.Id, personId, transaction)) continue;
                    var read = new DuyuruOkuma { DuyuruId = version.DuyuruId, DuyuruGosterimId = version.Id,
                        PersonelId = personId, OkumaTarihi = DateTime.Now, OlusturmaTarihi = DateTime.Now, Olusturan = context.UserLogin };
                    repository.Save("DuyuruOkuma_Table", read, 0, transaction);
                    changed = true;
                }
                transaction.Complete();
            }
            return changed ? PortalDuyuruResult.Completed : PortalDuyuruResult.Unchanged;
        }

        public static bool IsRecipient(string recipients, int personId)
        {
            return personId > 0 && (recipients ?? string.Empty).Split(',').Contains(personId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        private static bool Valid(Duyuru input)
        {
            return input != null && input.Id >= 0 && !string.IsNullOrWhiteSpace(input.Baslik)
                && !string.IsNullOrWhiteSpace(input.Metin) && !string.IsNullOrWhiteSpace(input.DuyuruAlicilari)
                && input.YayinBasTar >= new DateTime(1753, 1, 1) && input.YayinBitTar >= input.YayinBasTar
                && new[] { ProjeConstants.DUYURU_TEKRAR_YOK, ProjeConstants.DUYURU_TEKRARLA_HAFTA,
                    ProjeConstants.DUYURU_TEKRARLA_AY, ProjeConstants.DUYURU_TEKRARLA_YIL }.Contains(input.Tekrar);
        }

        private void SaveVersion(Duyuru item, SqlTransactionContext transaction)
        {
            var version = new DuyuruGosterim { DuyuruId = item.Id, GosterildigiTarih = DateTime.Now,
                Baslik = item.Baslik, Metin = item.Metin, YayinBasTar = item.YayinBasTar, YayinBitTar = item.YayinBitTar,
                Tekrar = item.Tekrar, DuyuruAlicilari = item.DuyuruAlicilari, Resim = item.Resim, Aciklama = item.Aciklama,
                Aktif = item.Aktif, Popup = item.Popup, Olusturan = context.UserLogin, OlusturmaTarihi = DateTime.Now };
            repository.Save("DuyuruGosterim_Table", version, 0, transaction);
        }

        private static T First<T>(DataTable rows) where T : new()
        {
            return new Duyuru().ToList<T>(rows).FirstOrDefault();
        }
    }
}
