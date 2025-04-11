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
using DayTodayTransactions.Extensions;

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

        [ObservableProperty]
        private bool isFilterExpanded = false;

        [ObservableProperty]
        private bool isIncomeExpanded = false;
        
        [ObservableProperty]
        private bool isExpensesExpanded = false;


        [RelayCommand]
        private void ToggleFilter()
        {
            IsFilterExpanded = !IsFilterExpanded;
        }

        [RelayCommand]
        private void ToggleIncome()
        {
            IsIncomeExpanded = !IsIncomeExpanded;
        }
        [RelayCommand]
        private void ToggleExpenses()
        {
            IsExpensesExpanded = !IsExpensesExpanded;
        }
        partial void OnSelectedFilterOptionChanged(string value)
        {
            switch (value)
            {
                case "Day":
                    FilterByDay();
                    break;
                case "Week":
                    FilterByWeek();
                    break;
                case "Month":
                    FilterByMonth();
                    break;
                case "Year":
                    FilterByYear();
                    break;
                //case "Interval":
                //    FilterByIntervalCommand.Execute(null);
                //    break;
                //case "Choose Date":
                //    ChooseDateCommand.Execute(null);
                //    break;
            }
        }


        private readonly SQLiteAsyncConnection _database;
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;


        public TransactionHistoryViewModel(string dbPath, ITransactionService transactionService, IAccountService accountService, ICategoryService categoryService)
        {
            //SelectedFilterOption = FilterOptions[0]; // Default to "Day"

            this._accountService = accountService;
            this._categoryService = categoryService;
            this._categoryService.GetCategoriesAsync();
            _database = new SQLiteAsyncConnection(dbPath);
            var transactions = transactionService.GetTransactionsAsync().Result;
            //var t1 = transactions.ToList<Transaction>();
            //var t2 = transactionService.GetTransactionsAsync();
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
            var breakdown = transactions.Where(t => t.Category != null && t.Type == type && t.Category.Name  ==  category.Name)
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
                .Where(t => t.Category != null)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                });

            var incomeGroupedData = transactions.Where(r=> r.Category != null && r.Type=="Income")
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


            var expenseGroupedData = transactions.Where(r => r.Type == "Expense" && r.Category != null)
              .GroupBy(t => t.Category.Name)
              .Select(g => new
              {
                  Category = g.Key,
                  TotalAmount = g.Sum(t => t.Amount)
              });
              //.Where(r => r !=null &&  r.Category != null);
            try
            {
                // Map the grouped data to ChartEntry objects
                ExpenseChartEntries = new ObservableCollection<ChartEntry>(
                    expenseGroupedData.Select(
                        data => 
                        new ChartEntry((float)data.TotalAmount)
                    {
                        Label = data.Category,
                        ValueLabel = data.TotalAmount.ToString("F0"), // Format as integer
                        Color = GetCategoryColor(data.Category) // Assign a color based on the category
                    })
                );
            }
            catch (Exception e)
            {

            }
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
            //LoadTransactionsAndSetGrid(Transactions);
            CalculateBalances();
            await LoadAccountsAsync(0);
            this.notifyChanges();

        }

        private void notifyChanges()
        {
            // Notify UI of changes
            OnPropertyChanged(nameof(Transactions));
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Balance));
            /*OnPropertyChanged(nameof(IncomeChart))*/
            ;
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

            if (File.Exists(sourcePath))
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

        [ObservableProperty]
        private ObservableCollection<string> filterOptions = new()
        {
            "Day",
            "Week",
            "Month",
            "Year",
            "Interval",
            "Choose Date"
        };

        [ObservableProperty]
        private string selectedFilterOption = String.Empty;

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

        [RelayCommand]
        public void FilterByDay()
        {
            FilterDate = DateTime.Now.ToString("yyyy-MM-dd"); // current day
            FilterTransactions();
        }

        [RelayCommand]
        public void FilterByWeek()
        {
            // Get the start of the week (e.g., Monday)
            var startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7); // 7 days later

            FilterDate = $"From {startOfWeek.ToString("yyyy-MM-dd")} to {endOfWeek.ToString("yyyy-MM-dd")}";
            FilterTransactionsByRange(startOfWeek, endOfWeek);
        }


        [RelayCommand]
        public void FilterByMonth()
        {
            // First day of the current month
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            // Last day of the current month
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            FilterDate = $"From {startOfMonth.ToString("yyyy-MM-dd")} to {endOfMonth.ToString("yyyy-MM-dd")}";
            FilterTransactionsByRange(startOfMonth, endOfMonth);
        }


        [RelayCommand]
        public void FilterByYear()
        {
            // First day of the current year
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            // Last day of the current year
            var endOfYear = new DateTime(DateTime.Now.Year, 12, 31);

            FilterDate = $"From {startOfYear.ToString("yyyy-MM-dd")} to {endOfYear.ToString("yyyy-MM-dd")}";
            FilterTransactionsByRange(startOfYear, endOfYear);
        }
        public async Task<Dictionary<string, int>> GetRecordCountsByMonthFromDatabaseAsync()
        {
            var query = @"
        
                SELECT  
                    strftime('%Y-%m-%d', FormattedDate) AS Day,  
                    COUNT(*) AS RecordCount 
                FROM 
                    (SELECT datetime((Date / 10000000) - 62135596800, 'unixepoch') AS FormattedDate, * 
                     FROM [Transaction]) AS T1
                WHERE 
                    T1.FormattedDate >= '2025-01-01 00:00:00'
                GROUP BY 
                    strftime('%Y-%m-%d', FormattedDate)
                ORDER BY 
                    Day;
                ";

            // Use QueryAsync to execute the query
            var result = await _database.QueryAsync<(string Month, int RecordCount)>(query);

            // Convert the result to a dictionary
            return result.ToDictionary(x => x.Month, x => x.RecordCount);
        }


        public void FilterTransactionsByRange(DateTime startDate, DateTime endDate)
        {
            var filteredTransactions = _database.Table<Transaction>();
            var t =  filteredTransactions.ToListAsync().Result;
            // Get record counts grouped by month from the database
            var dbMonthlyCounts = GetRecordCountsByMonthFromDatabaseAsync().Result;

            if (selectedAccount != null && selectedAccount.Id>0)
            filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate && t.AccountId == selectedAccount.Id);
            else
                filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
            var data = filteredTransactions.ToListAsync().Result;
            allTransactions = new ObservableCollection<Transaction>(data);

            // Execute the query and update the Transactions list
            Transactions = new ObservableCollection<Transaction>(data);

            CalculateBalances();
            OnPropertyChanged(nameof(Transactions));
            this.LoadTransactionsAndSetGrid(Transactions);

        }


        [RelayCommand]
        public void FilterByAll()
        {
            FilterDate = null; // No date filter
            FilterTransactions();
        }


        // Filter transactions by the given criteria
        public async void FilterTransactions()
        {
            var filteredTransactions = _database.Table<Transaction>();

            if (!string.IsNullOrEmpty(FilterDate))
            {
                // If FilterDate contains a range (e.g., "From 2023-01-01 to 2023-01-07")
                if (FilterDate.Contains("to"))
                {
                    var dateRange = FilterDate.Split(" to ");
                    var startDate = DateTime.Parse(dateRange[0]);
                    var endDate = DateTime.Parse(dateRange[1]);

                    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
                }
                else
                {
                    // Filter by a single date (e.g., current day)
                    var date = DateTime.Parse(FilterDate);
                    filteredTransactions = filteredTransactions.Where(t => t.Date == date.Date);
                }
            }

            if (!string.IsNullOrEmpty(FilterCategory))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Category.Name == FilterCategory);
            }

            if (!string.IsNullOrEmpty(FilterType))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Type.Equals(FilterType, StringComparison.OrdinalIgnoreCase));
            }

            //var transactionCount = await filteredTransactions.CountAsync(); // Await the async method to get the count

            //if (transactionCount > 0) // Now you can compare the count
            {
                // Execute the query asynchronously and update the Transactions list
                var transactionList = await filteredTransactions.ToListAsync();
                allTransactions = new ObservableCollection<Transaction>(transactionList);
                // Update the ObservableCollection with the filtered results
                Transactions = new ObservableCollection<Transaction>(transactionList);

                CalculateBalances();
                OnPropertyChanged(nameof(Transactions));
            }
            LoadTransactionsAndSetGrid(Transactions);
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
