using Microsoft.Office.Interop.Word;
using System.IO;
using System.Runtime.InteropServices;

namespace WriteTableWordTool.Services
{
    public class WordFileProcessingService(FileInfo wordFileInfo, DirectoryInfo scriptsDirectoryInfo, string[] tableNamesWord, int startRowTableWord, string[] subDirectoryOrder, string fileFilters)
    {
        private readonly FileInfo _wordFileInfo = wordFileInfo;
        private readonly DirectoryInfo _scriptsDirectoryInfo = scriptsDirectoryInfo; 
        private readonly string[] _tableNamesWord = tableNamesWord;
        private readonly int _startRowTableWord = startRowTableWord;
        private readonly string[] _subDirectoryOrder = subDirectoryOrder;
        private readonly string _fileFilters = fileFilters;

        public void ProcessWordFile()
        {
            Application? app = null;
            Document? doc = null;

            try
            {
                // Initialization.
                app = new Application { Visible = false };

                doc = app.Documents.Open(_wordFileInfo.FullName);

                var scriptsFileInfo = _scriptsDirectoryInfo.GetFiles(_fileFilters, SearchOption.AllDirectories);

                foreach (var tableName in _tableNamesWord.Select((name, index) => (name, index)))
                {
                    var scriptNumber = 0;

                    // Try to get table by the name in the Word document.
                    var table = GetTableByName(doc.Tables, tableName.name);
                    if (table == null)
                    {
                        continue;
                    }

                    // Clear table.
                    ClearTableRowsExceptNFirstRows(table);

                    for (var scriptDirectoryIndex = 0; scriptDirectoryIndex < _subDirectoryOrder.Length; scriptDirectoryIndex++)
                    {
                        var scriptsFilteredFileInfo = scriptsFileInfo.Where(x => x.FullName.Contains(@$"{_subDirectoryOrder[scriptDirectoryIndex]}\{tableName.name}"));
                        foreach (var scriptFileInfo in scriptsFilteredFileInfo)
                        {
                            table.Rows.Add();

                            var scriptNumberString = $"0{scriptNumber + 1}";
                            table.Cell(_startRowTableWord + scriptNumber, 1).Range.Text = $"{tableName.index}{scriptNumberString[^2..]}";
                            table.Cell(_startRowTableWord + scriptNumber, 2).Range.Text = scriptFileInfo.FullName.Replace(_scriptsDirectoryInfo.FullName, "");
                            table.Cell(_startRowTableWord + scriptNumber, 3).Range.Text = scriptDirectoryIndex == 0 ? "Script" : "Package";
                            table.Cell(_startRowTableWord + scriptNumber, 4).Range.Text = "2 min";

                            scriptNumber++;
                        }
                    }
                }

                // Save doc.
                doc.Save();
            }
            finally
            {
                // Release ressources.
                if (doc != null)
                {
                    doc.Close();
                    Marshal.ReleaseComObject(doc);
                }

                if (app != null)
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
            }
        }

        private void ClearTableRowsExceptNFirstRows(Table table)
        {
            for (int rowIndex = table.Rows.Count; rowIndex > _startRowTableWord; rowIndex--)
            {
                table.Rows[rowIndex].Delete();
            }
        }

        private static Table? GetTableByName(Tables tables, string name)
        {
            foreach (Table? table in tables)
            {
                // Try to get table by name.
                if (table?.Title?.Trim() == name)
                {
                    return table;
                }

                // Try to get table by name in subtables.
                if (table?.Tables != null)
                {
                    Table? tableFound = GetTableByName(table.Tables, name);
                    if (tableFound != null)
                    {
                        return tableFound;
                    }
                }
            }

            return null;
        }
    }
}
