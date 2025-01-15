using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Models;
using ExcelDataReader;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DayTodayTransactions.ViewModels
{
    public partial class ExcelUploaderViewModel : ObservableObject
    {
        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool canUpload;

        public ObservableCollection<Transaction> Transactions { get; } = new();

        public ExcelUploaderViewModel()
        {
            CanUpload = false; // Upload button is disabled initially
        }


    public void ExportDatabase()
    {
        // Source path for the database in app-specific storage
        var sourcePath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");

        // Destination path in the public "Downloads" folder
        var destinationPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "expenses.db");

        // Copy the file to the destination
        File.Copy(sourcePath, destinationPath, true);
    }


    [RelayCommand]
        private async Task BrowseAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/csv" } }
            }),
                    PickerTitle = "Select an Excel or CSV File"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    // Handle the file (read/write)
                    var fileContents = await ReadFileContentsAsync(stream);
                    SelectedFilePath = result.FullPath;
                    CanUpload = true; // Enable upload button
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to select a file: {ex.Message}");
            }
        }

        private async Task CopyFileToLocalStorageAsync(FileResult fileResult)
        {
            var destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileResult.FileName);

            using var sourceStream = await fileResult.OpenReadAsync();
            using var destinationStream = File.Create(destinationPath);
            await sourceStream.CopyToAsync(destinationStream);

            SelectedFilePath = destinationPath;
        }


        private async Task<string> ReadFileContentsAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            return await reader.ReadToEndAsync();
        }



        [RelayCommand]
        private async Task UploadAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedFilePath))
                {
                    await ShowErrorAsync("No file selected!");
                    return;
                }

                // Read the Excel file
                var transactions = ReadExcel(SelectedFilePath);

                // Update the ObservableCollection
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

                await App.Current.MainPage.DisplayAlert("Success", "File uploaded and transactions loaded!", "OK");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to upload file: {ex.Message}");
            }
        }

        private List<Transaction> ReadExcel(string filePath)
        {
            var transactions = new List<Transaction>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    var dataTable = dataSet.Tables[0];

                    for (int i = 1; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];

                        var category = new Category
                        {
                            Name = row[2]?.ToString()
                        };

                        var transaction = new Transaction
                        {
                            Date = DateTime.ParseExact(row[0]?.ToString() ?? string.Empty, "d/M/yyyy", CultureInfo.InvariantCulture),
                            AccountId = ResolveAccountId(row[1]?.ToString()),
                            Category = category,
                            Amount = decimal.Parse(row[3]?.ToString() ?? "0"),
                            Reason = row[4]?.ToString()
                        };

                        transactions.Add(transaction);
                    }
                }
            }

            return transactions;
        }

        private int ResolveAccountId(string accountName)
        {
            var accountMapping = new Dictionary<string, int>
        {
            { "Cash in Bank", 1 },
            { "Paytm", 2 },
            { "Cash in Hand", 3 }
        };

            return accountMapping.GetValueOrDefault(accountName, 0);
        }

        private async Task ShowErrorAsync(string message)
        {
            await App.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
}
