namespace Converter
{
    partial class FrmConverter
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtPathVideo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.txtSaveTo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnConverter = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnOpenVideo = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSair = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.listBoxVideos = new Krypton.Toolkit.KryptonListBox();
            this.lblStatus = new Krypton.Toolkit.KryptonLabel();
            this.progressBar = new Krypton.Toolkit.KryptonProgressBar();
            this.btnCancelar = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.lblTotalVideos = new Krypton.Toolkit.KryptonLabel();
            this.cmbNivelCompressao = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cmbNivelCompressao)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(19, 19);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(107, 20);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Imput vídeo path:";
            // 
            // txtPathVideo
            // 
            this.txtPathVideo.Location = new System.Drawing.Point(19, 40);
            this.txtPathVideo.Name = "txtPathVideo";
            this.txtPathVideo.Size = new System.Drawing.Size(356, 23);
            this.txtPathVideo.TabIndex = 1;
            // 
            // txtSaveTo
            // 
            this.txtSaveTo.Location = new System.Drawing.Point(19, 323);
            this.txtSaveTo.Name = "txtSaveTo";
            this.txtSaveTo.Size = new System.Drawing.Size(356, 23);
            this.txtSaveTo.TabIndex = 2;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(19, 302);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(66, 20);
            this.kryptonLabel2.TabIndex = 3;
            this.kryptonLabel2.Values.Text = "Salvar em:";
            // 
            // btnConverter
            // 
            this.btnConverter.Location = new System.Drawing.Point(19, 427);
            this.btnConverter.Name = "btnConverter";
            this.btnConverter.Size = new System.Drawing.Size(130, 25);
            this.btnConverter.TabIndex = 4;
            this.btnConverter.Values.Text = "Converter";
            this.btnConverter.Click += new System.EventHandler(this.btnConverter_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(381, 323);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 25);
            this.btnSave.TabIndex = 5;
            this.btnSave.Values.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpenVideo
            // 
            this.btnOpenVideo.Location = new System.Drawing.Point(381, 38);
            this.btnOpenVideo.Name = "btnOpenVideo";
            this.btnOpenVideo.Size = new System.Drawing.Size(94, 25);
            this.btnOpenVideo.TabIndex = 6;
            this.btnOpenVideo.Values.Text = "Open Vídeo";
            this.btnOpenVideo.Click += new System.EventHandler(this.btnOpenVideo_Click);
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(345, 427);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(130, 25);
            this.btnSair.TabIndex = 7;
            this.btnSair.Values.Text = "&Sair";
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // listBoxVideos
            // 
            this.listBoxVideos.Location = new System.Drawing.Point(19, 70);
            this.listBoxVideos.Name = "listBoxVideos";
            this.listBoxVideos.Size = new System.Drawing.Size(456, 173);
            this.listBoxVideos.TabIndex = 8;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(0, 478);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(44, 20);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Values.Text = "Status";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(19, 246);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(456, 26);
            this.progressBar.StateCommon.Back.Color1 = System.Drawing.Color.Green;
            this.progressBar.StateDisabled.Back.ColorStyle = Krypton.Toolkit.PaletteColorStyle.OneNote;
            this.progressBar.StateNormal.Back.ColorStyle = Krypton.Toolkit.PaletteColorStyle.OneNote;
            this.progressBar.TabIndex = 10;
            this.progressBar.Values.Text = "";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(187, 427);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 25);
            this.btnCancelar.TabIndex = 11;
            this.btnCancelar.Values.Text = "&C&ancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblTotalVideos
            // 
            this.lblTotalVideos.Location = new System.Drawing.Point(19, 278);
            this.lblTotalVideos.Name = "lblTotalVideos";
            this.lblTotalVideos.Size = new System.Drawing.Size(75, 20);
            this.lblTotalVideos.TabIndex = 12;
            this.lblTotalVideos.Values.Text = "TotalVideos";
            // 
            // cmbNivelCompressao
            // 
            this.cmbNivelCompressao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNivelCompressao.DropDownWidth = 356;
            this.cmbNivelCompressao.Items.AddRange(new object[] {
            "32",
            "64",
            "80",
            "128",
            "256"});
            this.cmbNivelCompressao.Location = new System.Drawing.Point(19, 373);
            this.cmbNivelCompressao.Name = "cmbNivelCompressao";
            this.cmbNivelCompressao.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Blue;
            this.cmbNivelCompressao.Size = new System.Drawing.Size(127, 21);
            this.cmbNivelCompressao.TabIndex = 13;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(19, 352);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(127, 20);
            this.kryptonLabel3.TabIndex = 14;
            this.kryptonLabel3.Values.Text = "Nível de compressão:";
            // 
            // FrmConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(487, 499);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.cmbNivelCompressao);
            this.Controls.Add(this.lblTotalVideos);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.listBoxVideos);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnOpenVideo);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnConverter);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.txtSaveTo);
            this.Controls.Add(this.txtPathVideo);
            this.Controls.Add(this.kryptonLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
            this.Name = "FrmConverter";
            this.StateActive.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateActive.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateCommon.Header.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(128)))), ((int)(((byte)(244)))));
            this.StateCommon.Header.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(244)))));
            this.StateCommon.Header.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.SolidBottomLine;
            this.StateCommon.Header.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StateCommon.Header.Border.Rounding = 20;
            this.StateCommon.Header.Border.Width = 20;
            this.StateCommon.Header.Content.LongText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.SolidLeftLine;
            this.StateInactive.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.SolidAllLine;
            this.StateInactive.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StateInactive.Header.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Header.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Header.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Header.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StateInactive.Header.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Text = "Converter Mp4 pra Mp3";
            ((System.ComponentModel.ISupportInitialize)(this.cmbNivelCompressao)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtPathVideo;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtSaveTo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnConverter;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSave;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOpenVideo;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSair;
        private Krypton.Toolkit.KryptonListBox listBoxVideos;
        private Krypton.Toolkit.KryptonLabel lblStatus;
        private Krypton.Toolkit.KryptonProgressBar progressBar;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancelar;
        private Krypton.Toolkit.KryptonLabel lblTotalVideos;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cmbNivelCompressao;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
    }
}

