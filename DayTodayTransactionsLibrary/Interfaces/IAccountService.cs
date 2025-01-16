using DayTodayTransactionsLibrary.Models;

namespace DayTodayTransactionsLibrary.Interfaces
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsAsync();
        Task AddAccountAsync(Account account);
        Task DeleteAccountAsync(int? id);
        Task UpdateAccountAsync(Account account);
    }
}
