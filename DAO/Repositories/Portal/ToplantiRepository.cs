using Utility.Logging;
using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Portal
{
    public class ToplantiRepository
    {
        private readonly PortalSqlExecutor sql = new PortalSqlExecutor("ToplantiRepository");
        public ToplantiRepository(PortalLogContext context)
        {
            sql = new PortalSqlExecutor(context, "ToplantiRepository");
        }
        private DataTable Select(SqlQuery query, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            return sql.Select(query, method: method);
        }

        public ToplantiRepository()
        {
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectById(int id, PortalLogContext context)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(q, context, "ToplantiRepository.SelectById");
        }

        public DataTable SelectAll()
        {
            return Select(new SqlQuery("SELECT * FROM Toplanti_Table ORDER BY ToplantiKonusu"));
        }

        public DataTable SelectByYeri(int id)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri=@Yeri");
            q.AddParameter("@Yeri", id);
            return Select(q);
        }

        public DataTable SelectByStart(int y, DateTime d)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri=@Yeri AND BaslangicTarihi<=@Tarih AND BitisTarihi>=@Tarih");
            q.AddParameter("@Yeri", y);
            q.AddParameter("@Tarih", d);
            return Select(q);
        }

        public DataTable SelectByEnd(int y, DateTime d)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri!=@Diger AND ToplantiYeri=@Yeri AND BitisTarihi>=@Tarih AND BaslangicTarihi<=@Tarih");
            q.AddParameter("@Diger", ProjeConstants.PARAM_DIGER_INT);
            q.AddParameter("@Yeri", y);
            q.AddParameter("@Tarih", d);
            return Select(q);
        }

        public DataTable SelectByParticipantMeeting(int id, string aktif, int? meeting)
        {
            string f = aktif == ProjeConstants.TOPLANTI_AKTIF ? " AND A.BitisTarihi>GETDATE()" : aktif == ProjeConstants.TOPLANTI_PASIF ? " AND A.BitisTarihi<GETDATE()" : "";

            if (meeting.HasValue)
                f += " AND B.ToplantiId=@ToplantiId";

            var q = new SqlQuery("SELECT A.Id ToplantiId,A.*,B.KatilimciId,B.Id KatilimId,B.Bilgi,C.Adi,C.Soyadi,D.ProtokolSiraNo FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE 1=1" + f + " ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");

            if (meeting.HasValue)
                q.AddParameter("@ToplantiId", meeting.Value);

            return Select(q);
        }

        public DataTable SelectByParticipantMeetingDate(int id, string aktif, int? meeting, DateTime from, DateTime to)
        {
            string f = aktif == ProjeConstants.TOPLANTI_AKTIF ? " AND A.BitisTarihi>GETDATE()" : aktif == ProjeConstants.TOPLANTI_PASIF ? " AND A.BitisTarihi<GETDATE()" : "";

            if (meeting.HasValue)
                f += " AND B.ToplantiId=@ToplantiId";

            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,B.Bilgi,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE A.BitisTarihi>=@From AND A.BaslangicTarihi<=@To" + f + " ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");

            q.AddParameter("@From", from);
            q.AddParameter("@To", to);

            if (meeting.HasValue)
                q.AddParameter("@ToplantiId", meeting.Value);

            return Select(q);
        }

        public DataTable SelectByParticipant(int id)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");
            q.AddParameter("@Id", id);
            return Select(q);
        }

        public DataTable SelectByParticipant(int id, PortalLogContext context)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");
            q.AddParameter("@Id", id);
            return PortalSqlReadExecutor.Select(q, context, "ToplantiRepository.SelectByParticipant");
        }

        public DataTable SelectByParticipantDate(int id, DateTime from, DateTime to)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id AND (A.BaslangicTarihi<=@To AND A.BitisTarihi>=@From) ORDER BY A.BaslangicTarihi");
            q.AddParameter("@Id", id);
            q.AddParameter("@From", from);
            q.AddParameter("@To", to);
            return Select(q);
        }

        public DataTable SelectByParticipantDate(int id, DateTime from, DateTime to, PortalLogContext context)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id AND (A.BaslangicTarihi<=@To AND A.BitisTarihi>=@From) ORDER BY A.BaslangicTarihi");
            q.AddParameter("@Id", id);
            q.AddParameter("@From", from);
            q.AddParameter("@To", to);
            return PortalSqlReadExecutor.Select(q, context, "ToplantiRepository.SelectByParticipantDate");
        }
    }
}
