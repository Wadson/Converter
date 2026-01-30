using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using FontAwesome.Sharp;


namespace Converter
{
    public partial class FrmDowload : Form
    {
        private bool isDownloading = false;
        private bool isPaused = false; // <--- nova flag

        private Process currentProcess = null;
        private string selectedVideoQuality = "1080";
        private string selectedAudioQuality = "128";
        private readonly string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
        private readonly string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
        private CancellationTokenSource cancellationTokenSource;

        // Lista interna expandida de vídeos
        private readonly BindingSource expandedList = new BindingSource();

        // Pasta base de downloads (usa o campo txtFilePath atual; fallback para Desktop)
        private string BaseDownloadFolder =>
            !string.IsNullOrWhiteSpace(txtFilePath?.Text)
                ? txtFilePath.Text
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "YouTube Downloads");


        public FrmDowload()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            txtUrl.Text = "Enter video or playlist link";
            txtUrl.GotFocus += (s, e) => { if (txtUrl.Text.Contains("Enter")) txtUrl.Clear(); };
            txtUrl.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtUrl.Text)) txtUrl.Text = "Enter video or playlist link"; };

            cmbVideoQuality.Items.AddRange(new object[] { "4320p (8K)", "2160p (4K)", "1440p", "1080p", "720p", "480p", "360p", "Melhor disponível" });
            cmbVideoQuality.SelectedIndex = 3;

            cmbAudioQuality.Items.AddRange(new object[] { "320", "256", "192", "160", "128", "96" });
            cmbAudioQuality.SelectedIndex = 4;

            txtFilePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "YouTube Downloads");
            if (!Directory.Exists(txtFilePath.Text))
                Directory.CreateDirectory(txtFilePath.Text);

            ListBoxURL.DataSource = expandedList;

            AtualizarTotalLinks();
            AtualizarBotoes();

            // ====== VALIDAÇÃO MAIS ROBUSTA DOS EXECUTÁVEIS ======
            if (!File.Exists(ytDlpPath))
            {
                MessageBox.Show("yt-dlp.exe não encontrado!\nColoque o arquivo 'yt-dlp.exe' na mesma pasta do programa.\n\nBaixar: https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe",
                    "Aviso Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show("ffmpeg.exe não encontrado!\nColoque o arquivo 'ffmpeg.exe' na mesma pasta do programa.\n\nBaixar: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip",
                    "Aviso Crítico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // ====== FIM DA VALIDAÇÃO ======

            btnExcluirSelecionados.Enabled = false;
            AtualizarBotoes();
        }

        private void AtualizarTotalLinks()
        {
            lblTotalLinks.Text = $"Total: {expandedList.Count}";
        }

        private void AtualizarBotoes()
        {
            bool temItens = expandedList.Count > 0;

            // controles básicos
            btnAdicionarURL.Enabled = !isDownloading;
            btnDownload.Enabled = temItens && !isDownloading;
            btnCancelar.Enabled = isDownloading;

            // excluir só quando houver itens E quando NÃO estiver baixando
            btnExcluirSelecionados.Enabled = temItens && !isDownloading;

            // pausa/continua dependem de isDownloading/isPaused
            if (!isDownloading)
            {
                btnPausar.Enabled = false;
                btnContinuar.Enabled = false;
            }
            else
            {
                // está baixando - usar isPaused para alternar
                if (!isPaused)
                {
                    btnPausar.Enabled = true;
                    btnContinuar.Enabled = false;
                }
                else
                {
                    btnPausar.Enabled = false;
                    btnContinuar.Enabled = true;
                }
            }
        }


        private bool IsPlaylistLink(string url)
        {
            return url.Contains("list=") && Regex.IsMatch(url, @"list=([a-zA-Z0-9_-]+)");
        }

        private async Task<(bool IsValid, bool IsPlaylist, string Title, int Count)> GetInfoAsync(string url)
        {
            if (!File.Exists(ytDlpPath))
                return (false, false, "", 0);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"--dump-single-json --flat-playlist --no-warnings \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8
                };

                using var p = Process.Start(psi);
                string jsonStr = await p.StandardOutput.ReadToEndAsync();
                await p.WaitForExitAsync();

                if (p.ExitCode != 0 || string.IsNullOrWhiteSpace(jsonStr))
                {
                    return await GetBasicInfoAsync(url);
                }

                try
                {
                    var json = JsonNode.Parse(jsonStr) as JsonObject;

                    bool ehPlaylist = json?["_type"]?.ToString() == "playlist" ||
                                     json?["playlist_id"]?.ToString() is string id && !string.IsNullOrEmpty(id);

                    int count = json?["playlist_count"]?.GetValue<int>() ??
                               json?["n_entries"]?.GetValue<int>() ?? 1;

                    string tituloPlaylist = json?["playlist_title"]?.ToString() ?? "";
                    string tituloVideo = json?["title"]?.ToString() ?? "Vídeo";

                    string tituloFinal = ehPlaylist && !string.IsNullOrEmpty(tituloPlaylist) ? tituloPlaylist : tituloVideo;

                    return (true, ehPlaylist, tituloFinal, count);
                }
                catch
                {
                    return (true, false, "Vídeo válido", 1);
                }
            }
            catch
            {
                return (false, false, "", 0);
            }
        }

        private async Task<(bool IsValid, bool IsPlaylist, string Title, int Count)> GetBasicInfoAsync(string url)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"--print \"%(title)s\" --print \"%(playlist_title)s\" --print \"%(playlist_count)s\" --no-warnings \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8
                };

                using var p = Process.Start(psi);
                string output = await p.StandardOutput.ReadToEndAsync();
                await p.WaitForExitAsync();

                if (p.ExitCode != 0) return (false, false, "", 0);

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string title = lines.Length > 0 ? lines[0] : "Vídeo";
                string playlistTitle = lines.Length > 1 ? lines[1] : "";
                string playlistCountStr = lines.Length > 2 ? lines[2] : "1";

                bool isPlaylist = !string.IsNullOrEmpty(playlistTitle) && playlistTitle != "NA";
                int count = int.TryParse(playlistCountStr, out int c) ? c : 1;

                return (true, isPlaylist, isPlaylist ? playlistTitle : title, count);
            }
            catch
            {
                return (false, false, "", 0);
            }
        }

        // ========================= PEGAR URLs DA PLAYLIST =========================
        private async Task<System.Collections.Generic.List<PlaylistVideo>> GetPlaylistVideosAsync(string url)
        {
            var videos = new System.Collections.Generic.List<PlaylistVideo>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"--dump-single-json --flat-playlist --no-warnings \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8
                };

                using var p = Process.Start(psi);
                string jsonStr = await p.StandardOutput.ReadToEndAsync();
                await p.WaitForExitAsync();

                var json = JsonNode.Parse(jsonStr) as JsonObject;
                if (json == null) return videos;

                string playlistTitle = json?["title"]?.ToString() ?? "Playlist";

                var entries = json?["entries"] as JsonArray;
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        string videoId = entry?["id"]?.ToString();
                        string videoTitle = entry?["title"]?.ToString() ?? videoId;
                        if (!string.IsNullOrEmpty(videoId))
                        {
                            videos.Add(new PlaylistVideo
                            {
                                Url = $"https://www.youtube.com/watch?v={videoId}",
                                Title = videoTitle,
                                PlaylistTitle = playlistTitle
                            });
                        }
                    }
                }
            }
            catch { }

            return videos;
        }

        public class PlaylistVideo
        {
            public string Url { get; set; }
            public string Title { get; set; }
            public string PlaylistTitle { get; set; } // pode ser null para vídeo avulso
            public override string ToString()
            {
                return string.IsNullOrEmpty(PlaylistTitle) ? Title : $"[{PlaylistTitle}] {Title}";
            }
        }


        // ========================= DOWNLOAD =========================


        private async Task DownloadAllAsync(CancellationToken cancellationToken)
        {
            // total geral
            int totalCount = expandedList.Count;

            // construir mapa de totais por playlistKey (use "__NO_PLAYLIST__" para avulsos)
            var playlistTotals = new Dictionary<string, int>();
            for (int i = 0; i < expandedList.Count; i++)
            {
                if (expandedList[i] is PlaylistVideo pv)
                {
                    string key = string.IsNullOrEmpty(pv.PlaylistTitle) ? "__NO_PLAYLIST__" : pv.PlaylistTitle;
                    if (!playlistTotals.ContainsKey(key)) playlistTotals[key] = 0;
                    playlistTotals[key]++;
                }
            }

            // trackers por playlist
            var playlistProgress = playlistTotals.ToDictionary(kvp => kvp.Key, kvp => 0);

            for (int i = 0; i < expandedList.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                var video = expandedList[i] as PlaylistVideo;
                if (video == null) continue;

                // atualiza índices por playlist
                string key = string.IsNullOrEmpty(video.PlaylistTitle) ? "__NO_PLAYLIST__" : video.PlaylistTitle;
                playlistProgress[key]++;

                int currentInPlaylist = playlistProgress[key];
                int totalInPlaylist = playlistTotals.ContainsKey(key) ? playlistTotals[key] : 1;

                ListBoxURL.SelectedIndex = i;

                Invoke((MethodInvoker)(() =>
                {
                    // exibe progresso geral e por playlist no mesmo rótulo
                    lblStatusContagem.Text = $"{i + 1}/{totalCount}";
                    string playlistLabel = key == "__NO_PLAYLIST__" ? "Avulsos" : key;
                    Status.Text = $"📥 Preparando {i + 1}/{totalCount} — [{playlistLabel} {currentInPlaylist}/{totalInPlaylist}]";
                    progressBar.Value = 0;
                }));

                await DownloadVideoAsync(video, i + 1, totalCount, cancellationToken);

                if (cancellationToken.IsCancellationRequested) break;
            }
        }

        private async Task DownloadVideoAsync(
    PlaylistVideo video,
    int current,
    int total,
    CancellationToken cancellationToken)
        {
            string outputDir = txtFilePath.Text;

            // Se for playlist, cria subpasta
            if (!string.IsNullOrWhiteSpace(video.PlaylistTitle))
            {
                outputDir = Path.Combine(outputDir, SanitizeFileName(video.PlaylistTitle));
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);
            }

            // ===== FORMATO ESTÁVEL (SEM HLS / SEM DASH) =====
            string format = chkAudioOnly.Checked
      ? "bestaudio/best"
      : "bv*+ba/best";



            string outputTemplate = Path.Combine(outputDir, "%(title)s.%(ext)s");

            // ===== ARGUMENTOS LIMPOS E SEGUROS =====
            string args =
                $"--newline " +
                $"--output \"{outputTemplate}\" " +
                $"--format \"{format}\" " +
                "--ignore-errors " +
                "--no-warnings " +
                "--retries 10 " +
                "--fragment-retries 10 " +
                "--concurrent-fragments 3 " +
                "--user-agent \"Mozilla/5.0\" " +
                "--referer \"https://www.youtube.com/\" " +
             (chkAudioOnly.Checked
    ? $"--extract-audio --audio-format mp3 --audio-quality {selectedAudioQuality}k "
    :"--merge-output-format mp4 --embed-thumbnail --add-metadata "

+ $"\"{video.Url}\"");



            // ffmpeg
            if (File.Exists(ffmpegPath))
            {
                string ffmpegDir = Path.GetDirectoryName(ffmpegPath);
                args += $" --ffmpeg-location \"{ffmpegDir}\"";
            }

            Invoke((MethodInvoker)(() =>
            {
                Status.Text = $"📥 Baixando {current}/{total}";
                Status.BackColor = Color.RoyalBlue;
                progressBar.Value = 0;
                lblProgress.Text = "0%";
            }));

            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi };
            currentProcess = process;

            process.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data)) return;

                string line = e.Data.Trim();
                Console.WriteLine(line);

                Invoke((MethodInvoker)(() =>
                {
                    if (line.Contains("[download]") && line.Contains("%"))
                    {
                        var m = Regex.Match(line, @"(\d+(\.\d+)?)%");
                        if (m.Success)
                        {
                            int percent = (int)float.Parse(m.Groups[1].Value);
                            progressBar.Value = Math.Min(percent, 100);
                            lblProgress.Text = percent + "%";
                            Status.Text = $"📥 {current}/{total} - {percent}%";
                        }
                    }
                    else if (line.Contains("Merging formats"))
                    {
                        Status.Text = "🔗 Mesclando áudio e vídeo...";
                        Status.BackColor = Color.DarkBlue;
                    }
                    else if (line.Contains("Extracting audio"))
                    {
                        Status.Text = "🎵 Convertendo áudio...";
                        Status.BackColor = Color.Purple;
                    }
                }));
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    Console.WriteLine("[ERR] " + e.Data);
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync(cancellationToken);

                if (process.ExitCode != 0)
                    throw new Exception("Falha no download. Vídeo pode estar restrito ou indisponível.");

                Invoke((MethodInvoker)(() =>
                {
                    progressBar.Value = 100;
                    lblProgress.Text = "100%";
                    Status.Text = $"✅ {current}/{total} Concluído";
                    Status.BackColor = Color.LimeGreen;
                }));
            }
            finally
            {
                currentProcess = null;
            }

            await Task.Delay(1000, cancellationToken);
        }




        // Método fallback para quando o formato principal falha
        private async Task TrySimpleFallback(PlaylistVideo video, int current, int total, CancellationToken cancellationToken)
        {
            Console.WriteLine("Usando fallback simples...");

            string outputDir = txtFilePath.Text;
            bool isPlaylistItem = !string.IsNullOrEmpty(video.PlaylistTitle);

            if (isPlaylistItem)
            {
                outputDir = Path.Combine(outputDir, SanitizeFileName(video.PlaylistTitle));
            }

            // FORMATO ULTRA SIMPLES - só o básico
            string formato = "best[ext=mp4]/best";
            string outputTemplate = Path.Combine(outputDir, "%(title)s.%(ext)s");

            string args = $"--newline " +
                          $"--output \"{outputTemplate}\" " +
                          $"--format \"{formato}\" " +
                          "--ignore-errors " +
                          "--no-warnings " +
                          "--no-check-certificates " +
                          "--user-agent \"Mozilla/5.0\" " +
                          "--referer \"https://www.youtube.com/\" " +
                          (isPlaylistItem ? "--yes-playlist " : "--no-playlist ") +
                          (chkAudioOnly.Checked
                              ? $"--extract-audio --audio-format mp3 --audio-quality 128k "
                              : "--merge-output-format mp4 ") +
                          $"\"{video.Url}\"";

            if (File.Exists(ffmpegPath))
            {
                string ffmpegDir = Path.GetDirectoryName(ffmpegPath);
                args += $" --ffmpeg-location \"{ffmpegDir}\"";
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                {
                    throw new Exception("Fallback também falhou. O vídeo pode estar restrito.");
                }
            }
        }



        // Método auxiliar para análise de erros
        private string AnalyzeError(int exitCode, string log)
        {
            Console.WriteLine($"Analyzing error: Code={exitCode}, Log length={log.Length}");

            // Códigos de erro do yt-dlp
            switch (exitCode)
            {
                case 1:
                    return "Erro geral do yt-dlp. " + GetErrorDetails(log);
                case 2:
                    return "Processo interrompido pelo usuário ou erro de execução. " + GetErrorDetails(log);
                case 3:
                    return "Erro de autenticação. " + GetErrorDetails(log);
                case 4:
                    return "Erro de rede ou conexão. " + GetErrorDetails(log);
                case 5:
                    return "Erro de configuração. " + GetErrorDetails(log);
                case 6:
                    return "Erro de permissão. " + GetErrorDetails(log);
                case 7:
                    return "Erro de sistema. " + GetErrorDetails(log);
                case 8:
                    return "Erro de pipeline. " + GetErrorDetails(log);
                case 100:
                    return "Vídeo não disponível. " + GetErrorDetails(log);
                case 101:
                    return "Vídeo privado. " + GetErrorDetails(log);
                case 102:
                    return "Vídeo restrito por idade. " + GetErrorDetails(log);
                default:
                    return $"Erro desconhecido (código {exitCode}). " + GetErrorDetails(log);
            }
        }

        private string GetErrorDetails(string log)
        {
            // Extrair mensagens de erro específicas
            string[] lines = log.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains("ERROR:"))
                {
                    return line.Replace("ERROR:", "").Trim();
                }
                else if (line.Contains("unable to download"))
                {
                    return line;
                }
                else if (line.Contains("HTTP Error"))
                {
                    return line;
                }
                else if (line.Contains("Video unavailable"))
                {
                    return "Vídeo indisponível ou removido.";
                }
                else if (line.Contains("Private video"))
                {
                    return "Vídeo privado - requer login.";
                }
                else if (line.Contains("Sign in"))
                {
                    return "Requer autenticação no YouTube.";
                }
            }

            // Se não encontrar erro específico, retorna primeiras linhas
            if (lines.Length > 0)
            {
                return $"Primeira linha: {lines[0]}";
            }

            return "Sem detalhes disponíveis.";
        }

        private string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
        private void CancelarDownload()
        {
            try
            {
                cancellationTokenSource?.Cancel();
            }
            catch { }

            try
            {
                if (currentProcess != null && !currentProcess.HasExited)
                    currentProcess.Kill();
            }
            catch { }
        }
        private async Task<bool> TestYtDlpConnectionAsync()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                await process.WaitForExitAsync();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }


        // ========================= HELPER PARA PAUSA/RESUME =========================
        public static class ProcessHelper
        {
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            private static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            private static extern uint SuspendThread(IntPtr hThread);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            private static extern int ResumeThread(IntPtr hThread);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            private static extern bool CloseHandle(IntPtr handle);

            private const uint THREAD_SUSPEND_RESUME = 0x0002;

            public static void Suspend(int pid)
            {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
            }

            public static void Resume(int pid)
            {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        while (ResumeThread(pOpenThread) > 0) ;
                        CloseHandle(pOpenThread);
                    }
                }
            }
        }
        private void cmbVideoQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVideoQuality.SelectedItem == null) return;

            string item = cmbVideoQuality.SelectedItem.ToString();

            if (item.Contains("Melhor disponível") || item.Contains("Melhor"))
            {
                selectedVideoQuality = "99999"; // Flag para melhor qualidade
                Status.Text = "Qualidade: Melhor disponível (automática)";
            }
            else if (item.Contains("4320p"))
            {
                selectedVideoQuality = "4320";
            }
            else if (item.Contains("2160p"))
            {
                selectedVideoQuality = "2160";
            }
            else if (item.Contains("1440p"))
            {
                selectedVideoQuality = "1440";
            }
            else if (item.Contains("1080p"))
            {
                selectedVideoQuality = "1080";
            }
            else if (item.Contains("720p"))
            {
                selectedVideoQuality = "720";
            }
            else if (item.Contains("480p"))
            {
                selectedVideoQuality = "480";
            }
            else if (item.Contains("360p"))
            {
                selectedVideoQuality = "360";
            }
            else
            {
                // Extrai número da string (ex: "1080p (Full HD)" -> "1080")
                var match = Regex.Match(item, @"(\d+)");
                selectedVideoQuality = match.Success ? match.Groups[1].Value : "1080";
            }

            Status.Text = $"Qualidade selecionada: {item}";
        }

        private void cmbAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAudioQuality = cmbAudioQuality.SelectedItem.ToString();
        }


        private async void btnAdicionarURL_Click(object sender, EventArgs e)
        {
            string input = txtUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(input) || input.Contains("Enter")) return;

            btnAdicionarURL.Enabled = false;
            lblAnalisando.Visible = true;
            lblAnalisando.Text = "Analisando link...";

            try
            {
                // VERIFICAÇÃO SIMPLIFICADA DE PLAYLIST
                bool isPlaylist = input.Contains("list=") || input.Contains("playlist");

                if (isPlaylist)
                {
                    // Extrai ID da playlist
                    var playlistMatch = Regex.Match(input, @"list=([a-zA-Z0-9_-]+)");
                    if (playlistMatch.Success)
                    {
                        string playlistId = playlistMatch.Groups[1].Value;
                        string playlistUrl = $"https://www.youtube.com/playlist?list={playlistId}";

                        var resposta = MessageBox.Show(
                            "📁 PLAYLIST DETECTADA!\n\nDeseja baixar TODA a playlist?",
                            "Playlist Encontrada", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (resposta == DialogResult.Cancel) return;

                        if (resposta == DialogResult.Yes)
                        {
                            // Adiciona URL da playlist como item especial
                            // 🔹 obtém o nome real da playlist
                            var info = await GetInfoAsync(playlistUrl);

                            string playlistTitleReal = info.IsValid && !string.IsNullOrWhiteSpace(info.Title)
                                ? info.Title
                                : playlistId;

                            // 🔹 EXPANDE a playlist em vídeos
                            var videos = await GetPlaylistVideosAsync(playlistUrl);

                            if (videos.Count == 0)
                            {
                                MessageBox.Show("Não foi possível carregar os vídeos da playlist.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // 🔹 adiciona cada vídeo corretamente
                            foreach (var v in videos)
                            {
                                expandedList.Add(new PlaylistVideo
                                {
                                    Url = v.Url,
                                    Title = v.Title,
                                    PlaylistTitle = playlistTitleReal
                                    // cada item é UM vídeo, nunca playlist
                                });

                            }

                            txtTitle.Text = $"📁 Playlist: {playlistTitleReal}";                           
                        }
                        else
                        {
                            // Extrai apenas o vídeo (remove parâmetros de playlist)
                            var videoMatch = Regex.Match(input, @"(?:v=|youtu\.be/)([a-zA-Z0-9_-]{11})");
                            if (videoMatch.Success)
                            {
                                string videoUrl = $"https://www.youtube.com/watch?v={videoMatch.Groups[1].Value}";
                                expandedList.Add(new PlaylistVideo
                                {
                                    Url = videoUrl,
                                    Title = "Vídeo único",
                                    PlaylistTitle = null
                                });

                                txtTitle.Text = $"🎥 Vídeo único";
                            }
                        }
                    }
                }
                else
                {
                    // É vídeo único
                    expandedList.Add(new PlaylistVideo
                    {
                        Url = input,
                        Title = "Vídeo único",
                        PlaylistTitle = null
                    });

                    txtTitle.Text = $"🎥 Vídeo único";
                }

                Status.Text = "✅ Adicionado à lista";
                Status.BackColor = Color.Green;
                AtualizarTotalLinks();
                AtualizarBotoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblAnalisando.Visible = false;
                txtUrl.Clear();
                txtUrl.Focus();
                AtualizarBotoes();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                txtFilePath.Text = fbd.SelectedPath;
        }

        private void btnLimparLista_Click(object sender, EventArgs e)
        {
            // Limpa o conteúdo do BindingSource (lista interna)
            expandedList.Clear();

            // Re-restabelece a ligação do ListBox ao BindingSource (garante que a ListBox mostre o conteúdo atual)
            ListBoxURL.DataSource = null;
            ListBoxURL.DataSource = expandedList;

            // Zera labels/indicadores
            lblStatusContagem.Text = "";
            lblProgress.Text = "";
            txtTitle.Text = "";

            // Atualiza total/estado dos botões
            AtualizarTotalLinks();
            AtualizarBotoes();
            AtualizarTotalLinks();
            AtualizarBotoes();
        }

        private void btnExcluirSelecionados_Click(object sender, EventArgs e)
        {
            if (ListBoxURL.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione ao menos um vídeo para remover.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirma exclusão da lista
            var confirm = MessageBox.Show(
                "Deseja realmente remover os itens selecionados da lista?",
                "Remover Vídeos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // Coletar os itens antes de remover (evita alterar coleção durante iteração)
            var remover = new List<PlaylistVideo>();

            foreach (var item in ListBoxURL.SelectedItems)
            {
                if (item is PlaylistVideo pv)
                    remover.Add(pv);
            }

            // Perguntar se deseja excluir também os arquivos baixados
            bool excluirArquivos = false;

            var respostaArquivos = MessageBox.Show(
                "Deseja também excluir os arquivos já baixados desses vídeos?",
                "Excluir arquivos?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respostaArquivos == DialogResult.Yes)
                excluirArquivos = true;

            // Remover efetivamente
            foreach (var video in remover)
            {
                expandedList.Remove(video);

                if (excluirArquivos)
                {
                    try
                    {
                        string pastaDownload;

                        // Se for playlist, o arquivo está dentro da pasta da playlist
                        if (!string.IsNullOrWhiteSpace(video.PlaylistTitle))
                        {
                            pastaDownload = Path.Combine(BaseDownloadFolder, SanitizeFileName(video.PlaylistTitle));
                        }
                        else
                        {
                            // Caso não seja playlist → raiz principal
                            pastaDownload = BaseDownloadFolder;
                        }

                        string caminhoArquivoMp4 = Path.Combine(pastaDownload, SanitizeFileName(video.Title) + ".mp4");
                        string caminhoArquivoMp3 = Path.Combine(pastaDownload, SanitizeFileName(video.Title) + ".mp3");

                        if (File.Exists(caminhoArquivoMp4))
                            File.Delete(caminhoArquivoMp4);

                        if (File.Exists(caminhoArquivoMp3))
                            File.Delete(caminhoArquivoMp3);
                    }
                    catch
                    {
                        // Não travar o app caso esteja em uso / bloqueado
                    }
                }
            }

            // Recarregar DataSource
            ListBoxURL.DataSource = null;
            ListBoxURL.DataSource = expandedList;

            AtualizarTotalLinks();
            AtualizarBotoes();

            Status.Text = "🗑️ Itens removidos";
            Status.BackColor = Color.DarkOrange;
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if (!isDownloading) return;

            // reseta flag
            isPaused = false;
            AtualizarBotoes();

            if (currentProcess != null && !currentProcess.HasExited)
            {
                ProcessHelper.Resume(currentProcess.Id);
                Status.Text = "▶️ CONTINUANDO";
                Status.BackColor = Color.Orange;
            }
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            if (!isDownloading) return;

            // seta flag
            isPaused = true;
            AtualizarBotoes();

            if (currentProcess != null && !currentProcess.HasExited)
            {
                ProcessHelper.Suspend(currentProcess.Id);
                Status.Text = "⏸️ PAUSADO";
                Status.BackColor = Color.Orange;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CancelarDownload();
            isDownloading = false;
            isPaused = false;
            AtualizarBotoes();

            Status.Text = "⏹️ Cancelado pelo usuário";
            Status.BackColor = Color.Red;
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            // ====== VALIDAÇÃO INICIAL ======
            if (!File.Exists(ytDlpPath))
            {
                MessageBox.Show("yt-dlp.exe não encontrado! Coloque na pasta do programa.",
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Testar yt-dlp
            try
            {
                var testPsi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var testProcess = Process.Start(testPsi);
                string version = await testProcess.StandardOutput.ReadToEndAsync();
                await testProcess.WaitForExitAsync();

                Console.WriteLine($"yt-dlp version: {version}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao testar yt-dlp: {ex.Message}",
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isDownloading || expandedList.Count == 0) return;

            // ====== INICIAR DOWNLOAD ======
            isDownloading = true;
            isPaused = false;
            cancellationTokenSource = new CancellationTokenSource();
            AtualizarBotoes();

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Visible = true;
            progressBar.Value = 0;
            Status.Text = "🚀 Preparando downloads...";
            Status.BackColor = Color.Orange;

            try
            {
                Console.WriteLine("=== STARTING DOWNLOAD BATCH ===");
                Console.WriteLine($"Total videos: {expandedList.Count}");

                await DownloadAllAsync(cancellationTokenSource.Token);

                Status.Text = "✅ TODOS OS DOWNLOADS CONCLUÍDOS!";
                Status.BackColor = Color.LimeGreen;

                System.Media.SystemSounds.Exclamation.Play();
            }
            catch (OperationCanceledException)
            {
                Status.Text = "⏹️ DOWNLOAD CANCELADO";
                Status.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                Status.Text = $"❌ ERRO: {ex.Message.Split('\n')[0]}";
                Status.BackColor = Color.Red;

                MessageBox.Show($"Erro durante o download:\n\n{ex.Message}",
                               "Erro no Download", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                isDownloading = false;
                isPaused = false;
                currentProcess = null;
                AtualizarBotoes();

                Console.WriteLine("=== DOWNLOAD BATCH FINISHED ===");
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    namespace Converter
    {
        // ====== EXTENSÕES PARA PROCESS ======
        public static class ProcessExtensions
        {
            public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
            {
                var tcs = new TaskCompletionSource<bool>();

                process.Exited += (sender, args) => tcs.TrySetResult(true);
                process.EnableRaisingEvents = true;

                if (process.HasExited)
                {
                    return;
                }

                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    await tcs.Task;
                }
            }
        }
        // ====== FIM DAS EXTENSÕES ======

        public partial class FrmDowload : Form
        {
            // ... resto do código da classe FrmDowload
        }
    }
}
