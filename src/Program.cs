using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ImpromptuInterface;
using Dynamitey;

namespace enigma
{
    public static class TaskThenExtensionMethods { 
    
    //    public static async Task Then(this Task antecedent, Action continuation)
    //    {
    //        await antecedent;
    //        continuation();
    //    }

    //    public static async Task<TNewResult> Then<TNewResult>(this Task antecedent, Func<TNewResult> continuation)
    //    {
    //        await antecedent;
    //        return continuation();
    //    }

    //    public static async Task Then<TResult>(this Task<TResult> antecedent, Action<TResult> continuation)
    //    {
    //        continuation(await antecedent);
    //    }

    //    public static async Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> antecedent, Func<TResult, TNewResult> continuation)
    //    {
    //        return continuation(await antecedent);
    //    }

           //public static Task Unwarp(this dynamic tt)
           // {
           //     return 
           // }

        //public static Task Then<TResult>(this Task<TResult> @this, Func<TResult, Task> continuation)
        //{
        //    @this.Wait();
        //    return new Task(async () =>
        //    {
        //        await continuation.Invoke(@this.Result);
        //    });
            
        //}
    }


    public interface IGlobal: IGeneratedAP
    {
        Task<JToken> IsDesktopMode();
        Task<dynamic> OpenDoc(string json);
    }

    public interface IApp: IGeneratedAP
    {
        Task<JToken> GetScript();
        Task<JToken> SetScript(string json);


    }


    class Program
    {
        static void Main(string[] args)
        {
            //  Main2(null);
          
            var session = new Session();

            var globalTask = session.OpenAsync();
            globalTask.Wait();

            IGlobal global = Impromptu.ActLike<IGlobal>(globalTask.Result);


            var IsDesktopModeTask = global.IsDesktopMode();        
            IsDesktopModeTask.Wait();
            Console.WriteLine("Result: " + IsDesktopModeTask.Result.ToString());

            global.OpenDoc(@"{ 'qDocName' : 'C:\\Users\\KMattheis\\Documents\\Qlik\\Sense\\Apps\\Executive Dashboard.qvf' }")
                    .ContinueWith((newApp) =>
                    {
                        Console.WriteLine("Object " + (newApp.Result).ToString());
                                              
                        IApp app = Impromptu.ActLike<IApp>(newApp.Result);

                        app.Changed += App_Changed;
                        app.GetScript()
                            .ContinueWith((script) =>
                            {
                                Console.WriteLine("Script" + script.Result.ToString().Substring(1,100));
                            });

                        app.SetScript(@"{qScript:'HALLO'}");

                    });

            //   var isDesktop = (string)global.Result.IsDesktopMode();

            //isDesktop.input.Wait();

            //var tt = isDesktop as string;
            //Console.WriteLine("Result: " + isDesktop.input.Result.ToString());


            //.ContinueWith((globalTR) =>
            //{
            //    //.ContinueWith(
            //    //    (result) => {
            //    //    Console.WriteLine("Engine VErsion: " + result.Result.ToString());
            //    //}
            //    //;

            //    try
            //    {
            //        var mm = globalTR.Result.IsDesktopMode();


            //        var bn = mm as Task<JToken>;
            //        Console.WriteLine(bn);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("ex #######", ex.ToString());

            //    }


            //    //bn.ContinueWith((result) => {
            //    // Console.WriteLine("IsDesktop: "+ result.Result.ToString());
            //    //}
            //    //);

            //   
            //});



            //.ContinueWith((evTR) =>
            //{
            //    Console.WriteLine("reachedn");

            //    Console.WriteLine(evTR.Result);
            //});



            //session.SendAsync(ssd, CancellationToken.None).ContinueWith((res) =>
            //{
            //    Console.WriteLine("RESULT: " + res.Result.ToString());
            //});

            Thread.Sleep(3000);

            //Enigma.Create(new EnigmaConfigurations())
            //    .OpenAsync()
            //    .ContinueWith((global) =>
            //    {
            //        return (global.Result.test) as Task<bool>;
            //    });              


            var tn = new GeneratedAPI("", "", "", null, -1) as dynamic;
            var mk = "{ type: 'qBarchart' }";
            var tn2 = JToken.Parse(mk);

                
            //tn.GetOpenDoc(tn2);
            //tn.GetOpenDoc(mk);       
            //tn.GetOpenDoc(mk);
            Console.ReadLine();
        }

        private static void App_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("************* APP CHANGES *****************************");
        }

        //static async void Main2(string[] args)
        //{
        //    //using (var cts = new CancellationTokenSource())
        //    //using (var client = new WebSocketTextClient(cts.Token))
        //    //{
        //    //    client.MessageReceived += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);

        //    //    await client.ConnectAsync(new Uri("ws://127.0.0.1:4848/app/engineData"));
        //    //    await client.SendAsync("ping");
        //    //    await Task.Delay(5000);
        //    //}
        //    //var tt2 = new JsonRpc(new WebSocketMessageHandler())

        //    //ClientWebSocket socket = new ClientWebSocket();
        //    //var socket2 = socket.ConnectAsync(new Uri("wss://nb-fc-t460s-04:443/app/engineData/"), new System.Threading.CancellationToken());
        //    //socket2.Wait();     

        //    //Console.WriteLine("teste"+ socket.State);
        //}


    }
}
