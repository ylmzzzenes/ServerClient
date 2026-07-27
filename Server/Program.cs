using System.Net;
using System.Net;
namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            const int port = 8000;

            var server = new Server(
                IPAddress.Loopback,
                port);

            await server.StartAsync();
        }
    }
}
