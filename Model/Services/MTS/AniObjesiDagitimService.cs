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
    public class AniObjesiDagitimService
    {
        private const string TableName = "AniObjesiDagitim_Table";
        private readonly MtsLookupRepository repository;

        public AniObjesiDagitimService() : this(new MtsLookupRepository())
        {
        }

        public AniObjesiDagitimService(MtsLookupRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public AniObjesiDagitim GetById(int id)
        {
            return Map(repository.SelectById(TableName, id));
        }

        public List<AniObjesiDagitim> GetAll()
        {
            return ToList(repository.SelectAll(TableName));
        }

        public List<AniObjesiDagitim> GetByFaaliyetId(int faaliyetId)
        {
            return ToList(repository.SelectAniObjesiDagitimByFilter(faaliyetId, 0, 0, null, null, "activity"));
        }

        public AniObjesiDagitim GetGetirilen(int faaliyetId, int katilimciId)
        {
            return Map(repository.SelectGetirilenAniObjesi(faaliyetId, katilimciId, 0, ProjeConstants.ANIOBJESI_GETIRILEN_INT));
        }

        public DataTable GetDistributionTable(int faaliyetId, int katilimciId, string stokluMu = null)
        {
            return repository.SelectAniObjesiDagitimByFilter(
                faaliyetId,
                katilimciId,
                0,
                stokluMu,
                null,
                string.IsNullOrEmpty(stokluMu) ? "return" : "stok");
        }

        public List<AniObjesiDagitim> GetStoksuz(int faaliyetId, int katilimciId)
        {
            return ToList(repository.SelectAniObjesiDagitimByFilter(
                faaliyetId,
                katilimciId,
                0,
                ProjeConstants.MTS_ANIOBJESISTOKSUZ,
                null,
                "stoksuz"));
        }

        public AniObjesiDagitim GetByActivityParticipantObject(int faaliyetId, int katilimciId, int aniObjesiId)
        {
            return Map(repository.SelectAniObjesiDagitimByFilter(
                faaliyetId,
                katilimciId,
                aniObjesiId,
                null,
                null,
                "single"));
        }

        public List<AniObjesiDagitim> GetByParticipant(int katilimciId, string verilenGetirilen)
        {
            return ToList(repository.SelectAniObjesiDagitimByFilter(
                0,
                katilimciId,
                0,
                null,
                verilenGetirilen,
                "kisi"));
        }

        public string GetGetirilenText(int katilimciId, int faaliyetId)
        {
            DataTable table = repository.SelectGetirilenAniObjesiText(
                katilimciId,
                faaliyetId,
                ProjeConstants.GETIRILEN_ANIOBJESIID_INT);
            string result = string.Empty;
            if (table == null)
                return result;

            foreach (DataRow row in table.Rows)
            {
                string value = row["GetirilenAniObjesi"].ReturnEmptyIfNull().ToString();
                if (!string.IsNullOrEmpty(value))
                    result += "<br> * " + value;
            }
            return result;
        }

        public DataTable GetByParticipantActivity(int katilimciId, int faaliyetId, string stokluMu)
        {
            return repository.SelectAniObjesiDagitimByFilter(
                faaliyetId,
                katilimciId,
                0,
                stokluMu,
                null,
                "katilimciFaaliyet");
        }

        public int DeleteByActivityAndParticipant(int faaliyetId, int katilimciId, string aniObjesiIdList)
        {
            if (string.IsNullOrWhiteSpace(aniObjesiIdList))
                return 0;

            List<int> ids = aniObjesiIdList.Split(',').Select(value =>
            {
                int id;
                if (!int.TryParse(value.Trim(), out id))
                    throw new ArgumentException("Ani objesi listesi geçersiz bir değer içeriyor.", "aniObjesiIdList");
                return id;
            }).ToList();

            DataTable deletedItems;
            int deleted = repository.DeleteAniObjesiDagitim(faaliyetId, katilimciId, ids, out deletedItems);
            if (deleted > 0 && ProjeConstants.MTS_DELETE_LOG)
            {
                foreach (AniObjesiDagitim item in ToList(deletedItems))
                    new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
            }
            return deleted;
        }

        public int Save(AniObjesiDagitim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(TableName, item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
            return item.Id;
        }

        public bool Update(AniObjesiDagitim item)
        {
            AniObjesiDagitim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(TableName, item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
            return updated;
        }

        public bool Delete(AniObjesiDagitim item)
        {
            AniObjesiDagitim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(TableName, item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESIDAGITIM);
            return deleted;
        }

        private static List<AniObjesiDagitim> ToList(DataTable table)
        {
            return new AniObjesiDagitim().ToList<AniObjesiDagitim>(table);
        }

        private static AniObjesiDagitim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}
