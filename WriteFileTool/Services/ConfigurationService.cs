using System.Configuration;

namespace WriteFileTool.Services
{
    public static class ConfigurationService
    {
        private const string SELECTED_DIRECTORY_DEFAULT = "SelectedDirectoryDefault";
        private const string CONTENT_START_FILE_DEFAULT = "ContentStartFileDefault";
        private const string CONTENT_END_FILE_DEFAULT = "ContentEndFileDefault";
        private const string NAME_FILE_CHAR_SEPARATOR = "NameFileCharSeparator";

        public static string? SelectedDirectoryDefault => ConfigurationManager.AppSettings[SELECTED_DIRECTORY_DEFAULT];

        public static string? ContentStartFileDefault => ConfigurationManager.AppSettings[CONTENT_START_FILE_DEFAULT];

        public static string? ContentEndFileDefault => ConfigurationManager.AppSettings[CONTENT_END_FILE_DEFAULT];

        public static string? NameFileCharSeparator => ConfigurationManager.AppSettings[NAME_FILE_CHAR_SEPARATOR];

        public static void Save(string selectedDirectory, string contentStartFile, string contentEndFile)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateAppSettings(config, SELECTED_DIRECTORY_DEFAULT, selectedDirectory);
            UpdateAppSettings(config, CONTENT_START_FILE_DEFAULT, contentStartFile);
            UpdateAppSettings(config, CONTENT_END_FILE_DEFAULT, contentEndFile);
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
