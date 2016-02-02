
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading;

namespace HttpService
{
    public class SQL
    {
        private static string connectionString;
        private static ArrayList connectionList = new ArrayList();  //lista otwartych po��cze� z serwerem SQL

        public static string ConnectionString
        {
            set
            {
                zamknijPolaczeniaSQL();     //zamykamy po��czenia zrobione dla poprzedniego ConnectionString (praktycznie nie wystapi)
                connectionString = value;
            }
        }
        public static int Dopuszczalna_liczba_polaczen = 0;     //max. dopuszczalna liczba po��cze� z serwerem SQL
        public static int Dopuszczalny_czas_bezczynnosci = 300;   //czas bezczynno�ci po��czenia SQL w [s], po kt�rym jest automatycznie zamykane

        /// <summary>
        /// Zamkni�cie wszystkich po��cze�
        /// </summary>
        private static void zamknijPolaczeniaSQL()
        {
            if (connectionList != null)
            {
                for (int i = 0; i < connectionList.Count; i++)
                {
                    PolaczenieSQL polaczenieSQL;
                    polaczenieSQL = (PolaczenieSQL)connectionList[i];
                    polaczenieSQL.SqlConn.Close();
                }
            }
            connectionList = new ArrayList();
        }

        public static void SprawdzPolaczenieSql()
        {
            PolaczenieSQL polaczenieSQL = getPolaczenieSql();
            polaczenieSQL.Aktualnie_wykorzystywane = false;
        }

        /// <summary>
        /// Funkcja zwraca po��czenie SQL (istniej�ce lub tworzy nowe)
        /// </summary>
        /// <returns></returns>
        private static PolaczenieSQL getPolaczenieSql()
        {
            lock (connectionList)
            {
                PolaczenieSQL polaczenieSQL;
                PolaczenieSQL polaczenieSQLdostepne = null;
                for (int i=connectionList.Count-1; i >= 0; i--)
                {
                    polaczenieSQL = (PolaczenieSQL)connectionList[i];
                    if (polaczenieSQL.Aktualnie_wykorzystywane)
                        continue;


                    if (polaczenieSQLdostepne == null)
                    {
                        polaczenieSQL.Czas_ostatniego_uzycia = DateTime.Now;
                        polaczenieSQL.Aktualnie_wykorzystywane = true;
                        polaczenieSQLdostepne = polaczenieSQL;
                    }

                    //usuwamy nieu�ywane
                    if (polaczenieSQL.Czas_ostatniego_uzycia.AddSeconds(Dopuszczalny_czas_bezczynnosci).CompareTo(DateTime.Now) < 0)
                    {
                        connectionList.RemoveAt(i);
                    }
                }

                //jesli nie ma istniej�cego, dost�pnego po��czenia to tworzymy nowe
                if (polaczenieSQLdostepne == null)
                {
                    if (Dopuszczalna_liczba_polaczen > 0 && connectionList.Count >= Dopuszczalna_liczba_polaczen)
                        throw new Exception("Przekroczona liczba po��cze� SQL");

                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString = connectionString;
                    try
                    {
                        connection.Open();
                    }
                    catch (SqlException e)
                    {
                        throw new Exception(e.Message);
                    }
                    if (connection.State != ConnectionState.Open)
                        throw new Exception("Brak po��czenia");

                    polaczenieSQLdostepne = new PolaczenieSQL();
                    polaczenieSQLdostepne.SqlConn = connection;
                    polaczenieSQLdostepne.Czas_ostatniego_uzycia = DateTime.Now;
                    polaczenieSQLdostepne.Aktualnie_wykorzystywane = true;

                    connectionList.Add(polaczenieSQLdostepne);
                }
                return polaczenieSQLdostepne;
            }
        }


        private static void obslugaSqlException(SqlException e)
        {
            try
            {
                if (e.Class == 16 && e.State == 3)  //B��d walidacji
                {
                    //DataTable dt = SQL.SQLDataTable("select * from valid_error where spid=@@SPID");
                    //ArrayList l = new ArrayList();
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


        public static DataSet SQLDataSet(string sql_command)
        {
            return SQLDataSet(new SqlCommand(sql_command));
        }

        public static DataSet SQLDataSet(SqlCommand cmd)
        {

            DataSet ds = null;
            SqlDataAdapter da = null;
            PolaczenieSQL polaczenieSQL = getPolaczenieSql(); //pobiera po��czenie SQL i zaznacza je jako aktualnie_wykorzystywane

            ds = new DataSet();

            try
            {
                cmd.Connection = polaczenieSQL.SqlConn;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                //Thread.Sleep(4000);
            }
            catch (SqlException e)
            {
                obslugaSqlException(e);
            }
            finally
            {
                polaczenieSQL.Aktualnie_wykorzystywane = false;
            }
            return ds;

        }
        public static void SQLCommand(string sql_command)
        {
            PolaczenieSQL polaczenieSQL = getPolaczenieSql(); //pobiera po��czenie SQL i zaznacza je jako aktualnie_wykorzystywane
            SqlCommand cmd = new SqlCommand(sql_command, polaczenieSQL.SqlConn);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (SqlException e)
            {
                obslugaSqlException(e);
            }
            finally
            {
                polaczenieSQL.Aktualnie_wykorzystywane = false;
            }

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

        static public string StringForXML_JSON(string str, string format)
        {
            if (format=="xml")
                return StringForXML(str);
            else
                return StringForJSON(str);
        }

        /// <summary>
        /// Normalizuje string na potrzeby XML
        /// Zamienia zarezerwowane znaki na symbole: [<]->[&lt;],[>]->[&gt;],[&]->[&amp;],["]->[&quot],[']->[&apos]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string StringForXML(string str)
        {
            if (str == null)
                return "";
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\"", "&quot;");
            return str.Replace("'", "&apos;");
        }

        /// <summary>
        /// Normalizuje string na potrzeby JSON
        /// Zamienia: ["]->[\"], [\]->[\\] 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string StringForJSON(string str)
        {
            if (str == null)
                return "";
            str = str.Replace("\\", "\\\\");
            return str.Replace("\"", "\\\"");
        }

        private class PolaczenieSQL
        {
            public SqlConnection SqlConn;
            public DateTime Czas_ostatniego_uzycia;
            public bool Aktualnie_wykorzystywane;
        }


        ///// Klasa w�tku wykonania zapytania SQL w trybie asynchronicznym
        /////
        ///// Np. u�ycie wygl�da�o by tak:
        /////                     
        /////     SQL.ThreadExecuteSQL tes = new SQL.ThreadExecuteSQL(proc_sql, param_proc);  // Tworzymy obiekt dla w�tku zapytania SQL
        /////     Thread t = new Thread(new ThreadStart(tes.ThreadProc));  // Tworzymy w�tek zapytania SQL
        /////     t.Start();  //uruchamiamy w�tek zapytania SQL
        /////     t.Join();   //tu bie��cy w�tek czeka a� w�tek ExecuteSQL si� wykona
        /////     RetSql ret_sql = tes.Ret;   //wynik dzia�ania procedury

        //public class ThreadExecuteSQL
        //{
        //    private string _proc_sql;
        //    private string _param_proc;
        //    public RetSql Ret;

        //    public ThreadExecuteSQL(string proc_sql, string param_proc )
        //    {
        //        _proc_sql = proc_sql;
        //        _param_proc = param_proc;
        //    }

        //    public void ThreadProc()
        //    {
        //        Ret = ExecuteSQL(_proc_sql, _param_proc);
        //    }
        //}

    }

}
