namespace ServerBroadcast
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var serverBroadcast = new ServerBroadcast.socket();
             serverBroadcast.Server();
        }
    }
}
