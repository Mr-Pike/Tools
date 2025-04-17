using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using WriteFileTool.Services;

namespace WriteFileTool
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
            VersionLabel.Content = ConfigurationService.Version;
            SelectedDirectoryTextBox.Text = ConfigurationService.SelectedDirectoryDefault;
            TextFileStartTextBox.Text = ConfigurationService.TextFileStartDefault;
            TextFileEndTextBox.Text = ConfigurationService.TextFileEndDefault;
            NameFileStringSeparatorTextBox.Text = ConfigurationService.NameFileStringSeparatorDefault;
            NameFileSearchPatternTextBox.Text = ConfigurationService.NameFileSearchPatternDefault;
        }

        private void SelectedDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Multiselect = false
            };

            if (folderDialog.ShowDialog() == true)
            {
                SelectedDirectoryTextBox.Text = folderDialog.FolderName;
            }
        }

        private async void UpdateContentFilesButton_Click(object sender, RoutedEventArgs e)
        {
            // Directory does not exist.
            if (!Directory.Exists(SelectedDirectoryTextBox.Text))
            {
                MessageBox.Show("Le répertoire n'existe pas.", "Répertoire inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                var fileService = new FileProcessingService(
                    directoryInfo: new DirectoryInfo(SelectedDirectoryTextBox.Text),
                    textFileStart: TextFileStartTextBox.Text,
                    textFileEnd: TextFileEndTextBox.Text,
                    nameFileStringSeparator: NameFileStringSeparatorTextBox.Text,
                    nameFileSearchPattern: NameFileSearchPatternTextBox.Text
                );

                await fileService.ProcessFilesAsync();
                MessageBox.Show($"Le traitement des fichiers a été effectué avec succès.", "Traitement avec succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors du traitement des fichiers :{Environment.NewLine}{ex.Message}", "Erreur de traitement des fichiers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // If the parameters are the same as the config, don't suggest this option.
            if (ConfigurationService.IsSameAsWindow(SelectedDirectoryTextBox.Text, TextFileStartTextBox.Text, TextFileEndTextBox.Text, NameFileStringSeparatorTextBox.Text, NameFileSearchPatternTextBox.Text))
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
                    ConfigurationService.Save(SelectedDirectoryTextBox.Text, TextFileStartTextBox.Text, TextFileEndTextBox.Text, NameFileStringSeparatorTextBox.Text, NameFileSearchPatternTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la sauvegarde du fichier de configuration :{Environment.NewLine}{ex.Message}", "Erreur de sauvegarde", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}