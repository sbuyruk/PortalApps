using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.HelperClasses
{
    public class ExceptionHelper
    {
        //public int CountSuccess { get; set; }
        //public int CountError { get; set; }

        public List<Exception> Exceptions = new List<Exception>();
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
                LogHelper.WriteTrace(ex);//Sharepoint log larına yazıyor.

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
                    alertHtml.Append("<div class=\'alert alert-danger\' role=\'alert\'>");

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
        public bool HasException()
        {
            return Exceptions.Count > 0;

        }
    }

}
