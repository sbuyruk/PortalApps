using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;

namespace Model.Services.NBYS
{
    public sealed class NbysImportValidationException : Exception
    {
        public NbysImportValidationException(string message, Exception inner) : base(message, inner) { }
    }

    public enum NbysEkstreWriteResult { Completed, NotFound, AlreadyTransferred, InvalidSelection, InvalidDonor }

    public sealed class NbysEkstreWriteService
    {
        private readonly PortalLogContext context;
        private readonly EkstreAktarmaRepository repository;
        private readonly EkstreAktarmaService service;

        public NbysEkstreWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new EkstreAktarmaRepository(context);
            service = new EkstreAktarmaService(repository);
        }

        public NbysEkstreWriteResult Save(EkstreAktarma input)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (input.Id < 0) return NbysEkstreWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                EkstreAktarma current = input.Id > 0 ? service.GetById(input.Id, transaction) : new EkstreAktarma { ElleKayit = true };
                if (current == null) return NbysEkstreWriteResult.NotFound;
                if (current.AktarildiMi) return NbysEkstreWriteResult.AlreadyTransferred;
                if (current.Id > 0 && new NakitBagisHareketService(context).GetByEkstreAktarmaId(current.Id, transaction) != null)
                    return NbysEkstreWriteResult.AlreadyTransferred;
                // Copy only editable form fields onto the locked, current record.
                foreach (string field in new[] { "Adi", "TCKimlikNo", "Telefon1", "Telefon2", "Adres", "Ili", "Ilcesi", "BankaAdi", "Eposta", "PostaKodu", "Tutar", "DovizCinsi", "DovizTutari", "DovizKuru", "KurTarihi", "FisNo", "TuzelKisi", "BelgeIstemiyor", "Aciklama", "BagisTarihi", "IslemTarihi" })
                {
                    var property = typeof(EkstreAktarma).GetProperty(field);
                    property.SetValue(current, property.GetValue(input, null), null);
                }
                if (input.NakitBagisciId > 0)
                {
                    if (new NakitBagisciService(context).GetById(input.NakitBagisciId, transaction) == null)
                        return NbysEkstreWriteResult.InvalidDonor;
                    current.NakitBagisciId = input.NakitBagisciId;
                }
                if (current.Id > 0)
                {
                    if (!service.Update(current, transaction)) throw new InvalidOperationException("Ekstre kaydı güncellenemedi.");
                }
                else if (service.Save(current, transaction) <= 0) throw new InvalidOperationException("Ekstre kaydı oluşturulamadı.");
                transaction.Complete();
            }
            return NbysEkstreWriteResult.Completed;
        }

        public NbysEkstreWriteResult Match(int id, int donorId)
        {
            if (id <= 0 || donorId <= 0) return NbysEkstreWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                EkstreAktarma current = service.GetById(id, transaction);
                if (current == null) return NbysEkstreWriteResult.NotFound;
                if (current.AktarildiMi) return NbysEkstreWriteResult.AlreadyTransferred;
                if (new NakitBagisHareketService(context).GetByEkstreAktarmaId(id, transaction) != null)
                    return NbysEkstreWriteResult.AlreadyTransferred;
                if (new NakitBagisciService(context).GetById(donorId, transaction) == null) return NbysEkstreWriteResult.InvalidDonor;
                current.NakitBagisciId = donorId;
                if (!service.Update(current, transaction)) throw new InvalidOperationException("Ekstre bağışçıyla eşleştirilemedi.");
                transaction.Complete();
            }
            return NbysEkstreWriteResult.Completed;
        }

        public NbysEkstreWriteResult Delete(IEnumerable<int> selection)
        {
            int[] ids = selection == null ? new int[0] : selection.Distinct().OrderBy(id => id).ToArray();
            if (ids.Length == 0 || ids.Any(id => id <= 0)) return NbysEkstreWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                List<EkstreAktarma> records = new List<EkstreAktarma>();
                foreach (int id in ids)
                {
                    EkstreAktarma current = service.GetById(id, transaction);
                    if (current == null) return NbysEkstreWriteResult.NotFound;
                    if (current.AktarildiMi || current.NakitBagisHareketId > 0) return NbysEkstreWriteResult.AlreadyTransferred;
                    if (new NakitBagisHareketService(context).GetByEkstreAktarmaId(id, transaction) != null)
                        return NbysEkstreWriteResult.AlreadyTransferred;
                    records.Add(current);
                }
                NbysBagisciWriteRepository archive = new NbysBagisciWriteRepository(context);
                foreach (EkstreAktarma current in records)
                {
                    archive.SaveArchive(new SilinenKayit
                    {
                        TabloAdi = "EkstreAktarma_Table", SilinenKayitBilgisi = "#EkstreAktarmaId=" + current.Id + " #Adi=" + current.Adi,
                        SilinmeSebebi = "Seçilen ekstre kaydı silindi.", Silen = context.UserLogin, SilinmeTarihi = DateTime.Now,
                        Olusturan = context.UserLogin, OlusturmaTarihi = DateTime.Now
                    }, transaction);
                    if (!repository.Delete(current.Id, transaction)) throw new InvalidOperationException("Ekstre kaydı silinemedi.");
                }
                transaction.Complete();
            }
            return NbysEkstreWriteResult.Completed;
        }
    }
}
