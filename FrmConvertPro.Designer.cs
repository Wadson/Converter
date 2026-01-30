namespace Converter
{
    partial class FrmConvertPro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConvertPro));
            pnlContainer = new Panel();
            pictureBox1 = new PictureBox();
            MenuVertical = new Panel();
            kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            btnConverterMpTreis = new Button();
            lbluser = new Label();
            pictureBox2 = new PictureBox();
            btnDownload = new Button();
            btnConvertMp = new Button();
            btnlogoInicio = new PictureBox();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            MenuVertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnlogoInicio).BeginInit();
            SuspendLayout();
            // 
            // pnlContainer
            // 
            pnlContainer.BackColor = Color.White;
            pnlContainer.Controls.Add(pictureBox1);
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.Location = new Point(152, 0);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(692, 545);
            pnlContainer.TabIndex = 8;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.None;
            pictureBox1.Image = Properties.Resources.LogoQuadradoTreis;
            pictureBox1.Location = new Point(235, 171);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(187, 174);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // MenuVertical
            // 
            MenuVertical.BackColor = Color.White;
            MenuVertical.BorderStyle = BorderStyle.FixedSingle;
            MenuVertical.Controls.Add(kryptonLabel1);
            MenuVertical.Controls.Add(btnConverterMpTreis);
            MenuVertical.Controls.Add(lbluser);
            MenuVertical.Controls.Add(pictureBox2);
            MenuVertical.Controls.Add(btnDownload);
            MenuVertical.Controls.Add(btnConvertMp);
            MenuVertical.Controls.Add(btnlogoInicio);
            MenuVertical.Dock = DockStyle.Left;
            MenuVertical.Location = new Point(0, 0);
            MenuVertical.Margin = new Padding(4, 3, 4, 3);
            MenuVertical.Name = "MenuVertical";
            MenuVertical.Size = new Size(152, 545);
            MenuVertical.TabIndex = 6;
            // 
            // kryptonLabel1
            // 
            kryptonLabel1.Location = new Point(3, 406);
            kryptonLabel1.Name = "kryptonLabel1";
            kryptonLabel1.Size = new Size(108, 52);
            kryptonLabel1.TabIndex = 1;
            kryptonLabel1.Values.Text = "ConvertPro\r\n• Versão 1.2.3\r\n• © 2026 WR Soft\r\n";
            // 
            // btnConverterMpTreis
            // 
            btnConverterMpTreis.FlatAppearance.CheckedBackColor = Color.FromArgb(183, 217, 226);
            btnConverterMpTreis.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 199, 137);
            btnConverterMpTreis.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnConverterMpTreis.FlatStyle = FlatStyle.Flat;
            btnConverterMpTreis.Font = new Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnConverterMpTreis.ForeColor = Color.FromArgb(10, 142, 255);
            btnConverterMpTreis.Image = Properties.Resources._32x32_Compress;
            btnConverterMpTreis.ImageAlign = ContentAlignment.MiddleLeft;
            btnConverterMpTreis.Location = new Point(1, 170);
            btnConverterMpTreis.Margin = new Padding(4, 3, 4, 3);
            btnConverterMpTreis.Name = "btnConverterMpTreis";
            btnConverterMpTreis.Size = new Size(189, 46);
            btnConverterMpTreis.TabIndex = 17;
            btnConverterMpTreis.Text = "Compress";
            btnConverterMpTreis.UseVisualStyleBackColor = true;
            btnConverterMpTreis.Click += btnConverterMpTreis_Click;
            // 
            // lbluser
            // 
            lbluser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbluser.AutoSize = true;
            lbluser.Font = new Font("Century Gothic", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbluser.ForeColor = Color.White;
            lbluser.Location = new Point(84, 501);
            lbluser.Margin = new Padding(4, 0, 4, 0);
            lbluser.Name = "lbluser";
            lbluser.Size = new Size(54, 17);
            lbluser.TabIndex = 16;
            lbluser.Text = "Usuario";
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(1, 463);
            pictureBox2.Margin = new Padding(4, 3, 4, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(82, 76);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 15;
            pictureBox2.TabStop = false;
            // 
            // btnDownload
            // 
            btnDownload.FlatAppearance.CheckedBackColor = Color.FromArgb(183, 217, 226);
            btnDownload.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 199, 137);
            btnDownload.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.Font = new Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDownload.ForeColor = Color.FromArgb(10, 142, 255);
            btnDownload.Image = Properties.Resources._32x32_Download;
            btnDownload.ImageAlign = ContentAlignment.MiddleLeft;
            btnDownload.Location = new Point(1, 122);
            btnDownload.Margin = new Padding(4, 3, 4, 3);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(189, 46);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // btnConvertMp
            // 
            btnConvertMp.FlatAppearance.CheckedBackColor = Color.FromArgb(183, 217, 226);
            btnConvertMp.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 199, 137);
            btnConvertMp.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 199, 137);
            btnConvertMp.FlatStyle = FlatStyle.Flat;
            btnConvertMp.Font = new Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnConvertMp.ForeColor = Color.FromArgb(10, 142, 255);
            btnConvertMp.Image = Properties.Resources._32x32_Converter;
            btnConvertMp.ImageAlign = ContentAlignment.MiddleLeft;
            btnConvertMp.Location = new Point(1, 75);
            btnConvertMp.Margin = new Padding(4, 3, 4, 3);
            btnConvertMp.Name = "btnConvertMp";
            btnConvertMp.Size = new Size(189, 46);
            btnConvertMp.TabIndex = 1;
            btnConvertMp.Text = "Conversor";
            btnConvertMp.UseVisualStyleBackColor = true;
            btnConvertMp.Click += btnConvertMp_Click;
            // 
            // btnlogoInicio
            // 
            btnlogoInicio.BackColor = Color.Transparent;
            btnlogoInicio.BackgroundImageLayout = ImageLayout.Stretch;
            btnlogoInicio.Dock = DockStyle.Top;
            btnlogoInicio.Image = Properties.Resources.LogoConvertPro2__WR;
            btnlogoInicio.Location = new Point(0, 0);
            btnlogoInicio.Margin = new Padding(4, 3, 4, 3);
            btnlogoInicio.Name = "btnlogoInicio";
            btnlogoInicio.Padding = new Padding(23, 0, 0, 0);
            btnlogoInicio.Size = new Size(150, 53);
            btnlogoInicio.SizeMode = PictureBoxSizeMode.StretchImage;
            btnlogoInicio.TabIndex = 0;
            btnlogoInicio.TabStop = false;
            // 
            // FrmConvertPro
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(844, 545);
            Controls.Add(pnlContainer);
            Controls.Add(MenuVertical);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmConvertPro";
            PaletteMode = Krypton.Toolkit.PaletteMode.Office2007Blue;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Converter Mp4 pra Mp3";
            pnlContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            MenuVertical.ResumeLayout(false);
            MenuVertical.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnlogoInicio).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Panel MenuVertical;
        private System.Windows.Forms.Label lbluser;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnConvertMp;
        private System.Windows.Forms.PictureBox btnlogoInicio;
        private System.Windows.Forms.Button btnConverterMpTreis;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}

