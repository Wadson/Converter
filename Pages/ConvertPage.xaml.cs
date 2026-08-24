using ConverPro.ViewModels;
namespace ConverPro.Pages;
public partial class ConvertPage : ContentPage
{
 private readonly MainViewModel vm; public ConvertPage(MainViewModel vm) { InitializeComponent(); BindingContext = this.vm = vm; }
 private async void Pick(object? s, EventArgs e) { try { await PageHelpers.PickFilesAsync(vm, vm.Conversions, ".mp4", ".mkv", ".avi", ".mov", ".webm"); } catch (Exception ex) { await DisplayAlertAsync("Seleção de vídeos", ex.Message, "OK"); } }
 private void Remove(object? s, EventArgs e) => vm.RemoveSelected(vm.Conversions);
 private async void Output(object? s, EventArgs e) { var path = await PageHelpers.PickOutputAsync(); if (path is not null) vm.OutputDirectory = path; }
 private async void Start(object? s, EventArgs e) { if (await vm.StartConversionsAsync()) await DisplayAlertAsync("Conversão concluída", vm.Status, "OK"); } private void Stop(object? s, EventArgs e) => vm.Stop();
}
