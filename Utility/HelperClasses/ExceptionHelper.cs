using System;
using System.Collections.Generic;
using System.Text;
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

        private string GetAllMessages(Exception ex)
        {


            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                LogHelper.WriteTrace(ex);//Sharepoint log larina yaziyor.

                if (!string.IsNullOrEmpty(ex.Message))
                {
                    if (sb.Length > 0)
                        sb.Append(" ");

                    sb.Append("<br>");
                    sb.Append(RemoveQuote(ex.Message));
                    sb.Append("<br>");
                    sb.Append(RemoveQuote(ex.StackTrace));
                }

                ex = ex.InnerException;
            }

            return sb.ToString();
        }
        private static string RemoveQuote(string value)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("\""))
                {
                    result = value.Replace("\"", "");
                }
                else
                {
                    result = value;
                }
            }

            return result;
        }
        private string GetExceptionMessageAsHtml()
        {
            StringBuilder alertHtml = new StringBuilder();
            int count = 0;
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
                    alertHtml.Append(RemoveQuote(exceptionItem.Message));
                    alertHtml.Append("</b>");
                    alertHtml.Append("</div>");

                    alertHtml.Append(string.Format("<br><a href=\'#\' onclick=ToggleItem(\'Detail_{0}\')>Hata Detay</a>", count));

                    alertHtml.Append(string.Format("<br><div class=\'exceptionPanelDetail\' id=\'Detail_{0}\'>", count));

                    alertHtml.Append(GetAllMessages(exceptionItem));

                    alertHtml.Append("</div>");

                    alertHtml.Append("</div>");

                    count++;
                }
                alertHtml.Append("</div>");
            }
            return alertHtml.ToString();
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
