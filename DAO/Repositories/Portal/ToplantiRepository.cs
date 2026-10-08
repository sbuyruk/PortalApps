using DAO.Ortak;
using System;
using System.Data;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Portal
{
    public class ToplantiRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder b;

        public ToplantiRepository()
            : this(new DbClass())
        {
        }

        public ToplantiRepository(DbClass db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            this.db = db;
            b = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE Id=@Id");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery("SELECT * FROM Toplanti_Table ORDER BY ToplantiKonusu"), "");
        }

        public DataTable SelectByYeri(int id)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri=@Yeri");
            q.AddParameter("@Yeri", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByStart(int y, DateTime d)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri=@Yeri AND BaslangicTarihi<=@Tarih AND BitisTarihi>=@Tarih");
            q.AddParameter("@Yeri", y);
            q.AddParameter("@Tarih", d);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByEnd(int y, DateTime d)
        {
            var q = new SqlQuery("SELECT * FROM Toplanti_Table WHERE ToplantiYeri!=@Diger AND ToplantiYeri=@Yeri AND BitisTarihi>=@Tarih AND BaslangicTarihi<=@Tarih");
            q.AddParameter("@Diger", ProjeConstants.PARAM_DIGER_INT);
            q.AddParameter("@Yeri", y);
            q.AddParameter("@Tarih", d);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByParticipantMeeting(int id, string aktif, int? meeting)
        {
            string f = aktif == ProjeConstants.TOPLANTI_AKTIF ? " AND A.BitisTarihi>GETDATE()" : aktif == ProjeConstants.TOPLANTI_PASIF ? " AND A.BitisTarihi<GETDATE()" : "";

            if (meeting.HasValue)
                f += " AND B.ToplantiId=@ToplantiId";

            var q = new SqlQuery("SELECT A.Id ToplantiId,A.*,B.KatilimciId,B.Id KatilimId,B.Bilgi,C.Adi,C.Soyadi,D.ProtokolSiraNo FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE 1=1" + f + " ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");

            if (meeting.HasValue)
                q.AddParameter("@ToplantiId", meeting.Value);

            return db.SelectFromDb(q, "");
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

            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByParticipant(int id)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id ORDER BY A.Id,ProtokolSiraNo,Adi,Soyadi");
            q.AddParameter("@Id", id);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectByParticipantDate(int id, DateTime from, DateTime to)
        {
            var q = new SqlQuery("SELECT C.Adi,C.Soyadi,B.KatilimciId,A.Id ToplantiId,B.Id KatilimId,D.ProtokolSiraNo,A.* FROM Toplanti_Table A LEFT JOIN ToplantiKatilim_Table B ON A.Id=B.ToplantiId LEFT JOIN Personel_Table C ON C.Id=B.KatilimciId LEFT JOIN IsBilgileri_Table D ON D.PersonelId=C.Id WHERE B.KatilimciId=@Id AND (A.BaslangicTarihi<=@To AND A.BitisTarihi>=@From) ORDER BY A.BaslangicTarihi");
            q.AddParameter("@Id", id);
            q.AddParameter("@From", from);
            q.AddParameter("@To", to);
            return db.SelectFromDb(q, "");
        }

        public int Insert<T>(T x)
        {
            return db.Insert(b.BuildInsert(x, "Toplanti_Table"));
        }

        public bool Update<T>(T x)
        {
            return db.Update2Db(b.BuildUpdate(x, "Toplanti_Table"));
        }

        public bool Delete(int id)
        {
            return db.DeleteFromDb(b.BuildDelete("Toplanti_Table", id), "");
        }
    }
}
