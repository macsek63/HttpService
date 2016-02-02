using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace HttpService
{
    public class DAL
    {
        private static Hashtable proceduryTab = new Hashtable();  //tabela dostępnych procedur SQL
        private static Hashtable usersTab = new Hashtable();  //tabela userów serwisu Http


        /// <summary>
        /// sprawdzenie czy podana procedura jest na liście procedur dostępnych dla serwisu http
        /// </summary>
        /// <param name="nazwa_procedury"></param>
        /// <returns></returns>
        static private bool czy_procedura_dostepna(string nazwa_procedury)
        {
            lock (proceduryTab)
            {
                //jeśli tablica procedur pusta to odczytujemy z bazy listę procedur dostępnych
                if (proceduryTab.Count == 0)
                {
                    DataSet ds = SQL.SQLDataSet("select * from hts_dostepne_procedury");

                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string nazwa_proc = Convert.ToString(dt.Rows[i]["nazwa_proc"]);
                        proceduryTab.Add(nazwa_proc, null);
                    }
                }
                return proceduryTab.ContainsKey(nazwa_procedury);
            }
        }

        /// <summary>
        /// Logowanie użytkownika. 
        /// Jeśli użytkownik nie miał wpisanego hasła (lub zostało wyczyszczone) to podane hasło jest zapisywane w tabeli.
        /// </summary>
        /// <param name="user_idn"></param>
        /// <param name="haslo"></param>
        /// <returns></returns>
        static public decimal Zaloguj(string user_idn, string haslo)
        {
            User user;

            lock (usersTab)
            {
                //jeśli tablica userów to odczytujemy z bazy listę userów
                if (usersTab.Count == 0)
                {
                    DataSet ds = SQL.SQLDataSet("select *  from hts_user");

                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        user = new User();
                        user.User_id = Convert.ToDecimal(dt.Rows[i]["user_id"]);
                        user.User_idn = Convert.ToString(dt.Rows[i]["user_idn"]);
                        user.Haslo = Convert.ToString(dt.Rows[i]["haslo"]);
                        proceduryTab.Add(user.User_idn, user);
                    }
                }
                user = (User)usersTab[user_idn];
            }

            if (user != null)   //jest user o takim idn
            {
                haslo = Crypto.Encrypt(haslo, user_idn);    //szyfrujemy podane haslo
                if (String.IsNullOrEmpty(user.Haslo))   //jesli user miał puste hasło to zapisujemy podane jako jego haslo
                {
                    try
                    {
                        SQL.SQLCommand("update hts_user set haslo = '" + SQL.StringForSQL(haslo) + "'" + " where user_id=" + user.User_id.ToString());
                    }
                    catch
                    {
                        throw new Exception("Wystąpił błąd przy zapisie hasła.");
                    }
                    user.Haslo = haslo;
                    return user.User_id;            //logowanie OK - haslo się zgadza
                }
                else
                {
                    if (haslo.Equals(user.Haslo))
                        return user.User_id;            //logowanie OK - haslo się zgadza
                }
            }

            throw new Exception("Błędny identyfikator użytkownika lub hasło ");
        }

        /// <summary>
        /// Wykonanie procedury SQL dostępnej dla serwisu http
        /// </summary>
        /// <param name="proc_sql"></param>
        /// <param name="param_proc"></param>
        /// <returns></returns>
        static public RetSql Execute_procedure_hts(string proc_sql, string param_proc)
        {
            int ret_status;
            string ret_message;
            DataTable dt = null;
            SqlCommand cmd;

            if (String.IsNullOrEmpty(proc_sql))
            {
                ret_status = -1;
                ret_message = "Brak nazwy procedury w wywołaniu SqlExecute.";
                return new RetSql(ret_status, ret_message, null);
            }

            if (!czy_procedura_dostepna(proc_sql))
            {
                ret_status = -1;
                ret_message = "Na liście dostępnych procedur brak procedury o nazwie: " + proc_sql;
                return new RetSql(ret_status, ret_message, null);
            }

            try
            {
                cmd = new SqlCommand(proc_sql);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@param_xml", param_proc);
                SqlParameter par = new SqlParameter("@ret", SqlDbType.Int);
                par.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(par);
                par = new SqlParameter("@ret_mess", SqlDbType.VarChar);
                par.Size = 2000;
                par.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(par);
                DataSet ds = SQL.SQLDataSet(cmd);

                dt = ds.Tables[0];
                ret_status = Convert.ToInt32(cmd.Parameters["@ret"].Value);
                ret_message = Convert.ToString(cmd.Parameters["@ret_mess"].Value);
                return new RetSql(ret_status, ret_message, dt);

            }
            catch (Exception e)
            {
                ret_status = -1;
                ret_message = "Błąd wykonania procedury: " + proc_sql + Environment.NewLine + e.Message;
                return new RetSql(ret_status, ret_message, null);
            }

        }
    }
}
