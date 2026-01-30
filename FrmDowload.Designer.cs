namespace Converter
{
    partial class FrmDowload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDowload));
            cmbVideoQuality = new ComboBox();
            cmbAudioQuality = new ComboBox();
            ListBoxURL = new ListBox();
            txtTitle = new TextBox();
            txtFilePath = new TextBox();
            lblAnalisando = new Label();
            chkAudioOnly = new CheckBox();
            lblStatusContagem = new Label();
            label5 = new Label();
            lblProgress = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            progressBar = new ProgressBar();
            label1 = new Label();
            toolTip1 = new ToolTip(components);
            btnAdicionarURL = new FontAwesome.Sharp.IconButton();
            btnBrowse = new FontAwesome.Sharp.IconButton();
            btnLimparLista = new FontAwesome.Sharp.IconButton();
            btnExcluirSelecionados = new FontAwesome.Sharp.IconButton();
            btnPausar = new FontAwesome.Sharp.IconButton();
            btnContinuar = new FontAwesome.Sharp.IconButton();
            btnCancelar = new FontAwesome.Sharp.IconButton();
            btnDownload = new FontAwesome.Sharp.IconButton();
            btnFechar = new FontAwesome.Sharp.IconButton();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            Status = new ToolStripStatusLabel();
            Dataprogress = new ToolStripStatusLabel();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            folderBrowserDialog1 = new FolderBrowserDialog();
            Downloader_BackProcess = new System.ComponentModel.BackgroundWorker();
            bgWorkerGetVideo = new System.ComponentModel.BackgroundWorker();
            txtUrl = new TextBox();
            pictureBox1 = new PictureBox();
            lblTotalLinks = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            panel1 = new Panel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // cmbVideoQuality
            // 
            cmbVideoQuality.FormattingEnabled = true;
            cmbVideoQuality.Location = new Point(164, 84);
            cmbVideoQuality.Margin = new Padding(4, 3, 4, 3);
            cmbVideoQuality.Name = "cmbVideoQuality";
            cmbVideoQuality.Size = new Size(107, 23);
            cmbVideoQuality.TabIndex = 59;
            cmbVideoQuality.SelectedIndexChanged += cmbVideoQuality_SelectedIndexChanged;
            // 
            // cmbAudioQuality
            // 
            cmbAudioQuality.FormattingEnabled = true;
            cmbAudioQuality.Location = new Point(387, 84);
            cmbAudioQuality.Margin = new Padding(4, 3, 4, 3);
            cmbAudioQuality.Name = "cmbAudioQuality";
            cmbAudioQuality.Size = new Size(109, 23);
            cmbAudioQuality.TabIndex = 58;
            cmbAudioQuality.SelectedIndexChanged += cmbAudioQuality_SelectedIndexChanged;
            // 
            // ListBoxURL
            // 
            ListBoxURL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ListBoxURL.FormattingEnabled = true;
            ListBoxURL.Location = new Point(7, 21);
            ListBoxURL.Margin = new Padding(4, 3, 4, 3);
            ListBoxURL.Name = "ListBoxURL";
            ListBoxURL.Size = new Size(654, 109);
            ListBoxURL.TabIndex = 57;
            // 
            // txtTitle
            // 
            txtTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTitle.BackColor = Color.Aquamarine;
            txtTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTitle.Location = new Point(70, 53);
            txtTitle.Margin = new Padding(4, 3, 4, 3);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(538, 23);
            txtTitle.TabIndex = 56;
            // 
            // txtFilePath
            // 
            txtFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePath.BackColor = Color.Aquamarine;
            txtFilePath.Location = new Point(71, 115);
            txtFilePath.Margin = new Padding(4, 3, 4, 3);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(537, 23);
            txtFilePath.TabIndex = 55;
            // 
            // lblAnalisando
            // 
            lblAnalisando.Anchor = AnchorStyles.Top;
            lblAnalisando.AutoSize = true;
            lblAnalisando.BackColor = Color.Transparent;
            lblAnalisando.Font = new Font("Microsoft Sans Serif", 12F);
            lblAnalisando.ForeColor = Color.FromArgb(8, 142, 254);
            lblAnalisando.Location = new Point(39, 49);
            lblAnalisando.Margin = new Padding(4, 0, 4, 0);
            lblAnalisando.Name = "lblAnalisando";
            lblAnalisando.Size = new Size(100, 20);
            lblAnalisando.TabIndex = 50;
            lblAnalisando.Text = "Analisando...";
            lblAnalisando.Visible = false;
            // 
            // chkAudioOnly
            // 
            chkAudioOnly.Anchor = AnchorStyles.Top;
            chkAudioOnly.AutoSize = true;
            chkAudioOnly.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            chkAudioOnly.ForeColor = Color.FromArgb(6, 130, 200);
            chkAudioOnly.Location = new Point(519, 90);
            chkAudioOnly.Margin = new Padding(4, 3, 4, 3);
            chkAudioOnly.Name = "chkAudioOnly";
            chkAudioOnly.Size = new Size(87, 17);
            chkAudioOnly.TabIndex = 48;
            chkAudioOnly.Text = "Audio Only";
            chkAudioOnly.UseVisualStyleBackColor = true;
            // 
            // lblStatusContagem
            // 
            lblStatusContagem.Anchor = AnchorStyles.Top;
            lblStatusContagem.AutoSize = true;
            lblStatusContagem.BackColor = Color.Transparent;
            lblStatusContagem.Font = new Font("Microsoft Sans Serif", 10F);
            lblStatusContagem.ForeColor = Color.FromArgb(8, 142, 254);
            lblStatusContagem.Location = new Point(45, 416);
            lblStatusContagem.Margin = new Padding(4, 0, 4, 0);
            lblStatusContagem.Name = "lblStatusContagem";
            lblStatusContagem.Size = new Size(13, 17);
            lblStatusContagem.TabIndex = 45;
            lblStatusContagem.Text = "-";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(6, 130, 200);
            label5.Location = new Point(9, 119);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(54, 13);
            label5.TabIndex = 41;
            label5.Text = "Destino:";
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblProgress.AutoSize = true;
            lblProgress.BackColor = Color.Transparent;
            lblProgress.Font = new Font("Microsoft Sans Serif", 12F);
            lblProgress.ForeColor = Color.FromArgb(6, 130, 200);
            lblProgress.Location = new Point(593, 500);
            lblProgress.Margin = new Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(45, 20);
            lblProgress.TabIndex = 44;
            lblProgress.Text = "0,0%";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top;
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.FromArgb(6, 130, 200);
            label4.Location = new Point(294, 89);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(86, 13);
            label4.TabIndex = 39;
            label4.Text = "Audio Quality:";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(6, 130, 200);
            label3.Location = new Point(70, 88);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(86, 13);
            label3.TabIndex = 38;
            label3.Text = "Video Quality:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(6, 130, 200);
            label2.Location = new Point(9, 56);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(43, 13);
            label2.TabIndex = 35;
            label2.Text = "Titulo:";
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            progressBar.BackColor = Color.White;
            progressBar.Location = new Point(11, 502);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(574, 18);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 46;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(6, 130, 200);
            label1.Location = new Point(32, 25);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(27, 13);
            label1.TabIndex = 3;
            label1.Text = "Url:";
            // 
            // btnAdicionarURL
            // 
            btnAdicionarURL.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdicionarURL.FlatAppearance.BorderSize = 0;
            btnAdicionarURL.FlatStyle = FlatStyle.Flat;
            btnAdicionarURL.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            btnAdicionarURL.IconColor = Color.FromArgb(6, 130, 200);
            btnAdicionarURL.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnAdicionarURL.IconSize = 36;
            btnAdicionarURL.Location = new Point(612, 18);
            btnAdicionarURL.Name = "btnAdicionarURL";
            btnAdicionarURL.Size = new Size(32, 32);
            btnAdicionarURL.TabIndex = 76;
            toolTip1.SetToolTip(btnAdicionarURL, "Adicionar Url do vídeo");
            btnAdicionarURL.UseVisualStyleBackColor = true;
            btnAdicionarURL.Click += btnAdicionarURL_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            btnBrowse.IconColor = Color.FromArgb(6, 130, 200);
            btnBrowse.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnBrowse.IconSize = 36;
            btnBrowse.Location = new Point(613, 106);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(32, 32);
            btnBrowse.TabIndex = 77;
            toolTip1.SetToolTip(btnBrowse, "Escolha o local para salvar os vídeos");
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnLimparLista
            // 
            btnLimparLista.FlatAppearance.BorderSize = 0;
            btnLimparLista.FlatStyle = FlatStyle.Flat;
            btnLimparLista.ForeColor = SystemColors.ActiveCaption;
            btnLimparLista.IconChar = FontAwesome.Sharp.IconChar.Broom;
            btnLimparLista.IconColor = Color.FromArgb(6, 130, 200);
            btnLimparLista.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnLimparLista.IconSize = 36;
            btnLimparLista.Location = new Point(9, 136);
            btnLimparLista.Name = "btnLimparLista";
            btnLimparLista.Size = new Size(32, 32);
            btnLimparLista.TabIndex = 78;
            toolTip1.SetToolTip(btnLimparLista, "Limpar Lista");
            btnLimparLista.UseVisualStyleBackColor = true;
            btnLimparLista.Click += btnLimparLista_Click;
            // 
            // btnExcluirSelecionados
            // 
            btnExcluirSelecionados.FlatAppearance.BorderSize = 0;
            btnExcluirSelecionados.FlatStyle = FlatStyle.Flat;
            btnExcluirSelecionados.ForeColor = SystemColors.ActiveCaption;
            btnExcluirSelecionados.IconChar = FontAwesome.Sharp.IconChar.Trash;
            btnExcluirSelecionados.IconColor = Color.FromArgb(6, 130, 200);
            btnExcluirSelecionados.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnExcluirSelecionados.IconSize = 36;
            btnExcluirSelecionados.Location = new Point(56, 136);
            btnExcluirSelecionados.Name = "btnExcluirSelecionados";
            btnExcluirSelecionados.Size = new Size(32, 32);
            btnExcluirSelecionados.TabIndex = 79;
            toolTip1.SetToolTip(btnExcluirSelecionados, "Exclui vídeo selecionado na lista");
            btnExcluirSelecionados.UseVisualStyleBackColor = true;
            btnExcluirSelecionados.Click += btnExcluirSelecionados_Click;
            // 
            // btnPausar
            // 
            btnPausar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPausar.FlatAppearance.BorderSize = 0;
            btnPausar.FlatStyle = FlatStyle.Flat;
            btnPausar.IconChar = FontAwesome.Sharp.IconChar.Pause;
            btnPausar.IconColor = Color.FromArgb(6, 130, 200);
            btnPausar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnPausar.IconSize = 36;
            btnPausar.Location = new Point(532, 18);
            btnPausar.Name = "btnPausar";
            btnPausar.Size = new Size(32, 32);
            btnPausar.TabIndex = 78;
            toolTip1.SetToolTip(btnPausar, "Pausa o Download");
            btnPausar.UseVisualStyleBackColor = true;
            btnPausar.Click += btnPausar_Click;
            // 
            // btnContinuar
            // 
            btnContinuar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnContinuar.FlatAppearance.BorderSize = 0;
            btnContinuar.FlatStyle = FlatStyle.Flat;
            btnContinuar.IconChar = FontAwesome.Sharp.IconChar.Play;
            btnContinuar.IconColor = Color.FromArgb(6, 130, 200);
            btnContinuar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnContinuar.IconSize = 36;
            btnContinuar.Location = new Point(484, 18);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(32, 32);
            btnContinuar.TabIndex = 79;
            toolTip1.SetToolTip(btnContinuar, "Continua download após pausa");
            btnContinuar.UseVisualStyleBackColor = true;
            btnContinuar.Click += btnContinuar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.IconChar = FontAwesome.Sharp.IconChar.Stop;
            btnCancelar.IconColor = Color.FromArgb(6, 130, 200);
            btnCancelar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnCancelar.IconSize = 36;
            btnCancelar.Location = new Point(580, 18);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(32, 32);
            btnCancelar.TabIndex = 80;
            toolTip1.SetToolTip(btnCancelar, "Cancela o Download");
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnDownload
            // 
            btnDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.IconChar = FontAwesome.Sharp.IconChar.Download;
            btnDownload.IconColor = Color.FromArgb(6, 130, 200);
            btnDownload.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnDownload.IconSize = 36;
            btnDownload.Location = new Point(628, 18);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(32, 32);
            btnDownload.TabIndex = 81;
            toolTip1.SetToolTip(btnDownload, "Iniciar o Download");
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // btnFechar
            // 
            btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.FlatStyle = FlatStyle.Flat;
            btnFechar.IconChar = FontAwesome.Sharp.IconChar.DoorOpen;
            btnFechar.IconColor = Color.White;
            btnFechar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnFechar.IconSize = 36;
            btnFechar.Location = new Point(657, 0);
            btnFechar.Name = "btnFechar";
            btnFechar.Size = new Size(32, 32);
            btnFechar.TabIndex = 78;
            toolTip1.SetToolTip(btnFechar, "Adicionar Url do vídeo");
            btnFechar.UseVisualStyleBackColor = true;
            btnFechar.Click += btnFechar_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, Status, Dataprogress, toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 523);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(692, 22);
            statusStrip1.TabIndex = 47;
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
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.ForeColor = Color.White;
            toolStripStatusLabelStatus.LinkColor = Color.FromArgb(8, 128, 255);
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(12, 17);
            toolStripStatusLabelStatus.Text = "-";
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.BackColor = Color.Aquamarine;
            txtUrl.BorderStyle = BorderStyle.FixedSingle;
            txtUrl.Location = new Point(70, 22);
            txtUrl.Margin = new Padding(4, 3, 4, 3);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(538, 23);
            txtUrl.TabIndex = 58;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(303, 1);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(54, 41);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 73;
            pictureBox1.TabStop = false;
            // 
            // lblTotalLinks
            // 
            lblTotalLinks.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotalLinks.AutoSize = true;
            lblTotalLinks.BackColor = Color.Transparent;
            lblTotalLinks.Font = new Font("Microsoft Sans Serif", 10F);
            lblTotalLinks.ForeColor = Color.FromArgb(6, 130, 200);
            lblTotalLinks.Location = new Point(559, 134);
            lblTotalLinks.Margin = new Padding(4, 0, 4, 0);
            lblTotalLinks.Name = "lblTotalLinks";
            lblTotalLinks.Size = new Size(28, 17);
            lblTotalLinks.TabIndex = 51;
            lblTotalLinks.Text = "0/0";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(btnBrowse);
            groupBox1.Controls.Add(btnAdicionarURL);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtUrl);
            groupBox1.Controls.Add(cmbVideoQuality);
            groupBox1.Controls.Add(txtFilePath);
            groupBox1.Controls.Add(txtTitle);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(chkAudioOnly);
            groupBox1.Controls.Add(cmbAudioQuality);
            groupBox1.Location = new Point(11, 102);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(668, 149);
            groupBox1.TabIndex = 75;
            groupBox1.TabStop = false;
            groupBox1.Text = "Entrada/Configurações";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(btnExcluirSelecionados);
            groupBox2.Controls.Add(btnLimparLista);
            groupBox2.Controls.Add(ListBoxURL);
            groupBox2.Controls.Add(lblAnalisando);
            groupBox2.Controls.Add(lblTotalLinks);
            groupBox2.Location = new Point(11, 254);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(668, 182);
            groupBox2.TabIndex = 76;
            groupBox2.TabStop = false;
            groupBox2.Text = "Lista de Downloads:";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(btnDownload);
            groupBox3.Controls.Add(btnCancelar);
            groupBox3.Controls.Add(btnContinuar);
            groupBox3.Controls.Add(btnPausar);
            groupBox3.Location = new Point(12, 437);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(667, 56);
            groupBox3.TabIndex = 77;
            groupBox3.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.Controls.Add(btnFechar);
            panel1.Controls.Add(kryptonLabel1);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(692, 72);
            panel1.TabIndex = 78;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Dock = DockStyle.Bottom;
            kryptonLabel1.Location = new Point(0, 42);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(692, 30);
            kryptonLabel1.StateCommon.ShortText.Color1 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Color2 = Color.White;
            kryptonLabel1.StateCommon.ShortText.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            kryptonLabel1.StateCommon.ShortText.TextH = Krypton.Toolkit.PaletteRelativeAlign.Center;
            kryptonLabel1.TabIndex = 75;
            kryptonLabel1.Values.Text = "Download de vídeos do YouTube";
            // 
            // FrmDowload
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 252, 252);
            ClientSize = new Size(692, 545);
            Controls.Add(panel1);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(lblStatusContagem);
            Controls.Add(statusStrip1);
            Controls.Add(lblProgress);
            Controls.Add(progressBar);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmDowload";
            ShowIcon = false;
            Text = "ConvertPro";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblStatusContagem;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ToolStripStatusLabel Dataprogress;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker Downloader_BackProcess;
        private System.ComponentModel.BackgroundWorker bgWorkerGetVideo;
        private System.Windows.Forms.CheckBox chkAudioOnly;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStatus;
        private System.Windows.Forms.Label lblAnalisando;
        private System.Windows.Forms.ListBox ListBoxURL;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.ComboBox cmbVideoQuality;
        private System.Windows.Forms.ComboBox cmbAudioQuality;
        private PictureBox pictureBox1;
        private Label lblTotalLinks;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Panel panel1;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private FontAwesome.Sharp.IconButton btnAdicionarURL;
        private FontAwesome.Sharp.IconButton btnBrowse;
        private FontAwesome.Sharp.IconButton btnExcluirSelecionados;
        private FontAwesome.Sharp.IconButton btnLimparLista;
        private FontAwesome.Sharp.IconButton btnDownload;
        private FontAwesome.Sharp.IconButton btnCancelar;
        private FontAwesome.Sharp.IconButton btnContinuar;
        private FontAwesome.Sharp.IconButton btnPausar;
        private FontAwesome.Sharp.IconButton btnFechar;
    }
}

