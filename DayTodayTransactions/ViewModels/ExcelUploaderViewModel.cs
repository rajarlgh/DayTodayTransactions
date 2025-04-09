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
                var transactions = 
                    ReadCsv(SelectedFilePath).Result;

                // Update the ObservableCollection
                Transactions.Clear();
                
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                    await _transactionService.AddTransactionAsync(transaction);

                }

                //await App.Current.MainPage.DisplayAlert("Success", "File uploaded and transactions loaded!", "OK");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to upload file: {ex.Message}");
            }
        }

        private async Task<List<Transaction>> ReadCsv(string filePath)
        {
            var transactions = new List<Transaction>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true // Set to false if the CSV has no headers
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    if (csv.GetField(0) != "date")
                    {
                        var accountAndCategoryField = csv.GetField(2); //Category
                        var account = await CreateAccount(csv.GetField(1));//Account
                        var transferredAccountTask = await DeriveAccountAsync(accountAndCategoryField);
                        var amount = decimal.Parse(csv.GetField(3) ?? "0");
                        // Update the Initital Amount & Currency for an account
                        if (account != null)
                        {
                            UpdateAccountAsync(account, accountAndCategoryField, amount);
                        }

                       // if (!(accountAndCategoryField.Contains("From") || accountAndCategoryField.Contains("To")))
                        {

                            var category = await DeriveCategory(accountAndCategoryField);

                            var transferredAccount = transferredAccountTask;
                            if (category == null)
                            {
                                //if (transferredAccount != null )
                                //    category = new Category() { Id = transferredAccount.Id, Name = transferredAccount.Name };
                            }
                           
                            var transaction = new Transaction
                            {
                                FromAccountId = account.Id,
                                ToAccountId = transferredAccountTask == null? null: transferredAccountTask.Id,
                                Category = category,
                                CategoryId = category == null ? null: category.Id,

                                Date = DateTime.ParseExact(csv.GetField(0), "d/M/yyyy", CultureInfo.InvariantCulture),
                                
                                
                                Amount = amount,
                                Type = (amount > 0) ? "Income": "Expense",
                                

                                Reason = csv.GetField(7)
                            };

                            transactions.Add(transaction);
                        }
                    }
                }
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
        private async Task<Account> DeriveAccountAsync(string? accountField)
        {
            Account account = null;

            if (accountField.Contains("To '"))
            {
                var field = accountField.Split("To '");
                if (field.Length > 0)
                {
                    var accField = field[1].Replace("'", "");
                    account = await _accountService.AddAccountAsync(new Account() { Name = accField });
                }
            }
            else if (accountField.Contains("From '"))
            {
                var field = accountField.Split("From '");
                if (field.Length > 0)
                {
                    var accField = field[1].Replace("'", "");
                    account = await _accountService.AddAccountAsync(new Account() { Name = accField });
                }
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
