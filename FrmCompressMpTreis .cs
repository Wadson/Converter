using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using FFMpegCore;
using NReco.VideoConverter;
//using NReco.VideoInfo;

namespace Converter
{
    public partial class FrmCompressMpTreis : KryptonForm
    {
        private List<string> mediaPaths = new List<string>();
        private string outputFolder;
        private BackgroundWorker worker;
        private bool cancelRequested = false;

        public FrmCompressMpTreis()
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
            btnOpenAudio.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnConverter.Enabled = enabled;
            txtSalvarEm.Enabled = enabled;
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
                txtSalvarEm.Text = outputFolder;
            }
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

        private void ComprimirMpTreis()
        {

            if (mediaPaths.Count == 0 || string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Adicione arquivos e escolha um diretório de saída antes de converter.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbNivelCompressao.SelectedItem == null || !int.TryParse(cmbNivelCompressao.SelectedItem.ToString(), out int bitrate))
            {
                MessageBox.Show("Selecione um nível de compressão válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Filtra apenas arquivos .mp3
            var mp3Files = mediaPaths.Where(p => Path.GetExtension(p).ToLower() == ".mp3").ToList();
            if (mp3Files.Count == 0)
            {
                MessageBox.Show("Nenhum arquivo MP3 foi encontrado para compressão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);
            progressBar.Maximum = mp3Files.Count;
            progressBar.Value = 0;
            lblStatus.Text = "Iniciando compressão...";
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
                for (int i = 0; i < mp3Files.Count; i++)
                {
                    if (cancelRequested)
                    {
                        ev.Cancel = true;
                        return;
                    }

                    string filePath = mp3Files[i];
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string compressedFilePath = Path.Combine(outputFolder, fileName + "_compressed.mp3");

                    worker.ReportProgress(i + 1, fileName);

                    try
                    {
                        var audioSettings = new ConvertSettings
                        {
                            CustomOutputArgs = $"-b:a {bitrate}k -acodec libmp3lame"
                        };

                        converter.ConvertMedia(filePath, null, compressedFilePath, "mp3", audioSettings);

                        if (File.Exists(compressedFilePath))
                        {
                            Thread.Sleep(500); // Pequeno delay para garantir que o sistema finalize o processo

                            string finalPath = Path.Combine(outputFolder, fileName + "_compressed.mp3");
                            File.Move(compressedFilePath, finalPath);

                            if (File.Exists(finalPath))
                            {
                                Console.WriteLine($"Arquivo \"{fileName}\" compactado com sucesso!");
                            }
                            else
                            {
                                MessageBox.Show($"Erro ao mover o arquivo compactado \"{fileName}\".", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Falha ao comprimir \"{fileName}\". O arquivo de saída não foi criado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao comprimir \"{fileName}\": {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                lblStatus.Text = $"Processando {ev.UserState} ({ev.ProgressPercentage}/{mp3Files.Count})";
                lblProgress.Text = $"{(ev.ProgressPercentage * 100 / mp3Files.Count):0}%";

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
                    MessageBox.Show("A compressão foi cancelada.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lblStatus.Text = "Processo concluído!";
                    MessageBox.Show("Compressão finalizada!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                lblProgress.Text = "100%";
                ToggleControls(true);
            };

            worker.RunWorkerAsync();
        }
        private void btnConverter_Click(object sender, EventArgs e)
        {
          ComprimirMpTreis();
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

        private void btnOpenAudio_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Arquivos MP3|*.mp3"
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
            string[] arquivosMp3 = arquivos.Where(f => Path.GetExtension(f).ToLower() == ".mp3").ToArray();
            AdicionarArquivos(arquivosMp3);
        }

        private void txtSalvarEm_TextChanged(object sender, EventArgs e)
        {
            outputFolder = txtSalvarEm.Text;
        }
    }
}
