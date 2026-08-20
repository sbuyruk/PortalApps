namespace DAO.Ortak
{
    public static class DBProcess
    {
        public static string getConnectString()
        {
            //return Global.ConnectionString;
            return ConnectionString;
        }
        /// <SB>
        /// ConnectionString, calisan SharePoint web uygulamasinin web.config dosyasindan okunur
        /// (or: C:\inetpub\wwwroot\wss\VirtualDirectories\<port>\web.config).
        /// Her ortamda (test/production) 'TSKGV_Connection' adiyla tanimli olmalidir; bulunamazsa hata firlatilir.
        /// TESTDB için bu TSKGV-DEV3 içinde C:\inetpub\wwwroot\wss\VirtualDirectories\80
        /// Production'da bu TSKGV-SQL içinde aynı yerde
        /// </SB>
        private const string CONNECTION_STRING_NAME = "TSKGV_Connection";
        private static string _ConnectionString;
        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_ConnectionString))
                    return _ConnectionString;

                System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
                System.Configuration.ConnectionStringSettings connString;

                if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
                {
                    connString = rootWebConfig.ConnectionStrings.ConnectionStrings[CONNECTION_STRING_NAME];
                    if (connString != null)
                    {
                        _ConnectionString = connString.ConnectionString;
                    }

                }
                if (string.IsNullOrEmpty(_ConnectionString))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
                        string.Format("'{0}' baglanti dizesi web.config icinde bulunamadi. " +
                        "Lutfen <connectionStrings> bolumune '{0}' adiyla gecerli bir baglanti dizesi ekleyin.",
                        CONNECTION_STRING_NAME));
                }
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }


        }


    }
}
