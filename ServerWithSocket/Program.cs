using ServerWithSocket;
namespace ServerWithSocket
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Server Başlatılıyor");

            await ServerWithSocket.CreateServerSocket();

            Console.WriteLine("Server işini tamamladı, kapatmak için bir tuşa basın");
            Console.ReadKey();
        }
    }
}
