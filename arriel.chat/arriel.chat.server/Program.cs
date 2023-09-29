using arriel.chat.server.Classes;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                    if (message == null)
                    {
                        continue;
                    }
                    Console.WriteLine(message);
                    var parsedMessage = message.Split(" ");
                    if (parsedMessage.Length == 0)
                    {
                        continue;
                    }
                    //if (DateTime.Now > threadTimeStamp.AddMinutes(5))
                    //    break; // 5 minutos de inatividade quebra o loop
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

        private static async Task ProcessQuitCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            var senderInfo = GetSenderInfo(writer);
            var conexao = senderInfo.ConnectionInfo;
            if (conexao.Equals(default(KeyValuePair<StreamWriter, string>)))
            {
                await writer.WriteLineAsync("0|Conexão inválida");
                await writer.FlushAsync();
                return;
            }
            var nickname = senderInfo.GetUserName();
            var usuario = senderInfo.UserInfo;
            if (usuario.Equals(default(KeyValuePair<string, StreamWriter>)))
            {
                await writer.WriteLineAsync("0|Conexão inválida");
                await writer.FlushAsync();
                return;
            }
            Usuarios.Remove(usuario.Key);
            Conexoes.Remove(conexao.Key);
            var message = $"{nickname} desconectou.";
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
            await BroadcastMessage(message);
            writer.Close();

        }

        private static SenderInfo GetSenderInfo(StreamWriter writer)
        {            
            var conexao = Conexoes.Where(x => x.Key.Equals(writer)).FirstOrDefault();                        
            var usuario = Usuarios.Where(x => x.Key == conexao.Value).FirstOrDefault();
            var senderInfo = new SenderInfo
            {
                UserInfo = usuario,
                ConnectionInfo = conexao
            };
            return senderInfo;
        }

        private static async Task ProcessMsgCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            if (parsedMessage.Length <= 1)
            {
                Console.WriteLine("/msg sem parâmetro");
                return;
            };
            var senderInfo = GetSenderInfo(writer);
            var nickName = senderInfo.GetUserName();
            var message = $"{nickName}: {ObterMensagem(parsedMessage, false)}";
            await BroadcastMessage(message);
        }

        private static async Task WarnUnknownMessage(string message, DateTime threadTimeStamp, StreamWriter writer)
        {
            Console.WriteLine("Não sei o que fazer com esta mensagem");
            await writer.WriteLineAsync($@"Comando desconhecido - utilize /help para saber quais comandos atendemos");
            await writer.FlushAsync();
        }

        private static async Task ProcessWhisperCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            if (parsedMessage.Length < 2)
            {
                await writer.WriteAsync("/whisper requer um parâmetro. Digite /help pra mais info");
                await writer.FlushAsync();
                return;
            }

            var parameter = ObterParametro(parsedMessage);
            var message = ObterMensagem(parsedMessage, true);
            if (Usuarios.Where(x => x.Key == parameter).Any() == false)
            {
                await writer.WriteAsync("/whisper requer um usuário que esteja conectado na sala. Utilize um /who para ver quem estar conectado");
                await writer.FlushAsync();
                return;
            }
            var senderInfo = GetSenderInfo(writer);
            var nickName = senderInfo.GetUserName(); ;
            var usuario = Usuarios.Where(x => x.Key == parameter).FirstOrDefault();
            var userWriter = usuario.Value;
            await userWriter.WriteLineAsync($"{nickName} enviou uma mensagem privada para wqvocê: {message}");
            await userWriter.FlushAsync();
            await writer.WriteLineAsync($"Você enviou uma mensagem privada para {parameter}: {message}");
            await writer.FlushAsync();
        }

        private static string ObterMensagem(string[] parsedMessage, bool parameterValidation)
        {
            var initialIndex = 1;
            if (parameterValidation)
            {
                var parameter1 = ObterParametro(parsedMessage);
                var spaceCount = parameter1.Count(c => c == ' ');
                initialIndex = 2 + spaceCount;
            }            
            string result = string.Join(" ", parsedMessage, initialIndex, parsedMessage.Length - initialIndex);
            return result;
        }

        private static async Task ProcessHelpCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            await writer.WriteLineAsync($@"/help ativado - comandos disponíveis nesse chat:
/help - chama a ajuda
/whisper ""Apelido"" <mensagem> - manda uma mensagem privada para a pessoa citada em Apelido, contanto que alguém esteja conectado com esse apelido na sala
/who - obtém uma lista de usuários que estão na sala
/quit - desconecta do chat");
            await writer.FlushAsync();    
        }

        private static async Task ProcessWhoCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Listagem de usuários conectados:");
            var count = 0;
            foreach(var usuario in Usuarios)
            {
                count++;
                sb.AppendLine(usuario.Key);
            }
            sb.AppendLine($"Total de usuários conectados: {count}");
            var message = sb.ToString();
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();            
        }

        private static async Task ProcessJoinCommand(string[] parsedMessage, DateTime threadTimeStamp, StreamWriter writer)
        {
            if (parsedMessage.Length < 2)
            {
                await writer.WriteAsync("0|/join requer um parâmetro com ou sem aspas");
                await writer.FlushAsync();
                return;
            }

            var parameter = ObterParametro(parsedMessage);

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

        private static string ObterParametro(string[] parsedMessage)
        {
            var parameter = "";
            if (parsedMessage[1].Contains("\""))
            {
                parameter = parsedMessage[1];
                // procurar a próxima " para fechar o parâmetro

                for (var i = 2; i < parsedMessage.Length; i++)
                {
                    parameter += $" {parsedMessage[i]}";
                    if (parsedMessage[i].Contains("\""))
                    {                        
                        break;
                    }
                }

                parameter = parameter.Replace("\"", "");
            }
            else
                parameter = parsedMessage[1];

            return parameter;
        }

        private static async Task BroadcastMessage(string message)
        {
            Console.WriteLine(message);
            foreach(var usuario in Usuarios)
            {
                var writer = usuario.Value;
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();  
            }
        }

        private static void JoinUser(string parameter, StreamWriter writer)
        {
            Usuarios.Add(parameter, writer);
            Conexoes.Add(writer, parameter);
        }
    }
}