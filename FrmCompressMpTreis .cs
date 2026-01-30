using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NReco.VideoConverter;
using System.Drawing;

namespace Converter
{
    public partial class FrmCompressMpTreis : Form
    {
        private List<string> mediaPaths = new List<string>();
        private string outputFolder;
        private BackgroundWorker worker;
        private bool cancelRequested = false;

        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private bool isPaused = false;

        public FrmCompressMpTreis()
        {
            InitializeComponent();
            lblTotalVideos.Text = "Total de arquivos: 0";
            btnCancelar.Enabled = false;
            btnPausar.Enabled = false;
            btnContinuar.Enabled = false;

            // Níveis de compressão para MP3
            cmbNivelCompressao.Items.AddRange(new object[] { "32", "64", "80", "128", "256" });
            cmbNivelCompressao.SelectedIndex = 3; // Padrão: 128kbps

            // Pasta padrão "Músicas"
            outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            txtSalvarEm.Text = outputFolder;

        }

        private void ToggleControls(bool enabled)
        {
            btnOpenAudio.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnConverter.Enabled = enabled;
            txtSalvarEm.Enabled = enabled;
            cmbNivelCompressao.Enabled = enabled;
            btnRemoverSelecionados.Enabled = enabled;
            btnLimparLista.Enabled = enabled;

            if (!enabled)
            {
                // INICIANDO COMPRESSÃO
                btnCancelar.Enabled = true;
                btnPausar.Enabled = true;
                btnContinuar.Enabled = false;
            }
            else
            {
                // COMPRESSÃO TERMINADA/CANCELADA
                btnCancelar.Enabled = false;
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
            }
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
                isPaused = false;
                pauseEvent.Set(); // Libera a thread se estiver pausada

                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;

                worker.CancelAsync();
                lblStatus.Text = "Cancelando...";
            }
        }

        private void ComprimirMpTreis()
        {
            if (mediaPaths.Count == 0 || string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Adicione arquivos e escolha um diretório de saída antes de comprimir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbNivelCompressao.SelectedItem == null || !int.TryParse(cmbNivelCompressao.SelectedItem.ToString(), out int bitrate) || bitrate <= 0)
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

            // Obter bitrate ANTES de iniciar thread
            int selectedBitrate = bitrate;

            ToggleControls(false);
            progressBar.Maximum = mp3Files.Count;
            progressBar.Value = 0;
            lblStatus.Text = "Iniciando compressão...";
            lblProgress.Text = "0%";
            cancelRequested = false;
            isPaused = false;
            pauseEvent.Set();

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (s, ev) =>
            {
                var converter = new FFMpegConverter(); // DESCOMENTADO

                for (int i = 0; i < mp3Files.Count; i++)
                {
                    // Verificar cancelamento
                    if (cancelRequested || worker.CancellationPending)
                    {
                        ev.Cancel = true;
                        return;
                    }

                    // Verificar pausa
                    pauseEvent.WaitOne();

                    string filePath = mp3Files[i];
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    // Nome do arquivo comprimido
                    string compressedFileName = $"{fileName}_{selectedBitrate}kbps.mp3";
                    string compressedFilePath = Path.Combine(outputFolder, compressedFileName);

                    worker.ReportProgress(i + 1, fileName);

                    try
                    {
                        // Verificar se arquivo de entrada existe
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        // Comprimir MP3 - configurações corretas
                        var audioSettings = new ConvertSettings
                        {
                            CustomOutputArgs = $"-codec:a libmp3lame -b:a {selectedBitrate}k -ac 2"
                        };

                        // Executar compressão
                        converter.ConvertMedia(filePath, null, compressedFilePath, "mp3", audioSettings);

                        // Verificar se arquivo foi criado
                        if (File.Exists(compressedFilePath))
                        {
                            // Pequena pausa para sistema processar
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Usar Invoke para mensagens na thread UI
                        string errorMsg = ex.Message;
                        this.Invoke((MethodInvoker)delegate {
                            MessageBox.Show($"Erro ao comprimir \"{fileName}\": {errorMsg}",
                                            "Erro na compressão",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                        });
                        continue;
                    }
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                if (!isPaused)
                {
                    lblStatus.Text = $"Processando {ev.UserState} ({ev.ProgressPercentage}/{mp3Files.Count})";
                }
                lblProgress.Text = $"{(ev.ProgressPercentage * 100 / mp3Files.Count):0}%";

                int index = ev.ProgressPercentage - 1;
                if (index >= 0 && index < listBoxVideos.Items.Count)
                {
                    listBoxVideos.SelectedIndex = index;
                    listBoxVideos.TopIndex = Math.Max(0, index - 5);
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
                    MessageBox.Show($"Compressão finalizada! Arquivos salvos em:\n{outputFolder}",
                                   "Sucesso",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                }

                lblProgress.Text = "100%";
                progressBar.Value = progressBar.Maximum;

                // Restaurar controles
                ToggleControls(true);

                // Resetar estado de pausa
                isPaused = false;
                pauseEvent.Set();
            };

            worker.RunWorkerAsync();
        }
        private void btnConverter_Click(object sender, EventArgs e)
        {
            ComprimirMpTreis();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (worker != null && worker.IsBusy)
            {
                cancelRequested = true;
                isPaused = false;
                pauseEvent.Set();
                worker.CancelAsync();
            }

            pauseEvent?.Dispose();
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
                    listBoxVideos.Items.Add(Path.GetFileName(file)); // SÓ NOME, NÃO CAMINHO
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

        private void btnPausar_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy)
            {
                isPaused = true;
                pauseEvent.Reset(); // Bloqueia a thread
                btnPausar.Enabled = false;
                btnContinuar.Enabled = true;
                btnCancelar.Enabled = false;
                lblStatus.Text = "Compressão pausada...";
            }
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                pauseEvent.Set(); // Libera a thread
                btnPausar.Enabled = true;
                btnContinuar.Enabled = false;
                btnCancelar.Enabled = true;
                lblStatus.Text = "Continuando compressão...";
            }
        }

        private void btnRemoverSelecionados_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            if (listBoxVideos.SelectedItems.Count > 0)
            {
                var selectedItems = new List<object>();
                foreach (var item in listBoxVideos.SelectedItems)
                {
                    selectedItems.Add(item);
                }

                foreach (var item in selectedItems)
                {
                    // Encontrar o caminho completo correspondente ao nome exibido
                    string fileName = item.ToString();
                    string fullPath = mediaPaths.FirstOrDefault(p => Path.GetFileName(p) == fileName);

                    if (fullPath != null)
                    {
                        listBoxVideos.Items.Remove(item);
                        mediaPaths.Remove(fullPath);
                    }
                }

                lblTotalVideos.Text = $"Total de arquivos: {mediaPaths.Count}";
            }
        }
    }
}
