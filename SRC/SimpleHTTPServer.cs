using System;
using System.Collections.Generic;
using System.Collections;
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
        private string _rootDirectory;
        private string _indexFile_http;
        private string _indexFile_https;
        private string _certificateFile;
        private HttpListener _http_listener;
        private Thread _http_listener_thread;
        private int _port_http;
        private int _port_https;
        private Hashtable _tabelaPolaczen;


        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHTTPServer()
        {

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
            _indexFile_http = XmlConfiguration.Settings["IndexFile_http"];
            _indexFile_https = XmlConfiguration.Settings["IndexFile_https"];
            _certificateFile = XmlConfiguration.Settings["CertificateFile"];


            //jeśli ma być serwer HTTPS
            if (_port_https > 0 && !String.IsNullOrEmpty(_certificateFile))
            {
                try
                {
                    //odczytujemy certyfikat (powinien być w tym katalogu gdzie exe)
                    X509Certificate2 certificate = new X509Certificate2(_certificateFile, "", X509KeyStorageFlags.MachineKeySet); //hasło puste

                    //dodajemy certyfikat do magazynu certyfikatów na komputerze (nie wiadomo gdzie to się dodaje, nie widać w managerze certyfikatów)
                    X509Store store = new X509Store(StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    if (!store.Certificates.Contains(certificate))
                        store.Add(certificate);
                    store.Close();

                    //pobieramy GUID aplikacji
                    System.Reflection.Assembly assembly = Assembly.GetEntryAssembly();
                    string guid = assembly.GetType().GUID.ToString();


                    //robimy powiązanie portu z aplikacją i certyfikatem
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "netsh";
                    psi.Arguments = "http add sslcert ipport=0.0.0.0:" + _port_https.ToString() + " certhash=" + certificate.Thumbprint + " appid={" + guid + "}";
                    Process proc = System.Diagnostics.Process.Start(psi);
                    proc.WaitForExit();
                    psi.Arguments = "http add sslcert ipport=[::]:" + _port_https.ToString() + " certhash=" + certificate.Thumbprint + " appid={" + guid + "}";
                    proc = System.Diagnostics.Process.Start(psi);
                    proc.WaitForExit();
                }
                catch
                {
                }
            }

            _http_listener = new HttpListener();
            _http_listener.Prefixes.Add("http://*:" + _port_http.ToString() + "/");
            if (_port_https > 0)
                _http_listener.Prefixes.Add("https://*:" + _port_https.ToString() + "/");

            _tabelaPolaczen = new Hashtable();
        }


        /// <summary>
        /// Start server 
        /// </summary>
        public void Start()
        {
            _http_listener.Start();
            _http_listener_thread = new Thread(this.Listen);
            _http_listener_thread.Start();
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _http_listener_thread.Abort();
            _http_listener.Stop();
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    IAsyncResult result = _http_listener.BeginGetContext(new AsyncCallback(ListenerCallback), _http_listener);
                    result.AsyncWaitHandle.WaitOne();
                }
                catch 
                {

                }
            }
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            HttpListener listener;
            HttpListenerContext context;

            listener = ar.AsyncState as HttpListener;
            context = listener.EndGetContext(ar);
            string ip = context.Request.RemoteEndPoint.Address.ToString();  //ip komputera, który wysłał żądanie do ew. sprawdzenia na liście ip uprawnionych

            lock (_tabelaPolaczen)
            {
                _tabelaPolaczen.Add(context, DateTime.Now);
            }
            //Thread.Sleep(1000);

            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);


            if (context.Request.HttpMethod.Equals("OPTIONS"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");   //serwis dostępny dla wszystkich
                context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");   //dostępne metody 
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");  //wszystkie nagłówki dopuszczalne
                context.Response.Headers.Add("Access-Control-Max-Age", "1728000");  //max.czas przez jaki odpowiedź na OPTION może być pamiętana w przeglądarce

                context.Response.Headers.Add("Vary", "Accept-Encoding");
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                goto END;
            }
            if (context.Request.HttpMethod.Equals("POST"))
            {
                string post;

                try
                {
                    byte[] buff_byte = new byte[2024 * 16];
                    int nbytes = context.Request.InputStream.Read(buff_byte, 0, buff_byte.Length);
                    post = Encoding.UTF8.GetString(buff_byte);
                }
                catch
                {
                    goto END;
                }

                context.Response.ContentType = "text/plain";
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);   //serwis dostępny dla hosta wywołującego
                context.Response.StatusCode = (int)HttpStatusCode.OK;


                if (filename.Contains("SqlExecute"))
                {
                    string proc_sql;
                    string param_proc;
                    string response_format;
                    string resultset_transform_name;
                    System.Xml.XmlNode node_param_ws;

                    System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
                    try
                    {
                        xd.LoadXml(post);
                        proc_sql = xd.DocumentElement.SelectSingleNode("proc").InnerText;
                        param_proc = xd.DocumentElement.SelectSingleNode("param_proc").OuterXml;
                        node_param_ws = xd.DocumentElement.SelectSingleNode("param_ws");
                        response_format = xd.DocumentElement.SelectSingleNode("param_ws/response_format").InnerXml;
                        resultset_transform_name = xd.DocumentElement.SelectSingleNode("param_ws/resultset_transform/name").InnerXml;
                    }
                    catch
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.StatusDescription = "Bledna struktura parametrow wywolania SqlExecute."; //nie może być polskich znaków w haederze HTML, (wyskakuje Exception)
                        //"Wymagana struktura:\n" +
                        //"<param>\n" +
                        //"  <proc>nazwa procedury sql</proc>\n" +
                        //"  <param_proc>parametry dla procedury</param_proc>\n" +
                        //"  <param_ws>\n" +
                        //"   <param_ws>\n" +
                        //"       <response_format>format odpowiedzi:json/xml</response_format>\n" +
                        //"       <resultset_transform>\n" +
                        //"           <name>nazwa transformacji:for_grid</name>\n" +
                        //"       </resultset_transform>\n" +
                        //"   </param_ws>\n" +
                        //"</param>";

                        goto END;
                    }

                    RetSql ret_sql = DAL.Execute_procedure_hts(proc_sql, param_proc);  //wykonujemy procedurę SQL

                    try
                    {
                        send_wynik_sql_to_requester(ret_sql, context, response_format, resultset_transform_name);    //otrzmymane dane wysyłamy do klienta
                    }
                    catch   //jeśli wyjątek to prawdopodobnie połączenie zostało zamknięte przez klienta
                    {
                    }
                    goto END;
                } //koniec "SqlExecute"
            }


            send_file_to_requester(context, filename);

            END:

            lock (_tabelaPolaczen)
            {
                _tabelaPolaczen.Remove(context);
            }
            context.Response.OutputStream.Close();
        }


        /// <summary>
        /// Wysyła plik do komputera, który przysłał żądanie http
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filename"></param>
        private void send_file_to_requester(HttpListenerContext context, string filename)
        {
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
                Stream input = null;

                try
                {

                    input = new FileStream(filename, FileMode.Open);
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
                    context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    if (input != null)
                        input.Close();
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }


        /// <summary>
        /// Wysyła wyniki działania procedury SQL do komputera, który przysłał żądanie http
        /// </summary>
        /// <param name="ret_sql">Wyniki do wysłania</param>
        /// <param name="context">Context http</param>
        /// <param name="response_format">Format w jakim mają być wysłane wyniki. Może być "xml" lub "json" </param>
        /// <param name="resultset_transform_name">
        /// Informacja o tym jak dane mają być przekształcone przed wysłaniem. Dpuszczalne wartości:
        /// "for_grid" - dane są przeznaczone dla grida, dodawana jest numeracja wierszy
        /// </param>
        private void send_wynik_sql_to_requester(RetSql ret_sql, HttpListenerContext context, string response_format, string resultset_transform_name)
        {
            //string post;
            string buff_str = "";
            byte[] buff_byte = new byte[2024 * 16];
            //int nbytes;

            if (response_format == "xml")
            {
                buff_str =
                    "<ret_data>" +
                        "<status>" + ret_sql.status.ToString() + "</status>" +
                        "<message>" + SQL.StringForXML(ret_sql.message) + "</message>" +
                        "<data>";
            }
            if (response_format == "json")
            {
                buff_str =
                    "{\"ret_data\":{" +
                        "\"status\":\"" + ret_sql.status.ToString() + "\"," +
                        "\"message\":\"" + SQL.StringForJSON(ret_sql.message) + "\"," +
                        "\"data\":{";
            }

            buff_byte = Encoding.UTF8.GetBytes(buff_str);

            
            context.Response.OutputStream.Write(buff_byte, 0, buff_byte.Length);

            //transformacja resultseta na postać dla grida
            if (resultset_transform_name == "for_grid")
            {
                if (ret_sql.dataTable != null && ret_sql.dataTable.Rows.Count > 0)
                {
                    if (response_format == "xml")
                        buff_str = "<rows>";
                    if (response_format == "json")
                        buff_str = "\"rows\":[";
                    buff_byte = Encoding.UTF8.GetBytes(buff_str);
                    context.Response.OutputStream.Write(buff_byte, 0, buff_byte.Length);

                    for (int i = 0; i < ret_sql.dataTable.Rows.Count; i++)
                    {
                        if (response_format == "xml")
                            buff_str = "<row id=\"" + i.ToString() + "\">";
                        if (response_format == "json")
                        {
                            if (i == 0)
                                buff_str = "{\"id\":\"" + i.ToString() + "\", \"data\":[";
                            else
                                buff_str = ",{\"id\":\"" + i.ToString() + "\", \"data\":[";
                        }
                        for (int j = 0; j < ret_sql.dataTable.Columns.Count; j++)
                        {
                            string str_cell = Convert.ToString(ret_sql.dataTable.Rows[i][j]);
                            if (response_format == "xml")
                                buff_str += "<cell>" + SQL.StringForXML(str_cell) + "</cell>";
                            if (response_format == "json")
                            {
                                if (j == 0)
                                    buff_str += "\"" + SQL.StringForJSON(str_cell) + "\"";
                                else
                                    buff_str += ",\"" + SQL.StringForJSON(str_cell) + "\"";
                            }
                        }
                        if (response_format == "xml")
                            buff_str += "</row>";
                        if (response_format == "json")
                            buff_str += "]}";

                        buff_byte = Encoding.UTF8.GetBytes(buff_str);
                        context.Response.OutputStream.Write(buff_byte, 0, buff_byte.Length);
                    }

                    if (response_format == "xml")
                        buff_str = "</rows>";
                    if (response_format == "json")
                        buff_str = "]";

                    buff_byte = Encoding.UTF8.GetBytes(buff_str);
                    context.Response.OutputStream.Write(buff_byte, 0, buff_byte.Length);
                }
            }

            //koniec odpowiedzi
            if (response_format == "xml")
                buff_str = "</data></ret_data>";
            if (response_format == "json")
                buff_str = "}}}";

            buff_byte = Encoding.UTF8.GetBytes(buff_str);
            context.Response.OutputStream.Write(buff_byte, 0, buff_byte.Length);
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
    }
}