using System;

namespace Utility.HelperClasses
{
    /// <summary>
    /// Eski çağrılar için uyumluluk katmanı. Yeni kodda Messages.Success/Info/Warning/Error kullanılır.
    /// </summary>
    public static class MessageHelper
    {
        public static void PublishMessage(string message, string messageType)
        {
            PublishMessage(message, messageType, 0);
        }

        /// <summary>
        /// Türkçe düz metin gösterir. Pozitif süre mesaj gösterildikten sonraki bekleme
        /// süresidir; sıfır veya negatif değer mesajı kullanıcı kapatana kadar açık tutar.
        /// </summary>
        public static void PublishMessage(string message, string messageType, int fadeOutTime)
        {
            int duration = Math.Max(0, fadeOutTime);
            switch (messageType)
            {
                case "alert-danger": Messages.Error(message, duration); break;
                case "alert-success": Messages.Success(message, duration); break;
                case "alert-warning": Messages.Warning(message, duration); break;
                default: Messages.Info(message, duration); break;
            }
        }
    }
}
