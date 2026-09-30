using Model.NBYS;
using System;
using System.IO;
using Utility.HelperClasses;

namespace Model.Services.NBYS
{
    /// <summary>
    /// Banka dosyalarindan ekstre kaydi olusturan import islemlerinin giris noktasi.
    /// Geriye uyumluluk gecisi tamamlanana kadar mevcut parser uygulamalarina yonlendirir.
    /// </summary>
    public static class EkstreAktarmaImportService
    {
        public static ExceptionHelper SaveAkBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveAkBankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveAkBankEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveAkBankEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveFinansbankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveFinansbankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveFinansbankEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveFinansbankEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveGarantiBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveGarantiBankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveGarantiEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveGarantiEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveTEBBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveTEBBankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveHalkbankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveHalkbankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveHalkbank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveHalkbank2File(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveIsBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveIsBankFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveIsBankEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveIsBankEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveYKBEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveYKBEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveVakifBankGunlukTextFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveVakifBankGunlukTextFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveVakifBank2File(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveVakifKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveVakifKatilimFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveZiraatFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveZiraatFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveZiraatEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveZiraatEkstreFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveZiraatKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveZiraatKatilimFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveEDevletFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveEDevletFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveAlbarakaFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveAlbarakaFile(fileStream, processTime, currentUser);
        }

        public static ExceptionHelper SaveSMSVakifFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarma.SaveSMSVakifFile(fileStream, processTime, currentUser);
        }
    }
}
