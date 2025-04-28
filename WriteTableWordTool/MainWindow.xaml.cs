using Microsoft.Win32;
using System.IO;
using System.Windows;
using WriteTableWordTool.Services;

namespace WriteTableWordTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMainWindow();
        }

        private void InitializeMainWindow()
        {
            // Version.
            VersionLabel.Content = ConfigurationService.Version;

            // Paths.
            WordFilePathTextBox.Text = ConfigurationService.WordFilePathDefault;
            ScriptsDirectoryPathTextBox.Text = ConfigurationService.ScriptDirectoryPathDefault;

            // Parameters.
            TableNamesTextBox.Text = ConfigurationService.TablesNameWordDefault;
            StartRowTableWordTextBox.Text = ConfigurationService.StartRowTableWordDefault;
            SubdirectoryOrderTextBox.Text = ConfigurationService.SubDirectoryOrderDefault;
            FileFiltersTextBox.Text = ConfigurationService.FileFiltersDefault;
        }

        private void StartProcessingButton_Click(object sender, RoutedEventArgs e)
        {
            // Word file does not exist.
            if (!File.Exists(WordFilePathTextBox.Text))
            {
                MessageBox.Show("Le fichier Word n'existe pas.", "Fichier non trouvé", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Scrits directory does not exist.
            if (!Directory.Exists(ScriptsDirectoryPathTextBox.Text))
            {
                MessageBox.Show("Le répertoire des scripts n'existe pas.", "Répertoire inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!int.TryParse(StartRowTableWordTextBox.Text, out int startRowTableWord))
            {
                MessageBox.Show("La ligne de début de tableau doit être un entier", "Ligne de début de tableau doit être un entier", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                var wordFileProcessingService = new WordFileProcessingService(
                    wordFileInfo: new FileInfo(WordFilePathTextBox.Text),
                    scriptsDirectoryInfo: new DirectoryInfo(ScriptsDirectoryPathTextBox.Text),
                    tableNamesWord: TableNamesTextBox.Text.Split(';'),
                    startRowTableWord: startRowTableWord,
                    subDirectoryOrder: SubdirectoryOrderTextBox.Text.Split(';'),
                    fileFilters: FileFiltersTextBox.Text
                );

                wordFileProcessingService.ProcessWordFile();
                MessageBox.Show($"Le traitement des fichiers a été effectué avec succès.", "Traitement avec succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors du traitement du fichier :{Environment.NewLine}{ex.Message}", "Erreur de traitement du fichier", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseWordFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Documents word|*.docx|Documents Word 97-2003|*.doc"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                WordFilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseScriptsDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Multiselect = false
            };

            if (folderDialog.ShowDialog() == true)
            {
                ScriptsDirectoryPathTextBox.Text = folderDialog.FolderName;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If the parameters are the same as the config, don't suggest this option.
            if (ConfigurationService.IsSameAsWindow(WordFilePathTextBox.Text, ScriptsDirectoryPathTextBox.Text, TableNamesTextBox.Text, StartRowTableWordTextBox.Text, SubdirectoryOrderTextBox.Text, FileFiltersTextBox.Text))
            {
                return;
            }

            var messageBoxResult = MessageBox.Show("Voulez-vous sauvegarder les valeurs de configuration avant de quitter ?", "Sauvegarder et quitter", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (messageBoxResult == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    ConfigurationService.Save(WordFilePathTextBox.Text, ScriptsDirectoryPathTextBox.Text, TableNamesTextBox.Text, StartRowTableWordTextBox.Text, SubdirectoryOrderTextBox.Text, FileFiltersTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la sauvegarde du fichier de configuration :{Environment.NewLine}{ex.Message}", "Erreur de sauvegarde", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}