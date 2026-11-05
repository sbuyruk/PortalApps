using DAO.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public sealed class TeminatIslemWriteService
    {
        private readonly PortalLogContext context;

        public TeminatIslemWriteService(PortalLogContext context)
        {
            if (context == null || context.Project != "TBYS" || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new ArgumentException("TBYS kullanıcı bağlamı gereklidir.", "context");
            this.context = context;
        }

        public void Save(int contractId, TeminatIslem input)
        {
            Validate(input);
            using (var transaction = new SqlTransactionContext())
            {
                KiraSozlesme contract = LoadContract(contractId, transaction);
                var item = new TeminatIslem
                {
                    KiraciId = contract.KiraciId,
                    IslemTipi = input.IslemTipi,
                    IslemTarihi = input.IslemTarihi,
                    IslemTutari = input.IslemTutari,
                    DovizCinsi = ProjeConstants.DOVIZ_TL,
                    Aciklama = input.Aciklama
                };
                if (item.IslemTipi == ProjeConstants.TEMINAT_KIRAYAMAHSUP)
                {
                    KiraSozlesme target = LoadPaymentContract(contract.KiraciId, item.IslemTarihi, transaction);
                    int paymentId = 0;
                    if (!TBYSOrtak.OdemeYap(target, item.IslemTarihi, item.IslemTutari, item.Aciklama,
                        ref paymentId, context, transaction) || paymentId <= 0)
                        throw new InvalidOperationException("Mahsup için uygun ödeme planı bulunamadı; işlem kaydedilmedi.");
                    item.OdemeId = paymentId;
                }
                if (new TeminatIslemService(context).Save(item, transaction) <= 0)
                    throw new InvalidOperationException("Teminat işlemi kaydedilemedi.");
                UpdateTotals(contract, transaction);
                transaction.Complete();
            }
        }

        public void Update(int contractId, int itemId, TeminatIslem input)
        {
            Validate(input);
            using (var transaction = new SqlTransactionContext())
            {
                KiraSozlesme contract = LoadContract(contractId, transaction);
                var service = new TeminatIslemService(context);
                TeminatIslem item = LoadItem(service, itemId, contract.KiraciId, transaction);
                // The persisted type and payment link cannot be replaced by form values.
                if (item.IslemTipi != input.IslemTipi)
                    throw new InvalidOperationException("Teminat işlem türü değiştirilemez.");
                if (item.IslemTipi == ProjeConstants.TEMINAT_KIRAYAMAHSUP)
                {
                    var payments = new OdemeService(context);
                    Odeme payment = LoadPayment(payments, item, transaction);
                    KiraSozlesme target = LoadPaymentContract(contract.KiraciId, input.IslemTarihi, transaction);
                    OdemePlani plan = TBYSOrtak.MahsupOdemePlaniGetir(target, input.IslemTarihi, context, transaction);
                    if (plan == null)
                        throw new InvalidOperationException("Mahsup için uygun ödeme planı bulunamadı; işlem güncellenmedi.");
                    if (!payments.OdemeyiVeOdemePlaniniGuncelle(target.Id, payment.OdemePlaniId, plan.Id,
                        payment.Id, input.IslemTarihi, input.IslemTutari, input.Aciklama, context.UserLogin, transaction))
                        throw new InvalidOperationException("Mahsup ödemesi ve ödeme planı güncellenemedi.");
                }
                item.IslemTarihi = input.IslemTarihi;
                item.IslemTutari = input.IslemTutari;
                item.Aciklama = input.Aciklama;
                if (!service.Update(item, transaction))
                    throw new InvalidOperationException("Teminat işlemi güncellenemedi.");
                UpdateTotals(contract, transaction);
                transaction.Complete();
            }
        }

        public void Delete(int contractId, int itemId)
        {
            using (var transaction = new SqlTransactionContext())
            {
                KiraSozlesme contract = LoadContract(contractId, transaction);
                var service = new TeminatIslemService(context);
                TeminatIslem item = LoadItem(service, itemId, contract.KiraciId, transaction);
                if (item.IslemTipi == ProjeConstants.TEMINAT_KIRAYAMAHSUP)
                {
                    var payments = new OdemeService(context);
                    Odeme payment = LoadPayment(payments, item, transaction);
                    // Remove the link before the payment; every following failure rolls both back.
                    if (!service.Delete(item, transaction)
                        || !payments.OdemeyiSilOdemePlaniniGuncelle(payment.Id, item.Aciklama,
                            payment.OdemePlaniId, context.UserLogin, transaction))
                        throw new InvalidOperationException("Mahsup işlemi ve bağlı ödeme silinemedi.");
                }
                else if (!service.Delete(item, transaction))
                    throw new InvalidOperationException("Teminat işlemi silinemedi.");
                UpdateTotals(contract, transaction);
                transaction.Complete();
            }
        }

        private KiraSozlesme LoadContract(int id, SqlTransactionContext transaction)
        {
            KiraSozlesme contract = new KiraSozlesmeService(context).GetById(id, transaction);
            if (contract == null) throw new InvalidOperationException("Kira sözleşmesi bulunamadı.");
            return contract;
        }

        private KiraSozlesme LoadPaymentContract(int tenantId, DateTime date, SqlTransactionContext transaction)
        {
            KiraSozlesme contract = new KiraSozlesmeService(context).GetNearestByKiraciIdAndDate(tenantId, date, transaction);
            if (contract == null) throw new InvalidOperationException("Mahsup edilecek kira sözleşmesi bulunamadı.");
            return contract;
        }

        private static TeminatIslem LoadItem(TeminatIslemService service, int id, int tenantId, SqlTransactionContext transaction)
        {
            TeminatIslem item = service.GetById(id, transaction);
            if (item == null || item.KiraciId != tenantId)
                throw new InvalidOperationException("Teminat işlemi bulunamadı veya seçili kiracıya ait değil.");
            return item;
        }

        private Odeme LoadPayment(OdemeService service, TeminatIslem item, SqlTransactionContext transaction)
        {
            Odeme payment = item.OdemeId > 0 ? service.GetById(item.OdemeId, transaction) : null;
            if (payment == null || payment.KiraciId != item.KiraciId)
                throw new InvalidOperationException("Mahsup işleminin bağlı kira ödemesi bulunamadı veya tutarsız. Kayıt kontrol edilmelidir.");
            OdemePlani plan = new OdemePlaniService(context).GetById(payment.OdemePlaniId, transaction);
            KiraSozlesme contract = LoadContract(payment.SozlesmeId, transaction);
            if (plan == null || plan.SozlesmeId != payment.SozlesmeId || contract.KiraciId != item.KiraciId
                || new TeminatIslemService(context).GetByOdemeId(payment.Id, transaction).Count != 1)
                throw new InvalidOperationException("Mahsup ödemesinin plan veya teminat bağlantısı tutarsız. Kayıt kontrol edilmelidir.");
            return payment;
        }

        private void UpdateTotals(KiraSozlesme contract, SqlTransactionContext transaction)
        {
            if (!TBYSOrtak.TeminatIslemleriniHesaplaVeKaydet(contract, context, transaction))
                throw new InvalidOperationException("Teminat toplamları güncellenemedi.");
        }

        private static void Validate(TeminatIslem input)
        {
            if (input == null || input.IslemTutari <= 0 || input.IslemTarihi < new DateTime(1753, 1, 1))
                throw new ArgumentException("Geçerli bir işlem tarihi ve sıfırdan büyük tutar girilmelidir.", "input");
            switch (input.IslemTipi)
            {
                case ProjeConstants.TEMINAT_ODEMESI:
                case ProjeConstants.TEMINAT_GECICITEMINATODEMESI:
                case ProjeConstants.TEMINAT_KIRACIYAIADE:
                case ProjeConstants.TEMINAT_KIRAYAMAHSUP:
                case ProjeConstants.TEMINAT_HASARAMAHSUP:
                case ProjeConstants.TEMINAT_AIDATAMAHSUP:
                case ProjeConstants.TEMINAT_VAKFABAGIS:
                case ProjeConstants.TEMINAT_MUHASEBEDENAKTARILACAK:
                    return;
                default:
                    throw new ArgumentException("Geçerli bir teminat işlem türü seçilmelidir.", "input");
            }
        }
    }
}
