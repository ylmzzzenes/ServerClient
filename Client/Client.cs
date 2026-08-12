using System.Net.Sockets;

namespace Client
{
    public class Client
    {
        private readonly string _host;
        private readonly int _port;
        private readonly TcpClient _tcpClient;
        private NetworkStream? _networkStream;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        public Client(string host, int port)
        {
            _host = host;
            _port = port;
            _tcpClient = new TcpClient();
        }

        public async Task ConnectClient()
        {
            await _tcpClient.ConnectAsync(_host, _port);
            _networkStream = _tcpClient.GetStream();
            _reader = new StreamReader(_networkStream);
            _writer = new StreamWriter(_networkStream);
            _writer.AutoFlush = true;

            Console.WriteLine("Sunucuya bağlantı kuruldu.");
            Console.WriteLine("Kullanıcı adınızı giriniz:");
         
           string? response = await Register();

            if(response == null)
            {
                return;
            }


            Task receiveTask = ReceiveMessages();
            Task sendTask = SendMessages();

            await Task.WhenAll(receiveTask, sendTask);

        }

        private async Task SendMessages()
        {
            Console.WriteLine("Alıcı kullanıcı adı:");
            string receiveUsername = Console.ReadLine()!.Trim();
            while (true)
            {             

                string message = Console.ReadLine()!.Trim();

                
                string command = $"{receiveUsername}: {message}";

                await _writer!.WriteLineAsync(command);
            }
        }

        private async Task ReceiveMessages()
        {
;
            while (true)
            {
               
                string? message = await _reader!.ReadLineAsync();
               
                if (message == null)
                {
                    break;
                }

                Console.WriteLine(message);
            }
        }

        private async Task<string?> Register()
        {
            while (true)
            {

            Console.WriteLine("Lütfen kullanıcı adınızı giriniz");
            string? username = Console.ReadLine();
            if(username == null)
            {
                Console.WriteLine("Kullanıcı adı boş olamaz");
                return null;
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                continue;
            }
            username = username.Trim();

            await _writer.WriteLineAsync(username);

            string? response = await _reader.ReadLineAsync();
            if(response == null)
            {
                return null;
            }

            if(response == "Kullanıcı kaydı başarılı")
            {
                return username;
            }
                else
                {
                    Console.WriteLine(response);
                    continue;
                }
            
            }
        }
    }
}