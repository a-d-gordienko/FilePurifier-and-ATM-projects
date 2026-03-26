using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilePurifier.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilePurifier.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Параметры обработки
    [ObservableProperty] private bool _removeWords = true;
    [ObservableProperty] private int _minWordLength = 3;
    [ObservableProperty] private bool _removePunctuation = true;
    [ObservableProperty] private bool _isProcessing;

    [RelayCommand]
    private async Task ClearFiles()
    {
        SelectedFiles.Clear();
        StartCommand.NotifyCanExecuteChanged();
        await Task.CompletedTask;
    }

    // Список выбранных файлов и их статусы
    public ObservableCollection<FileItemViewModel> SelectedFiles { get; } = new();

    public IAsyncRelayCommand SelectFilesCommand { get; }
    public IAsyncRelayCommand StartCommand { get; }

    public MainViewModel()
    {
        SelectFilesCommand = new AsyncRelayCommand(OnSelectFiles);
        StartCommand = new AsyncRelayCommand(OnStartProcessing, () => !IsProcessing && SelectedFiles.Count > 0);
    }

    private async Task OnSelectFiles()
    {
        // Логика выбора файлов через StorageProvider (вызывается из View или через сервис)
        // Для примера предположим, что пути передаются из View
    }

    private async Task OnStartProcessing()
    {
        IsProcessing = true;
        var service = new FilePurifierService(RemoveWords, MinWordLength, RemovePunctuation);

        var paths = SelectedFiles.Select(f => f.Path).ToList();

        // Используем IProgress для обновления статуса конкретных файлов
        var progress = new Progress<string>(path =>
        {
            var file = SelectedFiles.FirstOrDefault(f => f.Path == path);
            if (file != null) file.Status = "Готово";
        });

        try
        {
            await service.ProcessFilesAsync(paths, progress);
        }
        finally
        {
            IsProcessing = false;
        }
    }


    partial void OnMinWordLengthChanged(int value)
    {
        if (value < 2) MinWordLength = 2;
    }
}

public partial class FileItemViewModel : ObservableObject
{
    public required string Path { get; init; }
    public string Name => System.IO.Path.GetFileName(Path);

    [ObservableProperty] private string _status = "Ожидание";
}

