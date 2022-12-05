using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace DAO.Ortak
{
    [Serializable]
    public class DbClass
    {
        string created = string.Empty;
        public List<DBObject> DBObjectList = new List<DBObject>();

        public int Insert(string sqlString)
        {
            int id = 0;

            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlString + ";SELECT SCOPE_IDENTITY()", con);
                        id = cmd.ExecuteScalar().ConvertToInt();
                        con.Close();
                        SorguyuLogla("INSERT", true, sqlString,"Id="+id);
                    }
                }
                catch (SqlException e)
                {
                    SorguyuLogla("INSERT", false, sqlString,e.Message);
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

            return id;
        }
        //public bool Update2Db(string sqlString, string identifier)
        //{
        //    bool isSaved = false;

        //    string connectString = DBProcess.getConnectString();
        //    SqlConnection con = new SqlConnection(connectString);
        //    using (con)
        //    {
        //        try
        //        {
        //            if (con.State == ConnectionState.Closed)
        //            {
        //                con.Open();
        //                //ShowMessage(1,"Taşınmaz", "Veri Tabanına Bağlandı.");
        //                SqlCommand cmd = new SqlCommand(sqlString, con);
        //                cmd.ExecuteNonQuery();
        //                //  Loga yaz      SonucLbl.Text = "Kaydedildi!";
        //                isSaved = true;
        //                con.Close();
        //                SorguyuLogla("UPDATE", false, sqlString,string.Empty);
        //            }
        //        }
        //        catch (SqlException e)
        //        {
        //            SorguyuLogla("UPDATE", false, sqlString, e.Message);
        //            isSaved = false;
        //            throw e;
        //        }
        //        catch (Exception e)
        //        {
        //            throw e;
        //        }
        //        return isSaved;
        //    }
        //}
        public bool Update2Db(string sqlString)
        {
            bool isUpdated = false;

            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        //ShowMessage(1,"Taşınmaz", "Veri Tabanına Bağlandı.");
                        SqlCommand cmd = new SqlCommand(sqlString, con);
                        cmd.ExecuteNonQuery();
                        //  Loga yaz      SonucLbl.Text = "Kaydedildi!";
                        isUpdated = true;
                        con.Close();
                        SorguyuLogla("UPDATE", true, sqlString, string.Empty);
                    }
                }
                catch (Exception e)
                {
                    isUpdated = false;
                    SorguyuLogla("UPDATE", false, sqlString, e.Message);
                    throw e;
                }
                return isUpdated;
            }
        }
        public bool DeleteFromDb(string sqlString, string aciklama)
        {
            bool isDeleted = false;
            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlString, con);
                        cmd.ExecuteNonQuery();
                        isDeleted = true;
                        con.Close();
                        SorguyuLogla("DELETE", true, sqlString, aciklama);
                    }
                }

                catch (Exception e)
                {
                    isDeleted = false;
                    throw e;
                }

                return isDeleted;
            }
        }
        public int DeleteFromDb(string sqlString, string aciklama, bool dummy)
        {
            int deleted = 0;
            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlString, con);
                        deleted = cmd.ExecuteNonQuery();
                        con.Close();
                        SorguyuLogla("DELETE", true, sqlString, aciklama);
                    }
                }
                catch (Exception e)
                {
                    deleted = 0;
                    throw e;
                }

                return deleted;
            }
        }
        public bool DeleteFromDb<T>(string sqlString, T objectToDelete)
        {
            string eskiDeger = "Silinen Kayıt: " +GetEskiDeger(objectToDelete);
            bool isDeleted = false;
            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlString, con);
                        cmd.ExecuteNonQuery();
                        isDeleted = true;
                        con.Close();
                        SorguyuLogla("DELETE", true, sqlString, eskiDeger);
                    }
                }

                catch (Exception e)
                {
                    SorguyuLogla("DELETE", false, sqlString, e.Message);
                    isDeleted = false;
                    throw e;
                }

                return isDeleted;
            }
        }
        private string GetEskiDeger<T>(T objectToDelete)
        {
            Type type = typeof(T);
            StringBuilder sbQry = new StringBuilder();
            System.Reflection.PropertyInfo[] propInfo = type.GetProperties();
            foreach (System.Reflection.PropertyInfo pi in propInfo)
            {
                string value = objectToDelete.GetType().GetProperty(pi.Name).GetValue(objectToDelete, null) == null ? string.Empty : objectToDelete.GetType().GetProperty(pi.Name).GetValue(objectToDelete, null).ToString();
                if (sbQry.ToString() == string.Empty)
                {
                    if (!string.IsNullOrEmpty(value))
                        sbQry.AppendFormat("{0}={1}", pi.Name, value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                        sbQry.AppendFormat(",{0}={1}", pi.Name, value);
                }
                    
            }
            return sbQry.ToString();
        }

        public DataTable selectFromDb(string sqlString, string identifier)
        {
            DataTable dataTable = new DataTable();
            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    SqlCommand cmd = new SqlCommand(sqlString, con);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter dataReader = new SqlDataAdapter();
                    dataReader.SelectCommand = cmd;
                    DataSet dataSet = new DataSet();
                    dataReader.Fill(dataSet);
                    dataTable = dataSet.Tables[0];
                    con.Close();
                    if (dataTable.Rows.Count == 0)
                        return null;
                    else
                        return dataTable;
                }
                catch (SqlException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }
        private SqlTransaction DBTransaction;
        public List<DBObject> ExecuteTransaction()
        {
            string connectString = DBProcess.getConnectString();
            SqlConnection dBConnection = new SqlConnection(connectString);
            using (dBConnection)
            {
                try
                {
                    if (dBConnection.State == ConnectionState.Closed)
                    {
                        dBConnection.Open();

                        DBTransaction = dBConnection.BeginTransaction();

                        foreach (DBObject item in DBObjectList)
                        {
                            string sqlString = item.SQLString;
                            int sqlType = item.SQLType;
                            switch (sqlType)
                            {
                                case ProjeConstants.SQL_INSERT:
                                    {
                                        if (item.UseReturnIdAsParam)
                                        {
                                            string pval = ((DBObject)DBObjectList[item.DbObjectParamIndex]).ReturnId.ToString();
                                            sqlString = string.Format(sqlString, pval);
                                        }
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        //cmd.ExecuteNonQuery();
                                        int id = cmd.ExecuteScalar().ConvertToInt();
                                        item.ReturnId = id;
                                        item.Success = true;
                                        SorguyuLogla("INSERT", true, sqlString, "Id=" + id );
                                        break;
                                    }
                                case ProjeConstants.SQL_UPDATE:
                                    {
                                        if (item.UseReturnIdAsParam)
                                        {
                                            string pval = ((DBObject)DBObjectList[item.DbObjectParamIndex]).ReturnId.ToString();
                                            sqlString = string.Format(sqlString, pval);
                                        }
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        item.RowsAffected = cmd.ExecuteNonQuery();
                                        item.Success = true;
                                        SorguyuLogla("UPDATE", true, sqlString, string.Empty);
                                        break;
                                    }
                                case ProjeConstants.SQL_DELETE:
                                    {
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        item.RowsAffected = cmd.ExecuteNonQuery();
                                        item.Success = true;
                                        SorguyuLogla("DELETE", true, sqlString, "RowsAffected="+ item.RowsAffected);
                                        break;
                                    }
                                case ProjeConstants.SQL_SELECT:
                                    {
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        item.RowsAffected = cmd.ExecuteNonQuery();
                                        item.Success = true;
                                        break;
                                    }
                            }
                        }
                    }
                    DBTransaction.Commit();
                }
                catch (Exception )
                {
                    DBTransaction.Rollback();

                    foreach (DBObject item in DBObjectList)
                    {
                        item.Success = false;
                    }
                    //throw e;
                }
                finally
                {
                    dBConnection.Close();
                }
                return DBObjectList;
            }
        }
        //private SqlTransaction DBTransaction {
        //    get;
        //    set;
        //}
        //private List<string> QueryList= new List<string>();
        //public SqlTransaction StartTransaction()
        //{
        //    DBTransaction=DBConnection.BeginTransaction();
        //    QueryList.Clear();
        //    return DBTransaction;

        //}
        //public bool EndTransaction()
        //{
        //    bool isSaved = false;
        //    using (DBConnection)
        //    {
        //        try
        //        {
        //            if (DBConnection.State == ConnectionState.Closed)
        //            {
        //                DBConnection.Open();

        //                foreach (string sqlString in QueryList)
        //                {
        //                    SqlCommand cmd = new SqlCommand(sqlString, DBConnection);
        //                    cmd.Transaction = DBTransaction;
        //                    cmd.ExecuteNonQuery();
        //                    isSaved = true;
        //                }

        //                DBConnection.Close();

        //            }
        //            if (isSaved)
        //                DBTransaction.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            isSaved = false;
        //            DBTransaction.Rollback();
        //            throw e;
        //        }
        //        return isSaved;
        //    }
        //}
        //private SqlConnection DBConnection = new SqlConnection(DBProcess.getConnectString());
        public static void SorguyuLogla(string komut, bool basarili, string sorgu, string aciklama)
        {
            string aciklamaStr = aciklama.Length > 250 ? aciklama.Substring(0, 249).Trim() : aciklama.Trim();
            sorgu = sorgu.Replace("'", "").Trim();
            string sorguStr = sorgu.Length > 1000 ? sorgu.Substring(0, 999): sorgu;
            string sqlString = string.Format(@"
                INSERT INTO SQLLog_Table (Kullanici, IslemTarihi,Komut,Tablo,Basarili,Sorgu,Aciklama)
                VALUES({0},{1},{2},{3},{4},{5},{6})", 
                    "'" + Global.GetCurrentUser() + "'", "'" + DateTime.Now + "'", "'" + komut + "'", "'"+Global.FindTable(sorgu) + "'", 
                    "'"+basarili + "'", "'" + sorguStr + "'", "'"+ aciklamaStr+ "'");

            string connectString = DBProcess.getConnectString();
            SqlConnection con = new SqlConnection(connectString);
            using (con)
            {
                int id = 0;
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlString + ";SELECT SCOPE_IDENTITY()", con);
                        id = cmd.ExecuteScalar().ConvertToInt();
                        con.Close();
                    }
                }
                catch (SqlException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

        }
    }
}
