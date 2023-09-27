using arriel.chat.application.Servidor.Services;
using arriel.chat.infrastructure.Delegates;
using System.Net;

namespace arriel.chat.server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("arriel.chat.server iniciado");            
            var chatServer = new ServidorChatAppService(IPAddress.Parse("127.0.0.1"));
            ServidorChatAppService.StatusChanged += new StatusChangedEventHandler(mainServer_statusChanged);
            chatServer.IniciarServidor();
            Console.WriteLine("Escutando mensagens...");
        }

        private static void mainServer_statusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine(e.EventMessage);
        }
    }
}