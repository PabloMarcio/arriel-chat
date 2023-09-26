using arriel.chat.domain.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.domain.Mensagens
{
    public class Mensagem
    {
        public string Conteudo { get; set; }
        public Cliente ClienteOrigem { get; set; }
        public DateTime DataHora { get; set; }
    }
}
