namespace enigma
{    
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using Newtonsoft.Json.Linq;    
    using Qlik.EngineAPI;
    #endregion

    public class ObjectInterface
    {
        public string qType { get; set; }
        public int qHandle { get; set; }
        public string qGenericType { get; set; }
        public string qGenericId { get; set; }
    }

    public class GeneratedAPIResult : DynamicObject
    {
        private Task<JToken> input = null;

        private Session session = null;

        public GeneratedAPIResult(Task<JToken> input, Session session)
        {
            this.input = input;
            this.session = session;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var gArgs = binder.ReturnType.GetGenericArguments();

          //  if (typeof(IGeneratedAPI).IsAssignableFrom(gArgs[0]))
            if (gArgs.Length == 0 || !typeof(JToken).IsAssignableFrom(gArgs[0]))
            {
                Type gTCS;
                if (gArgs.Length == 0)
                    gTCS = typeof(TaskCompletionSource<>).MakeGenericType(typeof(object));
                else
                    gTCS = typeof(TaskCompletionSource<>).MakeGenericType(gArgs);

                object tcsO = Activator.CreateInstance(gTCS);

                var SetResult = gTCS.GetMethod("SetResult");

                input.ContinueWith((message) => {
                    var qReturn = message?.Result?.SelectToken("qReturn");
                    if (qReturn != null && qReturn.Type == JTokenType.Object && qReturn["qHandle"] != null)
                    {
                        int handle = int.Parse(qReturn["qHandle"].ToString());
                        Console.WriteLine($"new OBJECT handle: {handle}");
                        var newObj = new GeneratedAPI(qReturn["qGenericId"].ToString(), "", "", session, handle);
                        IObjectInterface ia = ImpromptuInterface.Impromptu.ActLike(newObj, gArgs);                   
                        session.GeneratedApiObjects.TryAdd(handle, new WeakReference<IObjectInterface>(ia));
                        //tcs.SetResult(ia);
                        SetResult.Invoke(tcsO, new object[] { ia });
                    }
                    else
                    {
                        try
                        {
                            object newRes = message?.Result?.SelectToken("qVersion")?.ToObject(gArgs[0]);
                            SetResult.Invoke(tcsO, new object[] { newRes });
                        }
                        catch(Exception ex)
                        {
                            SetResult.Invoke(tcsO, new object[] { null });
                        }
                    }
                }
                );

                var getTask = gTCS.GetProperty("Task");
                result = getTask.GetValue(tcsO);
                return true;
            }

            result = input;
            return true;

        }

    }

    #region GeneratedAPI
    public class GeneratedAPI : DynamicObject, IObjectInterface
    {
        #region Variables & Properties
        public string qGenericId { get;  set; }

        public string qType { get;  set; }

        public string qGenericType { get;  set; }

        public Session Session { get; private set; }

        public int qHandle { get;  set; }
        #endregion

        #region Events
        public event EventHandler Changed;

        public event EventHandler Closed;

        public void OnChanged()
        {
            Changed?.Invoke(this, new EventArgs());
        }

        public void OnClosed()
        {
            Closed?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Constructor
        public GeneratedAPI(string Id, string Type, string GenericType, Session Session, int Handle)
        {
            // ToDo: check if all Parameters are okay?
            this.qGenericId = Id;
            this.qType = Type;
            this.qGenericType = GenericType;
            this.Session = Session;
            this.qHandle = Handle;
        }
        #endregion

        #region Dynamic Methods
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Session == null)
            {
                result = null;
                return true;
            }

            var argList = args.ToList();
            var ctsArg = argList.OfType<CancellationToken>().ToList();
            CancellationToken cts= ctsArg.SingleOrDefault();

            JToken jToken = null;

            if (args.Length == 1 || (args.Length==2 && ctsArg.Count == 1))
            {
                // special case one real argument beside CancellationToken
                // check for string or JToken

                if (args[0] is JToken innerJToken)
                {
                    jToken = innerJToken;
                }
                if (args[0] is string innerString)
                {
                    try
                    {
                        jToken = JToken.Parse(innerString);
                    }
                    catch
                    {
                    }
                }
            }

            if (jToken == null)
            {
                var jArray = new JArray();
                foreach (var item in args)
                {
                    if (item == null)
                        break;

                    if (item is CancellationToken)
                        continue;

                    jArray.Add(item);
                }
                jToken = jArray;
            }


            // ToDo: enhance parametercheck for enigma Parametermode with loaded schema file                      
            var request = new JsonRpcGeneratedAPIRequestMessage
            {
                Handle = this.qHandle,
                Method = binder.Name
            };
            if (request.Method.EndsWith("Async"))
                request.Method=request.Method.Substring(0, request.Method.Length - 5);

            //if (Char.IsLower(request.Method[0]))
            //    request.Method[0] = char.ToUpper(request.Method[0]);
            request.Parameters = jToken ?? JToken.Parse("{}");
            result = new GeneratedAPIResult(Session?.SendAsync(request, cts), Session);
            return true;
        }
        #endregion
    } 
    #endregion
}
