using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Server
    {
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<string, ClientSession> clients = new(StringComparer.OrdinalIgnoreCase);

        public Server(IPAddress ipAddress, int port)
        {
            _tcpListener = new TcpListener(ipAddress, port);

        }

        public async Task StartAsync()
        {
            _tcpListener.Start();
            Console.WriteLine($"Sunucu bağlantıları dinlemeye başladı");

            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Bağlantı kabul edildi {client.Client.RemoteEndPoint.ToString()}");
                ClientSession session =  new ClientSession(client);
               
                _ = HandleClientAsync(session);
            }

        }

        private async Task HandleClientAsync(ClientSession session)
        {
            try
            {
                var username = await session.Reader.ReadLineAsync();
                username = username.Trim();
                session.Username = username;
                clients[username] = session;
                await session.Writer.WriteLineAsync("Kullanıcı kaydı başarılı");
                Console.WriteLine($"{username} kullanıcı adı başarıyla kaydedildi.");

                while (true)
                {                   
                    string receivedMessage = await session.Reader.ReadLineAsync();
                    string[] parts = receivedMessage.Split(':', 2);
                    string receiveUsername = parts[0];
                    string message = parts[1];
                    await clients[receiveUsername].Writer.WriteLineAsync($"{username}:{message}");
                    await session.Writer.WriteLineAsync("Mesaj gönderildi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(session.Username != null)
                {
                    clients.TryRemove(session.Username, out _);
                    Console.WriteLine($"{session.Username} bağlantısı kapandı.");
                }
                session.Client.Close();
            }
        }        
    }

    }

