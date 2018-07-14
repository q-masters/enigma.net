namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Qlik.EngineAPI;
    #endregion

    /// <summary>
    /// Class for Enigma Sessions
    /// </summary>
    public class Session
    {
        internal ConcurrentDictionary<int, WeakReference<IObjectInterface>> GeneratedApiObjects = new ConcurrentDictionary<int, WeakReference<IObjectInterface>>();
        ConcurrentDictionary<int, TaskCompletionSource<JToken>> OpenRequests = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();

        ClientWebSocket socket = null;

        EnigmaConfigurations config;
        
        /// <summary>
        /// Constructor for a new 
        /// </summary>
        /// <param name="config"></param>
        public Session(EnigmaConfigurations config)
        {
            this.config = config;
        }

        #region Public
        /// <summary>
        /// Establishes the websocket against the configured URL.
        /// Eventually resolved with the QIX global interface when the connection has been established.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> OpenAsync(CancellationToken? ct= null)
        {
            CancellationToken ct2 = ct ?? CancellationToken.None;
            return await config.CreateSocketCall(ct2)
                .ContinueWith((res) =>
                {
                    socket = res.Result;
                    // Todo add here the global Cancelation Token that is
                    // triggered from the CloseAsync
                    this.ReceiveLoopAsync(ct2);
                    var global = new GeneratedAPI("Global", "Global", "Global", this, -1);

                    GeneratedApiObjects.TryAdd(-1, new WeakReference<IObjectInterface>(global));
                    return global;
                });
        }

        /// <summary>
        /// Closes the websocket and cleans up internal caches, also triggers the closed event on all generated APIs.
        /// Eventually resolved when the websocket has been closed.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            throw new NotImplementedException();
            //await new Task(() => { });
        }

        /// <summary>
        /// Suspends the enigma.js session by closing the websocket and rejecting all method calls until it has been resumed again.
        /// </summary>
        /// <returns></returns>
        public async Task SuspendAsync()
        {
            throw new NotImplementedException();
            //await new Task(() => { throw new NotSupportedException(); });
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
            throw new NotImplementedException();
            //await new Task(() => { throw new NotSupportedException(); });
        }
        #endregion

        private int requestID=0;
        
        internal async Task<JToken> SendAsync(JsonRpcRequestMessage request,  CancellationToken ct)
        {
            var sendID = Interlocked.Increment(ref requestID); ;
            var tcs = new TaskCompletionSource<JToken>();
            request.Id = sendID;
            OpenRequests.TryAdd(request.Id, tcs);
            string json = "";
            try {
              json = JsonConvert.SerializeObject(request);        
            }
            catch(Exception ex)
            {

            }
            Console.WriteLine("Send Request " + json);
                var bt = Encoding.UTF8.GetBytes(json);
            socket.SendAsync(bt, WebSocketMessageType.Text, true, ct);

            return await tcs.Task;            
        }

        private int initialRecieveBufferSize = 4096*8;        

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[initialRecieveBufferSize];

            try
            {
                while (true)
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
                    Console.WriteLine(response);
                    try
                    {
                        var message = JsonConvert.DeserializeObject<JsonRpcGeneratedAPIResponseMessage>(response);
                        OpenRequests.TryRemove(message.Id, out var tcs);

                        tcs?.SetResult(message.Result);

                        if (message?.Change != null)
                        {
                            foreach (var item in message.Change)
                            {
                                GeneratedApiObjects.TryGetValue(item, out var wkValues);
                                wkValues.TryGetTarget(out var generatedAPI);
                                generatedAPI?.OnChanged();
                            }
                        }
                    }

                    catch (Exception ex)
                    {

                    }

                    //this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs { Message = responce });
                }
            }
            catch (Exception ex)
            {
                //this.ErrorReceived?.Invoke(this, new SocketErrorEventArgs { Exception = ex });
            }
        }


    }
}
