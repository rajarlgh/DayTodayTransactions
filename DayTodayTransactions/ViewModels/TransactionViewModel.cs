using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayTodayTransactionsLibrary.Interfaces;
using DayTodayTransactionsLibrary.Models;

namespace DayTodayTransactions.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        private readonly ITransactionService _transactionService;

        public TransactionViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string reason;

        [ObservableProperty]
        private string type; // Income or Expense

        [ObservableProperty]
        private string category;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [RelayCommand]
        public async Task AddTransactionAsynca()
        {
            var transaction = new Transaction
            {
                Amount = Amount,
                Reason = Reason,
                Type = Type,
                Category = Category,
                Date = Date
            };

            await _transactionService.AddTransactionAsync(transaction);
        }
    }

}
