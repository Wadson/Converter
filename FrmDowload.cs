using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NReco.VideoConverter;
using Xabe.FFmpeg.Downloader;
using FFMpegCore;
using System.Threading;
using VideoLibrary;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;


namespace Converter
{
    public partial class FrmDowload : KryptonForm
    {
        private YouTube youtube = YouTube.Default;
        public HashSet<string> videoId = new HashSet<string>();
        private long totalbytes = 0;
        private long collctedbytes = 0;
        private string selectedVideoQuality = "";
        private string selectedAudioQuality = "";
        private static string re = @"list=([A-z0-9-]+(&|$))";
        private static Regex listreg = new Regex(re);
        private static string watchLink = "https://www.youtube.com/watch?v=";
        private string formattedUrl;
        int totalVideos = 0;
        int videoAtual = 0;
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private VideoDownloader downloader; // Corrigido o erro CS0246

        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true); // Começa ativo
        public FrmDowload()
        {
            InitializeComponent();

            // Adicione este código ao carregar o formulário
            this.Load += new EventHandler(FrmDowload_Load);

            // Configurações iniciais do TextBox
            txtUrl.Text = "Enter video link";
            txtUrl.GotFocus += RemoveText;
            txtUrl.LostFocus += AddText;

            AtualizarTotalLinks();    //implementado por Wadson
            InitializeButtons();
            DataGridViewURL.RowsAdded += new DataGridViewRowsAddedEventHandler(CheckDataGridView);
            DataGridViewURL.RowsRemoved += new DataGridViewRowsRemovedEventHandler(CheckDataGridView);

            downloader = new VideoDownloader(); // Instância global

        }
        private void CheckDataGridView(object sender, EventArgs e)
        {
            if (DataGridViewURL.Rows.Count > 0)
            {
                btnBrowse.Enabled = true;
            }
            else
            {
                btnCancelar.Enabled = false;
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
                btnDownload.Enabled = false;
                btnBrowse.Enabled = false;
            }
        }
        private void InitializeButtons()
        {
            btnAdicionarURL.Enabled = true;
            btnCancelar.Enabled = false;
            btnPausar.Enabled = false;
            btnContinuar.Enabled = false;
            btnDownload.Enabled = false;
            btnBrowse.Enabled = false;
        }
        //implementado por Wadson
        // Evento para limpar o texto quando o usuário clicar no TextBox
        public void RemoveText(object sender, EventArgs e)
        {
            if (txtUrl.Text == "Enter video link")
            {
                txtUrl.Text = "";
            }
        }

        // Evento para adicionar o texto novamente se o campo estiver vazio
        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                txtUrl.Text = "Enter video link";
            }
        }
        private void AtualizarTotalLinks()
        {
            lblTotalLinks.Text = $"Total de Links: {DataGridViewURL.Rows.Count}";
        }
        // Implementado por Wadson

        private string LimparUrl(string url)
        {
            // Remove parâmetros extras da URL
            Uri uri = new Uri(url);
            string cleanUrl = uri.GetLeftPart(UriPartial.Path); // Obtém apenas o caminho, sem parâmetros
            return cleanUrl;
        }

        // Função para limpar caracteres inválidos no nome do arquivo
        private string CleanFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), string.Empty);
            }
            return fileName;
        }

        // Função para garantir que o diretório de destino exista
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); // Cria o diretório se não existir
            }
        }
        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdicionarURL_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Por favor, insira uma URL.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verifica se a coluna "URLs" já foi adicionada
                if (DataGridViewURL.Columns.Count == 0)
                {
                    DataGridViewURL.Columns.Add("URLs", "URLs");
                    DataGridViewURL.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                // Obtém os vídeos da URL fornecida
                var video = youtube.GetVideo(url);

                if (video == null)
                {
                    MessageBox.Show("URL inválida! Por favor, insira um link válido do YouTube.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string formattedUrl = url;

                // Verifica se a URL já foi adicionada ao DataGridView
                bool isUrlExists = DataGridViewURL.Rows.Cast<DataGridViewRow>()
                                    .Any(row => row.Cells["URLs"].Value?.ToString() == formattedUrl);

                if (!isUrlExists)
                {
                    // Adiciona a URL ao DataGridView
                    DataGridViewURL.Rows.Add(formattedUrl);
                    AtualizarTotalLinks();//implementado por Wadson
                }
                else
                {
                    MessageBox.Show("Este link já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar a URL: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Método para extrair o ID do vídeo do YouTube
        private string ExtrairVideoId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

                if (uri.Host.Contains("youtu.be"))
                {
                    return uri.AbsolutePath.Trim('/');
                }
                else if (uri.Host.Contains("youtube.com"))
                {
                    return query["v"]; // Obtém o parâmetro "v" da URL
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        private void EmptyUrl()
        {
            cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Clear()));
            cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Clear()));
            txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = ""));
            selectedVideoQuality = "";
            selectedAudioQuality = "";
        }

        private string ByteConverter(long b)
        {
            string final;
            //to kb 
            float c = (float)b;
            c /= 1024;
            final = c.ToString("0.00") + " KB";
            if (c >= (float)1)
            {//to mb
                c /= 1024;
                final = c.ToString("0.00") + " MB";
            }
            else if (c >= (float)1)
            {
                //to gb
                c /= 1024;
                final = c.ToString("0.00") + " GB";
            }
            return final;
        }
        private void FileDelete(string pa)
        {
            if (File.Exists(pa))
                File.Delete(pa);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

            // Lógica para iniciar o download
            btnAdicionarURL.Enabled = false;
            btnCancelar.Enabled = true;
            btnPausar.Enabled = true;
            btnContinuar.Enabled = false;
            btnDownload.Enabled = false;


            progressBar.Visible = true;
            if (Downloader_BackProcess.IsBusy != true)
                Downloader_BackProcess.RunWorkerAsync();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Lógica para cancelar o download
            btnCancelar.Enabled = false;
            btnPausar.Enabled = false;
            btnContinuar.Enabled = false;
            btnDownload.Enabled = true;

            cancelTokenSource.Cancel();  // Interrompe o download
            pauseEvent.Set();  // Caso o download esteja pausado, retoma imediatamente.
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            // Lógica para continuar o download
            btnPausar.Enabled = true;
            btnContinuar.Enabled = false;

            ContinuarDownload();
            //downloader.ContinuarDownload();
            ResumeDownload();
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            // Lógica para pausar o download
            btnPausar.Enabled = false;
            btnContinuar.Enabled = true;
            pauseEvent.Reset();  // Pausa o download, bloqueando o fluxo
        }

        private void cmbQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedVideoQuality = cmbQuality.SelectedItem.ToString();
        }

        private void cmbAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAudioQuality = cmbAudioQuality.SelectedItem.ToString();
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            if (!bgWorkerGetVideo.IsBusy)
                bgWorkerGetVideo.RunWorkerAsync();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = true;
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    txtFilePath.Text = folderDialog.SelectedPath + "\\";
                else
                    txtFilePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
        private void ResumeDownload()
        {
            pauseEvent.Set();  // Retoma o download
        }
        private void GetVideoData(string link, bool playlist = false)
        {
            var videoData = youtube.GetAllVideos(link);
            var resolution = videoData.Where(r => r.AdaptiveKind == AdaptiveKind.Video && r.Format == VideoFormat.Mp4).
                              Select(r => r.Resolution);
            var bitRate = videoData.Where(r => r.AdaptiveKind == AdaptiveKind.Audio).Select(r => r.AudioBitrate);
            foreach (var item in resolution)
            {
                if (!cmbQuality.Items.Contains(item))
                    cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Add(item)));
            }
            foreach (var item in bitRate)
            {
                if (!cmbAudioQuality.Items.Contains(item))
                    cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Add(item)));
            }

            if (cmbQuality.Items.Count > 0)
            {
                cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Sorted = true));
                cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.SelectedIndex = 0));
                cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Sorted = true));
                cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.SelectedIndex = 0));
                txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = videoData.ToList()[0].Title));
            }
            if (playlist)
                Status.Text = videoId.Count + " Videos";
            else
                Status.Text = "Single Video";
        }

        // Método para pausar o download
        void PausarDownload()
        {
            pauseEvent.Reset(); // Faz o download esperar
            Status.Text = "Download pausado!";
        }

        // Método para continuar o download
        void ContinuarDownload()
        {
            pauseEvent.Set(); // Permite que o download continue
            Status.Text = "Download retomado!";
        }

        public class VideoDownloader
        {
            private CancellationTokenSource cancelToken = new CancellationTokenSource();
            private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true); // Começa sem estar pausado

            public async Task SourceDownloader(string name, YouTubeVideo vid)
            {
                using (var client = new HttpClient())
                {
                    long? totalByte = 0;
                    Stream output = null;
                    try
                    {
                        output = new FileStream(name, FileMode.Append, FileAccess.Write, FileShare.None);
                        long existingLength = output.Length;

                        using (var request = new HttpRequestMessage(HttpMethod.Head, vid.Uri))
                        {
                            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                            totalByte = response.Content.Headers.ContentLength;
                        }

                        using (var input = await client.GetStreamAsync(vid.Uri))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            int read;
                            int totalRead = (int)existingLength;

                            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                if (cancelToken.Token.IsCancellationRequested)
                                {
                                    break; // Sai do loop ao cancelar
                                }

                                pauseEvent.Wait(); // Aguarda até que o download seja retomado

                                output.Write(buffer, 0, read);
                                totalRead += read;
                            }
                        }
                    }
                    finally
                    {
                        output?.Close();
                        output?.Dispose();
                    }



                }
            }

            // Método para pausar o download
            public void PausarDownload()
            {
                pauseEvent.Reset(); // Pausa o download
            }

            // Método para continuar o download
            public void ContinuarDownload()
            {
                pauseEvent.Set(); // Retoma o download
            }

            // Método para cancelar o download

            public void CancelarDownload()
            {
                cancelToken.Cancel(); // Cancela o download
            }
        }

        async private void Downloader_BackProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            totalbytes = 0;
            collctedbytes = 0;
            Status.Text = "Preparando downloads...";
            cancelTokenSource = new CancellationTokenSource(); // Reinicia o token de cancelamento
            CancellationToken cancelToken = cancelTokenSource.Token;

            List<string> urlsParaDownload = new List<string>();

            async Task SourceDownloader(string name, YouTubeVideo vid)
            {
                using (var client = new HttpClient())
                {
                    long? totalByte = 0;
                    Stream output = null;
                    try
                    {
                        output = new FileStream(name, FileMode.Append, FileAccess.Write, FileShare.None);
                        long existingLength = output.Length;

                        using (var request = new HttpRequestMessage(HttpMethod.Head, vid.Uri))
                        {
                            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                            totalByte = response.Content.Headers.ContentLength;
                        }

                        totalbytes += (long)totalByte;

                        using (var input = await client.GetStreamAsync(vid.Uri))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            int read;
                            int totalRead = (int)existingLength; // Ajustar para retomar o download corretamente

                            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                // Verifica se foi solicitado o cancelamento
                                if (cancelToken.IsCancellationRequested)
                                {
                                    lblStatusContagem.Invoke((MethodInvoker)(() => lblStatusContagem.Text = "Download cancelado!"));
                                    Status.Text = "Download interrompido!";
                                    break;
                                }

                                // Verifica se o download está pausado
                                pauseEvent.Wait(); // Espera até que o download seja retomado

                                output.Write(buffer, 0, read);
                                totalRead += read;
                                collctedbytes += read;

                                long x = totalbytes > 0 ? collctedbytes * 100 / totalbytes : 0;

                                progressBar.Invoke((MethodInvoker)(() =>
                                {
                                    progressBar.Value = (int)x;
                                    lblProgress.Text = $"{x}%";
                                }));

                                Dataprogress.Text = ByteConverter(collctedbytes) + "/" + ByteConverter(totalbytes);
                            }
                        }
                    }
                    finally
                    {
                        output?.Close();
                        output?.Dispose();
                    }

                    if (cancelToken.IsCancellationRequested && File.Exists(name))
                    {
                        try
                        {
                            File.Delete(name);
                        }
                        catch (IOException ex)
                        {
                            lblStatusContagem.Invoke((MethodInvoker)(() => lblStatusContagem.Text = "Erro ao excluir arquivo: " + ex.Message));
                        }
                    }
                }
            }

            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full);

            async Task DownloadWork(string link)
            {
                if (cancelTokenSource.Token.IsCancellationRequested)
                {
                    Status.Text = "Download cancelado!";
                    return;
                }

                Status.Text = $"Baixando: {link}";

                try
                {
                    var video = youtube.GetAllVideos(link);
                    var targetAudio = video.FirstOrDefault(r => r.AdaptiveKind == AdaptiveKind.Audio &&
                                                                r.AudioBitrate.ToString() == selectedAudioQuality);
                    var targetVideo = video.FirstOrDefault(r => r.AdaptiveKind == AdaptiveKind.Video &&
                                                                r.Format == VideoFormat.Mp4 &&
                                                                r.Resolution.ToString() == selectedVideoQuality);

                    if (targetAudio == null || (chkAudioOnly.Checked != true && targetVideo == null))
                    {
                        MessageBox.Show("Erro ao obter vídeo/áudio. Verifique a URL.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string cleanTitle = CleanFileName(video.FirstOrDefault()?.Title ?? "video");
                    string videoPath = Path.Combine(txtFilePath.Text, cleanTitle + ".mp4");
                    string audioPath = Path.Combine(txtFilePath.Text, cleanTitle + ".mp3");

                    EnsureDirectoryExists(txtFilePath.Text);

                    // Caminhos temporários
                    string tempAudio = Path.Combine(txtFilePath.Text, "AudioTemp.mp4");
                    string tempVideo = Path.Combine(txtFilePath.Text, "VideoTemp.mp4");

                    // Baixa o áudio
                    Task audioDownload = SourceDownloader(tempAudio, targetAudio);

                    if (!chkAudioOnly.Checked)
                    {
                        // Baixa o vídeo
                        Task videoDownload = SourceDownloader(tempVideo, targetVideo);

                        await audioDownload;

                        if (cancelTokenSource.Token.IsCancellationRequested)
                        {
                            Status.Text = "Download cancelado!";
                            return;
                        }

                        if (File.Exists(tempAudio))
                        {
                            FFMpeg.ExtractAudio(tempAudio, audioPath);
                            File.Delete(tempAudio);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao baixar áudio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        await videoDownload;

                        if (cancelTokenSource.Token.IsCancellationRequested)
                        {
                            Status.Text = "Download cancelado!";
                            return;
                        }

                        if (File.Exists(tempVideo))
                        {
                            FFMpeg.ReplaceAudio(tempVideo, audioPath, videoPath);
                            File.Delete(tempVideo);
                            File.Delete(audioPath);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao baixar vídeo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        await audioDownload;

                        if (cancelTokenSource.Token.IsCancellationRequested)
                        {
                            Status.Text = "Download cancelado!";
                            return;
                        }

                        if (File.Exists(tempAudio))
                        {
                            FFMpeg.ExtractAudio(tempAudio, audioPath);
                            File.Delete(tempAudio);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao baixar áudio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    Status.Text = "Download concluído!";
                }
                catch (Exception ex)
                {
                    if (cancelTokenSource.Token.IsCancellationRequested)
                    {
                        Status.Text = "Download cancelado!";
                    }
                    else
                    {
                        MessageBox.Show($"Erro durante o download: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Status.Text = "Erro no download!";
                    }
                }


            }


            foreach (DataGridViewRow row in DataGridViewURL.Rows)
            {
                if (row.Cells["URLs"].Value != null)
                {
                    string url = row.Cells["URLs"].Value.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        urlsParaDownload.Add(url);
                    }
                }
            }

            totalVideos = urlsParaDownload.Count;
            videoAtual = 0;

            foreach (var url in urlsParaDownload)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    lblStatusContagem.Invoke((MethodInvoker)(() => lblStatusContagem.Text = "Download cancelado!"));
                    Status.Text = "Download interrompido!";
                    return;
                }

                videoAtual++;
                lblStatusContagem.Invoke((MethodInvoker)(() =>
                {
                    lblStatusContagem.Text = $"Baixando... {videoAtual}/{totalVideos}";
                }));

                await DownloadWork(url);
            }

            lblStatusContagem.Invoke((MethodInvoker)(() =>
            {
                lblStatusContagem.Text = "Download concluído!";
            }));
            Status.Text = "Todos os downloads concluídos!";
        }

        private void bgWorkerGetVideo_DoWork(object sender, DoWorkEventArgs e)
        {
            videoId.Clear();
            Status.Text = "Processing link...";
            if (txtUrl.Text == "")
            {
                EmptyUrl();
                Status.Text = "Empty or Invalid Link";
            }
            else
            {
                try
                {
                    Match m = listreg.Match(txtUrl.Text);
                    if (m.Success)
                    {
                        string listId = m.Groups[1].Value;
                        using (var client = new WebClient())
                        {
                            var responseString = client.DownloadString(txtUrl.Text);
                            string re2 = "\"videoId\":\"([A-z0-9-]+)\",\"playlistId\":\"" + listId + "\"";
                            Regex listOfVideos = new Regex(re);
                            foreach (Match ma in listOfVideos.Matches(responseString))
                            {
                                videoId.Add(ma.Groups[1].Value);
                            }
                            if (videoId.Count > 0)
                                GetVideoData(watchLink + videoId.ElementAt(0), true);
                        }
                    }
                    else
                        GetVideoData(txtUrl.Text);
                }
                catch
                {
                    EmptyUrl();
                    Status.Text = "Invalid Link";
                }
            }
        }

        private void FrmDowload_Load(object sender, EventArgs e)
        {            
        }
    }
}
