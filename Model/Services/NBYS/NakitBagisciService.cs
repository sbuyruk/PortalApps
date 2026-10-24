using Utility.Logging;
using DAO.Repositories.NBYS;
using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class NakitBagisciService
    {
        private readonly NakitBagisciRepository repository;

        public NakitBagisciService(PortalLogContext context) : this(new NakitBagisciRepository(context)) { }

        public NakitBagisciService()
            : this(new NakitBagisciRepository())
        {
        }

        public NakitBagisciService(NakitBagisciRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public int Save(NakitBagisci nakitBagisci, SqlTransactionContext transaction)
        {
            if (!repository.HasLogContext) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (nakitBagisci == null)
                throw new ArgumentNullException("nakitBagisci");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            nakitBagisci.OlusturmaTarihi = DateTime.Now;
            nakitBagisci.Olusturan = UtilityHelper.GetCurrentUserName();
            nakitBagisci.Id = repository.Insert(nakitBagisci, transaction);

            return nakitBagisci.Id;
        }

        public NakitBagisci GetById(int id)
        {
            return MapSingle(repository.SelectById(id));
        }

        public NakitBagisci GetById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return MapSingle(repository.SelectById(id, transaction));
        }

        public NakitBagisci GetBagisiOlmayanById(int id)
        {
            return MapSingle(repository.SelectBagisiOlmayanBagisciById(id));
        }

        public NakitBagisci GetByTcKimlikNo(long tcKimlikNo)
        {
            return MapSingle(repository.SelectByTcKimlikno(tcKimlikNo));
        }

        public NakitBagisci GetByTcKimlikNo(long tcKimlikNo, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return MapSingle(repository.SelectByTcKimlikno(tcKimlikNo, transaction));
        }

        public NakitBagisci GetByAdAndTelefon(string adi, string telefon)
        {
            return MapSingle(repository.SelectByAdAndTelefon(adi, telefon));
        }

        public NakitBagisci GetByAdAndTelefon(
            string adi,
            string telefon,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return MapSingle(repository.SelectByAdAndTelefon(adi, telefon, transaction));
        }

        public NakitBagisci GetByTelefon(string telefon)
        {
            return MapSingle(repository.SelectByTelefon(telefon));
        }

        public NakitBagisci GetByEposta(string eposta)
        {
            return MapSingle(repository.SelectByEposta(eposta));
        }

        public DataTable SearchByName(string adi)
        {
            return repository.SelectByAd(adi);
        }

        public DataTable Search(string filter, int excludedId)
        {
            return repository.SelectByFilter(filter, excludedId);
        }

        public DataTable ListByIl(int ilId)
        {
            return repository.SelectByIl(NormalizeIlId(ilId));
        }

        public DataTable ListByIlAndBagisTarihi(
            int ilId,
            string baslangicTarihi,
            string bitisTarihi)
        {
            return repository.SelectByIlBagisTarihi(
                NormalizeIlId(ilId),
                baslangicTarihi,
                bitisTarihi);
        }

        public DataTable ListNewByIlAndBagisTarihi(
            int ilId,
            string baslangicTarihi,
            string bitisTarihi)
        {
            return repository.SelectByIlBagisTarihiYeni(
                NormalizeIlId(ilId),
                baslangicTarihi,
                bitisTarihi);
        }

        public bool Update(NakitBagisci nakitBagisci, SqlTransactionContext transaction)
        {
            if (!repository.HasLogContext) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (nakitBagisci == null)
                throw new ArgumentNullException("nakitBagisci");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            if (nakitBagisci.Id == 0)
                return false;

            nakitBagisci.DegistirmeTarihi = DateTime.Now;
            nakitBagisci.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = repository.Update(nakitBagisci, transaction);

            return updated;
        }

        public int CreateBilinmeyenBagisci(SqlTransactionContext transaction)
        {
            if (!repository.HasLogContext) throw new InvalidOperationException("Yazma işlemi için NBYS log bağlamı gereklidir.");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            NakitBagisci bagisci = new NakitBagisci();
            bagisci.OlusturmaTarihi = DateTime.Now;
            bagisci.Olusturan = UtilityHelper.GetCurrentUserName();
            return repository.CreateBilinmeyenBagisci(
                bagisci,
                ProjeConstants.NAKITBAGISCI_BILINMEYEN,
                DateTime.Now,
                UtilityHelper.GetCurrentUserName(),
                transaction);
        }

        private static NakitBagisci MapSingle(DataTable dataTable)
        {
            NakitBagisci mapper = new NakitBagisci();
            List<NakitBagisci> list = mapper.ToList<NakitBagisci>(dataTable);
            return list.FirstOrDefault();
        }

        private static int? NormalizeIlId(int ilId)
        {
            return ilId > ProjeConstants.IL_HEPSI ? (int?)ilId : null;
        }
    }
}
