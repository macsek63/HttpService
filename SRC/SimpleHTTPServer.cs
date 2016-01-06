using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;


namespace HttpService
{
    class SimpleHTTPServer
    {

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
    };
        private Thread _serverThread;
        private string _rootDirectory;
        //private string _indexDirectory;
        private string _indexFile_http;
        private string _indexFile_https;
        private HttpListener _listener;
        private int _port_http;
        private int _port_https;


        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHTTPServer()
        {
            ////get an empty port
            //TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            //l.Start();
            //_port_http = ((IPEndPoint)l.LocalEndpoint).Port;
            //l.Stop();

            try
            {
                _port_http = 0;
                _port_https = 0;
                _port_http = Convert.ToInt32(XmlConfiguration.Settings["Port_http"]);
                _port_https = Convert.ToInt32(XmlConfiguration.Settings["Port_https"]);
            }
            catch
            {
            }
            if (_port_http == 0)
            {
                throw new Exception("Błędna wartość parametru: Port_http");
            }
            _rootDirectory = XmlConfiguration.Settings["RootDirectory"];
            //_indexDirectory = XmlConfiguration.Settings["IndexDirectory"];
            _indexFile_http = XmlConfiguration.Settings["IndexFile_http"];
            _indexFile_https = XmlConfiguration.Settings["IndexFile_https"];

            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }


        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            //jeśli ma być serwer HTTPS
            if (_port_https > 0)
            {
                //odczytujemy certyfikat (powinien być w tym katalogu gdzie exe)
                X509Certificate2 certificate = new X509Certificate2("cert.pfx", "", X509KeyStorageFlags.MachineKeySet); //hasło puste

                //dodajemy certyfikat do magazynu certyfiaktów na komputerze
                X509Store store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(certificate))
                    store.Add(certificate);
                store.Close();

                //pobieramy GUID aplikacji
                System.Reflection.Assembly assembly = Assembly.GetEntryAssembly();
                string guid = assembly.GetType().GUID.ToString();


                //robimy powiązanie portu z aplikacją 
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "netsh";
                psi.Arguments = "http add sslcert ipport=0.0.0.0:" + _port_https.ToString() + " certhash=" + certificate.Thumbprint + " appid={" + guid + "}";
                Process proc = System.Diagnostics.Process.Start(psi);
                proc.WaitForExit();
                psi.Arguments = "http add sslcert ipport=[::]:" + _port_https.ToString() + " certhash=" + certificate.Thumbprint + " appid={" + guid + "}";
                proc = System.Diagnostics.Process.Start(psi);
                proc.WaitForExit();
            }


            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + _port_http.ToString() + "/");
            if (_port_https > 0)
                _listener.Prefixes.Add("https://*:" + _port_https.ToString() + "/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch 
                {

                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);




            if (context.Request.HttpMethod.Equals("OPTIONS"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");   //serwis dostępny dla wszystkich
                context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");   //dostępne metody (w zasadzie jest tylko POST na razie)
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");  //wszystkie nagłówki dopuszczalne
                context.Response.Headers.Add("Access-Control-Max-Age", "1728000");  //max.czas przez jaki odpowiedź na OPTION może być pamiętana w przeglądarce

                context.Response.Headers.Add("Vary", "Accept-Encoding");
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Close();
                return;
            }

            if (filename.Contains("Execute"))
            {
                String wynik = "wynik działania procedury";
                byte[] wynik_byte = Encoding.UTF8.GetBytes(wynik);
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = wynik_byte.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);   //serwis dostępny dla hosta wywołującego

                context.Response.StatusCode = (int)HttpStatusCode.OK;

                context.Response.OutputStream.Write(wynik_byte, 0, wynik_byte.Length);
                context.Response.OutputStream.Flush();

                context.Response.OutputStream.Close();
                return;
            }

            //jeśli wywołanie wprost z przeglądarki i nie została podana nazwa pliku to doklejamy główny plik index.html
            if (context.Request.UrlReferrer == null)
            {
                if (String.IsNullOrEmpty(filename))
                {
                    if (context.Request.Url.Port == _port_http)
                        filename = _indexFile_http;
                    else
                        filename = _indexFile_https;
                }
            }

            filename = Path.Combine(_rootDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    string mime;
                    context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();
                    context.Response.OutputStream.Flush();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch 
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }


    }
}