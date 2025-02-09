using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NReco.VideoConverter;

namespace Converter
{
    public partial class FrmConverter : KryptonForm
    {
        private List<string> videoPaths = new List<string>();
        private string outputFolder;
        private BackgroundWorker worker;
        private bool cancelRequested = false;

        public FrmConverter()
        {
            InitializeComponent();
            lblTotalVideos.Text = "Total de vídeos: 0";
            btnCancelar.Enabled = false;

            // Preenche o ComboBox com os níveis de compressão
            cmbNivelCompressao.Items.AddRange(new object[] { "32", "64", "80", "128", "256" });
            cmbNivelCompressao.SelectedIndex = 3; // Define 128kbps como padrão
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpenVideo_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP4 Files|*.mp4"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    if (!videoPaths.Contains(file))
                    {
                        videoPaths.Add(file);
                        listBoxVideos.Items.Add(file);
                    }
                }
                lblTotalVideos.Text = $"Total de vídeos: {videoPaths.Count}";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                outputFolder = folderBrowserDialog.SelectedPath;
                txtSaveTo.Text = outputFolder;
            }
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            if (videoPaths.Count == 0 || string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Adicione vídeos e escolha um diretório de saída antes de converter.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbNivelCompressao.SelectedItem == null)
            {
                MessageBox.Show("Selecione um nível de compressão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bitrate = int.Parse(cmbNivelCompressao.SelectedItem.ToString()); // Obtém o bitrate escolhido

            ToggleControls(false);
            progressBar.Maximum = videoPaths.Count;
            progressBar.Value = 0;
            lblStatus.Text = "Iniciando conversão...";
            cancelRequested = false;

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (s, ev) =>
            {
                var converter = new FFMpegConverter();
                for (int i = 0; i < videoPaths.Count; i++)
                {
                    if (cancelRequested)
                    {
                        ev.Cancel = true;
                        return;
                    }

                    string videoPath = videoPaths[i];
                    string videoName = System.IO.Path.GetFileNameWithoutExtension(videoPath);
                    string musicPath = System.IO.Path.Combine(outputFolder, videoName + ".mp3");
                    string compressedMusicPath = System.IO.Path.Combine(outputFolder, videoName + "_compressed.mp3");

                    worker.ReportProgress(i + 1, videoName);

                    // Passo 1: Converter para MP3
                    converter.ConvertMedia(videoPath, musicPath, "mp3");

                    // Passo 2: Comprimir o MP3 no bitrate escolhido
                    var audioSettings = new ConvertSettings();
                    audioSettings.CustomOutputArgs = $"-b:a {bitrate}k"; // Aplica o bitrate selecionado
                    converter.ConvertMedia(musicPath, "mp3", compressedMusicPath, "mp3", audioSettings);

                    // Substitui o arquivo original pelo compactado
                    System.IO.File.Delete(musicPath);
                    System.IO.File.Move(compressedMusicPath, musicPath);
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                lblStatus.Text = $"Convertendo {ev.UserState} ({ev.ProgressPercentage}/{videoPaths.Count})";

                int index = ev.ProgressPercentage - 1;
                if (index >= 0 && index < listBoxVideos.Items.Count)
                {
                    listBoxVideos.SelectedIndex = index;
                    listBoxVideos.TopIndex = index;
                }
            };

            worker.RunWorkerCompleted += (s, ev) =>
            {
                if (ev.Cancelled)
                {
                    lblStatus.Text = "Conversão cancelada!";
                    MessageBox.Show("A conversão foi cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lblStatus.Text = "Conversão concluída!";
                    MessageBox.Show("Todos os vídeos foram convertidos e compactados!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ToggleControls(true);
            };

            worker.RunWorkerAsync();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy)
            {
                cancelRequested = true;
                worker.CancelAsync();
                lblStatus.Text = "Cancelando...";
            }
        }

        private void ToggleControls(bool enabled)
        {
            btnOpenVideo.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnConverter.Enabled = enabled;
            txtSaveTo.Enabled = enabled;
            cmbNivelCompressao.Enabled = enabled;
            btnCancelar.Enabled = !enabled;
        }
    }
}
