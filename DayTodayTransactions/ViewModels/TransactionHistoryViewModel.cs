using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ObservableCollection<ChartEntry> chartEntries;
        [ObservableProperty]
        private DonutChart chart;
        [ObservableProperty]
        private bool doVisibleChart  = false;
        [ObservableProperty]
        private bool doVisibleChartBreakUp = true;

        private readonly SQLiteAsyncConnection _database;

        public TransactionHistoryViewModel(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            chartEntries = new ObservableCollection<ChartEntry>
            {
                new ChartEntry(212) { Label = "Car", ValueLabel = "212", Color = SKColor.Parse("#00FF00") },
                new ChartEntry(248) { Label = "Food", ValueLabel = "248", Color = SKColor.Parse("#FF5733") },
                new ChartEntry(128) { Label = "Pet", ValueLabel = "128", Color = SKColor.Parse("#3498DB") },
                new ChartEntry(514) { Label = "Health", ValueLabel = "514", Color = SKColor.Parse("#9B59B6") },
                new ChartEntry(300) { Label = "Cafe", ValueLabel = "300", Color = SKColor.Parse("#1ABC9C") },
                new ChartEntry(450) { Label = "Bar", ValueLabel = "450", Color = SKColor.Parse("#F1C40F") },
                new ChartEntry(380) { Label = "Dental", ValueLabel = "380", Color = SKColor.Parse("#E74C3C") },
                new ChartEntry(420) { Label = "Home", ValueLabel = "420", Color = SKColor.Parse("#34495E") },
                new ChartEntry(310) { Label = "Mobile", ValueLabel = "310", Color = SKColor.Parse("#2ECC71") },
                new ChartEntry(270) { Label = "Cloths", ValueLabel = "270", Color = SKColor.Parse("#E67E22") },
                new ChartEntry(390) { Label = "Sports", ValueLabel = "390", Color = SKColor.Parse("#16A085") },
                new ChartEntry(460) { Label = "Gift", ValueLabel = "460", Color = SKColor.Parse("#8E44AD") },
                new ChartEntry(510) { Label = "Fuel", ValueLabel = "510", Color = SKColor.Parse("#BDC3C7") },
            };

            chart = new DonutChart
            {
                Entries = ChartEntries,
                LabelMode = LabelMode.LeftAndRight,
                IsAnimated = true,
                Margin = 10,
                LabelTextSize = 40
            };
            LoadTransactions();
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
        public void OnAddMoneyClicked()
        {

        }
        public List<Transaction> Transactions { get; set; }
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
        private async void LoadTransactions()
        {
            // Load all transactions from the database
            Transactions = await _database.Table<Transaction>().ToListAsync();
            CalculateBalances();
            CreateIncomeExpenseChart();
            OnPropertyChanged(nameof(Transactions));
        }

        private void CalculateBalances()
        {
            TotalIncome = Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            TotalExpenses = Transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            Balance = TotalIncome - TotalExpenses;
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Balance));
        }

        private void CreateIncomeExpenseChart()
        {
            //var entries = new[]
            //{

            //    new Microcharts.ChartEntry((float)TotalIncome)
            //    {
            //        Label = "Income",
            //        ValueLabel = TotalIncome.ToString("C"),
            //        Color = SKColor.Parse("#00FF00")
            //    },
            //    new Microcharts.ChartEntry((float)TotalExpenses)
            //    {
            //        Label = "Expenses",
            //        ValueLabel = TotalExpenses.ToString("C"),
            //        Color = SKColor.Parse("#FF0000")
            //    }
            //};
            var entries = new[]
            {
            new ChartEntry(212) { Label = "Car", ValueLabel = "212", Color = SKColor.Parse("#00FF00") }, // Green
            new ChartEntry(248) { Label = "Food", ValueLabel = "248", Color = SKColor.Parse("#FF5733") }, // Orange
            new ChartEntry(128) { Label = "Pet", ValueLabel = "128", Color = SKColor.Parse("#3498DB") }, // Blue
            new ChartEntry(514) { Label = "Health", ValueLabel = "514", Color = SKColor.Parse("#9B59B6") }, // Purple
            new ChartEntry(300) { Label = "Cafe", ValueLabel = "300", Color = SKColor.Parse("#1ABC9C") }, // Teal
            new ChartEntry(450) { Label = "Bar", ValueLabel = "450", Color = SKColor.Parse("#F1C40F") }, // Yellow
            new ChartEntry(380) { Label = "Dental", ValueLabel = "380", Color = SKColor.Parse("#E74C3C") }, // Red
            new ChartEntry(420) { Label = "Home", ValueLabel = "420", Color = SKColor.Parse("#34495E") }, // Dark Blue
            new ChartEntry(310) { Label = "Mobile", ValueLabel = "310", Color = SKColor.Parse("#2ECC71") }, // Light Green
            new ChartEntry(270) { Label = "Cloths", ValueLabel = "270", Color = SKColor.Parse("#E67E22") }, // Amber
            new ChartEntry(390) { Label = "Sports", ValueLabel = "390", Color = SKColor.Parse("#16A085") }, // Emerald Green
            new ChartEntry(460) { Label = "Gift", ValueLabel = "460", Color = SKColor.Parse("#8E44AD") }, // Violet
            new ChartEntry(510) { Label = "Fuel", ValueLabel = "510", Color = SKColor.Parse("#BDC3C7") }, // Light Gray
};
            IncomeExpenseChart = 
            new DonutChart
            {
                Entries = entries,
                LabelMode = LabelMode.RightOnly,
                Margin = 10,
                LabelTextSize = 40 // Adjust font size for better readability
            };
            OnPropertyChanged(nameof(IncomeExpenseChart));
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
            Transactions = filteredTransactions.ToListAsync().Result;
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
