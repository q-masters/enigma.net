namespace SenseDesktop
{
    #region Usings
    using Dynamitey;
    using enigma;
    using ImpromptuInterface;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using NLog.Config;
    using Qlik.EngineAPI;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.WebSockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
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

        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            SetLoggerSettings("App.config");
            logger.Info("Start");

            //Result exception in TryConvert.
            var app = CreateConnection("Executive Dashboard");
            var mytasks = new List<Task>();
            var ce1 = new CalculationExample(app);
            ce1.CalcRandom(120);

            Task.WaitAll(mytasks.ToArray());

            var count = mytasks.Count;

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
            var example = new ChangeEventsExample(app);
            example.RunExample();

            var tasks = new List<Task>();
            //Set bookmark test
            var bookmark = app.GetBookmarkAsync("demobookmark").Result;

            //evaluate request
            var request = JObject.FromObject(new
            {
                qExpression = "'$(vCurrentYear)'"
            });

            //Use this Overload from EvaluateExAsync it works fine.
            var result = app.EvaluateExAsync(request).Result;

            //Use this Overload it crashes!!!
            result = app.EvaluateExAsync("'$(vCurrentYear)'").Result;

            //Caluculation Test
            var calc = new CalculationExample(app);
            calc.CalcRandom(1);

            //find the bookmark with type
            var bookmarkExample = new BookmarkExample(app);
            tasks.Add(bookmarkExample.ListBookmarksAsync());

            //find dimensions
            var dimensionExample = new DimensionExample(app);
            tasks.Add(dimensionExample.ListDimensionsAsync());

            //find current selections
            var selectionExample = new SelectionExample(app);
            tasks.Add(selectionExample.ListCurrentSelectionsAsync());

            ////find list object data
            var listObjectExample = new ListObjectExample(app);
            tasks.Add(listObjectExample.ListListObjectDataAsync());

            ////Fire Multiple Requests
            var multipleRequestsExample = new MultipleRequests(app);
            tasks.Add(multipleRequestsExample.FireMultipleRequestsAsync());

            Task.WaitAll(tasks.ToArray());

            var task5 = listObjectExample.GetGenericObjectAsync("Region");
            var task6 = listObjectExample.GetListObjectDataAsync(task5.Result);

            dynamic jsonObject = task6.Result;
            foreach (var item in jsonObject[0].qMatrix)
            {
                Console.WriteLine(item[0]?.qText + "");
            }

            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        private static void App_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("************* APP CHANGES *****************************");
        }

        private static IDoc CreateConnection(string appName)
        {
            try
            {
                var config = new EnigmaConfigurations()
                {
                    Url = $"ws://127.0.0.1:4848/app/engineData/identity/{Guid.NewGuid()}",

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

                IGlobal global = Impromptu.ActLike<IGlobal>(globalTask.Result);
                appName = SenseUtilities.GetFullAppName(appName);                
                return global.OpenDocAsync(appName).Result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
    }
}