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
            _tcpListener = new TcpListener(ipAddress, 8000);

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
                string? register = await Register(session);
                if(register == null)
                {
                    return;
                }
                
                    await Messaging(session);

               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            finally
            {
                if(session.Username != null)
                {
                    if(clients.TryGetValue(session.Username, out ClientSession? currentSession))
                    {
                        if (ReferenceEquals(session, currentSession))
                        {
                            clients.TryRemove(session.Username, out _);
                            Console.WriteLine($"{session.Username} bağlantısı kapandı.");
                        }
                    }
                   
                }
                session.Client.Close();

                
            }
        }        

        private async Task<string?> Register(ClientSession session)
        {
            while (true)
            {
                string? username = await session.Reader.ReadLineAsync();
                if(username is null)
                {
                    Console.WriteLine("Client bağlantıyı kapattı");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(username))
                {
                    await session.Writer.WriteLineAsync("HATA: Kullanıcı adı boş olamaz.");
                    Console.WriteLine("Kullanıcı adı boş gönderildi.");
                    continue;
                }
                username = username.Trim();

                if(clients.TryAdd(username, session))
                {
                    session.Username = username;
                    await session.Writer.WriteLineAsync("Kullanıcı kaydı başarılı");
                    Console.WriteLine($"{username} adlı kullanıcı adı başarıyla kaydedildi");
                   
                }
                else
                {
                    await session.Writer.WriteLineAsync("Bu kullanıcı adı zaten alınmış");
                    Console.WriteLine($"{username} adlı kullanıcı adı başkası tarafından kullanılıyor");
                    continue;
                }

                return username;
            }
            
        }

        private async Task Messaging(ClientSession session)
        {
            string? senderUsername = session.Username;
            if(senderUsername == null)
            {
                Console.WriteLine("Kayıtı tamamlanmamış bir nesne ile mesajlaşmaya devam edilemez");
                return;
            }
            while (true)
            {
                string? receivedMessage = await session.Reader.ReadLineAsync();

                if(receivedMessage == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(receivedMessage))
                {
                    Console.WriteLine("Gönderilen mesaj boş olamaz");
                    await session.SendAsync("Gönderilen mesaj boş olamaz");
                    continue;
                }

              string[] parts = receivedMessage.Split(":",2);

                if(parts.Length == 2 )
                {
                    string recipientUsername = parts[0];
                    string message = parts[1];

                    if (string.IsNullOrWhiteSpace(recipientUsername))
                    {
                        await session.SendAsync("Kullanıcı adınız boş formata uygun değil");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        await session.SendAsync("Mesajınız boş formata ugun değil");
                        continue;
                    }

                    recipientUsername = recipientUsername.Trim();
                    message = message.Trim();

                    if(clients.TryGetValue(recipientUsername, out ClientSession? targetSession))
                    {
                        bool response = await targetSession.SendAsync($"{senderUsername}:{message}");
                        if (response)
                        {                          
                            await session.SendAsync("Mesaj başarıyla gönderildi");
                        }
                        else
                        {
                            await session.SendAsync("Mesaj başarıyla gönderilmedi");
                        }
                    }
                    else
                    {
                        await session.SendAsync("Kullanıcı bulunamadı");
                        continue;
                    }

                       
                }
                else
                {
                    await session.SendAsync("Format hatalı. Alıcı : Mesaj ");
                    continue;
                }

            }

            
        }
    }

    }

