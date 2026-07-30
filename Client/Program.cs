namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string _host = "127.0.0.1";
            const int _port = 8000;

            var client = new Client( _host, _port );

            await client.ConnectServer();
        }
    }
}
