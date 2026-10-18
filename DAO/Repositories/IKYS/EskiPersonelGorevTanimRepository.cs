using DAO.Ortak;
using System;
using System.Data;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Repositories.IKYS
{
    public class EskiPersonelGorevTanimRepository
    {
        private readonly DbClass db;
        private readonly CrudQueryBuilder queryBuilder;

        public EskiPersonelGorevTanimRepository() : this(new DbClass()) { }
        public EskiPersonelGorevTanimRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
            queryBuilder = new CrudQueryBuilder();
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAll() { return db.SelectFromDb(new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table"), ""); }
        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "EskiPersonelGorevTanim_Table")); }
        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "EskiPersonelGorevTanim_Table")); }
        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("EskiPersonelGorevTanim_Table", id), ""); }
        public DataTable SelectById(int id, PortalLogContext context) { return TraceSelect("EskiPersonelGorevTanim.SelectById", () => SelectById(id), context); }
        public DataTable SelectById(int id, SqlTransactionContext transaction, PortalLogContext context) { if(transaction==null) throw new ArgumentNullException("transaction"); SqlQuery q=new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id"); q.AddParameter("@Id",id); Stopwatch clock=Stopwatch.StartNew(); bool ok=false; try { DataTable result=transaction.Select(q); ok=true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context,"SELECT","EskiPersonelGorevTanim.SelectById",clock.ElapsedMilliseconds,ok); } }
        public DataTable SelectAll(PortalLogContext context) { return TraceSelect("EskiPersonelGorevTanim.SelectAll", () => SelectAll(), context); }
        public int Insert<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return TraceInsert(queryBuilder.BuildInsert(item,"EskiPersonelGorevTanim_Table"),transaction,context); }
        public bool Update<T>(T item, SqlTransactionContext transaction, PortalLogContext context) { return TraceWrite(queryBuilder.BuildUpdate(item,"EskiPersonelGorevTanim_Table"),"UPDATE","EskiPersonelGorevTanim.Update",transaction,context); }
        public bool Delete(int id, SqlTransactionContext transaction, PortalLogContext context) { return TraceWrite(queryBuilder.BuildDelete("EskiPersonelGorevTanim_Table",id),"DELETE","EskiPersonelGorevTanim.Delete",transaction,context); }
        public DataTable SelectByPersonelId(int personelId, PortalLogContext context) { return TraceSelect("EskiPersonelGorevTanim.SelectByPersonelId", () => SelectByPersonelId(personelId), context); }
        public DataTable SelectByBirimId(int birimId, PortalLogContext context) { return TraceSelect("EskiPersonelGorevTanim.SelectByBirimId", () => SelectByBirimId(birimId), context); }
        public DataTable SelectAllReturnDataTable(int? personelTipi, PortalLogContext context) { return TraceSelect("EskiPersonelGorevTanim.SelectAllReturnDataTable", () => SelectAllReturnDataTable(personelTipi), context); }
        private DataTable TraceSelect(string name, Func<DataTable> action, PortalLogContext context) { Stopwatch clock=Stopwatch.StartNew(); bool ok=false; try { DataTable result=action(); ok=true; return result; } finally { PortalTechnicalLog.WriteSqlTrace(context,"SELECT",name,clock.ElapsedMilliseconds,ok); } }
        private static int TraceInsert(SqlQuery q, SqlTransactionContext transaction, PortalLogContext context) { if(transaction==null) throw new ArgumentNullException("transaction"); Stopwatch clock=Stopwatch.StartNew(); bool ok=false; try { int id=transaction.Insert(q); ok=id>0; return id; } finally { PortalTechnicalLog.WriteSqlTrace(context,"INSERT","EskiPersonelGorevTanim.Insert",clock.ElapsedMilliseconds,ok); } }
        private static bool TraceWrite(SqlQuery q,string operation,string name,SqlTransactionContext transaction,PortalLogContext context) { if(transaction==null) throw new ArgumentNullException("transaction"); Stopwatch clock=Stopwatch.StartNew(); bool ok=false; try { ok=transaction.Update(q); return ok; } finally { PortalTechnicalLog.WriteSqlTrace(context,operation,name,clock.ElapsedMilliseconds,ok); } }

        public DataTable SelectByPersonelId(int personelId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE PersonelId=@PersonelId ORDER BY Id");
            query.AddParameter("@PersonelId", personelId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByBirimId(int birimId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM EskiPersonelGorevTanim_Table WHERE BirimId=@BirimId ORDER BY Id");
            query.AddParameter("@BirimId", birimId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllReturnDataTable(int? personelTipi)
        {
            string personelFilter = personelTipi.HasValue ? " AND B.Tipi=@Tipi" : "";
            string sql = @"
                SELECT A.Id EskiPersonelGorevTanimId, A.Adi GorevAdi, A.KisaAdi GorevKisaAdi, C.Sira,
                       ISNULL(B.Adi,'') + ' ' + ISNULL(B.Soyadi,'') Personel,
                       C.Adi BirimAdi
                FROM EskiPersonelGorevTanim_Table A
                LEFT JOIN Personel_Table B ON B.Id=A.PersonelId" + personelFilter + @"
                LEFT JOIN BirimTanim_Table C ON C.Id=A.BirimId
                WHERE A.Id>0
                ORDER BY C.Sira,A.Id";
            SqlQuery query = new SqlQuery(sql);
            if (personelTipi.HasValue) query.AddParameter("@Tipi", personelTipi.Value);
            return db.SelectFromDb(query, "");
        }
    }
}
