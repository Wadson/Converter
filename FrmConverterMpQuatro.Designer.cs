namespace Converter
{
    partial class FrmConverterMpQuatro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConverterMpQuatro));
            btnFechar = new PictureBox();
            btnSave = new Button();
            btnOpenVideo = new Button();
            btnConverter = new Button();
            btnCancelar = new Button();
            lblProgress = new Label();
            btnLimparLista = new Button();
            toolTip1 = new ToolTip(components);
            btnPausar = new Button();
            btnContinuar = new Button();
            btnRemoverSelecionados = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            Status = new ToolStripStatusLabel();
            Dataprogress = new ToolStripStatusLabel();
            lblStatus = new ToolStripStatusLabel();
            listBoxVideos = new ListBox();
            txtPathVideo = new TextBox();
            progressBar = new ProgressBar();
            cmbAudioQuality = new ComboBox();
            label2 = new Label();
            txtSaveTo = new TextBox();
            label3 = new Label();
            lblTotalVideos = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)btnFechar).BeginInit();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // btnFechar
            // 
            btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFechar.Image = Properties.Resources._24Exit;
            btnFechar.Location = new Point(506, 0);
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
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackgroundImageLayout = ImageLayout.Stretch;
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Image = Properties.Resources._24Pasta;
            btnSave.Location = new Point(490, 341);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(28, 24);
            btnSave.TabIndex = 43;
            toolTip1.SetToolTip(btnSave, "Local de destino dos arquivos");
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnOpenVideo
            // 
            btnOpenVideo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenVideo.BackgroundImageLayout = ImageLayout.Stretch;
            btnOpenVideo.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnOpenVideo.FlatAppearance.BorderSize = 0;
            btnOpenVideo.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnOpenVideo.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnOpenVideo.FlatStyle = FlatStyle.Flat;
            btnOpenVideo.Image = Properties.Resources._24Pasta;
            btnOpenVideo.Location = new Point(484, 30);
            btnOpenVideo.Margin = new Padding(4, 3, 4, 3);
            btnOpenVideo.Name = "btnOpenVideo";
            btnOpenVideo.Size = new Size(28, 24);
            btnOpenVideo.TabIndex = 44;
            toolTip1.SetToolTip(btnOpenVideo, "Adicionar arquivos de vídeos");
            btnOpenVideo.UseVisualStyleBackColor = true;
            btnOpenVideo.Click += btnOpenVideo_Click;
            // 
            // btnConverter
            // 
            btnConverter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnConverter.BackgroundImageLayout = ImageLayout.Stretch;
            btnConverter.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnConverter.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnConverter.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnConverter.FlatStyle = FlatStyle.Flat;
            btnConverter.Image = Properties.Resources._32x32_Converter;
            btnConverter.Location = new Point(477, 408);
            btnConverter.Margin = new Padding(4, 3, 4, 3);
            btnConverter.Name = "btnConverter";
            btnConverter.Size = new Size(39, 36);
            btnConverter.TabIndex = 45;
            toolTip1.SetToolTip(btnConverter, "Iniciar conversão");
            btnConverter.UseVisualStyleBackColor = true;
            btnConverter.Click += btnConverter_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.BackgroundImageLayout = ImageLayout.Stretch;
            btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnCancelar.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnCancelar.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Image = Properties.Resources._32Stop;
            btnCancelar.Location = new Point(437, 408);
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
            lblProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblProgress.AutoSize = true;
            lblProgress.BackColor = Color.Transparent;
            lblProgress.Font = new Font("Microsoft Sans Serif", 12F);
            lblProgress.ForeColor = Color.FromArgb(8, 142, 254);
            lblProgress.Location = new Point(459, 447);
            lblProgress.Margin = new Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(45, 20);
            lblProgress.TabIndex = 47;
            lblProgress.Text = "0,0%";
            // 
            // btnLimparLista
            // 
            btnLimparLista.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLimparLista.BackgroundImageLayout = ImageLayout.Stretch;
            btnLimparLista.FlatAppearance.BorderColor = Color.FromArgb(8, 142, 252);
            btnLimparLista.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 217, 226);
            btnLimparLista.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnLimparLista.FlatStyle = FlatStyle.Flat;
            btnLimparLista.Image = Properties.Resources._32Limpar;
            btnLimparLista.Location = new Point(425, 120);
            btnLimparLista.Margin = new Padding(4, 3, 4, 3);
            btnLimparLista.Name = "btnLimparLista";
            btnLimparLista.Size = new Size(39, 36);
            btnLimparLista.TabIndex = 51;
            toolTip1.SetToolTip(btnLimparLista, "Limpar Lista");
            btnLimparLista.UseVisualStyleBackColor = true;
            btnLimparLista.Click += btnLimparLista_Click;
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
            btnPausar.Location = new Point(357, 408);
            btnPausar.Margin = new Padding(4, 3, 4, 3);
            btnPausar.Name = "btnPausar";
            btnPausar.Size = new Size(39, 36);
            btnPausar.TabIndex = 80;
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
            btnContinuar.Location = new Point(397, 408);
            btnContinuar.Margin = new Padding(4, 3, 4, 3);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(39, 36);
            btnContinuar.TabIndex = 81;
            toolTip1.SetToolTip(btnContinuar, "Continuar Download");
            btnContinuar.UseVisualStyleBackColor = true;
            btnContinuar.Click += btnContinuar_Click;
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
            btnRemoverSelecionados.Location = new Point(472, 120);
            btnRemoverSelecionados.Margin = new Padding(4, 3, 4, 3);
            btnRemoverSelecionados.Name = "btnRemoverSelecionados";
            btnRemoverSelecionados.Size = new Size(39, 36);
            btnRemoverSelecionados.TabIndex = 83;
            toolTip1.SetToolTip(btnRemoverSelecionados, "Excluir Link / Vídeo Selecionado");
            btnRemoverSelecionados.UseVisualStyleBackColor = true;
            btnRemoverSelecionados.Click += btnRemoverSelecionados_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, Status, Dataprogress, lblStatus });
            statusStrip1.Location = new Point(0, 473);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(539, 22);
            statusStrip1.TabIndex = 55;
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
            lblStatus.ActiveLinkColor = Color.FromArgb(8, 128, 244);
            lblStatus.ForeColor = Color.FromArgb(235, 42, 83);
            lblStatus.LinkColor = Color.FromArgb(8, 128, 255);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(12, 17);
            lblStatus.Text = "-";
            // 
            // listBoxVideos
            // 
            listBoxVideos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listBoxVideos.FormattingEnabled = true;
            listBoxVideos.Location = new Point(7, 22);
            listBoxVideos.Margin = new Padding(4, 3, 4, 3);
            listBoxVideos.Name = "listBoxVideos";
            listBoxVideos.Size = new Size(504, 94);
            listBoxVideos.TabIndex = 61;
            listBoxVideos.DragDrop += listBoxVideos_DragDrop;
            listBoxVideos.DragEnter += listBoxVideos_DragEnter;
            // 
            // txtPathVideo
            // 
            txtPathVideo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPathVideo.BackColor = Color.SkyBlue;
            txtPathVideo.Location = new Point(7, 32);
            txtPathVideo.Margin = new Padding(4, 3, 4, 3);
            txtPathVideo.Name = "txtPathVideo";
            txtPathVideo.Size = new Size(477, 23);
            txtPathVideo.TabIndex = 60;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(9, 449);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(442, 18);
            progressBar.TabIndex = 62;
            // 
            // cmbAudioQuality
            // 
            cmbAudioQuality.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbAudioQuality.FormattingEnabled = true;
            cmbAudioQuality.Location = new Point(407, 373);
            cmbAudioQuality.Margin = new Padding(4, 3, 4, 3);
            cmbAudioQuality.Name = "cmbAudioQuality";
            cmbAudioQuality.Size = new Size(82, 23);
            cmbAudioQuality.TabIndex = 70;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.Highlight;
            label2.Location = new Point(325, 377);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(80, 15);
            label2.TabIndex = 69;
            label2.Text = "Audio Quality";
            // 
            // txtSaveTo
            // 
            txtSaveTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSaveTo.BackColor = Color.SkyBlue;
            txtSaveTo.Location = new Point(11, 344);
            txtSaveTo.Margin = new Padding(4, 3, 4, 3);
            txtSaveTo.Name = "txtSaveTo";
            txtSaveTo.Size = new Size(478, 23);
            txtSaveTo.TabIndex = 68;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.Highlight;
            label3.Location = new Point(13, 327);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 67;
            label3.Text = "Salvar em:";
            // 
            // lblTotalVideos
            // 
            lblTotalVideos.AutoSize = true;
            lblTotalVideos.ForeColor = SystemColors.Highlight;
            lblTotalVideos.Location = new Point(7, 58);
            lblTotalVideos.Margin = new Padding(4, 0, 4, 0);
            lblTotalVideos.Name = "lblTotalVideos";
            lblTotalVideos.Size = new Size(82, 15);
            lblTotalVideos.TabIndex = 66;
            lblTotalVideos.Text = "Total de Links:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.Highlight;
            label1.Location = new Point(8, 17);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(161, 15);
            label1.TabIndex = 71;
            label1.Text = "Local dos arquivos de vídeos:";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(244, 1);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(47, 45);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 72;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = SystemColors.Highlight;
            panel1.Controls.Add(kryptonLabel1);
            panel1.Controls.Add(btnFechar);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(539, 76);
            panel1.TabIndex = 79;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Dock = DockStyle.Bottom;
            kryptonLabel1.Location = new Point(0, 46);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(539, 30);
            kryptonLabel1.StateCommon.ShortText.Color1 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Color2 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            kryptonLabel1.StateCommon.ShortText.TextH = Krypton.Toolkit.PaletteRelativeAlign.Center;
            kryptonLabel1.TabIndex = 73;
            kryptonLabel1.Values.Text = "Converter Vídeo MP4 para Audio Mp3";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.BackColor = Color.White;
            groupBox1.Controls.Add(txtPathVideo);
            groupBox1.Controls.Add(btnOpenVideo);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lblTotalVideos);
            groupBox1.Location = new Point(5, 82);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(523, 76);
            groupBox1.TabIndex = 84;
            groupBox1.TabStop = false;
            groupBox1.Text = "Entrada";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.BackColor = Color.White;
            groupBox2.Controls.Add(listBoxVideos);
            groupBox2.Controls.Add(btnRemoverSelecionados);
            groupBox2.Controls.Add(btnLimparLista);
            groupBox2.Location = new Point(5, 164);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(521, 164);
            groupBox2.TabIndex = 85;
            groupBox2.TabStop = false;
            groupBox2.Text = "Lista de Downloads";
            // 
            // FrmConverterMpQuatro
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(539, 495);
            Controls.Add(cmbAudioQuality);
            Controls.Add(label2);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnPausar);
            Controls.Add(btnContinuar);
            Controls.Add(panel1);
            Controls.Add(txtSaveTo);
            Controls.Add(label3);
            Controls.Add(progressBar);
            Controls.Add(statusStrip1);
            Controls.Add(lblProgress);
            Controls.Add(btnCancelar);
            Controls.Add(btnConverter);
            Controls.Add(btnSave);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmConverterMpQuatro";
            PaletteMode = Krypton.Toolkit.PaletteMode.Office2007BlueLightMode;
            ShowIcon = false;
            Text = "Converter Mp4 pra Mp3";
            ((System.ComponentModel.ISupportInitialize)btnFechar).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox btnFechar;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpenVideo;
        private System.Windows.Forms.Button btnConverter;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnLimparLista;
        private System.Windows.Forms.ToolTip toolTip1;
       
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ToolStripStatusLabel Dataprogress;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ListBox listBoxVideos;
        private System.Windows.Forms.TextBox txtPathVideo;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox cmbAudioQuality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSaveTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTotalVideos;
        private System.Windows.Forms.Label label1;
        private PictureBox pictureBox1;
        private Panel panel1;
        private Button btnPausar;
        private Button btnContinuar;
        private Button btnRemoverSelecionados;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}

