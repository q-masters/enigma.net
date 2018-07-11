#region Usings
using System;
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
using Qlik.EngineAPI;

#endregion

namespace enigma {
    #region ToDos
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
    #endregion

    class Program {
        static void Main (string[] args) {
            var session = new Session ();
            
            var globalTask = session.OpenAsync ();
            globalTask.Wait ();

            dynamic globalDyn = (GeneratedAPI) globalTask.Result;

            ((Task<dynamic>) globalDyn.EngineVersion())
            .ContinueWith ((res) => {
                Console.WriteLine ("EngineVER: " + res.Result.qComponentVersion.ToString ());
            });

            IGlobal global = Impromptu.ActLike<IGlobal> (globalTask.Result);


            global.EngineVersionAsync()
                .ContinueWith ((engVer) => {
                    Console.WriteLine ("CastedEngineVer:"+ engVer.Result.qComponentVersion);
                });
            


            CancellationToken ct = new CancellationToken();
            

            global.OpenDocAsync (@"C:\\Users\\KMattheis\\Documents\\Qlik\\Sense\\Apps\\Executive Dashboard.qvf", token: ct)
                .ContinueWith ((newApp) => {
                    
                    Console.WriteLine ("Object " + (newApp.Result).ToString ());

                    var app = newApp.Result;

                    app.Changed += App_Changed;
                    app.GetScriptAsync ()
                        .ContinueWith ((script) => {
                            Console.WriteLine ("Script" + script.Result.ToString ().Substring (1, 100));
                        });

                    app.SetScriptAsync(@"{qScript:'HALLO'}")
                        .ContinueWith((res) =>
                        {
                            app.GetScriptAsync()
                                .ContinueWith((script) => {
                                    Console.WriteLine("Script2" + script.Result.ToString().Substring(1, 100));
                                });
                        });

                });

            Thread.Sleep (3000);

            Console.ReadLine ();
        }

        private static void App_Changed (object sender, EventArgs e) {
            Console.WriteLine ("************* APP CHANGES *****************************");
        }

    }
}