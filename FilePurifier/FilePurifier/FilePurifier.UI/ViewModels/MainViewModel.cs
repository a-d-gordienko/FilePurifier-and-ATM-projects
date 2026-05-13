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
    private readonly IFilePurifierService _filePurifierService;

    [ObservableProperty]
    private bool _removeWords = true;

    [ObservableProperty]
    private int _minWordLength = 3;

    [ObservableProperty]
    private bool _removePunctuation = true;

    [ObservableProperty]
    private bool _isProcessing;

    public ObservableCollection<FileItemViewModel> SelectedFiles { get; } = new();

    public IAsyncRelayCommand SelectFilesCommand { get; }
    public IAsyncRelayCommand StartCommand { get; }

    [RelayCommand]
    private void ClearFiles()
    {
        SelectedFiles.Clear();
        StartCommand.NotifyCanExecuteChanged();
    }

    public MainViewModel() : this(new FilePurifierService())
    {
    }

    public MainViewModel(IFilePurifierService filePurifierService)
    {
        _filePurifierService = filePurifierService;

        SelectFilesCommand = new AsyncRelayCommand(OnSelectFiles);
        StartCommand = new AsyncRelayCommand(
            OnStartProcessing,
            () => !IsProcessing && SelectedFiles.Count > 0);
    }

    private async Task OnSelectFiles()
    {
    }

    private async Task OnStartProcessing()
    {
        IsProcessing = true;

        var options = new TextCleanerOptions
        {
            RemoveShortWords = RemoveWords,
            MinWordLength = MinWordLength,
            RemovePunctuation = RemovePunctuation
        };

        var paths = SelectedFiles.Select(f => f.Path).ToList();

        var progress = new Progress<FileProcessingStatus>(status =>
        {
            var file = SelectedFiles.FirstOrDefault(f => f.Path == status.FilePath);
            if (file != null)
            {
                file.Status = status.Result switch
                {
                    FileProcessingResult.Success => "Готово",
                    FileProcessingResult.Skipped => "Пропущено",
                    FileProcessingResult.Failed => $"Ошибка: {status.ErrorMessage}",
                    _ => file.Status
                };
            }
        });

        try
        {
            await _filePurifierService.ProcessFilesAsync(paths, options, progress);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    partial void OnMinWordLengthChanged(int value)
    {
        if (value < 2)
            MinWordLength = 2;
    }
}

public partial class FileItemViewModel : ObservableObject
{
    public required string Path { get; init; }
    public string Name => System.IO.Path.GetFileName(Path);

    [ObservableProperty]
    private string _status = "Ожидание";
}