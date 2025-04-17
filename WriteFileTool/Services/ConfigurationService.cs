using System.Configuration;
using System.Reflection;

namespace WriteFileTool.Services
{
    public static class ConfigurationService
    {
        private const string SELECTED_DIRECTORY_DEFAULT = "SelectedDirectoryDefault";
        private const string TEXT_FILE_START_DEFAULT = "TextFileStartDefault";
        private const string TEXT_FILE_END_DEFAULT = "TextFileEndDefault";
        private const string NAME_FILE_STRING_SEPARATOR_DEFAULT = "NameFileStringSeparatorDefault";
        private const string NAME_FILE_SEARCH_PATTERN_DEFAULT = "NameFileSearchPatternDefault";

        public static string? SelectedDirectoryDefault => ConfigurationManager.AppSettings[SELECTED_DIRECTORY_DEFAULT];

        public static string? TextFileStartDefault => ConfigurationManager.AppSettings[TEXT_FILE_START_DEFAULT];

        public static string? TextFileEndDefault => ConfigurationManager.AppSettings[TEXT_FILE_END_DEFAULT];

        public static string? NameFileStringSeparatorDefault => ConfigurationManager.AppSettings[NAME_FILE_STRING_SEPARATOR_DEFAULT];

        public static string? NameFileSearchPatternDefault => ConfigurationManager.AppSettings[NAME_FILE_SEARCH_PATTERN_DEFAULT];

        public static string? Version => $"Version {Assembly.GetExecutingAssembly().GetName().Version}";

        public static bool IsSameAsWindow(string selectedDirectory, string textFileStart, string textFileEnd, string nameFileStringSeparator, string nameFileSearchPattern)
        {
            return SelectedDirectoryDefault == selectedDirectory
                && TextFileStartDefault == textFileStart
                && TextFileEndDefault == textFileEnd
                && NameFileStringSeparatorDefault == nameFileStringSeparator
                && NameFileSearchPatternDefault == nameFileSearchPattern;
        }

        public static void Save(string selectedDirectory, string textFileStart, string textFileEnd, string nameFileStringSeparator, string nameFileSearchPattern)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateAppSettings(config, SELECTED_DIRECTORY_DEFAULT, selectedDirectory);
            UpdateAppSettings(config, TEXT_FILE_START_DEFAULT, textFileStart);
            UpdateAppSettings(config, TEXT_FILE_END_DEFAULT, textFileEnd);
            UpdateAppSettings(config, NAME_FILE_STRING_SEPARATOR_DEFAULT, nameFileStringSeparator);
            UpdateAppSettings(config, NAME_FILE_SEARCH_PATTERN_DEFAULT, nameFileSearchPattern);
            config.Save(ConfigurationSaveMode.Modified);
        }

        private static void UpdateAppSettings(Configuration configuration, string key, string value)
        {
            // Add the key if not exists.
            if (configuration.AppSettings.Settings[key] == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
                return;
            }

            // Update the key.
            configuration.AppSettings.Settings[key].Value = value;
        }
    }
}
