using System;
using System.Collections.Generic;
using System.Web;

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

        private string GetExceptionMessageAsHtml()
        {
            var alertHtml = new System.Text.StringBuilder();
            if (this.Exceptions.Count > 0)
            {
                alertHtml.Append("<div id=\'exceptionAll\'>");
                foreach (Exception exceptionItem in this.Exceptions)
                {
                    string alertClass = exceptionItem is ImportAlreadyProcessedException
                        ? "alert alert-warning" : "alert alert-danger";
                    alertHtml.Append("<div class=\'" + alertClass + "\' role=\'alert\'>");

                    alertHtml.Append("<div class=\'exceptionMessage\'>");
                    alertHtml.Append("<b>");
                    alertHtml.Append(HttpUtility.HtmlEncode(GetUserMessage(exceptionItem)));
                    alertHtml.Append("</b>");
                    alertHtml.Append("</div>");
                    alertHtml.Append("</div>");

                }
                alertHtml.Append("</div>");
            }
            return alertHtml.ToString();
        }
        private static string GetUserMessage(Exception exception)
        {
            for (Exception detail = exception; detail != null; detail = detail.InnerException)
                LogHelper.WriteTrace(detail);

            return exception is ImportAlreadyProcessedException
                ? exception.Message
                : "İşlem tamamlanamadı. Lütfen yeniden deneyin.";
        }
        public void PublishException()
        {
            var messageAsHtml = GetExceptionMessageAsHtml();
            if (!string.IsNullOrEmpty(messageAsHtml))
            {
                MessageHelper.PublishMessage(messageAsHtml, "");
            }
        }
        public void PublishImportExceptions()
        {
            foreach (Exception error in Exceptions)
            {
                for (Exception detail = error; detail != null; detail = detail.InnerException)
                    LogHelper.WriteTrace(detail);

                string message = HttpUtility.HtmlEncode(error.Message ?? "Dosya işlenemedi.");
                string css = error is ImportAlreadyProcessedException ? "alert-warning" : "alert-danger";
                MessageHelper.PublishMessage(message, css);
            }
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
