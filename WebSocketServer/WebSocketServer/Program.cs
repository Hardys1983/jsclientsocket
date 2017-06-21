using System;
using System.Threading;

namespace WebSocketServer
{
    class Program
    {
        private static readonly WebsocketServer Server = new WebsocketServer("http://localhost:52000/Fingerprint/", OnMessageArrive);

        static void Main(string[] args)
        {
            Server.Start();
            
            while (true)
            {
                Console.WriteLine("Enter some comment to send:");
                var comment = Console.ReadLine();
                Server.SendAsync(comment).Wait(CancellationToken.None);
                Server.Stop();
                Server.Start();
            }
        }

        private static async void OnMessageArrive(object sender, SocketMessageArg socketMessageArg)
        {
            Console.WriteLine($"New message: {socketMessageArg.MessageType} = {socketMessageArg.Content}");
            await Server.SendAsync("Hello world!!!");
        }
    }
}
