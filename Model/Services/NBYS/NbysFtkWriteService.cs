using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysFtkWriteResult
    {
        Completed, NotFound, InvalidName, InvalidSurname, InvalidLocation, InvalidDate,
        DuplicateIdentity, DuplicateUpdateDate, NoSnapshotToCorrect, NoActiveMembers, ReferencedPerson, Conflict
    }

    public sealed class NbysFtkWriteService
    {
        private readonly PortalLogContext context;
        private readonly NbysFtkWriteRepository repository;

        public NbysFtkWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new NbysFtkWriteRepository(context);
        }

        public NbysFtkWriteResult SavePerson(FTKKisi input, out int savedId)
        {
            savedId = 0;
            if (input == null) throw new ArgumentNullException("input");
            if (input.Id < 0) return NbysFtkWriteResult.NotFound;
            if (string.IsNullOrWhiteSpace(input.Adi)) return NbysFtkWriteResult.InvalidName;
            if (string.IsNullOrWhiteSpace(input.Soyadi)) return NbysFtkWriteResult.InvalidSurname;
            int id;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockWrites(transaction);
                int region;
                if (!ValidLocation(input.Ili, input.Ilcesi, transaction, out region)) return NbysFtkWriteResult.InvalidLocation;
                FTKKisi current = input.Id > 0 ? First<FTKKisi>(repository.Person(input.Id, transaction)) : new FTKKisi();
                if (current == null) return NbysFtkWriteResult.NotFound;
                if (input.TCKimlikNo > 0 && repository.DuplicateIdentity(input.TCKimlikNo, input.Id, transaction))
                    return NbysFtkWriteResult.DuplicateIdentity;
                foreach (string field in new[] { "Adi", "Soyadi", "TCKimlikNo", "DogumTarihi", "Telefon1", "Telefon2", "Unvani", "Vali", "Kaymakam", "Ili", "Ilcesi", "Adres", "Aciklama", "KartNo", "FTKGorevi", "UyelikDurumu" })
                {
                    var property = typeof(FTKKisi).GetProperty(field);
                    property.SetValue(current, property.GetValue(input, null), null);
                }
                Stamp(current);
                id = repository.Save(current, "FTKKisi_Table", current.Id, transaction);
                transaction.Complete();
            }
            savedId = id;
            return NbysFtkWriteResult.Completed;
        }

        public NbysFtkWriteResult DeletePerson(int id)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockWrites(transaction);
                if (First<FTKKisi>(repository.Person(id, transaction)) == null) return NbysFtkWriteResult.NotFound;
                if (repository.PersonHasHistory(id, transaction)) return NbysFtkWriteResult.ReferencedPerson;
                repository.Delete("FTKKisi_Table", id, transaction);
                transaction.Complete();
            }
            return NbysFtkWriteResult.Completed;
        }

        public NbysFtkWriteResult Deactivate(int province, int district, IEnumerable<int> selection)
        {
            int[] ids = selection == null ? new int[0] : selection.Distinct().OrderBy(id => id).ToArray();
            if (ids.Length == 0) return NbysFtkWriteResult.NoActiveMembers;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockWrites(transaction);
                List<FTKKisi> members = new List<FTKKisi>();
                foreach (int id in ids)
                {
                    FTKKisi member = First<FTKKisi>(repository.Person(id, transaction));
                    if (member == null) return NbysFtkWriteResult.NotFound;
                    if (member.Ili != province || member.Ilcesi != district || member.UyelikDurumu != ProjeConstants.FTK_UYELIK_DURUMU_AKTIF)
                        return NbysFtkWriteResult.Conflict;
                    members.Add(member);
                }
                foreach (FTKKisi member in members)
                {
                    member.UyelikDurumu = ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL;
                    Stamp(member);
                    repository.Save(member, "FTKKisi_Table", member.Id, transaction);
                }
                transaction.Complete();
            }
            return NbysFtkWriteResult.Completed;
        }

        public NbysFtkWriteResult SaveCommittee(FTKIslem input, bool correction, out int savedId)
        {
            savedId = 0;
            if (input == null) throw new ArgumentNullException("input");
            if (input.KurulusTarihi.Year < 1901 || input.GuncellemeTarihi.Year < 1901) return NbysFtkWriteResult.InvalidDate;
            int operationId;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                repository.LockWrites(transaction);
                int region;
                if (!ValidLocation(input.Ili, input.Ilcesi, transaction, out region)) return NbysFtkWriteResult.InvalidLocation;
                List<FTKIslem> operations = Map<FTKIslem>(repository.Operations(input.Ili, input.Ilcesi, transaction));
                if (operations.Count > 1) return NbysFtkWriteResult.Conflict;
                FTKIslem current = operations.FirstOrDefault();
                if (input.Id > 0 && (current == null || current.Id != input.Id)) return NbysFtkWriteResult.Conflict;
                List<FTK> history = Map<FTK>(repository.History(input.Ili, input.Ilcesi, transaction));
                if (!correction && history.Any(row => row.GuncellemeTarihi == input.GuncellemeTarihi))
                    return NbysFtkWriteResult.DuplicateUpdateDate;
                List<FTKKisi> members = Map<FTKKisi>(repository.ActiveMembers(input.Ili, input.Ilcesi, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF, transaction));
                if (members.Count == 0) return NbysFtkWriteResult.NoActiveMembers;
                List<FTK> previous = current == null ? new List<FTK>() : history.Where(row => row.FTKIslemId == current.Id).ToList();
                if (correction && previous.Count == 0) return NbysFtkWriteResult.NoSnapshotToCorrect;
                int counter = correction ? previous.Max(row => row.Sayac) : history.Select(row => row.Sayac).DefaultIfEmpty(0).Max() + 1;
                if (current == null) current = new FTKIslem();
                current.Ili = input.Ili;
                current.Ilcesi = input.Ilcesi;
                current.BolgeId = region;
                current.KurulusTarihi = input.KurulusTarihi;
                current.GuncellemeTarihi = input.GuncellemeTarihi;
                current.Aciklama = input.Aciklama;
                Stamp(current);
                operationId = repository.Save(current, "FTKIslem_Table", current.Id, transaction);
                if (correction)
                    foreach (FTK row in previous.Where(row => row.Sayac == counter)) repository.Delete("FTK_Table", row.Id, transaction);
                foreach (FTKKisi member in members)
                {
                    FTK snapshot = new FTK
                    {
                        FTKIslemId = operationId, Ili = current.Ili, Ilcesi = current.Ilcesi, BolgeId = region,
                        KurulusTarihi = current.KurulusTarihi, GuncellemeTarihi = current.GuncellemeTarihi,
                        FTKGorevi = Role(member.FTKGorevi), Adi = member.Adi, Soyadi = member.Soyadi, Unvani = member.Unvani,
                        Telefon = member.Telefon1 + (!string.IsNullOrEmpty(member.Telefon1) && !string.IsNullOrEmpty(member.Telefon2) ? " / " : "") + member.Telefon2,
                        KartNo = member.KartNo, Sayac = counter, KisiId = member.Id,
                        Aciklama = correction ? DateTime.Now.ToString("HH:mm") + " tarihinde " + context.UserLogin + " tarafından kayıt düzeltmesi yapıldı." : string.Empty
                    };
                    Stamp(snapshot);
                    repository.Save(snapshot, "FTK_Table", 0, transaction);
                }
                transaction.Complete();
            }
            savedId = operationId;
            return NbysFtkWriteResult.Completed;
        }

        private bool ValidLocation(int province, int district, SqlTransactionContext transaction, out int region)
        {
            region = 0;
            DataTable rows = repository.Province(province, transaction);
            if (province <= 0 || rows == null || rows.Rows[0].IsNull("BolgeId")) return false;
            region = Convert.ToInt32(rows.Rows[0]["BolgeId"]);
            if (district == ProjeConstants.VALILIK_INT) return true;
            DataTable districts = repository.District(district, transaction);
            return districts != null && Convert.ToInt32(districts.Rows[0]["IlId"]) == province;
        }

        private void Stamp(EntityBase item)
        {
            if (item.Id == 0) { item.Olusturan = context.UserLogin; item.OlusturmaTarihi = DateTime.Now; }
            else { item.Degistiren = context.UserLogin; item.DegistirmeTarihi = DateTime.Now; }
        }

        private static string Role(int role)
        {
            if (role == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT) return ProjeConstants.FTK_GOREVI_FAHRIBASKAN;
            if (role == ProjeConstants.FTK_GOREVI_BASKAN_INT) return ProjeConstants.FTK_GOREVI_BASKAN;
            if (role == ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT) return ProjeConstants.FTK_GOREVI_GENELSEKRETER;
            return ProjeConstants.FTK_GOREVI_UYE;
        }

        private static List<T> Map<T>(DataTable table) where T : new() { return new FTK().ToList<T>(table); }
        private static T First<T>(DataTable table) where T : class, new() { return Map<T>(table).FirstOrDefault(); }
    }
}
