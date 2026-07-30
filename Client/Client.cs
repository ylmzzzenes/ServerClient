using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client
    {
        private readonly string _host;
        private readonly int _port;
        private readonly TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        
        public Client(string host, int port)
        {
            _host = host;
            _port = port;
            _tcpClient = new TcpClient();
        }

        public async Task ConnectServer()
        {
              _tcpClient.Connect(_host, _port);
            _networkStream = _tcpClient.GetStream();
            _streamReader = new StreamReader(_networkStream);
            _streamWriter = new StreamWriter(_networkStream);
            _streamWriter.AutoFlush  = true;
            Console.WriteLine("Bağlantı kuruldu");
            Console.WriteLine("Kullanıcı adınızı giriniz");

            var username = Console.ReadLine().Trim();

            await _streamWriter.WriteLineAsync(username);

            
            Task receiveTask = ReceiveMessages();
            Task sendTask = SendMessages();

            Task.WhenAll(receiveTask,sendTask);


        }

        private async Task SendMessages()
        {
            Console.WriteLine("Alıcı ismini giriniz");
            string receiveUsername = Console.ReadLine().Trim();

            while (true)
            {
                string message = Console.ReadLine().Trim();
                var command = $"{receiveUsername}:{message}";

                await _streamWriter.WriteLineAsync(command);
            }
        }

        private async Task ReceiveMessages()
        {
            while (true)
            {

             string? messages = await _streamReader.ReadLineAsync();

            if(messages == null)
            {
                break;
            }

            Console.WriteLine(messages);
            }
        }
    }
}
