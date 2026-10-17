using System;

namespace Utility.Logging
{
    /// <summary>
    /// Shared event envelope. Do not place SQL parameters, entity snapshots or personal data in Name.
    /// Audit field allowlists and database persistence belong to the project migration stage.
    /// </summary>
    public sealed class PortalLogRecord
    {
        public PortalLogKind Kind { get; private set; }
        public PortalLogContext Context { get; private set; }
        public DateTime OccurredUtc { get; private set; }
        public string Name { get; private set; }
        public string Outcome { get; private set; }
        public long? DurationMilliseconds { get; private set; }

        public PortalLogRecord(PortalLogKind kind, PortalLogContext context, string name,
            string outcome, long? durationMilliseconds = null)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Olay adı gereklidir.", "name");
            if (durationMilliseconds < 0) throw new ArgumentOutOfRangeException("durationMilliseconds", "Süre negatif olamaz.");
            Kind = kind;
            Context = context;
            Name = name;
            Outcome = outcome ?? string.Empty;
            DurationMilliseconds = durationMilliseconds;
            OccurredUtc = DateTime.UtcNow;
        }
    }
}
