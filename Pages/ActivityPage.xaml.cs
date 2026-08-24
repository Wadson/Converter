using ConverPro.ViewModels;
namespace ConverPro.Pages;
public partial class ActivityPage : ContentPage
{
 private readonly MainViewModel vm; public ActivityPage(MainViewModel vm) { InitializeComponent(); BindingContext = this.vm = vm; }
 private void Retry(object? s, EventArgs e) => vm.RetryFailures();
 private async void Clear(object? s, EventArgs e) { if (vm.Logs.Count > 0 && await DisplayAlertAsync("Limpar histórico", "Deseja apagar todas as atividades desta sessão?", "Limpar", "Cancelar")) vm.ClearLogs(); }
}
