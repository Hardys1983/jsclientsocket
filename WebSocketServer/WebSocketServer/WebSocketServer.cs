using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServer
{
    public class WebsocketServer
    {
        private readonly IList<WebSocket> _connectedClients = new List<WebSocket>();
        private readonly HttpListener _httpListener = new HttpListener();

        public event EventHandler<SocketMessageArg> OnMessageArrive;

        public WebsocketServer(string httpListenerPrefix, EventHandler<SocketMessageArg> onMessageArrive)
        {
            _httpListener.Prefixes.Add(httpListenerPrefix);

            OnMessageArrive += onMessageArrive;
            _httpListener.Start();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_httpListener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(async c =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                await ProcessRequestAsync(ctx);
                            }
                            catch
                            {
                                // suppress any exceptions  
                            }
                            finally
                            {
                                ctx?.Response.OutputStream.Close();
                            }
                        }, _httpListener.GetContext());
                    }
                }
                catch
                {
                    // suppress any exceptions  
                }
            });
        }

        private async Task ProcessRequestAsync(HttpListenerContext httpListenerContext)
        {
            WebSocket webSocket;
            try
            {
                var webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);
                webSocket = webSocketContext.WebSocket;

                Console.WriteLine($"Accepting socket #{webSocket.GetHashCode()}");
                _connectedClients.Add(webSocket);
                Console.WriteLine($"Clients count = {_connectedClients.Count}");
            }
            catch
            {
                httpListenerContext.Response.StatusCode = 500;
                httpListenerContext.Response.Close();
                return;
            }

            var receiveBuffer = new byte[1024];
            if (webSocket.State == WebSocketState.Open)
            {
                while (true)
                {
                    var receiveResult =
                        await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"Closing socket #{webSocket.GetHashCode()}");
                        _connectedClients.Remove(webSocket);
                        Console.WriteLine($"Clients count = {_connectedClients.Count}");
                        break;
                    }

                    if (receiveResult.MessageType == WebSocketMessageType.Text && receiveResult.Count > 0)
                    {
                        OnMessageArrive?.Invoke(this, new SocketMessageArg
                        {
                            MessageType = receiveResult.MessageType,
                            Content = Encoding.Default.GetString(receiveBuffer, 0, receiveResult.Count)
                        });
                    }
                }
            }
        }

        public async Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var data = new ArraySegment<byte>(bytes, 0, bytes.Length);

            foreach (var client in _connectedClients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
