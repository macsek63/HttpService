using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HttpService
{
    public partial class ConnectForm : Form
    {

        public ConnectForm()
        {
            InitializeComponent();

            this.tb_Login.Text = XmlConfiguration.Settings["Login"];
            this.tb_Baza.Text = XmlConfiguration.Settings["Database"];
            this.tb_Serwer.Text = XmlConfiguration.Settings["SQLServer"];
        }

        private void button_wy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            //Sprawdzamy czy podano dane do logowania
            if (String.IsNullOrEmpty(this.tb_Login.Text) ||
                String.IsNullOrEmpty(this.tb_Haslo.Text) ||
                String.IsNullOrEmpty(this.tb_Baza.Text) ||
                String.IsNullOrEmpty(this.tb_Serwer.Text))
            {
                MessageBox.Show("Wpisz dane we wszystkich polach.","");
                return;
            }

            if (!ConnectForm.Connect(this.tb_Login.Text, this.tb_Haslo.Text, this.tb_Serwer.Text, this.tb_Baza.Text))
            {
                return;
            }
            else
            {
                Crypto security = new Crypto();

                string haslo = security.Encrypt(this.tb_Haslo.Text, this.tb_Login.Text);

                XmlConfiguration.writeXMLValue("SQLServer", this.tb_Serwer.Text);
                XmlConfiguration.writeXMLValue("Database", this.tb_Baza.Text);
                XmlConfiguration.writeXMLValue("Login", this.tb_Login.Text);
                XmlConfiguration.writeXMLValue("Password", haslo);

                XmlConfiguration.Settings["SQLServer"] = this.tb_Serwer.Text;
                XmlConfiguration.Settings["Database"] = this.tb_Baza.Text;
                XmlConfiguration.Settings["Login"] = this.tb_Login.Text;
                XmlConfiguration.Settings["Password"] = haslo;
            }

            this.Close();

        }



        private void textBox_user_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)    //Enter
            {
                e.Handled = true;
                button_ok_Click(sender, e);
            }
            if (e.KeyChar == 27)    //Esc
            {
                e.Handled = true;
                button_wy_Click(sender, e);
            }
        }

        /// <summary>
        /// Połączenie usera z bazą danych
        /// </summary>
        public static bool ConnectUser(string user, string pass)
        {
            return Connect(user, pass, XmlConfiguration.Settings["SQLServer"], XmlConfiguration.Settings["Database"]);
        }

        /// <summary>
        /// Połączenie na podany login 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="haslo"></param>
        /// <param name="serwer"></param>
        /// <param name="baza"></param>
        /// <returns></returns>
        public static bool Connect(string login, string haslo, string serwer, string baza)
        {

            try
            {
                SQL.Connect("App=SSN; Data Source=" + serwer + ";Initial Catalog=" + baza + ";User ID=" + login + "; Password=" + haslo);
                return true;
            }
            catch
            {
            }
            MessageBox.Show("Brak dostępu do bazy ERP! (serwer: " + serwer + ", baza: " + baza + ", użytkownik: " + login, "");
            return false;
        }

        /// <summary>
        /// Połączenie na login dla aplikacji
        /// </summary>
        /// <returns></returns>
        public static bool ConnectApp()
        {
            string login;
            string haslo;
            Crypto security = new Crypto();

            login = XmlConfiguration.Settings["Login"];
            haslo = XmlConfiguration.Settings["Password"];

            haslo = security.Decrypt((string)haslo, (string)XmlConfiguration.Settings["Login"]);

            return Connect(login, haslo, XmlConfiguration.Settings["SQLServer"], XmlConfiguration.Settings["Database"]);

        }

    }
}