using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
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
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;


namespace Converter
{
    public partial class FrmDowload : KryptonForm
    {
        private YouTube youtube = YouTube.Default;
        public HashSet<string> videoId = new HashSet<string>();
        private List<string> mediaPaths = new List<string>();
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
            
            // Configurações iniciais do TextBox
            txtUrl.Text = "Enter video link";
            txtUrl.GotFocus += RemoveText;
            txtUrl.LostFocus += AddText;           
            InitializeButtons();
            ListBoxURL.SelectedIndexChanged += new EventHandler(CheckListBox);
            downloader = new VideoDownloader(this); // Instância global
        }
        private void CheckListBox(object sender, EventArgs e)
        {
            if (ListBoxURL.Items.Count > 0)
            {
                // Habilita os botões de Browse e Download quando houver itens no ListBox
                btnBrowse.Enabled = true;
                btnDownload.Enabled = true;
            }
            else
            {
                // Desabilita todos os botões quando não houver itens no ListBox
                btnCancelar.Enabled = false;
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
                btnDownload.Enabled = false;
                btnBrowse.Enabled = false;
            }
        }

        private void InitializeButtons()
        {
            // Inicializa os botões com seus estados iniciais
            btnAdicionarURL.Enabled = true;
            btnCancelar.Enabled = false;
            btnPausar.Enabled = false;
            btnContinuar.Enabled = false;
            btnDownload.Enabled = false;
            btnBrowse.Enabled = false;
        }       
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

        //private void AdicionarLinkListBoxURL()
        //{
        //    string url = txtUrl.Text.Trim();

        //    if (string.IsNullOrEmpty(url))
        //    {
        //        MessageBox.Show("Please enter a valid URL");
        //        return;
        //    }

        //    // Verifica se a URL já foi adicionada ao ListBox antes de adicionar
        //    if (ListBoxURL.Items.Contains(url))
        //    {
        //        MessageBox.Show("Este link já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        // Obtém os vídeos da URL fornecida
        //        var video = youtube.GetVideo(url);

        //        if (video == null)
        //        {
        //            MessageBox.Show("URL inválida! Por favor, insira um link válido do YouTube.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        // Adiciona a URL ao ListBox somente se não for duplicada
        //        ListBoxURL.Items.Add(url);
        //        txtUrl.Clear();  // Limpa o campo de texto

        //        CheckListBox(null, EventArgs.Empty);  // Verifica se os botões precisam ser habilitados ou desabilitados
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Erro ao processar a URL: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void AtualizarTotalLinks()
        {
            lblTotalLinks.Text = $"Total de Links: {ListBoxURL.Items.Count}";
        }

        private void GetVideoData(string link, bool playlist = false)
        {
            var videoData = youtube.GetAllVideos(link);

            // Verificação de link inválido
            if (videoData == null || !videoData.Any())
            {
                MessageBox.Show("Link inválido ou o vídeo não pôde ser carregado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var resolutions = videoData
                .Where(r => r.AdaptiveKind == AdaptiveKind.Video && r.Format == VideoFormat.Mp4)
                .Select(r => r.Resolution)
                .Distinct()
                .OrderBy(r => r);

            var bitRates = videoData
                .Where(r => r.AdaptiveKind == AdaptiveKind.Audio)
                .Select(r => r.AudioBitrate)
                .Distinct()
                .OrderBy(b => b);

            // Limpa os itens anteriores antes de adicionar novos
            cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Clear()));
            cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Clear()));

            foreach (var item in resolutions)
            {
                cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Add(item)));
            }

            foreach (var item in bitRates)
            {
                cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Add(item)));
            }

            if (cmbQuality.Items.Count > 0)
            {
                cmbQuality.Invoke((MethodInvoker)(() =>
                {
                    cmbQuality.Sorted = true;
                    cmbQuality.SelectedIndex = 0;
                }));
                cmbAudioQuality.Invoke((MethodInvoker)(() =>
                {
                    cmbAudioQuality.Sorted = true;
                    cmbAudioQuality.SelectedIndex = 0;
                }));
            }

            // Atualização do título do vídeo
            var firstVideo = videoData.FirstOrDefault();
            if (firstVideo != null)
            {
                txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = firstVideo.Title));
            }
            else
            {
                txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = "Título não disponível"));
            }

            Status.Text = playlist ? $"{videoData.Count()} Vídeos" : "Vídeo Único";
            Status.BackColor = System.Drawing.Color.Green;
        }
        private async void AdicionarURL()
        {
            string url = txtUrl.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Por favor, insira uma URL.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cts = new CancellationTokenSource();

            try
            {
                HabilitarControles(false); // Desabilita controles
                lblAnalisando.Visible = true; // Exibe "Analisando..."
                var token = cts.Token;
                var animationTask = AnalisarMensagemAnimation(token); // Inicia animação

                // 🔎 Verificação completa do link
                bool linkValido = await VerificarLinkAsync(url);

                cts.Cancel(); // Encerra a animação após a verificação



                if (!linkValido)
                {
                    MessageBox.Show("O link inserido é inválido ou não está acessível.", "Link Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Impede a adição no ListBox
                }

                // Verifica se o link é duplicado
                if (linkValido)
                {
                    if (!ListBoxURL.Items.Contains(url))
                    {
                        ListBoxURL.Items.Add(url);
                        AtualizarTotalLinks();
                        // Chamar GetVideoData de forma assíncrona para não bloquear a UI
                        Task.Run(() => GetVideoData(url));
                        txtUrl.Clear(); // Limpa o campo de texto
                        txtUrl.Focus(); // Foca no campo de texto
                        txtUrl.BackColor = System.Drawing.Color.YellowGreen; // Restaura a cor de fundo
                    }
                    else
                    {
                        MessageBox.Show("Este link já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                //Acima pode deletar

                //// Se válido e não estiver na lista, adiciona
                //// Verifica se a URL já foi adicionada à ListBox
                //if (!ListBoxURL.Items.Contains(url))
                //{
                //    ListBoxURL.Items.Add(url);
                //    AtualizarTotalLinks();
                //}
                //else
                //{
                //    MessageBox.Show("Este link já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar a URL: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblAnalisando.Visible = false;
                HabilitarControles(true); // Reabilita controles
                cts.Dispose();
            }
        }
        private async Task<bool> VerificarLinkAsync(string url)
        {
            // 1️⃣ Verifica se a URL é válida e usa http/https
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) ||
                !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return false;
            }

            try
            {
                // 2️⃣ Verificação rápida de acessibilidade (HTTP)
                using (var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
                {
                    var response = await httpClient.GetAsync(uriResult);
                    if (!response.IsSuccessStatusCode)
                        return false; // Link não acessível
                }

                // 3️⃣ Verificação se o link é de um vídeo válido do YouTube
                var videoData = await youtube.GetVideoAsync(url); // Usa método assíncrono (se disponível)

                return videoData != null; // Retorna true se informações do vídeo forem carregadas
            }
            catch
            {
                return false; // Qualquer erro => link inválido
            }
        }


        //private async void AdicionarURL()
        //{
        //    string url = txtUrl.Text.Trim();

        //    if (string.IsNullOrEmpty(url))
        //    {
        //        MessageBox.Show("Por favor, insira uma URL.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    var cts = new CancellationTokenSource();

        //    try
        //    {
        //        // Desabilitar todos os controles do formulário
        //        HabilitarControles(false);

        //        // Mostrar mensagem de análise e iniciar animação
        //        lblAnalisando.Visible = true;
        //        var token = cts.Token;
        //        var animationTask = AnalisarMensagemAnimation(token); // Inicia a animação (não bloqueia a UI)

        //        // Simulação da análise do link (substitua pelo seu método real de verificação)
        //        await Task.Delay(3000); // Simula análise de 3 segundos (ou chame seu método assíncrono aqui)

        //        // Verifica se a URL já foi adicionada à ListBox
        //        if (!ListBoxURL.Items.Contains(url))
        //        {
        //            ListBoxURL.Items.Add(url); // Adiciona a URL
        //            AtualizarTotalLinks();     // Atualiza total de links
        //        }
        //        else
        //        {
        //            MessageBox.Show("Este link já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Erro ao processar a URL: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        cts.Cancel(); // Encerra a animação
        //        lblAnalisando.Visible = false;
        //        HabilitarControles(true); // Reabilita os controles
        //        cts.Dispose(); // Libera recursos
        //    }
        //}

        private async Task AnalisarMensagemAnimation(CancellationToken token)
        {
            int pontos = 0;

            // Atualiza o texto antes de iniciar a animação
            lblAnalisando.Text = "Analisando";

            while (!token.IsCancellationRequested)
            {
                pontos = (pontos + 1) % 4; // Incrementa de 0 a 3 pontos
                lblAnalisando.Text = "Analisando" + new string('.', pontos);
                await Task.Delay(500, token); // Aguarda 500ms ou cancela se solicitado
            }
        }

        private void HabilitarControles(bool habilitar)
        {
            txtUrl.Enabled = habilitar;
            btnAdicionarURL.Enabled = habilitar;
            ListBoxURL.Enabled = habilitar;
            // Inclua outros controles que precisar aqui
        }


        private void btnAdicionarURL_Click(object sender, EventArgs e)
        {
          AdicionarURL();
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
            try
            {
                cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Clear()));
                cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Clear()));
                txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = ""));
                selectedVideoQuality = "";
                selectedAudioQuality = "";
            }
            catch
            {
                return;
            }
        }

        public static string ByteConverter(long bytes)
        {
            // Sua implementação para converter bytes em uma string legível
            // Exemplo:
            if (bytes >= 1073741824)
                return $"{bytes / 1073741824} GB";
            if (bytes >= 1048576)
                return $"{bytes / 1048576} MB";
            if (bytes >= 1024)
                return $"{bytes / 1024} KB";
            return $"{bytes} bytes";
        }
        private void FileDelete(string pa)
        {
            if (File.Exists(pa))
                File.Delete(pa);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

            // Inicia o download e configura os botões
            btnAdicionarURL.Enabled = false;   // Desabilita o botão de adicionar URL
            btnCancelar.Enabled = true;        // Habilita o botão de cancelar
            btnPausar.Enabled = true;          // Habilita o botão de pausar
            btnContinuar.Enabled = false;      // Desabilita o botão de continuar
            btnDownload.Enabled = false;       // Desabilita o botão de download

            progressBar.Visible = true;        // Exibe a barra de progresso

            // Inicia o processo de download em segundo plano
            if (!Downloader_BackProcess.IsBusy)
                Downloader_BackProcess.RunWorkerAsync();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Cancela o download e altera os botões
            btnCancelar.Enabled = false;   // Desabilita o botão de cancelar
            btnPausar.Enabled = false;     // Desabilita o botão de pausar
            btnContinuar.Enabled = false;  // Desabilita o botão de continuar
            btnDownload.Enabled = true;    // Habilita o botão de download

            cancelTokenSource.Cancel();    // Interrompe o download
            pauseEvent.Set();              // Retoma imediatamente se o download estiver pausado
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            // Continua o download
            btnPausar.Enabled = true;      // Habilita o botão de pausar
            btnContinuar.Enabled = false;  // Desabilita o botão de continuar

            ContinuarDownload();           // Função que retoma o download
            ResumeDownload();              // Retoma o download pausado
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            // Pausa o download
            pauseEvent.Reset();            // Faz o download esperar
            Status.Text = "Download pausado!";
            Status.BackColor = System.Drawing.Color.Red;
            btnContinuar.Enabled = true;   // Habilita o botão de continuar
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
            btnDownload.Enabled = true;  // Habilita o botão de Download após escolher a pasta
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    txtFilePath.Text = folderDialog.SelectedPath + "\\";
                else
                    txtFilePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
        void PausarDownload()
        {
            pauseEvent.Reset(); // Faz o download esperar
            Status.Text = "Download pausado!"; Status.BackColor = System.Drawing.Color.Red;
        }

        private void ResumeDownload()
        {
            pauseEvent.Set();  // Retoma o download
        }
        // Método para continuar o download
        void ContinuarDownload()
        {
            pauseEvent.Set(); // Permite que o download continue
            Status.Text = "Download retomado!"; Status.BackColor = System.Drawing.Color.Green;
        }

        // Método para cancelar o download
        public void CancelarDownload()
        {
            cancelToken.Cancel(); // Cancela o download
        }
        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }














       


        //private void GetVideoData(string link, bool playlist = false)
        //{
        //    var videoData = youtube.GetAllVideos(link);
        //    var resolution = videoData.Where(r => r.AdaptiveKind == AdaptiveKind.Video && r.Format == VideoFormat.Mp4).
        //                               Select(r => r.Resolution);
        //    var bitRate = videoData.Where(r => r.AdaptiveKind == AdaptiveKind.Audio).Select(r => r.AudioBitrate);
        //    foreach (var item in resolution)
        //    {
        //        if (!cmbQuality.Items.Contains(item))
        //            cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Items.Add(item)));
        //    }
        //    foreach (var item in bitRate)
        //    {
        //        if (!cmbAudioQuality.Items.Contains(item))
        //            cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Items.Add(item)));
        //    }

        //    if (cmbQuality.Items.Count > 0)
        //    {
        //        cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.Sorted = true));
        //        cmbQuality.Invoke((MethodInvoker)(() => cmbQuality.SelectedIndex = 0));
        //        cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.Sorted = true));
        //        cmbAudioQuality.Invoke((MethodInvoker)(() => cmbAudioQuality.SelectedIndex = 0));
        //        txtTitle.Invoke((MethodInvoker)(() => txtTitle.Text = videoData.ToList()[0].Title));
        //    }
        //    if (playlist)
        //        Status.Text = videoId.Count + " Videos";
        //    else
        //        Status.Text = "Single Video"; Status.BackColor = System.Drawing.Color.Green;
        //}

        public class VideoDownloader
        {
            private CancellationTokenSource cancelToken = new CancellationTokenSource();
            private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true); // Começa sem estar pausado
            private int collctedbytes;
            private FrmDowload _form;

            // Construtor que recebe a referência do formulário
            public VideoDownloader(FrmDowload form)
            {
                _form = form;
            }

            public async Task SourceDownloader(string name, YouTubeVideo vid)
            {
                using (var client = new HttpClient())
                {
                    long? totalByte = 0;  // Inicializa a variável totalByte
                    Stream output = null;

                    try
                    {
                        // Cria o arquivo para gravar os dados
                        output = new FileStream(name, FileMode.Append, FileAccess.Write, FileShare.None);
                        long existingLength = output.Length;

                        // Obter o tamanho total do arquivo
                        using (var request = new HttpRequestMessage(HttpMethod.Head, vid.Uri))
                        {
                            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                            totalByte = response.Content.Headers.ContentLength;
                        }

                        // Se totalByte for zero, usa o tamanho do arquivo existente
                        if (totalByte == 0)
                        {
                            totalByte = existingLength;
                        }

                        using (var input = await client.GetStreamAsync(vid.Uri))
                        {
                            byte[] buffer = new byte[16 * 1024];  // Buffer de 16 KB
                            int read;
                            int totalRead = (int)existingLength;

                            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                // Se o cancelamento for solicitado, interrompe o download
                                if (cancelToken.Token.IsCancellationRequested)
                                {
                                    break; // Sai do loop ao cancelar
                                }

                                // Aguarda até que o download seja retomado
                                pauseEvent.Wait();

                                // Escreve os dados no arquivo
                                output.Write(buffer, 0, read);
                                totalRead += read;

                                // Atualizando os bytes coletados
                                collctedbytes += read;

                                // Calculando o progresso (percentual)
                                int progress = totalByte > 0 ? (int)(collctedbytes * 100 / totalByte) : 0;

                                // Atualizando a barra de progresso na UI
                                _form.Invoke((MethodInvoker)(() =>
                                {
                                    _form.progressBar.Value = progress;  // Atualiza o valor da barra
                                    _form.lblProgress.Text = $"{progress}%";  // Atualiza o texto da porcentagem
                                }));

                                // Atualizando a exibição de bytes baixados
                                _form.Invoke((MethodInvoker)(() =>
                                {
                                    // Usando o ByteConverter do formulário para exibir os bytes baixados
                                    _form.Dataprogress.Text = FrmDowload.ByteConverter(collctedbytes) + "/" + FrmDowload.ByteConverter(totalByte.Value);
                                }));
                            }
                        }
                    }
                    finally
                    {
                        // Fecha e libera os recursos de arquivo
                        output?.Close();
                        output?.Dispose();
                    }
                }
            }
        }


        async private void Downloader_BackProcess_DoWork(object sender, DoWorkEventArgs e)
        {

            totalbytes = 0;
            collctedbytes = 0;
            Status.Text = "Preparando downloads..."; Status.BackColor = System.Drawing.Color.Green;
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

            // Mudança para utilizar o ListBoxURL
            foreach (var urlItem in ListBoxURL.Items)
            {
                string url = urlItem.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    urlsParaDownload.Add(url);
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

                // Encontrar o índice correspondente ao URL no ListBoxURL
                int index = ListBoxURL.Items.IndexOf(url);
                if (index >= 0)
                {
                    ListBoxURL.Invoke((MethodInvoker)(() =>
                    {
                        ListBoxURL.SelectedIndex = index;
                        ListBoxURL.SelectedItem = url; // Define o item selecionado
                    }));
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

        async private Task DownloadWork(string link, bool isAudioOnly = false)
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
                var targetAudio = video.FirstOrDefault(r => r.AdaptiveKind == AdaptiveKind.Audio);
                var targetVideo = video.FirstOrDefault(r => r.AdaptiveKind == AdaptiveKind.Video && r.Format == VideoFormat.Mp4);

                // Se o usuário selecionou baixar só o áudio
                if (isAudioOnly)
                {
                    if (targetAudio == null)
                    {
                        MessageBox.Show("Erro: Não foi possível encontrar o áudio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Status.Text = "Erro ao obter áudio.";
                        return;
                    }

                    string cleanTitle = CleanFileName(video.FirstOrDefault()?.Title ?? "audio");
                    string audioPath = Path.Combine(txtFilePath.Text, cleanTitle + "_audio.mp4");

                    EnsureDirectoryExists(txtFilePath.Text);

                    if (targetAudio.Uri == null)
                    {
                        MessageBox.Show("Erro: URL de áudio inválida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Status.Text = "Erro nas URLs.";
                        return;
                    }

                    // Baixar apenas o áudio
                    Uri audioUri = new Uri(targetAudio.Uri); // Convertendo para Uri
                    await DownloadFile(audioUri, audioPath);

                    if (cancelTokenSource.Token.IsCancellationRequested)
                    {
                        Status.Text = "Download cancelado!";
                        return;
                    }

                    Status.Text = "Download de áudio concluído!";
                }
                else
                {
                    // Se o usuário não selecionou apenas o áudio, baixar vídeo e áudio
                    if (targetAudio == null || targetVideo == null)
                    {
                        MessageBox.Show("Erro ao obter vídeo/áudio. Verifique a URL.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Status.Text = "Erro ao obter vídeo/áudio.";
                        return;
                    }

                    string cleanTitle = CleanFileName(video.FirstOrDefault()?.Title ?? "video");
                    string videoPath = Path.Combine(txtFilePath.Text, cleanTitle + "_video.mp4");
                    string audioPath = Path.Combine(txtFilePath.Text, cleanTitle + "_audio.mp4");

                    EnsureDirectoryExists(txtFilePath.Text);

                    if (targetVideo.Uri == null || targetAudio.Uri == null)
                    {
                        MessageBox.Show("Erro: URL de vídeo ou áudio inválida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Status.Text = "Erro nas URLs.";
                        return;
                    }

                    // Baixar vídeo e áudio separadamente
                    Uri videoUri = new Uri(targetVideo.Uri); // Convertendo para Uri
                    Uri audioUri = new Uri(targetAudio.Uri); // Convertendo para Uri
                    await DownloadFile(videoUri, videoPath);
                    await DownloadFile(audioUri, audioPath);

                    if (cancelTokenSource.Token.IsCancellationRequested)
                    {
                        Status.Text = "Download cancelado!";
                        return;
                    }

                    // Combinar vídeo e áudio usando FFmpeg
                    string outputPath = Path.Combine(txtFilePath.Text, cleanTitle + ".mp4");
                    CombineVideoAndAudio(videoPath, audioPath, outputPath);

                    // Excluir arquivos temporários de vídeo e áudio
                    File.Delete(videoPath);
                    File.Delete(audioPath);

                    Status.Text = "Download de vídeo e áudio concluído!";
                }
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



        async private Task DownloadFile(Uri fileUri, string outputPath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Inicia o download
                    using (var response = await client.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode(); // Lança uma exceção se o status não for sucesso

                        // Abertura do fluxo para escrever os dados no arquivo
                        using (var inputStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                            {
                                // Copiar o conteúdo do fluxo de entrada para o fluxo de saída
                                await inputStream.CopyToAsync(outputStream);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Erro relacionado à requisição HTTP
                MessageBox.Show($"Erro de rede: {httpEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioEx)
            {
                // Erro relacionado ao fluxo de dados ou ao arquivo
                MessageBox.Show($"Erro ao gravar o arquivo: {ioEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Captura de outros erros gerais
                MessageBox.Show($"Erro durante o download: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CombineVideoAndAudio(string videoPath, string audioPath, string outputPath)
        {
            // Usando FFmpeg para combinar o vídeo e o áudio
            string ffmpegPath = @"C:\path\to\ffmpeg.exe"; // Substitua pelo caminho correto do FFmpeg no seu sistema
            string arguments = $"-i \"{videoPath}\" -i \"{audioPath}\" -c:v copy -c:a aac -strict experimental \"{outputPath}\"";

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            process.WaitForExit();
        }


        //private void bgWorkerGetVideo_DoWork(object sender, DoWorkEventArgs e)
        //{
        //      if (InvokeRequired)
        //    {
        //        Invoke(new MethodInvoker(() => bgWorkerGetVideo_DoWork(sender, e)));
        //        return;
        //    }

        //    // Evita erro se o txtUrl não existir ainda
        //    if (txtUrl == null || string.IsNullOrEmpty(txtUrl.Text))
        //    {
        //        return; // Se não há URL, não há o que processar
        //    }

        //    videoId.Clear();

        //    // Atualizar Status na UI thread
        //    if (this.InvokeRequired)
        //    {
        //        this.Invoke(new MethodInvoker(() =>
        //        {
        //            Status.ForeColor = System.Drawing.Color.Black;
        //            Status.Text = "Processing link...";
        //        }));
        //    }
        //    else
        //    {
        //        Status.ForeColor = System.Drawing.Color.Black;
        //        Status.Text = "Processing link...";
        //    }

        //    try
        //    {
        //        Match m = listreg.Match(txtUrl.Text);
        //        if (m.Success)
        //        {
        //            string listId = m.Groups[1].Value;
        //            using (var client = new WebClient())
        //            {
        //                var responseString = client.DownloadString(txtUrl.Text);
        //                string re2 = "\"videoId\":\"([A-z0-9-]+)\",\"playlistId\":\"" + listId + "\"";
        //                Regex listOfVideos = new Regex(re2);
        //                foreach (Match ma in listOfVideos.Matches(responseString))
        //                {
        //                    videoId.Add(ma.Groups[1].Value);
        //                }
        //                if (videoId.Count > 0)
        //                    GetVideoData(watchLink + videoId.ElementAt(0), true);
        //            }
        //        }
        //        else
        //        {
        //            GetVideoData(txtUrl.Text);
        //        }
        //    }
        //    catch
        //    {
        //        EmptyUrl();

        //        // Atualizar Status na UI thread em caso de erro
        //        if (this.InvokeRequired)
        //        {
        //            this.Invoke(new MethodInvoker(() =>
        //            {
        //                Status.ForeColor = System.Drawing.Color.Red;
        //                Status.Text = "Invalid Link";
        //            }));
        //        }
        //    }
        //}
        private void bgWorkerGetVideo_DoWork(object sender, DoWorkEventArgs e)
        {
            AtualizarStatus("Processing link...", System.Drawing.Color.Black);

            if (txtUrl == null || string.IsNullOrEmpty(txtUrl.Text))
                return;

            videoId.Clear();

            try
            {
                Match m = listreg.Match(txtUrl.Text);
                if (m.Success)
                {
                    string listId = m.Groups[1].Value;
                    using (var client = new WebClient())
                    {
                        var responseString = client.DownloadString(txtUrl.Text);
                        string re2 = $"\"videoId\":\"([A-Za-z0-9-]+)\",\"playlistId\":\"{listId}\"";
                        Regex listOfVideos = new Regex(re2);

                        foreach (Match ma in listOfVideos.Matches(responseString))
                        {
                            videoId.Add(ma.Groups[1].Value);
                        }

                        if (videoId.Count > 0)
                            GetVideoData(watchLink + videoId.ElementAt(0), true);
                    }
                }
                else
                {
                    GetVideoData(txtUrl.Text); // Vídeo único
                }
            }
            catch
            {
                EmptyUrl();
                AtualizarStatus("Invalid Link", System.Drawing.Color.Red);
            }
        }

        private void AtualizarStatus(string mensagem, System.Drawing.Color cor)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new MethodInvoker(() =>
                {
                    Status.ForeColor = cor;
                    Status.Text = mensagem;
                }));
            }
            else
            {
                Status.ForeColor = cor;
                Status.Text = mensagem;
            }
        }


        private void btnLimparLista_Click(object sender, EventArgs e)
        {
            ListBoxURL.Items.Clear();
            mediaPaths.Clear();        
            txtTitle.Text = "";
            cmbQuality.Items.Clear();
            cmbAudioQuality.Items.Clear();
            txtUrl.Focus();
        }

        private void txtUrl_Click(object sender, EventArgs e)
        {
            txtUrl.BackColor = System.Drawing.Color.YellowGreen;
        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            txtUrl.BackColor = System.Drawing.Color.White;
        }

        private void txtUrl_Enter(object sender, EventArgs e)
        {
            txtUrl.BackColor = System.Drawing.Color.YellowGreen;
        }
    }
}
