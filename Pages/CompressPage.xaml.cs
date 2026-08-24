using ConverPro.ViewModels;
namespace ConverPro.Pages;
public partial class CompressPage : ContentPage
{
 private readonly MainViewModel vm; public CompressPage(MainViewModel vm) { InitializeComponent(); BindingContext = this.vm = vm; }
 private async void Pick(object? s, EventArgs e) { try { await PageHelpers.PickFilesAsync(vm, vm.Compressions, ".mp3"); } catch (Exception ex) { await DisplayAlertAsync("Seleção de MP3", ex.Message, "OK"); } }
 private void Remove(object? s, EventArgs e) => vm.RemoveSelected(vm.Compressions);
 private async void Output(object? s, EventArgs e) { var path = await PageHelpers.PickOutputAsync(); if (path is not null) vm.OutputDirectory = path; }
 private async void Start(object? s, EventArgs e) { if (await vm.StartCompressionsAsync()) await DisplayAlertAsync("Compressão concluída", vm.Status, "OK"); } private void Stop(object? s, EventArgs e) => vm.Stop();
}
