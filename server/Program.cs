using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ChatServer
{
    static async Task Main(string[] args)
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
        using Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSocket.Bind(ipPoint);
        serverSocket.Listen();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        while (true)
        {
            Socket clientSocket = await serverSocket.AcceptAsync(); 
            Console.WriteLine("Клиент подключен.");


            byte[] buffer = new byte[1024];
            int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            string username = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"Имя клиента: {username}");

            string okMessage = "OK";
            byte[] okData = Encoding.UTF8.GetBytes(okMessage);
            await clientSocket.SendAsync(okData, SocketFlags.None);
            Console.WriteLine($"Отправлено сообщение 'OK' клиенту {username}.");

           
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine($"Соединение с клиентом {username} закрыто.");
        }
    }
}
