using DAO.Repositories.NBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using System.Globalization;
using Utility.Logging;
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
    public enum NBYSParametreWriteResult { Succeeded, NotFound, Duplicate }

    public class NBYSParametreService
    {
        private readonly NBYSParametreRepository repository;
        private readonly PortalLogContext context;

        public NBYSParametreService(PortalLogContext context) : this(new NBYSParametreRepository(context))
        {
            this.context = context;
        }

        public NBYSParametreService() : this(new NBYSParametreRepository())
        {
        }

        public NBYSParametreService(NBYSParametreRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public NBYSParametre GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<NBYSParametre> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public List<NBYSParametre> GetByGroup(string group)
        {
            return Map(repository.SelectByGroup(group));
        }

        public NBYSParametre GetByGroupAndKey(string group, string key)
        {
            return Map(repository.SelectByGroupAndKey(group, key)).FirstOrDefault();
        }

        public NBYSParametre GetByGroupAndKey(string group, string key, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            return Map(repository.SelectByGroupAndKey(group, key, context)).FirstOrDefault();
        }

        public NBYSParametre GetByGroupKeyAndValue(string group, string key, string value)
        {
            return Map(repository.SelectByGroupKeyAndValue(group, key, value)).FirstOrDefault();
        }

        public NBYSParametreWriteResult SaveChecked(NBYSParametre item)
        {
            return Write(item, "INSERT");
        }

        public NBYSParametreWriteResult UpdateChecked(NBYSParametre item)
        {
            return Write(item, "UPDATE");
        }

        public NBYSParametreWriteResult DeleteChecked(NBYSParametre item)
        {
            return Write(item, "DELETE");
        }

        private NBYSParametreWriteResult Write(NBYSParametre item, string operation)
        {
            if (context == null) throw new InvalidOperationException("NBYS log bağlamı gereklidir.");
            if (item == null) throw new ArgumentNullException("item", "Parametre kaydı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                DataTable previous = null;
                if (operation != "INSERT")
                {
                    previous = repository.SelectById(item.Id, transaction);
                    if (previous == null || previous.Rows.Count == 0) return NBYSParametreWriteResult.NotFound;
                    NBYSParametre old = Map(previous).First();
                    // Group is not editable in this screen; retain the transaction's current value.
                    item.Grup = old.Grup;
                    item.Olusturan = old.Olusturan;
                    item.OlusturmaTarihi = old.OlusturmaTarihi;
                }
                if (operation != "DELETE")
                {
                    DataTable duplicate = repository.SelectDuplicate(item.Grup, item.Anahtar, item.Deger,
                        operation == "INSERT" ? 0 : item.Id, transaction);
                    if (duplicate != null && duplicate.Rows.Count > 0) return NBYSParametreWriteResult.Duplicate;
                }
                DataTable saved = null;
                if (operation == "INSERT")
                {
                    item.Olusturan = UtilityHelper.GetCurrentUserName();
                    item.OlusturmaTarihi = DateTime.Now;
                    item.Id = repository.Insert(item, transaction);
                    if (item.Id <= 0) throw new InvalidOperationException("Parametre kaydedilemedi.");
                }
                else if (operation == "UPDATE")
                {
                    item.Degistiren = UtilityHelper.GetCurrentUserName();
                    item.DegistirmeTarihi = DateTime.Now;
                    if (!repository.Update(item, transaction)) throw new InvalidOperationException("Parametre güncellenemedi.");
                }
                else if (!repository.Delete(item.Id, transaction))
                    throw new InvalidOperationException("Parametre silinemedi.");

                if (operation != "DELETE")
                {
                    saved = repository.SelectById(item.Id, transaction);
                    if (saved == null || saved.Rows.Count == 0)
                        throw new InvalidOperationException("Kaydedilen parametre okunamadı.");
                }
                RecordAudit(operation, item.Id, previous, saved, transaction);
                transaction.Complete();
            } // Dispose commits before the successful result reaches the UI.
            return NBYSParametreWriteResult.Succeeded;
        }

        private void RecordAudit(string operation, int entityId, DataTable previous, DataTable saved,
            SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "NBYSParametre",
                entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            // Anahtar/Deger are free text and may contain sensitive configuration.
            foreach (string field in new[] { "Sira" })
            {
                string oldValue = AuditValue(previous, field);
                string newValue = AuditValue(saved, field);
                if (oldValue != newValue)
                    log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0].IsNull(field)) return null;
            return Convert.ToString(table.Rows[0][field], CultureInfo.InvariantCulture);
        }
        private static List<NBYSParametre> Map(DataTable table)
        {
            return new NBYSParametre().ToList<NBYSParametre>(table);
        }
    }
}
