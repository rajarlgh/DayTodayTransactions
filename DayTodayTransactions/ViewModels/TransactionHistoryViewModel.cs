using CommunityToolkit.Mvvm.ComponentModel;
using DayTodayTransactionsLibrary.Models;
using Microcharts;
using SkiaSharp;
using SQLite;

namespace DayTodayTransactions.ViewModels
{
    public class TransactionHistoryViewModel : ObservableObject
    {
        private readonly SQLiteAsyncConnection _database;

        public TransactionHistoryViewModel(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            LoadTransactions();
        }

        public List<Transaction> Transactions { get; set; }
        public string FilterDate { get; set; }
        public string FilterCategory { get; set; }
        public string FilterType { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }

        public Chart IncomeExpenseChart { get; set; }

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
            var entries = new[]
            {
                
                new Microcharts.ChartEntry((float)TotalIncome)
                {
                    Label = "Income",
                    ValueLabel = TotalIncome.ToString("C"),
                    Color = SKColor.Parse("#00FF00")
                },
                new Microcharts.ChartEntry((float)TotalExpenses)
                {
                    Label = "Expenses",
                    ValueLabel = TotalExpenses.ToString("C"),
                    Color = SKColor.Parse("#FF0000")
                }
            };

            IncomeExpenseChart = new DonutChart { Entries = entries };
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
    }
}
