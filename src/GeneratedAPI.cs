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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    #endregion

    #region ObjectResult
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class ObjectResult
    {
        [JsonProperty(Required = Required.Always)]
        public string QType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int QHandle { get; set; }

        public string QGenericType { get; set; } = "";
        public string QGenericId { get; set; } = "";
    } 
    #endregion

    #region GeneratedAPIResult
    /// <summary>
    /// Class to Handle the ondemand the result of methods calls of the GeneratedAPI Class
    /// </summary>
    public class GeneratedAPIResult : DynamicObject
    {
        #region Variables & Properties
        private Task<JToken> input = null;

        private Session session = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the GeneratedAPIResult
        /// </summary>
        /// <param name="input">The result of the engine method call as Task of a JSON Object</param>
        /// <param name="session">The enigma session</param>
        public GeneratedAPIResult(Task<JToken> input, Session session)
        {
            this.input = input;
            this.session = session;
        }
        #endregion

        #region Dynamic Methods
#pragma warning disable 1591 // no XML Comment Warning for override
        public override bool TryConvert(ConvertBinder binder, out object result)
        {            
            Type gArgs = binder.ReturnType.GetGenericArguments().FirstOrDefault() ?? typeof(object);

            if (!typeof(JToken).IsAssignableFrom(gArgs))
            {
                var tcs = new TaskCompletionSource(gArgs);

                input.ContinueWith((message) =>
                {
                    if (message.IsCanceled)
                        tcs.SetCanceled();
                    if (message.IsFaulted)
                        tcs.SetException(message.Exception);

                    var qReturn = message?.Result?.SelectToken("qReturn");
                    if (qReturn != null && qReturn.Type == JTokenType.Object && qReturn["qHandle"] != null)
                    {
                        var objectResult = qReturn.ToObject<ObjectResult>();
                        var newObj = new GeneratedAPI(objectResult, session);
                        session.GeneratedApiObjects.TryAdd(objectResult.QHandle, new WeakReference<GeneratedAPI>(newObj));
                        IObjectInterface ia = ImpromptuInterface.Impromptu.ActLike(newObj, gArgs);
                        tcs.SetResult(ia);
                    }
                    else
                    {
                        try
                        {
                            object newRes = null;
                            JToken resultToken = null;
                            var results = message.Result.Children().ToList();
                            if (results.Count == 1)
                            {
                                resultToken = results.FirstOrDefault().First();
                            }
                            else
                            {
                                resultToken = message.Result;
                            }

                            if (gArgs != typeof(object))
                                newRes = resultToken.ToObject(gArgs);
                            else
                                newRes = resultToken;

                            tcs.SetResult(newRes);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);                            
                        }
                    }
                }
                );

                result = tcs.Task;
                return true;
            }

            result = input;
            return true;
        }
#pragma warning restore 1591

        #endregion
    } 
    #endregion

    #region GeneratedAPI
    /// <summary>
    /// Generated API Object on the Engine side, tracked by qHandle
    /// </summary>
    public class GeneratedAPI : DynamicObject, IObjectInterface
    {
        #region Variables & Properties
        /// <summary>
        /// qGenericId of the engine object
        /// </summary>
        public string qGenericId { get; set; }

        /// <summary>
        /// qGenericType of the engine object
        /// </summary>
        public string qGenericType { get; set; }

        /// <summary>
        /// qType of the engine object for exampe (Doc, GenericObject, GenericBookmark,...)
        /// </summary>
        public string qType { get; set; }

        /// <summary>
        /// qHandle of the engine object
        /// </summary>
        public int qHandle { get; set; }

        /// <summary>
        /// The current enigma Session for this Generated API Object
        /// </summary>
        public Session Session { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Changed Event that is called if the engines notify the change of the Generated Object
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Closed Event that is called if the engines notify the close of the Generated Object
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Trigger the Changed Event for this  Generated Object
        /// </summary>
        public void OnChanged()
        {
            Changed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Trigger the Closed Event for this  Generated Object
        /// </summary>
        public void OnClosed()
        {
            Closed?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Construct GeneratedAPI Class 
        /// </summary>
        /// <param name="objectResult">The properties for this Generated API Object</param>
        /// <param name="session">The current enigma Session for this Generated API Object</param>
        internal GeneratedAPI(ObjectResult objectResult, Session session)
        {
            this.qGenericId = objectResult.QGenericId;
            this.qType = objectResult.QType;
            this.qGenericId = objectResult.QGenericType;
            this.qHandle = objectResult.QHandle;
            this.Session = session;
        }        
        #endregion

        #region Dynamic Methods        
#pragma warning disable 1591 // no XML Comment Warning for override
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
                    if (item is CancellationToken)
                        continue;

                    if (item == null)
                        break;
                    //    jArray.Add("");
                    //else

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

            if (Char.IsLower(request.Method[0]))
            {
                request.Method = char.ToUpper(request.Method[0])+ request.Method.Substring(1);
            }
            request.Parameters = jToken ?? JToken.Parse("{}");
            result = new GeneratedAPIResult(Session?.SendAsync(request, cts), Session);
            return true;
        }
#pragma warning restore  1591
        #endregion
    }
    #endregion
}
