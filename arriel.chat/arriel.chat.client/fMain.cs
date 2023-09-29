using System.Net.Sockets;
using System.Net;
using arriel.chat.client.Classes;

namespace arriel.chat.client
{
    public partial class fMain : Form
    {
        private bool _conectado = false;
        public bool RecebeuMensagemNova = false;
        public bool Conectado
        {
            get { return _conectado; }
            private set
            {
                _conectado = value;
            }
        }

        public string Apelido { get; private set; }
        public Thread ThreadMensagens { get; private set; }
        public TcpClient TcpServidor { get; private set; }
        public StreamWriter Remetente { get; private set; }
        public StreamReader Receptor { get; private set; }
        public static fMain Instance { get; private set; }

        public fMain()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            ChatLog.TextLog = "";
            InitializeComponent();
            txtServidor.Text = "127.0.0.1";
            Instance = this;
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
            Remetente.WriteLine("/quit");
            Remetente.Flush();
            AtualizarLog(motivo);
            FecharConexao(motivo);
        }

        private void Conectar()
        {
            var enderecoIP = IPAddress.Parse(txtServidor.Text);

            TcpServidor = new TcpClient();
            try
            {
                TcpServidor.Connect(enderecoIP, 2502);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Servidor está inacessível. Tente novamente em alguns minutos");
            }

            Conectado = true;
            Apelido = txtApelido.Text;

            Instance.txtServidor.Enabled = false;
            Instance.txtApelido.Enabled = false;
            Instance.btnConectar.Text = "Sair";
            Instance.txtMensagem.Enabled = true;
            Instance.btnEnviar.Enabled = true;
            Instance.txtLog.Enabled = true;

            Remetente = new StreamWriter(TcpServidor.GetStream());
            if (Apelido.Contains(" "))
            {
                Apelido.Replace("\"", "");
                Apelido = $@"""{Apelido}""";
            }
            Remetente.WriteLine($"/join {Apelido}");
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
                AtualizarLog("Conectado com sucesso. Digite /help para obter ajuda com os comandos disponíveis");
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
                try
                {
                    AtualizarLog(Receptor.ReadLine());
                }
                catch (Exception ex)
                {
                    FecharConexao(ex.Message);
                }

            }
        }

        private void FecharConexao(string motivo)
        {
            Conectado = false;
        }

        private void AtualizarLog(string mensagem)
        {
            if (string.IsNullOrEmpty(mensagem))
            {
                return;
            }
            ChatLog.TextLog += mensagem + "\r\n";
            RecebeuMensagemNova = true;
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
            if (mensagem.Trim().StartsWith("/") == false)
            {
                mensagem = $"/msg {mensagem}";
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

        private void tmrUpdateLog_Tick(object sender, EventArgs e)
        {
            if (txtLog.Text != ChatLog.TextLog)
            {
                txtLog.Text = ChatLog.TextLog;
            }

            if (RecebeuMensagemNova)
            {
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
                RecebeuMensagemNova = false;
            }

        }

        private void tmrStatusConexao_Tick(object sender, EventArgs e)
        {
            Instance.txtServidor.Enabled = !Conectado;
            Instance.txtApelido.Enabled = !Conectado;
            Instance.txtMensagem.Enabled = Conectado;
            Instance.btnEnviar.Enabled = Conectado;
            Instance.btnConectar.Text = !Conectado ? "Conectar" : "Sair";
            lblStatusConexao.Text = Conectado ? $"Conectado como {Apelido}" : "Desconectado";
            lblStatusConexao.ForeColor = Conectado ? Color.DarkGreen : Color.DarkRed;

            if (!Conectado)
            {
                try
                {
                    Apelido = "";
                    Remetente?.Close();
                    Receptor?.Close();
                    TcpServidor?.Close();
                }
                catch
                {
                    return;
                }
            }


        }
    }
}