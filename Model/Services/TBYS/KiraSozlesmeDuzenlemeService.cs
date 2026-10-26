using DAO.Ortak;
using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public enum KiraSozlesmeDuzenlemeResult { Completed, Changed, NotFound, InvalidInput, ConfirmationRequired }

    public sealed class KiraSozlesmeDuzenlemeSnapshot
    {
        public KiraSozlesme Contract { get; internal set; }
        public string Version { get; internal set; }
    }

    public sealed class KiraSozlesmeDuzenlemeService
    {
        private readonly PortalLogContext context;
        private readonly KiraSozlesmeRepository repository;

        public KiraSozlesmeDuzenlemeService(PortalLogContext context)
        {
            if (context == null || context.Project != "TBYS" || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("TBYS kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
            repository = new KiraSozlesmeRepository(context);
        }

        public KiraSozlesmeDuzenlemeSnapshot Load(int id)
        {
            return Snapshot(repository.SelectById(id));
        }

        public KiraSozlesmeDuzenlemeResult Save(KiraSozlesme input, string expectedVersion,
            bool confirmPlanDeletion, out KiraSozlesmeDuzenlemeSnapshot saved)
        {
            saved = null;
            if (input == null || input.Id <= 0) return KiraSozlesmeDuzenlemeResult.InvalidInput;
            KiraSozlesmeDuzenlemeSnapshot completed;
            using (var transaction = new SqlTransactionContext())
            {
                var current = Snapshot(repository.SelectById(input.Id, transaction, context));
                if (current == null) return KiraSozlesmeDuzenlemeResult.NotFound;
                if (string.IsNullOrEmpty(expectedVersion) || current.Version != expectedVersion)
                    return KiraSozlesmeDuzenlemeResult.Changed;
                if (input.SozBasTar < new DateTime(1753, 1, 1) || input.SozBitTar < input.SozBasTar
                    || input.KiraBedeli <= 0 || input.TaksitSayisi < 1 || input.TaksitSayisi > 12)
                    return KiraSozlesmeDuzenlemeResult.InvalidInput;

                KiraSozlesme contract = current.Contract;
                bool critical = contract.SozBasTar.Date != input.SozBasTar.Date || contract.SozBitTar.Date != input.SozBitTar.Date
                    || contract.KiraBedeli != input.KiraBedeli || contract.TaksitSayisi != input.TaksitSayisi;
                if (critical && !confirmPlanDeletion) return KiraSozlesmeDuzenlemeResult.ConfirmationRequired;
                CopyEditableFields(input, contract);
                Kiraci tenant = new Kiraci().ToList<Kiraci>(
                    new KiraciRepository(context).SelectById(contract.KiraciId, transaction, context)).FirstOrDefault();
                if (tenant == null) throw new InvalidOperationException("Sözleşmenin kiracı kaydı bulunamadı.");
                DataTable region = repository.SelectRenewalRegion(tenant.Ili, transaction, context);
                contract.BolgeId = region == null || region.Rows[0].IsNull("BolgeId") ? 0 : Convert.ToInt32(region.Rows[0]["BolgeId"]);

                var plans = new OdemePlaniService(context);
                var existingPlans = plans.GetBySozlesmeId(contract.Id, transaction);
                if (critical && existingPlans.Count > 0)
                {
                    if (!new OdemeAyrintiService(context).DeleteBySozlesmeId(contract.Id, transaction)
                        || !plans.DeleteBySozlesmeId(existingPlans[0], contract.Id, transaction))
                        throw new InvalidOperationException("Sözleşmenin ödeme planı silinemedi.");
                }
                else if (!critical)
                {
                    OdemePlani carry = existingPlans.FirstOrDefault(p => p.Sira == 0);
                    if (carry != null && (carry.AnaPara != contract.DevirAnaPara || carry.FaizTutari != contract.DevirFaizTutari
                        || carry.FaizliBakiye != contract.DevirFaizliBakiye))
                    {
                        carry.AnaPara = contract.DevirAnaPara;
                        carry.FaizTutari = contract.DevirFaizTutari;
                        carry.FaizliBakiye = carry.AnaPara + carry.FaizTutari;
                        if (!plans.Update(carry, transaction)) throw new InvalidOperationException("Devir ödeme planı güncellenemedi.");
                        UpdateLegalTracking(contract, plans, transaction);
                    }
                }
                if (!new KiraSozlesmeService(context).Update(contract, transaction))
                    throw new InvalidOperationException("Sözleşme güncellenemedi.");
                completed = Snapshot(repository.SelectById(contract.Id, transaction, context));
                if (completed == null) throw new InvalidOperationException("Güncellenen sözleşme okunamadı.");
                transaction.Complete();
            }
            saved = completed;
            return KiraSozlesmeDuzenlemeResult.Completed;
        }

        public KiraSozlesmeDuzenlemeResult SaveDocumentPath(int id, string path, string expectedVersion,
            out KiraSozlesmeDuzenlemeSnapshot saved)
        {
            saved = null;
            KiraSozlesmeDuzenlemeSnapshot completed;
            using (var transaction = new SqlTransactionContext())
            {
                var current = Snapshot(repository.SelectById(id, transaction, context));
                if (current == null) return KiraSozlesmeDuzenlemeResult.NotFound;
                if (string.IsNullOrEmpty(expectedVersion) || current.Version != expectedVersion)
                    return KiraSozlesmeDuzenlemeResult.Changed;
                current.Contract.SozlesmePDFDosyasi = path;
                if (!new KiraSozlesmeService(context).Update(current.Contract, transaction))
                    throw new InvalidOperationException("Sözleşmenin belge bilgisi kaydedilemedi.");
                completed = Snapshot(repository.SelectById(id, transaction, context));
                if (completed == null) throw new InvalidOperationException("Sözleşmenin belge bilgisi okunamadı.");
                transaction.Complete();
            }
            saved = completed;
            return KiraSozlesmeDuzenlemeResult.Completed;
        }

        private void UpdateLegalTracking(KiraSozlesme contract, OdemePlaniService plans, SqlTransactionContext transaction)
        {
            var service = new HukukiTakipService(context);
            HukukiTakip item;
            if (contract.Aktif && contract.SozlesmeDurumu == ProjeConstants.KIRASOZLESME_DURUMU_TAKIP)
            {
                DateTime date = contract.DurumDegismeTar;
                OdemePlani balance = plans.GetBySozlesmeIdOdemeTarihi(contract.Id, date, transaction)
                    ?? plans.GetBySozlesmeId(contract.Id, transaction).OrderBy(p => p.VadeBasTar).LastOrDefault();
                item = new HukukiTakip { SozlesmeId = contract.Id, KiraciId = contract.KiraciId,
                    BorcAnaPara = balance == null ? 0 : balance.AnaPara,
                    BorcFaiz = balance == null ? 0 : balance.FaizliBakiye - balance.AnaPara,
                    Aktif = true, Aciklama = ProjeConstants.KIRASOZLESME_DURUMU_TAKIP };
            }
            else
            {
                item = new HukukiTakip().ToList<HukukiTakip>(
                    new HukukiTakipRepository(context).SelectBySozlesmeId(contract.Id, transaction, context)).FirstOrDefault();
                if (item == null) return;
                item.Aktif = false;
                item.Aciklama = ProjeConstants.KIRASOZLESME_DURUMU_TAKIPSONUCLANDI;
            }
            item.IslemTarihi = DateTime.Today;
            if (service.Save(item, transaction) <= 0) throw new InvalidOperationException("Hukuki takip kaydı oluşturulamadı.");
        }

        private static void CopyEditableFields(KiraSozlesme input, KiraSozlesme target)
        {
            target.DosyaNo = input.DosyaNo;
            target.DevirAnaPara = input.DevirAnaPara;
            target.DevirFaizTutari = input.DevirFaizTutari;
            target.DevirFaizliBakiye = input.DevirFaizliBakiye;
            target.IlkSozlesmeTar = input.IlkSozlesmeTar;
            target.SozBasTar = input.SozBasTar;
            target.SozBitTar = input.SozBitTar;
            target.OdemeSekli = input.OdemeSekli;
            target.TaksitSayisi = input.TaksitSayisi;
            target.KiraBedeli = input.KiraBedeli;
            target.KefilAdiSoyadi = input.KefilAdiSoyadi;
            target.KefilTCKimlikNo = input.KefilTCKimlikNo;
            target.KefilAdresi = input.KefilAdresi;
            target.KefilTel = input.KefilTel;
            target.ArtisAyi = input.ArtisAyi;
            target.Aciklama = input.Aciklama;
            if (!target.Aktif) { target.SozlesmeDurumu = input.SozlesmeDurumu; target.DurumDegismeTar = input.DurumDegismeTar; }
            if (string.IsNullOrEmpty(target.GecikmeZammiTipi)) target.GecikmeZammiTipi = ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK;
        }

        private static KiraSozlesmeDuzenlemeSnapshot Snapshot(DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return null;
            // Hash the same raw row used to populate the form, including NULLs and audit fields.
            var values = table.Columns.Cast<DataColumn>().OrderBy(c => c.ColumnName, StringComparer.Ordinal)
                .Select(c => new { Name = c.ColumnName, Type = c.DataType.FullName,
                    Value = Value(table.Rows[0][c]) }).ToArray();
            string json = new JavaScriptSerializer().Serialize(values);
            string version;
            using (SHA256 hash = SHA256.Create()) version = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(json)));
            return new KiraSozlesmeDuzenlemeSnapshot { Contract = new KiraSozlesme().ToList<KiraSozlesme>(table).First(), Version = version };
        }

        private static string Value(object value)
        {
            if (value == DBNull.Value) return null;
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            if (value is byte[]) return Convert.ToBase64String((byte[])value);
            if (value is double) return ((double)value).ToString("R", CultureInfo.InvariantCulture);
            if (value is float) return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
