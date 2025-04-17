using System.IO;

namespace WriteFileTool.Services
{
    public class FileProcessingService(DirectoryInfo directoryInfo, string textFileStart, string textFileEnd, string nameFileStringSeparator, string nameFileSearchPattern)
    {
        private readonly DirectoryInfo _directoryInfo = directoryInfo;
        private readonly string _textFileStart = textFileStart;
        private readonly string _textFileEnd = textFileEnd;
        private readonly string _nameFileStringSeparator = nameFileStringSeparator;
        private readonly string _nameFileSearchPattern = string.IsNullOrEmpty(nameFileSearchPattern) ? "*.*" : nameFileSearchPattern;

        public async Task ProcessFilesAsync()
        {
            // Create list of tasks: one task by file.
            var tasks = new List<Task>();
            foreach (var fileInfo in _directoryInfo.GetFiles(_nameFileSearchPattern, SearchOption.AllDirectories))
            {
                tasks.Add(ProcessFileAsync(fileInfo));
            }

            // Create task who end when all of tasks are end.
            await Task.WhenAll(tasks);
        }

        private async Task ProcessFileAsync(FileInfo fileInfo)
        {
            // Try to get the number before file string separator.
            var number = string.Empty;
            if (!string.IsNullOrEmpty(_nameFileStringSeparator))
            {
                var separatorIndex = fileInfo.Name.IndexOf(_nameFileStringSeparator);

                // Separator not found: ignore the file.
                if (separatorIndex == -1) return;

                number = fileInfo.Name[..separatorIndex];
            }

            // Temporary file.
            var tempFilePath = $"{fileInfo.FullName}.tmp";

            using (var reader = new StreamReader(fileInfo.FullName))
            {
                var originalContent = await reader.ReadToEndAsync();
                var textFileStart = _textFileStart.Replace("{number}", number);

                // Don't modify file if contains content start and content end.
                if (originalContent.Contains(textFileStart) && originalContent.Contains(_textFileEnd))
                {
                    return;
                }

                using var writer = new StreamWriter(tempFilePath, false);

                if (!originalContent.Contains(textFileStart))
                {
                    await writer.WriteLineAsync(textFileStart);
                    await writer.WriteLineAsync();
                }

                await writer.WriteLineAsync(originalContent);

                if (!originalContent.Contains(_textFileEnd))
                {
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync(_textFileEnd);
                }
            }

            // Replace the original file.
            File.Delete(fileInfo.FullName);
            File.Move(tempFilePath, fileInfo.FullName);
        }
    }
}
