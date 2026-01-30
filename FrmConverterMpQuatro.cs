using Krypton.Toolkit;
using FFMpegCore;
// DEPOIS dos using existentes, ADICIONE:
using Microsoft.WindowsAPICodePack.Dialogs;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Converter
{
    public partial class FrmConverterMpQuatro : KryptonForm
    {
        private List<string> mediaPaths = new List<string>();
        private string outputFolder;
        private BackgroundWorker worker;
        private bool cancelRequested = false;

        // ====== VARIÁVEIS PARA PAUSA ======
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private bool isPaused = false;

        // ====== ADICIONE ESTAS LINHAS ======
        // Constantes para drag & drop visual
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        // ====== FIM DAS ADIÇÕES ======
        // ====== FIM DAS ADIÇÕES ======


        

        public FrmConverterMpQuatro()
        {
            InitializeComponent();

            InitializeCustomControls();

            lblTotalVideos.Text = "Total de arquivos: 0";
            btnCancelar.Enabled = false;
            btnPausar.Enabled = false;
            btnContinuar.Enabled = false;

            // Níveis de compressão para MP3
            cmbAudioQuality.Items.AddRange(new object[] { "32", "64", "80", "128", "256" });
            cmbAudioQuality.SelectedIndex = 3; // Padrão: 128kbps

            // Definir pasta padrão "Músicas" do Windows
            string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            outputFolder = musicFolder;
            txtSaveTo.Text = outputFolder;
        }
        // ====== ADICIONE ESTE MÉTODO NOVO ======
        private void InitializeCustomControls()
        {
            // Configurar drag & drop melhorado
            ConfigureDragDrop();
        }

        private void FlashButton(System.Windows.Forms.Button button)
        {
            var originalColor = button.BackColor;
            button.BackColor = Color.FromArgb(220, 235, 255);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 300;
            timer.Tick += (s, e) =>
            {
                button.BackColor = originalColor;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ConfigureDragDrop()
        {
            // Configurar drag & drop no KryptonListBox
            listBoxVideos.AllowDrop = true;
            listBoxVideos.DragEnter += listBoxVideos_DragEnter;
            listBoxVideos.DragDrop += listBoxVideos_DragDrop;
        }

        private void ToggleControls(bool enabled)
        {
            // Botões básicos sempre seguem 'enabled'
            btnOpenVideo.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnConverter.Enabled = enabled;
            txtSaveTo.Enabled = enabled;
            cmbAudioQuality.Enabled = enabled;
            btnRemoverSelecionados.Enabled = enabled;
            btnLimparLista.Enabled = enabled;

            // ====== LÓGICA PARA BOTÕES DE CONVERSÃO ======
            if (!enabled)
            {
                // INICIANDO CONVERSÃO
                btnCancelar.Enabled = true;
                btnPausar.Enabled = true;
                btnContinuar.Enabled = false;
            }
            else
            {
                // CONVERSÃO TERMINADA/CANCELADA
                btnCancelar.Enabled = false;
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnPausar_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy)
            {
                isPaused = true;
                pauseEvent.Reset(); // Bloqueia a thread
                btnPausar.Enabled = false;
                btnContinuar.Enabled = true;
                btnCancelar.Enabled = false; // <-- ADICIONE ESTA LINHA
                lblStatus.Text = "Conversão pausada...";
                FlashButton(btnPausar);
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
                    string path = item.ToString();
                    listBoxVideos.Items.Remove(item);
                    mediaPaths.Remove(path);
                }

                lblTotalVideos.Text = $"Total de arquivos: {mediaPaths.Count}";
                FlashButton(btnRemoverSelecionados);
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
                btnCancelar.Enabled = true; // <-- ADICIONE ESTA LINHA
                lblStatus.Text = "Continuando conversão...";
                FlashButton(btnContinuar);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.IsBusy) return;

            try
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.Title = "Selecionar pasta de destino";
                    dialog.IsFolderPicker = true;
                    dialog.Multiselect = false;
                    dialog.EnsurePathExists = true;
                    dialog.ShowPlacesList = true;

                    // Usar pasta atual ou Desktop
                    string initialDir = !string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder)
                        ? outputFolder
                        : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    dialog.InitialDirectory = initialDir;

                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        outputFolder = dialog.FileName;
                        txtSaveTo.Text = outputFolder;
                        ShowFeedbackPastaSelecionada();
                    }
                }
            }
            catch
            {
                // Fallback simplificado
                UsarSeletorTradicional();
            }
        }
        // ====== FALLBACK TRADICIONAL ======
        private void UsarSeletorTradicional()
        {
            using (var folderDialog = new FolderBrowserDialog()
            {
                Description = "Selecionar pasta de destino",
                ShowNewFolderButton = true,
                SelectedPath = !string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder)
                    ? outputFolder
                    : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            })
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFolder = folderDialog.SelectedPath;
                    txtSaveTo.Text = outputFolder;
                    ShowFeedbackPastaSelecionada();
                }
            }
        }

        // ====== FEEDBACK VISUAL ======
        private void ShowFeedbackPastaSelecionada()
        {
            // Feedback no textbox
            txtSaveTo.BackColor = Color.FromArgb(240, 255, 240);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 800;
            timer.Tick += (s, args) =>
            {
                txtSaveTo.BackColor = SystemColors.Window;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();

            // Mostrar caminho completo como tooltip
            toolTip1.SetToolTip(txtSaveTo, outputFolder);
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
                isPaused = false;
                pauseEvent.Set(); // Libera a thread se estiver pausada

                // ====== ADICIONE ESTAS 2 LINHAS ======
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
                // ====== FIM DA ADIÇÃO ======

                worker.CancelAsync();
                lblStatus.Text = "Cancelando...";
                FlashButton(btnCancelar);
            }
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            if (mediaPaths.Count == 0 || string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Adicione arquivos e escolha um diretório de saída antes de converter.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ====== ADICIONE ESTAS LINHAS ANTES DE CRIAR O BACKGROUNDWORKER ======
            // Obter valores da UI ANTES de iniciar a thread
            string selectedQuality = cmbAudioQuality.SelectedItem?.ToString() ?? "128";
            int bitrate = int.TryParse(selectedQuality, out int br) ? br : 128;
            // ====== FIM DA ADIÇÃO ======

            ToggleControls(false);
            progressBar.Maximum = mediaPaths.Count;
            progressBar.Value = 0;
            lblStatus.Text = "Iniciando conversão...";
            lblProgress.Text = "0%";
            cancelRequested = false;
            isPaused = false;
            pauseEvent.Set(); // Garante que não inicia pausado

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
                    // Verificar cancelamento
                    if (cancelRequested || worker.CancellationPending)
                    {
                        ev.Cancel = true;
                        return;
                    }

                    // Verificar pausa
                    pauseEvent.WaitOne();

                    string filePath = mediaPaths[i];
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string extension = Path.GetExtension(filePath).ToLower();

                    // Se não for MP4, ignora
                    if (extension != ".mp4") continue;

                    worker.ReportProgress(i + 1, fileName);
                    string outputFilePath = Path.Combine(outputFolder, fileName + ".mp3");

                    try
                    {
                        // Verifica se o arquivo existe e é acessível
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        // ====== REMOVA ESTA PARTE ======
                        // string quality = cmbAudioQuality.SelectedItem?.ToString() ?? "128";
                        // int bitrate = int.TryParse(quality, out int br) ? br : 128;
                        // ====== E USE A VARIÁVEL JÁ OBTIDA ======

                        // Converte MP4 para MP3
                        converter.ConvertMedia(filePath, null, outputFilePath, "mp3", new ConvertSettings
                        {
                            // ====== ALTERE PARA USAR A VARIÁVEL 'bitrate' ======
                            CustomOutputArgs = $"-vn -acodec libmp3lame -b:a {bitrate}k -map 0:a"
                        });
                    }
                    catch (Exception ex)
                    {
                        // Verificar se o erro é por falta de áudio
                        string errorMessage = ex.Message.ToLower();
                        if (errorMessage.Contains("no audio") ||
                            errorMessage.Contains("no streams") ||
                            errorMessage.Contains("stream specifier") ||
                            errorMessage.Contains("map"))
                        {
                            // Mostrar mensagem específica para arquivo sem áudio
                            this.Invoke((MethodInvoker)delegate {
                                MessageBox.Show($"O arquivo \"{fileName}\" não contém trilha de áudio válida e não pode ser convertido para MP3.",
                                                "Arquivo sem áudio",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                            });
                        }
                        else
                        {
                            // Outros erros
                            this.Invoke((MethodInvoker)delegate {
                                MessageBox.Show($"Erro ao converter \"{fileName}\": {ex.Message}",
                                                "Erro na conversão",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                            });
                        }

                        // Continuar com o próximo arquivo
                        continue;
                    }
                }
            };


            worker.ProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                if (!isPaused)
                {
                    lblStatus.Text = $"Processando {ev.UserState} ({ev.ProgressPercentage}/{mediaPaths.Count})";
                }
                lblProgress.Text = $"{(ev.ProgressPercentage * 100 / mediaPaths.Count)}%";

                int index = ev.ProgressPercentage - 1;
                if (index >= 0 && index < listBoxVideos.Items.Count)
                {
                    listBoxVideos.SelectedIndex = index;
                    listBoxVideos.TopIndex = Math.Max(0, index - 5); // Mantém contexto visual
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
                progressBar.Value = progressBar.Maximum;

                // ====== CHAMAR ToggleControls PARA HABILITAR TUDO ======
                ToggleControls(true);

                // Resetar estado de pausa
                isPaused = false;
                pauseEvent.Set();
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
            if (worker != null && worker.IsBusy) return;

            listBoxVideos.Items.Clear();
            mediaPaths.Clear();
            lblTotalVideos.Text = "Total de arquivos: 0";
            FlashButton(btnLimparLista);
        }

        private void listBoxVideos_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var mp4Files = files.Where(f => Path.GetExtension(f).ToLower() == ".mp4").ToArray();

                if (mp4Files.Length > 0)
                {
                    e.Effect = DragDropEffects.Copy;

                    // Feedback visual
                    listBoxVideos.BackColor = Color.FromArgb(240, 245, 255);
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }
        private void FlashListBox()
        {
            var originalColor = listBoxVideos.BackColor;
            listBoxVideos.BackColor = Color.FromArgb(220, 235, 255);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 200;
            timer.Tick += (s, e) =>
            {
                listBoxVideos.BackColor = originalColor;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        private void listBoxVideos_DragDrop(object sender, DragEventArgs e)
        {
            // Restaurar aparência normal do KryptonListBox
            listBoxVideos.BackColor = SystemColors.Window;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] mp4Files = files.Where(f => Path.GetExtension(f).ToLower() == ".mp4").ToArray();

            if (mp4Files.Length > 0)
            {
                AdicionarArquivos(mp4Files);

                // Feedback visual
                FlashListBox();
            }
        }

        private void txtSaveTo_TextChanged(object sender, EventArgs e)
        {
            outputFolder = txtSaveTo.Text;
        }
        // ====== ADICIONE ESTE MÉTODO PARA LIMPEZA DE RECURSOS ======
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
    }
}
