using System;
using System.Net.Sockets;
using System.Text;


class ChatClient
{
    private static Socket clientSocket;

    static async Task Main(string[] args)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await socket.ConnectAsync("127.0.0.1", 8888);
            Console.WriteLine($"Подключение к {socket.RemoteEndPoint} установлено");
            clientSocket = socket;

            Console.Write("Введите ваше имя: ");
            string username = Console.ReadLine();
            await SendMessage(username);

            
            string serverResponse = await ReceiveMessage();
            if (serverResponse != "OK")
            {
                Console.WriteLine("Ошибка: не удалось получить подтверждение от сервера.");
                return;
            }

            Console.WriteLine("Получено 'OK' от сервера.");
        }
        catch (SocketException)
        {
            Console.WriteLine($"Не удалось установить подключение с {socket.RemoteEndPoint}");
        }
        finally
        {
            CloseConnection();
        }

        Console.WriteLine("Нажмите любую клавишу, чтобы закрыть это окно: ");
        Console.ReadKey(true); 
    }

    private static async Task SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await clientSocket.SendAsync(data, SocketFlags.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
        }
    }

    private static async Task<string> ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
    }

    private static void CloseConnection()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Ошибка при завершении соединения: {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
                Console.WriteLine("Соединение с сервером закрыто.");
            }
        }
    }
}
