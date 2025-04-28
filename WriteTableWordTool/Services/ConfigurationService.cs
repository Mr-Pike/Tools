using System.Configuration;
using System.Reflection;

namespace WriteTableWordTool.Services
{
    public static class ConfigurationService
    {
        private const string WORD_FILE_PATH_DEFAULT = "WordFilePathDefault";
        private const string SCRIPT_DIRECTORY_PATH_DEFAULT = "ScriptsDirectoryPathDefault";
        private const string TABLE_NAMES_WORD_DEFAULT = "TableNamesWordDefault";
        private const string START_ROW_TABLE_WORD_DEFAULT = "StartRowTableWordDefault";
        private const string SUB_DIRECTORY_ORDER_DEFAULT = "SubDirectoryOrderDefault";
        private const string FILE_FILTERS_DEFAULT = "FileFiltersDefault";


        public static string? WordFilePathDefault => ConfigurationManager.AppSettings[WORD_FILE_PATH_DEFAULT];

        public static string? ScriptDirectoryPathDefault => ConfigurationManager.AppSettings[SCRIPT_DIRECTORY_PATH_DEFAULT];

        public static string? TablesNameWordDefault => ConfigurationManager.AppSettings[TABLE_NAMES_WORD_DEFAULT];

        public static string? StartRowTableWordDefault => ConfigurationManager.AppSettings[START_ROW_TABLE_WORD_DEFAULT];

        public static string? SubDirectoryOrderDefault => ConfigurationManager.AppSettings[SUB_DIRECTORY_ORDER_DEFAULT];

        public static string? FileFiltersDefault => ConfigurationManager.AppSettings[FILE_FILTERS_DEFAULT];

        public static string? Version => $"Version {Assembly.GetExecutingAssembly().GetName().Version}";

        public static bool IsSameAsWindow(string wordFilePath, string scriptDirectoryPath, string tableNamesWord, string startRowTableWord, string subDirectoryOrder, string fileFilters)
        {
            return wordFilePath == WordFilePathDefault
                && scriptDirectoryPath == ScriptDirectoryPathDefault
                && tableNamesWord == TablesNameWordDefault
                && startRowTableWord == StartRowTableWordDefault
                && subDirectoryOrder == SubDirectoryOrderDefault
                && fileFilters == FileFiltersDefault;
        }

        public static void Save(string wordFilePath, string scriptsDirectoryPath, string tableNamesWord, string startRowTableWord, string subDirectoryOrder, string filefilters)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateAppSettings(config, WORD_FILE_PATH_DEFAULT, wordFilePath);
            UpdateAppSettings(config, SCRIPT_DIRECTORY_PATH_DEFAULT, scriptsDirectoryPath);
            UpdateAppSettings(config, TABLE_NAMES_WORD_DEFAULT, tableNamesWord);
            UpdateAppSettings(config, START_ROW_TABLE_WORD_DEFAULT, startRowTableWord);
            UpdateAppSettings(config, SUB_DIRECTORY_ORDER_DEFAULT, subDirectoryOrder);
            UpdateAppSettings(config, FILE_FILTERS_DEFAULT, filefilters);
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
