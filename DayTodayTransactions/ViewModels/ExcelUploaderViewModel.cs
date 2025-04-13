using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Models;
using ExcelDataReader;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Globalization;
#if ANDROID
using Android.Content;
using Android.Net;
#endif
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
//using Android.Hardware.Usb;

using CsvHelper;
using CsvHelper.Configuration;
using DayTodayTransactionsLibrary.Interfaces;
using System.Security.Principal;

namespace DayTodayTransactions.ViewModels
{
    public partial class ExcelUploaderViewModel : ObservableObject
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;

        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool canUpload;

        public ObservableCollection<Transaction> Transactions { get; } = new();

        public ExcelUploaderViewModel(ITransactionService transactionService, IAccountService accountService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            CanUpload = false; // Upload button is disabled initially
            _categoryService = categoryService;
        }


        public void ExportDatabase()
    {
#if ANDROID
            // Source path for the database in app-specific storage
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");

        // Destination path in the public "Downloads" folder
        var destinationPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "expenses.db");

        // Copy the file to the destination
        File.Copy(sourcePath, destinationPath, true);
#endif
    }




        [RelayCommand]
        private async Task BrowseAsync()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
         { DevicePlatform.Android, new[] { "*/*" } } // ✅ Allows all file types
    }),
                PickerTitle = "Select an Excel or CSV File"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                SelectedFilePath = result.FullPath;
                CanUpload = true;
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

        [ObservableProperty]
        private bool isUploading;

        [ObservableProperty]
        private double uploadProgress;

        [ObservableProperty]
        private string uploadStatus;


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

                IsUploading = true;
                UploadStatus = "Reading file...";
                UploadProgress = 0;

                var transactions = await Task.Run(() => ReadCsv(SelectedFilePath));

                int total = transactions.Count;
                int batchSize = 100; // Set batch size to process records in smaller groups
                int batchCount = (total + batchSize - 1) / batchSize; // Calculate the number of batches
                int index = 0;

                // Process transactions in batches
                for (int i = 0; i < batchCount; i++)
                {
                    var batch = transactions.Skip(i * batchSize).Take(batchSize).ToList();
                    var batchTasks = new List<Task>();

                    foreach (var transaction in batch)
                    {
                        batchTasks.Add(_transactionService.AddTransactionAsync(transaction));
                    }

                    // Await all tasks in the current batch concurrently
                    await Task.WhenAll(batchTasks);

                    // Update progress after each batch
                    index += batch.Count;
                    UploadProgress = (double)index / total;
                    UploadStatus = $"Uploading... {index} of {total}";
                }

                UploadStatus = "Upload completed!";
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to upload file: {ex.Message}");
            }
            finally
            {
                await Task.Delay(1000);
                IsUploading = false;
            }
        }



        private async Task<List<Transaction>> ReadCsv(string filePath)
        {
            var transactions = new List<Transaction>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            var existingAccounts = (await _accountService.GetAccountsAsync())
                .ToDictionary(a => a.Name, a => a);

            var existingCategories = (await _categoryService.GetCategoriesAsync())
                .ToDictionary(c => c.Name, c => c);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            while (csv.Read())
            {
                if (csv.GetField(0) == "date")
                    continue;

                var date = DateTime.ParseExact(csv.GetField(0), "d/M/yyyy", CultureInfo.InvariantCulture);
                var accField = csv.GetField(1);
                var accCatField = csv.GetField(2);
                var amount = decimal.Parse(csv.GetField(3) ?? "0");
                var reason = csv.GetField(7);

                // Get or add account
                if (!existingAccounts.TryGetValue(accField, out var account))
                {
                    account = await _accountService.AddAccountAsync(new Account { Name = accField });
                    existingAccounts[accField] = account;
                }

                // Get or derive transferred account
                var transferredAccount = await DeriveAccountAsync(accCatField, existingAccounts);

                // Update account balance if needed
                if (accCatField.Contains("Initial balance '"))
                {
                    account.InititalAccBalance = amount;
                    account.InitialAccDate = DateTime.Now;
                    await _accountService.UpdateAccountAsync(account);
                }

                // Derive category
                Category category = null;
                if (!accCatField.Contains("From ") && !accCatField.Contains("To ") && !accCatField.Contains("Initial balance '"))
                {
                    if (!existingCategories.TryGetValue(accCatField, out category))
                    {
                        var categoryType = string.Empty;
                        if (amount > 0)
                            categoryType = "Income";
                        else
                            categoryType = "Expense";

                        category = await _categoryService.AddCategoryAsync(new Category { Name = accCatField, Type=categoryType});
                        existingCategories[accCatField] = category;
                    }
                }

                var transaction = new Transaction
                {
                    FromAccountId = account?.Id??0,
                    ToAccountId = transferredAccount?.Id,
                    Category = category,
                    CategoryId = category?.Id,
                    Date = date,
                    Amount = amount,
                    Type = amount > 0 ? "Income" : "Expense",
                    Reason = reason
                };

                transactions.Add(transaction);
            }

            return transactions;
        }

        private async Task<Category> DeriveCategory(string accountAndCategoryField)
        {
            Category cat = null;
            if (!accountAndCategoryField.Contains("From ") && !accountAndCategoryField.Contains("To ") && !accountAndCategoryField.Contains("Initial balance '"))
            {
                var category = accountAndCategoryField;
                if (category.Length > 0)    
                {
                    cat  = await _categoryService.AddCategoryAsync(new Category() { Name = category });
                }
            }
            return cat;
        }
        private async Task<Account> CreateAccount(string? accountField)
        {
            Account account = null;
            {
                var accField = accountField;
                if (accField.Length > 0)
                {
                    account = await _accountService.AddAccountAsync(new Account() { Name = accField });
                }
            }
            return account;
        }
        private async Task<Account> DeriveAccountAsync(string accountField, Dictionary<string, Account> accountCache)
        {
            string accName = null;

            if (accountField.Contains("To '"))
                accName = accountField.Split("To '").ElementAtOrDefault(1)?.Replace("'", "");
            else if (accountField.Contains("From '"))
                accName = accountField.Split("From '").ElementAtOrDefault(1)?.Replace("'", "");

            if (string.IsNullOrWhiteSpace(accName))
                return null;

            if (!accountCache.TryGetValue(accName, out var account))
            {
                account = await _accountService.AddAccountAsync(new Account { Name = accName });
                accountCache[accName] = account;
            }

            return account;
        }

        private async Task<Account> UpdateAccountAsync(Account account, string? accountField, decimal amount )
        {
            if (accountField.Contains("Initial balance '"))
            {
                var field = accountField.Split("Initial balance '");
                if (field.Length > 0)
                {
                    account.InititalAccBalance = amount;
                    account.InitialAccDate = DateTime.Now;
                    await _accountService.UpdateAccountAsync(account);
                }
            }
            return account;
        }



        private async Task ShowErrorAsync(string message)
        {
            await App.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
}
