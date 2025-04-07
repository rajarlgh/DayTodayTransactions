using DayTodayTransactionsLibrary.Models;

namespace DayTodayTransactionsLibrary.Interfaces
{
    public interface IAccountService
    {
        Task InitializeAsync();
        Task<List<Account>> GetAccountsAsync();
        Task<Account> AddAccountAsync(Account account);
        Task DeleteAccountAsync(int? id);
        Task UpdateAccountAsync(Account account);
    }
}
