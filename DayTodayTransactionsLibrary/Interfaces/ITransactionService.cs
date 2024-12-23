using DayTodayTransactionsLibrary.Models;

namespace DayTodayTransactionsLibrary.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
        Task UpdateTransactionAsync(Transaction transaction);
    }
}
