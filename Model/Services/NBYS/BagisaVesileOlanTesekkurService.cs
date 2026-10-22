using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;
using DAO.Ortak;

namespace Model.Services.NBYS
{
    public enum NbysReferralWriteResult { Completed, Unchanged, NotFound, InvalidSelection, NameRequired, InvalidDate, InvalidIdentity }

    public class BagisaVesileOlanTesekkurService
    {
        private readonly BagisaVesileOlanTesekkurRepository repository;
        private readonly PortalLogContext context;

        public BagisaVesileOlanTesekkurService(PortalLogContext context)
            : this(new BagisaVesileOlanTesekkurRepository(context)) { this.context = context; }

        public BagisaVesileOlanTesekkurService()
            : this(new BagisaVesileOlanTesekkurRepository())
        {
        }

        public BagisaVesileOlanTesekkurService(BagisaVesileOlanTesekkurRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public BagisaVesileOlanTesekkur GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public DataTable GetListTable()
        {
            return repository.SelectForList();
        }

        public string GetListJson()
        {
            return new BagisaVesileOlanTesekkur().ToJSON(repository.SelectForList());
        }

        public int Save(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(
                    item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return item.Id;
        }

        public bool Update(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            BagisaVesileOlanTesekkur previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(
                    item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return updated;
        }

        public bool Delete(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;
            BagisaVesileOlanTesekkur previous = GetById(item.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return deleted;
        }

        public NbysReferralWriteResult Persist(BagisaVesileOlanTesekkur item)
        {
            if (context == null) throw new InvalidOperationException("Merkezi kayıt işlemi için log bağlamı gereklidir.");
            if (item == null || item.Id < 0) return NbysReferralWriteResult.InvalidSelection;
            if (string.IsNullOrWhiteSpace(item.Adi)) return NbysReferralWriteResult.NameRequired;
            if (item.BelgeTarihi < new DateTime(1753, 1, 1)) return NbysReferralWriteResult.InvalidDate;
            if (item.TCKimlikNo < 0) return NbysReferralWriteResult.InvalidIdentity;
            int id = item.Id;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                BagisaVesileOlanTesekkur previous = id > 0 ? Map(repository.SelectById(id, transaction)).FirstOrDefault() : null;
                if (id > 0 && previous == null) return NbysReferralWriteResult.NotFound;
                if (previous != null)
                {
                    if (SameForm(previous, item)) return NbysReferralWriteResult.Unchanged;
                    // These fields are not edited by either form. Preserve the actual stored values.
                    item.Telefon = previous.Telefon;
                    item.Adres = previous.Adres;
                    item.Olusturan = previous.Olusturan;
                    item.OlusturmaTarihi = previous.OlusturmaTarihi;
                    item.Degistiren = context.UserLogin;
                    item.DegistirmeTarihi = DateTime.Now;
                    repository.Update(item, id, transaction);
                }
                else
                {
                    item.Olusturan = context.UserLogin;
                    item.OlusturmaTarihi = DateTime.Now;
                    id = repository.Insert(item, transaction);
                }
                transaction.Complete();
            }
            item.Id = id;
            return NbysReferralWriteResult.Completed;
        }

        public NbysReferralWriteResult Remove(int id)
        {
            if (context == null) throw new InvalidOperationException("Merkezi kayıt işlemi için log bağlamı gereklidir.");
            if (id <= 0) return NbysReferralWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (Map(repository.SelectById(id, transaction)).FirstOrDefault() == null) return NbysReferralWriteResult.NotFound;
                repository.Delete(id, transaction);
                transaction.Complete();
            }
            return NbysReferralWriteResult.Completed;
        }

        private static bool SameForm(BagisaVesileOlanTesekkur first, BagisaVesileOlanTesekkur second)
        {
            return first.BelgeTarihi == second.BelgeTarihi && first.TCKimlikNo == second.TCKimlikNo
                && Equal(first.Adi, second.Adi) && Equal(first.Soyadi, second.Soyadi) && Equal(first.Unvan, second.Unvan)
                && Equal(first.Aciklama, second.Aciklama) && Equal(first.VerilmeSebebi, second.VerilmeSebebi)
                && Equal(first.BelgeMetni1, second.BelgeMetni1) && Equal(first.BelgeMetni2, second.BelgeMetni2)
                && Equal(first.ImzalayanAdiSoyadi, second.ImzalayanAdiSoyadi)
                && Equal(first.ImzalayanUnvan, second.ImzalayanUnvan) && Equal(first.ImzalayanMakam, second.ImzalayanMakam);
        }

        private static bool Equal(string first, string second) { return (first ?? string.Empty) == (second ?? string.Empty); }

        private static List<BagisaVesileOlanTesekkur> Map(DataTable table)
        {
            return new BagisaVesileOlanTesekkur().ToList<BagisaVesileOlanTesekkur>(table);
        }
    }
}
