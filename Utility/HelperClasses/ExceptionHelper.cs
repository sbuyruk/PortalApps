using System;
using System.Collections.Generic;

namespace Utility.HelperClasses
{
    public class ExceptionHelper
    {
        //public int CountSuccess { get; set; }
        //public int CountError { get; set; }

        public List<Exception> Exceptions = new List<Exception>();
        public int SuccessCount { get; set; }
        public int AlreadyProcessedCount { get; set; }
        public int SkippedCount { get; set; }
        public int ErrorCount { get; set; }
        public ExceptionHelper()
        {

        }
        public ExceptionHelper(Exception ex)
        {
            Exceptions.Add(ex);
        }

        /// <summary>Legacy presentation entry point; new WebParts map results at their UI boundary.</summary>
        public void PublishException()
        {
            foreach (Exception error in Exceptions)
            {
                if (error == null) continue;
                try
                {
                    // Exception.ToString() includes the inner exception chain.
                    LogHelper.WriteTrace(error);
                }
                catch (Exception)
                {
                    // A logging failure must not prevent a safe user notification.
                }
                if (error is ImportAlreadyProcessedException)
                    Messages.Warning(string.IsNullOrWhiteSpace(error.Message)
                        ? "Bu kayıt daha önce işlenmiş." : error.Message);
                else
                    Messages.Error("İşlem tamamlanamadı. Lütfen yeniden deneyin.");
            }
        }

        public void PublishImportExceptions()
        {
            // Keep the import API, but never disclose arbitrary exception.Message values.
            PublishException();
        }
        public bool HasException()
        {
            return Exceptions.Count > 0;

        }

        public bool HasFatalException()
        {
            foreach (Exception exception in Exceptions)
                if (!(exception is ImportAlreadyProcessedException)) return true;
            return false;
        }
    }

}
