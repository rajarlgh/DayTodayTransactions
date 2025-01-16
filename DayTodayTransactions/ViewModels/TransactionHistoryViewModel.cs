using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
#if ANDROID
using AndroidOS = Android.OS;
#endif
using DayTodayTransactions.Pages;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using Microcharts;
using SkiaSharp;
using SQLite;
using System.Collections.ObjectModel;

namespace DayTodayTransactions.ViewModels
{
    public partial class TransactionHistoryViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ChartEntry> incomeChartEntries;
        [ObservableProperty]
        private ObservableCollection<ChartEntry> expenseChartEntries;
        [ObservableProperty]
        private ObservableCollection<Transaction> transactions;
        private ObservableCollection<Transaction> allTransactions;
        [ObservableProperty]
        private DonutChart incomeChart;
        [ObservableProperty]
        private DonutChart expenseChart;
        [ObservableProperty]
        private bool doVisibleChart  = false;
        [ObservableProperty]
        private bool doVisibleChartBreakUp = true;
        [ObservableProperty]
        private ObservableCollection<Transaction> selectedCategoryBreakdown;

        private readonly SQLiteAsyncConnection _database;
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;

        public TransactionHistoryViewModel(string dbPath, ITransactionService transactionService, IAccountService accountService)
        {
            this._accountService = accountService;

            _database = new SQLiteAsyncConnection(dbPath);
            //var transactions = transactionService.GetTransactionsAsync().Result;
            //var t = transactions.ToList<Transaction>();
            //  var t= transactionService.GetTransactionsAsync();
            //LoadTransactionsAndSetGrid(transactions);
            //Task.Run(async () => await LoadAccountsAsync(0));
            
            //RefreshDataAsync();
        }

        // Mark the property as observable
        [ObservableProperty]
        private Account selectedAccount;

        // Mark the property as observable
        [ObservableProperty]
        private ObservableCollection<Account> listOfAccounts;

        partial void OnSelectedAccountChanged(Account value)
        {
            if (value != null && value.Id != -1) /*All - Selected in Account dropdown.*/
            {
              var  trans = allTransactions.Where(r => r.AccountId == value.Id).ToList();
              Transactions = new ObservableCollection<Transaction>(trans);
              //IncomeChart = null;
              //ExpenseChart = null;
              //OnPropertyChanged(nameof(IncomeChart));
              //OnPropertyChanged(nameof(ExpenseChart));
            }
            else
            {
                Transactions = new ObservableCollection<Transaction>(allTransactions);
            }
            LoadTransactionsAndSetGrid(Transactions);
            CalculateBalances();
            if (value != null && value.Name == "Add New Account")
            {
                Shell.Current.GoToAsync(nameof(ManageAccountsPage));
            }
            SelectedCategoryBreakdown = null;
            //this.RefreshDataAsync();
        }

        public async Task LoadAccountsAsync(int accountId)
        {
            var accounts = await _accountService.GetAccountsAsync();
            listOfAccounts = new ObservableCollection<Account>(accounts);
            listOfAccounts.Add(new Account { Id = -1, Name = "All" });
            listOfAccounts.Add(new Account { Id = -2, Name = "Add New Account" });
            OnPropertyChanged(nameof(ListOfAccounts));
            var selectedAccount = accounts.FirstOrDefault(r => r.Id == accountId);
            if (selectedAccount != null)
            {
                SelectedAccount = listOfAccounts.FirstOrDefault(a => a.Id == selectedAccount.Id);
            }
            else
                SelectedAccount = listOfAccounts.FirstOrDefault(a => a.Id == -1);
            OnPropertyChanged(nameof(SelectedAccount));
           
        }


        public void ShowBreakdownForCategory(Category category, string type)
        {
            var breakdown = transactions.Where(t => t.Type == type && t.Category.Name  ==  category.Name)
                .ToList();

            SelectedCategoryBreakdown = new ObservableCollection<Transaction>(breakdown);
        }

        private DonutChart CreateChart(ObservableCollection<ChartEntry> entries)
        {
            return new DonutChart
            {
                Entries = entries,
                LabelMode = LabelMode.LeftAndRight,
                IsAnimated = true,
                Margin = 10,
                LabelTextSize = 40
            };
        }

        
        private void LoadTransactionsAndSetGrid(IList<Transaction> transactions)
        {
            // Group transactions by category and calculate the total amount for each category
            var groupedData = transactions
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                });

            var incomeGroupedData = transactions.Where(r=> r.Type=="Income" && r.Category != null)
               .GroupBy(t => t.Category.Name)
               .Select(g => new
               {
                   Category = g.Key,
                   TotalAmount = g.Sum(t => t.Amount)
               });

            if (incomeGroupedData!= null)
                // Map the grouped data to ChartEntry objects
                IncomeChartEntries = new ObservableCollection<ChartEntry>(
                    incomeGroupedData.Select(data => new ChartEntry((float)data.TotalAmount)
                    {
                        Label = data.Category,
                        ValueLabel = data.TotalAmount.ToString("F0"), // Format as integer
                        Color = GetCategoryColor(data.Category) // Assign a color based on the category
                    })
                );
            OnPropertyChanged(nameof(IncomeChartEntries));


            var expenseGroupedData = transactions.Where(r => r.Type == "Expense")
              .GroupBy(t => t.Category.Name)
              .Select(g => new
              {
                  Category = g.Key,
                  TotalAmount = g.Sum(t => t.Amount)
              });
            // Map the grouped data to ChartEntry objects
            ExpenseChartEntries = new ObservableCollection<ChartEntry>(
                expenseGroupedData.Select(data => new ChartEntry((float)data.TotalAmount)
                {
                    Label = data.Category,
                    ValueLabel = data.TotalAmount.ToString("F0"), // Format as integer
                    Color = GetCategoryColor(data.Category) // Assign a color based on the category
                })
            );
            OnPropertyChanged(nameof(ExpenseChartEntries));

            //LoadTransactions();

            // Recreate the chart instance with new entries
            //IncomeChart = new DonutChart
            //{
            //    Entries = IncomeChartEntries,
            //    LabelMode = LabelMode.LeftAndRight,
            //    IsAnimated = true,
            //    Margin = 10,
            //    LabelTextSize = 40
            //};

            // Replace chart creation calls:
            // Calculate chart entries...
            IncomeChart = CreateChart(IncomeChartEntries);
            ExpenseChart = CreateChart(ExpenseChartEntries);


            // Notify property changes
            //OnPropertyChanged(nameof(IncomeChart));
            //OnPropertyChanged(nameof(ExpenseChart));

            // Recreate the chart instance with new entries
            //ExpenseChart = new DonutChart
            //{
            //    Entries = ExpenseChartEntries,
            //    LabelMode = LabelMode.LeftAndRight,
            //    IsAnimated = true,
            //    Margin = 10,
            //    LabelTextSize = 40
            //};

        }

        public async Task RefreshDataAsync()
        {
            // Fetch the latest transactions
            var transactionsList = await _database.Table<Transaction>().ToListAsync();
            Transactions = new ObservableCollection<Transaction>(transactionsList);
            allTransactions = this.Transactions;

            //foreach (var transaction in newTransactions)
            //{
            //    if (Transactions == null)
            //        Transactions = new ObservableCollection<Transaction>();
            //    Transactions.Add(transaction);
            //}
            LoadTransactionsAndSetGrid(Transactions);
            CalculateBalances();
            await LoadAccountsAsync(0);
            // Notify UI of changes
            OnPropertyChanged(nameof(Transactions));
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Balance));
            /*OnPropertyChanged(nameof(IncomeChart))*/;
            OnPropertyChanged(nameof(IncomeChartEntries));
            //OnPropertyChanged(nameof(ExpenseChart));
            OnPropertyChanged(nameof(ExpenseChartEntries));
            OnPropertyChanged(nameof(SelectedAccount));
        }

        /// <summary>
        /// Assigns a unique color to each category.
        /// </summary>
        private readonly Dictionary<string, SKColor> _categoryColors = new();
        private readonly List<SKColor> _availableColors = new()
        {
            SKColor.Parse("#00FF00"), // Green
            SKColor.Parse("#FF5733"), // Orange
            SKColor.Parse("#3498DB"), // Blue
            SKColor.Parse("#9B59B6"), // Purple
            SKColor.Parse("#1ABC9C"), // Teal
            SKColor.Parse("#F1C40F"), // Yellow
            SKColor.Parse("#E74C3C"), // Red
            SKColor.Parse("#34495E"), // Dark Gray
            SKColor.Parse("#2ECC71"), // Light Green
            SKColor.Parse("#E67E22"), // Light Orange
            SKColor.Parse("#16A085"), // Dark Teal
            SKColor.Parse("#8E44AD"), // Deep Purple
            SKColor.Parse("#BDC3C7")  // Light Gray
        };
        private int _colorIndex = 0;

        /// <summary>
        /// Assigns a unique color to each category dynamically.
        /// </summary>
        private SKColor GetCategoryColor(string category)
        {
            if (!_categoryColors.ContainsKey(category))
            {
                // Assign the next available color
                var color = _availableColors[_colorIndex];
                _categoryColors[category] = color;

                // Update the color index, wrap around if necessary
                _colorIndex = (_colorIndex + 1) % _availableColors.Count;
            }

            return _categoryColors[category];
        }

        [RelayCommand]
        public void OnSearchClickeda()
        {

        }
        [RelayCommand]
        public void OnMoneyTransferClicked()
        {

        }
        [RelayCommand]
        public async void onAddMoneyClicked()
        {
            // Pass parameters as query parameters in the URL
            await Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}?type=Income");

        }
        [RelayCommand]
        public async void onWidthDrawMoneyClicked()
        {
            await Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}?type=Expense");
        }
        [RelayCommand]
        public async void onUploadPdfClicked()
        {
            await Shell.Current.GoToAsync($"{nameof(ExcelUploaderPage)}?type=Expense");
        }

        public void ExportDatabase()
        {
            // Source path for the database in app-specific storage
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
#if ANDROID
            // Destination path in the public "Downloads" folder
            var destinationPath = Path.Combine(AndroidOS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "expenses.db");

            // Copy the file to the destination
            File.Copy(sourcePath, destinationPath, true);
#endif
        }

        [RelayCommand]
        public async void onDownloadClicked()
        {

            this.ExportDatabase();
            /*
             * 
             * public void ExportDatabase()
                    {
                        // Source path for the database in app-specific storage
                        var sourcePath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");

                        // Destination path in app-specific external storage
                        var destinationPath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "expenses.db");

                        // Copy the file to the destination
                        File.Copy(sourcePath, destinationPath, true);
                    }


                    /storage/emulated/0/Android/data/com.companyname.daytodaytransactions/files/expenses.db

                    adb pull /storage/emulated/0/Download/expenses.db D:\T1\DTTdb


                    C:\Users\z0043c3n>adb pull /storage/emulated/0/Download/expenses.db D:\T1\DTTdb

             * */
        }


        public string FilterDate { get; set; }
        public string FilterCategory { get; set; }
        public string FilterType { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }


        [RelayCommand]
        public void ShowContent()
        {
            doVisibleChart = !this.doVisibleChart;

            doVisibleChartBreakUp = !this.doVisibleChartBreakUp;
            OnPropertyChanged(nameof(DoVisibleChartBreakUp));
            OnPropertyChanged(nameof(DoVisibleChart));

        }

        [RelayCommand]
        public void ShowMenu()
        {
            IncomeChartEntries = new ObservableCollection<ChartEntry>();
            ExpenseChartEntries = new ObservableCollection<ChartEntry>();

            IncomeChart = new DonutChart();
            ExpenseChart = new DonutChart();
            OnPropertyChanged(nameof(ExpenseChartEntries));
            OnPropertyChanged(nameof(IncomeChartEntries));

            //OnPropertyChanged(nameof(IncomeChart));
            //OnPropertyChanged(nameof(ExpenseChart));

        }

        [RelayCommand]
        public async Task EditTransactionDetailsAsync(Transaction transaction)
        {
            // Pass the TransactionViewModel to the transaction page
            await Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}?type={transaction.Type}", true, new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }   

        private void CalculateBalances()
        {
            if (Transactions != null)
            {
                TotalIncome = Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
                TotalExpenses = Transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
                Balance = TotalIncome - TotalExpenses;
                OnPropertyChanged(nameof(TotalIncome));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(Balance));
            }
        }



        // Filter transactions by the given criteria
        public void FilterTransactions()
        {
            var filteredTransactions = _database.Table<Transaction>();

            if (!string.IsNullOrEmpty(FilterDate))
            {
                var date = DateTime.Parse(FilterDate);
                filteredTransactions = filteredTransactions.Where(t => t.Date.Date == date.Date);
            }

            if (!string.IsNullOrEmpty(FilterCategory))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Category.Name ==(FilterCategory));
            }

            if (!string.IsNullOrEmpty(FilterType))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Type.Equals(FilterType, StringComparison.OrdinalIgnoreCase));
            }

            // Execute the query and update the Transactions list
            Transactions = new ObservableCollection<Transaction>(filteredTransactions.ToListAsync().Result);

            CalculateBalances();
            OnPropertyChanged(nameof(Transactions));
        }

        [ObservableProperty]
        string? scrollMessage = "Swipe up for more";

        public void UpdateScrollMessage(double scrollY, double contentHeight, double scrollViewHeight)
        {
            if (scrollY <= 0)
            {
                ScrollMessage = "Swipe down for more";
            }
            else if (scrollY >= contentHeight - scrollViewHeight)
            {
                ScrollMessage = "No more data down";
            }
            else
            {
                ScrollMessage = "Swipe up for more";
            }
        }
    }
}
