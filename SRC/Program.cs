using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace HttpService
{
    static class Program
    {
        public static ConnectForm connForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            try
            {
                if (args.Length > 0)
                    XmlConfiguration.ConfigFile = args[0];
                else
                    XmlConfiguration.ConfigFile = "appconfig.xml";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Plik konfiguracyjny.");
                return;
            }


            ////jeśli w pliku parametrycznym nie podane są wszystkie dane potrzebne do połączenia z bazą SQL 
            ////to podnosimy okno do podania tych danych 
            //if (String.IsNullOrEmpty(XmlConfiguration.Settings["SQLServer"]) ||
            //    String.IsNullOrEmpty(XmlConfiguration.Settings["Database"]) ||
            //    String.IsNullOrEmpty(XmlConfiguration.Settings["Login"]) ||
            //    String.IsNullOrEmpty(XmlConfiguration.Settings["Password"])
            //    )
            //{
            //    connForm = new ConnectForm();
            //    connForm.ShowDialog();
            //    connForm.Dispose();

            //    if (SQL.Connection == null || SQL.Connection.State != System.Data.ConnectionState.Open)
            //        return;
            //}
            //else
            //{
            //    if (!ConnectForm.ConnectApp())
            //        return;
            //}


            try
            {
                SimpleHTTPServer sh = new SimpleHTTPServer();
                Application.Run(new FormService());
                sh.Stop();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error:Program.Main");
                return;
            }

        }
 
    }


}
