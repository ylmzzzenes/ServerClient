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
        private SemaphoreSlim WriteLock { get; }


        public ClientSession(TcpClient client)
        {
            Client = client;
            NetworkStream = Client.GetStream();
            Reader = new StreamReader(NetworkStream);
            Writer = new StreamWriter(NetworkStream);
            Writer.AutoFlush = true;
            WriteLock = new SemaphoreSlim(1);

        }

        public async Task<bool> SendAsync(string message)
        {
            await WriteLock.WaitAsync();
            try
            {
                await Writer.WriteLineAsync(message);
                return true;
            }
            //IOException
            //ObjectDisposedException
            //OperationCanceledException
            catch(IOException)
            {
                return false;
            }
            catch(ObjectDisposedException)
            {
                return false;

            }
            finally
            {
                WriteLock.Release();
            }
        }
    
    }
}
