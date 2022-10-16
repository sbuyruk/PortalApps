namespace DAO.Ortak
{
    public static class DBProcess
    {
        public static string getConnectString()
        {
            //return Global.ConnectionString;
            return ConnectionString;
        }
        private static string _ConnectionStringDefaultValue = "Server=.;Database=TESTDB;Trusted_Connection=True;";
        private static string _ConnectionString;
        public static string ConnectionString
        {
            get
            {
                System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
                System.Configuration.ConnectionStringSettings connString;

                if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
                {
                    connString = rootWebConfig.ConnectionStrings.ConnectionStrings["TSKGV_Connection"];
                    if (connString != null)
                    {
                        _ConnectionString = connString.ConnectionString;
                    }

                }
                if (string.IsNullOrEmpty(_ConnectionString))
                {
                    _ConnectionString = _ConnectionStringDefaultValue;
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
