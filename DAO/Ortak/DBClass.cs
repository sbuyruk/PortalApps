using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
                    }
                }
                catch (Exception)
                {
                    isUpdated = false;
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
                    }
                }

                catch (Exception)
                {
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
                    }
                }
                catch (Exception)
                {
                    deleted = 0;
                    throw;
                }

                return deleted;
            }
        }
        public bool DeleteFromDb<T>(string sqlString, T objectToDelete)
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
                    }
                }

                catch (Exception)
                {
                    isDeleted = false;
                    throw;
                }

                return isDeleted;
            }
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
                    try
                    {
                        foreach (SqlQuery query in queries)
                        {
                            using (SqlCommand cmd = new SqlCommand(query.Sql, con, transaction))
                            {
                                cmd.Parameters.AddRange(query.Parameters.ToArray());
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
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
                }
                catch (Exception)
                {
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
                    return true;
                }
                catch (Exception)
                {
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
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public int DeleteFromDb(SqlQuery query, string aciklama, bool dummy)
        {
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            {
                try
                {
                    if (query.Parameters.Count > 0)
                        cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    int deleted = cmd.ExecuteNonQuery();
                    return deleted;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool DeleteFromDb<T>(SqlQuery query, T objectToDelete)
        {
            using (SqlConnection con = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand cmd = new SqlCommand(query.Sql, con))
            {
                try
                {
                    cmd.Parameters.AddRange(query.Parameters.ToArray());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
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
                                        break;
                                    }
                                case ProjeConstants.SQL_DELETE:
                                    {
                                        SqlCommand cmd = new SqlCommand(sqlString, dBConnection);
                                        cmd.Transaction = DBTransaction;
                                        item.RowsAffected = cmd.ExecuteNonQuery();
                                        item.Success = true;
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
                catch (Exception)
                {
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

    }
}
