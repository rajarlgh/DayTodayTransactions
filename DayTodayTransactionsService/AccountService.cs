using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using SQLite;

namespace DayTodayTransactionsService
{
    public class AccountService : IAccountService
    {
        private readonly SQLiteAsyncConnection _database;

        public AccountService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Account>().Wait();
        }

        public Task<List<Account>> GetAccountsAsync()
        {
            return _database.Table<Account>().ToListAsync();
        }

        public Task AddAccountAsync(Account account)
        {
            return _database.InsertAsync(account);
        }

        public Task DeleteAccountAsync(int id)
        {
            return _database.DeleteAsync<Account>(id);
        }

        public Task UpdateAccountAsync(Account account)
        {
            return _database.UpdateAsync(account);
        }
    }
}
