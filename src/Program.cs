using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

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

    class Program
    {
        static void Main(string[] args)
        {
            //  Main2(null);

            var session = new Session();
            var ssd = new JsonRpcGeneratedAPIRequestMessage();
            ssd.Delta = false;
            ssd.Handle = -1;
            ssd.Method = "EngineVersion";
            ssd.Id = 1;
            ssd.Parameters = JToken.Parse("{}");
            session.SendAsync(ssd, CancellationToken.None).ContinueWith((res) =>
            {
                Console.WriteLine("RESULT: " + res.Result.ToString());
            });

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


           

          

            tn.GetOpenDoc(tn2);
            tn.GetOpenDoc(mk);
            Console.ReadLine();
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
