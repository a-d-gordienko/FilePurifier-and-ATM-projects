namespace FilePurifier.Core;

public interface IFilePurifierService
{
    Task ProcessFilesAsync(
        IEnumerable<string> filePaths,
        TextCleanerOptions options,
        IProgress<FileProcessingStatus>? progress = null,
        CancellationToken cancellationToken = default);
}

public record FileProcessingStatus(
    string FilePath,
    FileProcessingResult Result,
    string? ErrorMessage = null);

public enum FileProcessingResult
{
    Success,
    Skipped,
    Failed
}

public class FilePurifierService : IFilePurifierService
{
    public async Task ProcessFilesAsync(
        IEnumerable<string> filePaths,
        TextCleanerOptions options,
        IProgress<FileProcessingStatus>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = filePaths.Select(path => ProcessSingleFileAsync(path, options, progress, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task ProcessSingleFileAsync(
        string path,
        TextCleanerOptions options,
        IProgress<FileProcessingStatus>? progress,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Run(() =>
            {
                var service = new TextCleanerService(options);
                service.ProcessFile(path);
            }, cancellationToken);

            progress?.Report(new FileProcessingStatus(path, FileProcessingResult.Success));
        }
        catch (OperationCanceledException)
        {
            progress?.Report(new FileProcessingStatus(path, FileProcessingResult.Skipped));
        }
        catch (Exception ex)
        {
            progress?.Report(new FileProcessingStatus(path, FileProcessingResult.Failed, ex.Message));
        }
    }
}