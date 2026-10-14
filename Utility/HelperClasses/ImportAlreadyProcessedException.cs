using System;

namespace Utility.HelperClasses
{
    public sealed class ImportAlreadyProcessedException : Exception
    {
        public ImportAlreadyProcessedException(string message) : base(message) { }
        public ImportAlreadyProcessedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
