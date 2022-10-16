using System;
using System.Text;

namespace Utility.HelperClasses
{
    public static class MessageHelper
    {
        private static void RegisterMessage(string message, string fadeOutString)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var classId = new Random().Next(10, 1000);
                var className = string.Format("customMessage_{0}", classId.ToString());
                message = message.Replace(Environment.NewLine, "<br>");

                message = string.Format("<div class='container'><div class='d-flex justify-content-center'><div class='{0} snackbar'><div class='card shadow'><div class='card-header'><span class='close' onclick=CloseMessage('{0}')>×</span></div><div class='card-body customMessageBody'>{1}</div></div></div></div></div>", className, message);

                var messageHtml = string.Format("$('body').append({0});", "\"" + message + "\"");

                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), messageHtml, true);
                if (!string.IsNullOrEmpty(fadeOutString))
                {
                    fadeOutString = string.Format("$('.{0}').{1};", className, fadeOutString);
                }

                var showScript = string.Format("$('.{0}').hide();SnackBarOpen('{0}');{1}", className, fadeOutString);

                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), showScript, true);
            }
        }
        public static void PublishMessage(string message, string messageType)
        {
            CreateMessageBody(message, messageType, string.Empty);
        }
        /// <summary>
        /// Ekrana mesaj basar
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="messageType">ProjeConstants.MESAJ_HATA,ProjeConstants.MESAJ_BASARILI ,ProjeConstants.MESAJ_BILGI</param>
        /// <param name="fadeOutTime">kaç milisaniye sonra kapanacağını belirler. boş bırakılırsa hiç kapanmaz. Örnek 2000 girilirse 2sn sonra kapanır.</param>
        public static void PublishMessage(string message, string messageType, int fadeOutTime)
        {
            string fadeOutString = string.Empty;
            if (fadeOutTime > 0)
            {
                fadeOutString = string.Format("fadeOut({0})", fadeOutTime.ToString());
            }
            CreateMessageBody(message, messageType, fadeOutString);
        }

        private static void CreateMessageBody(string message, string messageType, string fadeOutString)
        {
            StringBuilder alertHtml = new StringBuilder();

            alertHtml.Append(string.Format("<div class='alert {0}' >", messageType));
            alertHtml.Append(message);
            alertHtml.Append("</div>");

            RegisterMessage(alertHtml.ToString(), fadeOutString);
        }
    }
}
