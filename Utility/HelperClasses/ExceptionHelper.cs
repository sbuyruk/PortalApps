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
