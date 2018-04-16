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
    #endregion

    public class Session
    {
        internal ConcurrentDictionary<int, WeakReference<GeneratedAPI>> GeneratedApiObjects = new ConcurrentDictionary<int, WeakReference<GeneratedAPI>>();
        ConcurrentDictionary<int, TaskCompletionSource<JToken>> OpenRequests = new ConcurrentDictionary<int, TaskCompletionSource<JToken>>();

        ClientWebSocket socket = null;

        public Session()
        {
            socket = new ClientWebSocket();
            socket.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(500);
        }

        #region Public
        public async Task<dynamic> OpenAsync()
        {
        
            return await socket.ConnectAsync(new Uri("ws://127.0.0.1:4848/app/engineData/"), CancellationToken.None)
                .ContinueWith((res) =>
                {
                    var ct = new CancellationToken();
                    this.ReceiveLoopAsync(ct);
                    var global = new GeneratedAPI("Global", "Global", "Global", this, -1);
                    
                    GeneratedApiObjects.TryAdd(-1, new WeakReference<GeneratedAPI>(global));                    
                    return global;
                });                    
        }

        public async Task CloseAsync()
        {
            await new Task(() => { });
        }

        public async Task SuspendAsync()
        {            
            await new Task(() => { throw new NotSupportedException(); });
        }

        public async Task ResumedAsync(bool onlyIfAttached = false)
        {
            await new Task(() => { throw new NotSupportedException(); });
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
        private bool autoIncreaseRecieveBuffer = true;

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
                            if (autoIncreaseRecieveBuffer)
                            {
                                Array.Resize(ref buffer, buffer.Length * 2);
                                writeSegment = new ArraySegment<byte>(buffer, writeSegment.Offset, buffer.Length - writeSegment.Offset);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Socket receive buffer overflow. Buffer size = {buffer.Length}. Buffer auto-increase = {autoIncreaseRecieveBuffer}");
                            }
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
