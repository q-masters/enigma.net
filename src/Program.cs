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


    public interface IGlobal: IGeneratedAPI
    {
        Task<JToken> IsDesktopMode();
        Task<IApp> OpenDoc(string json);
    }

    public interface IApp: IGeneratedAPI
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

            dynamic globalDyn = (GeneratedAPI)globalTask.Result;

           // var ev = ;
            ((Task<dynamic>)globalDyn.EngineVersion())
            .ContinueWith((res) => {
                    Console.WriteLine("EngineVER: " + res.Result.qComponentVersion.ToString());
                });

            


            IGlobal global = Impromptu.ActLike<IGlobal>(globalTask.Result);

            var IsDesktopModeTask = global.IsDesktopMode();        
            IsDesktopModeTask.Wait();
            Console.WriteLine("Result: " + IsDesktopModeTask.Result.ToString());

            global.OpenDoc(@"{ 'qDocName' : 'C:\\Users\\KMattheis\\Documents\\Qlik\\Sense\\Apps\\Executive Dashboard.qvf' }")
                    .ContinueWith((newApp) =>
                    {
                        Console.WriteLine("Object " + (newApp.Result).ToString());

                        var app = newApp.Result; //Impromptu.ActLike<IApp>(newApp.Result);

                        app.Changed += App_Changed;
                        app.GetScript()
                            .ContinueWith((script) =>
                            {
                                Console.WriteLine("Script" + script.Result.ToString().Substring(1,100));
                            });

                        app.SetScript(@"{qScript:'HALLO'}");

                    });



            Thread.Sleep(3000);
         
            Console.ReadLine();
        }

        private static void App_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("************* APP CHANGES *****************************");
        }

     

    }
}
