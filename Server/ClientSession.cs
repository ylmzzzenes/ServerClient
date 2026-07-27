using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ClientSession
    {
        public string? Username { get;  set;  }
        public TcpClient Client { get; }
        public NetworkStream NetworkStream { get; }
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }


        public ClientSession(TcpClient client)
        {
            Client = client;
            NetworkStream = Client.GetStream();
            Reader = new StreamReader(NetworkStream);
            Writer = new StreamWriter(NetworkStream);
            Writer.AutoFlush = true;

        }
    
    }
}
