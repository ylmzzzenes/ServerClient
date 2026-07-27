using ClientService = Client.Client;
namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string host = "127.0.0.1";
            const int port = 8000;


            var client = new Client(host,port);
        

            await client.ConnectClient();


        }
    }
}
