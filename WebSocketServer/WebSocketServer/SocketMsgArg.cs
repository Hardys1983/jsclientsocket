using System.Net.WebSockets;

namespace WebSocketServer
{
    public class SocketMessageArg
    {
        public WebSocketMessageType MessageType { get; set; }
        public string Content { get; set; }
    }
}
