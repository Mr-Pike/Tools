using System.IO;

namespace WriteFileTool.Services
{
    public class FileProcessingService(DirectoryInfo directoryInfo, string contentStart, string contentEnd)
    {
        private readonly DirectoryInfo _directoryInfo = directoryInfo;
        private readonly string _contentStart = contentStart;
        private readonly string _contentEnd = contentEnd;

        public async Task ProcessFilesAsync()
        {
            // Create list of tasks: one task by file.
            var tasks = new List<Task>();
            foreach (var fileInfo in _directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                tasks.Add(ProcessFileAsync(fileInfo));
            }

            // Create task who end when all of tasks are end.
            await Task.WhenAll(tasks);
        }

        private async Task ProcessFileAsync(FileInfo fileInfo)
        {
            var separatorIndex = fileInfo.Name.IndexOf(ConfigurationService.NameFileCharSeparator?.ToString() ?? string.Empty);

            // Separator not found: ignore the file.
            if (separatorIndex == -1) return;

            var number = fileInfo.Name[..separatorIndex];

            // Temporary file.
            var tempFilePath = $"{fileInfo.FullName}.tmp";

            using (var reader = new StreamReader(fileInfo.FullName))
            {
                var contentOriginal = await reader.ReadToEndAsync();
                var contentStart = _contentStart.Replace("{number}", number);

                // Don't modify file if contains content start and content end.
                if (contentOriginal.Contains(contentStart) && contentOriginal.Contains(_contentEnd))
                {
                    return;
                }

                using var writer = new StreamWriter(tempFilePath, false);

                if (!contentOriginal.Contains(contentStart))
                {
                    await writer.WriteLineAsync(contentStart);
                    await writer.WriteLineAsync();
                }

                await writer.WriteLineAsync(contentOriginal);

                if (!contentOriginal.Contains(_contentEnd))
                {
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync(_contentEnd);
                }
            }

            // Replace the original file.
            File.Delete(fileInfo.FullName);
            File.Move(tempFilePath, fileInfo.FullName);
        }
    }
}
