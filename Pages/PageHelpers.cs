using ConverPro.Models;
using ConverPro.ViewModels;

namespace ConverPro.Pages;

internal static class PageHelpers
{
    public static async Task PickFilesAsync(MainViewModel vm, System.Collections.ObjectModel.ObservableCollection<MediaQueueItem> target, params string[] extensions)
    {
        var types = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { [DevicePlatform.WinUI] = extensions });
        var files = await FilePicker.Default.PickMultipleAsync(new PickOptions { FileTypes = types });
        if (files is not null) vm.AddFiles(files.Select(file => file?.FullPath).OfType<string>(), target);
    }

    public static async Task<string?> PickOutputAsync()
    {
#if WINDOWS
        var picker = new Windows.Storage.Pickers.FolderPicker(); picker.FileTypeFilter.Add("*");
        if (Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView is not Microsoft.UI.Xaml.Window window) return null;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window));
        return (await picker.PickSingleFolderAsync())?.Path;
#else
        await Task.CompletedTask; return null;
#endif
    }
}
