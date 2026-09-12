using ConverPro.ViewModels;

namespace ConverPro;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel;
    public MainPage(MainViewModel viewModel) { InitializeComponent(); BindingContext = this.viewModel = viewModel; }
    private async void AddUrl(object? sender, EventArgs e)
    {
        var url = UrlEntry.Text?.Trim(); if (string.IsNullOrWhiteSpace(url)) { await Pages.ThemedDialog.AlertAsync(this, "Link obrigatório", "Cole um link válido do YouTube."); return; }
        try { await viewModel.AddYoutubeUrlAsync(url, ChoosePlaylist); UrlEntry.Text = ""; }
        catch (Exception ex) { await Pages.ThemedDialog.AlertAsync(this, "Não foi possível analisar o link", ex.Message); }
    }
    private async Task<bool?> ChoosePlaylist()
    {
        var answer = await Pages.ThemedDialog.ChooseAsync(this, "Playlist detectada", "Este link pertence a uma playlist. Escolha como deseja continuar.", "Cancelar", "Baixar playlist inteira", "Baixar apenas este vídeo");
        return answer == "Baixar playlist inteira" ? true : answer == "Baixar apenas este vídeo" ? false : null;
    }
    private void SelectAll(object? sender, EventArgs e) => viewModel.SelectAll(viewModel.Downloads, true);
    private void ClearSelection(object? sender, EventArgs e) => viewModel.SelectAll(viewModel.Downloads, false);
    private void RemoveSelected(object? sender, EventArgs e) => viewModel.RemoveSelected(viewModel.Downloads);
    private async void PickOutput(object? sender, EventArgs e) { var path = await Pages.PageHelpers.PickOutputAsync(); if (path is not null) viewModel.OutputDirectory = path; }
    private async void Start(object? sender, EventArgs e)
    {
        if (await viewModel.StartDownloadsAsync())
            await Pages.ThemedDialog.AlertAsync(this, "Download concluído", viewModel.Status, "Concluir");
    }
    private void Stop(object? sender, EventArgs e) => viewModel.Stop();
}
