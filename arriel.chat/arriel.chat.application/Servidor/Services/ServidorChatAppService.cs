using arriel.chat.application.Classes;
using arriel.chat.application.Servidor.Interfaces;
using arriel.chat.domain.Mensagens;
using arriel.chat.infrastructure.Delegates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.application.Servidor.Services
{
    public class ServidorChatAppService : IServidorChatAppService
    {
        public static Hashtable Usuarios = new Hashtable(100);
        public static Hashtable Conexoes = new Hashtable(100);
        private IPAddress enderecoIp;
        private TcpClient tcpClient;

        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        private TcpListener clientListener;
        private bool servidorRodando = false;
        private Thread threadListener;
        private TcpClient tcpCliente;

        public ServidorChatAppService(IPAddress endereco)
        {
            enderecoIp = endereco;
        }

        public void IniciarServidor()
        {
            try
            {                
                IPAddress ipLocal = enderecoIp;                
                clientListener = new TcpListener(ipLocal, 2502);                
                clientListener.Start();                
                servidorRodando = true;                
                threadListener = new Thread(KeepAlive);
                threadListener.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnviarMensagem(string origem, string mensagem)
        {
            EnviarMensagemBase(origem, mensagem, true);
        }        

        public void IncluirUsuario(TcpClient socket, string apelido)
        {            
            Usuarios.Add(apelido, socket);
            Conexoes.Add(socket, apelido);
            
            EnviarMensagemGlobal(Conexoes[socket] + " entrou na sala.", false);
        }

        public void KeepAlive()
        {            
            while (servidorRodando == true)
            {                
                tcpCliente = clientListener.AcceptTcpClient();             
                Conexao newConnection = new Conexao(tcpCliente, this);
            }
        }

        public void RemoverUsuario(TcpClient socket)
        {            
            if (Conexoes[socket] != null)
            {                
                EnviarMensagemGlobal(Conexoes[socket] + " saiu...", false);             
                Usuarios.Remove(Conexoes[socket]);
                Conexoes.Remove(socket);
            }
        }
        
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {                
                statusHandler(null, e);
            }
        }

        private void EnviarMensagemBase(string origem, string mensagem, bool incluirOrigem)
        {
            StreamWriter sw;

            var inicioMensagem = incluirOrigem ? $"[{origem}] " : string.Empty;
            var mensagemCompleta = $"{inicioMensagem}{mensagem}";
            e = new StatusChangedEventArgs(mensagemCompleta);
            OnStatusChanged(e);


            TcpClient[] tcpClientes = new TcpClient[Usuarios.Count];
            Usuarios.Values.CopyTo(tcpClientes, 0);
            foreach (var client in tcpClientes)
            {
                try
                {
                    if (mensagem.Trim() == "" || client == null)
                    {
                        continue;
                    }

                    using (sw = new StreamWriter(client.GetStream()))
                    {
                        sw.WriteLine(mensagemCompleta);
                        sw.Flush();
                    };
                }
                catch
                {
                    RemoverUsuario(client);
                }
            }
        }

        public void EnviarMensagemGlobal(string mensagem, bool incluirOrigem)
        {
            EnviarMensagemBase("Administrador", mensagem, incluirOrigem);
        }
    }
}
