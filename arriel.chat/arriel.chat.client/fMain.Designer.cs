namespace arriel.chat.client
{
    partial class fMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txtServidor = new TextBox();
            txtApelido = new TextBox();
            lblServidor = new Label();
            lblApelido = new Label();
            btnConectar = new Button();
            txtLog = new TextBox();
            lblStatusConexao = new Label();
            txtMensagem = new TextBox();
            btnEnviar = new Button();
            tmrUpdateLog = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // txtServidor
            // 
            txtServidor.Location = new Point(65, 12);
            txtServidor.Name = "txtServidor";
            txtServidor.Size = new Size(177, 23);
            txtServidor.TabIndex = 0;
            // 
            // txtApelido
            // 
            txtApelido.Location = new Point(314, 12);
            txtApelido.Name = "txtApelido";
            txtApelido.Size = new Size(215, 23);
            txtApelido.TabIndex = 1;
            // 
            // lblServidor
            // 
            lblServidor.AutoSize = true;
            lblServidor.Location = new Point(9, 15);
            lblServidor.Name = "lblServidor";
            lblServidor.Size = new Size(50, 15);
            lblServidor.TabIndex = 2;
            lblServidor.Text = "Servidor";
            // 
            // lblApelido
            // 
            lblApelido.AutoSize = true;
            lblApelido.Location = new Point(260, 15);
            lblApelido.Name = "lblApelido";
            lblApelido.Size = new Size(48, 15);
            lblApelido.TabIndex = 3;
            lblApelido.Text = "Apelido";
            // 
            // btnConectar
            // 
            btnConectar.Location = new Point(549, 12);
            btnConectar.Name = "btnConectar";
            btnConectar.Size = new Size(75, 23);
            btnConectar.TabIndex = 4;
            btnConectar.Text = "Conectar";
            btnConectar.UseVisualStyleBackColor = true;
            btnConectar.Click += btnConectar_Click;
            // 
            // txtLog
            // 
            txtLog.Enabled = false;
            txtLog.Location = new Point(12, 58);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(612, 306);
            txtLog.TabIndex = 5;
            // 
            // lblStatusConexao
            // 
            lblStatusConexao.AutoSize = true;
            lblStatusConexao.Location = new Point(9, 40);
            lblStatusConexao.Name = "lblStatusConexao";
            lblStatusConexao.Size = new Size(0, 15);
            lblStatusConexao.TabIndex = 6;
            // 
            // txtMensagem
            // 
            txtMensagem.Enabled = false;
            txtMensagem.Location = new Point(12, 373);
            txtMensagem.Name = "txtMensagem";
            txtMensagem.Size = new Size(528, 23);
            txtMensagem.TabIndex = 7;
            txtMensagem.KeyPress += txtMensagem_KeyPress;
            // 
            // btnEnviar
            // 
            btnEnviar.Enabled = false;
            btnEnviar.Location = new Point(549, 373);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(75, 23);
            btnEnviar.TabIndex = 8;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = true;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // tmrUpdateLog
            // 
            tmrUpdateLog.Enabled = true;
            tmrUpdateLog.Tick += tmrUpdateLog_Tick;
            // 
            // fMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(636, 408);
            Controls.Add(btnEnviar);
            Controls.Add(txtMensagem);
            Controls.Add(lblStatusConexao);
            Controls.Add(txtLog);
            Controls.Add(btnConectar);
            Controls.Add(lblApelido);
            Controls.Add(lblServidor);
            Controls.Add(txtApelido);
            Controls.Add(txtServidor);
            Name = "fMain";
            Text = "arriel.chat.client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtServidor;
        private TextBox txtApelido;
        private Label lblServidor;
        private Label lblApelido;
        private Button btnConectar;
        private TextBox txtLog;
        private Label lblStatusConexao;
        private TextBox txtMensagem;
        private Button btnEnviar;
        private System.Windows.Forms.Timer tmrUpdateLog;
    }
}