using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class MTSKurumGorevService
    {
        private readonly MTSKurumGorevRepository repository;
        public MTSKurumGorevService() : this(new MTSKurumGorevRepository())
        {
        }

        public MTSKurumGorevService(MTSKurumGorevRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public MTSKurumGorev GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<MTSKurumGorev> GetAll()
        {
            return ToList(repository.SelectAll());
        }
        public string GetByKisiId(int id, ref string kurum, ref string gorev)
        {
            DataTable table = repository.SelectByKisiId(id);
            if (table != null && table.Rows.Count > 0)
            {
                kurum = table.Rows[0]["Kurum"].ToString();
                gorev = table.Rows[0]["Gorev"].ToString();
            }
            return kurum + " " + gorev;
        }
        public int Save(MTSKurumGorev item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KURUMGOREV);
            return item.Id;
        }

        public bool Update(MTSKurumGorev item)
        {
            MTSKurumGorev old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_KURUMGOREV);
            return updated;
        }

        public bool Delete(MTSKurumGorev item)
        {
            MTSKurumGorev old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);
            return deleted;
        }

        private static List<MTSKurumGorev> ToList(DataTable table)
        {
            return new MTSKurumGorev().ToList<MTSKurumGorev>(table);
        }

        private static MTSKurumGorev Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
