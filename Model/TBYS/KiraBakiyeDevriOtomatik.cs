using DAO.Ortak;
using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;

namespace Model.TBYS
{
    /// <summary>Transfers outstanding balances and recalculates the tenant's plans in one transaction.</summary>
    public class KiraBakiyeDevriOtomatik
    {
        private readonly PortalLogContext context;

        public KiraBakiyeDevriOtomatik(PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            this.context = context;
        }

        public KiraBakiyeDevriSonuc TumAktifSozlesmeleriIsle()
        {
            KiraBakiyeDevriSonuc sonuc = new KiraBakiyeDevriSonuc();
            foreach (KiraSozlesme item in new KiraSozlesmeService(context).GetAllActive())
            {
                try
                {
                    if (DevirAl(item)) sonuc.IslenenSozlesmeIdleri.Add(item.Id);
                    else sonuc.AtlananSozlesmeIdleri.Add(item.Id);
                }
                catch (Exception error)
                {
                    if (error is System.Threading.ThreadAbortException) throw;
                    PortalTechnicalLog.WriteError(context, "TBYS.KiraBakiyeDevriOtomatik.DevirAl", error);
                    sonuc.HataliSozlesmeler.Add(item.Id,
                        "Bakiye devri tamamlanamadı. Başvuru kodu: " + context.OperationId.ToString("N"));
                }
            }
            return sonuc;
        }

        public bool DevirAl(KiraSozlesme item)
        {
            if (item == null) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                KiraSozlesmeService contracts = new KiraSozlesmeService(context);
                OdemePlaniService plans = new OdemePlaniService(context);
                KiraSozlesme current = contracts.GetById(item.Id, transaction);
                if (current == null) return false;
                KiraSozlesme previous = contracts.GetPreviousByTenant(current.KiraciId, current.SozBasTar, transaction);
                if (previous == null) return false;
                List<OdemePlani> previousPlans = plans.GetBySozlesmeId(previous.Id, transaction);
                if (previousPlans.Count == 0) return false;
                OdemePlani last = previousPlans.Last();
                decimal principal = last.AnaPara;
                decimal balance = last.FaizliBakiye;
                decimal interest = balance - principal;
                OdemePlani carry = plans.GetBySozlesmeId(current.Id, transaction).FirstOrDefault(x => x.Sira == 0);
                if (carry == null) throw new InvalidOperationException("Sözleşmenin devir satırı bulunamadı.");

                bool contractChanged = current.DevirAnaPara != principal
                    || current.DevirFaizTutari != interest || current.DevirFaizliBakiye != balance;
                bool planChanged = carry.AnaPara != principal || carry.FaizTutari != interest || carry.FaizliBakiye != balance;
                if (!contractChanged && !planChanged) return false;
                if (contractChanged)
                {
                    current.DevirAnaPara = principal;
                    current.DevirFaizTutari = interest;
                    current.DevirFaizliBakiye = balance;
                    if (!contracts.Update(current, transaction))
                        throw new InvalidOperationException("Sözleşmenin devir tutarları güncellenemedi.");
                }
                if (planChanged)
                {
                    carry.AnaPara = principal;
                    carry.FaizTutari = interest;
                    carry.FaizliBakiye = balance;
                    if (!plans.Update(carry, transaction))
                        throw new InvalidOperationException("Ödeme planının devir satırı güncellenemedi.");
                }
                foreach (KiraSozlesme contract in contracts.GetAllByKiraciId(current.KiraciId, transaction).OrderBy(x => x.SozBasTar))
                    if (!TBYSOrtak.BakiyeBorcHesapla(contract, context, transaction))
                        throw new InvalidOperationException("Kiracının ödeme planları güncellenemedi.");
                transaction.Complete();
            }
            return true;
        }
    }

    public class KiraBakiyeDevriSonuc
    {
        public KiraBakiyeDevriSonuc()
        {
            IslenenSozlesmeIdleri = new List<int>();
            AtlananSozlesmeIdleri = new List<int>();
            HataliSozlesmeler = new Dictionary<int, string>();
        }
        public List<int> IslenenSozlesmeIdleri { get; set; }
        public List<int> AtlananSozlesmeIdleri { get; set; }
        public Dictionary<int, string> HataliSozlesmeler { get; set; }
    }
}
