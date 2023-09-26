using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.application.Servidor.Interfaces
{
    public interface IServidorChatAppService
    {
        void IncluirUsuario(TcpClient socket, string apelido);
        void RemoverUsuario(TcpClient socket);
        void EnviarMensagemGlobal(string mensagem, bool incluirOrigem);
        void EnviarMensagem(string origem, string mensagem);
        void IniciarServidor();
        void KeepAlive();
    }
}
