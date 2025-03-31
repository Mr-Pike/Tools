using System.Configuration;
using System.Windows.Navigation;

namespace WriteFileTool.Services
{
    public static class ConfigurationService
    {
        private const string SELECTED_DIRECTORY_DEFAULT = "SelectedDirectoryDefault";
        private const string TEXT_FILE_START_DEFAULT = "TextFileStartDefault";
        private const string TEXT_FILE_END_DEFAULT = "TextFileEndDefault";
        private const string NAME_FILE_CHAR_SEPARATOR = "NameFileCharSeparator";

        public static string? SelectedDirectoryDefault => ConfigurationManager.AppSettings[SELECTED_DIRECTORY_DEFAULT];

        public static string? TextFileStartDefault => ConfigurationManager.AppSettings[TEXT_FILE_START_DEFAULT];

        public static string? TextFileEndDefault => ConfigurationManager.AppSettings[TEXT_FILE_END_DEFAULT];

        public static string? NameFileCharSeparator => ConfigurationManager.AppSettings[NAME_FILE_CHAR_SEPARATOR];

        public static bool IsSameAsWindow(string selectedDirectory, string textFileStart, string textFileEnd)
        {
            return SelectedDirectoryDefault == selectedDirectory && TextFileStartDefault == textFileStart && TextFileEndDefault == textFileEnd;
        }

        public static void Save(string selectedDirectory, string textFileStart, string textFileEnd)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateAppSettings(config, SELECTED_DIRECTORY_DEFAULT, selectedDirectory);
            UpdateAppSettings(config, TEXT_FILE_START_DEFAULT, textFileStart);
            UpdateAppSettings(config, TEXT_FILE_END_DEFAULT, textFileEnd);
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
