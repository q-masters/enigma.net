namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;    
    using System.Net.WebSockets;
	using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    #endregion

    #region Session
    /// <summary>
    /// Class for Enigma Sessions
    /// </summary>
    public class Session
    {
        #region Logger
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Variables / Properties
        internal ConcurrentDictionary<int, WeakReference<GeneratedAPI>> GeneratedApiObjects = new ConcurrentDictionary<int, WeakReference<GeneratedAPI>>();
        private ConcurrentDictionary<int, TaskCompletionSource<JToken>> OpenRequests = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();

        private ClientWebSocket socket = null;

        private EnigmaConfigurations config;

        private int requestID = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the Session Class
        /// </summary>
        /// <param name="config">The enigma configuration</param>
        public Session(EnigmaConfigurations config)
        {
            this.config = config;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Establishes the websocket against the configured URL.
        /// Try to get the QIX global interface when the connection has been established.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> OpenAsync(CancellationToken? ct = null)
        {
            CancellationToken ct2 = ct ?? CancellationToken.None;
            return await config.CreateSocketCall(ct2)
                .ContinueWith((res) =>
                {
                    socket = res.Result;
                    // Todo add here the global Cancelation Token that is
                    // triggered from the CloseAsync
                    this.ReceiveLoopAsync(ct2);
                    Task.Run(() => { this.SendLoop(ct2); });
                    var global = new GeneratedAPI(new ObjectResult() { QHandle = -1, QType = "Global" }, this);
                    GeneratedApiObjects.TryAdd(-1, new WeakReference<GeneratedAPI>(global));
                    return global;
                }, continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        #region Helpers
        private void CloseOpenRequests()
        {
            lock (OpenRequests)
            {
                foreach (var item in OpenRequests.Keys)
                {
                    try
                    {
                        if (OpenRequests.TryRemove(item, out var value))
                        {
                            value.SetCanceled();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
        }

        private void ClearGeneratedApiObjects()
        {
            lock (GeneratedApiObjects)
            {
                foreach (var item in GeneratedApiObjects.Keys)
                {
                    try
                    {
                        if (GeneratedApiObjects.TryRemove(item, out var value))
                        {
                            if (value.TryGetTarget(out var target))
                                target.OnClosed();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
        }

        private void ClearSendRequests()
        {
            lock (OpenSendRequest)
            {
                while (!OpenSendRequest.IsEmpty) OpenSendRequest.TryDequeue(out var _);
            }
        } 
        #endregion

        /// <summary>
        /// Closes the websocket and cleans up internal caches, also triggers the closed event on all generated APIs.
        /// Eventually resolved when the websocket has been closed.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync(CancellationToken? ct = null)
        {
            if (socket == null)
                return;
            
			 await socket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "", ct ?? CancellationToken.None)
				.ContinueWith((res) =>
                {
                    try
                    {
                        ClearGeneratedApiObjects();

                        CloseOpenRequests();

                        ClearSendRequests();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    socket.Dispose();
                    socket = null;
                });
        }


        /// <summary>
        /// Suspends the enigma.js session by closing the websocket and rejecting all method calls until it has been resumed again.
        /// </summary>
        /// <returns></returns>
        public async Task SuspendAsync()
        {            
            await Task.Run(() => { throw new NotSupportedException(); });
        }

        /// <summary>
        /// Resume a previously suspended enigma.js session by re-creating the websocket and, if possible, re-open the document as well as refreshing the internal caches. If successful, changed events will be triggered on all generated APIs, and on the ones it was unable to restore, the closed event will be triggered.
        /// 
        /// Eventually resolved when the websocket (and potentially the previously opened document, and generated APIs) has been restored, rejected when it fails any of those steps, or when onlyIfAttached is true and a new QIX Engine session was created.
        /// </summary>
        /// <param name="onlyIfAttached">onlyIfAttached can be used to only allow resuming if the QIX Engine session was reattached properly.</param>
        /// <returns></returns>
        public async Task ResumedAsync(bool onlyIfAttached = false)
        {
            await Task.Run(() => { throw new NotSupportedException(); });
        }
        #endregion

        #region SendAsync
        internal async Task<JToken> SendAsync(JsonRpcRequestMessage request, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<JToken>();
            try
            {
                request.Id = Interlocked.Increment(ref requestID);

                string json = "";
                try
                {
                    json = JsonConvert.SerializeObject(request,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore,
                           });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                logger.Trace("Send Request " + json);                
                var bt = Encoding.UTF8.GetBytes(json);
                ArraySegment<byte> nm = new ArraySegment<byte>(bt);
                OpenRequests.TryAdd(request.Id, tcs);
                OpenSendRequest.Enqueue(new SendRequest() { ct = ct, tcs = tcs, message = nm, id = request.Id });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);                
            }

            return await tcs.Task;
        }
        #endregion

        internal class SendRequest
        {
            public ArraySegment<byte> message;
			public int id;
            public CancellationToken ct;
            public TaskCompletionSource<JToken> tcs;
        }

        private ConcurrentQueue<SendRequest> OpenSendRequest = new ConcurrentQueue<SendRequest>();

        private void SendLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && socket != null && socket?.State == WebSocketState.Open)
                {
                    while (!OpenSendRequest.IsEmpty)
                    {
                        if (OpenSendRequest.TryDequeue(out SendRequest sr))
                        {
                            socket.SendAsync(sr.message, WebSocketMessageType.Text, true, sr.ct)
                                  .ContinueWith(
                                       result =>
                                       {										  
										   if (result.IsFaulted || result.IsCanceled)
											   OpenRequests.TryRemove(sr.id, out var _);
										   if (result.IsFaulted)
                                               sr.tcs.SetException(result.Exception);
                                           if (result.IsCanceled)
                                               sr.tcs.SetCanceled();
                                       })
#if NET452
                                  .Wait(sr.ct)
#endif
                                       ;
                        }
                    }

                    Thread.Sleep(10);
                }

                CloseOpenRequests();                

                ClearSendRequests();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region ReceiveLoopAsync
#pragma warning disable CS4014 
        private async void ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096 * 8];

            try
            {
                while (!cancellationToken.IsCancellationRequested && socket != null && socket.State == WebSocketState.Open)
                {
                    var writeSegment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await socket.ReceiveAsync(writeSegment, cancellationToken);                        
                        writeSegment = new ArraySegment<byte>(buffer, writeSegment.Offset + result.Count, writeSegment.Count - result.Count);

                        // check buffer overflow
                        if (!result.EndOfMessage && writeSegment.Count == 0)
                        {
                            // autoIncreaseRecieveBuffer)
                            Array.Resize(ref buffer, buffer.Length * 2);
                            writeSegment = new ArraySegment<byte>(buffer, writeSegment.Offset, buffer.Length - writeSegment.Offset);
                        }

                    } while (!result.EndOfMessage);

                    var response = Encoding.UTF8.GetString(buffer, 0, writeSegment.Offset);
                    logger.Trace("Reponse" + response);
                    try
                    {
                        var message = JsonConvert.DeserializeObject<JsonRpcGeneratedAPIResponseMessage>(response);

                        if (message != null)
                        {
                            OpenRequests.TryRemove(message.Id, out var tcs);
                            if (message.Error != null)
                            {
                                tcs?.SetException(new Exception(message.Error?.ToString()));
                            }
                            else
                                tcs?.SetResult(message.Result);

                            if (message?.Change != null)
                            {
                                foreach (var item in message.Change)
                                {
                                    logger.Trace($"Object Id: {item} changed.");
                                    GeneratedApiObjects.TryGetValue(item, out var wkValues);
                                    if (wkValues != null)
                                    {
                                        wkValues.TryGetTarget(out var generatedAPI);
                                        Task.Run(() =>
                                        {
                                            try
                                            {
                                                generatedAPI?.OnChanged();
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex);
                                            }
                                        }
                                        );
                                    }
                                }
                            }

                            if (message?.Closed != null)
                            {
                                foreach (var item in message.Closed)
                                {
                                    logger.Trace($"Object Id: {item} closed.");
                                    GeneratedApiObjects.TryRemove(item, out var wkValues);
                                    wkValues.TryGetTarget(out var generatedAPI);
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            generatedAPI?.OnClosed();
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex);
                                        }
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                ClearGeneratedApiObjects();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
#pragma warning restore CS4014 
        #endregion
    }
    #endregion
}
