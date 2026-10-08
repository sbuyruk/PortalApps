using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.MTS
{
    public class AniObjesiDagitim : EntityBase
    {
        public int AniObjesiId { get; set; }
        public int Adet { get; set; }
        public int KatilimciId { get; set; }
        public int FaaliyetId { get; set; }
        public int VerilenAlinan { get; set; }
        public int DagitimYeriTanimId { get; set; } = 0;
        public int CikisDepoId { get; set; } = 0;
        public string GetirilenAniObjesi { get; set; }
        public string Aciklama { get; set; }
        public DateTime VerilisTarihi { get; set; }

        public int Save()
        {
            return new AniObjesiDagitimService().Save(this);
        }

        public bool Update()
        {
            return new AniObjesiDagitimService().Update(this);
        }

        public bool Delete()
        {
            return new AniObjesiDagitimService().Delete(this);
        }

        public AniObjesiDagitim Select(int id)
        {
            Id = id;
            return new AniObjesiDagitimService().GetById(id);
        }

        public int Delete(int faaliyetId, int katilimciId, string aniObjesiIdList = "")
        {
            return new AniObjesiDagitimService().DeleteByActivityAndParticipant(faaliyetId, katilimciId, aniObjesiIdList);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new AniObjesiDagitimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new AniObjesiDagitimService().GetAll(), typeof(List<T>));
        }

        public AniObjesiDagitim SelectGetirilenAniObjesi(int faaliyetId, int katilimciId)
        {
            return new AniObjesiDagitimService().GetGetirilen(faaliyetId, katilimciId);
        }

        public DataTable SelectReturnDT(int faaliyetId, int katilimciId)
        {
            return new AniObjesiDagitimService().GetDistributionTable(faaliyetId, katilimciId);
        }

        public DataTable SelectByFaaliyetIdKatilimciIdStokReturnDT(int faaliyetId, int katilimciId, string stokluMu)
        {
            return new AniObjesiDagitimService().GetDistributionTable(faaliyetId, katilimciId, stokluMu);
        }

        public List<AniObjesiDagitim> SelectStoksuzAniObjeleriReturnList(int faaliyetId, int katilimciId)
        {
            return new AniObjesiDagitimService().GetStoksuz(faaliyetId, katilimciId);
        }

        public AniObjesiDagitim Select(int faaliyetId, int katilimciId, int aniObjesiId)
        {
            return new AniObjesiDagitimService().GetByActivityParticipantObject(faaliyetId, katilimciId, aniObjesiId);
        }

        public List<AniObjesiDagitim> SelectByKisiIdReturnList(int katilimciId, string verilenGetirilen)
        {
            return new AniObjesiDagitimService().GetByParticipant(katilimciId, verilenGetirilen);
        }

        public string SelectGetirilenByKatilimcidFaaliyetId(int katilimciId, int faaliyetId)
        {
            return new AniObjesiDagitimService().GetGetirilenText(katilimciId, faaliyetId);
        }

        public DataTable SelectByKatilimcidFaaliyetId(int katilimciId, int faaliyetId, string stokluMu)
        {
            return new AniObjesiDagitimService().GetByParticipantActivity(katilimciId, faaliyetId, stokluMu);
        }

        public List<AniObjesiDagitim> SelectByFaaliyetId(int faaliyetId)
        {
            return new AniObjesiDagitimService().GetByFaaliyetId(faaliyetId);
        }
    }
}
