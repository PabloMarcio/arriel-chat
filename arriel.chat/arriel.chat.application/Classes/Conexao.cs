using arriel.chat.application.Servidor.Interfaces;
using arriel.chat.application.Servidor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.application.Classes
{
    public class Conexao
    {
        private TcpClient _tcpCliente;
        private IServidorChatAppService _serverAppService;
        private StreamReader receptor;
        private StreamWriter remetente;
        private string? usuarioAtual;
        private string? resposta;

        public Conexao(TcpClient tcpCliente, IServidorChatAppService serverAppService)
        {
            _tcpCliente = tcpCliente;
            _serverAppService = serverAppService;
        }

        private void AceitarCliente()
        {
            receptor = new StreamReader(_tcpCliente.GetStream());
            remetente = new StreamWriter(_tcpCliente.GetStream());

            // Lê a informação da conta do cliente
            usuarioAtual = receptor.ReadLine();

            // temos uma resposta do cliente
            if (usuarioAtual != "")
            {
                // Armazena o nome do usuário na hash table
                if (ServidorChatAppService.Usuarios.Contains(usuarioAtual) == true)
                {
                    // 0 => significa não conectado
                    remetente.WriteLine("0|Este nome de usuário já existe.");
                    remetente.Flush();
                    FecharConexao();
                    return;
                }
                else if (usuarioAtual == "Administrator")
                {
                    // 0 => não conectado
                    remetente.WriteLine("0|Este nome de usuário é reservado.");
                    remetente.Flush();
                    FecharConexao();
                    return;
                }
                else
                {
                    // 1 => conectou com sucesso
                    remetente.WriteLine("1");
                    remetente.Flush();

                    // Inclui o usuário na hash table e inicia a escuta de suas mensagens
                    _serverAppService.IncluirUsuario(_tcpCliente, usuarioAtual);
                }
            }
            else
            {
                FecharConexao();
                return;
            }
            //
            try
            {
                // Continua aguardando por uma mensagem do usuário
                while ((resposta = receptor.ReadLine()) != "")
                {
                    // Se for inválido remove-o
                    if (resposta == null)
                    {
                        _serverAppService.RemoverUsuario(_tcpCliente);
                    }
                    else
                    {
                        // envia a mensagem para todos os outros usuários
                        _serverAppService.EnviarMensagem(usuarioAtual, resposta);
                    }
                }
            }
            catch
            {
                // Se houve um problema com este usuário desconecta-o
                _serverAppService.RemoverUsuario(_tcpCliente);
            }
        }

        private void FecharConexao()
        {
            try
            {
                // Fecha o receptor (StreamReader) e remetente (StreamWriter)
                receptor.Close();
                remetente.Close();

                // Remove o usuário da lista de usuários ativos no servidor
                if (!string.IsNullOrEmpty(usuarioAtual))
                {
                    _serverAppService.RemoverUsuario(_tcpCliente);
                }

                // Fecha o TcpClient para encerrar a conexão
                _tcpCliente.Close();
            }
            catch (Exception ex)
            {
                // Lidere com qualquer exceção que possa ocorrer durante o fechamento
                Console.WriteLine($"Erro ao fechar a conexão: {ex.Message}");
            }
        }
    }
}
