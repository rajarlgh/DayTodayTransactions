using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;
using SQLite;

namespace DayTodayTransactionsService
{
    public class AccountService : IAccountService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1); // Thread-safe initialization

        public AccountService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    await _database.CreateTableAsync<Account>();
                    _isInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Account>().ToListAsync();
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            await EnsureInitializedAsync(); // Critical to prevent hanging

            var existingAccount = _database.Table<Account>()
                                                 .Where(a => a.Name == account.Name)
                                                 .FirstOrDefaultAsync().Result;

            if (existingAccount != null)
                return existingAccount;

            var t = _database.InsertAsync(account).Result;
            return account;
        }

        public async Task DeleteAccountAsync(int? id)
        {
            await EnsureInitializedAsync();
            await _database.DeleteAsync<Account>(id);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await EnsureInitializedAsync();
            await _database.UpdateAsync(account);
        }

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<Account>();
        }
    }


}
