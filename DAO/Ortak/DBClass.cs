using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO.Ortak
{
    [Serializable]
    public class DbClass
    {

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

    }
}
