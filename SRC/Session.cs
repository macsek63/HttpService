using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace HttpService
{
    /// <summary>
    /// Klasa sesji zawiera informację o zalogowanym userze i czasie jego ostatniej aktywności
    /// </summary>
    public class Session
    {
        public static int Maks_liczba_polaczen = 0;     //maksymalna liczba połączeń do serwisu Http, jeśli 0 to nie ma ograniczeń
        private static Hashtable listaSesji = new Hashtable();  //tablica otwartych sesji

        decimal uzytkownikID;          //uzytkownikID usera
        string login;           //login
        DateTime lastRequest;   //ostatnie żądanie obsługi ze strony usera

        public Session(string a_login, decimal UzytkownikID, DateTime a_lastRequest)
        {
            login = a_login;
            uzytkownikID = UzytkownikID;
            lastRequest = a_lastRequest;
        }
        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                login = value;
            }
        }
        public DateTime LastRequest
        {
            get
            {
                return lastRequest;
            }
            set
            {
                lastRequest = value;
            }
        }
        public decimal UzytkownikID
        {
            get
            {
                return uzytkownikID;
            }
            set
            {
                uzytkownikID = value;
            }
        }


        /// <summary>
        /// Dodaje nową sesję 
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="UzytkownikID">uzytkownikID</param>
        /// <param name="ip">Adres Ip klienta</param>
        /// <param name="sesje">Tablica sesji</param>
        /// <returns>Klucz sesji</returns>
        static public string DodajSesje(string login, int uzytkownikID, string ip)
        {
            string guid;
            string key;
            guid = System.Guid.NewGuid().ToString();
            key = guid + ip;   //klucz sesji
            Session sesja = new Session(login, uzytkownikID, DateTime.Now);
            lock (listaSesji)
            {
                listaSesji.Add(key, sesja);
                kasujNieaktywneSesje();
                if (Maks_liczba_polaczen > 0 && listaSesji.Count > Maks_liczba_polaczen)
                    throw new Exception("Przekroczona liczba połączeń do serwisu Http.");
            }

            return guid;
        }

        /// <summary>
        /// Zwraca sesję jeśli jest aktywna lub null jeśli brak sesji o podanym kluczu lub już 
        /// jest nieaktywna
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sesje"></param>
        /// <returns></returns>
        static public Session DajSesje(string key)
        {
            lock (listaSesji)
            {
                if (key == null || !listaSesji.Contains(key))
                    return null;   //nie ma sesji o podanym kluczu
                Session s = (Session)listaSesji[key];
                if (s.LastRequest.AddMinutes(15).CompareTo(DateTime.Now) < 0)
                {
                    listaSesji.Remove(key);
                    return null;
                }
                s.LastRequest = DateTime.Now;
                return s;
            }
        }

        /// <summary>
        /// Usuwanie nieaktywnych sesji tzn. takich w których user nie zażądał obsługi przez 
        /// ostatnie 15 minut
        /// </summary>
        /// <param name="sesje">Tablica sesji</param>
        static private void kasujNieaktywneSesje()
        {
            ArrayList a = new ArrayList();
            IDictionaryEnumerator myEnumerator = listaSesji.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                Session s = (Session)myEnumerator.Value;
                if (s.LastRequest.AddMinutes(15).CompareTo(DateTime.Now) < 0)
                    a.Add(myEnumerator.Key);
            }

            for (int i = 0; i < a.Count; i++)
                listaSesji.Remove(a[i]);
        }

        //static public string DajLoginSesji(string key, Hashtable sesje)
        //{
        //    if (key == null || !sesje.Contains(key))
        //        return null;
        //    return ((Session)sesje[key]).Login;
        //}
        //static public int DajUzytkownikID(string key, Hashtable sesje)
        //{
        //    if (key == null || !sesje.Contains(key))
        //        return 0;
        //    return ((Session)sesje[key]).uzytkownikID;
        //}



        ///// <summary>
        ///// Sprawdza czy sesja jest aktywna
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="sesje"></param>
        ///// <returns></returns>
        //static public bool CzySesjaAktywna(string key, Hashtable sesje)
        //{
        //    if (key == null || !sesje.Contains(key))
        //        return false;   //nie ma sesji o podanym kluczu
        //    Session s = (Session)sesje[key];
        //    if (s.LastRequest.AddMinutes(15).CompareTo(DateTime.Now) < 0)
        //    {
        //        sesje.Remove(key);
        //        return false;
        //    }
        //    s.LastRequest = DateTime.Now;
        //    return true;
        //}

    }

}
