using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FilePurifier.UI.ViewModels;
using System.Linq;

namespace FilePurifier.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Привязываем ViewModel (если не сделано через XAML)
        DataContext = new MainViewModel();
    }

    // Метод для вызова из кнопки "Выбрать файлы" (через событие Click)
    public async void OnSelectFilesClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите текстовые файлы",
            AllowMultiple = true,
            FileTypeFilter = new[] { FilePickerFileTypes.TextPlain }
        });

        if (files.Count > 0 && DataContext is MainViewModel vm)
        {
            foreach (var file in files)
            {
                // Проверяем, нет ли уже такого файла в списке
                if (!vm.SelectedFiles.Any(f => f.Path == file.Path.LocalPath))
                {
                    vm.SelectedFiles.Add(new FileItemViewModel { Path = file.Path.LocalPath });
                }
            }
            // Обновляем состояние кнопки StartCommand
            vm.StartCommand.NotifyCanExecuteChanged();
        }
    }

}
