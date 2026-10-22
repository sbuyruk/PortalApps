using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysRegularWriteResult { Completed, ExistingCertificate, AlreadyMatched, NotFound, InvalidSelection, Inactive, NoCertificateWanted, NotEligible, NoDonation, Conflict }

    public sealed class NbysRegularImportResult
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Unmatched { get; set; }
        public int Skipped { get; set; }
        public bool InvalidWorkbook { get; set; }
        public List<int> InvalidRows { get; private set; }
        public List<Exception> Errors { get; private set; }
        public NbysRegularImportResult() { InvalidRows = new List<int>(); Errors = new List<Exception>(); }
    }

    public sealed class NbysRegularWriteService
    {
        private readonly PortalLogContext context;
        private readonly NbysRegularWriteRepository repository;
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo(ProjeConstants.CULTUREINFO);
        public NbysRegularWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new NbysRegularWriteRepository(context);
        }

        public NbysRegularImportResult Import(Stream stream)
        {
            NbysRegularImportResult result = new NbysRegularImportResult();
            DataTable rows;
            try { rows = ExcelHelper.ReadXLSXAsTypedDataTable(stream, ProjeConstants.DUZENLIBAGISCI_ILKKACSATIRHARIC, ProjeConstants.DUZENLIBAGISCI_SONKACSATIRHARIC, true); }
            catch (InvalidDataException error)
            {
                PortalTechnicalLog.WriteError(context, "NBYS.RegularImport.FileValidation", error);
                result.InvalidWorkbook = true; return result;
            }
            catch (FormatException error)
            {
                PortalTechnicalLog.WriteError(context, "NBYS.RegularImport.FileValidation", error);
                result.InvalidWorkbook = true; return result;
            }
            if (rows == null || rows.Columns.Count < 10) { result.InvalidWorkbook = true; return result; }
            for (int index = 0; index < rows.Rows.Count; index++)
            {
                try
                {
                    DataRow row = rows.Rows[index];
                    decimal amount;
                    if (!Money(row[6], out amount)) { result.InvalidRows.Add(index + 1); continue; }
                    if (amount < 1) { result.Skipped++; continue; }
                    decimal total, countValue, identityValue;
                    DateTime start;
                    if (!Money(row[8], out total) || !Money(row[7], out countValue) || countValue < 0 || countValue > int.MaxValue || decimal.Truncate(countValue) != countValue
                        || !Money(row[4], out identityValue) || identityValue < 0 || identityValue > long.MaxValue || decimal.Truncate(identityValue) != identityValue
                        || !StartDate(row[9], object.Equals(rows.ExtendedProperties["Excel.Date1904"], true), out start))
                    { result.InvalidRows.Add(index + 1); continue; }
                    DuzenliNakitBagisci input = new DuzenliNakitBagisci
                    {
                        Aciklama = Convert.ToString(row[0]).Replace("'", ""), BagisciAdi = Convert.ToString(row[1]).Trim(),
                        EPosta = Convert.ToString(row[2]).Trim(), Telefon = Convert.ToString(row[3], CultureInfo.InvariantCulture).Trim(), TCKimlikNo = (long)identityValue,
                        Tutar = amount, BagisAdedi = (int)countValue, BagisToplami = total, BaslamaTarihi = start, Aktif = true
                    };
                    bool created, unmatched;
                    NbysRegularWriteResult outcome = ImportRow(input, out created, out unmatched);
                    if (outcome != NbysRegularWriteResult.Completed) { result.InvalidRows.Add(index + 1); continue; }
                    if (created) result.Created++; else result.Updated++;
                    if (unmatched) result.Unmatched++;
                }
                catch (OverflowException) { result.InvalidRows.Add(index + 1); }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error) { result.Errors.Add(new InvalidOperationException("Düzenli bağışçı veri satırı işlenemedi. Satır: " + (index + 1), error)); }
            }
            return result;
        }

        private NbysRegularWriteResult ImportRow(DuzenliNakitBagisci input, out bool created, out bool unmatched)
        {
            created = false; unmatched = false;
            bool insert, missing;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                string match;
                NakitBagisci donor = FindDonor(input, transaction, out match);
                int donorId = donor == null ? -1 : donor.Id;
                List<DuzenliNakitBagisci> active = donor == null ? new List<DuzenliNakitBagisci>() : Map<DuzenliNakitBagisci>(repository.ActiveForDonor(donorId, transaction));
                if (active.Count > 1) return NbysRegularWriteResult.Conflict;
                DuzenliNakitBagisci current = active.FirstOrDefault() ?? new DuzenliNakitBagisci();
                insert = current.Id == 0; missing = donor == null;
                current.BagisciId = donorId; current.TCKimlikNo = input.TCKimlikNo; current.BagisciAdi = input.BagisciAdi;
                current.Tutar = input.Tutar; current.BaslamaTarihi = input.BaslamaTarihi; current.Aktif = true;
                current.Telefon = input.Telefon; current.EPosta = input.EPosta; current.EslesmeBilgisi = match; current.Aciklama = input.Aciklama;
                current.BagisAdedi = input.BagisAdedi; current.BagisToplami = input.BagisToplami;
                Stamp(current);
                repository.Save(current, "DuzenliNakitBagisci_Table", current.Id, transaction);
                transaction.Complete();
            }
            created = insert; unmatched = missing;
            return NbysRegularWriteResult.Completed;
        }

        private NakitBagisci FindDonor(DuzenliNakitBagisci input, SqlTransactionContext transaction, out string match)
        {
            foreach (string kind in new[] { "Identity", "NamePhone", "Phone", "Email" })
            {
                if (kind == "Identity" && input.TCKimlikNo <= 0) continue;
                if ((kind == "NamePhone" || kind == "Phone") && string.IsNullOrWhiteSpace(input.Telefon)) continue;
                if (kind == "NamePhone" && string.IsNullOrWhiteSpace(input.BagisciAdi)) continue;
                if (kind == "Email" && string.IsNullOrWhiteSpace(input.EPosta)) continue;
                List<NakitBagisci> candidates = Map<NakitBagisci>(repository.Candidates(kind, input.TCKimlikNo, input.BagisciAdi, input.Telefon, input.EPosta, transaction));
                if (candidates.Count == 0) continue;
                if (candidates.Count > 1) { match = "Birden fazla bağışçı bulundu; elle eşleştirme gerekli."; return null; }
                match = kind == "Identity" ? "TCKN ile bulundu" : kind == "NamePhone" ? "Ad ve telefondan bulundu" : kind == "Phone" ? "Telefondan bulundu" : "E-postadan bulundu";
                return candidates[0];
            }
            match = "Bağışçı bulunamadı; elle eşleştirme gerekli.";
            return null;
        }

        public NbysRegularWriteResult Match(int regularId, int donorId)
        {
            if (regularId <= 0 || donorId <= 0) return NbysRegularWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (First<NakitBagisci>(repository.Donor(donorId, transaction)) == null) return NbysRegularWriteResult.NotFound;
                DuzenliNakitBagisci regular = First<DuzenliNakitBagisci>(repository.Regular(regularId, transaction));
                if (regular == null) return NbysRegularWriteResult.NotFound;
                if (!regular.Aktif) return NbysRegularWriteResult.Inactive;
                if (regular.BagisciId == donorId) return NbysRegularWriteResult.AlreadyMatched;
                if (regular.ArmaganId > 0 || regular.NakitBagisHareketId > 0) return NbysRegularWriteResult.Conflict;
                if (Map<DuzenliNakitBagisci>(repository.ActiveForDonor(donorId, transaction)).Any(row => row.Id != regularId)) return NbysRegularWriteResult.Conflict;
                regular.BagisciId = donorId;
                regular.EslesmeBilgisi = "Kullanıcı tarafından eşleştirildi.";
                Stamp(regular);
                repository.Save(regular, "DuzenliNakitBagisci_Table", regular.Id, transaction);
                transaction.Complete();
            }
            return NbysRegularWriteResult.Completed;
        }

        public NbysRegularWriteResult CreateCertificate(int regularId, int donorId)
        {
            if (regularId <= 0 || donorId <= 0) return NbysRegularWriteResult.InvalidSelection;
            bool existing;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                NakitBagisci donor = First<NakitBagisci>(repository.Donor(donorId, transaction));
                DuzenliNakitBagisci regular = First<DuzenliNakitBagisci>(repository.Regular(regularId, transaction));
                if (donor == null || regular == null) return NbysRegularWriteResult.NotFound;
                if (regular.BagisciId != donor.Id) return NbysRegularWriteResult.Conflict;
                if (!regular.Aktif) return NbysRegularWriteResult.Inactive;
                if (donor.BelgeIstemiyor) return NbysRegularWriteResult.NoCertificateWanted;
                int number = DuzenliNakitBagisci.GetHakedilenBelgeSirasi(regular.BagisAdedi);
                if (number <= 0) return NbysRegularWriteResult.NotEligible;
                Armagan gift = First<Armagan>(repository.Certificate(donor.Id, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, number, transaction));
                existing = gift != null;
                if (existing)
                {
                    NakitBagisHareket linked = First<NakitBagisHareket>(repository.DonationForCertificate(donor.Id, gift.Id, transaction));
                    if (regular.ArmaganId == gift.Id && regular.NakitBagisHareketId == (linked == null ? 0 : linked.Id)) return NbysRegularWriteResult.ExistingCertificate;
                    regular.ArmaganId = gift.Id;
                    regular.NakitBagisHareketId = linked == null ? 0 : linked.Id;
                }
                else
                {
                    if (regular.BaslamaTarihi.Year < 1901) return NbysRegularWriteResult.NotEligible;
                    DateTime start;
                    try { start = regular.BaslamaTarihi.AddMonths((number - 1) * 12); }
                    catch (ArgumentOutOfRangeException) { return NbysRegularWriteResult.NotEligible; }
                    if (start.Year >= 9999) return NbysRegularWriteResult.NotEligible;
                    NakitBagisHareket donation = First<NakitBagisHareket>(repository.LastDonation(donor.Id, start, transaction));
                    if (donation == null) return NbysRegularWriteResult.NoDonation;
                    string status = donor.Ulasilamiyor ? ProjeConstants.DURUM_ULASILAMADI : ProjeConstants.DURUM_GONDERILMEDI;
                    if (repository.HasRefundGift(donor.Id, ProjeConstants.DURUM_PARAIADE, transaction)) status = ProjeConstants.DURUM_DAHAONCEIADE;
                    gift = new Armagan
                    {
                        BagisciId = donor.Id, ArmaganTanimId = ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID,
                        DuzenliBagis = true, KacinciBelge = number, CokluBagis = false, BagisMiktari = regular.BagisToplami,
                        DovizCinsi = ProjeConstants.DOVIZ_TL, Tarih = donation.BagisTarihi, Durum = status
                    };
                    Stamp(gift);
                    gift.Id = repository.Save(gift, "Armagan_Table", 0, transaction);
                    donation.ArmaganId = gift.Id;
                    Stamp(donation);
                    repository.Save(donation, "NakitBagisHareket_Table", donation.Id, transaction);
                    regular.ArmaganId = gift.Id;
                    regular.NakitBagisHareketId = donation.Id;
                }
                Stamp(regular);
                repository.Save(regular, "DuzenliNakitBagisci_Table", regular.Id, transaction);
                transaction.Complete();
            }
            return existing ? NbysRegularWriteResult.ExistingCertificate : NbysRegularWriteResult.Completed;
        }

        private static bool Money(object value, out decimal amount)
        {
            amount = 0;
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value))) return true;
            if (value is decimal || value is double || value is float || value is int || value is long) { amount = Convert.ToDecimal(value, Culture); return true; }
            return Convert.ToString(value).Replace("₺", "").Trim().TryConvertToDecimal(Culture, out amount);
        }

        private static bool StartDate(object value, bool date1904, out DateTime date)
        {
            date = DateTime.MinValue;
            if (value is decimal || value is double || value is int)
            {
                try { date = DateTime.FromOADate(Convert.ToDouble(value, CultureInfo.InvariantCulture) + (date1904 ? 1462 : 0)); }
                catch (ArgumentException) { return false; }
            }
            else if (!DateTime.TryParse(Convert.ToString(value, Culture), Culture, DateTimeStyles.None, out date)) return false;
            return date.Year >= 1901;
        }

        private void Stamp(EntityBase item)
        {
            if (item.Id == 0) { item.Olusturan = context.UserLogin; item.OlusturmaTarihi = DateTime.Now; }
            else { item.Degistiren = context.UserLogin; item.DegistirmeTarihi = DateTime.Now; }
        }
        private static List<T> Map<T>(DataTable rows) where T : new() { return new DuzenliNakitBagisci().ToList<T>(rows); }
        private static T First<T>(DataTable rows) where T : class, new() { return Map<T>(rows).FirstOrDefault(); }
    }
}
