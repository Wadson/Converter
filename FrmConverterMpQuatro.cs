using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using FFMpegCore;
using NReco.VideoConverter;
//using NReco.VideoInfo;

namespace Converter
{
    public partial class FrmConverterMpQuatro : KryptonForm
    {
        private List<string> mediaPaths = new List<string>();
        private string outputFolder;
        private BackgroundWorker worker;
        private bool cancelRequested = false;

        public FrmConverterMpQuatro()
        {
            InitializeComponent();
            lblTotalVideos.Text = "Total de arquivos: 0";
            btnCancelar.Enabled = false;

            // Níveis de compressão para MP3
            cmbNivelCompressao.Items.AddRange(new object[] { "32", "64", "80", "128", "256" });
            cmbNivelCompressao.SelectedIndex = 3; // Padrão: 128kbps
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

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnOpenVideo_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Vídeos MP4|*.mp4"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AdicionarArquivos(openFileDialog.FileNames);
            }
        }

       

        private void AdicionarArquivos(string[] arquivos)
        {
            foreach (var file in arquivos)
            {
                if (!mediaPaths.Contains(file))
                {
                    mediaPaths.Add(file);
                    listBoxVideos.Items.Add(file);
                }
            }
            lblTotalVideos.Text = $"Total de arquivos: {mediaPaths.Count}";
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

        private void btnConverter_Click(object sender, EventArgs e)
        {
            if (mediaPaths.Count == 0 || string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Adicione arquivos e escolha um diretório de saída antes de converter.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);
            progressBar.Maximum = mediaPaths.Count;
            progressBar.Value = 0;
            lblStatus.Text = "Iniciando conversão...";
            lblProgress.Text = "0%";
            cancelRequested = false;

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (s, ev) =>
            {
                var converter = new FFMpegConverter();
                for (int i = 0; i < mediaPaths.Count; i++)
                {
                    if (cancelRequested)
                    {
                        ev.Cancel = true;
                        return;
                    }

                    string filePath = mediaPaths[i];
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    string extension = System.IO.Path.GetExtension(filePath).ToLower();

                    // Se não for MP4, ignora
                    if (extension != ".mp4") continue;

                    worker.ReportProgress(i + 1, fileName);
                    string outputFilePath = System.IO.Path.Combine(outputFolder, fileName + ".mp3");

                    try
                    {
                        // Verifica se o arquivo tem trilha de áudio
                        if (!HasAudioTrack(filePath))
                        {
                            MessageBox.Show($"O arquivo \"{fileName}\" não contém áudio e não pode ser convertido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Converte MP4 para MP3
                        converter.ConvertMedia(filePath, null, outputFilePath, "mp3", new ConvertSettings
                        {
                            CustomOutputArgs = "-vn -acodec libmp3lame -q:a 2 -map 0:a"
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao converter \"{fileName}\": {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                lblStatus.Text = $"Processando {ev.UserState} ({ev.ProgressPercentage}/{mediaPaths.Count})";
                lblProgress.Text = $"{(ev.ProgressPercentage * 100 / mediaPaths.Count)}%";

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
                    lblStatus.Text = "Processo cancelado!";
                    MessageBox.Show("A conversão foi cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lblStatus.Text = "Processo concluído!";
                    MessageBox.Show("Conversão finalizada!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                lblProgress.Text = "100%";
                ToggleControls(true);
            };

            worker.RunWorkerAsync();
        }

        private bool HasAudioTrack(string videoPath)
        {
            try
            {
                var videoInfo = FFProbe.Analyse(videoPath);
                return videoInfo.AudioStreams.Any(s => s != null);
            }
            catch
            {
                return false;
            }
        }

        private void btnLimparLista_Click(object sender, EventArgs e)
        {
            listBoxVideos.Items.Clear();
            mediaPaths.Clear();
            lblTotalVideos.Text = "Total de arquivos: 0";
        }

        private void listBoxVideos_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listBoxVideos_DragDrop(object sender, DragEventArgs e)
        {
            string[] arquivos = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] arquivosMp4 = arquivos.Where(f => Path.GetExtension(f).ToLower() == ".mp4").ToArray();
            AdicionarArquivos(arquivosMp4);
        }

        private void txtSaveTo_TextChanged(object sender, EventArgs e)
        {
            outputFolder = txtSaveTo.Text;
        }
    }
}
