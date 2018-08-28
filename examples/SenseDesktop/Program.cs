namespace SenseDesktop
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
    using enigma;
    #endregion

    class Program
    {
        #region nlog helper for netcore
        private static void SetLoggerSettings(string configName)
        {            
#if NET452
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configName);
#else
            var path = Path.Combine(System.AppContext.BaseDirectory, configName);
#endif
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

            //// full work with Dynamic
            //((Task<dynamic>)globalDyn.EngineVersion())
            //.ContinueWith((res) =>
            //{
            //    Console.WriteLine("EngineVER1: " + res.Result.qComponentVersion.ToString());
            //});

            //// show that all functions can be called with Async or without Async
            //((Task<dynamic>)globalDyn.EngineVersionAsync())
            //.ContinueWith((res) =>
            //{
            //    Console.WriteLine("EngineVER2: " + res.Result.qComponentVersion.ToString());
            //});

            //// even with small letter like enigma.js is possible
            //((Task<dynamic>)globalDyn.engineVersion())
            //.ContinueWith((res) =>
            //{
            //    Console.WriteLine("EngineVER3: " + res.Result.qComponentVersion.ToString());
            //});

            //// now with cool full type support
            IGlobal global = Impromptu.ActLike<IGlobal>(globalTask.Result);
            var appName = Path.GetFileName("%USERPROFILE%\\Documents\\Qlik\\Sense\\Apps\\Executive Dashboard.qvf");
            var app = global.OpenDocAsync(appName).Result;

            //global.EngineVersionAsync()
            //    .ContinueWith((engVer) =>
            //    {
            //        Console.WriteLine("CastedEngineVer:" + engVer.Result.qComponentVersion);
            //    });

            //global.OpenDocAsync(appName)
            //    .ContinueWith((newApp) =>
            //    {

            //        Console.WriteLine("Object " + (newApp.Result).ToString());

            //        var app = newApp.Result;

            //        // test the changed notification of the opend app
            //        app.Changed += App_Changed;

            //        // just a normal get script
            //        app.GetScriptAsync()
            //            .ContinueWith((script) =>
            //            {
            //                Console.WriteLine("Script" + script.Result.ToString().Substring(1, 100));
            //            });

            //        // change the script, so that the app changed is triggered
            //        app.SetScriptAsync("HALLO")
            //            .ContinueWith((res) =>
            //            {
            //                // read the changed script
            //                app.GetScriptAsync()
            //                    .ContinueWith((script) =>
            //                    {
            //                        Console.WriteLine("Script2" + script.Result.ToString());
            //                    });
            //            });

            //    });

            //Thread.Sleep(3000);

            //Caluculation Test
            var calc = new CalculationExample(app);
            calc.CalcRandom(10);

            ////find the bookmark with type
            //var bookmarkExample = new BookmarkExample(app);
            //var task1 = bookmarkExample.ListBookmarksAsync();
            //task1.Wait();

            ////find dimensions
            //var dimensionExample = new DimensionExample(app);
            //var task2 = dimensionExample.ListDimensionsAsync();
            //task2.Wait();

            ////find current selections
            //var selectionExample = new SelectionExample(app);
            //var task3 = selectionExample.ListCurrentSelectionsAsync();
            //task3.Wait();

            //////find list object data
            //var listObjectExample = new ListObjectExample(app);
            //var task4 = listObjectExample.ListListObjectDataAsync();
            //task4.Wait();

            //var task5 = listObjectExample.GetGenericObjectAsync("Region");

            // GetListObjectDataAsync2 should work with the new qlik-engineAPI
            //var task6 = listObjectExample.GetListObjectDataAsync(task5.Result);

            //dynamic jsonObject = task6.Result;
            //  var jsonObject = task6.Result;
            //foreach (var item in jsonObject[0].qMatrix)
            //{
            //    Console.WriteLine(item[0].qText);
            //}

            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        private static void App_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("************* APP CHANGES *****************************");
        }

    }
}