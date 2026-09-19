using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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

                        object scalar = cmd.ExecuteScalar();
                        id = scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);

                        con.Close();
                        if (ProjeConstants.GENEL_SAVE_LOG)
                            SorguyuLogla("INSERT", true, sqlString, "Id=" + id);
                    }
                }
                catch (SqlException e)
                {
                    SorguyuLogla("INSERT", false, sqlString, e.Message);
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return id;
        }
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
                        SqlCommand cmd = new SqlCommand(sqlString, con);
                        cmd.ExecuteNonQuery();
                        isUpdated = true;
                        con.Close();
                        if (ProjeConstants.GENEL_UPDATE_LOG) 
                            SorguyuLogla("UPDATE", true, sqlString, string.Empty);
                    }
                }
                catch (Exception e)
                {
                    isUpdated = false;
                    SorguyuLogla("UPDATE", false, sqlString, e.Message);
                    throw;
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
                        if (ProjeConstants.GENEL_DELETE_LOG) 
                            SorguyuLogla("DELETE", true, sqlString, aciklama);
                    }
                }

                catch (Exception e)
                {
                    SorguyuLogla("DELETE", false, sqlString, e.Message);
                    isDeleted = false;
                    throw;
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
                        if (ProjeConstants.GENEL_DELETE_LOG) 
                            SorguyuLogla("DELETE", true, sqlString, aciklama);
                    }
                }
                catch (Exception e)
                {
                    deleted = 0;
                    throw;
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
                        if (ProjeConstants.GENEL_DELETE_LOG) 
                            SorguyuLogla("DELETE", true, sqlString, eskiDeger);
                    }
                }

                catch (Exception e)
                {
                    SorguyuLogla("DELETE", false, sqlString, e.Message);
                    isDeleted = false;
                    throw;
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

        public DataTable SelectFromDb(string sqlString, string identifier)
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
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        #region Parametreli sorgu overload'ları (SQL injection korumalı)

        /// <summary>
        /// Verilen sorguları tek bir SqlTransaction içinde sırayla çalıştırır.
        /// Herhangi biri hata verirse tümü geri alınır (rollback) ve hata fırlatılır.
        /// </summary>
        public void ExecuteTransaction(List<SqlQuery> queries)
        {
            if (queries == null || queries.Count == 0)
                return;

            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    SqlQuery current = null;
                    try
                    {
                        foreach (SqlQuery query in queries)
                        {
                            current = query;
                            using (SqlCommand cmd = new SqlCommand(query.Sql, con, transaction))
                            {
                                cmd.Parameters.AddRange(query.Parameters.ToArray());
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        SorguyuLogla("TRANSACTION", false, current != null ? current.ToLogString() : string.Empty, e.Message);
                        throw;
                    }
                }
            }
        }

        public int Insert(SqlQuery query)
        {
            int id = 0;
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql + ";SELECT SCOPE_IDENTITY()", con))
            {
                try
                {
                    cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    object scalar = cmd.ExecuteScalar();
                    id = scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
                    if (ProjeConstants.GENEL_SAVE_LOG)
                        SorguyuLogla("INSERT", true, query.ToLogString(), "Id=" + id);
                }
                catch (Exception e)
                {
                    SorguyuLogla("INSERT", false, query.ToLogString(), e.Message);
                    throw;
                }
            }
            return id;
        }

        public bool Update2Db(SqlQuery query)
        {
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            {
                try
                {
                    cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    if (ProjeConstants.GENEL_UPDATE_LOG)
                        SorguyuLogla("UPDATE", true, query.ToLogString(), string.Empty);
                    return true;
                }
                catch (Exception e)
                {
                    SorguyuLogla("UPDATE", false, query.ToLogString(), e.Message);
                    throw;
                }
            }
        }

        public bool DeleteFromDb(SqlQuery query, string aciklama)
        {
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            {
                try
                {
                    cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    if (ProjeConstants.GENEL_DELETE_LOG)
                        SorguyuLogla("DELETE", true, query.ToLogString(), aciklama);
                    return true;
                }
                catch (Exception e)
                {
                    SorguyuLogla("DELETE", false, query.ToLogString(), e.Message);
                    throw;
                }
            }
        }

        public bool DeleteFromDb<T>(SqlQuery query, T objectToDelete)
        {
            string eskiDeger = "Silinen Kayıt: " + GetEskiDeger(objectToDelete);
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            {
                try
                {
                    cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    if (ProjeConstants.GENEL_DELETE_LOG)
                        SorguyuLogla("DELETE", true, query.ToLogString(), eskiDeger);
                    return true;
                }
                catch (Exception e)
                {
                    SorguyuLogla("DELETE", false, query.ToLogString(), e.Message);
                    throw;
                }
            }
        }

        public DataTable SelectFromDb(SqlQuery query, string identifier)
        {
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddRange(query.Parameters.ToArray());
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                DataTable dataTable = dataSet.Tables[0];
                if (dataTable.Rows.Count == 0)
                    return null;
                return dataTable;
            }
        }

        #endregion

        private SqlTransaction DBTransaction;
        public List<DBObject> ExecuteTransaction()
        {
            string connectString = DBProcess.getConnectString();
            SqlConnection dBConnection = new SqlConnection(connectString);
            using (dBConnection)
            {
                string sqlString = string.Empty;
                int sqlType = ProjeConstants.SQL_BOS;
                try
                {
                    if (dBConnection.State == ConnectionState.Closed)
                    {
                        dBConnection.Open();

                        DBTransaction = dBConnection.BeginTransaction();

                        foreach (DBObject item in DBObjectList)
                        {
                            sqlString = item.SQLString.Replace(ProjeConstants.SQL_GENERIC_INT_VALUE.ToString(),"{"+ item.SQLStringParamIndex + "}");
                            sqlType = item.SQLType;
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
                                        if (ProjeConstants.GENEL_SAVE_LOG)
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
                                        if (ProjeConstants.GENEL_UPDATE_LOG)
                                            SorguyuLogla("UPDATE", true, sqlString, string.Empty);
                                        break;
                                    }
                                case ProjeConstants.SQL_DELETE:
                                    {
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        item.RowsAffected = cmd.ExecuteNonQuery();
                                        item.Success = true;
                                        if (ProjeConstants.GENEL_DELETE_LOG)
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
                catch (Exception ex )
                {
                    SorguyuLogla( "SQL_TYPE="+sqlType, false, sqlString, ex.Message);
                    MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
                    DBTransaction.Rollback();

                    foreach (DBObject item in DBObjectList)
                    {
                        item.Success = false;
                    }
                    throw;
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
        //            throw;
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
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }

            }

        }
    }
}
