using System.Net.Sockets;
using System.Net;

namespace arriel.chat.client
{
    public partial class fMain : Form
    {
        private bool _conectado = false;
        public bool Conectado
        {
            get { return _conectado; }
            private set
            {
                _conectado = value;
                lblStatusConexao.Text = _conectado ? $"Conectado como {Apelido}" : "Desconectado";
                lblStatusConexao.ForeColor = _conectado ? Color.DarkGreen : Color.DarkRed;
            }
        }

        public string Apelido { get; private set; }
        public Thread ThreadMensagens { get; private set; }
        public TcpClient TcpServidor { get; private set; }
        public StreamWriter Remetente { get; private set; }
        public StreamReader Receptor { get; private set; }

        public fMain()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }

        private void OnApplicationExit(object? sender, EventArgs e)
        {
            if (Conectado)
            {
                FecharConexao("Aplicação terminou");
            }
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            if (Conectado == false)
            {
                Conectar();
            }
            else
            {
                Desconectar("Desconectado a pedido do usuário.");
            }
        }

        private void Desconectar(string motivo)
        {
            AtualizarLog(motivo);
            FecharConexao(motivo);
        }

        private void Conectar()
        {
            var enderecoIP = IPAddress.Parse(txtServidor.Text);

            TcpServidor = new TcpClient();
            TcpServidor.Connect(enderecoIP, 2502);
            Conectado = true;
            Apelido = txtApelido.Text;

            txtServidor.Enabled = false;
            txtApelido.Enabled = false;
            btnConectar.Text = "Sair";
            txtMensagem.Enabled = true;
            btnEnviar.Enabled = true;
            txtLog.Enabled = true;

            Remetente = new StreamWriter(TcpServidor.GetStream());
            Remetente.WriteLine(Apelido);
            Remetente.Flush();

            //Inicia a thread para receber mensagens e nova comunicação
            ThreadMensagens = new Thread(new ThreadStart(ReceberMensagens));
            ThreadMensagens.Start();
        }

        private void ReceberMensagens()
        {
            Receptor = new StreamReader(TcpServidor.GetStream());
            string resposta = Receptor.ReadLine();
            if (resposta[0] == '1')
            {
                AtualizarLog("Conectado com sucesso!");
            }
            else
            {
                string motivo = "Não Conectado: ";
                motivo += resposta.Substring(2, resposta.Length - 2);
                FecharConexao(motivo);
                return;
            }

            while (Conectado)
            {
                AtualizarLog(Receptor.ReadLine());
            }
        }

        private void FecharConexao(string motivo)
        {
            txtServidor.Enabled = true;
            txtApelido.Enabled = true;
            txtMensagem.Enabled = false;
            btnEnviar.Enabled = false;
            btnConectar.Text = "Conectar";

            Conectado = false;
            Remetente.Close();
            Receptor.Close();
            TcpServidor.Close();
        }

        private void AtualizarLog(string mensagem)
        {
            txtLog.AppendText(mensagem + "\r\n");
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            var mensagem = txtMensagem.Text;
            EnviarMensagem(mensagem);

        }

        private void EnviarMensagem(string mensagem)
        {
            if (mensagem.Trim().Length == 0)
            {
                return;
            }
            Remetente.WriteLine(mensagem);
            Remetente.Flush();
            txtMensagem.Clear();
        }

        private void txtMensagem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                EnviarMensagem(txtMensagem.Text);
            }
        }
    }
}