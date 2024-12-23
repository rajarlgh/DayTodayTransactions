using CommunityToolkit.Mvvm.ComponentModel;
using DayTodayTransactionsLibrary.Models;
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

        private async void LoadTransactions()
        {
            // Load all transactions from the database
            Transactions = await _database.Table<Transaction>().ToListAsync();
            OnPropertyChanged(nameof(Transactions));
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
            OnPropertyChanged(nameof(Transactions));
        }

    }
}
