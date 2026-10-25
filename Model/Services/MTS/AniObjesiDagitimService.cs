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
        private readonly AniObjesiDagitimRepository repository;

        public AniObjesiDagitimService() : this(new AniObjesiDagitimRepository())
        {
        }

        public AniObjesiDagitimService(Utility.Logging.PortalLogContext context) : this(new AniObjesiDagitimRepository(context)) { }

        public AniObjesiDagitimService(AniObjesiDagitimRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public AniObjesiDagitim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<AniObjesiDagitim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<AniObjesiDagitim> GetByFaaliyetId(int faaliyetId)
        {
            return ToList(repository.SelectByFilter(faaliyetId, 0, 0, null, null, "activity"));
        }

        public DataTable GetEventDistribution(int faaliyetId, int katilimciId, Utility.Logging.PortalLogContext context)
        {
            return repository.SelectEventDistribution(faaliyetId, katilimciId, context);
        }
        public AniObjesiDagitim GetGetirilen(int faaliyetId, int katilimciId)
        {
            return Map(repository.SelectGetirilen(faaliyetId, katilimciId, ProjeConstants.ANIOBJESI_GETIRILEN_INT));
        }
        public AniObjesiDagitim GetGetirilen(int faaliyetId, int katilimciId, Utility.Logging.PortalLogContext context) { return Map(repository.SelectGetirilen(faaliyetId, katilimciId, ProjeConstants.ANIOBJESI_GETIRILEN_INT, context)); }

        public DataTable GetDistributionTable(int faaliyetId, int katilimciId, string stokluMu = null)
        {
            return repository.SelectByFilter(
                faaliyetId,
                katilimciId,
                0,
                stokluMu,
                null,
                string.IsNullOrEmpty(stokluMu) ? "return" : "stok");
        }

        public List<AniObjesiDagitim> GetStoksuz(int faaliyetId, int katilimciId)
        {
            return ToList(repository.SelectByFilter(
                faaliyetId,
                katilimciId,
                0,
                ProjeConstants.MTS_ANIOBJESISTOKSUZ,
                null,
                "stoksuz"));
        }

        public AniObjesiDagitim GetByActivityParticipantObject(int faaliyetId, int katilimciId, int aniObjesiId)
        {
            return Map(repository.SelectByFilter(
                faaliyetId,
                katilimciId,
                aniObjesiId,
                null,
                null,
                "single"));
        }

        public List<AniObjesiDagitim> GetByParticipant(int katilimciId, string verilenGetirilen)
        {
            return ToList(repository.SelectByFilter(
                0,
                katilimciId,
                0,
                null,
                verilenGetirilen,
                "kisi"));
        }

        public string GetGetirilenText(int katilimciId, int faaliyetId)
        {
            DataTable table = repository.SelectGetirilenText(
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
            return repository.SelectByFilter(
                faaliyetId,
                katilimciId,
                0,
                stokluMu,
                null,
                "katilimciFaaliyet");
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
