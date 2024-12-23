using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using SQLite;

namespace DayTodayTransactionsService
{
    public class TransactionService : ITransactionService
    {
        private readonly SQLiteAsyncConnection _database;

        public TransactionService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Transaction>().Wait();
        }

        public Task<List<Transaction>> GetTransactionsAsync()
        {
            return _database.Table<Transaction>().ToListAsync();
        }

        public Task AddTransactionAsync(Transaction transaction)
        {
            return _database.InsertAsync(transaction);
        }

        public Task DeleteTransactionAsync(int id)
        {
            return _database.DeleteAsync<Transaction>(id);
        }

        public Task UpdateTransactionAsync(Transaction transaction)
        {
            return _database.UpdateAsync(transaction);
        }
    }


}
