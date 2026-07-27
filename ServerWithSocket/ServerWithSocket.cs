using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerWithSocket
{
    public class ServerWithSocket
    {
        public  static async Task CreateServerSocket()
        {
            var hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new(ipAddress, 11_000);

            using Socket listener = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            listener.Bind(ipEndPoint);
            listener.Listen(100);
           

            var handler = await listener.AcceptAsync();
            while (true)
            {

                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"Socket client received acknowledgment: \"{response}\"");
               
                // send message 
                var message = Console.ReadLine();
                var messageBytes = Encoding.UTF8.GetBytes(response);
                _ = await listener.SendAsync(messageBytes);
                Console.WriteLine($"Socket server sent message: \"{message}\"");

                
            }
        }
    }
}
