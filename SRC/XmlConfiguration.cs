using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Configuration;
using System.Xml;

namespace HttpService
{
    /// <summary>
    /// Klasa odczytująca parametry i ich wartości z pliku konfiguracyjnego appconfig.xml
    /// </summary>
    public static class XmlConfiguration
    {
        private static string _configFileFullPath;
        public static System.Collections.Specialized.NameValueCollection Settings;

        public static string ConfigFile
        {
            set
            {
                Settings = new System.Collections.Specialized.NameValueCollection();
                //System.Reflection.Assembly ass = Assembly.GetExecutingAssembly();

                //string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                //string configFileFullPath = Path.Combine(appPath, value);

                //if (!File.Exists(configFileFullPath))
                //    throw new FileNotFoundException("Brak pliku: " + configFileFullPath);

                //_configFileFullPath = configFileFullPath;
                _configFileFullPath = value;
                readConfiguration();
            }

        }

        private static void readConfiguration()
        {

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(_configFileFullPath);
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("appSettings");

            foreach (XmlNode node in nodeList)
            {
                foreach (XmlNode key in node.ChildNodes)
                {
                    Settings.Add(key.Attributes["key"].Value, key.Attributes["value"].Value);
                }
            }
        }

        public static bool writeXMLValue(string klucz, string NewValue)
        {
            bool brak_klucza = true;
            try
            {

                System.Xml.XmlDocument xd = new System.Xml.XmlDocument();

                //load the xml file
                xd.Load(_configFileFullPath);

                XmlNodeList nodeList = xd.GetElementsByTagName("appSettings");

                foreach (XmlNode node in nodeList)
                {
                    foreach (XmlNode key in node.ChildNodes)
                    {
                        if (key.Attributes["key"].Value == klucz)
                        {
                            key.Attributes["value"].Value = NewValue;
                            brak_klucza = false;
                        }
                    }
                }
                if (brak_klucza)
                {
                    System.Xml.XmlElement Node = (System.Xml.XmlElement)(xd.DocumentElement.SelectSingleNode((string)("/configuration/appSettings")));
                    System.Xml.XmlElement nowy = xd.CreateElement("add");
                    nowy.SetAttribute("key", klucz);
                    nowy.SetAttribute("value", NewValue);
                    Node.AppendChild(nowy);
                }

                xd.Save(_configFileFullPath);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "konfiguracja");
                return false;
            }
            return true;
        }
    }


}
