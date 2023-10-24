using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ServerApp
{
    class Server
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static int client_port = 8001;
        private static int server_port = 8000;
        private static string ip = "127.0.0.1";
        static UdpClient udpClient;
        private static async Task SendMessageAsync(string msg)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(msg);
                await udpClient.SendAsync(data, data.Length, ip, client_port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static async Task<string> ReceiveMessageAsync()
        {
            var remoteIP = (IPEndPoint)udpClient.Client.LocalEndPoint;
            string message = "";
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                message = Encoding.Unicode.GetString(result.Buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return message;
        }
        static async Task Main(string[] args)
        {
            try
            {
                udpClient = new UdpClient(server_port);
                Console.WriteLine($"Сервер запущен. Порт: {server_port}");
                await Task.Run(() => ProcessingRequest());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task ProcessingRequest()
        {
            Controller cr = new Controller();
            while (true)
            {
                string option = await ReceiveMessageAsync();
                string[] req = option.Split('|');
                switch (req[0])
                {
                    case "get_all":
                        {
                            Console.WriteLine($"Запрос всех записей. {client_port}");
                            try
                            {
                                await SendMessageAsync(cr.GetAll());
                            }
                            catch (Exception e)
                            {
                                logger.Error(e.ToString());
                            }
                            break;
                        }
                    case "save":
                        {
                            Console.WriteLine($"Сохранение БД. {client_port}");
                            try
                            {
                                cr.ClearDB();
                                string[] message = req[1].Split('\n');
                                cr.AddToFile(message);
                                Console.WriteLine("Сохранение успешно");
                            }
                            catch(Exception e)
                            {
                                logger.Error(e.ToString());
                            }
                            break;
                        }
                    case "clear":
                        {
                            Console.WriteLine($"Очистка БД. {client_port}");
                            cr.ClearDB();
                            break;
                        }
                    default:
                        {
                            await SendMessageAsync($"Неверный ввод {option}\n");
                            break;
                        }
                }
            }
        }
    }
}