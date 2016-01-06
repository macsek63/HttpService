
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace HttpService
{
    public class SQL
    {

        public static SqlConnection Connection = new SqlConnection();
        public static void Connect(string conn_string)
        {
            Connection.ConnectionString = conn_string;
            try
            {
                Connection.Open();
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            if (Connection.State != ConnectionState.Open)
                throw new Exception("Brak po³¹czenia");
        }

        public static void SQLCommand(string sql_command)
        {
            SqlCommand sql_comm = null;
            sql_comm = new SqlCommand(sql_command, Connection);
            try
            {
                sql_comm.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                obslugaSqlException(e);
            }
        }
        private static void obslugaSqlException(SqlException e)
        {
            try
            {
                if (e.Class == 16 && e.State == 3)  //B³¹d walidacji
                {
                    DataTable dt = SQL.SQLDataTable("select * from valid_error where spid=@@SPID");
                    ArrayList l = new ArrayList();
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //    l.Add(new ErrorValidate(Convert.ToString(dt.Rows[i]["kolumna"]), Convert.ToString(dt.Rows[i]["komunikat"])));
                    //ValidatingException ve = new ValidatingException(l);
                    //throw ve;
                    throw new Exception(e.Message);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(e.Message + "; " + ex.Message);
            }

            throw new Exception(e.Message);
        }

        public static SqlDataReader SQLReader(string sql_command)
        {
            SqlCommand sql_comm = null;
            sql_comm = new SqlCommand(sql_command, Connection);
            return sql_comm.ExecuteReader();
        }

        public static DataSet SQLDataSet(string sql_command)
        {

            DataSet ds = null;
            SqlDataAdapter da = null;

            ds = new DataSet();

            try
            {
                da = new SqlDataAdapter(sql_command, Connection);
                da.Fill(ds);
            }
            catch (SqlException e)
            {
                obslugaSqlException(e);
            }
            return ds;

        }
        public static DataTable SQLDataTable(string sql_command)
        {
            DataSet ds = SQLDataSet(sql_command);
            return ds.Tables[0];
        }

        /// <summary>
        /// Zamienia tablicê parametrów na jeden parametr xml
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string ParamToXml(Hashtable param)
        {
            string param_xml;
            param_xml = "<param>";
            IDictionaryEnumerator e = param.GetEnumerator();
            while (e.MoveNext())
            {
                param_xml += "<" + (string)e.Key + ">";
                param_xml += e.Value.ToString();
                param_xml += "</" + (string)e.Key + ">";
            }
            param_xml += "</param>";
            return param_xml;
        }

        /// <summary>
        /// Normalizuje string na potrzeby zapytania SQL: zamienia ' na ''
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string StringForSQL(string str)
        {
            if (str == null)
                return "";
            return str.Replace("'", "''");
        }

        /// <summary>
        /// Normalizuje string na potrzeby klauzuli LIKE zapytania SQL 
        /// Zamienia wild chars: %->[%] , _->[_]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string StringForSQL_LIKE(string str)
        {
            if (str == null)
                return "";
            string str1 = str.Replace("%", "[%]");
            return str1.Replace("_", "[_]");
        }
    }

}
