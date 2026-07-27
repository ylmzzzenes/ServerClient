using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerTcp
{
    public class Socket
    {
        private TcpListener _tcpListener;
        private NetworkStream _networkStream;
        private const int _port = 8000;
        private readonly static IPAddress _ip = IPAddress.Any;
        private byte[] buffer = new byte[1024];
        Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        private int bytesRead;
        private StringBuilder receivedData = new StringBuilder();
        private readonly object _lock = new object();

        public async Task Server()
        {
            int count = 1;

            // var hostName = Dns.GetHostName();
            // IPHostEntry localhost = await Dns.GetHostEnryAsync(hostName);
            // IPAddress ipAddress = localhost.AddressList[0] ;
            // var is EndPoint = new IPEndPoint(ipAddress, 13);
            _tcpListener = new TcpListener(_ip, _port);
            _tcpListener.Start();
            Console.WriteLine($"Dinleme başlatıldı: {_ip} - {_port}");


            _ = Task.Run (() => CommandPrompt());

            while (true)
            {
                TcpClient _tcpClient = _tcpListener.AcceptTcpClient();
                lock (_lock) clients.Add(count, _tcpClient);
                Console.WriteLine($" Bağlantı başarılı: {_tcpClient.Client.RemoteEndPoint.ToString()}");

                _ = Task.Run(() => ReceivedDataAsync(_networkStream)); 

            }
        }

        public async Task ReceivedDataAsync(NetworkStream _networkStream)
        {
            try
            {
                Console.WriteLine("Kullanıcı adınızı giriniz:");
                var kullaniciAdi = Console.ReadLine();
                
                while ((bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    receivedData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    Console.WriteLine(receivedData.ToString());
                    receivedData.Clear();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Okuma hatası : {ex.Message}");
                _networkStream.Close();
            }
        }

        public async Task CommandPrompt()
        {
            while(true)
            {

            var input = Console.ReadLine();
            var mesaj = Encoding.UTF8.GetBytes(input);
            await _networkStream.WriteAsync(mesaj, 0, mesaj.Length);
           
            }
        }

       
    }
}
