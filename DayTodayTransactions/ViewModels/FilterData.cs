namespace DayTodayTransactions.ViewModels
{
    internal class FilterData
    {
        public async void FilterTransactionsByRange(DateTime startDate, DateTime endDate)
        {
            //var filteredTransactions = _database.Table<Transaction>();
            //var t = filteredTransactions.ToListAsync().Result;
            //// Get record counts grouped by month from the database
            //// var dbMonthlyCounts = await GetRecordCountsByMonthFromDatabaseAsync();

            //if (selectedAccount != null && selectedAccount.Id > 0)
            //    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate && t.AccountId == selectedAccount.Id);
            //else
            //    filteredTransactions = filteredTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
            //var data = filteredTransactions.ToListAsync().Result;
            //allTransactions = new ObservableCollection<Transaction>(data);

            //// Execute the query and update the Transactions list
            //Transactions = new ObservableCollection<Transaction>(data);

            //CalculateBalances();
            //OnPropertyChanged(nameof(Transactions));
            //this.LoadTransactionsAndSetGrid(Transactions);

        }
    }
}
