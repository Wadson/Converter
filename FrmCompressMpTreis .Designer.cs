namespace Converter
{
    partial class FrmCompressMpTreis
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCompressMpTreis));
            btnFechar = new PictureBox();
            btnSave = new Button();
            btnConverter = new Button();
            btnCancelar = new Button();
            lblProgress = new Label();
            btnLimparLista = new Button();
            toolTip1 = new ToolTip(components);
            btnOpenAudio = new Button();
            btnPausar = new Button();
            btnContinuar = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            Status = new ToolStripStatusLabel();
            Dataprogress = new ToolStripStatusLabel();
            lblStatus = new ToolStripStatusLabel();
            txtPathVideo = new TextBox();
            label1 = new Label();
            listBoxVideos = new ListBox();
            progressBar = new ProgressBar();
            lblTotalVideos = new Label();
            label3 = new Label();
            txtSalvarEm = new TextBox();
            label2 = new Label();
            cmbNivelCompressao = new ComboBox();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            btnRemoverSelecionados = new Button();
            ((System.ComponentModel.ISupportInitialize)btnFechar).BeginInit();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnFechar
            // 
            btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFechar.Image = Properties.Resources._24Exit;
            btnFechar.Location = new Point(655, 0);
            btnFechar.Margin = new Padding(4, 3, 4, 3);
            btnFechar.Name = "btnFechar";
            btnFechar.Size = new Size(33, 30);
            btnFechar.SizeMode = PictureBoxSizeMode.StretchImage;
            btnFechar.TabIndex = 15;
            btnFechar.TabStop = false;
            btnFechar.Click += btnFechar_Click;
            // 
            // btnSave
            // 
            btnSave.BackgroundImageLayout = ImageLayout.Stretch;
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Image = Properties.Resources._24Pasta;
            btnSave.Location = new Point(639, 387);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(28, 28);
            btnSave.TabIndex = 43;
            toolTip1.SetToolTip(btnSave, "Local de salvamento dos arquivos");
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnConverter
            // 
            btnConverter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConverter.BackgroundImageLayout = ImageLayout.Stretch;
            btnConverter.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnConverter.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnConverter.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnConverter.FlatStyle = FlatStyle.Flat;
            btnConverter.Image = Properties.Resources._32x32_Compress;
            btnConverter.Location = new Point(594, 428);
            btnConverter.Margin = new Padding(4, 3, 4, 3);
            btnConverter.Name = "btnConverter";
            btnConverter.Size = new Size(39, 36);
            btnConverter.TabIndex = 45;
            toolTip1.SetToolTip(btnConverter, "Iniciar compressão");
            btnConverter.UseVisualStyleBackColor = true;
            btnConverter.Click += btnConverter_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackgroundImageLayout = ImageLayout.Stretch;
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnCancelar.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnCancelar.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Image = Properties.Resources._32Stop;
            btnCancelar.Location = new Point(551, 429);
            btnCancelar.Margin = new Padding(4, 3, 4, 3);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(39, 36);
            btnCancelar.TabIndex = 46;
            toolTip1.SetToolTip(btnCancelar, "Cancelar");
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.BackColor = Color.Transparent;
            lblProgress.Font = new Font("Microsoft Sans Serif", 12F);
            lblProgress.ForeColor = Color.FromArgb(8, 142, 254);
            lblProgress.Location = new Point(634, 314);
            lblProgress.Margin = new Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(45, 20);
            lblProgress.TabIndex = 47;
            lblProgress.Text = "0,0%";
            // 
            // btnLimparLista
            // 
            btnLimparLista.BackgroundImageLayout = ImageLayout.Stretch;
            btnLimparLista.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnLimparLista.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnLimparLista.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnLimparLista.FlatStyle = FlatStyle.Flat;
            btnLimparLista.Image = Properties.Resources._32Limpar;
            btnLimparLista.Location = new Point(643, 163);
            btnLimparLista.Margin = new Padding(4, 3, 4, 3);
            btnLimparLista.Name = "btnLimparLista";
            btnLimparLista.Size = new Size(39, 36);
            btnLimparLista.TabIndex = 51;
            toolTip1.SetToolTip(btnLimparLista, "Limpar lista");
            btnLimparLista.UseVisualStyleBackColor = true;
            btnLimparLista.Click += btnLimparLista_Click;
            // 
            // btnOpenAudio
            // 
            btnOpenAudio.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenAudio.BackgroundImageLayout = ImageLayout.Stretch;
            btnOpenAudio.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnOpenAudio.FlatAppearance.BorderSize = 0;
            btnOpenAudio.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnOpenAudio.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnOpenAudio.FlatStyle = FlatStyle.Flat;
            btnOpenAudio.Image = Properties.Resources._24Pasta;
            btnOpenAudio.Location = new Point(638, 124);
            btnOpenAudio.Margin = new Padding(4, 3, 4, 3);
            btnOpenAudio.Name = "btnOpenAudio";
            btnOpenAudio.Size = new Size(28, 28);
            btnOpenAudio.TabIndex = 44;
            toolTip1.SetToolTip(btnOpenAudio, "Adicionar arquivos de aúdio mp3");
            btnOpenAudio.UseVisualStyleBackColor = true;
            btnOpenAudio.Click += btnOpenAudio_Click;
            // 
            // btnPausar
            // 
            btnPausar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPausar.BackgroundImageLayout = ImageLayout.Stretch;
            btnPausar.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnPausar.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnPausar.FlatStyle = FlatStyle.Flat;
            btnPausar.ForeColor = SystemColors.Highlight;
            btnPausar.Image = Properties.Resources._32Pause;
            btnPausar.Location = new Point(465, 429);
            btnPausar.Margin = new Padding(4, 3, 4, 3);
            btnPausar.Name = "btnPausar";
            btnPausar.Size = new Size(39, 36);
            btnPausar.TabIndex = 82;
            toolTip1.SetToolTip(btnPausar, "Pausar Download");
            btnPausar.UseVisualStyleBackColor = true;
            btnPausar.Click += btnPausar_Click;
            // 
            // btnContinuar
            // 
            btnContinuar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnContinuar.BackgroundImageLayout = ImageLayout.Stretch;
            btnContinuar.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnContinuar.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnContinuar.FlatStyle = FlatStyle.Flat;
            btnContinuar.ForeColor = SystemColors.Highlight;
            btnContinuar.Image = Properties.Resources._32Play;
            btnContinuar.Location = new Point(508, 429);
            btnContinuar.Margin = new Padding(4, 3, 4, 3);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(39, 36);
            btnContinuar.TabIndex = 83;
            toolTip1.SetToolTip(btnContinuar, "Continuar Download");
            btnContinuar.UseVisualStyleBackColor = true;
            btnContinuar.Click += btnContinuar_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, Status, Dataprogress, lblStatus });
            statusStrip1.Location = new Point(0, 523);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(692, 22);
            statusStrip1.TabIndex = 56;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 17);
            // 
            // Status
            // 
            Status.Name = "Status";
            Status.Size = new Size(0, 17);
            // 
            // Dataprogress
            // 
            Dataprogress.Name = "Dataprogress";
            Dataprogress.Size = new Size(0, 17);
            // 
            // lblStatus
            // 
            lblStatus.ActiveLinkColor = Color.Red;
            lblStatus.ForeColor = Color.SeaGreen;
            lblStatus.LinkColor = Color.FromArgb(8, 128, 255);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(12, 17);
            lblStatus.Text = "-";
            // 
            // txtPathVideo
            // 
            txtPathVideo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPathVideo.BackColor = Color.SkyBlue;
            txtPathVideo.Location = new Point(22, 127);
            txtPathVideo.Margin = new Padding(4, 3, 4, 3);
            txtPathVideo.Name = "txtPathVideo";
            txtPathVideo.Size = new Size(612, 23);
            txtPathVideo.TabIndex = 57;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.Highlight;
            label1.Location = new Point(23, 105);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(102, 15);
            label1.TabIndex = 58;
            label1.Text = "Imput audio path:";
            // 
            // listBoxVideos
            // 
            listBoxVideos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listBoxVideos.FormattingEnabled = true;
            listBoxVideos.Location = new Point(22, 163);
            listBoxVideos.Margin = new Padding(4, 3, 4, 3);
            listBoxVideos.Name = "listBoxVideos";
            listBoxVideos.Size = new Size(612, 94);
            listBoxVideos.TabIndex = 59;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(22, 314);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(612, 18);
            progressBar.TabIndex = 60;
            // 
            // lblTotalVideos
            // 
            lblTotalVideos.AutoSize = true;
            lblTotalVideos.ForeColor = SystemColors.Highlight;
            lblTotalVideos.Location = new Point(27, 344);
            lblTotalVideos.Margin = new Padding(4, 0, 4, 0);
            lblTotalVideos.Name = "lblTotalVideos";
            lblTotalVideos.Size = new Size(68, 15);
            lblTotalVideos.TabIndex = 61;
            lblTotalVideos.Text = "TotalVideos";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.Highlight;
            label3.Location = new Point(27, 368);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 62;
            label3.Text = "Salvar em:";
            // 
            // txtSalvarEm
            // 
            txtSalvarEm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSalvarEm.BackColor = Color.SkyBlue;
            txtSalvarEm.Location = new Point(28, 390);
            txtSalvarEm.Margin = new Padding(4, 3, 4, 3);
            txtSalvarEm.Name = "txtSalvarEm";
            txtSalvarEm.Size = new Size(606, 23);
            txtSalvarEm.TabIndex = 63;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.Highlight;
            label2.Location = new Point(27, 428);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(166, 15);
            label2.TabIndex = 64;
            label2.Text = "Alterar o tamanho do arquivo:";
            // 
            // cmbNivelCompressao
            // 
            cmbNivelCompressao.FormattingEnabled = true;
            cmbNivelCompressao.Location = new Point(201, 425);
            cmbNivelCompressao.Margin = new Padding(4, 3, 4, 3);
            cmbNivelCompressao.Name = "cmbNivelCompressao";
            cmbNivelCompressao.Size = new Size(82, 23);
            cmbNivelCompressao.TabIndex = 65;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(320, 3);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(45, 41);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 73;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.Controls.Add(kryptonLabel1);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(btnFechar);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(692, 74);
            panel1.TabIndex = 74;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Dock = DockStyle.Bottom;
            kryptonLabel1.Location = new Point(0, 44);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(692, 30);
            kryptonLabel1.StateCommon.ShortText.Color1 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Color2 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            kryptonLabel1.StateCommon.ShortText.TextH = Krypton.Toolkit.PaletteRelativeAlign.Center;
            kryptonLabel1.TabIndex = 74;
            kryptonLabel1.Values.Text = "Compactar aúdios Mp3";
            // 
            // btnRemoverSelecionados
            // 
            btnRemoverSelecionados.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoverSelecionados.BackgroundImageLayout = ImageLayout.Stretch;
            btnRemoverSelecionados.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnRemoverSelecionados.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnRemoverSelecionados.FlatStyle = FlatStyle.Flat;
            btnRemoverSelecionados.ForeColor = SystemColors.Highlight;
            btnRemoverSelecionados.Image = Properties.Resources._32Excluir;
            btnRemoverSelecionados.Location = new Point(642, 221);
            btnRemoverSelecionados.Margin = new Padding(4, 3, 4, 3);
            btnRemoverSelecionados.Name = "btnRemoverSelecionados";
            btnRemoverSelecionados.Size = new Size(39, 36);
            btnRemoverSelecionados.TabIndex = 84;
            toolTip1.SetToolTip(btnRemoverSelecionados, "Excluir Link / Vídeo Selecionado");
            btnRemoverSelecionados.UseVisualStyleBackColor = true;
            btnRemoverSelecionados.Click += btnRemoverSelecionados_Click;
            // 
            // FrmCompressMpTreis
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 252, 252);
            ClientSize = new Size(692, 545);
            Controls.Add(btnRemoverSelecionados);
            Controls.Add(btnPausar);
            Controls.Add(btnContinuar);
            Controls.Add(panel1);
            Controls.Add(cmbNivelCompressao);
            Controls.Add(label2);
            Controls.Add(txtSalvarEm);
            Controls.Add(label3);
            Controls.Add(lblTotalVideos);
            Controls.Add(progressBar);
            Controls.Add(listBoxVideos);
            Controls.Add(label1);
            Controls.Add(txtPathVideo);
            Controls.Add(statusStrip1);
            Controls.Add(btnLimparLista);
            Controls.Add(lblProgress);
            Controls.Add(btnCancelar);
            Controls.Add(btnConverter);
            Controls.Add(btnOpenAudio);
            Controls.Add(btnSave);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmCompressMpTreis";
            ShowIcon = false;
            Text = "Converter Mp4 pra Mp3";
            ((System.ComponentModel.ISupportInitialize)btnFechar).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox btnFechar;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnConverter;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnLimparLista;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOpenAudio;
       
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ToolStripStatusLabel Dataprogress;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TextBox txtPathVideo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxVideos;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblTotalVideos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSalvarEm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbNivelCompressao;
        private PictureBox pictureBox1;
        private Panel panel1;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Button btnPausar;
        private Button btnContinuar;
        private Button btnRemoverSelecionados;
    }
}

