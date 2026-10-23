using DAO.Ortak;
using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public enum MtsContactResult { Completed, NotFound, InvalidInput, Duplicate, InUse, Changed, Restricted }

    public sealed class MtsContactWriteService
    {
        private readonly PortalLogContext context;
        private readonly MtsContactWriteRepository repository;
        public MtsContactWriteService(PortalLogContext context)
        {
            if (context == null || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("MTS işlemi için kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
            repository = new MtsContactWriteRepository(context);
        }
        public static string Version<T>(T item)
        {
            using (SHA256 hash = SHA256.Create())
                return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(item))));
        }
        public MtsContactResult SavePerson(Kisi input, string expectedVersion, out int savedId)
        {
            savedId = 0;
            if (input == null || input.Id < 0 || string.IsNullOrWhiteSpace(input.Adi) || string.IsNullOrWhiteSpace(input.Soyadi)
                || input.TCKimlikNo < 0 || (input.DogumTarihi != DateTime.MinValue
                    && (input.DogumTarihi < new DateTime(1753, 1, 1) || input.DogumTarihi.Date > DateTime.Today)))
                return MtsContactResult.InvalidInput;
            int id;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                Kisi current = input.Id > 0 ? First<Kisi>(repository.Read("Kisi_Table", input.Id, transaction)) : null;
                if (input.Id > 0 && current == null) return MtsContactResult.NotFound;
                if (current != null && Version(current) != expectedVersion) return MtsContactResult.Changed;
                if (repository.DuplicateIdentity(input.Id, input.TCKimlikNo, transaction)) return MtsContactResult.Duplicate;
                if (!repository.ValidPersonReferences(input.MTSUnvanTanimId, input.Ili, input.Ilcesi, transaction)) return MtsContactResult.InvalidInput;
                input.Adi = input.Adi.Trim(); input.Soyadi = input.Soyadi.Trim();
                if (current != null) input.EPosta = current.EPosta;
                Stamp(input, current);
                id = repository.Save("Kisi_Table", input, input.Id, transaction);
                transaction.Complete();
            }
            savedId = id;
            return MtsContactResult.Completed;
        }
        public MtsContactResult DeletePerson(int id, string expectedVersion)
        {
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                Kisi current = First<Kisi>(repository.Read("Kisi_Table", id, transaction));
                if (current == null) return MtsContactResult.NotFound;
                if (Version(current) != expectedVersion) return MtsContactResult.Changed;
                if (repository.PersonInUse(id, transaction)) return MtsContactResult.InUse;
                repository.Delete("Kisi_Table", id, transaction);
                transaction.Complete();
            }
            return MtsContactResult.Completed;
        }
        public MtsContactResult SaveCall(AramaGorusme input, string expectedVersion, out int savedId)
        {
            savedId = 0;
            if (input == null || input.Id < 0 || string.IsNullOrWhiteSpace(input.Konu) || input.Tarih < new DateTime(1753, 1, 1)
                || !new[] { ProjeConstants.ARAMAGORUSME_GELENTELEFON, ProjeConstants.ARAMAGORUSME_GIDENTELEFON,
                    ProjeConstants.ARAMAGORUSME_YUZYUZEGORUSME, ProjeConstants.ARAMAGORUSME_YONETICIDIREKTIFI }.Contains(input.GorusmeSekli))
                return MtsContactResult.InvalidInput;
            int id;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var current = input.Id > 0 ? First<AramaGorusme>(repository.Read("AramaGorusme_Table", input.Id, transaction)) : null;
                if (input.Id > 0 && current == null) return MtsContactResult.NotFound;
                if (current != null && Version(current) != expectedVersion) return MtsContactResult.Changed;
                var person = First<Kisi>(repository.Read("Kisi_Table", input.ArayanId, transaction));
                if (person == null) return MtsContactResult.NotFound;
                if (current != null && current.ArayanId != input.ArayanId) return MtsContactResult.InvalidInput;
                // The meeting relation is changed only by the dedicated atomic creation path.
                input.FaaliyetId = current == null ? 0 : current.FaaliyetId;
                if (input.FaaliyetId > 0 && repository.Read("Faaliyet_Table", input.FaaliyetId, transaction) == null)
                    return MtsContactResult.Changed;
                if (person.RandevuKisiti && input.RandevuIstendi && input.FaaliyetId <= 0) return MtsContactResult.Restricted;
                if (input.FaaliyetId > 0) input.RandevuIstendi = true;
                input.Konu = input.Konu.Trim(); Stamp(input, current);
                id = repository.Save("AramaGorusme_Table", input, input.Id, transaction);
                transaction.Complete();
            }
            savedId = id;
            return MtsContactResult.Completed;
        }
        public MtsContactResult DeleteCall(int id, string expectedVersion)
        {
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var current = First<AramaGorusme>(repository.Read("AramaGorusme_Table", id, transaction));
                if (current == null) return MtsContactResult.NotFound;
                if (Version(current) != expectedVersion) return MtsContactResult.Changed;
                repository.Delete("AramaGorusme_Table", id, transaction);
                transaction.Complete();
            }
            return MtsContactResult.Completed;
        }
        public MtsContactResult CreateActivity(int personId, int callId, string callVersion,
            bool participant, bool contact, string note, out int activityId)
        {
            activityId = 0;
            int completedId;
            using (var transaction = new SqlTransactionContext())
            {
                repository.Lock(transaction);
                var call = callId > 0 ? First<AramaGorusme>(repository.Read("AramaGorusme_Table", callId, transaction)) : null;
                if (callId > 0 && call == null) return MtsContactResult.NotFound;
                if (call != null)
                {
                    personId = call.ArayanId;
                    // A repeated confirmation navigates to the existing activity; no second insert.
                    if (call.FaaliyetId > 0)
                    {
                        if (repository.Read("Faaliyet_Table", call.FaaliyetId, transaction) == null) return MtsContactResult.Changed;
                        activityId = call.FaaliyetId;
                        transaction.Complete();
                        return MtsContactResult.Completed;
                    }
                    if (Version(call) != callVersion) return MtsContactResult.Changed;
                }
                var person = First<Kisi>(repository.Read("Kisi_Table", personId, transaction));
                if (person == null) return MtsContactResult.NotFound;
                if (person.RandevuKisiti) return MtsContactResult.Restricted;
                var activity = new Faaliyet {
                    UniqueId = Guid.NewGuid(), FaaliyetAmaciId = Convert.ToInt32(ProjeConstants.FAALIYET_AMACI_ZIYARET_INT),
                    FaaliyetKonusu = call == null ? string.Empty : call.Konu, FaaliyetDurumu = ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT,
                    AcikTarih = call == null, TumGun = false, BaslangicTarihi = DateTime.Today.AddDays(1).AddHours(10),
                    BitisTarihi = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30), BaslangicSaati = "10:00", BitisSaati = "10:30",
                    Aciklama = call == null ? note : call.Aciklama, DisIrtibatId = contact || call != null ? personId : 0,
                    FaaliyetTipi = call == null ? null : call.GorusmeSekli == ProjeConstants.ARAMAGORUSME_GIDENTELEFON ? ProjeConstants.RANDEVU_ALINAN : ProjeConstants.RANDEVU_VERILEN };
                Stamp(activity, null);
                completedId = repository.Save("Faaliyet_Table", activity, 0, transaction);
                if (participant)
                {
                    var participation = new FaaliyetKatilim { FaaliyetId = completedId, KatilimciId = personId,
                        KurumGorev = repository.Organization(personId, transaction) ?? person.Kurumu + " / " + person.Gorevi };
                    Stamp(participation, null);
                    repository.Save("FaaliyetKatilim_Table", participation, 0, transaction);
                }
                if (call != null)
                {
                    call.FaaliyetId = completedId; call.RandevuIstendi = true; Stamp(call, call);
                    repository.Save("AramaGorusme_Table", call, call.Id, transaction);
                }
                transaction.Complete();
            }
            activityId = completedId;
            return MtsContactResult.Completed;
        }
        private void Stamp(EntityBase input, EntityBase current)
        {
            input.Olusturan = current == null ? context.UserLogin : current.Olusturan;
            input.OlusturmaTarihi = current == null ? DateTime.Now : current.OlusturmaTarihi;
            input.Degistiren = context.UserLogin; input.DegistirmeTarihi = DateTime.Now;
        }
        private static T First<T>(DataTable rows) where T : new()
        {
            return new Kisi().ToList<T>(rows).FirstOrDefault();
        }
    }
}
