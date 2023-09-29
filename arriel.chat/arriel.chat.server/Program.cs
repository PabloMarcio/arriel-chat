using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace arriel.chat.server
{
    public class Program
    {
        static Dictionary<string, StreamWriter> Usuarios = new Dictionary<string, StreamWriter>();
        static Dictionary<StreamWriter, string> Conexoes = new Dictionary<StreamWriter, string>();

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("arriel.chat.server iniciado");
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 2502);
            listener.Start();
            Console.WriteLine("Escutando mensagens...");
            while (true)
            {
                var client = listener.AcceptTcpClientAsync().Result;
                Thread clientThread = new(() => HandleClient(client));
                clientThread.Start();                
            }
        }

        private static async void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new(stream);
                StreamWriter writer = new(stream);
                DateTime threadTimeStamp = DateTime.Now;
                while (true) 
                {
                    var message = await reader.ReadLineAsync();
                    var parsedMessage = message.Split(" ");
                    if (parsedMessage.Length == 0)
                    {
                        continue;
                    }
                    if (DateTime.Now > threadTimeStamp.AddMinutes(5))
                        break; // 5 minutos de inatividade quebra o loop
                    var command = parsedMessage[0];
                    switch (command) 
                    {
                        case "/join":
                            await ProcessJoinCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        case "/who":
                            await ProcessWhoCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        case "/help":
                            await ProcessHelpCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        case "/whisper":
                            await ProcessWhisperCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        case "/msg":
                            await ProcessMsgCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        case "/quit":
                            await ProcessQuitCommand(parsedMessage, threadTimeStamp, writer);
                            break;
                        default:
                            await WarnUnknownMessage(message, threadTimeStamp, writer);
                            break;
                    }                    
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally 
            {
                client.Close();
            }
        }

        private static Task ProcessQuitCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            throw new NotImplementedException();
        }

        private static Task ProcessMsgCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            throw new NotImplementedException();
        }

        private static Task WarnUnknownMessage(string message, DateTime threadTimeStamp, StreamWriter writer)
        {
            throw new NotImplementedException();
        }

        private static Task ProcessWhisperCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            throw new NotImplementedException();
        }

        private static async Task ProcessHelpCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            writer.WriteLineAsync($@"/help ativado - comandos disponíveis nesse chat:
/help - chama a ajuda
/whisper ""Apelido"" <mensagem> - manda uma mensagem privada para a pessoa citada em Apelido, contanto que alguém esteja conectado com esse apelido na sala
/who - obtém uma lista de usuários que estão na sala
/quit - desconecta do chat");
            writer.FlushAsync();    
        }

        private static Task ProcessWhoCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            throw new NotImplementedException();
        }

        private static async Task ProcessJoinCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            if (parsedMessage.Length < 2)
            {
                writer.WriteAsync("0|/join requer um parâmetro com ou sem aspas");
                writer.FlushAsync();
                return;
            }

            var parameter = "";
            if (parsedMessage[1].Contains("\""))
            {
                parameter = parsedMessage[1];
                // procurar a próxima " para fechar o parâmetro

                for (var i = 2; i < parsedMessage.Length - 1; i++)
                {
                    if (parsedMessage[i].Contains("\""))
                    {
                        parameter += parsedMessage[i];
                        break;
                    }
                }

                parameter = parameter.Replace("\"", "");
            }
            else
                parameter = parsedMessage[1];

            if (Usuarios.Where(x => x.Key == parameter).Any()) 
            {
                await writer.WriteLineAsync($"0|Usuário {parameter} já está conectado, escolha outro apelido");
                await writer.FlushAsync();
                return;
            }

            await writer.WriteLineAsync("1");
            await writer.FlushAsync();
            JoinUser(parameter, writer);
            await BroadcastMessage($"{parameter} entrou na sala...");
            threadTimeStamp = DateTime.Now;
        }

        private static async Task BroadcastMessage(string message)
        {
            Console.WriteLine(message);
            foreach(var usuario in Usuarios)
            {
                var writer = usuario.Value;
                await writer.WriteLineAsync(message);
            }
        }

        private static void JoinUser(string parameter, StreamWriter writer)
        {
            Usuarios.Add(parameter, writer);
            Conexoes.Add(writer, parameter);
        }
    }
}