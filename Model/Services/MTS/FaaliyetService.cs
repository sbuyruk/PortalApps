using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class FaaliyetService
    {
        private const string TableName = "Faaliyet_Table";
        private readonly MtsLookupRepository repository;

        public FaaliyetService() : this(new MtsLookupRepository())
        {
        }

        public FaaliyetService(MtsLookupRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public Faaliyet GetById(int id)
        {
            return Map(repository.SelectById(TableName, id));
        }

        public List<Faaliyet> GetAll()
        {
            return ToList(repository.SelectAll(TableName));
        }

        public List<Faaliyet> GetAll(string acikTarih)
        {
            return ToList(repository.SelectFaaliyetByAcikTarih(acikTarih));
        }

        public DataTable GetAllData(string acikTarih)
        {
            return repository.SelectFaaliyetByAcikTarih(acikTarih);
        }

        public List<Faaliyet> GetByDate(DateTime tarih)
        {
            DateTime baslangic = new DateTime(tarih.Year, tarih.Month, tarih.Day);
            DateTime bitis = UtilityHelper.TariheSaatEkle(tarih, "23:59");
            return ToList(repository.SelectFaaliyetByDate(baslangic, bitis));
        }

        public DataTable GetParticipantList(int faaliyetId, int monthBefore, string acikTarihli, DateTime basTar, DateTime bitTar, string faaliyetAmaci)
        {
            return repository.SelectFaaliyetParticipants(
                faaliyetId,
                monthBefore,
                acikTarihli,
                basTar,
                bitTar,
                faaliyetAmaci,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE,
                ProjeConstants.REFERANS_TARIHI);
        }

        public DataTable GetByParticipant(int katilimciId, int faaliyetId)
        {
            return repository.SelectFaaliyetByParticipant(
                katilimciId,
                faaliyetId,
                ProjeConstants.MTSGOREVDURUMU_GOREVDE);
        }

        public List<string> GetDistinctPlaces()
        {
            return repository.SelectDistinctFaaliyetYeri()
                .AsEnumerable()
                .Select(row => row.Field<string>("FaaliyetYeri"))
                .ToList();
        }

        public int Save(Faaliyet item)
        {
            item.UniqueId = Guid.NewGuid();
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(TableName, item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return item.Id;
        }

        public bool Update(Faaliyet item)
        {
            Faaliyet old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(TableName, item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return updated;
        }

        public bool Delete(Faaliyet item)
        {
            Faaliyet old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(TableName, item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
            return deleted;
        }

        private static List<Faaliyet> ToList(DataTable table)
        {
            return new Faaliyet().ToList<Faaliyet>(table);
        }

        private static Faaliyet Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
