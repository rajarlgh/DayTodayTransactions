using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public TransactionHistoryViewModel(string dbPath, ITransactionService transactionService)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            var transactions = transactionService.GetTransactionsAsync().Result;
            var t = transactions.ToList<Transaction>();
            //  var t= transactionService.GetTransactionsAsync();
            LoadTransactionsAndSetGrid(transactions);
        }
        public void ShowBreakdownForCategory(string category)
        {
            var breakdown = transactions.Where(t => t.Type == "Income" && t.Category == category)
                .ToList();

            SelectedCategoryBreakdown = new ObservableCollection<Transaction>(breakdown);
        }
        private void LoadTransactionsAndSetGrid(IList<Transaction> transactions)
        {
            // Group transactions by category and calculate the total amount for each category
            var groupedData = transactions
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                });

            var incomeGroupedData = transactions.Where(r=> r.Type=="Income")
               .GroupBy(t => t.Category)
               .Select(g => new
               {
                   Category = g.Key,
                   TotalAmount = g.Sum(t => t.Amount)
               });

            // Map the grouped data to ChartEntry objects
            IncomeChartEntries = new ObservableCollection<ChartEntry>(
                incomeGroupedData.Select(data => new ChartEntry((float)data.TotalAmount)
                {
                    Label = data.Category,
                    ValueLabel = data.TotalAmount.ToString("F0"), // Format as integer
                    Color = GetCategoryColor(data.Category) // Assign a color based on the category
                })
            );

            var expenseGroupedData = transactions.Where(r => r.Type == "Expense")
              .GroupBy(t => t.Category)
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
            //LoadTransactions();

            // Recreate the chart instance with new entries
            IncomeChart = new DonutChart
            {
                Entries = IncomeChartEntries,
                LabelMode = LabelMode.LeftAndRight,
                IsAnimated = true,
                Margin = 10,
                LabelTextSize = 40
            };

            // Recreate the chart instance with new entries
            ExpenseChart = new DonutChart
            {
                Entries = ExpenseChartEntries,
                LabelMode = LabelMode.LeftAndRight,
                IsAnimated = true,
                Margin = 10,
                LabelTextSize = 40
            };

            //Chart.Entries = ChartEntries;
            //Chart.PropertyChanged += Chart_PropertyChanged;

            //Chart.LabelColor = SkiaSharp.SKColor.Parse("FFCCBB");
        }

        public async Task RefreshDataAsync()
        {
            // Fetch the latest transactions
            var transactionsList = await _database.Table<Transaction>().ToListAsync();
            Transactions = new ObservableCollection<Transaction>(transactionsList);
            //foreach (var transaction in newTransactions)
            //{
            //    if (Transactions == null)
            //        Transactions = new ObservableCollection<Transaction>();
            //    Transactions.Add(transaction);
            //}
            LoadTransactionsAndSetGrid(Transactions);
            CalculateBalances();
            // Notify UI of changes
            OnPropertyChanged(nameof(Transactions));
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Balance));
            OnPropertyChanged(nameof(IncomeExpenseChart));
            OnPropertyChanged(nameof(IncomeChart));
            OnPropertyChanged(nameof(IncomeChartEntries));
            OnPropertyChanged(nameof(ExpenseChart));
            OnPropertyChanged(nameof(ExpenseChartEntries));
        }

        /// <summary>
        /// Assigns a unique color to each category.
        /// </summary>
        private SKColor GetCategoryColor(string category)
        {
            return category switch
            {
                "Car" => SKColor.Parse("#00FF00"),
                "Food" => SKColor.Parse("#FF5733"),
                "Pet" => SKColor.Parse("#3498DB"),
                "Health" => SKColor.Parse("#9B59B6"),
                "Cafe" => SKColor.Parse("#1ABC9C"),
                "Bar" => SKColor.Parse("#F1C40F"),
                "Dental" => SKColor.Parse("#E74C3C"),
                "Home" => SKColor.Parse("#34495E"),
                "Mobile" => SKColor.Parse("#2ECC71"),
                "Cloths" => SKColor.Parse("#E67E22"),
                "Sports" => SKColor.Parse("#16A085"),
                "Gift" => SKColor.Parse("#8E44AD"),
                "Fuel" => SKColor.Parse("#BDC3C7"),
                _ => SKColor.Parse("#95A5A6") // Default color
            };
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

        [ObservableProperty]
        private ObservableCollection<Transaction> transactions;

        public string FilterDate { get; set; }
        public string FilterCategory { get; set; }
        public string FilterType { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }

        public Chart IncomeExpenseChart { get; set; }

        [RelayCommand]
        public void ShowContent()
        {
            doVisibleChart = !this.doVisibleChart;

            doVisibleChartBreakUp = !this.doVisibleChartBreakUp;
            OnPropertyChanged(nameof(DoVisibleChartBreakUp));
            OnPropertyChanged(nameof(DoVisibleChart));

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
                filteredTransactions = filteredTransactions.Where(t => t.Category.Contains(FilterCategory));
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
