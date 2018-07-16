namespace enigma
{
    #region Usings
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.WebSockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Dynamitey;
    using ImpromptuInterface;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog.Config;
    using Qlik.EngineAPI;
    using System.Linq;
    using NLog;
    #endregion
   
    class Program
    {
        #region nlog helper for netcore
        private static void SetLoggerSettings(string configName)
        {
            var path = Path.Combine(System.AppContext.BaseDirectory, configName);
            if (!File.Exists(path))
            {
                var root = new FileInfo(path).Directory?.Parent?.Parent?.Parent;
                var files = root.GetFiles("App.config", SearchOption.AllDirectories).ToList();
                path = files.FirstOrDefault()?.FullName;
            }

            LogManager.Configuration = new XmlLoggingConfiguration(path, false);
        } 
        #endregion

        static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            SetLoggerSettings("App.config");
            logger.Info("Start");

            var config = new EnigmaConfigurations()
            {
                Url = "ws://127.0.0.1:4848/app/engineData/",

                //if you want to create your own Connection with for example header / cookies, just inject this in line
                //CreateSocket = async (url) =>
                //{
                //    var ws = new ClientWebSocket();
                //    !!!!!!!!! here you can inject your cookie, header authentification,...
                //    await ws.ConnectAsync(new Uri(url), CancellationToken.None);
                //    return ws;
                //}
            };


            var session = Enigma.Create(config);

            // connect to the engine
            var globalTask = session.OpenAsync();
            globalTask.Wait();

            dynamic globalDyn = (GeneratedAPI)globalTask.Result;

            // full work with Dynamic
            ((Task<dynamic>)globalDyn.EngineVersion())
            .ContinueWith((res) =>
            {
                Console.WriteLine("EngineVER1: " + res.Result.qComponentVersion.ToString());
            });

            // show that all functions can be called with Async or without Async
            ((Task<dynamic>)globalDyn.EngineVersionAsync())
            .ContinueWith((res) =>
            {
                Console.WriteLine("EngineVER2: " + res.Result.qComponentVersion.ToString());
            });

            // even with small letter like enigma.js is possible
            ((Task<dynamic>)globalDyn.engineVersion())
            .ContinueWith((res) =>
            {
                Console.WriteLine("EngineVER3: " + res.Result.qComponentVersion.ToString());
            });

            // now with cool full type support
            IGlobal global = Impromptu.ActLike<IGlobal>(globalTask.Result);

            global.EngineVersionAsync()
                .ContinueWith((engVer) =>
                {
                    Console.WriteLine("CastedEngineVer:" + engVer.Result.qComponentVersion);
                });
            
            global.OpenDocAsync(Path.GetFileName("%USERPROFILE%\\Documents\\Qlik\\Sense\\Apps\\Executive Dashboard.qvf"))
                .ContinueWith((newApp) =>
                {

                    Console.WriteLine("Object " + (newApp.Result).ToString());

                    var app = newApp.Result;

                    // test the changed notification of the opend app
                    app.Changed += App_Changed;

                    // just a normal get script
                    app.GetScriptAsync()
                        .ContinueWith((script) =>
                        {
                            Console.WriteLine("Script" + script.Result.ToString().Substring(1, 100));
                        });

                    // change the script, so that the app changed is triggered
                    app.SetScriptAsync("HALLO")
                        .ContinueWith((res) =>
                        {
                            // read the changed script
                            app.GetScriptAsync()
                                .ContinueWith((script) =>
                                {
                                    Console.WriteLine("Script2" + script.Result.ToString());
                                });
                        });

                });

            //Thread.Sleep(3000);

            Console.ReadLine();
        }

        private static void App_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("************* APP CHANGES *****************************");
        }

    }
}